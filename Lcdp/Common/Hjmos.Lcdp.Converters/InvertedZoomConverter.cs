using System;
using System.Globalization;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    public class InvertedZoomConverter : IValueConverter
    {
        public static readonly InvertedZoomConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1.0 / ((double)value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return 1.0 / ((double)value);
        }
    }
}
