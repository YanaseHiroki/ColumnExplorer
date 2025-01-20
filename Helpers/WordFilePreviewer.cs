using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using DocumentFormat.OpenXml.Packaging;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// Word�t�@�C���̃v���r���[��\�����邽�߂̃w���p�[�N���X�B
    /// </summary>
    public static class WordFilePreviewer
    {
        public static void PreviewWordFile(ListBox listBox, string filePath)
        {
            listBox.Items.Clear();
            if (File.Exists(filePath))
            {
                try
                {
                    var lines = ReadLinesWithTimeout(filePath, TimeSpan.FromSeconds(5), 1024 * 1024); // Increase timeout to 5 seconds
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

        private static IEnumerable<string> ReadLinesWithTimeout(string filePath, TimeSpan timeout, int maxBytes)
        {
            var lines = new List<string>();
            var cts = new CancellationTokenSource(timeout);
            var token = cts.Token;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var wordDoc = WordprocessingDocument.Open(stream, false))
            {
                var body = wordDoc.MainDocumentPart.Document.Body;
                var bytesRead = 0;

                foreach (var text in body.Descendants<DocumentFormat.OpenXml.Wordprocessing.Text>())
                {
                    token.ThrowIfCancellationRequested();

                    var chunk = text.Text;
                    bytesRead += System.Text.Encoding.UTF8.GetByteCount(chunk);

                    if (bytesRead > maxBytes)
                        break;

                    lines.Add(chunk);
                }
            }

            return lines;
        }
    }
}