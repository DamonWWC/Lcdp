using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    public class ThicknessPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new System.Windows.Controls.TextBox
        {
            IsReadOnly = propertyItem.IsReadOnly
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

