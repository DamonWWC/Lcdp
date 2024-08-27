using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.Attached
{
    public class PartAttached
    {
        public static readonly DependencyProperty IsPartProperty = DependencyProperty.RegisterAttached("IsPart", typeof(bool), typeof(PartAttached));

        public static bool GetIsPart(DependencyObject d) => (bool)d.GetValue(IsPartProperty);

        public static void SetIsPart(DependencyObject d, bool value) => d.SetValue(IsPartProperty, value);
    }
}
