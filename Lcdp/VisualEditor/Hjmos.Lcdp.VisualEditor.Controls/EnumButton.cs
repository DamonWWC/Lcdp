using System.Windows.Controls.Primitives;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    public class EnumButton : ToggleButton
    {
        static EnumButton() => DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumButton), new FrameworkPropertyMetadata(typeof(EnumButton)));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(EnumButton));

        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }
    }
}
