using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TicketShop.Core
{
    public class Member
    {
        public int Id { get; set; }
        public string Pass { get; set; }
        public string Email { get; set; }
        private string _token = string.Empty;
        /// <summary>
        /// Token is used for API clients only. Token is valid until TokenDate
        /// </summary>
        public string Token
        {
            get { return _token; }
            set { _token = value; }
        }

        private string _fullName = string.Empty;
        public string FullName
        {
            get { return _fullName; }
            set { _fullName = value; }
        }

        private DateTime _tokenDate = DateTime.MinValue;
        /// <summary>
        /// Token is valid until TokenDate
        /// </summary>
        public DateTime TokenDate
        {
            get { return _tokenDate; }
            set { _tokenDate = value; }
        }
        
        private DateTime _startDate = DateTime.MinValue;
        /// <summary>
        /// The application start time point 
        /// </summary>
        [JsonIgnore]
        public DateTime StartDate
        {
            get { return _startDate; }
            set { _startDate = value; }
        }

        public override string ToString()
        {
            return String.Format("Email: {0} Id: {1}", Email, Id);
        }
    }
}