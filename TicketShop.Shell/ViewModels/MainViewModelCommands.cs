using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using TicketShop.Core;
using TicketShop.Data;
using TicketShop.Shell.Commands;
using TicketShop.Shell.Models;
using TicketShop.Shell.Views;
using Application = System.Windows.Application;
using Settings = TicketShop.Shell.Properties.Settings;

namespace TicketShop.Shell.ViewModels
{
    public partial class MainViewModel
    {
        private DelegateCommand _toggleFullScreen;

        public ICommand ToggleFullScreen
        {
            get
            {
                if (_toggleFullScreen == null)
                {
                    _toggleFullScreen = new DelegateCommand(OnToggleFullScreen);
                }
                return _toggleFullScreen;
            }
        }

        private void OnToggleFullScreen()
        {
            MetroWindow.ToggleFullScreen = !MetroWindow.ToggleFullScreen;
            FullScreen = MetroWindow.ToggleFullScreen;
        }

        private DelegateCommand clearSelection;

        public ICommand ClearSelection
        {
            get
            {
                if (clearSelection == null)
                {
                    clearSelection = new DelegateCommand(OnClearSelection, CanClearSelection);
                }
                return clearSelection;
            }
        }

        private void OnClearSelection()
        {
            SelectedSpots.Clear();
            foreach (Spot spot in Spots)
            {
                spot.IsSelected = false;
                spot.Height = 20;
                spot.Width = 20;

                //if (spot.IsSelected)
                //{
                //    spot.IsSelected = false;
                //    spot.Height = 20;
                //    spot.Width = 20;
                //}
            }
        }

        private bool CanClearSelection()
        {
            return SelectedSpots.Count > 0;
        }

        private DelegateCommand _findOrders;

        public ICommand FindOrders
        {
            get
            {
                if (_findOrders == null)
                {
                    _findOrders = new DelegateCommand(OnFindOrders, CanFindOrders);
                }
                return _findOrders;
            }
        }
        private void OnFindOrders()
        {
            ShowOrders();
        }

        private bool CanFindOrders()
        {
            return Query.Length > 0;
        }

        private DelegateCommand _refreshOrders;

        public ICommand RefreshOrders
        {
            get
            {
                if (_refreshOrders == null)
                {
                    _refreshOrders = new DelegateCommand(OnRefreshOrders);
                }
                return _refreshOrders;
            }
        }
        private void OnRefreshOrders()
        {
            ShowOrders();
        }

        private DelegateCommand _refreshBlanks;

        public ICommand RefreshBlanks
        {
            get
            {
                if (_refreshBlanks == null)
                {
                    _refreshBlanks = new DelegateCommand(OnRefreshBlanks);
                }
                return _refreshBlanks;
            }
        }
        private void OnRefreshBlanks()
        {
            ShowBlanks();
        }

        private DelegateCommand _refreshReport;

        public ICommand RefreshReport
        {
            get
            {
                if (_refreshReport == null)
                {
                    _refreshReport = new DelegateCommand(OnRefreshReport, CanRefreshReport);
                }
                return _refreshReport;
            }
        }
        private void OnRefreshReport()
        {
            ShowReport();
        }

        private bool CanRefreshReport()
        {
            return SelectedDate < DateTime.Now;
        }

        private DelegateCommand _loadReport;

        public ICommand LoadReport
        {
            get
            {
                if (_loadReport == null)
                {
                    _loadReport = new DelegateCommand(OnLoadReport, CanLoadReport);
                }
                return _loadReport;
            }
        }

        private void OnLoadReport()
        {
            byte[] byteArray = DownloadReport();
            if (byteArray != null)
            {
                using (SaveFileDialog dlg = new SaveFileDialog())
                {
                    dlg.FileName = String.Format("Отчет {0} за {1}.xls", Member.FullName, SelectedDate.ToString("d"));
                    dlg.DefaultExt = ".xls";
                    dlg.Filter = "Excel document (.xls)|*.xls";
                    DialogResult result = dlg.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        string filename = dlg.FileName;
                        File.WriteAllBytes(filename, byteArray);
                    }
                }
            }
        }

        private bool CanLoadReport()
        {
            return SelectedDate < DateTime.Now;
        }

        private DelegateCommand _removeSpot;

        /// <summary>
        /// Removes a spot from SelectionView
        /// </summary>
        public ICommand RemoveSpot
        {
            get
            {
                if (_removeSpot == null)
                {
                    _removeSpot = new DelegateCommand(OnRemoveSpot, CanRemoveSpot);
                }
                return _removeSpot;
            }
        }

