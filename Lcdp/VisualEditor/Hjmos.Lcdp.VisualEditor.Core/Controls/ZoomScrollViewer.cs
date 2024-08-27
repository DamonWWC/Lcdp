using Hjmos.Lcdp.VisualEditor.Core.UIExtensions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.Controls
{
    /// <summary>
    /// 控制ScrollView内容缩放的类
    /// </summary>
    public class ZoomScrollViewer : ScrollViewer
    {
        static ZoomScrollViewer() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomScrollViewer), new FrameworkPropertyMetadata(typeof(ZoomScrollViewer)));

        public bool EnableHorizontalWheelSupport
        {
            get => (bool)GetValue(EnableHorizontalWheelSupportProperty);
            set => SetValue(EnableHorizontalWheelSupportProperty, value);
        }

        public static readonly DependencyProperty EnableHorizontalWheelSupportProperty =
            DependencyProperty.Register("EnableHorizontalWheelSupport", typeof(bool), typeof(ZoomScrollViewer), new PropertyMetadata(false, EnableHorizontalWheelSupportChanged));


        private static void EnableHorizontalWheelSupportChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ZoomScrollViewer ctl = d as ZoomScrollViewer;
            if ((bool)e.NewValue == false)
            {
                MouseHorizontalWheelEnabler.RemoveMouseHorizontalWheelHandler(ctl, ctl.OnMouseHorizontalWheel);
            }
            else
            {
                MouseHorizontalWheelEnabler.AddMouseHorizontalWheelHandler(ctl, ctl.OnMouseHorizontalWheel);
            }
        }

        public static readonly DependencyProperty CurrentZoomProperty =
            DependencyProperty.Register("CurrentZoom", typeof(double), typeof(ZoomScrollViewer),
                                        new FrameworkPropertyMetadata(1.0, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, CalculateZoomButtonCollapsed, CoerceZoom));

        public double CurrentZoom
        {
            get => (double)GetValue(CurrentZoomProperty);
            set => SetValue(CurrentZoomProperty, value);
        }

        private static object CoerceZoom(DependencyObject d, object baseValue)
        {
            double zoom = (double)baseValue;
            ZoomScrollViewer sv = (ZoomScrollViewer)d;
            return Math.Max(sv.MinimumZoom, Math.Min(sv.MaximumZoom, zoom));
        }

        public static readonly DependencyProperty MinimumZoomProperty =
            DependencyProperty.Register("MinimumZoom", typeof(double), typeof(ZoomScrollViewer), new FrameworkPropertyMetadata(0.2));

        public double MinimumZoom
        {
            get => (double)GetValue(MinimumZoomProperty);
            set => SetValue(MinimumZoomProperty, value);
        }

        public static readonly DependencyProperty MaximumZoomProperty =
            DependencyProperty.Register("MaximumZoom", typeof(double), typeof(ZoomScrollViewer), new FrameworkPropertyMetadata(5.0));

        public double MaximumZoom
        {
            get => (double)GetValue(MaximumZoomProperty);
            set => SetValue(MaximumZoomProperty, value);
        }

        public static readonly DependencyProperty MouseWheelZoomProperty =
            DependencyProperty.Register("MouseWheelZoom", typeof(bool), typeof(ZoomScrollViewer), new FrameworkPropertyMetadata(SharedInstances.BoxedTrue));

        public bool MouseWheelZoom
        {
            get => (bool)GetValue(MouseWheelZoomProperty);
            set => SetValue(MouseWheelZoomProperty, value);
        }

        public static readonly DependencyProperty AlwaysShowZoomButtonsProperty =
            DependencyProperty.Register("AlwaysShowZoomButtons", typeof(bool), typeof(ZoomScrollViewer),
                                        new FrameworkPropertyMetadata(SharedInstances.BoxedFalse, CalculateZoomButtonCollapsed));

        public bool AlwaysShowZoomButtons
        {
            get => (bool)GetValue(AlwaysShowZoomButtonsProperty);
            set => SetValue(AlwaysShowZoomButtonsProperty, value);
        }

        private static readonly DependencyPropertyKey ComputedZoomButtonCollapsedPropertyKey =
            DependencyProperty.RegisterReadOnly("ComputedZoomButtonCollapsed", typeof(bool), typeof(ZoomScrollViewer),
                                                new FrameworkPropertyMetadata(SharedInstances.BoxedTrue));

        public static readonly DependencyProperty ComputedZoomButtonCollapsedProperty = ComputedZoomButtonCollapsedPropertyKey.DependencyProperty;

        public bool ComputedZoomButtonCollapsed
        {
            get => (bool)GetValue(ComputedZoomButtonCollapsedProperty);
            private set => SetValue(ComputedZoomButtonCollapsedPropertyKey, value);
        }

        private static void CalculateZoomButtonCollapsed(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ZoomScrollViewer z)
                z.ComputedZoomButtonCollapsed = (z.AlwaysShowZoomButtons == false) && (z.CurrentZoom == 1.0);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            if (!e.Handled && Keyboard.Modifiers == ModifierKeys.Control && MouseWheelZoom)
            {
                double oldZoom = CurrentZoom;
                double newZoom = RoundToOneIfClose(CurrentZoom * Math.Pow(1.001, e.Delta));
                newZoom = Math.Max(this.MinimumZoom, Math.Min(this.MaximumZoom, newZoom));

                // 调整滚动位置，使鼠标停留在相同的可视坐标上
                Vector relMousePos;
                if (Template.FindName("PART_Presenter", this) is ContentPresenter presenter)
                {
                    Point mousePos = e.GetPosition(presenter);
                    relMousePos = new Vector(mousePos.X / presenter.ActualWidth, mousePos.Y / presenter.ActualHeight);
                }
                else
                {
                    relMousePos = new Vector(0.5, 0.5);
                }

                Point scrollOffset = new Point(this.HorizontalOffset, this.VerticalOffset);
                Vector oldHalfViewport = new Vector(this.ViewportWidth / 2, this.ViewportHeight / 2);
                Vector newHalfViewport = oldHalfViewport / newZoom * oldZoom;
                Point oldCenter = scrollOffset + oldHalfViewport;
                Point virtualMousePos = scrollOffset + new Vector(relMousePos.X * this.ViewportWidth, relMousePos.Y * this.ViewportHeight);

                // As newCenter, we want to choose a point between oldCenter and virtualMousePos. The more we zoom in, the closer
                // to virtualMousePos. We'll create the line x = oldCenter + lambda * (virtualMousePos-oldCenter).
                // On this line, we need to choose lambda between -1 and 1:
                // -1 = zoomed out completely
                //  0 = zoom unchanged
                // +1 = zoomed in completely
                // But the zoom factor (newZoom/oldZoom) we have is in the range [0,+Infinity].

                // Basically, I just played around until I found a function that maps this to [-1,1] and works well.
                // "f" is squared because otherwise the mouse simply stays over virtualMousePos, but I wanted virtualMousePos
                // to move towards the middle -> squaring f causes lambda to be closer to 1, giving virtualMousePos more weight
                // then oldCenter.

                double f = Math.Min(newZoom, oldZoom) / Math.Max(newZoom, oldZoom);
                double lambda = 1 - f * f;
                if (oldZoom > newZoom)
                    lambda = -lambda;

                Point newCenter = oldCenter + lambda * (virtualMousePos - oldCenter);
                scrollOffset = newCenter - newHalfViewport;

                SetCurrentValue(CurrentZoomProperty, newZoom);

                this.ScrollToHorizontalOffset(scrollOffset.X);
                this.ScrollToVerticalOffset(scrollOffset.Y);

                e.Handled = true;
            }
            base.OnMouseWheel(e);
        }

        private void OnMouseHorizontalWheel(object d, RoutedEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control)
            {
                MouseHorizontalWheelEventArgs ea = e as MouseHorizontalWheelEventArgs;

                this.ScrollToHorizontalOffset(this.HorizontalOffset + ea.HorizontalDelta);
            }
        }

        internal static double RoundToOneIfClose(double val)
        {
            return Math.Abs(val - 1.0) < 0.001 ? 1.0 : val;
        }
    }

    internal sealed class IsNormalZoomConverter : IValueConverter
    {
        public static readonly IsNormalZoomConverter Instance = new();

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return parameter is bool b && b ? true : (object)(((double)value) == 1.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) => throw new NotImplementedException();
    }
}
