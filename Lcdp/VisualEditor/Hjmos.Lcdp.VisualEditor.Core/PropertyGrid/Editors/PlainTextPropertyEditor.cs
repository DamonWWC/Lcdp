using System.Windows;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public class PlainTextPropertyEditor : PropertyEditorBase
    {
        public override FrameworkElement CreateElement(PropertyItem propertyItem) => new System.Windows.Controls.TextBox
        {
            IsReadOnly = propertyItem.IsReadOnly,
            Background = new SolidColorBrush(Color.FromRgb(60, 60, 60)),
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0)
        };

        public override DependencyProperty GetDependencyProperty() => System.Windows.Controls.TextBox.TextProperty;
    }
}
