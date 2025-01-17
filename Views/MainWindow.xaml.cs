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
        // ドライブリストを表す文字列
        private const string DRIVE = "Drive";
        // 選択されたアイテムを表す文字列
        private const string SELECTED_ITEMS = "Selected Items";
        // ホームディレクトリ
        private static string _homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private string Column1Path = string.Empty;
        private string Column2Path = string.Empty;
        private string Column3Path = string.Empty;

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
            LoadAllContent(_homeDirectory);
        }

        /// <summary>
        /// 指定されたパスの内容を各カラムに読み込みます。
        /// </summary>
        private void LoadAllContent(string path)
        {
            try
            {
                Column2Path = path;

                // 中央のパスがない場合、左：なし、中央：ドライブリスト、右：選択されたアイテム
                if (string.IsNullOrEmpty(Column2Path))
                {
                    // 中央にドライブリストを表示
                    ContentLoader.AddDrives(Column2);
                    Column2Label.Text = DRIVE;
                }
                // 中央のパスがある場合、左：親ディレクトリ、中央：パスの内容、右：選択されたアイテム
                else
                {
                    // 中央にフォルダーの内容を表示
                    DirectoryHelper.LoadDirectoryContent(Column2, Column2Path);
                    Column2Label.Text = GetLabel(Column2Path);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in LoadAllContent: {ex.Message}");
                throw;
            }

            // 中央でアイテムを選択状態にする
            SelectItemInColumn(Column2, Column3Path);


            // 左カラム
            if (Column2Path != string.Empty)
            {
                // 中央カラムの親ディレクトリを取得して左カラムのパスとする
                Column1Path = Directory.GetParent(Column2Path)?.FullName;
                Column1Path = (Column1Path == null) ? string.Empty : Column1Path;

                if (string.IsNullOrEmpty(Column1Path))
                {
                    // ドライブリストを表示
                    ContentLoader.AddDrives(Column1);
                    Column1Label.Text = DRIVE;
                }
                else
                {
                    // フォルダーの内容を表示
                    DirectoryHelper.LoadDirectoryContent(Column1, Column1Path);
                    Column1Label.Text = GetLabel(Column1Path);
                }

                if (Column1.Items.Count > 0 && Column2Path != null)
                {
                    // 中央のパスを左カラムで選択状態にする
                    SelectItemInColumn(Column1, Column2Path);
                }
            }


            // 中央で選択されたアイテムがフォルダーであれば、その内容を右カラムに表示
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string? itemPath = selectedItem.Tag?.ToString();
                if (itemPath != null && Directory.Exists(itemPath))
                {
                    DirectoryHelper.LoadDirectoryContent(Column3, itemPath);
                    Column3Label.Text = GetLabel(itemPath);
                }
            }

            // カラム2のアイテムにフォーカス
            Column2.Focus();
        }

        /// <summary>
        /// 指定されたパスに一致するアイテムをカラム内で選択状態にします。
        /// </summary>
        /// <param name="column">アイテムを選択する対象のListBox。</param>
        /// <param name="path">選択するアイテムのパス。</param>
        private void SelectItemInColumn(ListBox column, string path)
        {
            //　選択対象のパスがない場合、1つ目のアイテムを選択
            if (string.IsNullOrEmpty(path))
            {
                column.SelectedIndex = 0;
                return;
            }

            // 対象カラムに候補のアイテムがあれば、選択するアイテムを検索
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
            }
        }
        private void Column1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Column1.SelectedItem is ListBoxItem selectedItem)
            {
                string? itemPath = selectedItem.Tag?.ToString();
                if (itemPath != null && Directory.Exists(itemPath))
                {
                    // TODO：中央カラムのパスを更新して全体の内容を読み込み直す
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
        private void Column3_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Column3.SelectedItem is ListBoxItem selectedItem)
            {
                string? itemPath = selectedItem.Tag?.ToString();
                if (itemPath != null && Directory.Exists(itemPath))
                {
                    // TODO：中央カラムのパスを更新して全体の内容を読み込み直す
                }
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
        /// 右キーが押されたとき中央カラムでディレクトリが選択されていれば、そこに移動します。
        /// </summary>
        private void MoveToSubDirectory()
        {
            if (Column2.SelectedItem is ListBoxItem selectedItem)
            {
                string? path = selectedItem.Tag?.ToString();
                if (path != null && Directory.Exists(path))
                {
                    LoadAllContent(path);
                }
            }
        }

        /// <summary>
        /// 右キーが押されたとき親ディレクトリがあれば移動します。
        /// </summary>
        private void MoveToParentDirectory()
        {
            // 左カラムに表示されているか
            if (Directory.Exists(Column1Path) || string.Equals(Column1Path, DRIVE))
            {
                string? newColumn1Path = Directory.GetParent(Column1Path)?.FullName;
                // アイテムを右にずらして、左カラムに親ディレクトリの内容を表示
                MoveItemsRignt(newColumn1Path);
            }
        }

        /// <summary>
        /// 中央カラムの内容を右カラムに移動し、左カラムの内容を中央カラムに移動します。
        /// その後、親ディレクトリの内容を左カラムに表示します。
        /// </summary>
        /// <param name="parentDirectory">左カラムに表示するディレクトリのパス。</param>
        private void MoveItemsRignt(string? newColumn1Path)
        {
            // 右カラムに中央カラムの内容を移動
            MoveItems(Column2, Column3);
            Column3Label.Text = Column2Label.Text;
            Column3Path = Column2Path;

            // 中央カラムに左カラムの内容を移動
            MoveItems(Column1, Column2);
            Column2Label.Text = Column1Label.Text;
            Column2Path = Column1Path;

            // 左カラムに表示
            if (string.Equals(Column2Path, DRIVE))
            {
                // 表示をクリアする
                Column2.Items.Clear();
                Column2Path = string.Empty;
                Column2Path = string.Empty;
            }
            else if (!string.Equals(Column2Path, DRIVE) && string.IsNullOrEmpty(newColumn1Path))
            {
                // ドライブリストを表示
                ContentLoader.AddDrives(Column1);
                Column1Label.Text = DRIVE;
                Column1Path = DRIVE;
            }
            else
            {
                // ディレクトリの内容を表示
                DirectoryHelper.LoadDirectoryContent(Column1, newColumn1Path);
                Column1Label.Text = GetLabel(newColumn1Path);
                Column1Path = newColumn1Path;
            }
            // 右カラムのパスを中央カラムで選択
            if (!string.IsNullOrEmpty(Column3Path))
            {
                SelectItemInColumn(Column2, Column3Path);
            }
            Column2.Focus();
        }

        /// <summary>
        /// 指定されたパスに基づいてラベルを取得します。
        /// </summary>
        /// <param name="path">ラベルを取得するパス。</param>
        /// <returns>パスに基づくラベル。</returns>
        private string GetLabel(string path)
        {
            return (path == Path.GetPathRoot(path) || path == DRIVE)
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
                }
            }
        }

        /// <summary>
        /// 中央カラムで選択されたアイテムを右カラムに表示します。
        /// </summary>
        private void UpdateColumn3WithSelectedItems()
        {
            // 右カラムのアイテムを消去
            Column3.Items.Clear();
            // 右カラムのラベルを更新
            Column3Label.Text = SELECTED_ITEMS;
            // 中央カラムで選択されたアイテムを右カラムに表示
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
            // 右カラムのアイテムをすべて選択状態にする
            Column3.SelectAll();
        }

        /// <summary>
        /// アイテムを一つのListBoxから別のListBoxに移動します。
        /// </summary>
        /// <param name="source">移動元のListBox。</param>
        /// <param name="destination">移動先のListBox。</param>
        private void MoveItems(ListBox source, ListBox destination)
        {
            // 移動先のアイテムを消去
            destination.Items.Clear();

            // 移動先の選択アイテム情報を消去
            destination.SelectedItems.Clear();

            // アイテムを移動先にコピー
            foreach (var item in source.Items)
            {
                var clonedItem = new ListBoxItem
                {
                    Content = ((ListBoxItem)item).Content,
                    Tag = ((ListBoxItem)item).Tag
                };
                destination.Items.Add(clonedItem);
            }

            // 選択アイテム情報をコピー
            foreach (var item in source.SelectedItems)
            {
                destination.SelectedItems.Add(item);
            }
            // 移動元のアイテムを消去
            source.Items.Clear();
        }
    }
}
