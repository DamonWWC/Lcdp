using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.Plugins.NccControl.Converters
{
    public class HeightToMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double va)
            {
                return new Thickness(0, va * 0.08, 0, va * 0.08);
            }
            return new Thickness(0);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
