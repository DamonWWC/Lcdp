using Hjmos.Lcdp.VisualEditor.Core.BaseClass;
using Hjmos.Lcdp.VisualEditor.Core.CustomTypes;
using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public class DimensionPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new System.Windows.Controls.ComboBox
        {
            IsEnabled = !propertyItem.IsReadOnly,
            ItemsSource = (propertyItem.Value as DataFieldsBase).DimensionArray
        };

        public override DependencyProperty GetDependencyProperty() => Selector.SelectedValueProperty;

        protected override IValueConverter GetConverter(PropertyItem propertyItem) => new StringToDimensionConverter();
    }

    public class StringToDimensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return string.Empty;
            Dimension dimension = (Dimension)value;
            return dimension.Value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Dimension(value.ToString());
        }
    }
}