using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using ColumnExplorer.Helpers;

namespace ColumnExplorer.Views
{
    /// <summary>
    /// MainWindowクラスは、アプリケーションのメインウィンドウを表します。
    /// </summary>
    public partial class MainWindow : Window
    {
        private const string DRIVE_LABEL = "Drive";
        private string _homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        /// <summary>
        /// MainWindowクラスの新しいインスタンスを初期化します。
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        /// <summary>
        /// ウィンドウがロードされたときに呼び出されるイベントハンドラー。
        /// </summary>
        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadHomeDirectory();
        }

        /// <summary>
        /// ホームディレクトリの内容を読み込みます。
        /// </summary>
        private void LoadHomeDirectory()
        {
            LoadAllContent(_homeDirectory, null);
            Column2.Focus();
        }

        /// <summary>
        /// 指定されたパスの内容を各カラムに読み込みます。
        /// </summary>
        /// <param name="path">読み込むディレクトリのパス（null の場合はドライブリストを表示）。</param>
        /// <param name="selectedItem">選択するアイテムのパス（null の場合は1つ目のアイテムを選択）。</param>
        private void LoadAllContent(string? path, string selectedItem)
        {

            // カラム1
            Column1.Items.Clear();
            if (path != null)
            {
                // 親ディレクトリを取得
                string? parentDirectory = Directory.GetParent(path)?.FullName;

                if (parentDirectory == null)
                {
                    // ドライブリストを表示
                    ContentLoader.AddDrives(Column1);
                    Column1Label.Text = DRIVE_LABEL;
                }
                else
                {
                    // フォルダーの内容を表示
                    ContentLoader.AddDirectories(Column1, parentDirectory);
                    ContentLoader.AddFiles(Column1, parentDirectory);
                    Column1Label.Text = Path.GetFileName(parentDirectory);
                }
                if (Column1.Items.Count > 0 && path != null)
                {
                    // 遷移先パスをカラム1で選択状態にする
                    SelectItemInColumn(Column1, path);
                }
            }

            // カラム2
            Column2.Items.Clear();
            if (path == null)
            {
                // ドライブリストを表示
                ContentLoader.AddDrives(Column2);
                Column2Label.Text = DRIVE_LABEL;
            }
            else
            {
                // フォルダーの内容を表示
                DirectoryHelper.LoadDirectoryContent(Column2, path);
                Column2Label.Text = path == Path.GetPathRoot(path) ? path : Path.GetFileName(path);
            }
            // アイテムを選択状態にする
            if (Column2.Items.Count > 0)
            {
                SelectItemInColumn(Column2, selectedItem);
            }

            // カラム3
            Column3.Items.Clear();
            // フォルダーの内容を表示
            DirectoryHelper.LoadDirectoryContent(Column3, selectedItem);
            Column3Label.Text = selectedItem != null ? Path.GetFileName(selectedItem) : string.Empty;
        }


        /// <summary>
        /// 指定されたパスに一致するアイテムをカラム内で選択状態にします。
        /// </summary>
        /// <param name="column">アイテムを選択する対象のListBox。</param>
        /// <param name="path">選択するアイテムのパス。</param>
        private void SelectItemInColumn(ListBox column, string path)
        {
            if (column.Items.Count > 0 && path != null)
            {
                foreach (var item in column.Items)
                {
                    if (item is ListBoxItem listBoxItem && listBoxItem.Tag.ToString() == path)
                    {
                        column.SelectedItem = listBoxItem;
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Column2の選択が変更されたときに呼び出されるイベントハンドラー。
        /// </summary>
        private void Column2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EventHandlers.Column2_SelectionChanged(sender, e, Column3);
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                Column3Label.Text = selectedItem.Content.ToString();
            }
        }

        /// <summary>
        /// キーが押されたときに呼び出されるイベントハンドラー。
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
            else if (e.Key == Key.Up)
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
            else if (e.Key == Key.Down)
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
            else if (e.Key == Key.Enter)
            {
                OpenSelectedItems();
            }
        }

        /// <summary>
        /// Column2の選択を次のアイテムに移動します。
        /// </summary>
        private void SelectLowerItem()
        {
            if (Column2.SelectedIndex < Column2.Items.Count - 1)
            {
                Column2.SelectedIndex++;
            }
        }

        /// <summary>
        /// Column2の選択を前のアイテムに移動します。
        /// </summary>
        private void SelectUpperItem()
        {
            if (Column2.SelectedIndex > 0)
            {
                Column2.SelectedIndex--;
            }
        }

        /// <summary>
        /// Shiftキーを押しながらColumn2の選択を次のアイテムに移動します。
        /// </summary>
        private void SelectLowerItemWithShift()
        {
            if (Column2.SelectedIndex < Column2.Items.Count - 1)
            {
                int nextIndex = Column2.SelectedIndex + 1;
                Column2.SelectedItems.Add(Column2.Items[nextIndex]);
                Column2.SelectedIndex = nextIndex;
                UpdateColumn3WithSelectedItems();
            }
        }

        /// <summary>
        /// Shiftキーを押しながらColumn2の選択を前のアイテムに移動します。
        /// </summary>
        private void SelectUpperItemWithShift()
        {
            if (Column2.SelectedIndex > 0)
            {
                int previousIndex = Column2.SelectedIndex - 1;
                Column2.SelectedItems.Add(Column2.Items[previousIndex]);
                Column2.SelectedIndex = previousIndex;
                UpdateColumn3WithSelectedItems();
            }
        }

        /// <summary>
        /// 選択されたディレクトリに移動します。
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
        /// 親ディレクトリに移動します。
        /// </summary>
        private void MoveToParentDirectory()
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string? itemPath = selectedItem.Tag.ToString();
                if (itemPath == null) return;
                string? currentDirectory = Directory.GetParent(itemPath)?.FullName;
                if (currentDirectory == null)
                {
                    // ルートディレクトリの場合、ドライブのリストを表示
                    LoadAllContent(null, null);
                }
                else
                {
                    string? parentDirectory = Directory.GetParent(currentDirectory)?.FullName;
                    LoadAllContent(parentDirectory, currentDirectory);
                }
            }
        }

        /// <summary>
        /// 選択されたアイテムを開きます。
        /// </summary>
        private void OpenSelectedItems()
        {
            foreach (var selectedItem in Column2.SelectedItems)
            {
                if (selectedItem is ListBoxItem listBoxItem)
                {
                    string? path = listBoxItem.Tag?.ToString();
                    if (path != null && File.Exists(path))
                    {
                        Process.Start(new ProcessStartInfo(path) { UseShellExecute = true });
                    }
                    else if (path != null && Directory.Exists(path))
                    {
                        LoadAllContent(path, null);
                    }
                    else if (Directory.Exists(path))
                    {
                        LoadAllContent(path, null);
                    }
                }
            }
        }

        /// <summary>
        /// エラーメッセージをColumn3に表示します。
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

        /// <summary>
        /// Column2で選択されたアイテムをColumn3に表示します。
        /// </summary>
        private void UpdateColumn3WithSelectedItems()
        {
            Column3.Items.Clear();
            foreach (var selectedItem in Column2.SelectedItems)
            {
                if (selectedItem is ListBoxItem listBoxItem)
                {
                    var newItem = new ListBoxItem
                    {
                        Content = listBoxItem.Content,
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1)
                    };
                    Column3.Items.Add(newItem);
                }
            }
        }
    }
}
