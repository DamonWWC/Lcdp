using Hjmos.Lcdp.Helpers;
using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Core.Controls
{
    public class ZoomButtons : RangeBase
    {
        static ZoomButtons() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomButtons), new FrameworkPropertyMetadata(typeof(ZoomButtons)));

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ButtonBase uxPlus = (ButtonBase)Template.FindName("uxPlus", this);
            ButtonBase uxMinus = (ButtonBase)Template.FindName("uxMinus", this);
            ButtonBase uxReset = (ButtonBase)Template.FindName("uxReset", this);
            ButtonBase ux100Percent = (ButtonBase)Template.FindName("ux100Percent", this);

            if (uxPlus != null)
                uxPlus.Click += OnZoomInClick;
            if (uxMinus != null)
                uxMinus.Click += OnZoomOutClick;
            if (uxReset != null)
                uxReset.Click += OnResetClick;
            if (ux100Percent != null)
                ux100Percent.Click += On100PercentClick;
        }

        private const double ZoomFactor = 1.1;

        private void OnZoomInClick(object sender, EventArgs e) => SetCurrentValue(ValueProperty, ZoomScrollViewer.RoundToOneIfClose(this.Value * ZoomFactor));

        private void OnZoomOutClick(object sender, EventArgs e) => SetCurrentValue(ValueProperty, ZoomScrollViewer.RoundToOneIfClose(this.Value / ZoomFactor));

        private void OnResetClick(object sender, EventArgs e) => SetCurrentValue(ValueProperty, 1.0);

        private void On100PercentClick(object sender, EventArgs e)
        {
            ZoomControl zctl = this.TryFindParent<ZoomControl>();
            double contentWidth = ((FrameworkElement)zctl.Content).ActualWidth;
            double contentHeight = ((FrameworkElement)zctl.Content).ActualHeight;
            double width = zctl.ActualWidth;
            double height = zctl.ActualHeight;

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
