using Newtonsoft.Json;
using System.Windows.Media;

namespace TicketShop.Core
{
    public class Point
    {
        [JsonProperty(PropertyName = "id")]
        public int SeatId { get; set; }
        [JsonProperty(PropertyName = "side")]
        public string SideName { get; set; }
        [JsonProperty(PropertyName = "sector")]
        public string SectorName { get; set; }
        [JsonProperty(PropertyName = "row")]
        public string RowNum { get; set; }
        [JsonProperty(PropertyName = "seat")]
        public string SeatNum { get; set; }
        [JsonProperty(PropertyName = "x")]
        public int XPos { get; set; }
        [JsonProperty(PropertyName = "y")]
        public int YPos { get; set; }
        [JsonProperty(PropertyName = "price")]
        public decimal Price { get; set; }
        [JsonProperty(PropertyName = "status")]
        public ItemStatus SeatStatus { get; set; }
        [JsonIgnore]
        public Color Color { get; set; }
    }
}