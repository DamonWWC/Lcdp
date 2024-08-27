using System;
using System.Globalization;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    public class FalseWhenNullConverter : IValueConverter
    {
        public static readonly FalseWhenNullConverter Instance = new FalseWhenNullConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
