using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using TicketShop.Core;
using TicketShop.Print.Base;
using TicketShop.Print.Plugin;

namespace TicketShop.Print
{
    public class PrintRequest
    {
        #region EventHandlers

        public event NetworkDelegate OnPrinterDiscoveryBegin;

        protected void PrinterDiscoveryBegin(NetworkEventArgs e)
        {
            if (OnPrinterDiscoveryBegin != null)
                OnPrinterDiscoveryBegin(this, e);
        }

        public event NetworkDelegate OnPrinterDiscoveryCompleted;

        protected void PrinterDiscoveryCompleted(NetworkEventArgs e)
        {
            if (OnPrinterDiscoveryCompleted != null)
                OnPrinterDiscoveryCompleted(this, e);
        }

        /// <summary>
        /// The printer is found
        /// </summary>
        public event PrintDelegate OnPrinting;

        protected void Printing(PrinterEventArgs e)
        {
            if (OnPrinting != null)
                OnPrinting(this, e);
        }

        /// <summary>
        /// Printing is over
        /// </summary>
        public event PrintDelegate OnPrinted;
        protected void Printed(PrinterEventArgs e)
        {
            if (OnPrinted != null)
                OnPrinted(this, e);
        }

        #endregion

        /// <summary>
        /// Принтеры, ключ - мак адресс
        /// </summary>
        public Dictionary<string, Printer> Printers = new Dictionary<string, Printer>();

        /// <summary>
        /// Принтер, который нашел сканер и который будет использоваться далее.
        /// </summary>
        private Printer _printer;

        private void ScanNetworkForPrinter()
        {
            IPAddress ipAddress;
            string msg = "Некорретный IP адрес принтера или маска сети";
            if (IPAddress.TryParse(_printer.PrinterIp, out ipAddress))
            {
                IPAddress networkMask;
                if (IPAddress.TryParse(_printer.NetworkMask, out networkMask))
                {
                    msg = "Поиск принтера в локальной сети";
                    PrinterDiscoveryBegin(new NetworkEventArgs(_printer, msg));
                    _printer = PrinterPinger.ScanNetwork(ipAddress, networkMask);

                    if (_printer.IsFound)
                    {
                        msg = String.Format("Принтер подключен по адресу {0}", _printer.PrinterIp);
                    }
                    else
                    {
                        msg = "Принтер не найден в локальной сети";
                    }
                }
            }

            PrinterDiscoveryCompleted(new NetworkEventArgs(_printer, msg));
        }

        public void Begin(object state)
        {
            _printer = (Printer)state;
            string ipAddress = _printer.PrinterIp;
            string mask = _printer.NetworkMask;

            PrinterDiscoveryBegin(new NetworkEventArgs(_printer, "Инициализация плагина печати"));
            bool completed = TargetPlugin.Instance.Init();
            if (completed)
            {
                PrinterDiscoveryBegin(new NetworkEventArgs(_printer, "Плагин печати инициализирован"));

                if (!string.IsNullOrEmpty(ipAddress))
                {
                    PrinterDiscoveryBegin(new NetworkEventArgs(_printer, String.Format("Поиск принтера по адресу {0}", ipAddress)));

                    bool pingResult = PingHost(ipAddress);
                    if (pingResult)
                    {
                        TargetPlugin.Instance.SetIp(ipAddress);
                        Dictionary<string, object> printerInfos = TargetPlugin.Instance.RunQuery("");
                        if (printerInfos.Count > 1)
                        {
                            foreach (KeyValuePair<string, object> en in printerInfos)
                            {
                                _printer.PrinterInfo += en.Value.ToString();
                            }

                            _printer.IsFound = true;
                            PrinterDiscoveryCompleted(new NetworkEventArgs(_printer, String.Format("Принтер подключен по адресу {0}", ipAddress)));
                        }
                        else
                        {
                            _printer.IsFound = false;
                            PrinterDiscoveryCompleted(new NetworkEventArgs(_printer, String.Format("Принтер по адресу {0} не найден", ipAddress)));
                            if (_printer.AutoSearch)
                            {
                                ScanNetworkForPrinter();
                            }
                        }
                    }
                    else
                    {
                        _printer.IsFound = false;
                        PrinterDiscoveryCompleted(new NetworkEventArgs(_printer, String.Format("Принтер {0} не отвечает", ipAddress)));
                        if (_printer.AutoSearch)
                        {
                            ScanNetworkForPrinter();
                        }
                    }
                }
                else
                {
                    _printer.IsFound = false;
                    PrinterDiscoveryCompleted(new NetworkEventArgs(_printer, "IP адрес принтера не указан"));
                    if (_printer.AutoSearch)
                    {
                        ScanNetworkForPrinter();
                    }
                }
            }
            else
            {
                _printer.IsFound = false;
                PrinterDiscoveryCompleted(new NetworkEventArgs(_printer, "Плагин печати не инициализирован"));
            }
        }

        public static bool PingHost(string nameOrAddress)
        {
            bool result = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(nameOrAddress);
                if (reply != null) result = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // ignore
            }
            return result;
        }

        public bool Print(byte[] imageBytes)
        {
            return TargetPlugin.Instance.Print(imageBytes);
        }

        public void Setup()
        {
            TargetPlugin.Instance.Setup();
            //Thread.Sleep(3000);
            //TargetPlugin.Instance.Calibrate();
        }
    }
}