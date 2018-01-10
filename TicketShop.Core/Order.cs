using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TicketShop.Core
{
    /// <summary>
    /// Selected Order or current Order.
    /// Contains Seats object for server interaction
    /// As server always return Order object we have to pesist its updated state with SelectedSpots field
    /// </summary>
    /// <remarks>Persisted object</remarks>
    public class Order
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        private string _token = string.Empty;
        /// <summary>
        /// Token is key to change the order
        /// </summary>
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        private string _actionName = string.Empty;
        public string ActionName
        {
            get { return _actionName; }
            set { _actionName = value; }
        }

        private string _member = string.Empty;

        /// <summary>
        /// Member name, who reserved the order
        /// </summary>
        public string Member
        {
            get { return _member; }
            set { _member = value; }
        }

        public ItemStatus Status { get; set; }
        public DateTime ActionDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public PaymentType PaymentType { get; set; }

        private string _paymentUri = string.Empty;
        /// <summary>
        /// PaymentUri is Yandex Money payment request url 
        /// </summary>
        [XmlIgnore]
        public string PaymentUri
        {
            get { return _paymentUri; }
            set { _paymentUri = value; }
        }

        /// <summary>
        /// Shows items passed the last operation
        /// </summary>
        public int ItemsCount { get; set; }

        /// <summary>
        /// Used for reserve and pay seats 
        /// </summary>
        public List<Seat> Seats { get; set; }
    }
}