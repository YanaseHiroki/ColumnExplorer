using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColumnExplorer.Views
{
    public partial class MainWindow : Window
    {
        private bool _homeLoaded;

        public MainWindow()
        {
            InitializeComponent();
            if (!_homeLoaded)
            {
                LoadHomeDirectory();
                _homeLoaded = true;
            }
        }

        /// <summary>
        /// ホームディレクトリを中央カラムに表示する。
        /// </summary>
        private void LoadHomeDirectory()
        {
            string homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            LoadAllContent(homeDirectory, null);

            // 起動後に上下キーでアイテムを選択できるようにフォーカスを設定
            Column2.Focus();
        }

        /// <summary>
        /// 指定されたパスの内容を各カラムに読み込み、選択状態を設定する。
        /// </summary>
        /// <param name="path">読み込むディレクトリのパス。</param>
        /// <param name="selectedItem">選択するアイテムのパス（null の場合は1つ目のアイテムを選択）。</param>
        private void LoadAllContent(string path, string selectedItem)
        {
            // Column1にpathの親ディレクトリを表示
            string parentDirectory = Directory.GetParent(path)?.FullName;

            if (parentDirectory != null)
            {
                Column1.Items.Clear();
                // ディレクトリに存在するフォルダーをカラム1に追加
                foreach (var dir in Directory.GetDirectories(parentDirectory))
                {
                    Column1.Items.Add(CreateListBoxItem(Path.GetFileName(dir), dir, isDirectory: true));
                }
                // カラム1に追加したカレントディレクトリを選択状態にする
                if (Column1.Items.Count > 0)
                {
                    foreach (ListBoxItem item in Column1.Items)
                    {
                        if (item.Tag.ToString() == path)
                        {
                            Column1.SelectedItem = item;
                            break;
                        }
                    }
                }
                // ディレクトリに存在するファイルをカラム1に追加
                foreach (var file in Directory.GetFiles(parentDirectory))
                {
                    Column1.Items.Add(CreateListBoxItem(Path.GetFileName(file), file, isDirectory: false));
                }
            }

            // Column2にpathのディレクトリを表示
            Column2.Items.Clear();
            try
            {
                foreach (var dir in Directory.GetDirectories(path))
                {
                    Column2.Items.Add(CreateListBoxItem(Path.GetFileName(dir), dir, isDirectory: true));
                }

                foreach (var file in Directory.GetFiles(path))
                {
                    Column2.Items.Add(CreateListBoxItem(Path.GetFileName(file), file, isDirectory: false));
                }
            }
            catch
            {
                MessageBox.Show("アクセスできないフォルダやファイルがあります。");
            }

            if (Column2.Items.Count > 0)
            {
                if (selectedItem != null)
                {
                    foreach (ListBoxItem item in Column2.Items)
                    {
                        if (item.Tag.ToString() == selectedItem)
                        {
                            Column2.SelectedItem = item;
                            break;
                        }
                    }
                }
                else
                {
                    Column2.SelectedIndex = 0;
                }
            }
        }

        /// <summary>
        /// ListBoxItemを作成する。
        /// </summary>
        /// <param name="displayName">表示名。</param>
        /// <param name="path">アイテムのパス。</param>
        /// <param name="isDirectory">ディレクトリかどうか。</param>
        /// <returns>作成されたListBoxItem。</returns>
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

        /// <summary>
        /// 中央カラムの選択が変更されたときに右カラムに内容を表示する。
        /// </summary>
        private void Column2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string path = selectedItem.Tag.ToString();
                Column3.Items.Clear();
                if (Directory.Exists(path))
                {
                    // Column3にpathのディレクトリを表示
                    try
                    {
                        foreach (var dir in Directory.GetDirectories(path))
                        {
                            Column3.Items.Add(CreateListBoxItem(Path.GetFileName(dir), dir, isDirectory: true));
                        }

                        foreach (var file in Directory.GetFiles(path))
                        {
                            Column3.Items.Add(CreateListBoxItem(Path.GetFileName(file), file, isDirectory: false));
                        }
                    }
                    catch
                    {
                        MessageBox.Show("アクセスできないフォルダやファイルがあります。");
                    }
                }
            }
        }

        /// <summary>
        /// キーが押されたときの処理を行う。
        /// </summary>
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

        /// <summary>
        /// 中央カラムで選択されているフォルダーに移動する。
        /// </summary>
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

        /// <summary>
        /// 中央カラムの親ディレクトリに移動する。
        /// </summary>
        private void MoveToParentDirectory()
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string path = selectedItem.Tag.ToString();
                string parentDirectory = Directory.GetParent(path)?.FullName;
                if (parentDirectory != null)
                {
                    LoadAllContent(parentDirectory, path);
                }
            }
        }
    }
}
