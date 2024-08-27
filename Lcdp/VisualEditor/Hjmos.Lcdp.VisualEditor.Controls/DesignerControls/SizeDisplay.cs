using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    /// <summary>
    /// Display height of the element.
    /// </summary>
    public class HeightDisplay : Control
    {
        static HeightDisplay() => DefaultStyleKeyProperty.OverrideMetadata(typeof(HeightDisplay), new FrameworkPropertyMetadata(typeof(HeightDisplay)));
    }

    /// <summary>
    /// Display width of the element.
    /// </summary>
    public class WidthDisplay : Control
    {
        static WidthDisplay() => DefaultStyleKeyProperty.OverrideMetadata(typeof(WidthDisplay), new FrameworkPropertyMetadata(typeof(WidthDisplay)));
    }
}
