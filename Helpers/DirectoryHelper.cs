using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// Static class that provides helper methods for directory operations.
    /// </summary>
    public static class DirectoryHelper
    {
        /// <summary>
        /// Creates a ListBoxItem based on the specified parameters.
        /// </summary>
        /// <param name="displayName">The display name.</param>
        /// <param name="path">The path of the item.</param>
        /// <param name="isDirectory">Whether the item is a directory.</param>
        /// <returns>The created ListBoxItem.</returns>
        public static ListBoxItem CreateListBoxItem(string displayName, string path, bool isDirectory)
        {
            var textBlock = new TextBlock
            {
                Text = displayName,
                Foreground = isDirectory ? Brushes.Blue : Brushes.Black
            };

            if (isDirectory)
            {
                textBlock.TextDecorations = TextDecorations.Underline;
            }

            return new ListBoxItem
            {
                Content = textBlock,
                Tag = path
            };
        }

        /// <summary>
        /// Loads the content of the specified directory into the ListBox.
        /// </summary>
        /// <param name="listBox">The ListBox to display the content.</param>
        /// <param name="path">The path of the directory to load.</param>
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
