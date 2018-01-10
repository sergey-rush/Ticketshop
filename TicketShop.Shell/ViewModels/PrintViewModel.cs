namespace TicketShop.Shell.ViewModels
{
    public class PrintViewModel:ViewModelBase
    {
         private static MainViewModel _mainViewModel;

         public PrintViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
        }

        public static MainViewModel Instance
        {
            get { return _mainViewModel; }
        } 
    }
}