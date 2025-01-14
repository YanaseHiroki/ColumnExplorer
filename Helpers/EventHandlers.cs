using System.IO;
using System.Windows.Controls;

namespace ColumnExplorer.Helpers
{
    public static class EventHandlers
    {
        public static void Column2_SelectionChanged(object sender, SelectionChangedEventArgs e, ListBox column3)
        {
            if (sender is ListBox column2 && column2.SelectedItem is ListBoxItem selectedItem)
            {
                string path = selectedItem.Tag.ToString();
                DirectoryHelper.LoadDirectoryContent(column3, path, true);
            }
        }
    }
}
