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
        /// centerColumnの選択が変更されたときに呼び出されるイベントハンドラー。
        /// 選択されたディレクトリの内容をrightColumnに表示します。
        /// </summary>
        /// <param name="sender">イベントを発生させたオブジェクト。</param>
        /// <param name="e">イベントデータ。</param>
        /// <param name="rightColumn">内容を表示するListBox。</param>
        public static void CenterColumn_SelectionChanged(object sender, SelectionChangedEventArgs e, ListBox rightColumn)
        {
            if (sender is ListBox centerColumn && centerColumn.SelectedItem is ListBoxItem selectedItem)
            {
                string? path = (selectedItem.Tag == null) ? string.Empty : selectedItem.Tag.ToString();
                DirectoryHelper.LoadDirectoryContent(rightColumn, path);
            }
        }
    }
}
