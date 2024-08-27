using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.Plugins.NccControl.Converters
{
    public class EmergencyCommandMenuCursorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isEnabled && isEnabled ? Cursors.Hand : Cursors.No;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class EmergencyCommandMenuForegroundEnableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool isEnabled && isEnabled ? Brushes.White : Brushes.Gray;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class SelectedEmergencyCommandMenuConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isSeleted && isSeleted)
            {
                LinearGradientBrush brush = new () { EndPoint = new System.Windows.Point { X = 0.5, Y = 1 }, StartPoint = new System.Windows.Point { X = 0.5, Y = 0 } };
                brush.GradientStops.Add(new GradientStop(color: (Color)ColorConverter.ConvertFromString("#06BFEA"), offset: 0));
                brush.GradientStops.Add(new GradientStop(color: (Color)ColorConverter.ConvertFromString("#0073C9"), offset: 0.6));
                return brush;
            }
            return Brushes.Transparent;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
