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
        /// Column2�̑I�����ύX���ꂽ�Ƃ��ɌĂяo�����C�x���g�n���h���[�B
        /// �I�����ꂽ�f�B���N�g���̓��e��Column3�ɕ\�����܂��B
        /// </summary>
        /// <param name="sender">�C�x���g�𔭐��������I�u�W�F�N�g�B</param>
        /// <param name="e">�C�x���g�f�[�^�B</param>
        /// <param name="column3">���e��\������ListBox�B</param>
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
