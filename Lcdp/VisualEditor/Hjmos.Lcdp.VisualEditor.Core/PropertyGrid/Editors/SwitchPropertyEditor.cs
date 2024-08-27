using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public class SwitchPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new ToggleButton
        {
            Style = ResourceHelper.GetResourceInternal<Style>("ToggleButtonSwitch"),
            HorizontalAlignment = HorizontalAlignment.Left,
            IsEnabled = !propertyItem.IsReadOnly
        };

        public override DependencyProperty GetDependencyProperty() => ToggleButton.IsCheckedProperty;
    }
}
