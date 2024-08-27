using System.Windows;
using System.Windows.Controls.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    public class SwitchPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new ToggleButton
        {
            Style = Helpers.ResourceHelper.GetResourceInternal<Style>("ToggleButtonSwitch"),
            HorizontalAlignment = HorizontalAlignment.Left,
            IsEnabled = !propertyItem.IsReadOnly
        };

        public override DependencyProperty GetDependencyProperty() => ToggleButton.IsCheckedProperty;
    }
}
