using System;
using System.Windows;
using System.Windows.Media;
using TicketShop.Shell.ViewModels;

namespace TicketShop.Shell.Views
{
    public partial class PaymentTypeBox
    {
        private MainViewModel _mainViewModel;
        public PaymentTypeBox()
        {
            InitializeComponent();
            _mainViewModel = MainViewModel.Instance;
            DataContext = _mainViewModel;
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

        private void PaymentTypeBox_OnClosed(object sender, EventArgs e)
        {
            //DialogResult = false;
        }
    }
}