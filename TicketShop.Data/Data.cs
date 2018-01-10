using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using TicketShop.Core;

namespace TicketShop.Data
{
    public class Access : BaseData
    {
        #region GET Methods

        public static Provider[] GetProviders()
        {
            Provider[] providers = null;
            string key = "Data_GetProviders_";

            if (Cache[key] != null)
            {
                providers = (Provider[]) Cache[key];
            }
            else
            {
                IEnumerable<Provider> items = WebManager.Current.GetProviders();
                if (items != null)
                {
                    providers = items.ToArray();
                    CacheData(key, providers);
                }
            }
            return providers;
        }

        public static Stage GetStage(int stageId)
        {
            Stage stage = null;
            string key = "Data_GetStage_" + stageId;

            if (Cache[key] != null)
            {
                stage = (Stage) Cache[key];
            }
            else
            {
                stage = WebManager.Current.GetStage(stageId);
                CacheData(key, stage);
            }
            return stage;
        }

        public static ShowAction[] GetActions(int providerId)
        {
            ShowAction[] actions = null;
            string key = "Data_GetActions_" + providerId;

            if (Cache[key] != null)
            {
                actions = (ShowAction[]) Cache[key];
            }
            else
            {
                IEnumerable<ShowAction> items = WebManager.Current.GetActions(providerId);
                if (items != null)
                {
                    actions = items.ToArray();
                    CacheData(key, actions);
                }

            }
            return actions;
        }

        public static ShowAction GetAction(int actionId)
        {
            ShowAction action = null;
            string key = "Data_GetAction_" + actionId;

            if (Cache[key] != null)
            {
                action = (ShowAction) Cache[key];
            }
            else
            {
                action = WebManager.Current.GetAction(actionId);
                CacheData(key, action);
            }
            return action;
        }

        public static EventDate[] GetEvents(int actionId)
        {
            EventDate[] events = null;
            string key = "Data_GetEvents_" + actionId;

            if (Cache[key] != null)
            {
                events = (EventDate[]) Cache[key];
            }
            else
            {
                IEnumerable<EventDate> items = WebManager.Current.GetEvents(actionId);
                if (items != null)
                {
                    events = items.ToArray();
                    foreach (EventDate eventDate in events)
                    {
                        eventDate.Date = eventDate.ActionDate.ToString("f");
                    }

                    CacheData(key, events);
                }
            }
            return events;
        }

        public static EventDate GetEvent(int eventId)
        {
            EventDate action = null;
            string key = "Data_GetEvent_" + eventId;

            if (Cache[key] != null)
            {
                action = (EventDate) Cache[key];
            }
            else
            {
                action = WebManager.Current.GetEvent(eventId);
                CacheData(key, action);
            }
            return action;
        }

        public static Hall[] GetHallSeats(int eventId, ItemStatus status)
        {
            Hall[] seats = null;
            string key = "Data_GetHallSeats_" + eventId + "_" + status;
            if (Cache[key] != null)
            {
                seats = (Hall[]) Cache[key];
            }
            else
            {
                IEnumerable<Hall> items = WebManager.Current.GetHallSeats(eventId, status);
                if (items != null)
                {
                    seats = items.ToArray();
                }
                CacheData(key, seats);
            }
            return seats;
        }

        /// <summary>
        /// Downloads a map if not exists
        /// </summary>
        public static Image GetSeatingMap(int stageId)
        {
            Image image = null;
            Stage stage = GetStage(stageId);
            if (stage != null)
            {
                string path = Path.Combine(Environment.CurrentDirectory, stage.ImageMap);
                if (File.Exists(path))
                {
                    image = Image.FromFile(path);
                }
                else
                {
                    using (MemoryStream ms = new MemoryStream(WebManager.Current.GetSeatingMap(stageId)))
                    {
                        image = Image.FromStream(ms);
                        image.Save(path, ImageFormat.Png);
                    }
                }
            }
            return image;
        }

        public static Core.Point[] GetMapSeatPoints(int eventId)
        {
            Core.Point[] seats = null;
            string key = "Data_GetMapSeatPoints_" + eventId;

            if (Cache[key] != null)
            {
                seats = (Core.Point[]) Cache[key];
            }
            else
            {
                IEnumerable<Core.Point> points = WebManager.Current.GetMapSeatPoints(eventId);
                if (points != null)
                {
                    seats = points.ToArray();
                    foreach (Core.Point point in seats)
                    {
                        point.Color = SetColor(point.Price, point.SeatStatus);
                    }

                    CacheData(key, seats);
                }
            }
            return seats;
        }

        public static byte[] GetTicketPrinted(int orderId, int seatId, int formId)
        {
            byte[] image = null;
            string key = "Data_GetTicketImage_" + orderId + "_" + seatId + "_" + formId;

            if (Cache[key] != null)
            {
                image = (byte[])Cache[key];
            }
            else
            {
                image = WebManager.Current.GetTicketPrinted(orderId, seatId, formId);
            }
            return image;
        }

