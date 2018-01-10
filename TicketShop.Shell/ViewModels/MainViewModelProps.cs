 using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MahApps.Metro.Controls;
using TicketShop.Core;
using TicketShop.Shell.Models;
using TicketShop.Shell.Views;

namespace TicketShop.Shell.ViewModels
{
    public partial class MainViewModel
    {
        private ObservableCollection<Spot> _spots = new ObservableCollection<Spot>();
        private ObservableCollection<Spot> _selectedSpots = new ObservableCollection<Spot>();
        //private ObservableCollection<Spot> _orderSeats = new ObservableCollection<Spot>();
        private readonly ObservableCollection<ShowAction> _showActions = new ObservableCollection<ShowAction>();
        private ShowAction _selectedAction;
        private readonly ObservableCollection<EventDate> _eventDates = new ObservableCollection<EventDate>();
        private ProgressDialog _progressDialog;
        private PrintProgress _printProgress;
        private BackgroundWorker _worker;
        private UpdateProgressDelegate _updateProgressDelegate;
        private UpdatePrintDelegate _updatePrintDelegate;
        private OrderWindow _orderWindow;
        private WisardStep _wisardStep = WisardStep.None;
        private WizardWindow _wizardWindow;
        private BitmapImage _stageImage;

        public BitmapImage StageImage
        {
            get { return _stageImage; }
            set
            {
                _stageImage = value;
                OnPropertyChanged("StageImage");
            }
        }

        private ObservableCollection<Order> _orders = new ObservableCollection<Order>();

        public ObservableCollection<Order> Orders
        {
            get { return _orders; }
            set
            {
                _orders = value;
                OnPropertyChanged("Orders");
            }
        }

        private ObservableCollection<Hall> _hallSeats = new ObservableCollection<Hall>();

        public ObservableCollection<Hall> HallSeats
        {
            get { return _hallSeats; }
            set
            {
                _hallSeats = value;
                OnPropertyChanged("HallSeats");
            }
        }

        private ObservableCollection<Blank> _blanks = new ObservableCollection<Blank>();

        public ObservableCollection<Blank> Blanks
        {
            get { return _blanks; }
            set
            {
                _blanks = value;
                OnPropertyChanged("Blanks");
            }
        }

        private ObservableCollection<Ticket> _seats = new ObservableCollection<Ticket>();

        public ObservableCollection<Ticket> Seats
        {
            get
            {
                return _seats;
            }
            set
            {
                _seats = value;
                OnPropertyChanged("Seats");
            }
        }

        private ObservableCollection<AppearanceStat> _appearances = new ObservableCollection<AppearanceStat>();

        public ObservableCollection<AppearanceStat> Appearances
        {
            get { return _appearances; }
            set
            {
                _appearances = value;
                OnPropertyChanged("Appearances");
            }
        }

        private string _wizardTitle = String.Empty;

        public string WizardTitle
        {
            get { return _wizardTitle; }
            private set
            {
                if (_wizardTitle != value)
                {
                    _wizardTitle = value;
                    OnPropertyChanged("WizardTitle");
                }
            }
        }

        /// <summary>
        /// Used to sum selected seats
        /// </summary>
        public decimal SeatsAmount;

        /// <summary>
        /// Used to count selected seats
        /// </summary>
        public int SeatsCount;

