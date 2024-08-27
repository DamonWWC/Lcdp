﻿using System;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public class EnumPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new System.Windows.Controls.ComboBox
        {
            IsEnabled = !propertyItem.IsReadOnly,
            ItemsSource = Enum.GetValues(propertyItem.PropertyType)
        };

        public override DependencyProperty GetDependencyProperty() => Selector.SelectedValueProperty;
    }
}
