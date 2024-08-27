using System.Windows;
using System.Windows.Controls.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Core.Controls
{
    public class EnumButton : ToggleButton
    {
        static EnumButton() => DefaultStyleKeyProperty.OverrideMetadata(typeof(EnumButton), new FrameworkPropertyMetadata(typeof(EnumButton)));

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("Value", typeof(object), typeof(EnumButton));

        public object Value
        {
            get => GetValue(ValueProperty);
            set => SetValue(ValueProperty, value);
        }
    }
}
