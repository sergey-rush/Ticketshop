using System;
using System.Windows.Media.Imaging;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TicketShop.Core
{
    /// <summary>
    /// The most comprehensive entity for real ticket
    /// </summary>
    /// <remarks>Ticket object</remarks>
    public class Ticket : Seat
    {
        /// <summary>
        /// BlankId identificator
        /// </summary>
        public int BlankId { get; set; }
        public int OrderId { get; set; }
        public DateTime ActionDate { get; set; }
        private string _actionName = string.Empty;

        public string ActionName
        {
            get
            {
                return _actionName;
            }
            set { _actionName = value; }
        }
        public decimal Price { get; set; }
        private string _rowNum = string.Empty;

        public string RowNum
        {
            get { return _rowNum; }
            set { _rowNum = value; }
        }

        private string _seatNum = string.Empty;

        public string SeatNum
        {
            get { return _seatNum; }
            set { _seatNum = value; }
        }

        private string _sideName = string.Empty;

        public string SideName
        {
            get { return _sideName; }
            set { _sideName = value; }
        }

        private string _sectorName = string.Empty;

        public string SectorName
        {
            get { return _sectorName; }
            set { _sectorName = value; }
        }

        public DateTime SoldDate { get; set; }
        public int MemberId { get; set; }
        private string _barCode = string.Empty;

        /// <summary>
        /// Local system barcode of 12 symbols
        /// </summary>
        public string BarCode
        {
            get { return _barCode; }
            set { _barCode = value; }
        }

        private BitmapImage _ticketImage;

        public BitmapImage TicketImage
        {
            get { return _ticketImage; }
            set { _ticketImage = value; }
        }
    }
}