using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ImageMagick;

namespace ColumnExplorer.Previewers
{
    /// <summary>
    /// Helper class for displaying image file previews.
    /// </summary>
    public static class ImageFilePreviewer
    {
        /// <summary>
        /// Displays the content of the specified image file in the Image control.
        /// </summary>
        /// <param name="imageControl">The Image control to display the content.</param>
        /// <param name="filePath">The path of the image file to load.</param>
        public static void PreviewImageFile(Image imageControl, string filePath)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    BitmapImage bitmap = null;

                    var extension = Path.GetExtension(filePath).ToLower();
                    if (extension == ".heic" || extension == ".webp" || extension == ".svg" || extension == ".pdf" || extension == ".psd" || extension == ".ico" || extension == ".tga" || extension == ".exr" || extension == ".hdr" || extension == ".pnm" || extension == ".xbm" || extension == ".xpm" || extension == ".dng" || extension == ".cr2" || extension == ".jp2" || extension == ".djvu" || extension == ".eps")
                    {
                        using (var magickImage = new MagickImage(filePath))
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                magickImage.Write(memoryStream, MagickFormat.Bmp);
                                memoryStream.Position = 0;
                                bitmap = new BitmapImage();
                                bitmap.BeginInit();
                                bitmap.StreamSource = memoryStream;
                                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                                bitmap.EndInit();
                            }
                        }
                    }
                    else
                    {
                        bitmap = new BitmapImage();
                        bitmap.BeginInit();
                        bitmap.UriSource = new Uri(filePath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                    }

                    imageControl.Source = bitmap;
                }
                catch (IOException ex)
                {
                    // Add necessary error handling, such as displaying an error message
                    imageControl.Source = null;
                }
            }
        }
    }
}
