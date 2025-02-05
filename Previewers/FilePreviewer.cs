using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ColumnExplorer.Previewers
{
    public static class FilePreviewer
    {
        public static void PreviewFile(ListBox rightColumn, string rightColumnPath)
        {
            var extension = Path.GetExtension(rightColumnPath).ToLower();
            if (extension == ".txt")
            {
                TextFilePreviewer.PreviewTextFile(rightColumn, rightColumnPath);
            }
            else if (extension == ".docx")
            {
                WordFilePreviewer.PreviewWordFile(rightColumn, rightColumnPath);
            }
            else if (extension == ".pptx")
            {
                var stackPanel = new StackPanel();
                var slides = PowerPointFilePreviewer.GetSlidesAsBitmaps(rightColumnPath, TimeSpan.FromMinutes(1), 100 * 1024 * 1024); // 100MB memory limit
                rightColumn.Items.Clear();
                foreach (var slide in slides)
                {
                    var image = new Image
                    {
                        Source = PowerPointFilePreviewer.ConvertBitmapToBitmapImage(slide),
                        Margin = new Thickness(5)
                    };
                    stackPanel.Children.Add(image);
                }
                rightColumn.Items.Add(new ListBoxItem { Content = stackPanel });
            }
            else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp" || extension == ".gif" || extension == ".heic" || extension == ".webp")
            {
                var imageControl = new Image();
                ImageFilePreviewer.PreviewImageFile(imageControl, rightColumnPath);
                rightColumn.Items.Clear();
                rightColumn.Items.Add(new ListBoxItem { Content = imageControl });
            }
            else if (extension == ".pdf")
            {
                PdfFilePreviewer.PreviewPdfFile(rightColumn, rightColumnPath);
            }
            else
            {
                if (rightColumnPath != null && File.Exists(rightColumnPath))
                {
                    UnsupportedFilePreviewer.PreviewUnsupportedFile(rightColumn, rightColumnPath);
                }
            }
        }
    }
}