        public static Ticket[] GetTickets(int orderId)
        {
            Ticket[] tickets = null;
            string key = "Data_GetTickets_" + orderId;

            if (Cache[key] != null)
            {
                tickets = (Ticket[]) Cache[key];
            }
            else
            {
                IEnumerable<Ticket> items = WebManager.Current.GetTickets(orderId);
                if (items != null)
                {
                    tickets = items.ToArray();
                    CacheData(key, tickets);
                }
            }
            return tickets;
        }

        public static Order[] GetOrders(int index, int rows, int providerId, ItemStatus status, string query)
        {
            Order[] orders = null;
            string key = "Data_GetOrders_" + index + "_" + rows + "_" + providerId + "_" + status + "_" + query;

            if (Cache[key] != null)
            {
                orders = (Order[]) Cache[key];
            }
            else
            {
                IEnumerable<Order> items = WebManager.Current.GetOrders(index, rows, providerId, status, query);
                if (items != null)
                {
                    orders = items.ToArray();
                    CacheData(key, orders);
                }
            }
            return orders;
        }

        public static Customer GetCustomer(int customerId)
        {
            Customer customer = null;
            string key = "Data_GetCustomer_" + customerId;

            if (Cache[key] != null)
            {
                customer = (Customer) Cache[key];
            }
            else
            {
                customer = WebManager.Current.GetCustomer(customerId);
                CacheData(key, customer);
            }
            return customer;
        }

        public static Blank[] GetBlanks(int index, int rows, ItemStatus status)
        {
            Blank[] blanks = null;
            string key = "Data_GetBlanks_" + index + "_" + rows + "_" + status;

            if (Cache[key] != null)
            {
                blanks = (Blank[]) Cache[key];
            }
            else
            {
                IEnumerable<Blank> items = WebManager.Current.GetBlanks(index, rows, status);
                if (items != null)
                {
                    blanks = items.ToArray();
                    CacheData(key, blanks);
                }
            }
            return blanks;
        }

        public static Ticket[] GetReports(DateTime selectedDate)
        {
            string currentDate = selectedDate.ToString("yyyyMMdd");
            Ticket[] tickets = null;
            IEnumerable<Ticket> items = WebManager.Current.GetReports(currentDate);
            if (items != null)
            {
                tickets = items.ToArray();
            }
            return tickets;
        }

        public static byte[] GetReport(int memberId, DateTime selectedDate)
        {
            string currentDate = selectedDate.ToString("yyyyMMdd");
            byte[] image = WebManager.Current.GetReport(memberId, currentDate);
            return image;
        }

        #endregion

        #region DELETE Methods

        public static Order Cancel(Order order)
        {
            Order result = WebManager.Current.Cancel(order);
            RemoveFromCache("Data_");
            return result;
        }

        /// <summary>
        /// Releases seats on cancel reserving
        /// </summary>
        public static List<Seat> ReleaseSeats(List<Seat> seats)
        {
            List<Seat> result = WebManager.Current.ReleaseSeats(seats);
            RemoveFromCache("Data_");
            return result;
        }

        public static Blank Annul(Blank blank)
        {
            Blank result = WebManager.Current.Annul(blank);
            RemoveFromCache("Data_");
            return result;
        }

        #endregion

        #region PUT Methods

        /// <summary>
        /// Locks seats while reserving
        /// </summary>
        public static List<Seat> LockSeats(List<Seat> seats)
        {
            List<Seat> result = WebManager.Current.LockSeats(seats);
            RemoveFromCache("Data_");
            return result;
        }


        /// <summary>
        /// Confirms Order as paid.
        /// Contains Seat collection
        /// </summary>
        /// <param name="order">Order to be confirmed</param>
        /// <returns>Order confirmation result</returns>
        public static Order ConfirmPayment(Order order)
        {
            Order result = WebManager.Current.ConfirmPayment(order);
            RemoveFromCache("Data_");
            return result;
        }

        public static Ticket SetTicketPrinted(Ticket ticket)
        {
            Ticket t = WebManager.Current.SetTicketPrinted(ticket);
            RemoveFromCache("Data_");
            return t;
        }

        #endregion

        #region POST Methods

        public static Member Login(Member member)
        {
            member = WebManager.Current.Login(member);
            if (member != null)
            {
                HttpFactory.Token = member.Token;
            }
            RemoveFromCache("Data_");
            return member;
        }

        /// <summary>
        /// Creates order, reserves seats, registers customer
        /// </summary>
        /// <param name="customer">Person purchased the order</param>
        /// <returns>Order with seats with field Result as purchasing status of each ticket</returns>
        public static Order CreateOrder(Customer customer)
        {
            Order order = WebManager.Current.CreateOrder(customer);
            RemoveFromCache("Data_");
            return order;
        }

        public static Order UpdateOrder(Customer customer)
        {
            Order order = WebManager.Current.UpdateOrder(customer);
            RemoveFromCache("Data_");
            return order;
        }

        public static bool SendLog(Log log)
        {
            bool result = false;
            string response = WebManager.Current.SendLog(log);
            if (Boolean.TryParse(response, out result))
            {
                
            }
            return result;
        }

        public static bool SendMessage(Log log)
        {
            bool result = false;
            string response = WebManager.Current.SendMessage(log);
            if (Boolean.TryParse(response, out result))
            {

            }
            return result;
        }

        #endregion
    }
}
