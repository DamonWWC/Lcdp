using HandyControl.Controls;
using Hjmos.Lcdp.VisualEditor.Controls.BaseClass;
using Hjmos.Lcdp.VisualEditor.Core.CustomTypes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// TODO：ListBox的SelectedItems不能做绑定，目前自定义绑定功能改一半，后续有需要再完善
    /// </summary>
    public class MultiDimensionPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem)
        {
            CheckComboBox control = new()
            {
                IsEnabled = !propertyItem.IsReadOnly,
                ItemsSource = (propertyItem.Value as DataFieldsBase).DimensionArray

            };
            control.SelectionChanged += (s, e) => SelectedItemsList = control.SelectedItems.Cast<string>().ToList();
            return control;
        }

        public override DependencyProperty GetDependencyProperty() => SelectedItemsListProperty;

        protected override IValueConverter GetConverter(PropertyItem propertyItem) => new ListToMultiDimensionConverter();

        public List<string> SelectedItemsList
        {
            get => (List<string>)GetValue(SelectedItemsListProperty);
            set => SetValue(SelectedItemsListProperty, value);
        }

        public static readonly DependencyProperty SelectedItemsListProperty =
            DependencyProperty.Register(nameof(SelectedItemsList), typeof(List<string>), typeof(CheckComboBox), new PropertyMetadata(default(List<string>)));

        public override void CreateBinding(PropertyItem propertyItem, DependencyObject element)
        => BindingOperations.SetBinding(this, GetDependencyProperty(),
            new Binding($"{propertyItem.PropertyName}")
            {
                Source = propertyItem.Value,
                Mode = GetBindingMode(propertyItem),
                UpdateSourceTrigger = GetUpdateSourceTrigger(propertyItem),
                Converter = GetConverter(propertyItem)
            });
    }

    public class ListToMultiDimensionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null) return null;
            DimensionArray dimensions = (DimensionArray)value;
            return dimensions.Values;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => new DimensionArray(value as List<string>);
    }
}