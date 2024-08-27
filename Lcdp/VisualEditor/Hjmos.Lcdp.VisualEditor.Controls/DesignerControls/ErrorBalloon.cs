using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    /// <summary>
    /// 一个ErrorBalloon窗口。
    /// </summary>
    public class ErrorBalloon : Window
    {
        static ErrorBalloon() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ErrorBalloon), new FrameworkPropertyMetadata(typeof(ErrorBalloon)));
    }
}
