using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// �f�B���N�g������Ɋւ���w���p�[���\�b�h��񋟂���ÓI�N���X�B
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// �w�肳�ꂽ�p�����[�^�Ɋ�Â���ListBoxItem���쐬���܂��B
        /// </summary>
        /// <param name="displayName">�\�����B</param>
        /// <param name="path">�A�C�e���̃p�X�B</param>
        /// <param name="isDirectory">�f�B���N�g�����ǂ����B</param>
        /// <returns>�쐬���ꂽListBoxItem�B</returns>
        public static ListBoxItem CreateListBoxItem(string displayName, string path, bool isDirectory)
        {
            return new ListBoxItem
            {
                Content = displayName,
                Tag = path,
                FontWeight = isDirectory ? FontWeights.Bold : FontWeights.Normal,
                Foreground = isDirectory ? Brushes.Blue : Brushes.Black
            };
        }

        /// <summary>
        /// �w�肳�ꂽ�f�B���N�g���̓��e��ListBox�ɓǂݍ��݂܂��B
        /// </summary>
        /// <param name="listBox">���e��\������ListBox�B</param>
        /// <param name="path">�ǂݍ��ރf�B���N�g���̃p�X�B</param>
        /// <param name="isDirectory">�f�B���N�g�����ǂ����B</param>
        public static void LoadDirectoryContent(ListBox listBox, string path)
        {
            listBox.Items.Clear();
            if (Directory.Exists(path))
            {
                try
                {
                    foreach (var dir in Directory.GetDirectories(path))
                    {
                        listBox.Items.Add(CreateListBoxItem(Path.GetFileName(dir), dir, true));
                    }

                    foreach (var file in Directory.GetFiles(path))
                    {
                        listBox.Items.Add(CreateListBoxItem(Path.GetFileName(file), file, false));
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    listBox.Items.Add(new ListBoxItem
                    {
                        Content = ErrorMessages.Unauthorized.GetMessage(),
                        Foreground = Brushes.Red
                    });
                }
            }
        }
    }
}
