using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace ColumnExplorer.Previewers
{
    /// <summary>
    /// Helper class for displaying PDF file previews.
    /// </summary>
    public static class PdfFilePreviewer
    {
        /// <summary>
        /// Loads the specified PDF file and displays a preview in the ListBox.
        /// </summary>
        /// <param name="listBox">The ListBox to display the preview.</param>
        /// <param name="filePath">The path of the PDF file to preview.</param>
        public static void PreviewPdfFile(ListBox listBox, string filePath)
        {
            listBox.Items.Clear();
            if (File.Exists(filePath))
            {
                try
                {
                    var lines = ReadLinesWithTimeout(filePath, TimeSpan.FromSeconds(5), 1024 * 1024); // Set timeout to 5 seconds
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
        /// Loads the specified PDF file and extracts text considering timeout and maximum bytes.
        /// </summary>
        /// <param name="filePath">The path of the PDF file to load.</param>
        /// <param name="timeout">The timeout duration.</param>
        /// <param name="maxBytes">The maximum number of bytes.</param>
        /// <returns>An enumerable of extracted text.</returns>
        private static IEnumerable<string> ReadLinesWithTimeout(string filePath, TimeSpan timeout, int maxBytes)
        {
            var lines = new List<string>();
            var cts = new CancellationTokenSource(timeout);
            var token = cts.Token;

            try
            {
                using (var pdfDocument = new PdfDocument(new PdfReader(filePath)))
                {
                    var bytesRead = 0;

                    for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
                    {
                        token.ThrowIfCancellationRequested();

                        var text = ExtractTextFromPage(pdfDocument.GetPage(i));
                        bytesRead += System.Text.Encoding.UTF8.GetByteCount(text);

                        if (bytesRead > maxBytes)
                            break;

                        lines.Add(text);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                lines.Add("Operation was canceled due to timeout.");
            }
            catch (Exception ex)
            {
                lines.Add($"Error extracting text: {ex.Message}");
            }

            return lines;
        }

        /// <summary>
        /// Extracts text from the specified PDF page.
        /// </summary>
        /// <param name="pdfPage">The PDF page to extract text from.</param>
        /// <returns>The extracted text.</returns>
        private static string ExtractTextFromPage(PdfPage pdfPage)
        {
            var text = PdfTextExtractor.GetTextFromPage(pdfPage);

            // Output the extracted text to the console for debugging
            Console.WriteLine($"Extracted text from page: {text}");

            return text;
        }
    }
}
