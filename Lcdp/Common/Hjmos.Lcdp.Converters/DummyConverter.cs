using System;
using System.Globalization;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    public class DummyConverter : IValueConverter
    {
        public static readonly DummyConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
