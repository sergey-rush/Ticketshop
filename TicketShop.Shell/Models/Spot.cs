using System;
using System.Windows.Media.Imaging;
using TicketShop.Core;
using TicketShop.Shell.ViewModels;

namespace TicketShop.Shell.Models
{
    public class Spot : ViewModelBase, IComparable<Spot>
    {
        #region Data Members

        /// <summary>
        /// The X coordinate of the location of the ellipse (in content coordinates).
        /// </summary>
        private double _x = 0;

        /// <summary>
        /// The Y coordinate of the location of the ellipse (in content coordinates).
        /// </summary>
        private double _y = 0;


        /// <summary>
        /// Set to 'true' when the ellipse is selected in the ListBox.
        /// </summary>
        private bool _isSelected = false;

        #endregion Data Members

        public Spot()
        {

        }

        /// <summary>
        /// The X coordinate of the location of the ellipse (in content coordinates).
        /// </summary>
        public double X
        {
            get
            {
                return _x;
            }
            set
            {
                _x = value;
                OnPropertyChanged("X");
            }
        }

        /// <summary>
        /// The Y coordinate of the location of the ellipse (in content coordinates).
        /// </summary>
        public double Y
        {
            get
            {
                return _y;
            }
            set
            {
                _y = value;
                OnPropertyChanged("Y");
            }
        }

        private double _width = 20;
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                _width = value;
                OnPropertyChanged("Width");
            }
        }

        private double _height = 20;
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                _height = value;
                OnPropertyChanged("Height");
            }
        }

        private System.Windows.Media.Color _color;
        /// <summary>
        /// The color of the ellipse.
        /// </summary>
        public System.Windows.Media.Color Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                OnPropertyChanged("Color");
            }
        }

        private int _itemId;
        /// <summary>
        /// Ordinal number of Item
        /// </summary>
        public int ItemId
        {
            get
            {
                return _itemId;
            }
            set
            {
                _itemId = value;
                OnPropertyChanged("ItemId");
            }
        }

        private int _id;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
                OnPropertyChanged("Id");
            }
        }

        public string TicketNum
        {
            get
            {
                return String.Format("Билет №: {0}", Id);
            }
        }

        private decimal _price;
        public decimal Price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
                OnPropertyChanged("Price");
            }
        }

        private string _sideName;
        public string SideName
        {
            get
            {
                return _sideName;
            }
            set
            {
                _sideName = value;
                OnPropertyChanged("SideName");
            }
        }

        private string _sectorName;
        public string SectorName
        {
            get
            {
                return _sectorName;
            }
            set
            {
                _sectorName = value;
                OnPropertyChanged("SectorName");
            }
        }

        private string _seatNum;
        public string SeatNum
        {
            get
            {
                return _seatNum;
            }
            set
            {
                _seatNum = value;
                OnPropertyChanged("SeatNum");
            }
        }

        private string _rowNum;
        public string RowNum
        {
            get
            {
                return _rowNum;
            }
            set
            {
                _rowNum = value;
                OnPropertyChanged("RowNum");
            }
        }
        
        public string Info
        {
            get { return string.Format("Билет № {0}\r\n{1}\r\n{2}\r\nРяд: {3}\r\nМесто: {4}\r\nЦена: {5}", Id.ToString("### ### ###"), SideName, SectorName, RowNum, SeatNum, Price.ToString("C0")); }
        }

        private Blank _blank;
        public Blank Blank
        {
            get
            {
                return _blank;
            }
            set
            {
                _blank = value;
                OnPropertyChanged("Blank");
            }
        }

        /// <summary>
        /// Set to 'true' when the ellipse is selected in the ListBox.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        //private bool _isPrinted = false;
        //public bool IsPrinted
        //{
        //    get
        //    {
        //        return _isSelected;
        //    }
        //    set
        //    {
        //        _isSelected = value;
        //        OnPropertyChanged("IsPrinted");
        //    }
        //}

        private OperationResult _result = OperationResult.None;
        /// <summary>
        /// Result of CreateOrder operation.
        /// Persists for all objects
        /// </summary>
        public OperationResult Result
        {
            get
            {
                return _result;
            }
            set
            {
                _result = value;
                OnPropertyChanged("Result");
            }
        }

        private ItemStatus _status = ItemStatus.None;
        /// <summary>
        /// Current status of the ticket
        /// </summary>
        public ItemStatus Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
                OnPropertyChanged("Status");
            }
        }

        private BitmapImage _ticketImage;
        public BitmapImage TicketImage
        {
            get
            {
                return _ticketImage;
            }
            set
            {
                _ticketImage = value;
                OnPropertyChanged("TicketImage");
            }
        }

        private DateTime _reserveDate = DateTime.MinValue;

        /// <summary>
        /// The date and time a seat is reserved up to
        /// </summary>
        public DateTime ReserveDate
        {
            get { return _reserveDate; }
            set
            {
                _reserveDate = value;
                OnPropertyChanged("ReserveDate");
            }
        }

        /// <summary>
        /// Compares DESC
        /// </summary>
        public int CompareTo(Spot other)
        {
            if (other.Blank.Id > Blank.Id) return -1;
            if (other.Blank.Id == Blank.Id) return 0;
            return 1;
        }
    }
}