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
            foreach (var drive in DriveInfo.GetDrives())
            {
                if (drive.IsReady)
                {
                    listBox.Items.Add(DirectoryHelper.CreateListBoxItem(drive.Name, drive.Name, true));
                }
            }
        }

        /// <summary>
        /// 指定されたディレクトリのサブディレクトリをListBoxに追加します。
        /// </summary>
        /// <param name="listBox">サブディレクトリを追加するListBox。</param>
        /// <param name="directoryPath">ディレクトリのパス。</param>
        public static void AddDirectories(ListBox listBox, string directoryPath)
        {
            foreach (var dir in Directory.GetDirectories(directoryPath))
            {
                listBox.Items.Add(DirectoryHelper.CreateListBoxItem(Path.GetFileName(dir), dir, true));
            }
        }

        /// <summary>
        /// 指定されたディレクトリのファイルをListBoxに追加します。
        /// </summary>
        /// <param name="listBox">ファイルを追加するListBox。</param>
        /// <param name="directoryPath">ディレクトリのパス。</param>
        public static void AddFiles(ListBox listBox, string directoryPath)
        {
            foreach (var file in Directory.GetFiles(directoryPath))
            {
                listBox.Items.Add(DirectoryHelper.CreateListBoxItem(Path.GetFileName(file), file, false));
            }
        }
    }
}
