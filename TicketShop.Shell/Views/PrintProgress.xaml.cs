using System;
using System.Windows;

namespace TicketShop.Shell.Views
{
    public partial class PrintProgress
    {
        public PrintProgress()
        {
            InitializeComponent();
        }

        private void ProgressDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            Title = Header;
        }

        private string _header = "Пожалуйста, подождите...";
        public string Header
        {
            get { return _header; }
            set { _header = value; }
        }

        public string Message
        {
            set { DialogMessage.Text = value; }
        }

        public int ProgressValue
        {
            set
            {
                Progress.Value = value;
            }
        }

        public bool IsIndeterminate
        {
            set { Progress.IsIndeterminate = value; }
        }

        public string Description
        {
            set { DescriptionLabel.Content = value; }
        }
       
        public event EventHandler Cancel = delegate { };
        private void CancelButton_OnClick(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Cancel(sender, e);
            //Close();
        }
    }
}