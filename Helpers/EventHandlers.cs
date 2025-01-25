using System.IO;
using System.Windows.Controls;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// Static class that provides various event handlers.
    /// </summary>
    public static class EventHandlers
    {
        /// <summary>
        /// Event handler called when the selection in the centerColumn changes.
        /// Displays the content of the selected directory in the rightColumn.
        /// </summary>
        /// <param name="sender">The object that raised the event.</param>
        /// <param name="e">Event data.</param>
        /// <param name="rightColumn">The ListBox to display the content.</param>
        public static void CenterColumn_SelectionChanged(object sender, SelectionChangedEventArgs e, ListBox rightColumn)
        {
            if (sender is ListBox centerColumn && centerColumn.SelectedItem is ListBoxItem selectedItem)
            {
                string? path = (selectedItem.Tag == null) ? string.Empty : selectedItem.Tag.ToString();
                DirectoryHelper.LoadDirectoryContent(rightColumn, path);
            }
        }
    }
}
