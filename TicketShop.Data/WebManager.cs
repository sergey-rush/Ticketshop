using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using TicketShop.Core;

namespace TicketShop.Data
{
    public abstract class WebManager
    {
        protected static readonly Dictionary<string, string> Keys = Settings.Current.Keys;

        private static WebManager _current;

        public static WebManager Current
        {
            get
            {
                if (_current == null)
                {
                    _current = new WebClient();
                }
                return _current;
            }
        }

        #region GET Methods

        public abstract IEnumerable<Provider> GetProviders();
        public abstract Stage GetStage(int stageId);
        public abstract IEnumerable<ShowAction> GetActions(int providerId);
        public abstract ShowAction GetAction(int actionId);
        public abstract IEnumerable<EventDate> GetEvents(int actionId);
        public abstract EventDate GetEvent(int eventId);
        public abstract IEnumerable<Hall> GetHallSeats(int eventId, ItemStatus status);
        public abstract byte[] GetSeatingMap(int eventId);
        public abstract IEnumerable<TicketShop.Core.Point> GetMapSeatPoints(int eventId);
        public abstract byte[] GetTicketPrinted(int orderId, int seatId, int formId);
        public abstract IEnumerable<Ticket> GetTickets(int orderId);
        public abstract IEnumerable<Order> GetOrders(int index, int rows, int providerId, ItemStatus status, string query);
        public abstract Customer GetCustomer(int customerId);
        public abstract IEnumerable<Blank> GetBlanks(int index, int rows, ItemStatus status);
        public abstract IEnumerable<Ticket> GetReports(string selectedDate);
        public abstract byte[] GetReport(int memberId, string selectedDate);

        #endregion

        #region DELETE Methods

        public abstract List<Seat> ReleaseSeats(List<Seat> seats);
        public abstract Order Cancel(Order order);
        public abstract Blank Annul(Blank blank);

        #endregion

        #region PUT Methods

        public abstract Order ConfirmPayment(Order order);
        public abstract List<Seat> LockSeats(List<Seat> seats);
        public abstract Ticket SetTicketPrinted(Ticket ticket);

        #endregion

        #region POST Methods

        public abstract Member Login(Member member);
        public abstract Order CreateOrder(Customer customer);
        public abstract Order UpdateOrder(Customer customer);
        public abstract string SendLog(Log log);
        public abstract string SendMessage(Log log);

        #endregion

        public static bool ValidateRemoteCertificate(object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            bool result = true;
            if (policyErrors != SslPolicyErrors.None)
            {
                foreach (X509ChainStatus t in chain.ChainStatus)
                {
                    if (t.Status != X509ChainStatusFlags.RevocationStatusUnknown)
                    {
                        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
                        chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
                        chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
                        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
                        bool chainIsValid = chain.Build((X509Certificate2)cert);
                        if (!chainIsValid)
                        {
                            result = false;
                        }
                    }
                }
            }
            return result;
        }
    }
}
