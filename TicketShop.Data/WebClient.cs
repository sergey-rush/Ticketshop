using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net;
using TicketShop.Core;

namespace TicketShop.Data
{
    public class WebClient : WebManager
    {
        #region GET Methods

        public override IEnumerable<Provider> GetProviders()
        {
            Uri uri = new Uri(Keys["ProvidersUri"]);
            return HttpFactory.ProcessRequest<IEnumerable<Provider>, Type>(uri, HttpMethod.GET, null);
        }

        public override Stage GetStage(int stageId)
        {
            Uri uri = new Uri(String.Format("{0}/stageId/{1}", Keys["StageUri"], stageId));
            return HttpFactory.ProcessRequest<Stage, Type>(uri, HttpMethod.GET, null);
        }

        public override IEnumerable<ShowAction> GetActions(int providerId)
        {
            Uri uri = new Uri(String.Format("{0}/providerId/{1}", Keys["ActionsUri"], providerId));
            return HttpFactory.ProcessRequest<IEnumerable<ShowAction>, Type>(uri, HttpMethod.GET, null);
        }

        public override ShowAction GetAction(int actionId)
        {
            Uri uri = new Uri(String.Format("{0}/actionId/{1}", Keys["ActionUri"], actionId));
            return HttpFactory.ProcessRequest<ShowAction, Type>(uri, HttpMethod.GET, null);
        }

        public override IEnumerable<EventDate> GetEvents(int actionId)
        {
            Uri uri = new Uri(String.Format("{0}/actionId/{1}", Keys["EventsUri"], actionId));
            return HttpFactory.ProcessRequest<IEnumerable<EventDate>, Type>(uri, HttpMethod.GET, null);
        }

        public override EventDate GetEvent(int eventId)
        {
            Uri uri = new Uri(String.Format("{0}/eventId/{1}", Keys["EventUri"], eventId));
            return HttpFactory.ProcessRequest<EventDate, Type>(uri, HttpMethod.GET, null);
        }

        public override IEnumerable<Hall> GetHallSeats(int eventId, ItemStatus status)
        {
            Uri uri = new Uri(String.Format("{0}/eventId/{1}/status/{2}", Keys["SeatStatsUri"], eventId, status));
            return HttpFactory.ProcessRequest<IEnumerable<Hall>, Type>(uri, HttpMethod.GET, null);
        }

        public override byte[] GetSeatingMap(int stageId)
        {
            Uri uri = new Uri(String.Format("{0}/stageId/{1}", Keys["SeatingMapUri"], stageId));
            return HttpFactory.ProcessRequest(uri, HttpMethod.GET);
        }

        public override IEnumerable<Core.Point> GetMapSeatPoints(int eventId)
        {
            Uri uri = new Uri(String.Format("{0}/eventId/{1}/status/onsale", Keys["MapSeatPointsUri"], eventId));
            return HttpFactory.ProcessRequest<IEnumerable<Core.Point>, Type>(uri, HttpMethod.GET, null);
        }

        public override byte[] GetTicketPrinted(int orderId, int seatId, int formId)
        {
            Uri uri = new Uri(String.Format("{0}/orderId/{1}/seatId/{2}/formId/{3}", Keys["PrintUri"], orderId, seatId, formId));
            return HttpFactory.ProcessRequest(uri, HttpMethod.GET);
        }

        public override IEnumerable<Ticket> GetTickets(int orderId)
        {
            Uri uri = new Uri(String.Format("{0}/orderId/{1}", Keys["TicketsUri"], orderId));
            return HttpFactory.ProcessRequest<IEnumerable<Ticket>, Type>(uri, HttpMethod.GET, null);
        }

        public override IEnumerable<Order> GetOrders(int index, int rows, int providerId, ItemStatus status, string query)
        {
            Uri uri;
            if (string.IsNullOrEmpty(query))
            {
                uri = new Uri(String.Format("{0}/index/{1}/rows/{2}/providerid/{3}/status/{4}",Keys["OrdersUri"], index, rows, providerId, status));
            }
            else
            {
                uri = new Uri(String.Format("{0}/index/{1}/rows/{2}/providerid/{3}/status/{4}/query/{5}", Keys["OrdersUri"], index, rows, providerId, status, query));
            }
            return HttpFactory.ProcessRequest<IEnumerable<Order>, Type>(uri, HttpMethod.GET, null);
        }

