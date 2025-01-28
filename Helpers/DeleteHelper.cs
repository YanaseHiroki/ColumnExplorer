using System.Collections;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Threading.Tasks;

namespace ColumnExplorer.Helpers
{
    public static class DeleteHelper
    {
        private const string DELETED = " Deleted! ";
        private const string STATUS_BACKGROUND = "#FFFFAF";

        /// <summary>
        /// Deletes the selected items from the ListBox and the file system.
        /// </summary>
        /// <param name="selectedItems">The collection of selected items to delete.</param>
        /// <param name="centerColumnLabel">The label to update after deleting.</param>
        public static async void DeleteSelectedItems(IList selectedItems, TextBlock centerColumnLabel)
        {
            if (selectedItems.Count > 0)
            {
                var itemsToRemove = new List<ListBoxItem>();
                ListBox? parentListBox = null;

                foreach (var selectedItem in selectedItems)
                {
                    if (selectedItem is ListBoxItem listBoxItem)
                    {
                        string? path = listBoxItem.Tag?.ToString();
                        if (path != null && File.Exists(path))
                        {
                            File.Delete(path);
                            itemsToRemove.Add(listBoxItem);
                            parentListBox = listBoxItem.Parent as ListBox;
                        }
                    }
                }

                // Remove items from the ListBox
                foreach (var item in itemsToRemove)
                {
                    parentListBox?.Items.Remove(item);
                }

                // Select the first item in the ListBox if it exists
                if (parentListBox != null && parentListBox.Items.Count > 0)
                {
                    parentListBox.SelectedIndex = 0;
                }

                // Update the label to DELETED and change background color
                string originalText = centerColumnLabel.Text;
                Brush originalBackground = centerColumnLabel.Background;
                centerColumnLabel.Text = DELETED;
                centerColumnLabel.Background
                    = new SolidColorBrush((Color)ColorConverter.ConvertFromString(STATUS_BACKGROUND));

                // Wait for 1.5 seconds
                await Task.Delay(1500);

                // Restore the original text and background color
                centerColumnLabel.Text = originalText;
                centerColumnLabel.Background = originalBackground;
            }
        }
    }
}
