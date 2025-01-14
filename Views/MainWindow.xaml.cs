using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ColumnExplorer.Helpers;

namespace ColumnExplorer.Views
{
    public partial class MainWindow : Window
    {
        private bool _homeLoaded;
        private string _homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_homeLoaded)
            {
                LoadHomeDirectory();
                _homeLoaded = true;
            }
        }

        private void LoadHomeDirectory()
        {
            LoadAllContent(_homeDirectory, null);
        }

        private void LoadAllContent(string path, string selectedItem)
        {
            Column1.Items.Clear();
            string parentDirectory = Directory.GetParent(path)?.FullName;

            if (parentDirectory == null && path != null)
            {
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady)
                    {
                        Column1.Items.Add(DirectoryHelper.CreateListBoxItem(drive.Name, drive.Name, true));
                    }
                }
            }
            else if (parentDirectory != null)
            {
                foreach (var dir in Directory.GetDirectories(parentDirectory))
                {
                    Column1.Items.Add(DirectoryHelper.CreateListBoxItem(Path.GetFileName(dir), dir, true));
                }

                foreach (var file in Directory.GetFiles(parentDirectory))
                {
                    Column1.Items.Add(DirectoryHelper.CreateListBoxItem(Path.GetFileName(file), file, false));
                }
            }

            Column2.Items.Clear();
            DirectoryHelper.LoadDirectoryContent(Column2, path, true);

            Column3.Items.Clear();
            DirectoryHelper.LoadDirectoryContent(Column3, selectedItem, true);

            if (Column2.Items.Count > 0)
            {
                Column2.SelectedIndex = selectedItem != null ? Column2.Items.IndexOf(selectedItem) : 0;
            }

            Column2.Focus();
            var keyEvent = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Down)
            { RoutedEvent = Keyboard.KeyDownEvent };
            InputManager.Current.ProcessInput(keyEvent);
        }

        private void Column2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EventHandlers.Column2_SelectionChanged(sender, e, Column3);
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);
            Column2.Focus();

            if (Column2.SelectedItem is ListBoxItem selectedListBoxItem)
            {
                selectedListBoxItem.Focus();
            }

            if (e.Key == Key.Right)
            {
                MoveToSubDirectory();
            }
            else if (e.Key == Key.Left)
            {
                MoveToParentDirectory();
            }
            else if (e.Key == Key.Up)
            {
                SelectUpperItem();
            }
            else if (e.Key == Key.Down)
            {
                SelectLowerItem();
            }
        }

        private void SelectLowerItem()
        {
            if (Column2.SelectedIndex < Column2.Items.Count - 1)
            {
                Column2.SelectedIndex++;
            }
        }

        private void SelectUpperItem()
        {
            if (Column2.SelectedIndex > 0)
            {
                Column2.SelectedIndex--;
            }
        }

        private void MoveToSubDirectory()
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string path = selectedItem.Tag.ToString();
                if (Directory.Exists(path))
                {
                    LoadAllContent(path, null);
                }
            }
        }

        private void MoveToParentDirectory()
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string itemPath = selectedItem.Tag.ToString();
                string currentDirectory = Directory.GetParent(itemPath)?.FullName;
                if (currentDirectory == null) return;
                string parentDirectory = Directory.GetParent(currentDirectory)?.FullName;
                LoadAllContent(parentDirectory, currentDirectory);
            }
        }

        private void DisplayErrorMessage(string message)
        {
            Column3.Items.Clear();
            var errorMessageItem = new ListBoxItem
            {
                Content = message,
                Foreground = Brushes.Red
            };
            Column3.Items.Add(errorMessageItem);
        }
    }
}
