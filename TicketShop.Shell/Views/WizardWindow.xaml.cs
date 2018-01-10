using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TicketShop.Core;
using TicketShop.Map;
using TicketShop.Shell.Models;
using TicketShop.Shell.ViewModels;

namespace TicketShop.Shell.Views
{
    public partial class WizardWindow : MetroWindow
    {
        private readonly MainViewModel _mainViewModel;
        public WizardWindow()
        {
            _mainViewModel = MainViewModel.Instance;
            DataContext = _mainViewModel;
            InitializeComponent();
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            
        }
    }
}
