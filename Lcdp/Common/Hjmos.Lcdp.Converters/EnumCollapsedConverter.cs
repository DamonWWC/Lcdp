using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Converters
{
    public class EnumCollapsedConverter : IValueConverter
    {
        public static readonly EnumCollapsedConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string parameterString)
                return DependencyProperty.UnsetValue;

            if (Enum.IsDefined(value.GetType(), value) == false)
                return DependencyProperty.UnsetValue;

            object parameterValue = Enum.Parse(value.GetType(), parameterString);

            return parameterValue.Equals(value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string parameterString)
                return DependencyProperty.UnsetValue;

            return Enum.Parse(targetType, parameterString);
        }
    }
}
