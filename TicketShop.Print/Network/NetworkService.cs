using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace TicketShop.Print.Network
{
    public class NetworkService
    {
        public static IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIpAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIpAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIpAddressInformation.Address))
                        {
                            return unicastIpAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }
 
    }
}