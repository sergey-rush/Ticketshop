using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Markup;
using TicketShop.Data;
using TicketShop.Shell.Models;
using TicketShop.Shell.ViewModels;
using TicketShop.Shell.Views;

namespace TicketShop.Shell
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            ExceptionHandler.Init();

            Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("ru-RU");
            FrameworkElement.LanguageProperty.OverrideMetadata(typeof (FrameworkElement),
                new FrameworkPropertyMetadata(XmlLanguage.GetLanguage(CultureInfo.CurrentCulture.IetfLanguageTag)));

            Properties.Settings df = Shell.Properties.Settings.Default;
            AppThemeData theme = new AppThemeData() {Name = df.UserTheme};
            theme.ChangeTheme();
            AppThemeData accent = new AppThemeData() {Name = df.UserAccent};
            accent.ChangeAccent();

            base.OnStartup(e);
#if (DEBUG)
            //RunInDebugMode();
            RunInReleaseMode();
#else
            RunInReleaseMode();
#endif
            //ShutdownMode = ShutdownMode.OnMainWindowClose;
        }

        private static void RunInDebugMode()
        {
            Logger.Default.Append("Application debug started");
            LoginDialog loginDialog = new LoginDialog();
            loginDialog.Show();

            //MainView mainWindow = new MainView();
            //mainWindow.Show();
        }

        private static void RunInReleaseMode()
        {
            AppDomain.CurrentDomain.UnhandledException += AppDomainUnhandledException;
            try
            {
                Logger.Default.Append("Application started");
                LoginDialog loginDialog = new LoginDialog();
                loginDialog.Show();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        private static void AppDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException(e.ExceptionObject as Exception);
        }

        private static void HandleException(Exception ex)
        {
            if (ex == null)
                return;

            MessageBox.Show(ex.Message, "Error");
            Environment.Exit(1);
        }

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            MainViewModel.Instance.Dispose();
            if (MainViewModel.Instance != null)
            {
                //var w = MainViewModel.Instance._worker;
                //var d = MainViewModel.Instance._progressDialog;
                //MainViewModel.Instance = null;
            }
            //MessageBox.Show(String.Format("Приложение будет закрыто. Код выхода: {0}", e.ApplicationExitCode), "Завершение работы");
            Logger.Default.Append("Application closed");
            Logger.Exit();
            //ThreadPool.QueueUserWorkItem(Logger.Exit);
        }
    }
}
