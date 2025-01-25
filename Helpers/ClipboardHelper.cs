using System.Collections;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColumnExplorer.Helpers
{
    public static class ClipboardHelper
    {
        /// <summary>
        /// Copies the selected items to the clipboard and changes their appearance.
        /// </summary>
        /// <param name="selectedItems">The collection of selected items to copy to the clipboard.</param>
        public static void CopySelectedItemsToClipboard(System.Collections.IList selectedItems)
        {
            if (selectedItems.Count > 0)
            {
                var selectedPaths = new StringCollection();
                foreach (var selectedItem in selectedItems)
                {
                    if (selectedItem is ListBoxItem listBoxItem)
                    {
                        string? path = listBoxItem.Tag?.ToString();
                        if (path != null)
                        {
                            selectedPaths.Add(path);
                            // Change the appearance of the ListBoxItem
                            listBoxItem.FontWeight = FontWeights.Bold;
                            listBoxItem.Foreground = new SolidColorBrush(Colors.MediumOrchid);
                        }
                    }
                }
                if (selectedPaths.Count > 0)
                {
                    Clipboard.SetFileDropList(selectedPaths);
                }
            }
        }
    }
}
