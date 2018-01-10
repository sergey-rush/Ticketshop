using System;
using System.Collections.Generic;
using TicketShop.Core;

namespace DatamaxPlugin
{
    public class DataMax : IPlugin
    {
        private string _name = "Datamax E-Class Mark III";
        public string GetName()
        {
            return _name;
        }

        private string _ip = String.Empty;
        public void SetIp(string ip)
        {
            _ip = ip;
        }

        private int printPort = 9100;

        public bool Print(byte[] bytes)
        {
           return PrintProvider.Print(bytes, _ip, printPort);
        }

        public bool IsReady()
        {
            var isOk = false;
            var isWasBusy = false;
                
                var status = PrintProvider.GetExtendedStatus(out isOk);
                if (isOk == false)
                    

                if (isWasBusy == false)
                {
                    if (status[ExtendedStatus.BusyPrinting] == true)
                        isWasBusy = true;
                    
                }
                if (isWasBusy && status[ExtendedStatus.BusyPrinting] == false
                    && status[ExtendedStatus.Ready] == true)
                {
                    //Log("info: I was waiting for: " + (Environment.TickCount - start) + " ms. and got Busy state returned to Ready!");
                    return true;
                }

            
            return false;
        }

        public Dictionary<string, object> RunQuery(string clause)
        {
            return PrintProvider.RunQuery(clause, _ip, printPort);
        }
        
        public void Calibrate()
        {
            PrintProvider.Calibrate(_ip, printPort);
        }

        public void Setup()
        {
            PrintProvider.Setup(_ip, printPort);
        }
    }
}
