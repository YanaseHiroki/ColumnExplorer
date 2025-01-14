using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColumnExplorer.Helpers
{
    public static class DirectoryHelper
    {
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

        public static void LoadDirectoryContent(ListBox listBox, string path, bool isDirectory)
        {
            listBox.Items.Clear();
            if (isDirectory && Directory.Exists(path))
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
        }
    }
}
