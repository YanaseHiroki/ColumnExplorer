using System.IO;
using System.Windows.Controls;
using ColumnExplorer.Helpers;

namespace ColumnExplorer.Helpers
{
    public static class ContentLoader
    {
        /// <summary>
        /// ドライブのリストをListBoxに追加します。
        /// </summary>
        /// <param name="listBox">ドライブを追加するListBox。</param>
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
        /// 指定されたディレクトリのサブディレクトリとファイルをListBoxに追加します。
        /// </summary>
        /// <param name="listBox">サブディレクトリとファイルを追加するListBox。</param>
        /// <param name="directoryPath">ディレクトリのパス。</param>
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
