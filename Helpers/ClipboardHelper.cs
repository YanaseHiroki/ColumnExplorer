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
        private const string COPIED = " Copy! ";
        private const string PASTED = " Paste! ";
        private const string STATUS_BACKGROUND = "#FFFFAF";

        // List to store cut paths
        private static List<string> _cutPaths = new List<string>();

        /// <summary>
        /// Copies the selected items to the clipboard and changes their appearance.
        /// </summary>
        /// <param name="selectedItems">The collection of selected items to copy to the clipboard.</param>
        /// <param name="rightColumnLabel">The label to update after copying.</param>
        public static async void CopySelectedItemsToClipboard(System.Collections.IList selectedItems, string currentPath, TextBlock rightColumnLabel)
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
                    await UpdateLabelTemporarily(rightColumnLabel, currentPath, COPIED);
                }
            }
        }

        /// <summary>
        /// Pastes the items from the clipboard to the specified ListBox and copies the files to the specified directory.
        /// </summary>
        /// <param name="column">The ListBox to paste the items into.</param>
        /// <param name="columnLabel">Path for the label to update after pasting.</param>
        /// <param name="currentPath">The current path of the column.</param>
        /// <param name="destination">The directory to copy the files to.</param>
        public static async void PasteFromClipboard(ListBox column, TextBlock columnLabel, string currentPath, string destination)
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

                    // Check if the file is in use
                    if (IsFileInUse(sourceFilePath))
                    {
                        MessageBox.Show($"The file '{fileName}' is currently in use and cannot be copied.", "File In Use",
                            MessageBoxButton.OK, MessageBoxImage.Warning);
                        continue;
                    }

                    // Copy the file to the destination directory
                    try
                    {
                        File.Copy(sourceFilePath, targetFilePath, overwrite: true);
                    }
                    catch (IOException ex)
                    {
                        MessageBox.Show($"'{fileName}': {ex.Message}", "Copy Error",
                            MessageBoxButton.OK, MessageBoxImage.Error);
                        continue;
                    }

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
                await UpdateLabelTemporarily(columnLabel, currentPath, PASTED);
            }

            // move the cut items
            if (_cutPaths.Count > 0)
            {
                foreach (var cutPath in _cutPaths)
                {
                    string fileName = Path.GetFileName(cutPath);
                    string destinationPath = Path.Combine(destination, fileName);

                    try
                    {
                        File.Move(cutPath, destinationPath);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"ファイルの移動中にエラーが発生しました: {ex.Message}");
                    }
                }
                _cutPaths.Clear();
                // provide feedback to the user
                await UpdateLabelTemporarily(columnLabel, currentPath, PASTED);
            }
        }

        /// <summary>
        /// Stores the paths of the items to be cut.
        /// </summary>
        /// <param name="selectedItems">The collection of selected items to cut.</param>
        public static void CutSelectedItems(System.Collections.IList selectedItems)
        {
            _cutPaths = selectedItems.Cast<ListBoxItem>()
                .Select(item => item.Tag?.ToString()!)
                .Where(path => !string.IsNullOrEmpty(path))
                .ToList()!;
        }

        /// <summary>
        /// Checks if the specified file is in use.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        private static bool IsFileInUse(string filePath)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.None));
            }
            catch (IOException)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Updates the label temporarily with the specified message.
        /// </summary>
        /// <param name="label"></param>
        /// <param name="currenPath"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static async Task UpdateLabelTemporarily(TextBlock label, string currenPath, string message)
        {
            label.Text = message;
            label.Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString(STATUS_BACKGROUND));

            // Wait for 1.5 seconds
            await Task.Delay(1500);

            // Restore the original text and background color
            label.Text = PathHelper.GetLabel(currenPath);
            label.Background = Brushes.Transparent;
        }
    }
}