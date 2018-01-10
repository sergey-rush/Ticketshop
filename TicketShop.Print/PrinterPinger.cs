using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TicketShop.Core;
using TicketShop.Print.Plugin;

namespace TicketShop.Print
{
    /// <summary>
    /// Класс сканирует сеть на предмет поиска принетров DataMax
    /// </summary>
    internal static class PrinterPinger
    {
        /// <summary>
        /// Результат поиска принтеров
        /// </summary>
        private static readonly List<Printer> Printers = new List<Printer>();
        private static IPAddress _userIp;
        private static IPAddress _userMask;
        
        /// <summary>
        /// Давайте просканируем всю сеть, на вход IP желаемоего принтера,
        /// если не найдено сканируем всю сеть.
        /// </summary>
        internal static Printer ScanNetwork(IPAddress userIp, IPAddress userMask)
        {
            _userIp = userIp;
            _userMask = userMask;

            Printers.Clear();
            return PingHosts(GetIpAddressRange());
        }

        /// <summary>
        /// Возвращает список IP адресов локальной сети
        /// </summary>
        private static IEnumerable<IPAddress> GetIpAddressRange()
        {
            List<IPAddress> result = new List<IPAddress>();
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                if (ni.NetworkInterfaceType == NetworkInterfaceType.Wireless80211 ||
                    ni.NetworkInterfaceType == NetworkInterfaceType.Ethernet)
                {

                    var ipProps = ni.GetIPProperties();
                    IEnumerable<UnicastIPAddressInformation> ipv4Addrs = ipProps.UnicastAddresses.Where(addr => addr.Address.AddressFamily == AddressFamily.InterNetwork);

                    foreach (var addr in ipv4Addrs)
                    {
                        var network = CalculateNetwork(addr);
                        var userNetwork = CalculateUserNetwork();
                        if (network != null)
                        {
                            if (IsAddressOnSubnet(network, userNetwork, _userMask))
                            {
                                IPAddress mask = addr.IPv4Mask;
                                if (Equals(mask, _userMask))
                                {
                                    IEnumerable<IPAddress> array = GetListIpInNetwork(network, mask);
                                    result.AddRange(array);
                                }
                            }
                        }
                    }
                }
            }
            return result;
        }

        private static IPAddress GetLastAddress(IPAddress netAddress, IPAddress netmask)
        {
            uint ip = ToInt(netAddress);
            uint mask = ToInt(netmask);
            uint lastIp = (ip & mask) + ~mask;
            lastIp -= 16777216;
            return ToAddr(lastIp);
        }

        private static IPAddress GetFirstAddress(IPAddress netAddress, IPAddress netmask)
        {
            uint ip = ToInt(netAddress);
            uint mask = ToInt(netmask);
            uint firstIp = (ip & mask);

            firstIp += 16777216;

            IPAddress firstHost = ToAddr(firstIp);
            return firstHost;
        }

        private static uint ToInt(IPAddress addr)
        {
            return BitConverter.ToUInt32(addr.GetAddressBytes(), 0);
        }

        private static IPAddress ToAddr(uint address)
        {
            return new IPAddress(address);
        }

        private static List<IPAddress> GetListIpInNetwork(IPAddress netAddress, IPAddress netmaskIp)
        {
            List<IPAddress> ipInNetwork = new List<IPAddress>();

            IPAddress first = GetFirstAddress(netAddress, netmaskIp);
            IPAddress last = GetLastAddress(netAddress, netmaskIp);

            string[] stringStart = first.ToString().Split('.');
            string[] stringEnd = last.ToString().Split('.');

            int a = int.Parse(stringStart[0]);
            int b = int.Parse(stringStart[1]);
            int c = int.Parse(stringStart[2]);
            int d = int.Parse(stringStart[3]);

            int start = BitConverter.ToInt32(new byte[] { (byte)d, (byte)c, (byte)b, (byte)a }, 0);

            a = int.Parse(stringEnd[0]);
            b = int.Parse(stringEnd[1]);
            c = int.Parse(stringEnd[2]);
            d = int.Parse(stringEnd[3]);

            int end = BitConverter.ToInt32(new byte[] { (byte)d, (byte)c, (byte)b, (byte)a }, 0);

            for (int i = start; i <= end; i++)
            {
                byte[] bytes = BitConverter.GetBytes(i);
                var temp = new IPAddress(new[] { bytes[3], bytes[2], bytes[1], bytes[0] });
                ipInNetwork.Add(temp);
            }

            return ipInNetwork;
        }

        private static bool IsAddressOnSubnet(IPAddress address, IPAddress subnet, IPAddress mask)
        {
            Byte[] addressOctets = address.GetAddressBytes();
            Byte[] subnetOctets = mask.GetAddressBytes();
            Byte[] networkOctets = subnet.GetAddressBytes();

            return
                ((networkOctets[0] & subnetOctets[0]) == (addressOctets[0] & subnetOctets[0])) &&
                ((networkOctets[1] & subnetOctets[1]) == (addressOctets[1] & subnetOctets[1])) &&
                ((networkOctets[2] & subnetOctets[2]) == (addressOctets[2] & subnetOctets[2])) &&
                ((networkOctets[3] & subnetOctets[3]) == (addressOctets[3] & subnetOctets[3]));

        }

        private static IPAddress CalculateNetwork(UnicastIPAddressInformation addr)
        {
            if (addr.IPv4Mask == null)
                return null;

            var ip = addr.Address.GetAddressBytes();
            var mask = addr.IPv4Mask.GetAddressBytes();
            var result = new Byte[4];
            for (int i = 0; i < 4; ++i)
            {
                result[i] = (Byte)(ip[i] & mask[i]);
            }

            return new IPAddress(result);
        }
        
        private static IPAddress CalculateUserNetwork()
        {
            byte[] ip = _userIp.GetAddressBytes();
            byte[] mask = _userMask.GetAddressBytes();
            var result = new Byte[4];
            for (int i = 0; i < 4; ++i)
            {
                result[i] = (Byte)(ip[i] & mask[i]);
            }

            return new IPAddress(result);
        }

        private static Task<PingReply> PingAsync(IPAddress ipaddress)
        {
            var tcs = new TaskCompletionSource<PingReply>();
            AutoResetEvent are = new AutoResetEvent(false);

            Ping ping = new Ping();

            PingOptions options = new PingOptions();
            options.DontFragment = true;
            string data = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa";
            byte[] buffer = Encoding.ASCII.GetBytes(data);

            ping.PingCompleted += (obj, sender) =>
            {
                if (sender.Reply.Status == IPStatus.Success)
                {
                    Printers.Add(new Printer {PrinterIp = sender.Reply.Address.ToString()});
                }
                tcs.SetResult(sender.Reply);
            };

            try
            {
                ping.SendAsync(ipaddress, 3000, buffer, options);
            }
            catch (Exception)
            {
                tcs.SetResult(null);
            }

            return tcs.Task;
        }

        private static Printer PingHosts(IEnumerable<IPAddress> address)
        {
            Task.WaitAll(address.Select(ip => PingAsync(ip)).ToArray());
            int i = 0;
            while (i < Printers.Count)
            {
                Printer printer = Printers[i];
                TargetPlugin.Instance.SetIp(printer.PrinterIp);
                Dictionary<string, object> printerInfos = TargetPlugin.Instance.RunQuery("");
                if (printerInfos.Count > 0)
                {
                    printer.IsFound = true;
                    return printer;
                }
                else
                {
                    Printers.RemoveAt(i);
                    i--;
                }
                i++;
            }
            return new Printer();
        }
    }
}
