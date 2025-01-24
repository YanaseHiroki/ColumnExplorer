using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// PDF�t�@�C���̃v���r���[��\�����邽�߂̃w���p�[�N���X�B
    /// </summary>
    public static class PdfFilePreviewer
    {
        /// <summary>
        /// �w�肳�ꂽPDF�t�@�C����ǂݍ��݁A���X�g�{�b�N�X�Ƀv���r���[��\�����܂��B
        /// </summary>
        /// <param name="listBox">�v���r���[��\�����郊�X�g�{�b�N�X�B</param>
        /// <param name="filePath">�v���r���[����PDF�t�@�C���̃p�X�B</param>
        public static void PreviewPdfFile(ListBox listBox, string filePath)
        {
            listBox.Items.Clear();
            if (File.Exists(filePath))
            {
                try
                {
                    var lines = ReadLinesWithTimeout(filePath, TimeSpan.FromSeconds(5), 1024 * 1024); // �^�C���A�E�g��5�b�ɐݒ�
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
        /// �w�肳�ꂽPDF�t�@�C����ǂݍ��݁A�^�C���A�E�g�ƍő�o�C�g�����l�����ăe�L�X�g�𒊏o���܂��B
        /// </summary>
        /// <param name="filePath">�ǂݍ���PDF�t�@�C���̃p�X�B</param>
        /// <param name="timeout">�^�C���A�E�g�̎��ԁB</param>
        /// <param name="maxBytes">�ő�o�C�g���B</param>
        /// <returns>���o���ꂽ�e�L�X�g�̗񋓁B</returns>
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
        /// �w�肳�ꂽPDF�y�[�W����e�L�X�g�𒊏o���܂��B
        /// </summary>
        /// <param name="pdfPage">�e�L�X�g�𒊏o����PDF�y�[�W�B</param>
        /// <returns>���o���ꂽ�e�L�X�g�B</returns>
        private static string ExtractTextFromPage(PdfPage pdfPage)
        {
            var text = PdfTextExtractor.GetTextFromPage(pdfPage);

            // �f�o�b�O�p�ɒ��o���ꂽ�e�L�X�g���R���\�[���ɏo��
            Console.WriteLine($"Extracted text from page: {text}");

            return text;
        }
    }
}
