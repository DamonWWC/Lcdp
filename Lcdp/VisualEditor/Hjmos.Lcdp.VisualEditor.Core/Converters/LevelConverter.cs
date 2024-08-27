using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Core.Converters
{
    public class LevelConverter : IValueConverter
    {
        public static readonly LevelConverter Instance = new LevelConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Thickness(2 + 14 * (int)value, 0, 0, 0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
