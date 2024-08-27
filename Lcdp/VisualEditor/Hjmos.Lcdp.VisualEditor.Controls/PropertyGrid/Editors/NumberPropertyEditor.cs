using HandyControl.Controls;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    public class NumberPropertyEditor : PropertyEditorBase
    {
        public NumberPropertyEditor()
        {

        }

        public NumberPropertyEditor(double minimum, double maximum)
        {
            Minimum = minimum;
            Maximum = maximum;
        }

        public double Minimum { get; set; }

        public double Maximum { get; set; }

        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new NumericUpDown
        {
            IsReadOnly = propertyItem.IsReadOnly,
            Minimum = Minimum,
            Maximum = Maximum
        };

        public override DependencyProperty GetDependencyProperty() => NumericUpDown.ValueProperty;

        protected override IValueConverter GetConverter(PropertyItem propertyItem) => new NumberConverter(Minimum, Maximum);
    }

    public class NumberConverter : IValueConverter
    {

        public NumberConverter(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public double Min { get; }
        public double Max { get; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var v = System.Convert.ToDouble(value);

            if (v < Min) return Min;
            else if (v > Max) return Max;
            else return v;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => value;
    }
}