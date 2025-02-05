using System.Windows;
using System.Windows.Input;
using DocumentFormat.OpenXml.Vml.Spreadsheet;

namespace ColumnExplorer.Views
{
    public partial class InputDialog : Window
    {
        public InputDialog(string title, string defaultResponse = "")
        {
            InitializeComponent();
            Title = title;
            PromptText.Text = defaultResponse;
            ResponseTextBox.Text = defaultResponse;
            ResponseTextBox.SelectAll();
            ResponseTextBox.Focus();

            // Add event handlers for the buttons
            this.KeyDown += new KeyEventHandler(InputDialog_KeyDown);

            // Set tooltips for the buttons
            OkButton.ToolTip = "[Enter]";
            CancelButton.ToolTip = "[Esc]";
        }

        public string ResponseText => ResponseTextBox.Text;

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
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
