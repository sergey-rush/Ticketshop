namespace TicketShop.Shell.ViewModels
{
    public class PurchaseViewModel:ViewModelBase
    {
        private static MainViewModel _mainViewModel;

        public PurchaseViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public static MainViewModel Instance
        {
            get { return _mainViewModel; }
        } 
    }
}