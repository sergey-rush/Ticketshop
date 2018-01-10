using System;

namespace TicketShop.Core
{
    public class EventDate
    {
        public int Id { get; set; }
        public int ActionId { get; set; }
        public string Date { get; set; }
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        private DateTime _actionDate = DateTime.MinValue;
        public DateTime ActionDate
        {
            get { return _actionDate; }
            set { _actionDate = value; }
        }
        
        public int TicketsCount { get; set; }
        public int StageId { get; set; }
    }
}