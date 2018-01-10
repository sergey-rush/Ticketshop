using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TicketShop.Core;
using TicketShop.Data;
using TicketShop.Shell.Commands;
using TicketShop.Shell.Models;
using TicketShop.Shell.Views;

namespace TicketShop.Shell.ViewModels
{
    public partial class MainViewModel
    {
        private DelegateCommand _reserveSeats;

        /// <summary>
        /// Opens wizard window with ReserveSeats view
        /// </summary>
        public ICommand ReserveSeats
        {
            get
            {
                if (_reserveSeats == null)
                {
                    _reserveSeats = new DelegateCommand(OnReserveSeats, CanReserveSeats);
                }
                return _reserveSeats;
            }
        }

        /// <summary>
        /// Opens wizard window with ReserveSeats view
        /// Locks selected seats
        /// </summary>
        private void OnReserveSeats()
        {
            if (_wizardWindow == null)
            {
                WizardTitle = "Создать заказ";
                FirstName = String.Empty;
                MiddleName = String.Empty;
                LastName = String.Empty;
                CustomerEmail = String.Empty;
                CustomerPhone = String.Empty;
                SendSms = false;

                LockSeats(SelectedSpots.ToArray());
                _wisardStep = WisardStep.Locked;

                _wizardWindow = new WizardWindow {Owner = MetroWindow};

                ContentWindow = new ReserveViewModel(Instance);
                _wizardWindow.ShowDialog();
            }
        }

        private bool CanReserveSeats()
        {
            return SelectedSpots.Count > 0;
        }

        private DelegateCommand _createOrder;

        /// <summary>
        /// Handles CreateOrder button event and open PurchaseView in wizard
        /// </summary>
        public ICommand CreateOrder
        {
            get
            {
                if (_createOrder == null)
                {
                    _createOrder = new DelegateCommand(OnCreateOrder, CanCreateOrder);
                }
                return _createOrder;
            }
        }

        /// <summary>
        /// Handles CreateOrder button event and open PurchaseView in wizard
        /// </summary>
        private void OnCreateOrder()
        {
            Logger.Default.Append("OnCreateOrder Begin");
            InitCustomer();
            
            _worker = new BackgroundWorker();
            _worker.DoWork += delegate
            {
                WizardProgressVisibility = Visibility.Visible;

                Customer.Order = Access.CreateOrder(Customer);
                
                if (Customer.Order != null)
                {
                    PersistSelectedSpots(Customer.Order.Seats);

                    OrderPrice = SelectedSpots.Where(x => x.Result == Core.OperationResult.Success).Sum(x => x.Price);

                    StatusText = OperationResult = String.Format("Заказ № {0} создан. Сумма заказа: {1}", Customer.Order.Id, OrderPrice.ToString("C"));

                    if (_wizardWindow != null)
                    {
                        WizardTitle = "Оплата заказа";
                        ContentWindow = new PurchaseViewModel(Instance);
                    }
                    
                    _wisardStep = WisardStep.OrderCreated;
                }
                else
                {
                    StatusText = OperationResult = "Произошла ошибка. Места не зарезервированы";
                }
                
            };
            _worker.RunWorkerCompleted += delegate
            {
                WizardProgressVisibility = Visibility.Collapsed;
            };
            _worker.RunWorkerAsync();
            
            Logger.Default.Append("OnCreateOrder End");
        }
        
        private bool CanCreateOrder()
        {
            return SelectedSpots.Count > 0;
        }

        private DelegateCommand _payOrder;

        /// <summary>
        /// Handles PayOrder button event and open PrintView in wizard
        /// </summary>
        public ICommand PayOrder
        {
            get
            {
                if (_payOrder == null)
                {
                    _payOrder = new DelegateCommand(OnPayOrder, CanPayOrder);
                }
                return _payOrder;
            }
        }

