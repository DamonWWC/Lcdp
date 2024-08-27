using System;
using System.Globalization;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    public class FormatDoubleConverter : IValueConverter
    {
        public static readonly FormatDoubleConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Math.Round((double)value);
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
