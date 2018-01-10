using System;
using Newtonsoft.Json;

namespace TicketShop.Core
{
    /// <summary>
    /// Used as WorkUnit for client-server interaction
    /// </summary>
    /// <remarks>Payload</remarks>
    public class Seat
    {
        /// <summary>
        /// SeatId identifier, the primary key
        /// </summary>
        [JsonProperty(PropertyName = "Id")]
        public int Id { get; set; }

        
        private DateTime _reserveDate = DateTime.MinValue;

        /// <summary>
        /// Desired or actual Reserve Date of Seat or Ticket.
        /// The date and time a ticket is reserved up to
        /// </summary>
        [JsonProperty(PropertyName = "ReserveDate")]
        public DateTime ReserveDate
        {
            get { return _reserveDate; }
            set { _reserveDate = value; }
        }

        private string _extBarCode = string.Empty;
        /// <summary>
        /// External system barcode of 20 symbols
        /// </summary>
        [JsonProperty(PropertyName = "ExtBarCode")]
        public string ExtBarCode
        {
            get { return _extBarCode; }
            set { _extBarCode = value; }
        }
        
        /// <summary>
        /// Current status of the ticket
        /// </summary>
        public ItemStatus Status { get; set; }
        /// <summary>
        /// Result is operation status for reserve, purchase or cancel operation
        /// </summary>
        public OperationResult Result { get; set; }
    }
}