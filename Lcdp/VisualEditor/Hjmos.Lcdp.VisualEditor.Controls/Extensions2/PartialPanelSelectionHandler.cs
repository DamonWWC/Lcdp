using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using Hjmos.Lcdp.VisualEditor.Controls.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    public class PartialPanelSelectionHandler : BehaviorExtension, IHandlePointerToolMouseDown
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.ExtendedItem.AddBehavior(typeof(IHandlePointerToolMouseDown), this);
        }

        #region IHandlePointerToolMouseDown

        public void HandleSelectionMouseDown(IDesignPanel designPanel, MouseButtonEventArgs e, DesignPanelHitTestResult result)
        {
            if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left))
            {
                e.Handled = true;
                new PartialRangeSelectionGesture(result.ModelHit).Start(designPanel, e);
            }
        }

        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    internal class PartialRangeSelectionGesture : RangeSelectionGesture
    {
        public PartialRangeSelectionGesture(DesignItem container)
            : base(container)
        {
        }

        protected override ICollection<DesignItem> GetChildDesignItemsInContainer(Geometry geometry)
        {
            HashSet<DesignItem> resultItems = new HashSet<DesignItem>();
            ViewService viewService = container.Services.View;

            HitTestFilterCallback filterCallback = delegate (DependencyObject potentialHitTestTarget)
            {
                FrameworkElement element = potentialHitTestTarget as FrameworkElement;
                if (element != null)
                {
                    // ensure we are able to select elements with width/height=0
                    if (element.ActualWidth == 0 || element.ActualHeight == 0)
                    {
                        DependencyObject tmp = element;
                        DesignItem model = null;
                        while (tmp != null)
                        {
                            model = viewService.GetModel(tmp);
                            if (model != null) break;
                            tmp = VisualTreeHelper.GetParent(tmp);
                        }
                        if (model != container)
                        {
                            resultItems.Add(model);
                            return HitTestFilterBehavior.ContinueSkipChildren;
                        }
                    }
                }
                return HitTestFilterBehavior.Continue;
            };

            HitTestResultCallback resultCallback = delegate (HitTestResult result)
            {
                if (((GeometryHitTestResult)result).IntersectionDetail == IntersectionDetail.FullyInside || (Mouse.RightButton == MouseButtonState.Pressed && ((GeometryHitTestResult)result).IntersectionDetail == IntersectionDetail.Intersects))
                {
                    // find the model for the visual contained in the selection area
                    DependencyObject tmp = result.VisualHit;
                    DesignItem model = null;
                    while (tmp != null)
                    {
                        model = viewService.GetModel(tmp);
                        if (model != null) break;
                        tmp = VisualTreeHelper.GetParent(tmp);
                    }
                    if (model != container)
                    {
                        resultItems.Add(model);
                    }
                }
                return HitTestResultBehavior.Continue;
            };

            VisualTreeHelper.HitTest(container.View, filterCallback, resultCallback, new GeometryHitTestParameters(geometry));
            return resultItems;
        }
    }
}
