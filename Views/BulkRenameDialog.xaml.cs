using System.Windows;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using System.Windows.Input;

namespace ColumnExplorer.Views
{
    /// <summary>
    /// Interaction logic for BulkRenameDialog.xaml
    /// </summary>
    public partial class BulkRenameDialog : Window
    {
        public string FindText => FindTextBox.Text;
        public string ReplaceText => ReplaceTextBox.Text;

        /// <summary>
        /// Constructor for BulkRenameDialog
        /// </summary>
        public BulkRenameDialog()
        {
            InitializeComponent();
            Loaded += BulkRenameDialog_Loaded;
        }

        /// <summary>
        /// Ok button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        /// <summary>
        /// Cancel button click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        /// <summary>
        /// Loaded event handler to set focus on the first TextBox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BulkRenameDialog_Loaded(object sender, RoutedEventArgs e)
        {
            FindTextBox.Focus();

            // Add event handlers for the buttons
            this.KeyDown += new KeyEventHandler(InputDialog_KeyDown);

            // Set tooltips for the buttons
            OkButton.ToolTip = "[Enter]";
            CancelButton.ToolTip = "[Esc]";
        }

        private void InputDialog_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButton_Click(this, new RoutedEventArgs());
            }
            else if (e.Key == Key.Escape)
            {
                CancelButton_Click(this, new RoutedEventArgs());
            }
        }
    }
}
