using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    /// <summary>
    /// 将字符串转换为字符串的第一个字母
    /// </summary>
    [ValueConversion(typeof(string), typeof(string))]
    public class StringToFirstLetterConverter : IValueConverter
    {
        /// <summary>
        /// 将字符串转换为字符串的第一个字母
        /// </summary>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return DependencyProperty.UnsetValue;
            string stringValue = value.ToString();
            if (stringValue.Length < 1) return DependencyProperty.UnsetValue;
            return stringValue[0];
        }

        /// <summary>
        /// 该方法未实现
        /// </summary>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DependencyProperty.UnsetValue;
        }
    }
}