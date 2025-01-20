using System;
using System.IO;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using DocumentFormat.OpenXml.Packaging;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media;
using WpfImage = System.Windows.Controls.Image;
using DrawingImage = System.Drawing.Image;
using DrawingBrushes = System.Drawing.Brushes;
using DrawingPen = System.Drawing.Pen;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Drawing;
using DrawingShape = DocumentFormat.OpenXml.Drawing.Shape;
using PresentationShape = DocumentFormat.OpenXml.Presentation.Shape;
using static System.Drawing.Font;
using System.Diagnostics;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// PowerPoint�t�@�C���̃v���r���[��\�����邽�߂̃w���p�[�N���X�B
    /// </summary>
    public static class PowerPointFilePreviewer
    {
        public static void PreviewPowerPointFile(StackPanel stackPanel, string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    var bitmaps = GetSlidesAsBitmaps(filePath, TimeSpan.FromSeconds(10), 500 * 1024 * 1024); // 10�b��500MB�̐���
                    if (bitmaps != null && bitmaps.Any())
                    {
                        stackPanel.Children.Clear();
                        foreach (var bitmap in bitmaps)
                        {
                            var bitmapImage = ConvertBitmapToBitmapImage(bitmap);
                            var imageControl = new WpfImage
                            {
                                Source = bitmapImage,
                                Width = bitmapImage.Width,
                                Height = bitmapImage.Height,
                                Margin = new System.Windows.Thickness(5)
                            };
                            stackPanel.Children.Add(imageControl);
                        }
                    }
                    else
                    {
                        stackPanel.Children.Clear();
                    }
                }
                catch (IOException ex)
                {
                    // �G���[���b�Z�[�W���E�J�����ɕ\��
                    var errorMessage = new TextBlock
                    {
                        Text = $"�G���[: {ex.Message}",
                        Foreground = new SolidColorBrush(Colors.Red),
                        Margin = new System.Windows.Thickness(5)
                    };
                    stackPanel.Children.Add(errorMessage);

                }
            }
        }

        public static List<Bitmap> GetSlidesAsBitmaps(string filePath, TimeSpan timeLimit, long memoryLimit)
        {
            var bitmaps = new List<Bitmap>();
            var stopwatch = Stopwatch.StartNew();
            long memoryUsed = 0;

            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            using (var pptDoc = PresentationDocument.Open(stream, false))
            {
                foreach (var slidePart in pptDoc.PresentationPart.SlideParts)
                {
                    if (stopwatch.Elapsed > timeLimit || memoryUsed > memoryLimit)
                    {
                        break;
                    }

                    var slide = slidePart.Slide;
                    var bitmap = new Bitmap(960, 540); // �X���C�h�̃T�C�Y�ɍ��킹�Ē���
                    using (var graphics = Graphics.FromImage(bitmap))
                    {
                        graphics.Clear(System.Drawing.Color.White);

                        // �X���C�h�̃e�L�X�g���e��`�悷�鏈��
                        foreach (var shape in slide.Descendants<PresentationShape>())
                        {
                            var textBody = shape.TextBody;
                            if (textBody != null)
                            {
                                var transform = shape.ShapeProperties.Transform2D;
                                var offsetX = transform.Offset.X;
                                var offsetY = transform.Offset.Y;
                                var width = transform.Extents.Cx;
                                var height = transform.Extents.Cy;

                                foreach (var paragraph in textBody.Descendants<DocumentFormat.OpenXml.Drawing.Paragraph>())
                                {
                                    foreach (var text in paragraph.Descendants<DocumentFormat.OpenXml.Drawing.Text>())
                                    {
                                        graphics.DrawString(text.Text, new System.Drawing.Font("Arial", 20), DrawingBrushes.Black, new RectangleF(offsetX / 12700f, offsetY / 12700f, width / 12700f, height / 12700f));
                                    }
                                }
                            }
                        }

                        // �X���C�h�̉摜���e��`�悷�鏈��
                        foreach (var picture in slide.Descendants<DocumentFormat.OpenXml.Presentation.Picture>())
                        {
                            var blip = picture.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().FirstOrDefault();
                            if (blip != null)
                            {
                                var embed = blip.Embed.Value;
                                var imagePart = (ImagePart)slidePart.GetPartById(embed);
                                using (var imageStream = imagePart.GetStream())
                                {
                                    var image = DrawingImage.FromStream(imageStream);
                                    var transform = picture.ShapeProperties.Transform2D;
                                    var offsetX = transform.Offset.X;
                                    var offsetY = transform.Offset.Y;
                                    var width = transform.Extents.Cx;
                                    var height = transform.Extents.Cy;

                                    graphics.DrawImage(image, new RectangleF(offsetX / 12700f, offsetY / 12700f, width / 12700f, height / 12700f));
                                }
                            }
                        }

                        // �X���C�h�̐}�`���e��`�悷�鏈��
                        foreach (var graphicFrame in slide.Descendants<DocumentFormat.OpenXml.Presentation.GraphicFrame>())
                        {
                            var transform = graphicFrame.Transform;
                            var offsetX = transform.Offset.X;
                            var offsetY = transform.Offset.Y;
                            var width = transform.Extents.Cx;
                            var height = transform.Extents.Cy;

                            using (var pen = new DrawingPen(System.Drawing.Color.Black))
                            {
                                graphics.DrawRectangle(pen, new RectangleF(offsetX / 12700f, offsetY / 12700f, width / 12700f, height / 12700f));
                            }
                        }
                    }

                    bitmaps.Add(bitmap);
                    memoryUsed += bitmap.Width * bitmap.Height * 4; // �����悻�̃������g�p�ʂ��v�Z
                }
            }

            return bitmaps;
        }

        public static BitmapImage ConvertBitmapToBitmapImage(Bitmap bitmap)
        {
            using (var memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                memoryStream.Position = 0;

                var bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze(); // �}���`�X���b�h���ł̎g�p���\�ɂ��邽�߂Ƀt���[�Y

                return bitmapImage;
            }
        }
    }
}