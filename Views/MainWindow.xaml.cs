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

        /// <summary>
        /// ホームディレクトリを中央カラムに表示する。
        /// </summary>
        private void LoadHomeDirectory()
        {
            LoadAllContent(_homeDirectory, null);
        }

        /// <summary>
        /// 指定されたパスの内容を各カラムに読み込み、選択状態を設定する。
        /// </summary>
        /// <param name="path">読み込むディレクトリのパス。</param>
        /// <param name="selectedItem">選択するアイテムのパス（null の場合は1つ目のアイテムを選択）。</param>
        private void LoadAllContent(string path, string selectedItem)
        {
            Column1.Items.Clear();
            string parentDirectory = null;
            if (path != null)
            {
                parentDirectory = Directory.GetParent(path)?.FullName;
            }

            if (parentDirectory == null && path != null)
            {
                // Column1にドライブのリストを表示
                foreach (var drive in DriveInfo.GetDrives())
                {
                    if (drive.IsReady)
                    {
                        Column1.Items.Add(CreateListBoxItem(drive.Name, drive.Name, isDirectory: true));
                    }
                }
            }
            if(parentDirectory != null)
            {
                // Column1にpathの親ディレクトリを表示
                // フォルダーをカラム1に追加
                foreach (var dir in Directory.GetDirectories(parentDirectory))
                {
                    Column1.Items.Add(CreateListBoxItem(Path.GetFileName(dir), dir, isDirectory: true));
                }
                // カラム1にあるカレントディレクトリのアイテムを選択状態にする
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
                if (path == null)
                {
                    // カラム2にディレクトリのリストを表示
                    foreach (var drive in DriveInfo.GetDrives())
                    {
                        if (drive.IsReady)
                        {
                            Column2.Items.Add(CreateListBoxItem(drive.Name, drive.Name, isDirectory: true));
                        }
                    }
                }
                else
                {
                    // ディレクトリに存在するフォルダーをカラム2に追加
                    foreach (var dir in Directory.GetDirectories(path))
                    {
                        Column2.Items.Add(CreateListBoxItem(Path.GetFileName(dir), dir, isDirectory: true));
                    }

                    // ディレクトリに存在するファイルをカラム2に追加
                    foreach (var file in Directory.GetFiles(path))
                    {
                        Column2.Items.Add(CreateListBoxItem(Path.GetFileName(file), file, isDirectory: false));
                    }
                }

                // カラム3に選択されたアイテムの中身を表示
                Column3.Items.Clear();
                if (Directory.Exists(selectedItem))
                {
                    foreach (var dir in Directory.GetDirectories(selectedItem))
                    {
                        Column3.Items.Add(CreateListBoxItem(Path.GetFileName(dir), dir, isDirectory: true));
                    }

                    foreach (var file in Directory.GetFiles(selectedItem))
                    {
                        Column3.Items.Add(CreateListBoxItem(Path.GetFileName(file), file, isDirectory: false));
                    }
                }

                // カラム2にあるカレントディレクトリのアイテムを選択状態にする
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

                // 中央カラムにフォーカスを設定
                Column2.Focus();

                // 一度下キーを押したときの処理を実行
                var keyEvent = new KeyEventArgs(Keyboard.PrimaryDevice, PresentationSource.FromVisual(this), 0, Key.Down)
                { RoutedEvent = Keyboard.KeyDownEvent };
                InputManager.Current.ProcessInput(keyEvent);
            }
            catch
            {
                DisplayErrorMessage("アクセスできないフォルダまたはファイルがあります。");
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
                        DisplayErrorMessage("アクセスできないフォルダまたはファイルがあります。");
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

            // 中央カラムにフォーカスを設定
            Column2.Focus();

            // 選択されたアイテムにフォーカスを設定
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

            // 中央カラムにフォーカスを設定
            Column2.Focus();
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

        /// <summary>
        /// 下のディレクトリに移動する。
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
        /// 親ディレクトリに移動する。
        /// </summary>
        private void MoveToParentDirectory()
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                // 選択されていたアイテムのパスを取得
                string itemPath = selectedItem.Tag.ToString();
                string currentDirectory = itemPath;
                // アイテムがディレクトリである場合、それがあるディレクトリのパスをカレントディレクトリとする
                if (Directory.Exists(itemPath))
                {
                    currentDirectory = Directory.GetParent(itemPath)?.FullName;
                }
                // 移動先の親ディレクトリを取得
                string parentDirectory = Directory.GetParent(currentDirectory)?.FullName;
                //if (parentDirectory != null)
                //{
                    LoadAllContent(parentDirectory, currentDirectory);
                //}
            }
        }

        /// <summary>
        /// エラーメッセージをカラム3に表示する。
        /// </summary>
        /// <param name="message">表示するエラーメッセージ。</param>
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
