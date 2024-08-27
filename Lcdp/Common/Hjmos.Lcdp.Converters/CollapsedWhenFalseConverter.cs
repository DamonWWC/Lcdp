using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    public class CollapsedWhenFalseConverter : IValueConverter
    {
        public static readonly CollapsedWhenFalseConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
