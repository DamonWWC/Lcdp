using Hjmos.Lcdp.VisualEditor.Controls.UIExtensions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Controls.Thumbs
{
    /// <summary>
    /// 一个thumb，它的外观可以依赖于IsPrimarySelection属性。
    /// </summary>
    public class DesignerThumb : Thumb
    {
        /// <summary>
        /// Dependency property for <see cref="IsPrimarySelection"/>.
        /// </summary>
        public static readonly DependencyProperty IsPrimarySelectionProperty
            = DependencyProperty.Register("IsPrimarySelection", typeof(bool), typeof(DesignerThumb));

        /// <summary>
        /// Dependency property for <see cref="IsPrimarySelection"/>.
        /// </summary>
        public static readonly DependencyProperty ThumbVisibleProperty
            = DependencyProperty.Register("ThumbVisible", typeof(bool), typeof(DesignerThumb), new FrameworkPropertyMetadata(SharedInstances.BoxedTrue));

        /// <summary>
        /// Dependency property for <see cref="OperationMenu"/>.
        /// </summary>
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
        /// Gets/Sets if the resize thumb is attached to the primary selection.
        /// </summary>
        public bool IsPrimarySelection
        {
            get => (bool)GetValue(IsPrimarySelectionProperty);
            set => SetValue(IsPrimarySelectionProperty, value);
        }

        /// <summary>
        /// Gets/Sets if the resize thumb is visible.
        /// </summary>
        public bool ThumbVisible
        {
            get => (bool)GetValue(ThumbVisibleProperty);
            set => SetValue(ThumbVisibleProperty, value);
        }

        /// <summary>
        /// Gets/Sets the OperationMenu.
        /// </summary>
        public Control[] OperationMenu
        {
            get => (Control[])GetValue(OperationMenuProperty);
            set => SetValue(OperationMenuProperty, value);
        }
    }
}
