using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    public class PlainTextPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new System.Windows.Controls.TextBox
        {
            IsReadOnly = propertyItem.IsReadOnly
        };

        public override DependencyProperty GetDependencyProperty() => System.Windows.Controls.TextBox.TextProperty;
    }
}
