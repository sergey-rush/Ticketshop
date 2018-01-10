using TicketShop.Core;

namespace TicketShop.Shell.Models
{
    public class ComboBoxKey
    {
        public string Name { get; set; }
        public string Value { get; set; }
    
        public ItemStatus ValueItemStatusEnum { get; set; }
        public string ValueItemStatusString { get; set; }
    }
}