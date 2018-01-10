using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using TicketShop.Core;
using TicketShop.Map;
using TicketShop.Shell.Models;
using TicketShop.Shell.ViewModels;

namespace TicketShop.Shell.Views
{
    public partial class MainWindow : MetroWindow
    {
        private readonly MainViewModel _mainViewModel;
        public MainWindow(Member member)
        {
            _mainViewModel = MainViewModel.Instance;
            _mainViewModel.Member = member;
            DataContext = _mainViewModel;
            InitializeComponent();
            _mainViewModel.MetroWindow = this;
        }

        public static readonly DependencyProperty ToggleFullScreenProperty =
            DependencyProperty.Register("ToggleFullScreen",
                                        typeof(bool),
                                        typeof(MainWindow),
                                        new PropertyMetadata(default(bool), ToggleFullScreenPropertyChangedCallback));

        private static void ToggleFullScreenPropertyChangedCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var metroWindow = (MetroWindow)dependencyObject;
            if (e.OldValue != e.NewValue)
            {
                var fullScreen = (bool)e.NewValue;
                if (fullScreen)
                {
                    metroWindow.WindowState = WindowState.Maximized;
                    metroWindow.UseNoneWindowStyle = true;
                    metroWindow.IgnoreTaskbarOnMaximize = true;
                }
                else
                {
                    metroWindow.WindowState = WindowState.Normal;
                    metroWindow.UseNoneWindowStyle = false;
                    metroWindow.ShowTitleBar = true; // <-- this must be set to true
                    metroWindow.IgnoreTaskbarOnMaximize = false;
                }
            }
        }

        public bool ToggleFullScreen
        {
            get { return (bool)GetValue(ToggleFullScreenProperty); }
            set { SetValue(ToggleFullScreenProperty, value); }
        }

        private void LaunchBugReportView(object sender, RoutedEventArgs e)
        {
            WindowCollection wc = _mainViewModel.MetroWindow.OwnedWindows;
            foreach (Window window in wc)
            {
                if (window.GetType() == typeof(BugReportView))
                {
                    window.Topmost = true;
                    return;
                }
            }
            
            new BugReportView() { Owner = this }.Show();
        }


        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            //MessageBoxResult dr = MessageBox.Show("Приложение будет закрыть?", "Выход",
            //        MessageBoxButton.OKCancel, MessageBoxImage.Question);
            //if (dr == MessageBoxResult.OK)
            //{
            //    Application.Current.Shutdown();
            //}
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F5)
            {
                _mainViewModel.LoadData();
            }
        }
    }
}
