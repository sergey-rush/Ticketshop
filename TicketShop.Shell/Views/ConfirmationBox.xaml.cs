using System;
using System.Windows;

namespace TicketShop.Shell.Views
{
    public partial class ConfirmationBox
    {
        public ConfirmationBox()
        {
            InitializeComponent();
        }
        
        public string Header
        {
            set { Title = value; }
        }

        public string Message
        {
            set { ProgressMessage.Text = value; }
        }

        private void ConfirmButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            //Close();
        }

        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            //DialogResult = false;
            Close();
        }

        private void ConfirmationBox_OnClosed(object sender, EventArgs e)
        {
            //DialogResult = false;
        }
    }
}