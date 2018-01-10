namespace TicketShop.Core
{
    public class ShowAction
    {
        public int Id { get; set; }
        private string _name = string.Empty;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
        public int EventsCount { get; set; }
    }
}