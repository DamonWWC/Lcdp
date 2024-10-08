﻿using Hjmos.Lcdp.VisualEditor.Controls.UIExtensions;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    public class ZoomButtons : RangeBase
    {
        static ZoomButtons()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomButtons),
                                                     new FrameworkPropertyMetadata(typeof(ZoomButtons)));
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var uxPlus = (ButtonBase)Template.FindName("uxPlus", this);
            var uxMinus = (ButtonBase)Template.FindName("uxMinus", this);
            var uxReset = (ButtonBase)Template.FindName("uxReset", this);
            var ux100Percent = (ButtonBase)Template.FindName("ux100Percent", this);

            if (uxPlus != null)
                uxPlus.Click += OnZoomInClick;
            if (uxMinus != null)
                uxMinus.Click += OnZoomOutClick;
            if (uxReset != null)
                uxReset.Click += OnResetClick;
            if (ux100Percent != null)
                ux100Percent.Click += On100PercentClick;
        }

        const double ZoomFactor = 1.1;

        void OnZoomInClick(object sender, EventArgs e)
        {
            SetCurrentValue(ValueProperty, ZoomScrollViewer.RoundToOneIfClose(this.Value * ZoomFactor));
        }

        void OnZoomOutClick(object sender, EventArgs e)
        {
            SetCurrentValue(ValueProperty, ZoomScrollViewer.RoundToOneIfClose(this.Value / ZoomFactor));
        }

        void OnResetClick(object sender, EventArgs e)
        {
            SetCurrentValue(ValueProperty, 1.0);
        }
        void On100PercentClick(object sender, EventArgs e)
        {
            var zctl = this.TryFindParent<ZoomControl>();
            var contentWidth = ((FrameworkElement)zctl.Content).ActualWidth;
            var contentHeight = ((FrameworkElement)zctl.Content).ActualHeight;
            var width = zctl.ActualWidth;
            var height = zctl.ActualHeight;

            if (contentWidth > width || contentHeight > height)
            {
                double widthProportion = contentWidth / width;
                double heightProportion = contentHeight / height;

                if (widthProportion > heightProportion)
                    SetCurrentValue(ValueProperty, (width - 20.00) / contentWidth);
                else
                    SetCurrentValue(ValueProperty, (height - 20.00) / contentHeight);
            }
        }
    }
}
