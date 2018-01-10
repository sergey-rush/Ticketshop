using TicketShop.Shell.Models;

namespace TicketShop.Shell.ViewModels
{
    public class SelectedSpot : ViewModelBase
    {
        public SelectedSpot(Spot spotData)
        {
            _spotData = spotData;
        }

        private Spot _spotData;

        public Spot Spot
        {
            get { return _spotData; }
            set
            {
                _spotData = value;
                OnPropertyChanged("Spot");
            }
        }
    }
}