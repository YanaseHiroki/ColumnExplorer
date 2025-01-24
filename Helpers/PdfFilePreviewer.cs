using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// PDFファイルのプレビューを表示するためのヘルパークラス。
    /// </summary>
    public static class PdfFilePreviewer
    {
        /// <summary>
        /// 指定されたPDFファイルを読み込み、リストボックスにプレビューを表示します。
        /// </summary>
        /// <param name="listBox">プレビューを表示するリストボックス。</param>
        /// <param name="filePath">プレビューするPDFファイルのパス。</param>
        public static void PreviewPdfFile(ListBox listBox, string filePath)
        {
            listBox.Items.Clear();
            if (File.Exists(filePath))
            {
                try
                {
                    var lines = ReadLinesWithTimeout(filePath, TimeSpan.FromSeconds(5), 1024 * 1024); // タイムアウトを5秒に設定
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
        /// 指定されたPDFファイルを読み込み、タイムアウトと最大バイト数を考慮してテキストを抽出します。
        /// </summary>
        /// <param name="filePath">読み込むPDFファイルのパス。</param>
        /// <param name="timeout">タイムアウトの時間。</param>
        /// <param name="maxBytes">最大バイト数。</param>
        /// <returns>抽出されたテキストの列挙。</returns>
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
        /// 指定されたPDFページからテキストを抽出します。
        /// </summary>
        /// <param name="pdfPage">テキストを抽出するPDFページ。</param>
        /// <returns>抽出されたテキスト。</returns>
        private static string ExtractTextFromPage(PdfPage pdfPage)
        {
            var text = PdfTextExtractor.GetTextFromPage(pdfPage);

            // デバッグ用に抽出されたテキストをコンソールに出力
            Console.WriteLine($"Extracted text from page: {text}");

            return text;
        }
    }
}
