using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using MahApps.Metro;
using TicketShop.Core;
using TicketShop.Print;
using TicketShop.Print.Base;
using TicketShop.Shell.Models;
using TicketShop.Shell.Properties;

namespace TicketShop.Shell.ViewModels
{
    public delegate void UpdateProgressDelegate(string description);
    public delegate void UpdatePrintDelegate(int percentage, string description);

    public partial class MainViewModel : ViewModelBase, IDisposable
    {
        private static MainViewModel _instance;

        public List<AppThemeData> AccentColors { get; set; }
        public List<AppThemeData> AppThemes { get; set; }
        private PrintRequest _printRequest;
        private DispatcherTimer _timer = new DispatcherTimer();

        public MainViewModel()
        {
            PrinterIp = Settings.Default.PrinterIp;
            NetworkMask = Settings.Default.NetworkMask;
            TimerInterval = Settings.Default.TimerInterval;
            RowCount = Settings.Default.RowCount;
            Topmost = Settings.Default.Topmost;
            ShowTickets = Settings.Default.ShowTickets;
            AutoRefresh = Settings.Default.AutoRefresh;
            Init();
        }

        private void ToggleAutoRefresh()
        {
            if (AutoRefresh)
            {
                _timer.Tick += new EventHandler(Timer_Tick);
                TimeSpan timeSpan = new TimeSpan(0, Settings.Default.TimerInterval, 0);
                _timer.Interval = timeSpan;
                _timer.Start();
            }
            else
            {
                _timer.Stop();
            }
        }

        /// <summary>
        /// Ищет принтеры в сети
        /// 
        /// Параметр bool reScanNetwork уточняет, следует ли пересканировать всю сеть даже если принтер указан.
        /// Следует сказать False при автозапусе и True при настройке
        /// </summary>
        private void StartPrintService()
        {
            if (_printRequest == null)
            {
                _printRequest = new PrintRequest();
                _printRequest.OnPrinterDiscoveryBegin += OnPrinterDiscoveryBegin;
                _printRequest.OnPrinterDiscoveryCompleted += OnPrinterDiscoveryCompleted;
                _printRequest.OnPrinting += OnPrinting;
                _printRequest.OnPrinted += OnPrinted;
            }

            Printer printer = new Printer()
            {
                PrinterIp = PrinterIp,
                NetworkMask = NetworkMask,
                IsFound = false,
                AutoSearch = AutoSearch,
                PrinterInfo = String.Empty
            };

            ThreadPool.QueueUserWorkItem(_printRequest.Begin, printer);
        }

        private void OnPrinterDiscoveryBegin(object sender, NetworkEventArgs e)
        {
            PrinterStatus = e.Message;
            PrintProgressVisibility = Visibility.Visible;
        }

        private void OnPrinterDiscoveryCompleted(object sender, NetworkEventArgs e)
        {
            PrinterFound = e.Printer.IsFound;
            PrinterStatus = e.Message;
            PrinterInfo = e.Printer.PrinterInfo;
            PrintProgressVisibility = Visibility.Collapsed;
            if (e.Printer.IsFound)
            {
                PrinterIp = e.Printer.PrinterIp;
                Settings.Default.PrinterIp = PrinterIp;
                Settings.Default.Save();
            }
        }

        private void OnPrinting(object sender, PrinterEventArgs e)
        {
            PrinterStatus = e.Message;
            PrintProgressVisibility = Visibility.Visible;
        }

        private void OnPrinted(object sender, PrinterEventArgs e)
        {
            PrinterStatus = e.Message;
            PrintProgressVisibility = Visibility.Collapsed;
        }

        private void Init()
        {
            LoadData();
            //StartPrintService();
            LoadThemes();
            ToggleAutoRefresh();
        }

        private void LoadThemes()
        {
            Tuple<AppTheme, Accent> appTheme = ThemeManager.DetectAppStyle(Application.Current);
            AccentColors = ThemeManager.Accents
                .Select(
                    a =>
                        new AppThemeData() { Name = a.Name, ColorBrush = a.Resources["AccentColorBrush"] as Brush })
                .ToList();

            AppThemes = ThemeManager.AppThemes
                .Select(
                    a =>
                        new AppThemeData()
                        {
                            Name = a.Name,
                            BorderColorBrush = a.Resources["BlackColorBrush"] as Brush,
                            ColorBrush = a.Resources["WhiteColorBrush"] as Brush
                        })
                .ToList();

            foreach (AppThemeData theme in AppThemes)
            {
                if (theme.Name == appTheme.Item1.Name)
                {
                    _selectedTheme = theme;
                }
            }

            foreach (AppThemeData accent in AccentColors)
            {
                if (accent.Name == appTheme.Item2.Name)
                {
                    _selectedAccent = accent;
                }
            }
        }

        public void Dispose()
        {
            if (_timer != null)
            {
                _timer.Stop();
            }
            _worker = null;
            _printRequest = null;
        }
    }
}