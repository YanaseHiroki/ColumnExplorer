using System.IO;
using System.Windows.Controls;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// 各種イベントハンドラーを提供する静的クラス。
    /// </summary>
    public static class EventHandlers
    {
        /// <summary>
        /// Column2の選択が変更されたときに呼び出されるイベントハンドラー。
        /// 選択されたディレクトリの内容をColumn3に表示します。
        /// </summary>
        /// <param name="sender">イベントを発生させたオブジェクト。</param>
        /// <param name="e">イベントデータ。</param>
        /// <param name="column3">内容を表示するListBox。</param>
        public static void Column2_SelectionChanged(object sender, SelectionChangedEventArgs e, ListBox column3)
        {
            if (sender is ListBox column2 && column2.SelectedItem is ListBoxItem selectedItem)
            {
                string? path = selectedItem.Tag == null ? null : selectedItem.Tag.ToString();
                DirectoryHelper.LoadDirectoryContent(column3, path);
            }
        }
    }
}