        /// <summary>
        /// Handles PayOrder button event and open PrintView in wizard
        /// </summary>
        private void OnPayOrder()
        {
            BlankStatus = String.Empty;
            // if View called from orders
            if (Customer.Order.Seats == null)
            {
                Customer.Order.Seats = new List<Seat>();
                foreach (Ticket t in Tickets)
                {
                    Seat seat = new Seat()
                    {
                        Id = t.Id
                    };
                    Customer.Order.Seats.Add(seat);
                }
            }

            Customer.Order.PaymentType =
                (PaymentType) Enum.Parse(typeof (PaymentType), SelectedPaymentType.Key.ToString(), true);

            _worker = new BackgroundWorker();
            _worker.DoWork += delegate
            {
                WizardProgressVisibility = Visibility.Visible;

                Customer.Order = Access.ConfirmPayment(Customer.Order);

                if (Customer.Order != null && Customer.Order.Status == ItemStatus.Sold)
                {
                    Blank[] blanks = Access.GetBlanks(0, Customer.Order.ItemsCount, ItemStatus.OnSale);

                    if (blanks != null)
                    {
                        if (blanks.Length != Customer.Order.ItemsCount)
                        {
                            BlankStatus = "Количество бланков недостаточно для печати заказа";
                        }
                    }
                    else
                    {
                        BlankStatus = "У вас отсутствуют бланки";
                    }

                    for (int i = 0; i < Customer.Order.Seats.Count; i++)
                    {
                        Seat seat = Customer.Order.Seats[i];
                        Spot spot = SelectedSpots.FirstOrDefault(x => x.Id == seat.Id);
                        if (spot != null)
                        {
                            spot.Status = seat.Status;
                            //spot.Result = seat.Result;
                            spot.ReserveDate = seat.ReserveDate;
                            if (blanks != null && blanks.Length > i)
                            {
                                spot.Blank = blanks[i];
                            }
                        }
                    }

                    //PersistSelectedSpots(Customer.Order.Seats);
                    StatusText =
                        OperationResult =
                            String.Format("Заказ № {0} оплачен. Сумма заказа: {1}", Customer.Order.Id,
                                OrderPrice.ToString("C"));

                    if (_wizardWindow != null)
                    {
                        WizardTitle = "Печать заказа";
                        ContentWindow = new PrintViewModel(Instance);
                    }

                    _wisardStep = WisardStep.OrderPaid;

                    DisplayTickets();

                }
                else
                {
                    Customer.Order = new Order(); // Just not to be null
                    StatusText =
                        OperationResult = String.Format("Произошла ошибка. Заказ № {0} не оплачен", Customer.Order.Id);
                }
            };
            _worker.RunWorkerCompleted += delegate
            {
                WizardProgressVisibility = Visibility.Collapsed;
            };
            _worker.RunWorkerAsync();
        }

        /// <summary>
        /// Keeps SelectedSpots updated
        /// </summary>
        private void PersistSelectedSpots(List<Seat> seats)
        {
            foreach (Seat seat in seats)
            {
                Spot spot = SelectedSpots.FirstOrDefault(x => x.Id == seat.Id);
                if (spot != null)
                {
                    spot.Status = seat.Status;
                    spot.Result = seat.Result;
                    spot.ReserveDate = seat.ReserveDate;
                }
            }
        }

        private void DisplayTickets()
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

        private bool CanPayOrder()
        {
            bool result = false;
            if (Customer.Order != null)
            {
                result = Customer.Order.Status == ItemStatus.Reserved;
            }
            return result;
        }

        private DelegateCommand _printTickets;

        /// <summary>
        /// Handles PrintTickets button event and prints tickets
        /// </summary>
        public ICommand PrintTickets
        {
            get
            {
                if (_printTickets == null)
                {
                    _printTickets = new DelegateCommand(OnPrintTickets, CanPrintTickets);
                }
                return _printTickets;
            }
        }

        /// <summary>
        /// Handles PrintTickets button event and prints tickets
        /// </summary>
        private void OnPrintTickets()
        {
            ProcessPrint(SelectedSpots.ToList());
        }
        
