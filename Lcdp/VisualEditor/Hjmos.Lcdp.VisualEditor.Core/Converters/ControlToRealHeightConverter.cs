using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Core.Converters
{
    public class ControlToRealHeightConverter : IMultiValueConverter
    {
        public static readonly ControlToRealHeightConverter Instance = new();

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture) => PlacementOperation.GetRealElementSize((UIElement)values[0]).Height;

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}
