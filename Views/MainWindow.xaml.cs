using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ColumnExplorer.Helpers;
using ColumnExplorer.Previewers;
using System.Collections.Generic;
using System.Collections;

namespace ColumnExplorer.Views
{
    /// <summary>
    /// Represents the main window of the application.
    /// </summary>
    public partial class MainWindow : Window
    {
        // const string
        private const string DRIVE = "Drive";
        private const string SELECTED_ITEMS = "Selected Items";
        private const string MOVE = "Move!";
        private const string UNDO = "Undo!";
        private const string REDO = "Redo!";

        // Home directory
        internal static string _homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        internal string LeftColumnPath = string.Empty;
        internal string CenterColumnPath = string.Empty;
        internal string RightColumnPath = string.Empty;

        // Stack to store the previous directories
        private Stack<string> _previousDirectories = new Stack<string>();

        // Stack to store the forward directories
        private Stack<string> _forwardDirectories = new Stack<string>();

        // Stack to store the undo and redo actions
        private Stack<Action> _undoStack = new Stack<Action>();
        private Stack<Action> _undoStackDraft = new Stack<Action>();
        private Stack<Action> _redoStack = new Stack<Action>();
        private Stack<Action> _redoStackDraft = new Stack<Action>();

        // List to store selected paths
        private List<string?> _selectedPaths = new List<string?>();

        // List to store cut paths
        private List<string> _cutPaths = new List<string>();

        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            InitializeComponent();
            Loaded += MainWindow_Loaded;

            // event handler
            LeftColumn.MouseLeftButtonUp += LeftColumn_MouseLeftButtonUp;
            RightColumn.MouseLeftButtonUp += RightColumn_MouseLeftButtonUp;
            MouseDown += MainWindow_MouseDown;
            LeftColumn.PreviewMouseLeftButtonDown += ListBox_PreviewMouseLeftButtonDown;
            LeftColumn.PreviewMouseLeftButtonUp += ListBox_PreviewMouseLeftButtonUp;
            LeftColumn.PreviewMouseMove += ListBox_PreviewMouseMove;
            CenterColumn.PreviewMouseLeftButtonDown += ListBox_PreviewMouseLeftButtonDown;
            CenterColumn.PreviewMouseLeftButtonUp += ListBox_PreviewMouseLeftButtonUp;
            CenterColumn.PreviewMouseMove += ListBox_PreviewMouseMove;
            RightColumn.PreviewMouseLeftButtonDown += ListBox_PreviewMouseLeftButtonDown;
            RightColumn.PreviewMouseLeftButtonUp += ListBox_PreviewMouseLeftButtonUp;
            RightColumn.PreviewMouseMove += ListBox_PreviewMouseMove;

            // Event handler for dropping items
            LeftColumn.Drop += ListBox_Drop;
            CenterColumn.Drop += ListBox_Drop;
            RightColumn.Drop += ListBox_Drop;

            // Event handler to load the content of the column path
            RightColumnLabel.MouseLeftButtonUp += RightColumnLabel_MouseLeftButtonUp;
            CenterColumnLabel.MouseLeftButtonUp += CenterColumnLabel_MouseLeftButtonUp;
            LeftColumnLabel.MouseLeftButtonUp += LeftColumnLabel_MouseLeftButtonUp;

            // Event handler for the mouse over
            LeftColumn.MouseMove += Column_MouseMove;
            LeftColumn.MouseLeave += Column_MouseLeave;
            RightColumn.MouseMove += Column_MouseMove;
            RightColumn.MouseLeave += Column_MouseLeave;
        }

        /// <summary>
        /// Event handler called when an item in the right column is mouse over.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Column_MouseMove(object sender, MouseEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                Point position = e.GetPosition(listBox);
                HitTestResult result = VisualTreeHelper.HitTest(listBox, position);

