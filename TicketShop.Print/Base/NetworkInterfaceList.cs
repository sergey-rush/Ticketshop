using System.Collections.Generic;
using System.Net;

namespace TicketShop.Print.Base
{
    public class NetworkInterfaceList : List<IPAddress>
    {
        public NetworkInterfaceList()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.ToString().Contains("."))
                    Add(ip);
            }
        }
    }
}