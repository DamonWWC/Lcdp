using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Hjmos.Lcdp.Converters
{
    public class BlackWhenTrueConverter : IValueConverter
    {
        public static readonly BlackWhenTrueConverter Instance = new();

        private readonly Brush black = new SolidColorBrush(Colors.Black);

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? black : null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
