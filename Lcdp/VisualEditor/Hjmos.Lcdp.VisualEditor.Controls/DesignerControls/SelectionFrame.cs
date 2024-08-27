using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    /// <summary>
    /// 在橡皮筋选择(a rubber-band selecting operation)操作期间显示的矩形。
    /// </summary>
    public class SelectionFrame : Control
    {
        static SelectionFrame()
        {
            //This OverrideMetadata call tells the system that this element wants to provide a style that is different than its base class.
            //This style is defined in themes\generic.xaml
            // 这个OverrideMetadata调用告诉系统，这个元素想要提供与它的基类不同的样式。
            // 这种样式在themes\generic.xaml中定义
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SelectionFrame), new FrameworkPropertyMetadata(typeof(SelectionFrame)));
        }
    }
}
