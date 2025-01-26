using System.IO;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.ComponentModel;
using System.Linq;
using System.Security.AccessControl;
using NUnit.Framework.Internal;

namespace ColumnExplorer.Helpers
{
    public static class UnsupportedFilePreviewer
    {
        // Constants
        private const string DATE_TIME_FORMAT = "yyyy/MM/dd HH:mm:ss";

        /// <summary>
        /// Previews the properties of an unsupported file in the specified ListBox.
        /// </summary>
        /// <param name="listBox">The ListBox to display the file properties in.</param>
        /// <param name="filePath">The path of the file to preview.</param>
        public static void PreviewUnsupportedFile(ListBox listBox, string filePath)
        {
            if (File.Exists(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                var grid = new Grid();
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) });
                grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                AddPropertyRow(grid, "Name", fileInfo.Name);
                AddPropertyRow(grid, "Extension", fileInfo.Extension);
                AddPropertyRow(grid, "Directory", fileInfo.DirectoryName ?? string.Empty);
                AddPropertyRow(grid, "Size", FormatFileSize(fileInfo.Length));
                AddPropertyRow(grid, "Created at", fileInfo.CreationTime.ToString(DATE_TIME_FORMAT));
                AddPropertyRow(grid, "Last Access", fileInfo.LastAccessTime.ToString(DATE_TIME_FORMAT));
                AddPropertyRow(grid, "Last Write", fileInfo.LastWriteTime.ToString(DATE_TIME_FORMAT));
                AddPropertyRow(grid, "Writable", (!fileInfo.IsReadOnly).ToString());
                AddPropertyRow(grid, "Attributes", fileInfo.Attributes.ToString());

                // permissions
                var fileSecurity = fileInfo.GetAccessControl();
                var owner = fileSecurity.GetOwner(typeof(System.Security.Principal.NTAccount)).ToString();
                AddPropertyRow(grid, "Owner", owner);

                var accessRules = fileSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
                foreach (FileSystemAccessRule rule in accessRules)
                {
                    AddPropertyRow(grid, "Access Rule", $"{rule.IdentityReference}: {rule.FileSystemRights} ({rule.AccessControlType})");
                }

                listBox.Items.Clear();
                listBox.Items.Add(new ListBoxItem { Content = grid });
            }
        }

        /// <summary>
        /// Adds a row to the grid with the specified property name and value.
        /// </summary>
        /// <param name="grid">The grid to add the row to.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="propertyValue">The value of the property.</param>
        private static void AddPropertyRow(Grid grid, string propertyName, string propertyValue)
        {
            var row = new RowDefinition { Height = GridLength.Auto };
            grid.RowDefinitions.Add(row);

            var propertyNameTextBlock = new TextBlock
            {
                Text = propertyName,
                FontWeight = FontWeights.Bold,
                Margin = new Thickness(5),
                ToolTip = $"{propertyName} of the file."
            };
            Grid.SetRow(propertyNameTextBlock, grid.RowDefinitions.Count - 1);
            Grid.SetColumn(propertyNameTextBlock, 0);
            grid.Children.Add(propertyNameTextBlock);

            var propertyValueTextBox = new TextBox
            {
                Text = propertyValue,
                Margin = new Thickness(5),
                ToolTip =  propertyName,
                IsReadOnly = true,
                Background = Brushes.Transparent
            };
            Grid.SetRow(propertyValueTextBox, grid.RowDefinitions.Count - 1);
            Grid.SetColumn(propertyValueTextBox, 1);
            grid.Children.Add(propertyValueTextBox);
        }
        private static string GetFileDescription(string extension)
        {
            if (Enum.TryParse(typeof(FileExtensionDescriptions), extension.TrimStart('.'), true, out var result))
            {
                var fieldInfo = result.GetType().GetField(result.ToString() ?? string.Empty);
                var descriptionAttributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (descriptionAttributes.Any())
                {
                    return descriptionAttributes.First().Description;
                }
            }
            return "No description";
        }

        /// <summary>
        /// Formats the file size in bytes to a more readable format (KB, MB, GB).
        /// </summary>
        /// <param name="bytes">The file size in bytes.</param>
        /// <returns>A string representing the file size in KB, MB, or GB.</returns>
        private static string FormatFileSize(long bytes)
        {
            const long KB = 1024;
            const long MB = KB * 1024;
            const long GB = MB * 1024;

            if (bytes >= GB)
                return $"{bytes / (double)GB:0.##} GB";
            else if (bytes >= MB)
                return $"{bytes / (double)MB:0.##} MB";
            else if (bytes >= KB)
                return $"{bytes / (double)KB:0.##} KB";
            else
                return $"{bytes} bytes";
        }

        private static string GetToolTip(string property, string value)
        {
            propertyName == "Extension" ? GetFileDescription(propertyValue) :
            ToolTip = propertyName == "Attributes" ? GetAttributesDescription(propertyValue) : (propertyName == "Extension" ? GetFileDescription(propertyValue) : propertyValue),

        }

        /// <summary>
        /// Get the description for the value of the property "Attribute"
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        private static string GetAttributesDescription(string attributes)
        {
            var descriptions = attributes.Split(',')
                .Select(attr => attr.Trim() switch
                {
                    "Archive" => "This file is marked for backup or archiving.",
                    "Compressed" => "This file is compressed.",
                    "Directory" => "This is a directory.",
                    "Encrypted" => "This file is encrypted.",
                    "Hidden" => "This file is hidden.",
                    "Normal" => "This file has no special attributes.",
                    "ReadOnly" => "This file is read-only.",
                    "System" => "This file is a system file.",
                    "Temporary" => "This file is temporary.",
                    _ => attr
                });
            return string.Join(", ", descriptions);
        }
    }
}
