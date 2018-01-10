using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using TicketShop.Core;
using TicketShop.Shell.Models;
using TicketShop.Shell.Views;
using Point = TicketShop.Core.Point;

namespace TicketShop.Shell.ViewModels
{
    public partial class MainViewModel
    {
        public void UpdatePrintText(int percentage, string description)
        {
            _printProgress.Message = string.Format("Выполнено: {0}%", percentage);
            _printProgress.Description = description;
        }

        public void UpdateProgressText(string description)
        {
            _progressDialog.Message = description;
        }

        public void LoadData()
        {
            BaseData.ClearCache();

            _progressDialog = new ProgressDialog
            {
                Header = "Загрузка данных",
                Message = "Подготовка данных..."
            };
            _worker = new BackgroundWorker();
            // Do not remove (engaged below)
            _updateProgressDelegate = UpdateProgressText;
            _worker.DoWork += delegate
            {
                HandleProviders();
            };

            _worker.RunWorkerCompleted += delegate
            {
                _progressDialog.Close();
                _worker = null;
            };

            _worker.RunWorkerAsync();
            _progressDialog.ShowDialog();
        }

        private void OnProviderChanged()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                ShowActions.Clear();
            });
            Dispatcher pdDispatcher = _progressDialog.Dispatcher;
            if (_worker != null)
            {
                pdDispatcher.BeginInvoke(_updateProgressDelegate, "Загрузка списка программ...");
                HandleActions();
            }
            else
            {
                _progressDialog = new ProgressDialog
                {
                    Header = "Загрузка данных",
                    Message = "Загрузка списка программ..."
                };
                _worker = new BackgroundWorker();
                _worker.DoWork += delegate
                {
                    HandleActions();
                    
                };

                _worker.RunWorkerCompleted += delegate
                {
                    _progressDialog.Close();
                    _worker = null;
                };
                _worker.RunWorkerAsync();
                _progressDialog.ShowDialog();
            }
        }

        private void OnShowActionChanged()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                EventDates.Clear();
            });
            Dispatcher pdDispatcher = _progressDialog.Dispatcher;
            if (_worker != null)
            {
                pdDispatcher.BeginInvoke(_updateProgressDelegate, "Загрузка списка выступлений...");
                HandleEvents();
                
            }
            else
            {
                _progressDialog = new ProgressDialog
                {
                    Header = "Загрузка данных",
                    Message = "Загрузка списка выступлений..."
                };
                _worker = new BackgroundWorker();
                _worker.DoWork += delegate
                {
                    HandleEvents();
                    
                };
                _worker.RunWorkerCompleted += delegate
                {
                    _progressDialog.Close();
                    _worker = null;
                };
                _worker.RunWorkerAsync();
                _progressDialog.ShowDialog();
            }
        }

        public void OnEventDateChanged()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Spots.Clear();
                SelectedSpots.Clear();
                HallSeats.Clear();
            });

            Dispatcher pdDispatcher = _progressDialog.Dispatcher;
            if (_worker != null)
            {
                pdDispatcher.BeginInvoke(_updateProgressDelegate, "Загрузка схемы зала...");
                HandleMap();
                pdDispatcher.BeginInvoke(_updateProgressDelegate, "Загрузка мест...");
                HandleSpots();
                pdDispatcher.BeginInvoke(_updateProgressDelegate, "Загрузка статистики...");
                HandleHallSeats();
            }
            else
            {
                _progressDialog = new ProgressDialog
                {
                    Header = "Загрузка мест",
                    Message = "Загрузка мест..."
                };
                _worker = new BackgroundWorker();
                _worker.DoWork += delegate
                {
                    pdDispatcher.BeginInvoke(_updateProgressDelegate, "Загрузка схемы зала...");
                    HandleMap();
                    pdDispatcher.BeginInvoke(_updateProgressDelegate, "Загрузка мест...");
                    HandleSpots();
                    pdDispatcher.BeginInvoke(_updateProgressDelegate, "Загрузка статистики...");
                    HandleHallSeats();
                };
                _worker.RunWorkerCompleted += delegate
                {
                    _progressDialog.Close();
                    _worker = null;
                };
                _worker.RunWorkerAsync();
                _progressDialog.ShowDialog();
            }
        }

        public void OnSpotsChanged()
        {
            UpdateIconVisibility = Visibility.Visible;
            Application.Current.Dispatcher.Invoke((Action) delegate
            {
                SelectedSpots.Clear();
                HallSeats.Clear();
            });

            //Dispatcher pdDispatcher = _progressDialog.Dispatcher;

            //_progressDialog = new ProgressDialog
            //{
            //    Header = "Загрузка мест",
            //    Message = "Загрузка мест..."
            //};
            _worker = new BackgroundWorker();
            _worker.DoWork += delegate
            {
                
                //StatusText = "Обновление мест...";
                //pdDispatcher.BeginInvoke(_updateProgressDelegate, "Обновление мест...");
                RefreshSpots();
                //StatusText = "Обновление статистики...";
                //pdDispatcher.BeginInvoke(_updateProgressDelegate, "Обновление статистики...");
                HandleHallSeats();
                
            };
            _worker.RunWorkerCompleted += delegate
            {
                //_progressDialog.Close();
                _worker = null;
                UpdateIconVisibility = Visibility.Hidden;
            };
            _worker.RunWorkerAsync();
            //_progressDialog.ShowDialog();
        }

        private void HandleProviders()
        {
            Provider[] providers = Data.Access.GetProviders();
            if (providers != null)
            {
                Application.Current.Dispatcher.Invoke((Action) delegate
                {
                    foreach (Provider p in providers)
                    {
                        var provider = p;
                        _providers.Add(provider);
                    }
                });
                SelectedProvider = providers.FirstOrDefault();
            }
        }

        private void HandleActions()
        {
            if (SelectedProvider != null)
            {
                ShowAction[] actions = Data.Access.GetActions(SelectedProvider.Id);
                if (actions != null)
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        foreach (ShowAction sa in actions)
                        {
                            var action = sa;
                            _showActions.Add(action);
                        }
                    });
                    SelectedAction = actions.FirstOrDefault();
                }
            }
        }

        private void HandleMap()
        {
            if (SelectedProvider != null)
            {
                System.Drawing.Image image = Data.Access.GetSeatingMap(SelectedEvent.StageId);

                BitmapImage bi = new BitmapImage();
                bi.BeginInit();
                if (image != null)
                {
                    MemoryStream ms = new MemoryStream();
                    image.Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    bi.StreamSource = ms;
                }
                else
                {
                    bi.UriSource = new Uri("pack://application:,,,/Resources/NoSchema.png");
                }

                bi.EndInit();
                bi.Freeze();
                StageImage = bi;
            }
        }

        private void HandleEvents()
        {
            if (SelectedAction != null)
            {
                EventDate[] eventDates = Data.Access.GetEvents(SelectedAction.Id);
                if (eventDates != null)
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        foreach (EventDate ed in eventDates)
                        {
                            EventDate eventDate = ed;
                            _eventDates.Add(eventDate);
                        }

                    });
                    SelectedEvent = eventDates.FirstOrDefault();
                }
            }
        }

        /// <summary>
        /// Completely re-load spots collection
        /// </summary>
        private void HandleSpots()
        {
            if (SelectedEvent != null)
            {
                Point[] points = Data.Access.GetMapSeatPoints(SelectedEvent.Id);
                if (points != null)
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        foreach (Point point in points)
                        {
                            Point seat = point;

                            Spot spot = new Spot
                            {
                                X = seat.XPos,
                                Y = seat.YPos,
                                Price = seat.Price,
                                Color = seat.Color,
                                Id = seat.SeatId,
                                SideName = seat.SideName,
                                SectorName = seat.SectorName,
                                RowNum = seat.RowNum,
                                SeatNum = seat.SeatNum
                            };
                            _spots.Add(spot);
                        }
                    });
                    OnPropertyChanged("Spots");
                }
            }
        }

        private void HandleHallSeats()
        {
            if (SelectedEvent != null)
            {
                Hall[] seats = Data.Access.GetHallSeats(SelectedEvent.Id, ItemStatus.OnSale);
                if (seats != null)
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        foreach (Hall seat in seats)
                        {
                            HallSeats.Add(seat);
                        }
                    });
                }
            }
        }

        private void HandleOrders()
        {
            if (SelectedProvider != null)
            {
                Order[] orders = Data.Access.GetOrders(0, RowCount, SelectedProvider.Id, ItemStatus.None, Query);
                if (orders != null)
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        foreach (Order order in orders)
                        {
                            Orders.Add(order);
                        }
                    });
                }
            }
        }

        public void ShowBlanks()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Blanks.Clear();
            });

            _progressDialog = new ProgressDialog
            {
                Header = "Загрузка списка бланков",
                Message = String.Format("Загрузка списка бланков {0}", Query)
            };
            _worker = new BackgroundWorker();
            _worker.DoWork += delegate
            {
                HandleBlanks();
            };
            _worker.RunWorkerCompleted += delegate
            {
                _progressDialog.Close();
                _worker = null;
            };
            _worker.RunWorkerAsync();
            _progressDialog.ShowDialog();
        }
        
        private void HandleBlanks()
        {
            ItemStatus status;
            if (Enum.TryParse(SelectedBlankStatus.ToString(), true, out status))
            {
                Blank[] blanks = Data.Access.GetBlanks(0, 100, status);
                if (blanks != null)
                {
                    Application.Current.Dispatcher.Invoke((Action)delegate
                    {
                        foreach (Blank b in blanks)
                        {
                            Blanks.Add(b);
                        }
                    });
                }
            }
        }

        public byte[] DownloadReport()
        {
            byte[] byteArray = null;
            _progressDialog = new ProgressDialog
            {
                Header = "Пожалуйста, подождите",
                Message = String.Format("Загрузка отчета кассира за {0}", SelectedDate.ToString("d"))
            };
            _worker = new BackgroundWorker();
            _worker.DoWork += delegate
            {
                byteArray = Data.Access.GetReport(Member.Id, SelectedDate);
            };
            _worker.RunWorkerCompleted += delegate
            {
                _progressDialog.Close();
                _worker = null;
                
            };
            _worker.RunWorkerAsync();
            _progressDialog.ShowDialog();
            return byteArray;
        }

        public void ShowReport()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Seats.Clear();
            });

            _progressDialog = new ProgressDialog
            {
                Header = "Пожалуйста, подождите",
                Message = String.Format("Загрузка отчета кассира за {0}", SelectedDate.ToString("d"))
            };
            _worker = new BackgroundWorker();
            _worker.DoWork += delegate
            {
                HandleReport();
            };
            _worker.RunWorkerCompleted += delegate
            {
                _progressDialog.Close();
                _worker = null;
            };
            _worker.RunWorkerAsync();
            _progressDialog.ShowDialog();
        }

        private void HandleReport()
        {
            Ticket[] blanks = Data.Access.GetReports(SelectedDate);
            if (blanks != null)
            {
                Application.Current.Dispatcher.Invoke((Action) delegate
                {
                    foreach (Ticket b in blanks)
                    {
                        Seats.Add(b);
                    }
                });
            }

            int oc = Seats.Where(y => y.Status==ItemStatus.Sold).GroupBy(x => x.OrderId).Count();
            OrderCountText = String.Format("{0} заказов", oc);
            TicketCountText = String.Format("{0} билетов", Seats.Count(y => y.Status == ItemStatus.Sold).ToString("## ### ###"));
            decimal ast = Seats.Where(y => y.Status==ItemStatus.Sold).Sum(x => x.Price);
            AmountSumText = String.Format("Сумма: {0}", ast.ToString("C"));
        }

        public void ShowOrders()
        {
            Application.Current.Dispatcher.Invoke((Action)delegate
            {
                Orders.Clear();
            });

            _progressDialog = new ProgressDialog
            {
                Header = "Загрузка списка заказов",
                Message = String.Format("Загрузка списка заказов {0}", Query)
            };
            _worker = new BackgroundWorker();
            _worker.DoWork += delegate
            {
                HandleOrders();
            };
            _worker.RunWorkerCompleted += delegate
            {
                _progressDialog.Close();
                _worker = null;
            };
            _worker.RunWorkerAsync();
            _progressDialog.ShowDialog();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (_progressDialog != null)
            {
                if (SelectedSpots.Count == 0)
                {
                    BaseData.RemoveFromCache("MapSeatPoints");
                    OnSpotsChanged();
                    StatusText = String.Format("Места обновлены {0}", DateTime.Now.ToString("t"));
                }
            }
        }

        /// <summary>
        /// Partially re-load spots collection
        /// </summary>
        private void RefreshSpots()
        {
            if (SelectedEvent != null)
            {
                Point[] points = Data.Access.GetMapSeatPoints(SelectedEvent.Id);
                
                if (points != null)
                {
                    Application.Current.Dispatcher.Invoke((Action) delegate
                    {
                        foreach (Point point in points)
                        {
                            Point seat = point;
                            Spot spot = new Spot
                            {
                                Id = seat.SeatId,
                                SideName = seat.SideName,
                                SectorName = seat.SectorName,
                                RowNum = seat.RowNum,
                                SeatNum = seat.SeatNum,
                                X = seat.XPos,
                                Y = seat.YPos,
                                Price = seat.Price,
                                Color = seat.Color
                            };

                            // Firstly we add new spots
                            if (Spots.Contains(spot, new SpotIdComparer()))
                            {
                            }
                            else
                            {
                                Spots.Add(spot);
                            }
                        }
                        
                        List<Spot> spotsToRemove = new List<Spot>();  
                      // Then we remove spots that not exists in points
                        foreach (Spot spot in Spots)
                        {
                            Point fod = points.FirstOrDefault(x => x.SeatId == spot.Id);
                            if (fod == null)
                            {
                                Spot soldSpot = Spots.FirstOrDefault(x => x.Id == spot.Id);
                                //Spots.Remove(soldSpot);
                                spotsToRemove.Add(soldSpot);
                            }
                        }

                        foreach (Spot spot in spotsToRemove)
                        {
                            Spots.Remove(spot);
                        }
                    });
                }
            }
        }
        
        //private void OnRefreshSpots()
        //{
        //    Application.Current.Dispatcher.Invoke((Action)delegate
        //    {
        //        Spots.Clear();
        //        Orders.Clear();
        //    });

        //    _progressDialog = new ProgressDialog
        //    {
        //        Header = "Загрузка данных",
        //        Message = "Обновление списка мест..."
        //    };
        //    _worker = new BackgroundWorker();
        //    _worker.DoWork += delegate
        //    {
        //        HandleSpots();
        //    };
        //    _worker.RunWorkerCompleted += delegate
        //    {
        //        _progressDialog.Close();
        //        _worker = null;
        //    };
        //    _worker.RunWorkerAsync();
        //    _progressDialog.ShowDialog();
        //}
    }
}