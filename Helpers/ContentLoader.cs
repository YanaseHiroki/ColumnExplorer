using System.IO;
using System.Windows.Controls;
using ColumnExplorer.Helpers;

namespace ColumnExplorer.Helpers
{
    public static class ContentLoader
    {
        /// <summary>
        /// �h���C�u�̃��X�g��ListBox�ɒǉ����܂��B
        /// </summary>
        /// <param name="listBox">�h���C�u��ǉ�����ListBox�B</param>
        public static void AddDrives(ListBox listBox)
        {
            listBox.Items.Clear();
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    listBox.Items.Add(DirectoryHelper.CreateListBoxItem(drive.Name, drive.Name, true));
                }
            }
        }

        /// <summary>
        /// �w�肳�ꂽ�f�B���N�g���̃T�u�f�B���N�g���ƃt�@�C����ListBox�ɒǉ����܂��B
        /// </summary>
        /// <param name="listBox">�T�u�f�B���N�g���ƃt�@�C����ǉ�����ListBox�B</param>
        /// <param name="directoryPath">�f�B���N�g���̃p�X�B</param>
        public static void loadItems(ListBox listBox, string? directoryPath)
        {
            listBox.Items.Clear();
            if (directoryPath != null)
                {
                foreach (var dir in Directory.GetDirectories(directoryPath))
                {
                    listBox.Items.Add(DirectoryHelper.CreateListBoxItem(Path.GetFileName(dir), dir, true));
                }
                foreach (var file in Directory.GetFiles(directoryPath))
                {
                    listBox.Items.Add(DirectoryHelper.CreateListBoxItem(Path.GetFileName(file), file, false));
                }
            }
        }
    }
}
