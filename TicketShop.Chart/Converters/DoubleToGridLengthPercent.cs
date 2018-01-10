using System;
using System.Windows;
using System.Windows.Data;

namespace TicketShop.Chart.Converters
{
    #if NETFX_CORE
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml;
#else

#endif

    public class DoubleToGridLengthPercent : IValueConverter
    {
#if NETFX_CORE
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return InternalConvert(value, targetType, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
#else
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return InternalConvert(value, targetType, parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
#endif

        private object InternalConvert(object value, Type targetType, object parameter)
        {
            double percentage = (double)value;
            if (parameter != null)
            {
                if (percentage <= 1)
                {
                    return new GridLength(1.0 - (double)percentage, GridUnitType.Star);
                }
                return new GridLength(100.0 - (double)percentage, GridUnitType.Star);
            }
            else
            {
                return new GridLength((double)percentage, GridUnitType.Star);
            }
        }
    }
}
