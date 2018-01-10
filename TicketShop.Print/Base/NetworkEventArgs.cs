using System;
using TicketShop.Core;

namespace TicketShop.Print.Base
{
    public delegate void NetworkDelegate(object sender, NetworkEventArgs e);

    public class NetworkEventArgs : EventArgs
    {
        private Printer _printer;

        public Printer Printer
        {
            get { return _printer; }
        }

        private string _message;

        public string Message
        {
            get { return _message; }
        }

        public NetworkEventArgs(Printer printer, string message)
        {
            _printer = printer;
            _message = message;
        }
    }
}