using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public class ThicknessPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new System.Windows.Controls.TextBox
        {
            IsReadOnly = propertyItem.IsReadOnly,
            Background = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0)
        };

        public override DependencyProperty GetDependencyProperty() => System.Windows.Controls.TextBox.TextProperty;

        protected override IValueConverter GetConverter(PropertyItem propertyItem) => new StringToThicknessConverter();

        public override UpdateSourceTrigger GetUpdateSourceTrigger(PropertyItem propertyItem) => UpdateSourceTrigger.LostFocus;
    }

    public class StringToThicknessConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Thickness thickness = (Thickness)value;

            return $"{thickness.Left},{thickness.Top},{thickness.Right},{thickness.Bottom}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string str = value.ToString();
            List<int> list = str.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(x => int.Parse(x)).ToList();

            if (list.Count() == 4)
                return new Thickness(list[0], list[1], list[2], list[3]);
            else if (list.Count() == 2)
                return new Thickness(list[0], list[1], list[0], list[1]);
            else if (list.Count() == 1)
                return new Thickness(list[0]);
            else
                return new Thickness(0);

        }
    }
}

