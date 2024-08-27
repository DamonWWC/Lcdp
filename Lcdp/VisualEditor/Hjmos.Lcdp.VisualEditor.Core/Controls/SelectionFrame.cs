using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.Controls
{
    /// <summary>
    /// 在橡皮筋选择(a rubber-band selecting operation)操作期间显示的矩形。
    /// </summary>
    public class SelectionFrame : Control
    {
        static SelectionFrame() => DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectionFrame), new FrameworkPropertyMetadata(typeof(SelectionFrame)));
    }
}
