using System;

namespace TicketShop.Print.Base
{
    public delegate void PrintDelegate(object sender, PrinterEventArgs e);

    public class PrinterEventArgs : EventArgs
    {
        private string _message;

        public string Message
        {
            get { return _message; }
        }

        public PrinterEventArgs(string message)
        {
            
            _message = message;
        }
    }
}