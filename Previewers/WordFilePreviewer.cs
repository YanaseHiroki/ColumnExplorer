using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using DocumentFormat.OpenXml.Packaging;

namespace ColumnExplorer.Previewers
{
    /// <summary>
    /// Helper class for displaying Word file previews.
    /// </summary>
    public static class WordFilePreviewer
    {
        /// <summary>
        /// Loads the specified Word file and displays a preview in the ListBox.
        /// </summary>
        /// <param name="listBox">The ListBox to display the preview.</param>
        /// <param name="filePath">The path of the Word file to preview.</param>
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

        /// <summary>
        /// Reads lines from the specified Word file with a timeout and maximum byte limit.
        /// </summary>
        /// <param name="filePath">The path of the Word file to read.</param>
        /// <param name="timeout">The timeout duration.</param>
        /// <param name="maxBytes">The maximum number of bytes to read.</param>
        /// <returns>An enumerable of extracted text lines.</returns>
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
