using System;
using System.Globalization;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Core.DesignerControls.Converters
{
    /// <summary>
    /// Offset the Handle Length with MarginHandle.HandleLengthOffset
    /// 用MarginHandle.HandleLengthOffset偏移手柄长度
    /// </summary>
    public class HandleLengthWithOffset : IValueConverter
    {
        public static HandleLengthWithOffset Instance = new();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value is double ? Math.Max((double)value - MarginHandle.HandleLengthOffset, 0) : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}
