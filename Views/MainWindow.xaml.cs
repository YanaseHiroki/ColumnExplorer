using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ColumnExplorer.Helpers;
using System.Collections.Generic;

namespace ColumnExplorer.Views
{
    /// <summary>
    /// Represents the main window of the application.
    /// </summary>
    public partial class MainWindow : Window
    {
        // String representing the drive list
        private const string DRIVE = "Drive";
        // String representing the selected items
        private const string SELECTED_ITEMS = "Selected Items";
        // Home directory
        internal static string _homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        internal string LeftColumnPath = string.Empty;
        internal string CenterColumnPath = string.Empty;
        internal string RightColumnPath = string.Empty;
        // Stack to store the previous directories
        private Stack<string> _previousDirectories = new Stack<string>();
        // Stack to store the forward directories
        private Stack<string> _forwardDirectories = new Stack<string>();

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            InitializeComponent();
            Loaded += MainWindow_Loaded;
            LeftColumn.MouseLeftButtonUp += LeftColumn_MouseLeftButtonUp;
            RightColumn.SelectionChanged += RightColumn_SelectionChanged;
            MouseDown += MainWindow_MouseDown;
        }

        /// <summary>
        /// Event handler called when the selection in the right column changes.
        /// </summary>
        private void RightColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RightColumn.SelectedItem is ListBoxItem selectedItem)
            {
                // Path of the selected item in the right column
                string? selectedItemPath = selectedItem.Tag?.ToString();

                // If the item has a path, select it in the center column
                if (selectedItemPath != null)
                {

                    // Store old center column path
                    string? oldPath = CenterColumnPath;

                    // Shift all items to the left so that the right column content moves to the center
                    MoveItemsLeft();

                    // Select the clicked item in the center column
                    SelectItemInColumn(CenterColumn, selectedItemPath);

                    // Push the old center column path to the stack
                    PushToPreviousDirectories(oldPath);
                }
            }
        }

        /// <summary>
        /// Event handler called when an item in the left column is clicked.
        /// </summary>
        private void LeftColumn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (LeftColumn.SelectedItem is ListBoxItem selectedItem)
            {
                // Path of the selected item in the left column
                string? selectedItemPath = selectedItem.Tag?.ToString();

                // If the item has a path and it is different from the center column path
                if (selectedItemPath != null
                    && !string.Equals(selectedItemPath, CenterColumnPath))
                {
                    // Store old center column path
                    string? oldPath = CenterColumnPath;

                    // Clear the right column
                    RightColumn.Items.Clear();
                    RightColumnLabel.Text = string.Empty;

                    // Display the content of the selected item in the center column
                    LoadAllContent(selectedItemPath);

                    // Push the old center column path to the stack
                    PushToPreviousDirectories(oldPath);
                }
            }
        }

        /// <summary>
        /// Event handler called when the window is loaded.
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadHomeDirectory();
        }

        /// <summary>
        /// Loads the content of the home directory.
        /// </summary>
        internal void LoadHomeDirectory()
        {
            LoadAllContent(_homeDirectory);
        }

        /// <summary>
        /// Focuses on the selected item in the center column and displays its content in the right column if necessary.
        /// </summary>
        /// <param name="targetItem">The path of the item to focus on. If null or empty, the right column path is used.</param>
        private void FocusSelectedItemInCenterColumn(string? targetItem)
        {
            // If the target item is not specified
            if (string.IsNullOrEmpty(targetItem))
            {
                // Select the right column path in the center column
                SelectItemInColumn(CenterColumn, RightColumnPath);
            }
            // If the target item is specified
            else
            {
                // Select the target item
                SelectItemInColumn(CenterColumn, targetItem);
            }

            // If the selected item in the center column is a folder, display its content in the right column
            if (CenterColumn.SelectedItem is ListBoxItem selectedItem)
            {
                string? itemPath = selectedItem.Tag?.ToString();
                if (itemPath != null && Directory.Exists(itemPath))
                {
                    // Display the content of the folder in the right column
                    DirectoryHelper.LoadDirectoryContent(RightColumn, itemPath);
                    RightColumnLabel.Text = GetLabel(itemPath);
                    RightColumnPath = itemPath;
                }
            }

            // Focus on the selected item in the center column
            if (CenterColumn.SelectedItem is ListBoxItem selectedListBoxItem)
            {
                selectedListBoxItem.Focus();
            }
        }

        /// <summary>
        /// Selects the item in the column that matches the specified path.
        /// </summary>
        /// <param name="column">The ListBox to select the item in.</param>
        /// <param name="path">The path of the item to select.</param>
        internal void SelectItemInColumn(ListBox column, string path)
        {
            // If the path is not specified, select the first item
            if (string.IsNullOrEmpty(path))
            {
                column.SelectedIndex = 0;
                return;
            }
            else

            // Search for the item in the column
            if (column.Items.Count > 0)
            {
                foreach (var item in column.Items)
                {
                    if (item is ListBoxItem listBoxItem
                        && listBoxItem.Tag != null
                        && listBoxItem.Tag.ToString() == path)
                    {
                        column.SelectedItem = listBoxItem;
                        break;
                    }
                }

                // If the item is not found, select the first item
                if (column.SelectedItem == null && column.Items.Count > 0)
                {
                    column.SelectedIndex = 0;
                }

            }
        }

        /// <summary>
        /// Event handler called when the selection in the center column changes.
        /// </summary>
        private void CenterColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EventHandlers.CenterColumn_SelectionChanged(sender, e, RightColumn);
            if (CenterColumn.SelectedItem is ListBoxItem selectedItem)
            {
                if (selectedItem.Content is TextBlock textBlock)
                {
                    RightColumnLabel.Text = textBlock.Text;
                }
                else
                {
                    RightColumnLabel.Text = selectedItem.Content.ToString();
                }
                RightColumnPath = selectedItem.Tag?.ToString() ?? string.Empty;

                // If the selected item is a text file, display a preview in the right column
                if (RightColumnPath != null && File.Exists(RightColumnPath))
                {
                    var extension = Path.GetExtension(RightColumnPath).ToLower();
                    if (extension == ".txt")
                    {
                        TextFilePreviewer.PreviewTextFile(RightColumn, RightColumnPath);
                    }
                    else if (extension == ".docx")
                    {
                        WordFilePreviewer.PreviewWordFile(RightColumn, RightColumnPath);
                    }
                    else if (extension == ".pptx")
                    {
                        var stackPanel = new StackPanel();
                        var slides = PowerPointFilePreviewer.GetSlidesAsBitmaps(RightColumnPath, TimeSpan.FromMinutes(1), 100 * 1024 * 1024); // 100MB memory limit
                        RightColumn.Items.Clear();
                        foreach (var slide in slides)
                        {
                            var image = new Image
                            {
                                Source = PowerPointFilePreviewer.ConvertBitmapToBitmapImage(slide),
                                Margin = new Thickness(5)
                            };
                            stackPanel.Children.Add(image);
                        }
                        RightColumn.Items.Add(new ListBoxItem { Content = stackPanel });
                    }
                    else if (extension == ".jpg" || extension == ".jpeg" || extension == ".png" || extension == ".bmp" || extension == ".gif" || extension == ".heic" || extension == ".webp")
                    {
                        var imageControl = new Image();
                        ImageFilePreviewer.PreviewImageFile(imageControl, RightColumnPath);
                        RightColumn.Items.Clear();
                        RightColumn.Items.Add(new ListBoxItem { Content = imageControl });
                    }
                    // If the selected item is a PDF file, display a preview in the right column
                    else if (extension == ".pdf")
                    {
                        PdfFilePreviewer.PreviewPdfFile(RightColumn, RightColumnPath);
                    }
                }
            }
        }

        /// <summary>
        /// Event handler called when a key is pressed.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Right) // →
            {
                MoveToSubDirectory();
            }
            else if (e.Key == Key.Left) // ←
            {
                MoveToParentDirectory();
            }
            else if (e.Key == Key.Up) // ↑
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    SelectUpperItemWithShift();
                }
                else
                {
                    SelectUpperItem();
                }
            }
            else if (e.Key == Key.Down) // ↓
            {
                if (Keyboard.Modifiers == ModifierKeys.Shift)
                {
                    SelectLowerItemWithShift();
                }
                else
                {
                    SelectLowerItem();
                }
            }
            else if (e.Key == Key.Enter) // Enter
            {
                OpenSelectedItems();
            }
            else if (e.Key == Key.F5) // F5
            {
                LoadAllContent(CenterColumnPath);
            }
            else if (e.Key == Key.A && Keyboard.Modifiers == ModifierKeys.Control) // Ctrl + A
            {
                SelectAllItems();
            }
            else if (e.Key == Key.W && Keyboard.Modifiers == ModifierKeys.Control) // Ctrl + W
            {
                Close(); // Close the application
            }
            else if (e.Key == Key.C && Keyboard.Modifiers == ModifierKeys.Control) // Ctrl + C
            {
                ClipboardHelper.CopySelectedItemsToClipboard(CenterColumn.SelectedItems); // Copy selected items to clipboard
            }
        }

        /// <summary>
        /// Selects all items in the CenterColumn.
        /// </summary>
        private void SelectAllItems()
        {
            CenterColumn.SelectAll();
            UpdateRightColumnWithSelectedItems();
        }

        /// <summary>
        /// Moves the selection in the CenterColumn to the next item.
        /// </summary>
        private void SelectLowerItem()
        {
            if (CenterColumn.SelectedIndex < CenterColumn.Items.Count - 1)
            {
                CenterColumn.SelectedIndex++;
            }
        }

        /// <summary>
        /// Moves the selection in the CenterColumn to the previous item.
        /// </summary>
        private void SelectUpperItem()
        {
            if (CenterColumn.SelectedIndex > 0)
            {
                CenterColumn.SelectedIndex--;
            }
        }

        /// <summary>
        /// Moves the selection in the CenterColumn to the next item while holding the Shift key.
        /// </summary>
        private void SelectLowerItemWithShift()
        {
            if (CenterColumn.SelectedIndex < CenterColumn.Items.Count - 1)
            {
                int nextIndex = CenterColumn.SelectedIndex + 1;
                CenterColumn.SelectedItems.Add(CenterColumn.Items[nextIndex]);
                CenterColumn.SelectedIndex = nextIndex;
                UpdateRightColumnWithSelectedItems();
            }
        }

        /// <summary>
        /// Moves the selection in the CenterColumn to the previous item while holding the Shift key.
        /// </summary>
        private void SelectUpperItemWithShift()
        {
            if (CenterColumn.SelectedIndex > 0)
            {
                int previousIndex = CenterColumn.SelectedIndex - 1;
                CenterColumn.SelectedItems.Add(CenterColumn.Items[previousIndex]);
                CenterColumn.SelectedIndex = previousIndex;
                UpdateRightColumnWithSelectedItems();
            }
        }

        /// <summary>
        /// Moves to the subdirectory if a directory is selected in the center column when the right key is pressed.
        /// </summary>
        internal void MoveToSubDirectory()
        {
            if (CenterColumn.SelectedItem is ListBoxItem selectedItem)
            {
                string? selectedItemPath = selectedItem.Tag?.ToString();
                if (selectedItemPath != null && Directory.Exists(selectedItemPath))
                {
                    // store old center column path
                    string? oldPath = CenterColumnPath;

                    // Move the content of the center column to the left column
                    MoveItemsLeft();

                    // Push the old center column path to the stack
                    PushToPreviousDirectories(oldPath);
                }
            }
        }

        /// <summary>
        /// Move items to the left column and load contents of the right column
        /// </summary>
        private void MoveItemsLeft()
        {
            // Move the content of the center column to the left column
            MoveItems(CenterColumn, LeftColumn);
            LeftColumnLabel.Text = CenterColumnLabel.Text;
            LeftColumnPath = CenterColumnPath;

            // Move the content of the right column to the center column
            MoveItems(RightColumn, CenterColumn);
            CenterColumnLabel.Text = RightColumnLabel.Text;
            CenterColumnPath = RightColumnPath;

            // Clear the content of the right column
            RightColumnLabel.Text = string.Empty;
            RightColumn.Items.Clear();

            // Select the center column path in the left column
            if (!string.IsNullOrEmpty(CenterColumnPath))
            {
                SelectItemInColumn(LeftColumn, CenterColumnPath);
            }

            // Select the first item in the center column
            if (CenterColumn.Items.Count > 0)
            {
                CenterColumn.SelectedIndex = 0;
            }

            // Focus on the selected item in the center column
            if (CenterColumn.SelectedItem is ListBoxItem selectedListBoxItem)
            {
                selectedListBoxItem.Focus();
            }
        }

        /// <summary>
        /// Moves to the parent directory if it exists when the left key is pressed.
        /// </summary>
        internal void MoveToParentDirectory()
        {
            // store old center column path
            string? oldPath = CenterColumnPath;

            // Check if the left column is displaying a directory
            if (Directory.Exists(LeftColumnPath))
            {
                string? newLeftColumnPath = Directory.GetParent(LeftColumnPath)?.FullName;
                MoveItemsRignt(newLeftColumnPath);

            // Check if the left column is displaying the drive list
            } else if (string.Equals(LeftColumnPath, DRIVE))
            {
                string newLeftColumnPath = string.Empty;
                MoveItemsRignt(newLeftColumnPath);
            }

            // Push the old center column path to the stack
            PushToPreviousDirectories(oldPath);
        }

        /// <summary>
        /// Moves the content of the center column to the right column and the content of the left column to the center column.
        /// Then displays the content of the parent directory in the left column.
        /// </summary>
        /// <param name="parentDirectory">The path of the directory to display in the left column.</param>
        private void MoveItemsRignt(string? newLeftColumnPath)
        {
            // Move the content of the center column to the right column
            MoveItems(CenterColumn, RightColumn);
            RightColumnLabel.Text = CenterColumnLabel.Text;
            RightColumnPath = CenterColumnPath;

            // Move the content of the left column to the center column
            MoveItems(LeftColumn, CenterColumn);
            CenterColumnLabel.Text = LeftColumnLabel.Text;
            CenterColumnPath = LeftColumnPath;

            // Display in the left column
            // If the center column is displaying the drive list
            if (string.Equals(CenterColumnPath, DRIVE))
            {
                // Clear the content of the left column
                LeftColumn.Items.Clear();
                LeftColumnLabel.Text = string.Empty;
                LeftColumnPath = string.Empty;
            }
            else if (!string.Equals(CenterColumnPath, DRIVE) && string.IsNullOrEmpty(newLeftColumnPath))
            {
                // Display the drive list
                ContentLoader.AddDrives(LeftColumn);
                LeftColumnLabel.Text = DRIVE;
                LeftColumnPath = DRIVE;
            }
            else
            {
                // Display the content of the directory
                DirectoryHelper.LoadDirectoryContent(LeftColumn, newLeftColumnPath);
                LeftColumnLabel.Text = GetLabel(newLeftColumnPath);
                LeftColumnPath = newLeftColumnPath;
            }

            // Select the right column path in the center column
            if (!string.IsNullOrEmpty(RightColumnPath))
            {
                SelectItemInColumn(CenterColumn, RightColumnPath);
            }

            // Select the center column path in the left column
            if (!string.IsNullOrEmpty(CenterColumnPath))
            {
                SelectItemInColumn(LeftColumn, CenterColumnPath);
            }

            // Focus on the selected item in the center column
            if (CenterColumn.SelectedItem is ListBoxItem selectedListBoxItem)
            {
                selectedListBoxItem.Focus();
            }

            // Push the current directory to the stack
            PushToPreviousDirectories(CenterColumnPath);
        }

        /// <summary>
        /// Gets the label based on the specified path.
        /// </summary>
        /// <param name="path">The path to get the label for.</param>
        /// <returns>The label based on the path.</returns>
        private string GetLabel(string path)
        {
            return (path == System.IO.Path.GetPathRoot(path) || path == DRIVE)
                ? path
                : System.IO.Path.GetFileName(path);
        }

        /// <summary>
        /// Opens the selected items.
        /// </summary>
        internal void OpenSelectedItems()
        {
            foreach (var selectedItem in CenterColumn.SelectedItems)
            {
                if (selectedItem is ListBoxItem listBoxItem)
                {
                    string? path = listBoxItem.Tag?.ToString();
                    if (path != null && File.Exists(path))
                    {
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                    }
                }
            }
        }

        /// <summary>
        /// Displays the selected items in the center column in the right column
        /// </summary>
        internal void UpdateRightColumnWithSelectedItems()
        {
            // Clear the right column
            RightColumn.Items.Clear();
            // Update the label of the right column
            RightColumnLabel.Text = SELECTED_ITEMS;

            // Add the selected items to the right column
            foreach (var selectedItem in CenterColumn.SelectedItems)
            {
                if (selectedItem is ListBoxItem listBoxItem)
                {
                    var newItem = new ListBoxItem
                    {
                        Content = listBoxItem.Content,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };
                    RightColumn.Items.Add(newItem);
                }
            }
            // Select all items in the right column
            RightColumn.SelectAll();
        }

        /// <summary>
        /// Moves items from one ListBox to another ListBox.
        /// </summary>
        /// <param name="source">The source ListBox.</param>
        /// <param name="destination">The destination ListBox.</param>
        internal void MoveItems(ListBox source, ListBox destination)
        {
            // Clear the items in the destination
            destination.Items.Clear();

            // Clear the selected items information in the destination
            destination.SelectedItems.Clear();

            // Copy items to the destination
            foreach (var item in source.Items)
            {
                var clonedItem = new ListBoxItem
                {
                    Content = ((ListBoxItem)item).Content,
                    Tag = ((ListBoxItem)item).Tag,
                    Foreground = ((ListBoxItem)item).Foreground
                };
                destination.Items.Add(clonedItem);
            }

            // Copy selected items information
            foreach (var item in source.SelectedItems)
            {
                destination.SelectedItems.Add(item);
            }
            // Clear the items in the source
            source.Items.Clear();
        }

        /// <summary>
        /// Event handler for the mouse buttons.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.XButton1 == MouseButtonState.Pressed)
            {
                MoveToPreviousDirectory();
            }
            else if (e.XButton2 == MouseButtonState.Pressed)
            {
                MoveToForwardDirectory();
            }
        }

        /// <summary>
        /// Moves to the previous directory.
        /// </summary>
        private void MoveToPreviousDirectory()
        {
            if (_previousDirectories.Count > 0)
            {
                string previousDirectory = _previousDirectories.Pop();
                _forwardDirectories.Push(CenterColumnPath);
                LoadAllContent(previousDirectory);
            }
        }

        /// <summary> 
        /// Loads the content of the specified path into each column.
        /// </summary>
        internal void LoadAllContent(string path)
        {
            // Load the content of the center column
            try
            {
                CenterColumnPath = path;

                if (string.IsNullOrEmpty(CenterColumnPath))
                {
                    // Display the drive list in the center column
                    ContentLoader.AddDrives(CenterColumn);
                    CenterColumnLabel.Text = DRIVE;
                }
                else
                {
                    // Display the content of the directory
                    DirectoryHelper.LoadDirectoryContent(CenterColumn, CenterColumnPath);
                    CenterColumnLabel.Text = GetLabel(CenterColumnPath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in LoadAllContent: {ex.Message}");
                throw;
            }

            // Load the content of the left column
            if (CenterColumnPath != string.Empty)
            {
                LeftColumnPath = Directory.GetParent(CenterColumnPath)?.FullName ?? string.Empty;

                if (string.IsNullOrEmpty(LeftColumnPath))
                {
                    // Display the drive list
                    ContentLoader.AddDrives(LeftColumn);
                    LeftColumnLabel.Text = DRIVE;
                }
                else
                {
                    // Display the content of the directory
                    DirectoryHelper.LoadDirectoryContent(LeftColumn, LeftColumnPath);
                    LeftColumnLabel.Text = GetLabel(LeftColumnPath);
                }

                if (LeftColumn.Items.Count > 0 && CenterColumnPath != null)
                {
                    // Select the center column path in the left column
                    SelectItemInColumn(LeftColumn, CenterColumnPath);
                }
            }

            // Focus on the selected item in the center column
            // to display its content in the right column
            FocusSelectedItemInCenterColumn(string.Empty);
        }

        /// <summary>
        /// Pushes the specified path to the stack of previous directories.
        /// </summary>
        /// <param name="path"></param>
        private void PushToPreviousDirectories(string? path)
        {
            if (path != null &&
                (_previousDirectories.Count == 0 || _previousDirectories.Peek() != path))
            {
                _previousDirectories.Push(path);
            }
        }

        /// <summary>
        /// Moves to the forward directory.
        /// </summary>
        private void MoveToForwardDirectory()
        {
            if (_forwardDirectories.Count > 0)
            {
                string forwardDirectory = _forwardDirectories.Pop();
                _previousDirectories.Push(CenterColumnPath);
                LoadAllContent(forwardDirectory);
            }
        }
    }
}
