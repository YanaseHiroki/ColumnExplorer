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
            Column2.SelectedIndex = 0;
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
                    ContentLoader.loadItems(Column1, parentDirectory);
                    Column1Label.Text = GetLabel(parentDirectory);
                }
                if (Column1.Items.Count > 0 && path != null)
                {
                    // 遷移先パスをカラム1で選択状態にする
                    SelectItemInColumn(Column1, path);
                }
            }

            // カラム2
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
                Column2Label.Text = GetLabel(path);
            }
            // アイテムを選択状態にする
            if (Column2.Items.Count > 0)
            {
                SelectItemInColumn(Column2, selectedItem);
            }

            // カラム3
            // フォルダーの内容を表示
            DirectoryHelper.LoadDirectoryContent(Column3, selectedItem);
            Column3Label.Text = selectedItem != null ? GetLabel(selectedItem) : string.Empty;
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
                string? itemPath = selectedItem.Tag?.ToString();
                if (string.IsNullOrEmpty(itemPath)) return;

                // 遷移前と遷移先のディレクトリ
                string? currentDirectory = Directory.GetParent(itemPath)?.FullName;
                string? parentDirectory = currentDirectory == null ? null :
                    Directory.GetParent(currentDirectory)?.FullName;

                // これ以上、上のディレクトリがない場合
                if (currentDirectory == null || parentDirectory == null)
                {
                    // ドライブのリストをカラム2に表示
                    ContentLoader.AddDrives(Column2);
                    Column2Label.Text = DRIVE_LABEL;
                    Column2.SelectedIndex = 0;
                    Column2.Focus();
                    // カラム1の内容を消去
                    Column1.Items.Clear();
                    Column1Label.Text = string.Empty;
                    return;
                }

                // カラム3にカラム2の内容を移動
                MoveItems(Column2, Column3);
                Column3Label.Text = Column2Label.Text;

                // カラム2にカラム1の内容を移動
                MoveItems(Column1, Column2);
                Column2Label.Text = Column1Label.Text;

                if (parentDirectory != null)
                {
                    // カラム1に表示するディレクトリ
                    string? grandParentDirectory = Directory.GetParent(parentDirectory)?.FullName;

                    if (grandParentDirectory == null)
                    {
                        // ドライブリストを表示
                        ContentLoader.AddDrives(Column1);
                        Column1Label.Text = DRIVE_LABEL;
                    }
                    else
                    {
                        // 祖父母ディレクトリの内容を表示
                        ContentLoader.loadItems(Column1, grandParentDirectory);
                        Column1Label.Text = GetLabel(grandParentDirectory);
                    }
                }
            }
        }
        private string GetLabel(string path)
        {
            return (path == Path.GetPathRoot(path))
                ? path
                : Path.GetFileName(path);
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

        /// <summary>
        /// アイテムを一つのListBoxから別のListBoxに移動します。
        /// </summary>
        /// <param name="source">移動元のListBox。</param>
        /// <param name="destination">移動先のListBox。</param>
        private void MoveItems(ListBox source, ListBox destination)
        {
            destination.Items.Clear();
            foreach (var item in source.Items)
            {
                var clonedItem = new ListBoxItem
                {
                    Content = ((ListBoxItem)item).Content,
                    Tag = ((ListBoxItem)item).Tag
                };
                destination.Items.Add(clonedItem);
            }
        }
    }
}
