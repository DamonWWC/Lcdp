using Hjmos.Lcdp.VisualEditor.Core.CustomTypes;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    public class TextAreaPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new TextBox
        {
            IsReadOnly = propertyItem.IsReadOnly,
            TextWrapping = TextWrapping.Wrap,
            AcceptsReturn = true,
            VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            MinLines = 3,
            MaxLines = 10,
            VerticalContentAlignment = VerticalAlignment.Top
        };

        public override DependencyProperty GetDependencyProperty() => TextBox.TextProperty;

        protected override IValueConverter GetConverter(PropertyItem propertyItem) => new StringToTextAreaConverter();
    }

    public class StringToTextAreaConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            TextArea dimension = (TextArea)value;
            return dimension.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new TextArea(value.ToString());
        }
    }
}
