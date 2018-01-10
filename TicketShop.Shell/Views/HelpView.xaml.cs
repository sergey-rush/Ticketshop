using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TicketShop.Shell.Views
{
    public partial class HelpView : UserControl
    {
        public HelpView()
        {
            InitializeComponent();
            InitBrowser();
        }

        // change the UA string
        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);

        private const int URLMON_OPTION_USERAGENT = 0x10000001;

        private void InitBrowser()
        {
            myBrowser.Navigated += new NavigatedEventHandler(wbMain_Navigated);
            myBrowser.Navigate("http://main.zirk.ru/Help/Media?media=ticketshop");
            myUrl.Text = "http://main.zirk.ru/Help/Media?media=ticketshop";
            uri = "http://main.zirk.ru/Help/Media?media=ticketshop";

            //ChangeUserAgent("Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            ChangeUserAgent("Mozilla/5.0 (compatible; Ticketshop/1.0; +http://www.main.zirk.ru/ticketshop.html)");


            navigationKeys();

            // this code sets the height and the width of the WebBrowser element.
            //myBrowser.Width = this.Width;
            //myBrowser.Height = (this.Height - 59);
        }

        public void ChangeUserAgent(string Agent)
        {
            UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, Agent, Agent.Length, 0);
        }

        private string uri = string.Empty;

        // The navigation keys settings
        private void navigationKeys()
        {
            if (!myBrowser.CanGoForward)
            {
                BrowserGoForward.IsEnabled = false;
            }
            else
            {
                BrowserGoForward.IsEnabled = true;
            }

            if (!myBrowser.CanGoBack)
            {
                BrowserGoBack.IsEnabled = false;
            }
            else
            {
                BrowserGoBack.IsEnabled = true;
            }
        }

        private void myUrl_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string url = textBox.Text;
            if (e.Key == Key.Enter)
            {
                Navigate(url);
            }
        }

        private void Navigate(string url)
        {
            try
            {
                myBrowser.Navigate(url);
            }
            catch
            {
                // if there is an error in the URI string, handle it
                if (url.IndexOf("http://", StringComparison.Ordinal) == -1 || url.IndexOf("https://", StringComparison.Ordinal) == -1)
                {
                    // there was no URI, append it
                    url = "http://" + url;
                    myBrowser.Focus();
                    try
                    {
                        myBrowser.Navigate(url.Replace("..", "."));
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Адрес не корректный: {0}", ex.Message);
                    }
                    myUrl.Text = myBrowser.Source.ToString();
                }
            }
        }

        // Enter button handler, loads page if ENTER key was pressed
        private void myBrowser_KeyDown(object sender, KeyEventArgs e)
        {
            WebBrowser myBrowser = sender as WebBrowser;

            // get if the key is BACKSPACE then go back!
            if (e.Key == Key.Back)
            {
                if (myBrowser.CanGoBack)
                {
                    myBrowser.GoBack();
                }
            }
        }

        // Back button handler, loads previous page in history (if present)
        private void BrowserGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (myBrowser.CanGoBack)
            {
                myBrowser.GoBack();
            }
        }

        // Forward button handler, loads next page in history (if present)
        private void BrowserGoForward_Click(object sender, RoutedEventArgs e)
        {
            if (myBrowser.CanGoForward)
            {
                myBrowser.GoForward();
            }
        }
        
        private void myBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            uri = myUrl.Text;
        }

        private void myBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            navigationKeys();
            myUrl.Text = myBrowser.Source.ToString();
        }

        private void myBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            navigationKeys();
            myUrl.Text = myBrowser.Source.ToString();
        }
        
        private void BrowserRefresh_Click(object sender, RoutedEventArgs e)
        {
            myBrowser.Refresh();
            myUrl.Text = myBrowser.Source.ToString();
        }

        private void BrowserGo_Click(object sender, RoutedEventArgs e)
        {
            string url = myUrl.Text;
            Navigate(url);
        }

        private void BrowserHome_Click(object sender, RoutedEventArgs e)
        {
            string url = "http://main.zirk.ru/help/ticketshop.aspx";
            Navigate(url);
        }

        private void wbMain_Navigated(object sender, NavigationEventArgs e)
        {
            SetSilent(myBrowser, true); // make it silent
        }

        public static void SetSilent(WebBrowser browser, bool silent)
        {
            if (browser == null)
                throw new ArgumentNullException("browser");

            // get an IWebBrowser2 from the document
            IOleServiceProvider sp = browser.Document as IOleServiceProvider;
            if (sp != null)
            {
                Guid IID_IWebBrowserApp = new Guid("0002DF05-0000-0000-C000-000000000046");
                Guid IID_IWebBrowser2 = new Guid("D30C1661-CDAF-11d0-8A3E-00C04FC9E26E");

                object webBrowser;
                sp.QueryService(ref IID_IWebBrowserApp, ref IID_IWebBrowser2, out webBrowser);
                if (webBrowser != null)
                {
                    webBrowser.GetType()
                        .InvokeMember("Silent",
                            BindingFlags.Instance | BindingFlags.Public | BindingFlags.PutDispProperty, null, webBrowser,
                            new object[] { silent });
                }
            }
        }


        [ComImport, Guid("6D5140C1-7436-11CE-8034-00AA006009FA"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
        private interface IOleServiceProvider
        {
            [PreserveSig]
            int QueryService([In] ref Guid guidService, [In] ref Guid riid,
                [MarshalAs(UnmanagedType.IDispatch)] out object ppvObject);
        }
    }
}
