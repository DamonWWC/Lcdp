using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.Controls;
using Hjmos.Lcdp.VisualEditor.Core.DesignerControls;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    // 范围选择手势
    internal class RangeSelectionGesture : ClickOrDragMouseGesture
    {
        protected DesignItem _container;
        protected AdornerPanel _adornerPanel;
        protected SelectionFrame _selectionFrame;

        protected GrayOutDesignerExceptActiveArea grayOut;

        public RangeSelectionGesture(DesignItem container)
        {
            _container = container;
            _positionRelativeTo = container.View;
        }

        // 拖拽时显示矩形选择框。
        protected override void OnDragStarted(MouseEventArgs e)
        {
            _adornerPanel = new AdornerPanel();
            _adornerPanel.SetAdornedElement(_container.View, _container);

            _selectionFrame = new SelectionFrame();
            _adornerPanel.Children.Add(_selectionFrame);

            designPanel.Adorners.Add(_adornerPanel);

            GrayOutDesignerExceptActiveArea.Start(ref grayOut, services, _container.View);
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(sender, e);
            if (_hasDragStarted)
            {
                SetPlacement(e.GetPosition(_positionRelativeTo));
            }
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (_hasDragStarted == false)
            {
                services.Selection.SetSelectedComponents(new DesignItem[] { _container }, SelectionTypes.Auto);
            }
            else
            {
                Point endPoint = e.GetPosition(_positionRelativeTo);
                Rect frameRect = new(
                    Math.Min(_startPoint.X, endPoint.X),
                    Math.Min(_startPoint.Y, endPoint.Y),
                    Math.Abs(_startPoint.X - endPoint.X),
                    Math.Abs(_startPoint.Y - endPoint.Y)
                );

                ICollection<DesignItem> items = GetChildDesignItemsInContainer(new RectangleGeometry(frameRect));
                if (items.Count == 0)
                {
                    items.Add(_container);
                }
                services.Selection.SetSelectedComponents(items, SelectionTypes.Auto);
            }
            Stop();
        }

        protected virtual ICollection<DesignItem> GetChildDesignItemsInContainer(Geometry geometry)
        {
            HashSet<DesignItem> resultItems = new HashSet<DesignItem>();
            ViewService viewService = _container.Services.View;

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
                        if (model != _container)
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
                if (((GeometryHitTestResult)result).IntersectionDetail == IntersectionDetail.FullyInside)
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
                    if (model != _container)
                    {
                        resultItems.Add(model);
                    }
                }
                return HitTestResultBehavior.Continue;
            };

            VisualTreeHelper.HitTest(_container.View, filterCallback, resultCallback, new GeometryHitTestParameters(geometry));
            return resultItems;
        }

        private void SetPlacement(Point endPoint)
        {
            RelativePlacement p = new()
            {
                XOffset = Math.Min(_startPoint.X, endPoint.X),
                YOffset = Math.Min(_startPoint.Y, endPoint.Y)
            };
            p.WidthOffset = Math.Max(_startPoint.X, endPoint.X) - p.XOffset;
            p.HeightOffset = Math.Max(_startPoint.Y, endPoint.Y) - p.YOffset;
            AdornerPanel.SetPlacement(_selectionFrame, p);
        }

        protected override void OnStopped()
        {
            if (_adornerPanel != null)
            {
                designPanel.Adorners.Remove(_adornerPanel);
                _adornerPanel = null;
            }
            GrayOutDesignerExceptActiveArea.Stop(ref grayOut);
            _selectionFrame = null;
            base.OnStopped();
        }
    }
}