        public override Customer GetCustomer(int customerId)
        {
            Uri uri = new Uri(String.Format("{0}/customerId/{1}", Keys["CustomerUri"], customerId));
            return HttpFactory.ProcessRequest<Customer, Type>(uri, HttpMethod.GET, null);
        }

        public override IEnumerable<Blank> GetBlanks(int index, int rows, ItemStatus status)
        {
            Uri uri = new Uri(String.Format("{0}/index/{1}/rows/{2}/status/{3}", Keys["BlanksUri"], index, rows, status));
            return HttpFactory.ProcessRequest<IEnumerable<Blank>, Type>(uri, HttpMethod.GET, null);
        }

        public override IEnumerable<Ticket> GetReports(string selectedDate)
        {
            Uri uri = new Uri(String.Format("{0}/date/{1}", Keys["ReportsUri"], selectedDate));
            return HttpFactory.ProcessRequest<IEnumerable<Ticket>, Type>(uri, HttpMethod.GET, null);
        }

        public override byte[] GetReport(int memberId, string selectedDate)
        {
            Uri uri = new Uri(String.Format("{0}?type={1}&memberId={2}&date={3}", Keys["ReportUri"], "CashierDaily", memberId, selectedDate));
            return HttpFactory.ProcessRequest(uri, HttpMethod.GET);
        }

        #endregion

        #region DELETE Methods

        public override List<Seat> ReleaseSeats(List<Seat> seats)
        {
            Uri uri = new Uri(Keys["ReleaseUri"]);
            return HttpFactory.ProcessRequest<List<Seat>, List<Seat>>(uri, HttpMethod.DELETE, seats);
        }

        public override Order Cancel(Order order)
        {
            Uri uri = new Uri(Keys["CancelUri"]);
            return HttpFactory.ProcessRequest<Order, Order>(uri, HttpMethod.DELETE, order);
        }

        public override Blank Annul(Blank blank)
        {
            Uri uri = new Uri(Keys["AnnulUri"]);
            return HttpFactory.ProcessRequest<Blank, Blank>(uri, HttpMethod.DELETE, blank);
        }

        #endregion

        #region PUT Methods

        public override List<Seat> LockSeats(List<Seat> seats)
        {
            Uri uri = new Uri(Keys["LockUri"]);
            return HttpFactory.ProcessRequest<List<Seat>, List<Seat>>(uri, HttpMethod.PUT, seats);
        }

        public override Order ConfirmPayment(Order order)
        {
            Uri uri = new Uri(Keys["ConfirmPaymentUri"]);
            return HttpFactory.ProcessRequest<Order, Order>(uri, HttpMethod.PUT, order);
        }

        public override Ticket SetTicketPrinted(Ticket ticket)
        {
            Uri uri = new Uri(Keys["SetTicketPrintedUri"]);
            return HttpFactory.ProcessRequest<Ticket, Ticket>(uri, HttpMethod.PUT, ticket);
        }

        #endregion

        #region POST Methods

        public override Member Login(Member member)
        {
            Uri uri = new Uri(String.Format("{0}/login", Keys["LoginUri"]));
            return HttpFactory.ProcessRequest<Member, Member>(uri, HttpMethod.POST, member);
        }
        
        public override Order CreateOrder(Customer customer)
        {
            Uri uri = new Uri(Keys["CreateOrderUri"]);
            return HttpFactory.ProcessRequest<Order, Customer>(uri, HttpMethod.POST, customer);
        }

        public override Order UpdateOrder(Customer customer)
        {
            Uri uri = new Uri(Keys["UpdateOrderUri"]);
            return HttpFactory.ProcessRequest<Order, Customer>(uri, HttpMethod.POST, customer);
        }

        public override string SendLog(Log log)
        {
            Uri uri = new Uri(Keys["SendLogUri"]);
            return HttpFactory.ProcessRequest<string, Log>(uri, HttpMethod.POST, log);
        }

        public override string SendMessage(Log log)
        {
            Uri uri = new Uri(Keys["SendMessageUri"]);
            return HttpFactory.ProcessRequest<string, Log>(uri, HttpMethod.POST, log);
        }

        #endregion
    }
}
