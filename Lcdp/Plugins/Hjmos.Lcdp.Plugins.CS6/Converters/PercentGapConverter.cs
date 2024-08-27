using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Plugins.CS6.Converters
{
    /// <summary>
    /// 根据Gap生成右侧Margin
    /// </summary>
    [ValueConversion(typeof(int), typeof(int))]
    public class PercentGapConverter : IValueConverter
    {
        public static readonly PercentGapConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => new Thickness(0, 0, System.Convert.ToInt32(value), 0);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
