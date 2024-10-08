﻿using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{

    internal class PartialRangeSelectionGesture : RangeSelectionGesture
    {
        public PartialRangeSelectionGesture(DesignItem container) : base(container)
        {
        }

        protected override ICollection<DesignItem> GetChildDesignItemsInContainer(Geometry geometry)
        {
            HashSet<DesignItem> resultItems = new();
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
    }
}
