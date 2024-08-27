using Hjmos.Lcdp.Converters;
using Hjmos.Lcdp.Helpers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.Controls
{
    /// <summary>
	/// 一个Thumb，它的外观可以依赖于IsPrimarySelection属性。
	/// 由UIElementSelectionRectangle使用。
    /// </summary>
    public class ContainerDragHandle : Control
    {
        static ContainerDragHandle() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ContainerDragHandle), new FrameworkPropertyMetadata(typeof(ContainerDragHandle)));

        /// <summary>缩放变换</summary>
        private readonly ScaleTransform scaleTransform;

        public ContainerDragHandle()
        {
            scaleTransform = new ScaleTransform(1.0, 1.0);
            this.LayoutTransform = scaleTransform;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            DesignSurface surface = this.TryFindParent<DesignSurface>();
            if (surface != null && surface.ZoomControl != null)
            {
                Binding binding = new("CurrentZoom") { Source = surface.ZoomControl };
                binding.Converter = InvertedZoomConverter.Instance;

                BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, binding);
                BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, binding);
            }
        }
    }
}
