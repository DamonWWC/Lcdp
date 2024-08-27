using Hjmos.Lcdp.VisualEditor.Controls.Units;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    public class ZoomBox : Control
    {
        // 鸟瞰图画布内的拖动控件
        private Thumb ZoomThumb { get; set; }

        // 鸟瞰图画布
        private Canvas ZoomCanvas { get; set; }

        // 保存RootCanvas缩放比例
        private ScaleTransform ScaleTransform { get; set; }

        // 主界面内容视区大小
        private Size ViewPortSize { get; set; }

        // 鸟瞰图滑块
        public Slider ZoomSlider
        {
            get => (Slider)GetValue(ZoomSliderProperty);
            set => SetValue(ZoomSliderProperty, value);
        }

        public static readonly DependencyProperty ZoomSliderProperty = DependencyProperty.Register("ZoomSlider", typeof(Slider), typeof(ZoomBox));

        // 主界面滚动区域
        public ScrollViewer ScrollViewer
        {
            get => (ScrollViewer)GetValue(ScrollViewerProperty);
            set => SetValue(ScrollViewerProperty, value);
        }

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(ZoomBox));

        // 主界面内的画布
        public RootCanvas RootCanvas
        {
            get => (RootCanvas)GetValue(RootCanvasProperty);
            set => SetValue(RootCanvasProperty, value);
        }

        public static readonly DependencyProperty RootCanvasProperty = DependencyProperty.Register("RootCanvas", typeof(RootCanvas), typeof(ZoomBox),
            new FrameworkPropertyMetadata(default, (d, e) =>
            {
                ZoomBox zoomBox = (ZoomBox)d;
                if (zoomBox.RootCanvas != null)
                {
                    // 添加布局改变事件
                    zoomBox.RootCanvas.LayoutUpdated += new EventHandler(zoomBox.RootCanvas_LayoutUpdated);
                    // 设置RootCanvas缩放比例
                    zoomBox.RootCanvas.LayoutTransform = zoomBox.ScaleTransform;
                }
            }));

        /// <summary>
        /// RootCanvas布局改变时触发
        /// TODO：调试的时候这个方法会一直循环，到时研究一下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void RootCanvas_LayoutUpdated(object sender, EventArgs e)
        {
            // TODO：暂时注释
            //InvalidateScale(out double scale, out double xOffset, out double yOffset);

            //ZoomThumb.Width = ViewPortSize.Width * scale;
            //ZoomThumb.Height = ViewPortSize.Height * scale;
            //Canvas.SetLeft(ZoomThumb, xOffset + ScrollViewer.HorizontalOffset * scale);
            //Canvas.SetTop(ZoomThumb, yOffset + ScrollViewer.VerticalOffset * scale);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ScrollViewer == null) return;

            ZoomThumb = Template.FindName("PART_ZoomThumb", this) as Thumb;
            if (ZoomThumb == null) throw new Exception("PART_ZoomThumb template is missing!");

            ZoomCanvas = Template.FindName("PART_ZoomCanvas", this) as Canvas;
            if (ZoomCanvas == null) throw new Exception("PART_ZoomCanvas template is missing!");

            ZoomSlider = Template.FindName("PART_ZoomSlider", this) as Slider;
            if (ZoomSlider == null) throw new Exception("PART_ZoomSlider template is missing!");

            ZoomThumb.DragDelta += Thumb_DragDelta;
            ZoomSlider.ValueChanged += ZoomSlider_ValueChanged;
            ScaleTransform = new ScaleTransform();
        }

        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double scale = e.NewValue / e.OldValue;
            double halfViewportHeight = ViewPortSize.Height / 2;
            double newVerticalOffset = (ScrollViewer.VerticalOffset + halfViewportHeight) * scale - halfViewportHeight;
            double halfViewportWidth = ViewPortSize.Width / 2;
            double newHorizontalOffset = (ScrollViewer.HorizontalOffset + halfViewportWidth) * scale - halfViewportWidth;
            ScaleTransform.ScaleX *= scale;
            ScaleTransform.ScaleY *= scale;
            ScrollViewer.ScrollToHorizontalOffset(newHorizontalOffset);
            ScrollViewer.ScrollToVerticalOffset(newVerticalOffset);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            InvalidateScale(out double scale, out _, out _);
            ScrollViewer.ScrollToHorizontalOffset(ScrollViewer.HorizontalOffset + e.HorizontalChange / scale);
            ScrollViewer.ScrollToVerticalOffset(ScrollViewer.VerticalOffset + e.VerticalChange / scale);
        }

        private void InvalidateScale(out double scale, out double xOffset, out double yOffset)
        {
            // 缩放后RootCanvas的大小
            double w = RootCanvas.ActualWidth * ScaleTransform.ScaleX;
            double h = RootCanvas.ActualHeight * ScaleTransform.ScaleY;

            // 鸟瞰图画布大小
            double x = ZoomCanvas.ActualWidth;
            double y = ZoomCanvas.ActualHeight;
            double scaleX = x / w;
            double scaleY = y / h;
            scale = (scaleX < scaleY) ? scaleX : scaleY;
            xOffset = (x - scale * w) / 2;
            yOffset = (y - scale * h) / 2;

            // 主界面内容视区宽高（不超过缩放后RootCanvas宽高）
            double viewportWidth = ScrollViewer.ViewportWidth > w ? w : ScrollViewer.ViewportWidth;
            double viewportHeight = ScrollViewer.ViewportHeight > h ? h : ScrollViewer.ViewportHeight;

            ViewPortSize = new Size(viewportWidth, viewportHeight);
        }
    }
}
