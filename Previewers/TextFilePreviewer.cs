using System.IO;
using System.Text;
using System.Windows.Controls;

namespace ColumnExplorer.Previewers
{
    /// <summary>
    /// Helper class for displaying text file previews.
    /// </summary>
    public static class TextFilePreviewer
    {
        static TextFilePreviewer()
        {
            // Register the code pages encoding provider to support more encodings
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        }

        public static void PreviewTextFile(ListBox listBox, string filePath)
        {
            listBox.Items.Clear();
            if (File.Exists(filePath))
            {
                try
                {
                    var encoding = DetectEncoding(filePath);
                    var lines = ReadLinesWithTimeout(filePath, encoding, TimeSpan.FromSeconds(1), 1024 * 1024); // Timeout after 1 second or 1MB
                    foreach (var line in lines)
                    {
                        listBox.Items.Add(new ListBoxItem { Content = line });
                    }
                }
                catch (IOException ex)
                {
                    listBox.Items.Add(new ListBoxItem
                    {
                        Content = $"Error reading file: {ex.Message}",
                        Foreground = System.Windows.Media.Brushes.Red
                    });
                }
            }
        }

        private static Encoding DetectEncoding(string filePath)
        {
            using (var reader = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var bom = new byte[4];
                reader.Read(bom, 0, 4);

                if (bom[0] == 0xEF && bom[1] == 0xBB && bom[2] == 0xBF)
                    return Encoding.UTF8;

                if (bom[0] == 0xFF && bom[1] == 0xFE)
                    return Encoding.Unicode;

                if (bom[0] == 0xFE && bom[1] == 0xFF)
                    return Encoding.BigEndianUnicode;

                if (bom[0] == 0xFF && bom[1] == 0xFE && bom[2] == 0x00 && bom[3] == 0x00)
                    return Encoding.UTF32;

                if (bom[0] == 0x00 && bom[1] == 0x00 && bom[2] == 0xFE && bom[3] == 0xFF)
                    return new UTF32Encoding(true, true);

                // Additional encodings
                var encodings = new[]
                {
                            Encoding.GetEncoding("shift-jis"),
                            Encoding.GetEncoding("iso-2022-jp"),
                            Encoding.GetEncoding("euc-jp"),
                            Encoding.GetEncoding("gb2312"),
                            Encoding.GetEncoding("big5"),
                            Encoding.GetEncoding("iso-8859-1"),
                            Encoding.GetEncoding("windows-1252")
                        };

                foreach (var encoding in encodings)
                {
                    reader.Seek(0, SeekOrigin.Begin);
                    var buffer = new byte[reader.Length];
                    reader.Read(buffer, 0, buffer.Length);
                    var preamble = encoding.GetPreamble();
                    if (buffer.Length >= preamble.Length && buffer.Take(preamble.Length).SequenceEqual(preamble))
                    {
                        return encoding;
                    }
                }

                return Encoding.Default;
            }
        }

        private static IEnumerable<string> ReadLinesWithTimeout(string filePath, Encoding encoding, TimeSpan timeout, int maxBytes)
        {
            var lines = new List<string>();
            var cts = new CancellationTokenSource(timeout);
            var token = cts.Token;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var reader = new StreamReader(fs, encoding))
            {
                var buffer = new char[1024];
                var bytesRead = 0;

                while (!reader.EndOfStream && bytesRead < maxBytes)
                {
                    token.ThrowIfCancellationRequested();

                    var charsRead = reader.Read(buffer, 0, buffer.Length);
                    bytesRead += encoding.GetByteCount(buffer, 0, charsRead);

                    var chunk = new string(buffer, 0, charsRead);
                    lines.AddRange(chunk.Split(new[] { Environment.NewLine }, StringSplitOptions.None));
                }
            }

            return lines;
        }
    }
}
