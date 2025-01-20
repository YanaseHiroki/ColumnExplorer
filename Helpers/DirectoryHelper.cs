using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// ディレクトリ操作に関するヘルパーメソッドを提供する静的クラス。
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// 指定されたパラメータに基づいてListBoxItemを作成します。
        /// </summary>
        /// <param name="displayName">表示名。</param>
        /// <param name="path">アイテムのパス。</param>
        /// <param name="isDirectory">ディレクトリかどうか。</param>
        /// <returns>作成されたListBoxItem。</returns>
        public static ListBoxItem CreateListBoxItem(string displayName, string path, bool isDirectory)
        {
            return new ListBoxItem
            {
                Content = displayName,
                Tag = path,
                Foreground = isDirectory ? Brushes.Blue : Brushes.Black
            };
        }

        /// <summary>
        /// 指定されたディレクトリの内容をListBoxに読み込みます。
        /// </summary>
        /// <param name="listBox">内容を表示するListBox。</param>
        /// <param name="path">読み込むディレクトリのパス。</param>
        public static void LoadDirectoryContent(ListBox listBox, string path)
        {
            listBox.Items.Clear();
            if (Directory.Exists(path))
            {
                try
                {
                    foreach (var dir in Directory.GetDirectories(path))
                    {
                        var dirInfo = new DirectoryInfo(dir);
                        if ((dirInfo.Attributes & FileAttributes.Hidden) == 0 && !dirInfo.Name.StartsWith("."))
                        {
                            listBox.Items.Add(CreateListBoxItem(Path.GetFileName(dir), dir, true));
                        }
                    }

                    foreach (var file in Directory.GetFiles(path))
                    {
                        var fileInfo = new FileInfo(file);
                        if ((fileInfo.Attributes & FileAttributes.Hidden) == 0 && !fileInfo.Name.StartsWith("."))
                        {
                            listBox.Items.Add(CreateListBoxItem(Path.GetFileName(file), file, false));
                        }
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
