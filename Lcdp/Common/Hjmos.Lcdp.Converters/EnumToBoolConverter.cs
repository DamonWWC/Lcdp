using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    /// <summary>
    /// 如果Enum与输入参数的值匹配，则将枚举值转换为真布尔值，否则转换为假布尔值。
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(bool))]
    public class EnumToBoolConverter : IValueConverter
    {
        public static readonly EnumToBoolConverter Instance = new() { IsInverted = false };

        /// <summary>
        /// 指定布尔输出值在转换期间是否反转。  
        /// </summary>
        public bool IsInverted { get; set; }

        /// <summary>
        /// 将枚举值转换布尔值
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null || (value.GetType() != typeof(Enum) && value.GetType().BaseType != typeof(Enum))) return DependencyProperty.UnsetValue;
            string enumValue = value.ToString();
            string targetValue = parameter.ToString();
            bool boolValue = enumValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
            return IsInverted ? !boolValue : boolValue;
        }

        /// <summary>
        /// 将布尔值转换枚举值
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return DependencyProperty.UnsetValue;
            bool boolValue = (bool)value;
            string targetValue = parameter.ToString();
            if ((boolValue && !IsInverted) || (!boolValue && IsInverted)) return Enum.Parse(targetType, targetValue);
            return DependencyProperty.UnsetValue;
        }
    }
}