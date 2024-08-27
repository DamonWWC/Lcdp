using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    /// <summary>
    /// 枚举值和可见性转换
    /// </summary>
    [ValueConversion(typeof(Enum), typeof(Visibility))]
    public class EnumToVisibilityConverter : BaseVisibilityConverter, IValueConverter
    {
        public static readonly EnumToVisibilityConverter Instance = new();

        /// <summary>
        /// 将枚举值转换为可见性
        /// </summary>
        /// <param name="value">绑定源的值</param>
        /// <param name="targetType">绑定目标的类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture">The culture to use in the converter.</param>
        /// <returns>可见性</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || (value.GetType() != typeof(Enum) && value.GetType().BaseType != typeof(Enum)) || parameter == null) return false;
            string enumValue = value.ToString();
            string targetValue = parameter.ToString();
            bool boolValue = enumValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
            boolValue = IsInverted ? !boolValue : boolValue;
            return boolValue ? Visibility.Visible : FalseVisibilityValue;
        }

        /// <summary>
        /// 将可见性转换为枚举值
        /// </summary>
        /// <param name="value">绑定目标的值</param>
        /// <param name="targetType">要转换的类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture"></param>
        /// <returns>枚举值</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(Visibility) || parameter == null) return null;
            Visibility usedValue = (Visibility)value;
            string targetValue = parameter.ToString();
            if (IsInverted && usedValue != Visibility.Visible) return Enum.Parse(targetType, targetValue);
            else if (!IsInverted && usedValue == Visibility.Visible) return Enum.Parse(targetType, targetValue);
            return DependencyProperty.UnsetValue;
        }
    }
}