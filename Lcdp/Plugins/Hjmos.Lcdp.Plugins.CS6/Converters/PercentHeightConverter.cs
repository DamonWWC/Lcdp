using System;
using System.Globalization;
using System.Windows.Data;

namespace Hjmos.Lcdp.Plugins.CS6.Converters
{
    /// <summary>
    /// 按行数计算行高
    /// </summary>
    public class PercentHeightConverter : IMultiValueConverter
    {
        public static readonly PercentHeightConverter Instance = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return System.Convert.ToDouble(values[0]) / System.Convert.ToInt32(values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

}
