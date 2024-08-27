using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    /// <summary>
    /// 显示一个存根，表明没有设置边界
    /// 有设置边界的时候，显示线和箭头，没有设置边界的时候，显示一个空心圆圈（存根）
    /// </summary>
    public class MarginStub : Control
    {
        /// <summary>
        /// 使用此存根获取空白手柄。
        /// </summary>
        public MarginHandle Handle { get; }

        static MarginStub() => DefaultStyleKeyProperty.OverrideMetadata(typeof(MarginStub), new FrameworkPropertyMetadata(typeof(MarginStub)));

        public MarginStub(MarginHandle marginHandle) => this.Handle = marginHandle;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            Handle.DecideVisiblity(Handle.HandleLength);
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            this.Cursor = Cursors.Hand;
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            this.Cursor = Cursors.Arrow;
        }
    }
}
