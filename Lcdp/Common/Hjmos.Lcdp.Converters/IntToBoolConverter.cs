using System;
using System.Globalization;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    /// <summary>
    /// 将整数值转换为布尔值
    /// </summary>
    [ValueConversion(typeof(int), typeof(bool))]
    public class IntToBoolConverter : IValueConverter
    {

        public static readonly IntToBoolConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value != null && parameter != null && int.Parse(value.ToString()) == int.Parse(parameter.ToString());
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}
