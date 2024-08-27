using Hjmos.Lcdp.VisualEditor.Controls.DesignerControls;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    /// <summary>
    /// 为<see cref="Canvas"/>提供<see cref="IPlacementBehavior"/>行为
    /// </summary>
    [ExtensionFor(typeof(Canvas), OverrideExtension = typeof(DefaultPlacementBehavior))]
    public class CanvasPlacementSupport : SnaplinePlacementBehavior
    {
        private GrayOutDesignerExceptActiveArea grayOut;
        private FrameworkElement extendedComponent;
        private FrameworkElement extendedView;

        private static double GetCanvasProperty(UIElement element, DependencyProperty d)
        {
            double v = (double)element.GetValue(d);
            return double.IsNaN(v) ? 0 : v;
        }

        private static bool IsPropertySet(UIElement element, DependencyProperty d) => element.ReadLocalValue(d) != DependencyProperty.UnsetValue;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            extendedComponent = ExtendedItem.Component as FrameworkElement;
            extendedView = ExtendedItem.View as FrameworkElement;
        }

        public override Rect GetPosition(PlacementOperation operation, DesignItem item)
        {
            UIElement child = item.View;

            if (child == null)
                return Rect.Empty;

            double x, y;

            if (IsPropertySet(child, Canvas.LeftProperty) || !IsPropertySet(child, Canvas.RightProperty))
            {
                x = GetCanvasProperty(child, Canvas.LeftProperty);
            }
            else
            {
                x = extendedComponent.ActualWidth - GetCanvasProperty(child, Canvas.RightProperty) - PlacementOperation.GetRealElementSize(child).Width;
            }


            if (IsPropertySet(child, Canvas.TopProperty) || !IsPropertySet(child, Canvas.BottomProperty))
            {
                y = GetCanvasProperty(child, Canvas.TopProperty);
            }
            else
            {
                y = extendedComponent.ActualHeight - GetCanvasProperty(child, Canvas.BottomProperty) - PlacementOperation.GetRealElementSize(child).Height;
            }

            Point point = new(x, y);
            //Fixes, Empty Image Resized to 0
            //return new Rect(p, child.RenderSize);
            return new Rect(point, PlacementOperation.GetRealElementSize(item.View));
        }

        public override void SetPosition(PlacementInformation info)
        {
            base.SetPosition(info);
            info.Item.Properties[FrameworkElement.MarginProperty].Reset();

            UIElement child = info.Item.View;
            Rect newPosition = info.Bounds;

            if (IsPropertySet(child, Canvas.LeftProperty) || !IsPropertySet(child, Canvas.RightProperty))
            {
                if (newPosition.Left != GetCanvasProperty(child, Canvas.LeftProperty))
                {
                    info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty).SetValue(newPosition.Left);
                }
            }
            else
            {
                double newR = extendedComponent.ActualWidth - newPosition.Right;
                if (newR != GetCanvasProperty(child, Canvas.RightProperty))
                    info.Item.Properties.GetAttachedProperty(Canvas.RightProperty).SetValue(newR);
            }


            if (IsPropertySet(child, Canvas.TopProperty) || !IsPropertySet(child, Canvas.BottomProperty))
            {
                if (newPosition.Top != GetCanvasProperty(child, Canvas.TopProperty))
                {
                    info.Item.Properties.GetAttachedProperty(Canvas.TopProperty).SetValue(newPosition.Top);
                }
            }
            else
            {
                double newB = extendedComponent.ActualHeight - newPosition.Bottom;
                if (newB != GetCanvasProperty(child, Canvas.BottomProperty))
                    info.Item.Properties.GetAttachedProperty(Canvas.BottomProperty).SetValue(newB);
            }

            if (info.Item == Services.Selection.PrimarySelection)
            {
                Rect rect = new(0, 0, extendedView.ActualWidth, extendedView.ActualHeight);
                // 仅针对主选择项
                if (grayOut != null)
                {
                    grayOut.AnimateActiveAreaRectTo(rect);
                }
                else
                {
                    GrayOutDesignerExceptActiveArea.Start(ref grayOut, this.Services, this.ExtendedItem.View, rect);
                }
            }
        }

        public override void LeaveContainer(PlacementOperation operation)
        {
            GrayOutDesignerExceptActiveArea.Stop(ref grayOut);
            base.LeaveContainer(operation);
            foreach (PlacementInformation info in operation.PlacedItems)
            {
                info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty).Reset();
                info.Item.Properties.GetAttachedProperty(Canvas.TopProperty).Reset();
            }
        }

        public override void EnterContainer(PlacementOperation operation)
        {
            base.EnterContainer(operation);
            foreach (PlacementInformation info in operation.PlacedItems)
            {
                info.Item.Properties[FrameworkElement.HorizontalAlignmentProperty].Reset();
                info.Item.Properties[FrameworkElement.VerticalAlignmentProperty].Reset();
                info.Item.Properties[FrameworkElement.MarginProperty].Reset();

                if (operation.Type == PlacementType.PasteItem)
                {
                    if (!double.IsNaN((double)info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty).ValueOnInstance))
                    {
                        info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty)
                            .SetValue(((double)info.Item.Properties.GetAttachedProperty(Canvas.LeftProperty).ValueOnInstance) +
                                      PlacementOperation.PasteOffset);
                    }

                    if (!double.IsNaN((double)info.Item.Properties.GetAttachedProperty(Canvas.TopProperty).ValueOnInstance))
                    {
                        info.Item.Properties.GetAttachedProperty(Canvas.TopProperty)
                            .SetValue(((double)info.Item.Properties.GetAttachedProperty(Canvas.TopProperty).ValueOnInstance) +
                                      PlacementOperation.PasteOffset);
                    }
                }
            }
        }

        public override void EndPlacement(PlacementOperation operation)
        {
            GrayOutDesignerExceptActiveArea.Stop(ref grayOut);
            base.EndPlacement(operation);
        }
    }
}