        public static MainViewModel Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MainViewModel();
                    _instance.Appearances.Add(new AppearanceStat()
                    {
                        Name = "Игорь",
                        Starts = 10,
                        Count = 17

                    });
                    _instance.Appearances.Add(new AppearanceStat()
                    {
                        Name = "Тимур",
                        Starts = 3,
                        Count = 7
                    });
                    _instance.Appearances.Add(new AppearanceStat()
                    {
                        Name = "Владимир",
                        Starts = 5,
                        Count = 9
                    });

                }
                return _instance;
            }
        }

        public Member Member { get; set; }

        /// <summary>
        /// Shell window instance
        /// </summary>
        public MainWindow MetroWindow { get; set; }

        private ViewModelBase _contentWindow;

        public ViewModelBase ContentWindow
        {
            get { return _contentWindow; }
            set
            {
                _contentWindow = value;
                OnPropertyChanged("ContentWindow");
            }
        }

        private DateTime _reserveDate = DateTime.MinValue;

        /// <summary>
        /// DateTime.UtcNow.AddMinutes(20)
        /// </summary>
        public DateTime ReserveDate
        {
            get
            {
                if (_reserveDate == DateTime.MinValue)
                {
                    _reserveDate = DateTime.UtcNow.AddMinutes(20);
                }

                return _reserveDate;
            }
            set
            {
                _reserveDate = value;
                OnPropertyChanged("ReserveDate");
            }
        }

        private DateTime _selectedDate = DateTime.Now;
        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                OnPropertyChanged("SelectedDate");
            }
        }

        private readonly ObservableCollection<Provider> _providers = new ObservableCollection<Provider>();

        public ObservableCollection<Provider> Providers
        {
            get { return _providers; }
        }

        private string _windowTitle;

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                OnPropertyChanged("WindowTitle");
            }
        }

        private decimal _orderPrice;

        public decimal OrderPrice
        {
            get { return _orderPrice; }
            set
            {
                _orderPrice = value;
                OnPropertyChanged("OrderPrice");
            }
        }

        private string _orderCountText;
        public string OrderCountText
        {
            get { return _orderCountText; }
            set
            {
                _orderCountText = value;
                OnPropertyChanged("OrderCountText");
            }
        }

        private string _ticketCountText;
        public string TicketCountText
        {
            get { return _ticketCountText; }
            set
            {
                _ticketCountText = value;
                OnPropertyChanged("TicketCountText");
            }
        }

        private string _amountSumText;
        public string AmountSumText
        {
            get { return _amountSumText; }
            set
            {
                _amountSumText = value;
                OnPropertyChanged("AmountSumText");
            }
        }

        private Ticket _selectedSeat;
        public Ticket SelectedSeat
        {
            get { return _selectedSeat; }
            set
            {
                if (value != null)
                {
                    _selectedSeat = value;
                    SelectedSeatCount = Seats.Count(x => x.OrderId == _selectedSeat.OrderId);
                    SelectedSeatAmount = Seats.Where(x => x.OrderId == _selectedSeat.OrderId).Sum(x => x.Price);
                    OnPropertyChanged("SelectedSeat");
                }
            }
        }

        private int _selectedSeatCount;
        public int SelectedSeatCount
        {
            get { return _selectedSeatCount; }
            set
            {
                _selectedSeatCount = value;
                OnPropertyChanged("SelectedSeatCount");
            }
        }

        private decimal _selectedSeatAmount;
        public decimal SelectedSeatAmount
        {
            get { return _selectedSeatAmount; }
            set
            {
                _selectedSeatAmount = value;
                OnPropertyChanged("SelectedSeatAmount");
            }
        }

        private Provider _selectedProvider;

        public Provider SelectedProvider
        {
            get { return _selectedProvider; }
            set
            {
                if (_selectedProvider == value || value == null) return;
                _selectedProvider = value;
                OnPropertyChanged("SelectedProvider");
                if (_selectedProvider != null)
                {
                    OnProviderChanged();
                }
            }
        }

        public bool EnableProvider
        {
            get { return Providers.Count > 1; }
        }

        public ObservableCollection<ShowAction> ShowActions
        {
            get { return _showActions; }
        }

        public ShowAction SelectedAction
        {
            get { return _selectedAction; }
            set
            {
                if (_selectedAction == value || value == null) return;
                _selectedAction = value;


                OnPropertyChanged("SelectedAction");
                if (_selectedAction != null)
                {
                    OnShowActionChanged();
                }
            }
        }

        public ObservableCollection<EventDate> EventDates
        {
            get { return _eventDates; }
        }

        private EventDate _selectedEvent;

        /// <summary>
        /// Selected Event is global application selected Event object
        /// </summary>
        public EventDate SelectedEvent
        {
            get { return _selectedEvent; }
            set
            {
                if (_selectedEvent == value || value == null) return;
                _selectedEvent = value;
                WindowTitle = String.Format("{0} {1} {2}", SelectedProvider.Name, SelectedAction.Name,_selectedEvent.Date);
                OnPropertyChanged("SelectedEvent");
                OnPropertyChanged("CurrentEvent");
                if (_selectedEvent != null)
                {
                    OnEventDateChanged();
                }
            }
        }

        private Customer _customer = new Customer();

        public Customer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                OnPropertyChanged("Customer");
            }
        }

        public string CurrentEvent
        {
            get
            {
                if (_selectedEvent != null)
                {
                    return _selectedEvent.Date;
                }
                else
                {
                    return "Выберите выступление";
                }
            }
        }

        public ObservableCollection<Spot> Spots
        {
            get { return _spots; }
            set
            {
                _spots = value;
                OnPropertyChanged("Spots");
            }
        }

        public ObservableCollection<Spot> SelectedSpots
        {
            get
            {
                // Due to pop up collection from code-behind we notify TotalSeats property on get method
                OnPropertyChanged("TotalSeats");
                return _selectedSpots;
            }
            set
            {
                _selectedSpots = value;
                OnPropertyChanged("SelectedSpots");
                //OnPropertyChanged("TotalSeats");
            }
        }

        private AppThemeData _selectedTheme;

        public AppThemeData SelectedTheme
        {
            get { return _selectedTheme; }
            set
            {
                _selectedTheme = value;
                _selectedTheme.ChangeTheme();
            }
        }

        private AppThemeData _selectedAccent;

        public AppThemeData SelectedAccent
        {
            get { return _selectedAccent; }
            set
            {
                _selectedAccent = value;
                _selectedAccent.ChangeAccent();
            }
        }

        private Blank _selectedBlank;

        public Blank SelectedBlank
        {
            get
            {
                if (_selectedBlank == null)
                {
                    _selectedBlank = new Blank();
                }
                return _selectedBlank;
            }
            set
            {
                _selectedBlank = value;
                OnPropertyChanged("SelectedBlank");
            }
        }

        private Spot _selectedSpot;

        public Spot SelectedSpot
        {
            get { return _selectedSpot; }
            set
            {
                _selectedSpot = value;
                OnPropertyChanged("SelectedSpot");
            }
        }

        private Ticket _selectedTicket;

        public Ticket SelectedTicket
        {
            get { return _selectedTicket; }
            set
            {
                _selectedTicket = value;
                OnPropertyChanged("SelectedTicket");
            }
        }

        //private Order _currentOrder;

        /// <summary>
        /// Selected Order or current Order persisted object
        /// </summary>
        //public Order Customer.Order
        //{
        //    get { return _currentOrder; }
        //    set
        //    {
        //        _currentOrder = value;
        //        OnPropertyChanged("Customer.Order");
        //    }
        //}

        public string TotalSeats
        {
            get
            {
                SeatsAmount = _selectedSpots.Sum(x => x.Price);
                SeatsCount = _selectedSpots.Count;
                return String.Format("{0} | {1}", SeatsCount.ToString("### ##0"), SeatsAmount.ToString("C0"));
            }
            set
            {
                OnPropertyChanged("TotalSeats");
            }
        }

        private string _statusText = String.Empty;

        public string StatusText
        {
            get
            {
                if (string.IsNullOrEmpty(_statusText))
                {
                    _statusText = "Программа готова к работе";
                }
                return _statusText;
            }
            set
            {
                _statusText = value;
                OnPropertyChanged("StatusText");
            }
        }

        private string _customerName;

        public string CustomerName
        {
            get { return _customerName; }
            set
            {
                _customerName = value;
                OnPropertyChanged("CustomerName");
            }
        }

        private string _lastName;

        public string LastName
        {
            get { return _lastName; }
            set
            {
                _lastName = value;
                OnPropertyChanged("LastName");
            }
        }

        private string _firstName;

        public string FirstName
        {
            get { return _firstName; }
            set
            {
                _firstName = value;
                OnPropertyChanged("FirstName");
            }
        }

        private string _middleName;

        public string MiddleName
        {
            get { return _middleName; }
            set
            {
                _middleName = value;
                OnPropertyChanged("MiddleName");
            }
        }

        private string _customerEmail;

        public string CustomerEmail
        {
            get { return _customerEmail; }
            set
            {
                _customerEmail = value;
                OnPropertyChanged("CustomerEmail");
            }
        }

        private string _customerPhone;

        public string CustomerPhone
        {
            get { return _customerPhone; }
            set
            {
                _customerPhone = value;
                OnPropertyChanged("CustomerPhone");
            }
        }

        private bool _sendSms;

        public bool SendSms
        {
            get { return _sendSms; }
            set
            {
                _sendSms = value;
                OnPropertyChanged("SendSms");
            }
        }

        private Visibility _magnifierVisibility = Visibility.Collapsed;

        public Visibility MagnifierVisibility
        {
            get { return _magnifierVisibility; }
            private set
            {
                _magnifierVisibility = value;
                OnPropertyChanged("MagnifierVisibility");
            }
        }

        private bool _printerFound;

        public bool PrinterFound
        {
            get { return _printerFound; }
            set
            {
                _printerFound = value;
                OnPropertyChanged("PrinterFound");
            }
        }

        private bool _autoSearch;

        /// <summary>
        /// Indicates that printer initialization includes network scan
        /// </summary>
        public bool AutoSearch
        {
            get { return _autoSearch; }
            set
            {
                _autoSearch = value;
                OnPropertyChanged("AutoSearch");
            }
        }

        private string _query = String.Empty;

        public string Query
        {
            get { return _query; }
            set
            {
                if (_query != value)
                {
                    _query = value;
                    if (_query == String.Empty)
                    {
                        ShowOrders();
                    }
                    OnPropertyChanged("Query");
                }
            }
        }

        //public Order _order;
        //public Order Order
        //{
        //    get
        //    {
        //        return _order;
        //    }
        //    set
        //    {
        //        _order = value;
        //        OnPropertyChanged("Order");
        //    }
        //}

        public string OrderNum
        {
            get { return String.Format("Заказ № {0}", Customer.Order.Id); }
        }

        public string MemberName
        {
            get { return Customer.Order.Member; }
        }

        public string TicketsCount
        {
            get { return String.Format("Билетов: {0}", Customer.Order.ItemsCount); }
        }

        public string OrderDate
        {
            get { return Customer.Order.CreatedDate.ToString("f"); }
        }

        //public ObservableCollection<Spot> OrderSeats
        //{
        //    get
        //    {
        //        return _orderSeats;

        //    }
        //    set
        //    {
        //        _orderSeats = value;
        //        OnPropertyChanged("OrderSeats");
        //    }
        //}

        private string _operationResult = String.Empty;

        public string OperationResult
        {
            get { return _operationResult; }
            private set
            {
                if (_operationResult != value)
                {
                    _operationResult = value;
                    OnPropertyChanged("OperationResult");
                }
            }
        }

        private string _printerInfo = String.Empty;

        /// <summary>
        /// Information received from printer
        /// </summary>
        public string PrinterInfo
        {
            get { return _printerInfo; }
            set
            {
                _printerInfo = value;
                OnPropertyChanged("PrinterInfo");
            }
        }

        private string _printerStatus = String.Empty;

        /// <summary>
        /// StatusBar printer status text
        /// </summary>
        public string PrinterStatus
        {
            get { return _printerStatus; }
            private set
            {
                _printerStatus = value;
                OnPropertyChanged("PrinterStatus");
            }
        }

        private Visibility _printProgressVisibility = Visibility.Collapsed;

        public Visibility PrintProgressVisibility
        {
            get { return _printProgressVisibility; }
            private set
            {
                if (_printProgressVisibility != value)
                {
                    _printProgressVisibility = value;
                    OnPropertyChanged("PrintProgressVisibility");
                }
            }
        }

        private Visibility _wizardProgressVisibility = Visibility.Collapsed;

        public Visibility WizardProgressVisibility
        {
            get { return _wizardProgressVisibility; }
            private set
            {
                if (_wizardProgressVisibility != value)
                {
                    _wizardProgressVisibility = value;
                    OnPropertyChanged("WizardProgressVisibility");
                }
            }
        }

        private ObservableCollection<Ticket> _tickets = new ObservableCollection<Ticket>();

        public ObservableCollection<Ticket> Tickets
        {
            get { return _tickets; }
            set
            {
                _tickets = value;
                OnPropertyChanged("Tickets");
            }
        }

        private string _orderDialogTitle = String.Empty;

        public string OrderDialogTitle
        {
            get { return _orderDialogTitle; }
            private set
            {
                if (_orderDialogTitle != value)
                {
                    _orderDialogTitle = value;
                    OnPropertyChanged("OrderDialogTitle");
                }
            }
        }

        private ItemStatus _orderStatus = ItemStatus.None;

        public ItemStatus OrderStatus
        {
            get { return _orderStatus; }
            private set
            {
                if (_orderStatus != value)
                {
                    _orderStatus = value;
                    OnPropertyChanged("OrderStatus");
                }
            }
        }

        private int _selectedBlankStatus = 1;

        public int SelectedBlankStatus
        {
            get { return _selectedBlankStatus; }
            set
            {
                if (_selectedBlankStatus != value)
                {
                    _selectedBlankStatus = value;
                    OnPropertyChanged("SelectedBlankStatus");
                    ShowBlanks();
                }
            }
        }

        private string _blankStatus = String.Empty;

        public string BlankStatus
        {
            get { return _blankStatus; }
            set
            {
                _blankStatus = value;
                OnPropertyChanged("BlankStatus");
            }
        }

        private DictionaryEntry _selectedPaymentType = new DictionaryEntry(1, "Наличными");

        public DictionaryEntry SelectedPaymentType
        {
            get { return _selectedPaymentType; }
            set
            {
                _selectedPaymentType = value;
                OnPropertyChanged("SelectedPaymentType");
            }
        }

        public IList PaymentTypes
        {
            get { return new EnumName(typeof (PaymentType)).GetListValues(); }
        }

        /// <summary>
        /// TODO: Property is yet not bind, remove later 
        /// Button background highlighted
        /// Usage: Background="{Binding Path=Highlighted}"
        /// </summary>
        public Brush Highlighted
        {
            get { return Brushes.Yellow; }
        }

        private string _printerIp;

        public string PrinterIp
        {
            get { return _printerIp; }
            set
            {
                _printerIp = value;
                OnPropertyChanged("PrinterIp");
            }
        }

        private string _networkMask;

        public string NetworkMask
        {
            get { return _networkMask; }
            set
            {
                _networkMask = value;
                OnPropertyChanged("NetworkMask");
            }
        }

        private int _timerInterval;

        public int TimerInterval
        {
            get { return _timerInterval; }
            set
            {
                _timerInterval = value;
                OnPropertyChanged("TimerInterval");
            }
        }

        private int _rowCount;

        public int RowCount
        {
            get { return _rowCount; }
            set
            {
                _rowCount = value;
                OnPropertyChanged("RowCount");
            }
        }

        private bool _topmost;

        public bool Topmost
        {
            get { return _topmost; }
            set
            {
                _topmost = value;
                OnPropertyChanged("Topmost");
            }
        }

        private bool _fullScreen;

        public bool FullScreen
        {
            get { return _fullScreen; }
            set
            {
                _fullScreen = value;
                OnPropertyChanged("FullScreen");
            }
        }

        private bool _showTickets;

        public bool ShowTickets
        {
            get { return _showTickets; }
            set
            {
                _showTickets = value;
                OnPropertyChanged("ShowTickets");
            }
        }

        private bool _autoRefresh;

        public bool AutoRefresh
        {
            get { return _autoRefresh; }
            set
            {
                _autoRefresh = value;
                OnPropertyChanged("AutoRefresh");
            }
        }

        private Visibility _updateIconVisibility = Visibility.Hidden;

        public Visibility UpdateIconVisibility
        {
            get
            {
                return _updateIconVisibility;
            }
            set
            {
                _updateIconVisibility = value;
                OnPropertyChanged("UpdateIconVisibility");
            }
        }

        private Visibility _controlVisibility = Visibility.Hidden;

        public Visibility ControlVisibility
        {
            get
            {
                if (SelectedSpots.Count > 0)
                {
                    _controlVisibility = Visibility.Visible;
                }
                return _controlVisibility;
            }
            set
            {
                _controlVisibility = value;
                OnPropertyChanged("ControlVisibility");
            }
        }
    }
}