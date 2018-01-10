using System;
using System.Globalization;
using System.Windows.Data;
using TicketShop.Core;

namespace TicketShop.Shell.Converters
{
    /// <summary>
    /// Converts a Result value to a string.
    /// </summary>
    public class ResultToInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BaseData.GetResult((int)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}