                if (result != null && result.VisualHit is ListBoxItem)
                {
                    // アイテムにマウスオーバーしている場合は背景を変更しない
                    listBox.Background = Brushes.Transparent;
                }
                else
                {
                    // 余白にマウスオーバーしている場合は背景を #F0FFFF に変更
                    listBox.Background = new SolidColorBrush(Color.FromRgb(249, 255, 255));
                }
            }
        }

        /// <summary>
        /// Event handler called when an item in the right column is mouse over.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Column_MouseLeave(object sender, MouseEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null)
            {
                // マウスがカラムから離れたときに背景を元に戻す
                listBox.Background = Brushes.Transparent;
            }
        }

        //　Starting point of the drag
        private Point _startPoint;

        /// <summary>
        /// Event handler called when an item in the left column is clicked.
        /// </summary>
        private void LeftColumn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (LeftColumn.SelectedItem is ListBoxItem selectedItem)
            {
                // Path of the selected item in the left column
                string? selectedItemPath = selectedItem.Tag?.ToString();

                // If the item has a path
                if (selectedItemPath != null)
                {
                    // old center column path for stack
                    string? oldPath = CenterColumnPath;

                    // new left column path
                    string? newLeftColumnPath = Directory.GetParent(LeftColumnPath)?.FullName;

                    // Move the content of the left column to the center column
                    MoveItemsRignt(newLeftColumnPath);

                    // Select the clicked item in the center column
                    SelectItemInColumn(CenterColumn, selectedItemPath);

                    // Focus on the center column
                    CenterColumn.Focus();

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
                    else
                    {
                        // If the selected item is a file with an unsupported extension, display its properties
                        if (RightColumnPath != null && File.Exists(RightColumnPath))
                        {
                            UnsupportedFilePreviewer.PreviewUnsupportedFile(RightColumn, RightColumnPath);
                        }
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
                ClipboardHelper.CopySelectedItemsToClipboard(CenterColumn.SelectedItems, RightColumnLabel); // Copy selected items to clipboard

                // Clear cut paths
                _cutPaths.Clear();
            }
            else if (e.Key == Key.V && Keyboard.Modifiers == ModifierKeys.Control) // Ctrl + V
            {
                // ListBox that has focus
                ListBox activeListBox = Keyboard.FocusedElement as ListBox ?? CenterColumn;

                // Paste from clipboard to the active ListBox
                ClipboardHelper.PasteFromClipboard(activeListBox, CenterColumnLabel, CenterColumnPath);

                // Restore the text color of the cut items
                foreach (string cutPath in _cutPaths)
                {
                    ListBoxItem? item = CenterColumn.Items.Cast<ListBoxItem>()
                        .FirstOrDefault(i => i.Tag?.ToString() == cutPath);
                    if (item != null)
                    {
                        ResetCutItemColor(item);
                    }
                }

                // Clear cut paths
                _cutPaths.Clear();
            }
            else if (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control) // Ctrl + X
            {
                // Save the paths of the selected items to _cutPaths
                ClipboardHelper.CutSelectedItems(CenterColumn.SelectedItems);

                // Change the text color of the selected items
                foreach (ListBoxItem item in CenterColumn.SelectedItems)
                {
                    ChangeCutItemColor(item);
                }

                // Clear the selected items
                CenterColumn.SelectedItems.Clear();

                // Clear clipboard data
                Clipboard.Clear();
            }

            else if (e.Key == Key.Delete) // Delete
            {
                DeleteHelper.DeleteSelectedItems(CenterColumn.SelectedItems, CenterColumnLabel); // Delete selected items
            }
            else if (e.Key == Key.N && Keyboard.Modifiers == (ModifierKeys.Control | ModifierKeys.Shift)) // Ctrl + Shift + N
            {
                CreateNewFolder();
            }
            else if (e.Key == Key.X && Keyboard.Modifiers == ModifierKeys.Control) // Ctrl + X
            {
                // Save the paths of the selected items to _cutPaths
                _cutPaths = CenterColumn.SelectedItems.Cast<ListBoxItem>()
                    .Select(item => item.Tag?.ToString()!)
                    .Where(path => !string.IsNullOrEmpty(path))
                    .ToList()!;

                // Clear the selected items
                CenterColumn.SelectedItems.Clear();
            }
            else if (e.Key == Key.Z && Keyboard.Modifiers == ModifierKeys.Control) // Ctrl + Z
            {
                Undo();
            }
            else if (e.Key == Key.Y && Keyboard.Modifiers == ModifierKeys.Control) // Ctrl + Y
            {
                Redo();
            }
        }

        /// <summary>
        /// Undo the last action.
        /// Then push an action from the redo draft stack to the redo stack.
        /// </summary>
        private async void Undo()
        {
            if (_undoStack.Count > 0)
            {
                // Undo the last action
                var undoAction = _undoStack.Pop();
                undoAction();
                _undoStackDraft.Push(undoAction);

                //Then push an action from the redo draft stack to the redo stack
                if (_redoStackDraft.Count > 0)
                {
                    _redoStack.Push(_redoStackDraft.Pop());
                }

                // Load all contents
                LoadAllContent(CenterColumnPath);

                // Set _selectedPaths for the center column
                _selectedPaths = CenterColumn.Items.Cast<ListBoxItem>()
                    .Where(item => item.IsSelected)
                    .Select(item => item.Tag?.ToString())
                    .ToList();

                // Feedback "Undo!"
                await ClipboardHelper.UpdateLabelTemporarily(CenterColumnLabel, UNDO);
            }
        }

        // 操作を再現するメソッド
        private async Task Redo()
        {
            if (_redoStack.Count > 0)
            {
                var redoAction = _redoStack.Pop();
                redoAction();
                _redoStackDraft.Push(redoAction);

                //Then push an action from the undo draft stack to the undo stack
                if (_undoStackDraft.Count > 0)
                {
                    _undoStack.Push(_undoStackDraft.Pop());
                }

                // Load all contents
                LoadAllContent(CenterColumnPath);

                // Set _selectedPaths for the center column
                _selectedPaths = CenterColumn.Items.Cast<ListBoxItem>()
                    .Where(item => item.IsSelected)
                    .Select(item => item.Tag?.ToString())
                    .ToList();

                // Feedback "Redo!"
                await ClipboardHelper.UpdateLabelTemporarily(CenterColumnLabel, REDO);
            }
        }

        /// <summary>
        /// Creates a new folder in the current directory.
        /// </summary>
        private void CreateNewFolder()
        {
            string currentPath = CenterColumnPath;
            if (Directory.Exists(currentPath))
            {
                string newFolderPath = Path.Combine(currentPath, "New Folder");
                int count = 1;
                while (Directory.Exists(newFolderPath))
                {
                    newFolderPath = Path.Combine(currentPath, $"New Folder ({count})");
                    count++;
                }
                Directory.CreateDirectory(newFolderPath);
                LoadAllContent(currentPath);
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
            }
            else if (string.Equals(LeftColumnPath, DRIVE))
            {
                string newLeftColumnPath = string.Empty;
                MoveItemsRignt(newLeftColumnPath);
            }

            // Push the old center column path to the stack
            PushToPreviousDirectories(oldPath);

            // Focus on the center column
            CenterColumn.Focus();

            // Focus on the folder which was displayed center
            if (CenterColumn.SelectedItem is ListBoxItem selectedListBoxItem)
            {
                selectedListBoxItem.Focus();
            }
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
                // Display the drive list in the left column
                ContentLoader.AddDrives(LeftColumn);
                LeftColumnLabel.Text = DRIVE;
                LeftColumnPath = DRIVE;
            }
            else
            {
                // Display the content of the directory in the left column
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
        /// Displays the selected items in the center column, in the right column
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
                // Clear all labels and all column items
                ClearAllColumns();
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

        // ドラッグ開始のイベントハンドラ
        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _startPoint = e.GetPosition(null);

            if  (sender is ListBox listBox && listBox != null)
            {
                // 選択されたすべてのアイテムのパスを取得して _selectedPaths に保存
                _selectedPaths = listBox.SelectedItems.Cast<ListBoxItem>()
                    .Select(item => item.Tag?.ToString())
                    .Where(path => !string.IsNullOrEmpty(path))
                    .ToList();
            }
        }

        // マウス移動のイベントハンドラ
        private void ListBox_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            Point mousePos = e.GetPosition(null);
            Vector diff = _startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                 Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                ListBox listBox = sender as ListBox;
                if (listBox != null)
                {
                    ListBoxItem listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
                    if (listBoxItem != null)
                    {

                        // _selectedPathsをCenterColumn.SelectedItemsに設定
                        if (_selectedPaths.Count > 1)
                        {
                            CenterColumn.SelectedItems.Clear();
                            foreach (var path in _selectedPaths)
                            {
                                ListBoxItem item = CenterColumn.Items.Cast<ListBoxItem>()
                                    .FirstOrDefault(i => i.Tag?.ToString() == path);
                                if (item != null)
                                {
                                    CenterColumn.SelectedItems.Add(item);
                                }
                            }
                        }

                        // ドラッグの処理で _selectedPaths を参照
                        if (_selectedPaths.Count > 0)
                        {
                            // ToolTipの設定
                            string toolTipText = string.Join("\n", _selectedPaths.Select(p => Path.GetFileName(p)));
                            ToolTip toolTip = new ToolTip
                            {
                                Content = toolTipText,
                                Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse
                            };
                            listBoxItem.ToolTip = toolTip;
                            toolTip.IsOpen = true;

                            // マウスが動くたびにToolTipを再表示
                            toolTip.PlacementTarget = listBoxItem;
                            toolTip.IsOpen = false;
                            toolTip.IsOpen = true;

                            DragDrop.DoDragDrop(listBoxItem, new DataObject("SelectedPaths", _selectedPaths.ToArray()), DragDropEffects.Move);

                            // ドラッグ終了後にToolTipを閉じる
                            toolTip.IsOpen = false;
                            listBoxItem.ToolTip = null;
                        }
                    }
                }
            }
        }

        // ドロップのイベントハンドラ
        private async void ListBox_Drop(object sender, DragEventArgs e)
        {
            ListBox listBox = sender as ListBox;
            if (listBox != null && e.Data.GetDataPresent("SelectedPaths"))
            {
                string[] sourcePaths = e.Data.GetData("SelectedPaths") as string[];

                // ドロップ先のアイテムを取得
                Point dropPosition = e.GetPosition(listBox);
                var targetItem = listBox.InputHitTest(dropPosition) as DependencyObject;
                var listBoxItem = FindAncestor<ListBoxItem>(targetItem);

                string targetPath = listBoxItem?.Tag?.ToString();

                // ドロップ先がフォルダーでない場合、カラムのパスを使用
                if (string.IsNullOrEmpty(targetPath))
                {
                    if (listBox == LeftColumn)
                    {
                        targetPath = LeftColumnPath;
                    }
                    else if (listBox == CenterColumn)
                    {
                        targetPath = CenterColumnPath;
                    }
                    else if (listBox == RightColumn)
                    {
                        targetPath = RightColumnPath;
                    }
                }

                if (sourcePaths != null && !string.IsNullOrEmpty(targetPath) && Directory.Exists(targetPath))
                {
                    MoveFiles(sourcePaths, targetPath);
                    LoadAllContent(targetPath); // 移動先のコンテンツを再読み込み
                    await ClipboardHelper.UpdateLabelTemporarily(CenterColumnLabel, MOVE);
                }
            }
        }

        /// <summary>
        /// ファイルを移動し、元に戻すためのアクションを履歴に追加します。
        /// </summary>
        /// <param name="sourcePaths">移動元のファイルパスの配列。</param>
        /// <param name="targetPath">移動先のディレクトリパス。</param>
        private void MoveFiles(string[] sourcePaths, string targetPath)
        {
            List<(string sourcePath, string destinationPath)> movedFiles = new List<(string sourcePath, string destinationPath)>();

            foreach (var sourcePath in sourcePaths)
            {
                string fileName = Path.GetFileName(sourcePath);
                string destinationPath = Path.Combine(targetPath, fileName);

                try
                {
                    File.Move(sourcePath, destinationPath);
                    movedFiles.Add((sourcePath, destinationPath));
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"ファイルの移動中にエラーが発生しました: {ex.Message}");
                }
            }

            // 全体を1回の操作としてUndoにPush
            Action undoAction = () => UndoMoveFiles(movedFiles);
            Action redoAction = () => RedoMoveFiles(movedFiles);
            AddToUndoStack(undoAction, redoAction);
        }

        /// <summary>
        /// ファイル移動を元に戻すメソッド
        /// </summary>
        /// <param name="movedFiles">移動したファイルのリスト。</param>
        private void UndoMoveFiles(List<(string sourcePath, string destinationPath)> movedFiles)
        {
            foreach (var (sourcePath, destinationPath) in movedFiles)
            {
                if (File.Exists(destinationPath))
                {
                    File.Move(destinationPath, sourcePath);
                }
            }
        }

        /// <summary>
        /// ファイル移動を再現するメソッド
        /// </summary>
        /// <param name="movedFiles">移動したファイルのリスト。</param>
        private void RedoMoveFiles(List<(string sourcePath, string destinationPath)> movedFiles)
        {
            foreach (var (sourcePath, destinationPath) in movedFiles)
            {
                if (File.Exists(sourcePath))
                {
                    File.Move(sourcePath, destinationPath);
                }
            }
        }

        /// <summary>
        /// 操作を履歴に追加するメソッド
        /// </summary>
        /// <param name="undoAction">元に戻すアクション。</param>
        /// <param name="redoAction">やり直すアクション。</param>
        private void AddToUndoStack(Action undoAction, Action redoAction)
        {
            _undoStack.Push(undoAction);
            _redoStackDraft.Push(redoAction);

            // Clear the redo stack
            _redoStack.Clear();
        }

        // ヘルパーメソッド: 指定した型の先祖を見つける
        private static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T)
                {
                    return (T)current;
                }
                current = VisualTreeHelper.GetParent(current);
            }
            return null;
        }
        private void ListBox_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point endPoint = e.GetPosition(null);

            // ドラッグが発生していない場合にのみ選択を行う
            if (Math.Abs(_startPoint.X - endPoint.X) <= SystemParameters.MinimumHorizontalDragDistance &&
                Math.Abs(_startPoint.Y - endPoint.Y) <= SystemParameters.MinimumVerticalDragDistance)
            {
                ListBox listBox = sender as ListBox;
                if (listBox != null)
                {
                    ListBoxItem listBoxItem = FindAncestor<ListBoxItem>((DependencyObject)e.OriginalSource);
                    if (listBoxItem != null)
                    {
                        // アイテムが選択されていない場合は選択する
                        if (!listBox.SelectedItems.Contains(listBoxItem))
                        {
                            listBox.SelectedItems.Clear();
                            listBox.SelectedItem = listBoxItem;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Event handler called when an item in the right column is clicked.
        /// </summary>
        private void RightColumn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (RightColumn.SelectedItem is ListBoxItem selectedItem)
            {
                // Path of the selected item in the right column
                string? selectedItemPath = selectedItem.Tag?.ToString();

                // If the item has a path
                if (selectedItemPath != null)
                {
                    // old center column path for stack
                    string? oldPath = CenterColumnPath;

                    // new right column path
                    string? newRightColumnPath = Directory.GetParent(RightColumnPath)?.FullName;

                    // Move the content of the right column to the center column
                    MoveItemsLeft();

                    // Select the clicked item in the center column
                    SelectItemInColumn(CenterColumn, selectedItemPath);

                    // Focus on the center column
                    CenterColumn.Focus();

                    // Push the old center column path to the stack
                    PushToPreviousDirectories(oldPath);
                }
            }
            // if space in the right column is clicked
            else
            {
                // Move the content of the right column to the center column
                MoveItemsLeft();
            }
        }
        /// <summary>
        /// Event handler called when the right column label is clicked.
        /// Loads the content of the right column path.
        /// </summary>
        private void RightColumnLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoadAllContent(RightColumnPath);
        }

        /// <summary>
        /// Event handler called when the center column label is clicked.
        /// Loads the content of the center column path.
        /// </summary>
        private void CenterColumnLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoadAllContent(CenterColumnPath);
        }

        /// <summary>
        /// Event handler called when the left column label is clicked.
        /// Loads the content of the left column path.
        /// </summary>
        private void LeftColumnLabel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            LoadAllContent(LeftColumnPath);
        }
        private void ClearAllColumns()
        {
            LeftColumnLabel.Text = string.Empty;
            LeftColumn.Items.Clear();
            CenterColumnLabel.Text = string.Empty;
            CenterColumn.Items.Clear();
            RightColumnLabel.Text = string.Empty;
            RightColumn.Items.Clear();
        }

        /// <summary>
        /// カットしたアイテムの文字色を変更します。
        /// </summary>
        /// <param name="listBoxItem">カットしたアイテム。</param>
        private void ChangeCutItemColor(ListBoxItem listBoxItem)
        {
            string? path = listBoxItem.Tag?.ToString();
            if (path != null)
            {
                if (Directory.Exists(path))
                {
                    listBoxItem.Foreground = Brushes.LightBlue; // フォルダーの場合は水色
                }
                else if (File.Exists(path))
                {
                    listBoxItem.Foreground = Brushes.Gray; // ファイルの場合は灰色
                }
            }
        }

        /// <summary>
        /// カットしたアイテムの文字色を元に戻します。
        /// </summary>
        /// <param name="listBoxItem">カットしたアイテム。</param>
        private void ResetCutItemColor(ListBoxItem listBoxItem)
        {
            listBoxItem.Foreground = Brushes.Black; // 元の色に戻す
        }

    }
}
