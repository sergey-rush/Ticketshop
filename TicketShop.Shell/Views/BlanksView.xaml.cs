using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TicketShop.Shell.ViewModels;

namespace TicketShop.Shell.Views
{
    public partial class BlanksView : UserControl
    {
        private MainViewModel _mainViewModel;
        public BlanksView()
        {
            InitializeComponent();
            _mainViewModel = MainViewModel.Instance;
            //_mainViewModel.ShowBlanks();
            DataContext = _mainViewModel;
        }
    }
}