        private bool CanPrintTickets()
        {
            //bool result = false;
            //if (PrinterFound)
            //{
            //    if (Tickets.Count > 0 | Customer.Order.Seats.Count>0)
            //    {
            //        if (Customer.Order.Status == ItemStatus.Sold)
            //        {
            //            result = true;
            //        }
            //    }
            //}
            //return result;
            return Customer.Order.Status == ItemStatus.Sold;
        }

        private DelegateCommand _printTicket;

        /// <summary>
        /// Handles PrintTicket button event and print a ticket
        /// </summary>
        public ICommand PrintTicket
        {
            get
            {
                if (_printTicket == null)
                {
                    _printTicket = new DelegateCommand(OnPrintTicket, CanPrintTicket);
                }
                return _printTicket;
            }
        }

        private void OnPrintTicket()
        {
            List<Spot> spots = new List<Spot>();
            
            if (SelectedSpot.Status == ItemStatus.Printed)
            {
                ConfirmationBox cb = new ConfirmationBox
                {
                    Message = String.Format(
                        "Вы действительно желаете перепечатать билет № {0}, {1}, {2}, {3} ряд, {4} место?\nБланк {5} будет аннулирован.",
                        SelectedSpot.Id, SelectedSpot.SideName, SelectedSpot.SectorName, SelectedSpot.RowNum,
                        SelectedSpot.SeatNum, SelectedSpot.Blank.Key)
                };
                bool? result = cb.ShowDialog();
                if (result ?? false)
                {
                    SelectedSpot.Blank = Access.Annul(SelectedSpot.Blank);
                    SelectedSpot.Status = ItemStatus.Sold;
                    spots.Add(SelectedSpot);
                    ProcessPrint(spots);
                }
            }

            if (SelectedSpot.Status == ItemStatus.Sold)
            {
                spots.Add(SelectedSpot);
                ProcessPrint(spots);
            }
        }

        private bool CanPrintTicket()
        {
            return true;
        }
        
        /// <summary>
        /// Prints Ticket collection
        /// </summary>
        private void ProcessPrint(List<Spot> spots)
        {
            if (spots.Count == 0)
            {
                return;
            }

            Spot spotBlank = spots.FirstOrDefault(x => x.Blank == null);
            if (spotBlank != null)
            {
                StatusText = OperationResult ="Недостаточно бланков для печати. Печать отменена";
                return;
            }

            spots.Sort();

            _printProgress = new PrintProgress
            {
                Message = "Идет печать билетов..."
            };

            _printProgress.Cancel += CancelProcess;

            _worker = new BackgroundWorker {WorkerSupportsCancellation = true};
            Dispatcher dispatcher = _progressDialog.Dispatcher;
            int ticketsCount = spots.Count;
            int printedCount = 0;
            _worker.DoWork += delegate(object s, DoWorkEventArgs args)
            {
                decimal percent = 100 / (ticketsCount);
                int progress = 0;

                for (int i = 1; i <= ticketsCount; i++)
                {
                    if (_worker.CancellationPending)
                    {
                        args.Cancel = true;
                        return;
                    }

                    Spot spot = spots[i - 1];
                    if (spot.Status == ItemStatus.Sold)
                    {
                        string msg = string.Format("{0} из {1} - печать билета № {2} на бланке {3}", i, ticketsCount, spot.Id, spot.Blank.Key);
                        _updatePrintDelegate = UpdatePrintText;
                        dispatcher.BeginInvoke(_updatePrintDelegate, progress, msg);

                        byte[] bytes = GetTicketImage(Customer.Order.Id, spot.Id, 2).ToArray();
                        //Thread.Sleep(500);

                        //bool result = _printRequest.Print(bytes);
                        bool result = true;
                        if (!result)
                        {
                            PrinterStatus = string.Format("Ошибка печати билета № {0} ", spot.Id);
                            _worker.CancelAsync();
                        }
                        else
                        {
                            Ticket ticket = new Ticket
                            {
                                Id = spot.Id,
                                BlankId = spot.Blank.Id,
                                Status = ItemStatus.Printed
                            };
                            ticket = Access.SetTicketPrinted(ticket);
                            if (ticket.BlankId == spot.Blank.Id)
                            {
                                spot.Status = ItemStatus.Printed;
                            }
                            else
                            {
                                //t.Status = ItemStatus.Printed;
                            }
                        }
                    }
                    
                    progress = (int) (progress + percent);
                    printedCount++;
                }
            };
            _worker.RunWorkerCompleted += delegate
            {
                _printProgress.Close();
                _worker = null;
            };
            _worker.RunWorkerAsync();
            _printProgress.ShowDialog();

            StatusText =
                OperationResult =
                    String.Format("Печать завершена. Напечатано {0} из {1} билетов.", printedCount, ticketsCount);
           
            BaseData.ClearCache();
        }

