using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Media;
using TicketShop.Shell.ViewModels;

namespace TicketShop.Shell.Views
{
    public partial class OrderWindow
    {
        private MainViewModel _mainViewModel;
        public OrderWindow()
        {
            InitializeComponent();
            _mainViewModel = MainViewModel.Instance;
            DataContext = _mainViewModel;
            DtpReserveDate.Background = new SolidColorBrush(Colors.White);
        }
    }
}