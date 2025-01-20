using System.IO;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using ImageMagick;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// �摜�t�@�C���̃v���r���[��\�����邽�߂̃w���p�[�N���X�B
    /// </summary>
    public static class ImageFilePreviewer
    {
        /// <summary>
        /// �w�肳�ꂽ�摜�t�@�C���̓��e��Image�R���g���[���ɕ\�����܂��B
        /// </summary>
        /// <param name="imageControl">���e��\������Image�R���g���[���B</param>
        /// <param name="filePath">�ǂݍ��މ摜�t�@�C���̃p�X�B</param>
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
                    // �G���[���b�Z�[�W�̕\���ȂǁA�K�v�ȃG���[�n���h�����O��ǉ�
                    imageControl.Source = null;
                }
            }
        }
    }
}