        private void CancelProcess(object sender, EventArgs e)
        {
            _worker.CancelAsync();
        }

        private MemoryStream GetTicketImage(int orderId, int ticketId, int formId)
        {
            MemoryStream ms = new MemoryStream(Access.GetTicketPrinted(orderId, ticketId, formId));
            return ms;
        }

        private DelegateCommand _closeWizard;
        /// <summary>
        /// Closes wizard
        /// </summary>
        public ICommand CloseWizard
        {
            get
            {
                if (_closeWizard == null)
                {
                    _closeWizard = new DelegateCommand(OnCloseWizard);
                }
                return _closeWizard;
            }
        }

        /// <summary>
        /// Closes wizard
        /// </summary>
        private void OnCloseWizard()
        {
            if (_wisardStep == WisardStep.Locked)
            {
                _worker = null;
                _wisardStep = WisardStep.None;
                _wizardWindow.Close();
                ReleaseSeats(SelectedSpots.ToArray());
                OnSpotsChanged();
                StatusText = String.Empty;
            }

            if (_wisardStep == WisardStep.OrderCreated)
            {
                _worker = null;
                _wisardStep = WisardStep.None;
                _wizardWindow.Close();
                OnSpotsChanged();
                StatusText = String.Empty;
            }

            if (_wisardStep == WisardStep.OrderPaid)
            {
                _worker = null;
                _wisardStep = WisardStep.None;
                _wizardWindow.Close();
                OnSpotsChanged();
                StatusText = String.Empty;
            }

            if (_wisardStep == WisardStep.Printed)
            {
                _worker = null;
                _wisardStep = WisardStep.None;
                _wizardWindow.Close();
                OnSpotsChanged();
                StatusText = String.Empty;
            }

            _wizardWindow = null;
        }

        private DelegateCommand _printReservedTickets;

        /// <summary>
        /// Handles OrderWindow PrintReservedTickets button event
        /// </summary>
        public ICommand PrintReservedTickets
        {
            get
            {
                if (_printReservedTickets == null)
                {
                    _printReservedTickets = new DelegateCommand(OnPrintReservedTickets, CanPrintReservedTickets);
                }
                return _printReservedTickets;
            }
        }

