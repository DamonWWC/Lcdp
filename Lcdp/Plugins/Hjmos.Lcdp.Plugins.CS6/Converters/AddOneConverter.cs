using System;
using System.Globalization;
using System.Windows.Data;

namespace Hjmos.Lcdp.Plugins.CS6.Converters
{
    /// <summary>
    /// 数字加1
    /// </summary>
    [ValueConversion(typeof(int), typeof(int))]
    public class AddOneConverter : IValueConverter
    {
        public static readonly AddOneConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is null ? 0 : System.Convert.ToInt32(value) + 1;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
