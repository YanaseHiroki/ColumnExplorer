using System.IO;
using System.Windows.Controls;

namespace ColumnExplorer.Helpers
{
    /// <summary>
    /// �e��C�x���g�n���h���[��񋟂���ÓI�N���X�B
    /// </summary>
    public static class EventHandlers
    {
        /// <summary>
        /// centerColumn�̑I�����ύX���ꂽ�Ƃ��ɌĂяo�����C�x���g�n���h���[�B
        /// �I�����ꂽ�f�B���N�g���̓��e��rightColumn�ɕ\�����܂��B
        /// </summary>
        /// <param name="sender">�C�x���g�𔭐��������I�u�W�F�N�g�B</param>
        /// <param name="e">�C�x���g�f�[�^�B</param>
        /// <param name="rightColumn">���e��\������ListBox�B</param>
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
