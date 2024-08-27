using System;
using System.Globalization;
using System.Windows.Data;

namespace Hjmos.Lcdp.Plugins.CS6.Converters
{
    /// <summary>
    /// 按比例计算宽度
    /// </summary>
    public class PercentWidthConverter : IMultiValueConverter
    {
        public static readonly PercentWidthConverter Instance = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double width = System.Convert.ToDouble(values[0]);
            double total = System.Convert.ToDouble(values[1]);
            double value = System.Convert.ToDouble(values[2]);
            int sectionCount = System.Convert.ToInt32(values[3]);
            double gap = System.Convert.ToInt32(values[4]);

            // ( 总宽度 - 间隔) * 百分比
            return (width - (sectionCount - 1) * gap) * value / total;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotSupportedException();
    }

}
