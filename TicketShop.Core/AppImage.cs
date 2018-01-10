namespace TicketShop.Core
{
    public class AppImage
    {
        public int Id { get; set; }
        
        public string Title { get; set; }
       
        public string Description { get; set; }
        public byte[] ImageBytes { get; set; }
    }
}