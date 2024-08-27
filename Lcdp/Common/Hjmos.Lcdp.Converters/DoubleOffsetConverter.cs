using System;
using System.Globalization;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    public class DoubleOffsetConverter : IValueConverter
    {
        public double Offset { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value + Offset;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (double)value - Offset;
        }
    }
}
