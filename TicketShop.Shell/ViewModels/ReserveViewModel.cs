using System;
using Xceed.Wpf.Toolkit.Core.Converters;

namespace TicketShop.Shell.ViewModels
{
    public class ReserveViewModel : ViewModelBase
    {
        private static MainViewModel _mainViewModel;

        public ReserveViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public static MainViewModel Instance
        {
            get { return _mainViewModel; }
        }
    }
}