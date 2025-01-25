using System.IO;
using System.Windows.Controls;
using ColumnExplorer.Helpers;

namespace ColumnExplorer.Helpers
{
    public static class ContentLoader
    {
        /// <summary>
        /// Adds a list of drives to the ListBox.
        /// </summary>
        /// <param name="listBox">The ListBox to add the drives to.</param>
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
        /// Adds the subdirectories and files of the specified directory to the ListBox.
        /// </summary>
        /// <param name="listBox">The ListBox to add the subdirectories and files to.</param>
        /// <param name="directoryPath">The path of the directory.</param>
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
