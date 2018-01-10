using System;
using System.Globalization;
using System.Windows.Data;
using TicketShop.Core;

namespace TicketShop.Shell.Converters
{
    /// <summary>
    /// Converts a ItemStatus value to a status string.
    /// </summary>
    public class StatusToInfoConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return BaseData.GetStatus((ItemStatus)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}