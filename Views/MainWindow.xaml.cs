using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColumnExplorer.Views
{
    public partial class MainWindow : Window
    {
        private string _previouslyOpenedFolder; // 直前に開いたフォルダのパスを保持

        public MainWindow()
        {
            InitializeComponent();
            LoadHomeDirectory();
        }

        private void LoadHomeDirectory()
        {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            LoadContent(homeDirectory, Column2);

            var root = Directory.GetDirectoryRoot(homeDirectory);
            Column1.Items.Add(CreateListBoxItem(root, root, isDirectory: true));
        }

        private void LoadContent(string path, ListBox targetColumn)
        {
            targetColumn.Items.Clear();
            try
            {
                foreach (var dir in Directory.GetDirectories(path))
                {
                    targetColumn.Items.Add(CreateListBoxItem(Path.GetFileName(dir), dir, isDirectory: true));
                }

                foreach (var file in Directory.GetFiles(path))
                {
                    targetColumn.Items.Add(CreateListBoxItem(Path.GetFileName(file), file, isDirectory: false));
                }
            }
            catch
            {
                MessageBox.Show("アクセスできないフォルダやファイルがあります。");
            }

            if (targetColumn.Items.Count > 0)
            {
                targetColumn.SelectedIndex = 0;
            }
        }

        private ListBoxItem CreateListBoxItem(string displayName, string path, bool isDirectory)
        {
            return new ListBoxItem
            {
                Content = displayName,
                Tag = path,
                FontWeight = isDirectory ? FontWeights.Bold : FontWeights.Normal,
                Foreground = isDirectory ? Brushes.Blue : Brushes.Black
            };
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Right)
            {
                OpenSelectedFolderAndSelectFirstItem();
            }
            else if (e.Key == Key.Left)
            {
                NavigateToLeftColumnAndSelectPreviousFolder();
            }
        }
        private void OpenSelectedFolderAndSelectFirstItem()
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string path = selectedItem.Tag.ToString();
                if (Directory.Exists(path))
                {
                    _previouslyOpenedFolder = path; // 開いたフォルダのパスを記録
                    LoadContent(path, Column3);

                    // 右カラムで最初のアイテムを即座に選択状態にする
                    if (Column3.Items.Count > 0)
                    {
                        Column3.SelectedIndex = 0; // 最初のアイテムを選択
                        Column3.Focus(); // 右カラム全体にフォーカスを設定
                    }
                }
            }
        }


        private void NavigateToLeftColumnAndSelectPreviousFolder()
        {
            // 中央カラムがアクティブの場合
            if (Column3.SelectedItem != null)
            {
                Column3.Items.Clear(); // 右カラムをクリア
                RestorePreviousSelection(Column2, _previouslyOpenedFolder);
                Column2.Focus(); // 中央カラムにフォーカスを戻す
            }
            else if (Column2.SelectedItem != null)
            {
                Column2.Items.Clear(); // 中央カラムをクリア
                RestorePreviousSelection(Column1, _previouslyOpenedFolder);
                Column1.Focus(); // 左カラムにフォーカスを戻す
            }
        }

        private void RestorePreviousSelection(ListBox column, string folderPath)
        {
            foreach (ListBoxItem item in column.Items)
            {
                if (item.Tag.ToString() == folderPath)
                {
                    column.SelectedItem = item;
                    break;
                }
            }
        }

        private void Column1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Column1.SelectedItem is ListBoxItem selectedItem)
            {
                string path = selectedItem.Tag.ToString();
                if (Directory.Exists(path))
                {
                    LoadContent(path, Column2);
                    Column3.Items.Clear();
                }
            }
        }

        private void Column2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string path = selectedItem.Tag.ToString();
                if (Directory.Exists(path))
                {
                    LoadContent(path, Column3);
                }
            }
        }
    }
}
