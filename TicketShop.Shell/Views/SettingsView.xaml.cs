using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
using TicketShop.Shell.ViewModels;

namespace TicketShop.Shell.Views
{
    public partial class SettingsView : UserControl
    {
        private readonly MainViewModel _mainViewModel;
        public SettingsView()
        {
            _mainViewModel = MainViewModel.Instance;
            DataContext = _mainViewModel;
            InitializeComponent();
            InitAboutBox();
        }

        public void InitAboutBox()
        {
            ProductNameLabel.Content = AssemblyProduct;
            VersionLabel.Content = String.Format("{0} Version {1}", AssemblyTitle, AssemblyVersion);
            CopyrightLabel.Content = AssemblyCopyright;
            CompanyNameLabel.Content = AssemblyCompany;
            DescriptionTextBlock.Text = AssemblyDescription;
        }

        #region Assembly Attribute Accessors

        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "")
                    {
                        return titleAttribute.Title;
                    }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        {
            get
            {
                return Assembly.GetExecutingAssembly().GetName().Version.ToString();
            }
        }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }
        #endregion

        

        private void ReportButton_OnClick(object sender, RoutedEventArgs e)
        {
            WindowCollection wc = _mainViewModel.MetroWindow.OwnedWindows;
            foreach (Window window in wc)
            {
                if (window.GetType() == typeof (BugReportView))
                {
                    window.Topmost = true;
                    return;
                }
            }

            new BugReportView() { Owner = _mainViewModel.MetroWindow }.Show();
        }
    }
}
