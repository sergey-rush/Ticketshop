using System;
using System.Threading;
using System.Windows.Forms;
using TicketShop.Data;

namespace TicketShop.Shell
{
    public class ExceptionHandler
    {
        public static void Init()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException -= Handle;
            Application.ThreadException += Handle;
            AppDomain.CurrentDomain.UnhandledException -= CurrentDomain_UnhandledException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            //здесь обрабатываются исключения не UI потоков
            var ex = e.ExceptionObject as Exception;
            if (ex != null)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;
            }
            Logger.Default.Append(String.Format("CurrentDomainUnhandledException {0}", e.ExceptionObject));
            MessageBox.Show(e.ExceptionObject.ToString());
        }

        private static void Handle(object sender, ThreadExceptionEventArgs e)
        {
            var ex = e.Exception;
            while (ex.InnerException != null)
                ex = ex.InnerException;

#if DEBUG
            using (var exceptionDlg = new ThreadExceptionDialog(ex))
            {
                Logger.Default.Append(String.Format("ThreadExceptionDialog {0}", ex.Message));
                var res = exceptionDlg.ShowDialog();
                if (res == DialogResult.Abort)
                    Application.Exit();
            }
#else
            MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
#endif
        }
    }
}