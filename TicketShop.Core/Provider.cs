using System;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace TicketShop.Core
{
    public class Provider
    {
        public int Id { get; set; }
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int ActionsCount { get; set; }
        public int EventsCount { get; set; }
        public int TicketsCount { get; set; }
    }
}