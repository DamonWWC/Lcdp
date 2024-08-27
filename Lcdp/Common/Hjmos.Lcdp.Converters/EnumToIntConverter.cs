using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    /// <summary>
    /// 将枚举值转换为整数值
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(int))]
    public class EnumToIntConverter : IValueConverter
    {
        /// <summary>
        /// 将枚举值转换为整数值
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is Enum enumeration)) return DependencyProperty.UnsetValue;
            return System.Convert.ToInt32(enumeration);
        }

        /// <summary>
        /// 此方法未实现
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}