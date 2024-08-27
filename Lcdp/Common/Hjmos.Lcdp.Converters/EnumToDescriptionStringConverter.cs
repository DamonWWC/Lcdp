using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Hjmos.Lcdp.Extensions;

namespace Hjmos.Lcdp.Converters
{
    /// <summary>
    /// 将Enum转换为DescriptionAttribute的内容，未设置将转换为其名称
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(string))]
    public class EnumToDescriptionStringConverter : IValueConverter
    {
        /// <summary>
        /// 将Enum转换为DescriptionAttribute的内容，未设置将转换为其名称
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (value.GetType() != typeof(Enum) && value.GetType().BaseType != typeof(Enum))) return false;
            Enum enumInstance = (Enum)value;
            return enumInstance.GetDescription();
        }

        /// <summary>
        /// 返回DependencyProperty.UnsetValue.
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}