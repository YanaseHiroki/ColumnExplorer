using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColumnExplorer.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadHomeDirectory();
        }

        private void LoadHomeDirectory()
        {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            LoadContent(homeDirectory, Column2);

            string parentDirectory = Directory.GetParent(homeDirectory)?.FullName;
            if (parentDirectory != null)
            {
                LoadContent(parentDirectory, Column1);
            }
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

        private void Column2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string path = selectedItem.Tag.ToString();
                if (Directory.Exists(path))
                {
                    LoadContent(path, Column3);
                }
                else
                {
                    Column3.Items.Clear();
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (e.Key == Key.Right)
            {
                MoveToSubDirectory();
            }
            else if (e.Key == Key.Left)
            {
                MoveToParentDirectory();
            }
        }

        private void MoveToSubDirectory()
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string path = selectedItem.Tag.ToString();
                if (Directory.Exists(path))
                {
                    // 中央カラムの内容を左カラムに移動
                    LoadContent(path, Column2);

                    // 左カラムに親ディレクトリを表示
                    string parentDirectory = Directory.GetParent(path)?.FullName;
                    if (parentDirectory != null)
                    {
                        LoadContent(parentDirectory, Column1);
                    }
                    else
                    {
                        Column1.Items.Clear();
                    }

                    // 右カラムをクリア
                    Column3.Items.Clear();
                }
            }
        }

        private void MoveToParentDirectory()
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string path = selectedItem.Tag.ToString();
                string parentDirectory = Directory.GetParent(path)?.FullName;
                if (parentDirectory != null)
                {
                    // 中央カラムの内容を左カラムに移動
                    LoadContent(parentDirectory, Column2);

                    // 左カラムにさらに上の親ディレクトリを表示
                    string grandParentDirectory = Directory.GetParent(parentDirectory)?.FullName;
                    if (grandParentDirectory != null)
                    {
                        LoadContent(grandParentDirectory, Column1);
                    }
                    else
                    {
                        Column1.Items.Clear();
                    }

                    // 右カラムをクリア
                    Column3.Items.Clear();
                }
            }
        }
    }
}