        private void OnRemoveSpot()
        {
            if (SelectedSpot != null)
            {
                Spot spot = SelectedSpot;
                SelectedSpots.Remove(spot);
                foreach (Spot s in Spots)
                {
                    if (s.Id == spot.Id)
                    {
                        s.IsSelected = false;
                        s.Width = 20;
                        s.Height = 20;
                    }
                }
            }
        }

        private bool CanRemoveSpot()
        {
            return SelectedSpot != null;
        }

        private DelegateCommand _removeSeat;
        /// <summary>
        /// Removes a spot from SelectionView
        /// </summary>
        public ICommand RemoveSeat
        {
            get
            {
                if (_removeSeat == null)
                {
                    _removeSeat = new DelegateCommand(OnRemoveSeat, CanRemoveSeat);
                }
                return _removeSeat;
            }
        }

        private void OnRemoveSeat()
        {
            if (SelectedSpot != null)
            {
                Spot spot = SelectedSpot;
                ConfirmationBox cb = new ConfirmationBox
                {
                    Message = String.Format(
                        "Вы действительно желаете удалить место № {0}, {1}, {2}, {3} ряд, {4} место из списка выбранных мест?",
                        spot.Id, spot.SideName, spot.SectorName, spot.RowNum, spot.SeatNum)
                };
                bool? result = cb.ShowDialog();
                if (result ?? false)
                {
                    // First we release all the selected seats
                    List<Seat> seats = ReleaseSeats(SelectedSpots.ToArray());
                    if (seats != null)
                    {
                        // Then we remove a seat from selected seats collection
                        SelectedSpots.Remove(spot);
                        // and mark spot as unselected on the map
                        foreach (Spot s in Spots)
                        {
                            if (s.Id == spot.Id)
                            {
                                s.IsSelected = false;
                                s.Width = 20;
                                s.Height = 20;
                            }
                        }

                        // At last we again lock seats
                        if (SelectedSpots.Count > 0)
                        {
                            LockSeats(SelectedSpots.ToArray());
                        }
                    }
                }
            }
        }

        private bool CanRemoveSeat()
        {
            return SelectedSpot != null;
        }

        private DelegateCommand _removeTicket;
        /// <summary>
        /// Removes a ticket from reserved order
        /// </summary>
        public ICommand RemoveTicket
        {
            get
            {
                if (_removeTicket == null)
                {
                    _removeTicket = new DelegateCommand(OnRemoveTicket);
                }
                return _removeTicket;
            }
        }

        private void OnRemoveTicket()
        {
            Spot spot = SelectedSpot;
            ConfirmationBox cb = new ConfirmationBox
            {
                Message =
                    String.Format(
                        "Вы действительно желаете удалить билет № {0}, {1}, {2}, {3} ряд, {4} место из списка билетов заказа?",
                        spot.Id, spot.SideName, spot.SectorName, spot.RowNum, spot.SeatNum)
            };


            bool? result = cb.ShowDialog();
            if (result ?? false)
            {
                //First we cancel ticket in the order
                Customer.Order.Seats = new List<Seat> {new Seat() {Id = spot.Id}};
                Customer.Order = Data.Access.Cancel(Customer.Order);

                // Then we remove a seat from selected seats collection
                SelectedSpots.Remove(spot);
                // and mark spot as unselected on the map
                foreach (Spot s in Spots)
                {
                    if (s.Id == spot.Id)
                    {
                        s.IsSelected = false;
                        s.Width = 20;
                        s.Height = 20;
                    }
                }

                // At last we again lock seats
                if (SelectedSpots.Count > 0)
                {
                    LockSeats(SelectedSpots.ToArray());
                }
            }
        }

        /// <summary>
        /// Locks seats 
        /// </summary>
        private void LockSeats(Spot[] spots)
        {
            List<Seat> seats = new List<Seat>();
            foreach (Spot spot in spots)
            {
                Seat seat = new Seat();
                seat.Id = spot.Id;
                seat.ReserveDate = ReserveDate;
                seats.Add(seat);
            }

            List<Seat> lockedSeats = Data.Access.LockSeats(seats);
            if (lockedSeats != null)
            {
                HashSet<int> writerIds = new HashSet<int>(lockedSeats.Select(x => x.Id));
                seats.RemoveAll(x => writerIds.Contains(x.Id));

                foreach (Seat s in seats)
                {
                    SelectedSpots.Remove(SelectedSpots.Single(i => i.Id == s.Id));
                    Spots.Remove(Spots.Single(i => i.Id == s.Id));
                }

                int counter = 0;
                decimal price = 0;
                foreach (Spot spot in SelectedSpots)
                {
                    counter++;
                    price += spot.Price;
                    spot.ItemId = counter;
                }
                StatusText = String.Format("Удерживается {0} мест на сумму {1}", SeatsCount.ToString("### ##0"), SeatsAmount.ToString("C0"));
                OperationResult = String.Format("Выбрано мест {0} на сумму {1}", counter, price.ToString("C"));
            }
        }

