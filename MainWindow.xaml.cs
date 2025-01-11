using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ColumnExplorer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LoadDrives();
        }

        private void LoadDrives()
        {
            // 左カラムにドライブ一覧を表示
            foreach (var drive in DriveInfo.GetDrives())
            {
                Column1.Items.Add(CreateListBoxItem(drive.Name, isDirectory: true));
            }
        }

        private void Column1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Column1.SelectedItem is ListBoxItem selectedItem)
            {
                string path = selectedItem.Tag.ToString();
                if (Directory.Exists(path))
                {
                    // 中央カラムを更新
                    LoadContent(path, Column2);
                    Column3.Items.Clear(); // 右カラムをクリア
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
                    // 右カラムを更新
                    LoadContent(path, Column3);
                }
            }
        }

        private void LoadContent(string path, ListBox targetColumn)
        {
            targetColumn.Items.Clear();
            try
            {
                // フォルダを先に追加
                foreach (var dir in Directory.GetDirectories(path))
                {
                    targetColumn.Items.Add(CreateListBoxItem(dir, isDirectory: true));
                }

                // ファイルを追加
                foreach (var file in Directory.GetFiles(path))
                {
                    targetColumn.Items.Add(CreateListBoxItem(file, isDirectory: false));
                }
            }
            catch
            {
                MessageBox.Show("アクセスできないフォルダやファイルがあります。");
            }
        }

        private ListBoxItem CreateListBoxItem(string path, bool isDirectory)
        {
            // ListBoxItemを作成
            var item = new ListBoxItem
            {
                Content = isDirectory ? Path.GetFileName(path) : Path.GetFileName(path),
                Tag = path, // フルパスをTagに格納
                FontWeight = isDirectory ? FontWeights.Bold : FontWeights.Normal, // フォルダを太字で表示
                Foreground = isDirectory ? Brushes.Blue : Brushes.Black // フォルダを青色で表示
            };
            return item;
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            // 左右キーでカラム間を移動
            if (e.Key == Key.Left)
            {
                FocusPreviousColumn();
            }
            else if (e.Key == Key.Right)
            {
                FocusNextColumn();
            }
        }

        private void FocusPreviousColumn()
        {
            if (Column2.IsFocused) Column1.Focus();
            else if (Column3.IsFocused) Column2.Focus();
        }

        private void FocusNextColumn()
        {
            if (Column1.IsFocused) Column2.Focus();
            else if (Column2.IsFocused) Column3.Focus();
        }
    }
}
