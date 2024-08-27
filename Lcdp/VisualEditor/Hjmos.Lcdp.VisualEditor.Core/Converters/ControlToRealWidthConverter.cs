using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Core.Converters
{
    public class ControlToRealWidthConverter : IMultiValueConverter
    {
        public static readonly ControlToRealWidthConverter Instance = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return PlacementOperation.GetRealElementSize((UIElement)values[0]).Width;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

}
