using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    public class CollapsedWhenZeroConverter : IValueConverter
    {
        public static readonly CollapsedWhenZeroConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value == null || (value is int @int && @int == 0) ? Visibility.Collapsed : (object)Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
