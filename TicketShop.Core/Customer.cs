
using System;
using Newtonsoft.Json;

namespace TicketShop.Core
{
    public class Customer
    {
        private string _firstName = string.Empty;
        public string FirstName
        {
            get { return _firstName; }
            set { _firstName = value; }
        }

        private string _middleName = string.Empty;
        public string MiddleName
        {
            get { return _middleName; }
            set { _middleName = value; }
        }

        private string _lastName = string.Empty;
        public string LastName
        {
            get { return _lastName; }
            set { _lastName = value; }
        }

        private string _email = string.Empty;
        public string Email
        {
            get { return _email; }
            set { _email = value; }
        }

        private string _phone = string.Empty;
        public string Phone
        {
            get { return _phone; }
            set { _phone = value; }
        }

        private bool _sendSms;
        public bool SendSms
        {
            get { return _sendSms; }
            set { _sendSms = value; }
        }

        private Order _order;
        public Order Order
        {
            get { return _order; }
            set { _order = value; }
        }

        [JsonIgnore]
        public string Name
        {
            get { return String.Format("{0} {1} {2}", LastName, FirstName, MiddleName); }
        }
    }
}