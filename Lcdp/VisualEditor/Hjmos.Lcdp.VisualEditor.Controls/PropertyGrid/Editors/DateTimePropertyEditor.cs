using HandyControl.Controls;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    public class DateTimePropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new DateTimePicker
        {
            IsEnabled = !propertyItem.IsReadOnly
        };

        public override DependencyProperty GetDependencyProperty() => DateTimePicker.SelectedDateTimeProperty;
    }
}
