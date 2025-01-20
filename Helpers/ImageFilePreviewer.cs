using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ImageMagick;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// 画像ファイルのプレビューを表示するためのヘルパークラス。
    /// </summary>
    public static class ImageFilePreviewer
    {
        /// <summary>
        /// 指定された画像ファイルの内容をImageコントロールに表示します。
        /// </summary>
        /// <param name="imageControl">内容を表示するImageコントロール。</param>
        /// <param name="filePath">読み込む画像ファイルのパス。</param>
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
                        bitmap.UriSource = new System.Uri(filePath);
                        bitmap.CacheOption = BitmapCacheOption.OnLoad;
                        bitmap.EndInit();
                    }

                    imageControl.Source = bitmap;
                }
                catch (IOException ex)
                {
                    // エラーメッセージの表示など、必要なエラーハンドリングを追加
                    imageControl.Source = null;
                }
            }
        }
    }
}
