using Hjmos.Lcdp.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Core.Thumbs
{
    /// <summary>
    /// 一个thumb，它的外观可以依赖于IsPrimarySelection属性。
    /// </summary>
    public class DesignerThumb : Thumb
    {

        public static readonly DependencyProperty IsPrimarySelectionProperty
            = DependencyProperty.Register("IsPrimarySelection", typeof(bool), typeof(DesignerThumb));

        public static readonly DependencyProperty ThumbVisibleProperty
            = DependencyProperty.Register("ThumbVisible", typeof(bool), typeof(DesignerThumb), new FrameworkPropertyMetadata(SharedInstances.BoxedTrue));

        public static readonly DependencyProperty OperationMenuProperty =
            DependencyProperty.Register("OperationMenu", typeof(Control[]), typeof(DesignerThumb), new PropertyMetadata(null));

        public PlacementAlignment Alignment;

        static DesignerThumb() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignerThumb), new FrameworkPropertyMetadata(typeof(DesignerThumb)));

        public void ReDraw()
        {
            FrameworkElement parent = this.TryFindParent<FrameworkElement>();
            if (parent != null)
                parent.InvalidateArrange();
        }

        /// <summary>
        /// 获取/设置大小调整手柄是否附加到主选择
        /// </summary>
        public bool IsPrimarySelection
        {
            get => (bool)GetValue(IsPrimarySelectionProperty);
            set => SetValue(IsPrimarySelectionProperty, value);
        }

        /// <summary>
        /// 获取/设置大小调整手柄是否可见
        /// </summary>
        public bool ThumbVisible
        {
            get => (bool)GetValue(ThumbVisibleProperty);
            set => SetValue(ThumbVisibleProperty, value);
        }

        /// <summary>
        /// 获取操作菜单
        /// </summary>
        public Control[] OperationMenu
        {
            get => (Control[])GetValue(OperationMenuProperty);
            set => SetValue(OperationMenuProperty, value);
        }
    }
}
