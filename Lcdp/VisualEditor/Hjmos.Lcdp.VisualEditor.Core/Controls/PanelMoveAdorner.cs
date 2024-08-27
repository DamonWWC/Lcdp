using Hjmos.Lcdp.Converters;
using Hjmos.Lcdp.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public class PanelMoveAdorner : Control
    {
        static PanelMoveAdorner() => DefaultStyleKeyProperty.OverrideMetadata(typeof(PanelMoveAdorner), new FrameworkPropertyMetadata(typeof(PanelMoveAdorner)));

        private ScaleTransform scaleTransform;

        public PanelMoveAdorner(DesignItem item)
        {
            this.item = item;

            scaleTransform = new ScaleTransform(1.0, 1.0);
            this.LayoutTransform = scaleTransform;
        }

        private readonly DesignItem item;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            e.Handled = true;
            //item.Services.Selection.SetSelectedComponents(new DesignItem [] { item }, SelectionTypes.Auto);
            new DragMoveMouseGesture(item, false, true).Start(item.Services.DesignPanel, e);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            Binding bnd = new("IsVisible") { Source = item.Component };
            bnd.Converter = CollapsedWhenFalseConverter.Instance;
            BindingOperations.SetBinding(this, VisibilityProperty, bnd);

            DesignSurface surface = this.TryFindParent<DesignSurface>();
            if (surface != null && surface.ZoomControl != null)
            {
                bnd = new Binding("CurrentZoom") { Source = surface.ZoomControl };
                bnd.Converter = InvertedZoomConverter.Instance;

                BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleXProperty, bnd);
                BindingOperations.SetBinding(scaleTransform, ScaleTransform.ScaleYProperty, bnd);
            }
        }
    }
}
