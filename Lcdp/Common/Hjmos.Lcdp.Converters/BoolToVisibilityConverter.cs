using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    /// <summary>
    /// 布尔值和可见性转换
    /// </summary>
    [ValueConversion(typeof(bool), typeof(Visibility))]
    public class BoolToVisibilityConverter : BaseVisibilityConverter, IValueConverter
    {

        public static readonly BoolToVisibilityConverter Instance = new();


        /// <summary>
        /// 将布尔值转换为可见性
        /// </summary>
        /// <param name="value">绑定源的值</param>
        /// <param name="targetType">绑定目标的类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture"></param>
        /// <returns>可见性</returns>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(bool)) return null;
            bool boolValue = IsInverted ? !(bool)value : (bool)value;
            return boolValue ? Visibility.Visible : FalseVisibilityValue;
        }

        /// <summary>
        /// 将可见性转换为布尔值
        /// </summary>
        /// <param name="value">绑定目标的值</param>
        /// <param name="targetType">要转换的类型</param>
        /// <param name="parameter">参数</param>
        /// <param name="culture"></param>
        /// <returns>指示是否可见的布尔值</returns>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || value.GetType() != typeof(Visibility)) return null;
            if (IsInverted) return (Visibility)value != Visibility.Visible;
            return (Visibility)value == Visibility.Visible;
        }
    }
}