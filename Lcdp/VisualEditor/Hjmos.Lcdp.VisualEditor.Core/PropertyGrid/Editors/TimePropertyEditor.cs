using HandyControl.Controls;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public class TimePropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new DateTimePicker
        {
            IsEnabled = !propertyItem.IsReadOnly
        };

        public override DependencyProperty GetDependencyProperty() => TimePicker.SelectedTimeProperty;
    }
}
