using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro;
using TicketShop.Shell.Commands;
using TicketShop.Shell.Properties;

namespace TicketShop.Shell.Models
{
    public class AppThemeData
    {
        public string Name { get; set; }
        public Brush BorderColorBrush { get; set; }
        public Brush ColorBrush { get; set; }

        public void ChangeTheme()
        {
            Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(Application.Current);
            AppTheme appTheme = ThemeManager.GetAppTheme(Name);
            ThemeManager.ChangeAppStyle(Application.Current, theme.Item2, appTheme);
            Settings df = Settings.Default;
            df.UserTheme = Name;
            df.Save();
        }

        public void ChangeAccent()
        {
            Tuple<AppTheme, Accent> theme = ThemeManager.DetectAppStyle(Application.Current);
            Accent accent = ThemeManager.GetAccent(Name);
            ThemeManager.ChangeAppStyle(Application.Current, accent, theme.Item1);
            Settings df = Settings.Default;
            df.UserAccent = Name;
            df.Save();
        }
    }
}