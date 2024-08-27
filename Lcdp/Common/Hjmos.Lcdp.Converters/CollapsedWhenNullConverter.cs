using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    public class CollapsedWhenNullConverter : IValueConverter
    {
        public static readonly CollapsedWhenNullConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null ? Visibility.Visible : (object)Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
