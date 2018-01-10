using System;
using Newtonsoft.Json;

namespace TicketShop.Core
{
    public class Blank: IComparable<Blank>
    {
        public int Id { get; set; }
        public int ProviderId { get; set; }
        public string Serial { get; set; }
        public int Pack { get; set; }
        public int Num { get; set; }
        public ItemStatus Status { get; set; }
        public int MemberId { get; set; }
        public string Info { get; set; }
        public DateTime UpdatedDate { get; set; }
        
        [JsonIgnore]
        public string Key
        {
            get
            {
                return String.Format("{0} {1}", Serial, Num);
            }
        }

        public int CompareTo(Blank other)
        {
            if (Id > other.Id) return -1;
            if (Id == other.Id) return 0;
            return 1;
        }
    }
}