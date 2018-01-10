using System;
using System.Windows;

namespace TicketShop.Shell.Views
{
    public partial class ProgressDialog
    {
        public ProgressDialog()
        {
            InitializeComponent();
        }

        private string _header = "Пожалуйста, подождите...";
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }

        private string _message = String.Empty;
        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                ProgressMessage.Text = _message;
            }
        }

        private void ProgressDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            Title = Header;
            ProgressMessage.Text = Message;
        }
    }
}