using System.Collections;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using ColumnExplorer.Views;

namespace ColumnExplorer.Helpers
{
    public static class RenameHelper
    {
        public static void RenameSelectedItem(MainWindow mainWindow, ListBox? centerColumn, string centerColumnPath)
        {
            if (centerColumn != null &&
                centerColumn.SelectedItem is ListBoxItem selectedItem)
            {
                string? selectedItemPath = selectedItem.Tag?.ToString();
                if (selectedItemPath != null)
                {
                    string newName = PromptForNewName(Path.GetFileName(selectedItemPath));
                    if (!string.IsNullOrEmpty(newName))
                    {
                        string newPath = Path.Combine(Path.GetDirectoryName(selectedItemPath)!, newName);
                        try
                        {
                            if (Directory.Exists(selectedItemPath))
                            {
                                Directory.Move(selectedItemPath, newPath);
                            }
                            else if (File.Exists(selectedItemPath))
                            {
                                File.Move(selectedItemPath, newPath);
                            }
                            mainWindow.LoadAllContent(centerColumnPath);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error renaming item: {ex.Message}");
                        }
                    }
                }
            }
        }

        public static string PromptForNewName(string currentName)
        {
            var inputDialog = new InputDialog("Enter new name", currentName);
            if (inputDialog.ShowDialog() == true)
            {
                return inputDialog.ResponseText;
            }
            return string.Empty;
        }

        public static void BulkRename(IList selectedItems, string findText, string replaceText)
        {
            foreach (var item in selectedItems)
            {
                if (item is ListBoxItem listBoxItem)
                {
                    string? itemPath = listBoxItem.Tag?.ToString();
                    if (itemPath != null)
                    {
                        string itemName = Path.GetFileName(itemPath);
                        string newItemName = itemName.Replace(findText, replaceText);
                        if (!string.IsNullOrEmpty(newItemName) && newItemName != itemName)
                        {
                            string newPath = Path.Combine(Path.GetDirectoryName(itemPath)!, newItemName);
                            try
                            {
                                if (Directory.Exists(itemPath))
                                {
                                    Directory.Move(itemPath, newPath);
                                }
                                else if (File.Exists(itemPath))
                                {
                                    File.Move(itemPath, newPath);
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"Error renaming item: {ex.Message}");
                            }
                        }
                    }
                }
            }
        }
    }
}