        /// <summary>
        /// Locks seats 
        /// </summary>
        private List<Seat> ReleaseSeats(Spot[] spots)
        {
            List<Seat> seats = new List<Seat>();
            foreach (Spot spot in spots)
            {
                Seat seat = new Seat { Id = spot.Id };
                seats.Add(seat);
            }
            StatusText = "Освобождаю удерживаемые места...";
            seats = Data.Access.ReleaseSeats(seats);
            return seats;
        }
        
        private void InitCustomer()
        {
            Customer = new Customer
            {
                FirstName = FirstName,
                MiddleName = MiddleName,
                LastName = LastName,
                Email = CustomerEmail,
                Phone = CustomerPhone,
                SendSms = SendSms,
                Order = new Order { Seats = new List<Seat>() }
            };

            foreach (Spot selectedSpot in SelectedSpots)
            {
                Seat seat = new Seat
                {
                    Id = selectedSpot.Id,
                    ReserveDate = ReserveDate
                };
                Customer.Order.Seats.Add(seat);
            }

            FirstName = String.Empty;
            MiddleName = String.Empty;
            LastName = String.Empty;
            CustomerEmail = String.Empty;
            CustomerPhone = String.Empty;
            SendSms = false;
        }
        
        private DelegateCommand _refresh;

        public ICommand Refresh
        {
            get
            {
                if (_refresh == null)
                {
                    _refresh = new DelegateCommand(OnRefresh, CanRefresh);
                }
                return _refresh;
            }
        }

        private void OnRefresh()
        {
            LoadData();
        }

        private bool CanRefresh()
        {
            return SelectedSpots.Count == 0;
        }

        private DelegateCommand _showOrder;

        public ICommand ShowOrder
        {
            get
            {
                if (_showOrder == null)
                {
                    _showOrder = new DelegateCommand(OnShowOrder);
                }
                return _showOrder;
            }
        }

        private void OnShowOrder()
        {
            LoadCustomerAndOrderTickets(Customer.Order);
            OrderDialogTitle = String.Format("Заказ № {0} - {1}", Customer.Order.Id,
                BaseData.GetStatus(Customer.Order.Status));
            OrderStatus = Customer.Order.Status;
            _orderWindow = new OrderWindow {Owner = MetroWindow};
            _orderWindow.ShowDialog();
        }