        /// <summary>
        /// Handles OrderWindow PrintTickets button event
        /// </summary>
        private void OnPrintReservedTickets()
        {
            foreach (Ticket ticket in Tickets)
            {
                if (ticket.BlankId > 0)
                {
                    ConfirmationBox cb = new ConfirmationBox
                    {
                        Message = String.Format(
                            "Вы действительно желаете перепечатать билет № {0}, {1}, {2}, {3} ряд, {4} место?\nБланк № {5} будет аннулирован.",
                            ticket.Id, ticket.SideName, ticket.SectorName, ticket.RowNum, ticket.SeatNum, ticket.BlankId)
                    };
                    bool? result = cb.ShowDialog();
                    if (result ?? false)
                    {
                        Blank blank = new Blank(){Id = ticket.BlankId};
                        Access.Annul(blank);
                        ticket.BlankId = 0;
                    }
                }
            }

            int validTickets = Tickets.Where(x => x.BlankId == 0).Count(x => x.Status == ItemStatus.Sold);
            Blank[] blanks = Access.GetBlanks(0, validTickets, ItemStatus.OnSale);

            if (blanks != null)
            {
                if (blanks.Length != validTickets)
                {
                    BlankStatus = "Количество бланков недостаточно для печати заказа";
                    return;
                }
            }
            else
            {
                BlankStatus = "У вас отсутствуют бланки";
                return;
            }
            List<Spot> spots = new List<Spot>();
            IEnumerable<Ticket> tickets = Tickets.Where(x => x.BlankId == 0 && x.Status == ItemStatus.Sold);
            int counter = 0;
            foreach (Ticket ticket in tickets)
            {
                if (blanks.Length > counter)
                {
                    Spot spot = new Spot
                    {
                        Id = ticket.Id,
                        Status = ticket.Status,
                        Blank = blanks[counter]
                    };
                    ticket.BlankId = spot.Blank.Id;
                    spots.Add(spot);
                    counter++;
                }
            }

            ProcessPrint(spots);
            ReloadOrder();
        }

        private bool CanPrintReservedTickets()
        {
            bool result = false;
            //if (PrinterFound)
            //{
            //    if (Tickets.Count > 0 | Customer.Order.Seats.Count>0)
            //    {
            //        if (Customer.Order.Status == ItemStatus.Sold)
            //        {
            //            result = true;
            //        }
            //    }
            //}

            if (Customer.Order!=null)
            {
                result = Customer.Order.Status == ItemStatus.Sold;
            }
            return result;
        }
        
        private DelegateCommand _printReservedTicket;

        /// <summary>
        /// Handles OrderWindow PrintReservedTicket button event
        /// </summary>
        public ICommand PrintReservedTicket
        {
            get
            {
                if (_printReservedTicket == null)
                {
                    _printReservedTicket = new DelegateCommand(OnPrintReservedTicket, CanPrintReservedTicket);
                }
                return _printReservedTicket;
            }
        }

        /// <summary>
        /// Handles OrderWindow PrintReservedTicket button event
        /// </summary>
        private void OnPrintReservedTicket()
        {
            List<Spot> spots = new List<Spot>();

            if (SelectedTicket.BlankId > 0)
            {
                ConfirmationBox cb = new ConfirmationBox
                {
                    Message = String.Format(
                        "Вы действительно желаете перепечатать билет № {0}, {1}, {2}, {3} ряд, {4} место?\nБланк № {5} будет аннулирован.",
                        SelectedTicket.Id, SelectedTicket.SideName, SelectedTicket.SectorName, SelectedTicket.RowNum,
                        SelectedTicket.SeatNum, SelectedTicket.BlankId)
                };
                bool? result = cb.ShowDialog();
                if (result ?? false)
                {
                    Blank blank = new Blank(){Id = SelectedTicket.BlankId};
                    Access.Annul(blank);
                    SelectedTicket.BlankId = 0;
                }
            }

            if (SelectedTicket.Status == ItemStatus.Sold&&SelectedTicket.BlankId==0)
            {
                Blank[] blanks = Access.GetBlanks(0, 1, ItemStatus.OnSale);
                Spot spot = new Spot
                {
                    Id = SelectedTicket.Id,
                    Status = SelectedTicket.Status,
                    Blank = blanks[0]
                };
                SelectedTicket.BlankId = spot.Blank.Id;
                spots.Add(spot);
                ProcessPrint(spots);
            }

            ReloadOrder();
        }

        private bool CanPrintReservedTicket()
        {
            bool result = false;
            Ticket t = SelectedTicket;
            if (t != null)
            {
                result = SelectedTicket.Status == ItemStatus.Sold;
            }
            return result;
        }
    }
}