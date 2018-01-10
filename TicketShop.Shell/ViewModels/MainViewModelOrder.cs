using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using TicketShop.Core;
using TicketShop.Data;
using TicketShop.Shell.Commands;
using TicketShop.Shell.Models;
using TicketShop.Shell.Views;

namespace TicketShop.Shell.ViewModels
{
    public partial class MainViewModel
    {
        private DelegateCommand _cancelTicket;

        public ICommand CancelTicket
        {
            get
            {
                if (_cancelTicket == null)
                {
                    _cancelTicket = new DelegateCommand(OnCancelTicket, CanCancelTicket);
                }
                return _cancelTicket;
            }
        }

        private void OnCancelTicket()
        {
            Ticket t = SelectedTicket;
            if (t != null)
            {
                ConfirmationBox cb = new ConfirmationBox
                {
                    Message =
                        String.Format(
                            "Вы действительно желаете аннулировать билет № {0}, {1}, {2}, {3} ряд, {4} место?",
                            t.Id, t.SideName, t.SectorName, t.RowNum, t.SeatNum)
                };
                bool? result = cb.ShowDialog();
                if (result ?? false)
                {
                    Customer.Order.Seats = new List<Seat> {new Seat() {Id = t.Id}};
                    Customer.Order = Customer.Order = Access.Cancel(Customer.Order);
                    ReloadOrder();
                }
            }
        }

        private bool CanCancelTicket()
        {
            bool result = true;
            Ticket t = SelectedTicket;
            if (t != null)
            {
                if (Customer.Order.Status == ItemStatus.Cancelled || Customer.Order.Status == ItemStatus.Sold)
                {
                    result = false;
                }
                else
                {
                    if (t != null && t.Status == ItemStatus.Cancelled)
                    {
                        result = false;
                    }
                }
            }
            return result;
        }

        private DelegateCommand _cancelOrder;

        public ICommand CancelOrder
        {
            get
            {
                if (_cancelOrder == null)
                {
                    _cancelOrder = new DelegateCommand(OnCancelOrder, CanCancelOrder);
                }
                return _cancelOrder;
            }
        }

        private void OnCancelOrder()
        {
            ConfirmationBox cb = new ConfirmationBox
            {
                Message = String.Format("Вы действительно желаете отменить заказ № {0}?", Customer.Order.Id)
            };
            bool? result = cb.ShowDialog();
            if (result ?? false)
            {
                Customer.Order = Customer.Order = Access.Cancel(Customer.Order);
                ReloadOrder();
            }
        }

        private bool CanCancelOrder()
        {
            bool result = false;
            if (Customer.Order != null)
            {
                result = Customer.Order.Status == ItemStatus.Reserved;
            }

            return result;
        }

        private DelegateCommand _updateOrder;

        public ICommand UpdateOrder
        {
            get
            {
                if (_updateOrder == null)
                {
                    _updateOrder = new DelegateCommand(OnUpdateOrder, CanUpdateOrder);
                }
                return _updateOrder;
            }
        }

        private void OnUpdateOrder()
        {
            List<Seat> seats = new List<Seat>();
            foreach (Ticket ticket in Tickets)
            {
                Seat seat = new Seat();
                seat.Id = ticket.Id;
                seat.ExtBarCode = ticket.ExtBarCode;
                seat.ReserveDate = ReserveDate;
                seats.Add(seat);
            }
            Customer.Order.Seats = seats;
            Customer.FirstName = FirstName;
            Customer.MiddleName = MiddleName;
            Customer.LastName = LastName;
            Customer.Email = CustomerEmail;
            Customer.Phone = CustomerPhone;
            Access.UpdateOrder(Customer);
            _orderWindow.Close();
        }

        private bool CanUpdateOrder()
        {
            if (Customer.Order != null)
            {
                return Customer.Order.Status == ItemStatus.Reserved;
            }
            else
            {
                return false;
            }
        }

        private DelegateCommand _purchaseOrder;

        /// <summary>
        /// Handles PurchaseOrder button on OrderWindow
        /// </summary>
        public ICommand PurchaseOrder
        {
            get
            {
                if (_purchaseOrder == null)
                {
                    _purchaseOrder = new DelegateCommand(OnPurchaseOrder, CanPurchaseOrder);
                }
                return _purchaseOrder;
            }
        }

        /// <summary>
        /// Handles PurchaseOrder button on OrderWindow
        /// </summary>
        private void OnPurchaseOrder()
        {
            PaymentTypeBox ptb = new PaymentTypeBox();
            bool? result = ptb.ShowDialog();
            if (result ?? false)
            {
                BlankStatus = String.Empty;

                Customer.Order.Seats = new List<Seat>();
                foreach (Ticket t in Tickets)
                {
                    Seat seat = new Seat()
                    {
                        Id = t.Id
                    };
                    Customer.Order.Seats.Add(seat);
                }

                Customer.Order.PaymentType =
                    (PaymentType) Enum.Parse(typeof (PaymentType), SelectedPaymentType.Key.ToString(), true);

                Customer.Order = Customer.Order = Access.ConfirmPayment(Customer.Order);
                ReloadOrder();

                if (Customer.Order != null && Customer.Order.Status == ItemStatus.Sold)
                {
                    if (ShowTickets && Customer.Order.Seats.Count < 10)
                    {
                        Application.Current.Dispatcher.Invoke((Action) delegate
                        {
                            foreach (Seat seat in Customer.Order.Seats)
                            {
                                MemoryStream ms = GetTicketImage(Customer.Order.Id, seat.Id, 1);
                                if (ms != null)
                                {
                                    Spot spot = SelectedSpots.FirstOrDefault(x => x.Id == seat.Id);
                                    if (spot != null)
                                    {
                                        BitmapImage bi = new BitmapImage();
                                        bi.BeginInit();
                                        bi.StreamSource = ms;
                                        bi.EndInit();
                                        spot.TicketImage = bi;
                                    }
                                }
                            }
                        });
                    }
                }
                else
                {
                    Customer.Order = new Order(); // Just not to be null
                    OperationResult = String.Format("Произошла ошибка. Заказ № {0} не оплачен", Customer.Order.Id);
                }
            }
        }

        private bool CanPurchaseOrder()
        {
            bool result = false;
            if (Customer.Order != null)
            {
                result = Customer.Order.Status == ItemStatus.Reserved;
            }

            return result;
        }

        private void ReloadOrder()
        {
            LoadCustomerAndOrderTickets(Customer.Order);
            OrderDialogTitle = String.Format("Заказ № {0} - {1}", Customer.Order.Id,
                BaseData.GetStatus(Customer.Order.Status));
            OrderStatus = Customer.Order.Status;
            ShowOrders();
        }
    }
}