        /// <summary>
        /// Loads Order Tickets
        /// </summary>
        /// <param name="order">Order</param>
        private void LoadCustomerAndOrderTickets(Order order)
        {
            if (order.CustomerId > 0)
            {
                Customer = Access.GetCustomer(order.CustomerId);
                CustomerEmail = Customer.Email;
                CustomerName = Customer.Name;
                LastName = Customer.LastName;
                FirstName = Customer.FirstName;
                MiddleName = Customer.MiddleName;
                CustomerPhone = Customer.Phone;
                Customer.Order = order;
            }

            Ticket[] tickets = Access.GetTickets(order.Id);
            if (tickets != null)
            {
                OrderPrice =
                    tickets.Where(x => x.Status == ItemStatus.Reserved || x.Status == ItemStatus.Sold).Sum(x => x.Price);
                Ticket firstTicket = tickets.FirstOrDefault();
                if (firstTicket != null) ReserveDate = firstTicket.ReserveDate;
                if (tickets.Count() < 10)
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        foreach (Ticket ticket in tickets)
                        {
                            MemoryStream ms = GetTicketImage(order.Id, ticket.Id, 1);
                            if (ms != null)
                            {
                                Ticket t = ticket;
                                BitmapImage bi = new BitmapImage();
                                bi.BeginInit();
                                bi.StreamSource = ms;
                                bi.EndInit();
                                t.TicketImage = bi;
                            }
                        }
                    });
                }

                Application.Current.Dispatcher.Invoke((Action) delegate
                {
                    Tickets.Clear();
                    foreach (Ticket t in tickets)
                    {
                        Ticket ticket = t;
                        Tickets.Add(ticket);
                    }
                });
            }
        }

        private DelegateCommand _findPrinter;
        public ICommand FindPrinter
        {
            get
            {
                if (_findPrinter == null)
                {
                    _findPrinter = new DelegateCommand(OnFindPrinter, CanFindPrinter);
                }
                return _findPrinter;
            }
        }

        private void OnFindPrinter()
        {
            PrinterInfo = String.Empty;
            StartPrintService();
        }

        private bool CanFindPrinter()
        {
            bool result = false;
            
            IPAddress ipAddress;
            if (IPAddress.TryParse(PrinterIp, out ipAddress))
            {
                if (IPAddress.TryParse(NetworkMask, out ipAddress))
                {
                    result = true;
                }
            }
            return result;
        }
        
        private DelegateCommand _setupPrinter;
        public ICommand SetupPrinter
        {
            get
            {
                if (_setupPrinter == null)
                {
                    _setupPrinter = new DelegateCommand(OnSetupPrinter, CanSetupPrinter);
                }
                return _setupPrinter;
            }
        }

        private void OnSetupPrinter()
        {
            PrinterInfo=String.Empty;
            _printRequest.Setup();
            StartPrintService();
        }

        private bool CanSetupPrinter()
        {
            return PrinterFound;
        }

        private DelegateCommand _saveSettings;

        public ICommand SaveSettings
        {
            get
            {
                if (_saveSettings == null)
                {
                    _saveSettings = new DelegateCommand(OnSaveSettings, CanSaveSettings);
                }
                return _saveSettings;
            }
        }

        private void OnSaveSettings()
        {
            Settings.Default.TimerInterval = TimerInterval;
            Settings.Default.RowCount = RowCount;
            Settings.Default.PrinterIp = PrinterIp;
            Settings.Default.ShowTickets = ShowTickets;
            Settings.Default.AutoRefresh = AutoRefresh;
            Settings.Default.Save();
            ToggleAutoRefresh();
        }

        private bool CanSaveSettings()
        {
            bool result = false;
            if (TimerInterval > 0 && TimerInterval < 60)
            {
                if (RowCount > 0 && RowCount <= 100)
                {
                    IPAddress ipAddress;
                    if (IPAddress.TryParse(PrinterIp, out ipAddress))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        private DelegateCommand _toggleMagnifier;

        public ICommand ToggleMagnifier
        {
            get
            {
                if (_toggleMagnifier == null)
                {
                    _toggleMagnifier = new DelegateCommand(OnToggleMagnifier);
                }
                return _toggleMagnifier;
            }
        }

        private void OnToggleMagnifier()
        {
            if (MagnifierVisibility == Visibility.Visible)
            {
                MagnifierVisibility = Visibility.Collapsed;
            }
            else
            {
                MagnifierVisibility = Visibility.Visible;
            }
        }

        private DelegateCommand _cancelBlank;

        public ICommand CancelBlank
        {
            get
            {
                if (_cancelBlank == null)
                {
                    _cancelBlank = new DelegateCommand(OnCancelBlank, CanCancelBlank);
                }
                return _cancelBlank;
            }
        }

        private void OnCancelBlank()
        {
            ConfirmationBox cb = new ConfirmationBox
            {
                Message = String.Format("Функция отключена до выяснения подробностей аннулирования свободного бланка. Выбран бланк № {0}", SelectedBlank.Key)
            };
            bool? result = cb.ShowDialog();
            if (result ?? false)
            {
                SelectedBlank = Access.Annul(SelectedBlank);
            }
        }

        private bool CanCancelBlank()
        {
            return SelectedBlank.Status == ItemStatus.OnSale;
        }

        private DelegateCommand _testCommand;

        public ICommand TestCommand
        {
            get
            {
                if (_testCommand == null)
                {
                    _testCommand = new DelegateCommand(OnTestCommand);
                }
                return _testCommand;
            }
        }

        private void OnTestCommand()
        {
            var df = Spots;
            Spot spot = new Spot()
            {
                Color = Colors.Red,
                Height = 26.0,
                Id = 9088127,
                IsSelected = true,
                Price = 1000,
                RowNum = "1",
                SeatNum = "28",
                SectorName = "4 сектор",
                SideName = "Левая сторона",
                Width = 26.0,
                X = 851.0,
                Y = 445.0
            };
            if (Spots.Contains(spot, new SpotIdComparer()))
            {
                var dfd = Spots.FirstOrDefault(x => x.Id == spot.Id);
                Spots.Remove(dfd);
            }
            else
            {
                Spots.Add(spot);
            }
        }
    }
}