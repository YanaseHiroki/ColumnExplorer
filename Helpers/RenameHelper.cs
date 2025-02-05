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

        private static string PromptForNewName(string currentName)
        {
            var inputDialog = new InputDialog("Enter new name", currentName);
            if (inputDialog.ShowDialog() == true)
            {
                return inputDialog.ResponseText;
            }
            return string.Empty;
        }
    }
}
