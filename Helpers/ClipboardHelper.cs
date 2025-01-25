using System.Collections;
using System.Collections.Specialized;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ColumnExplorer.Helpers
{
    public static class ClipboardHelper
    {
        // Constants
        private const string COPIED = " Copied! ";
        private const string PASTED = " Pasted! ";
        private const string STATUS_BACKGROUND = "#FFFFAF";

        /// <summary>
        /// Copies the selected items to the clipboard and changes their appearance.
        /// </summary>
        /// <param name="selectedItems">The collection of selected items to copy to the clipboard.</param>
        /// <param name="rightColumnLabel">The label to update after copying.</param>
        public static async void CopySelectedItemsToClipboard(System.Collections.IList selectedItems, TextBlock rightColumnLabel)
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
                            listBoxItem.FontWeight = FontWeights.Bold;
                        }
                    }
                }
                if (selectedPaths.Count > 0)
                {
                    Clipboard.SetFileDropList(selectedPaths);

                    // Update the label to COPIED and change background color
                    string originalText = rightColumnLabel.Text;
                    Brush originalBackground = rightColumnLabel.Background;
                    rightColumnLabel.Text = COPIED;
                    rightColumnLabel.Background
                        = new SolidColorBrush((Color)ColorConverter.ConvertFromString(STATUS_BACKGROUND));

                    // Wait for 1.5 seconds
                    await Task.Delay(1500);

                    // Restore the original text and background color
                    rightColumnLabel.Text = originalText;
                    rightColumnLabel.Background = originalBackground;
                }
            }
        }

        /// <summary>
        /// Pastes the items from the clipboard to the specified ListBox and copies the files to the specified directory.
        /// </summary>
        /// <param name="column">The ListBox to paste the items into.</param>
        /// <param name="destinationDirectory">The directory to copy the files to.</param>
        public static async void PasteFromClipboard(ListBox column, TextBlock columnLabel, string destination)
        {
            if (Clipboard.ContainsFileDropList())
            {
                // Clear the column's selected list
                column.SelectedItems.Clear();

                // Get files in clipboard
                StringCollection clipboardFiles = Clipboard.GetFileDropList();
                foreach (string? sourceFilePath in clipboardFiles)
                {
                    if (sourceFilePath == null) continue;

                    string fileName = System.IO.Path.GetFileName(sourceFilePath) ?? "FILE_NAME";
                    string targetFilePath = System.IO.Path.Combine(destination, fileName);

                    // Copy the file to the destination directory
                    File.Copy(sourceFilePath, targetFilePath, overwrite: true);

                    // Add the file to the ListBox
                    ListBoxItem newItem = new ListBoxItem
                    {
                        Content = fileName,
                        Tag = targetFilePath // タグにはコピー先のフルパスを設定
                    };
                    column.Items.Add(newItem);

                    // Add file to the column's selected list
                    column.SelectedItems.Add(newItem);
                }

                // provide feedback to the user
                string originalText = columnLabel.Text;
                Brush originalBackground = columnLabel.Background;
                columnLabel.Text = PASTED;
                columnLabel.Background
                    = new SolidColorBrush((Color)ColorConverter.ConvertFromString(STATUS_BACKGROUND));

                // Wait for 1.5 seconds
                await Task.Delay(1500);

                // Restore the original text and background color
                columnLabel.Text = originalText;
                columnLabel.Background = originalBackground;
            }
            else
            {
                MessageBox.Show("The clipboard does not contain any files to paste.", "No Files to Paste",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}