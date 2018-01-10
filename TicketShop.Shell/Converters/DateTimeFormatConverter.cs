using System;
using System.Globalization;
using System.Windows.Data;

namespace TicketShop.Shell.Converters
{
    public class DateTimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((DateTime)value == DateTime.MinValue)
                return string.Empty;
            else
                return ((DateTime)value).ToString((string)parameter);
        }


        public object ConvertBack(object value, System.Type targetType, object parameter, CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }
}