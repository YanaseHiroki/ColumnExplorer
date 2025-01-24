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
        internal static string _homeDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        internal string LeftColumnPath = string.Empty;
        internal string CenterColumnPath = string.Empty;
        internal string RightColumnPath = string.Empty;

        /// <summary>
        /// MainWindowクラスの新しいインスタンスを初期化します。
        /// </summary>
        public MainWindow()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            InitializeComponent();
            Loaded += MainWindow_Loaded;
            LeftColumn.MouseLeftButtonUp += LeftColumn_MouseLeftButtonUp;
            RightColumn.SelectionChanged += RightColumn_SelectionChanged;
        }

        /// <summary>
        /// 右カラムの選択が変更されたときに呼び出されるイベントハンドラー。
        /// </summary>
        private void RightColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RightColumn.SelectedItem is ListBoxItem selectedItem)
            {
                // 右カラムで選択されているアイテムのパス
                string? selectedItemPath = selectedItem.Tag?.ToString();

                // アイテムのパスがある場合、中央カラムで選択状態にする
                if (selectedItemPath != null)
                {
                    // 右カラムの内容が中央に来るように全体を左にずらす
                    MoveItemsLeft();

                    // クリックされたアイテムを中央カラムで選択状態にする
                    SelectItemInColumn(CenterColumn, selectedItemPath);
                }
            }
        }

        /// <summary>
        /// 左カラムのアイテムがクリックされたときに呼び出されるイベントハンドラー。
        /// </summary>
        private void LeftColumn_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (LeftColumn.SelectedItem is ListBoxItem selectedItem)
            {
                // 左カラムで選択されているアイテムのパス
                string? selectedItemPath = selectedItem.Tag?.ToString();

                // アイテムのパスがあり、中央カラムの表示パスと異なるか
                if (selectedItemPath != null
                    && !string.Equals(selectedItemPath, CenterColumnPath))
                {
                    // 選択されたアイテムの内容を中央カラムに表示
                    LoadAllContent(selectedItemPath);
                }
            }
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
        internal void LoadHomeDirectory()
        {
            LoadAllContent(_homeDirectory);
        }


        /// <summary> 
        /// 指定されたパスの内容を各カラムに読み込みます。
        /// </summary>
        internal void LoadAllContent(string path)
        {
            try
            {
                CenterColumnPath = path;

                // 中央のパスがない場合、左：なし、中央：ドライブリスト、右：選択されたアイテム
                if (string.IsNullOrEmpty(CenterColumnPath))
                {
                    // 中央にドライブリストを表示
                    ContentLoader.AddDrives(CenterColumn);
                    CenterColumnLabel.Text = DRIVE;
                }
                // 中央のパスがある場合、左：親ディレクトリ、中央：パスの内容、右：選択されたアイテム
                else
                {
                    // 中央にフォルダーの内容を表示
                    DirectoryHelper.LoadDirectoryContent(CenterColumn, CenterColumnPath);
                    CenterColumnLabel.Text = GetLabel(CenterColumnPath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in LoadAllContent: {ex.Message}");
                throw;
            }

            // 左カラム
            if (CenterColumnPath != string.Empty)
            {
                // 中央カラムの親ディレクトリを取得して左カラムのパスとする
                LeftColumnPath = Directory.GetParent(CenterColumnPath)?.FullName;
                LeftColumnPath = (LeftColumnPath == null) ? string.Empty : LeftColumnPath;

                if (string.IsNullOrEmpty(LeftColumnPath))
                {
                    // ドライブリストを表示
                    ContentLoader.AddDrives(LeftColumn);
                    LeftColumnLabel.Text = DRIVE;
                }
                else
                {
                    // フォルダーの内容を表示
                    DirectoryHelper.LoadDirectoryContent(LeftColumn, LeftColumnPath);
                    LeftColumnLabel.Text = GetLabel(LeftColumnPath);
                }

                if (LeftColumn.Items.Count > 0 && CenterColumnPath != null)
                {
                    // 中央のパスを左カラムで選択状態にする
                    SelectItemInColumn(LeftColumn, CenterColumnPath);
                }
            }

            // 中央カラムで1番目のアイテムを選択状態にする
            FocusSelectedItemInCenterColumn(string.Empty);
        }


        /// <summary>
        /// 中央カラムで選択状態のアイテムにフォーカスを当て、必要に応じて右カラムにその内容を表示します。
        /// </summary>
        /// <param name="targetItem">フォーカスを当てる対象アイテムのパス。nullまたは空の場合、右カラムのパスを使用します。</param>
        private void FocusSelectedItemInCenterColumn(string? targetItem)
        {
            // 引数の対象アイテムがない場合
            if (string.IsNullOrEmpty(targetItem))
            {
                // 右カラムのパスを中央で選択状態にする
                SelectItemInColumn(CenterColumn, RightColumnPath);
            }
            // 引数の対象アイテムがある場合
            else
            {
                // 対象アイテムを選択状態にする
                SelectItemInColumn(CenterColumn, targetItem);
            }

            // 中央で選択されたアイテムがフォルダーであれば、その内容を右カラムに表示
            if (CenterColumn.SelectedItem is ListBoxItem selectedItem)
            {
                string? itemPath = selectedItem.Tag?.ToString();
                if (itemPath != null && Directory.Exists(itemPath))
                {
                    // 右カラムにフォルダーの内容を表示
                    DirectoryHelper.LoadDirectoryContent(RightColumn, itemPath);
                    RightColumnLabel.Text = GetLabel(itemPath);
                    RightColumnPath = itemPath;
                }
            }

            // 中央カラムで選択状態のアイテムにフォーカスを当てる
            if (CenterColumn.SelectedItem is ListBoxItem selectedListBoxItem)
            {
                selectedListBoxItem.Focus();
            }
        }

        /// <summary>
        /// 指定されたパスに一致するアイテムをカラム内で選択状態にします。
        /// </summary>
        /// <param name="column">アイテムを選択する対象のListBox。</param>
        /// <param name="path">選択するアイテムのパス。</param>
        internal void SelectItemInColumn(ListBox column, string path)
        {
            //　選択対象のパスがない場合、1つ目のアイテムを選択
            if (string.IsNullOrEmpty(path))
            {
                column.SelectedIndex = 0;
                return;
            }
            else

            // 対象カラムに候補のアイテムがあるか検索
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

                // 検索して見つからない場合、1番目のアイテムを選択
                if (column.SelectedItem == null && column.Items.Count > 0)
                {
                    column.SelectedIndex = 0;
                }

            }
        }

        /// <summary>
        /// 中央カラムの選択が変更されたときに呼び出されるイベントハンドラー。
        /// </summary>
        private void CenterColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EventHandlers.CenterColumn_SelectionChanged(sender, e, RightColumn);
            if (CenterColumn.SelectedItem is ListBoxItem selectedItem)
            {
                RightColumnLabel.Text = selectedItem.Content.ToString();
                RightColumnPath = selectedItem.Tag?.ToString() ?? string.Empty;

                // テキストファイルの場合、右カラムにプレビューを表示
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
                    // PDFファイルの場合
                    else if (extension == ".pdf")
                    {
                        PdfFilePreviewer.PreviewPdfFile(RightColumn, RightColumnPath);
                    }
                }
            }
        }
        
        /// <summary>
        /// キーが押されたときに呼び出されるイベントハンドラー。
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
        }

        /// <summary>
        /// CenterColumnの全アイテムを選択します。
        /// </summary>
        private void SelectAllItems()
        {
            CenterColumn.SelectAll();
            UpdateRightColumnWithSelectedItems();
        }

        /// <summary>
        /// CenterColumnの選択を次のアイテムに移動します。
        /// </summary>
        private void SelectLowerItem()
        {
            if (CenterColumn.SelectedIndex < CenterColumn.Items.Count - 1)
            {
                CenterColumn.SelectedIndex++;
            }
        }

        /// <summary>
        /// CenterColumnの選択を前のアイテムに移動します。
        /// </summary>
        private void SelectUpperItem()
        {
            if (CenterColumn.SelectedIndex > 0)
            {
                CenterColumn.SelectedIndex--;
            }
        }

        /// <summary>
        /// Shiftキーを押しながらCenterColumnの選択を次のアイテムに移動します。
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
        /// Shiftキーを押しながらCenterColumnの選択を前のアイテムに移動します。
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
        /// 右キーが押されたとき中央カラムでディレクトリが選択されていれば、そこに移動します。
        /// </summary>
        internal void MoveToSubDirectory()
        {
            if (CenterColumn.SelectedItem is ListBoxItem selectedItem)
            {
                string? selectedItemPath = selectedItem.Tag?.ToString();
                if (selectedItemPath != null && Directory.Exists(selectedItemPath))
                {
                    MoveItemsLeft();
                }
            }
        }

        private void MoveItemsLeft()
        {
            // 左カラムに中央カラムの内容を移動
            MoveItems(CenterColumn, LeftColumn);
            LeftColumnLabel.Text = CenterColumnLabel.Text;
            LeftColumnPath = CenterColumnPath;

            // 中央カラムに右カラムの内容を移動
            MoveItems(RightColumn, CenterColumn);
            CenterColumnLabel.Text = RightColumnLabel.Text;
            CenterColumnPath = RightColumnPath;

            // 右カラムの内容を消去
            RightColumnLabel.Text = string.Empty;
            RightColumn.Items.Clear();

            // 左カラムで中央カラムのパスを選択
            if (!string.IsNullOrEmpty(CenterColumnPath))
            {
                SelectItemInColumn(LeftColumn, CenterColumnPath);
            }

            // 中央カラムの１つ目のアイテムを選択
            if (CenterColumn.Items.Count > 0)
            {
                CenterColumn.SelectedIndex = 0;
            }

            // 中央カラムで選択状態のアイテムにフォーカスを当てる
            if (CenterColumn.SelectedItem is ListBoxItem selectedListBoxItem)
            {
                selectedListBoxItem.Focus();
            }
        }

        /// <summary>
        /// 左キーが押されたとき親ディレクトリがあれば移動します。
        /// </summary>
        internal void MoveToParentDirectory()
        {
            // 左カラムに表示されているか
            if (Directory.Exists(LeftColumnPath))
            {
                string? newLeftColumnPath = Directory.GetParent(LeftColumnPath)?.FullName;
                MoveItemsRignt(newLeftColumnPath);

            // 左カラムにドライブリストが表示されていたか
            } else if (string.Equals(LeftColumnPath, DRIVE))
            {
                string newLeftColumnPath = string.Empty;
                MoveItemsRignt(newLeftColumnPath);
            }
        }

        /// <summary>
        /// 中央カラムの内容を右カラムに移動し、左カラムの内容を中央カラムに移動します。
        /// その後、親ディレクトリの内容を左カラムに表示します。
        /// </summary>
        /// <param name="parentDirectory">左カラムに表示するディレクトリのパス。</param>
        private void MoveItemsRignt(string? newLeftColumnPath)
        {
            // 右カラムに中央カラムの内容を移動
            MoveItems(CenterColumn, RightColumn);
            RightColumnLabel.Text = CenterColumnLabel.Text;
            RightColumnPath = CenterColumnPath;

            // 中央カラムに左カラムの内容を移動
            MoveItems(LeftColumn, CenterColumn);
            CenterColumnLabel.Text = LeftColumnLabel.Text;
            CenterColumnPath = LeftColumnPath;

            // 左カラムに表示
            // 中央カラムにドライブリストが表示される場合
            if (string.Equals(CenterColumnPath, DRIVE))
            {
                // 左カラムの表示をクリアする
                LeftColumn.Items.Clear();
                LeftColumnLabel.Text = string.Empty;
                LeftColumnPath = string.Empty;
            }
            else if (!string.Equals(CenterColumnPath, DRIVE) && string.IsNullOrEmpty(newLeftColumnPath))
            {
                // ドライブリストを表示
                ContentLoader.AddDrives(LeftColumn);
                LeftColumnLabel.Text = DRIVE;
                LeftColumnPath = DRIVE;
            }
            else
            {
                // ディレクトリの内容を表示
                DirectoryHelper.LoadDirectoryContent(LeftColumn, newLeftColumnPath);
                LeftColumnLabel.Text = GetLabel(newLeftColumnPath);
                LeftColumnPath = newLeftColumnPath;
            }

            // 右カラムのパスを中央カラムで選択
            if (!string.IsNullOrEmpty(RightColumnPath))
            {
                SelectItemInColumn(CenterColumn, RightColumnPath);
            }

            // 中央カラムのパスを左カラムで選択
            if (!string.IsNullOrEmpty(CenterColumnPath))
            {
                SelectItemInColumn(LeftColumn, CenterColumnPath);
            }

            // 中央カラムで選択状態のアイテムにフォーカスを当てる
            if (CenterColumn.SelectedItem is ListBoxItem selectedListBoxItem)
            {
                selectedListBoxItem.Focus();
            }
        }

        /// <summary>
        /// 指定されたパスに基づいてラベルを取得します。
        /// </summary>
        /// <param name="path">ラベルを取得するパス。</param>
        /// <returns>パスに基づくラベル。</returns>
        private string GetLabel(string path)
        {
            return (path == System.IO.Path.GetPathRoot(path) || path == DRIVE)
                ? path
                : System.IO.Path.GetFileName(path);
        }

        /// <summary>
        /// 選択されたアイテムを開きます。
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
        /// 中央カラムで選択されたアイテムを右カラムに表示します。
        /// </summary>
        internal void UpdateRightColumnWithSelectedItems()
        {
            // 右カラムのアイテムを消去
            RightColumn.Items.Clear();
            // 右カラムのラベルを更新
            RightColumnLabel.Text = SELECTED_ITEMS;
            // 中央カラムで選択されたアイテムを右カラムに表示
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
            // 右カラムのアイテムをすべて選択状態にする
            RightColumn.SelectAll();
        }

        /// <summary>
        /// アイテムを一つのListBoxから別のListBoxに移動します。
        /// </summary>
        /// <param name="source">移動元のListBox。</param>
        /// <param name="destination">移動先のListBox。</param>
        internal void MoveItems(ListBox source, ListBox destination)
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
                    Tag = ((ListBoxItem)item).Tag,
                    Foreground = ((ListBoxItem)item).Foreground
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
