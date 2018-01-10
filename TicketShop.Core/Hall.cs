using Newtonsoft.Json;

namespace TicketShop.Core
{
    public class Hall
    {
        [JsonProperty(PropertyName = "SideNum")]
        public virtual int SideId { get; set; }
        [JsonProperty(PropertyName = "SideName")]
        public virtual string SideName { get; set; }
        [JsonProperty(PropertyName = "SectorNum")]
        public virtual int SectorId { get; set; }
        [JsonProperty(PropertyName = "SectorName")]
        public virtual string SectorName { get; set; }
        [JsonProperty(PropertyName = "SeatsCount")]
        public virtual int SeatsCount { get; set; }
        [JsonProperty(PropertyName = "MinPrice")]
        public virtual decimal MinPrice { get; set; }
        [JsonProperty(PropertyName = "MaxPrice")]
        public virtual decimal MaxPrice { get; set; } 
    }
}