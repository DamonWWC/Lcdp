using Hjmos.Lcdp.VisualEditor.Core.Units;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.Controls
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
        public ZoomControl ZoomControl
        {
            get => (ZoomControl)GetValue(ZoomControlProperty);
            set => SetValue(ZoomControlProperty, value);
        }

        public static readonly DependencyProperty ZoomControlProperty = DependencyProperty.Register("ZoomControl", typeof(ZoomControl), typeof(ZoomBox));


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
        private void RootCanvas_LayoutUpdated(object sender, EventArgs e)
        {
            InvalidateScale(out double scale, out double xOffset, out double yOffset);
            // ViewPortSize 主界面内容视区宽高（不超过缩放后MainContent宽高）
            ZoomThumb.Width = ViewPortSize.Width * scale;
            ZoomThumb.Height = ViewPortSize.Height * scale;
            // ScrollViewer.ViewportWidth主界面内容视区宽高
            // this.ZoomThumb.Width = ScrollViewer.ViewportWidth * scale;
            // this.ZoomThumb.Height = ScrollViewer.ViewportHeight * scale;

            // 设置橡皮圈位置（误差偏移量+滚动条偏移量*缩放比例）
            Canvas.SetLeft(ZoomThumb, xOffset + ZoomControl.HorizontalOffset * scale);
            Canvas.SetTop(ZoomThumb, yOffset + ZoomControl.VerticalOffset * scale);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (ZoomControl == null) return;

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

        /// <summary>
        /// 滑块值改变时触发
        /// </summary>
        private void ZoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double scale = e.NewValue / e.OldValue;
            double halfViewportHeight = ViewPortSize.Height / 2;
            double newVerticalOffset = (ZoomControl.VerticalOffset + halfViewportHeight) * scale - halfViewportHeight;
            double halfViewportWidth = ViewPortSize.Width / 2;
            double newHorizontalOffset = (ZoomControl.HorizontalOffset + halfViewportWidth) * scale - halfViewportWidth;
            ScaleTransform.ScaleX *= scale;
            ScaleTransform.ScaleY *= scale;
            ZoomControl.ScrollToHorizontalOffset(newHorizontalOffset);
            ZoomControl.ScrollToVerticalOffset(newVerticalOffset);
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            InvalidateScale(out double scale, out _, out _);
            ZoomControl.ScrollToHorizontalOffset(ZoomControl.HorizontalOffset + e.HorizontalChange / scale);
            ZoomControl.ScrollToVerticalOffset(ZoomControl.VerticalOffset + e.VerticalChange / scale);
        }

        /// <summary>
        /// 使缩放失效（重新计算缩放）
        /// </summary>
        /// <param name="scale"></param>
        /// <param name="xOffset"></param>
        /// <param name="yOffset"></param>
        private void InvalidateScale(out double scale, out double xOffset, out double yOffset)
        {
            // RootCanvas的测量宽高
            double w = RootCanvas.DesiredSize.Width;
            double h = RootCanvas.DesiredSize.Height;
            // DesiredSize表示MainContent测量后需要的宽高（内容宽高），ActualWidth表示实际渲染后的宽度
            // 如果想要橡皮圈矩形框比例和PageShell实际渲染后的宽高一样，就用下面这两句
            //double w = RootCanvas.ActualWidth * ScaleTransform.ScaleX;
            //double h = RootCanvas.ActualHeight * ScaleTransform.ScaleY;

            // 鸟瞰图画布实际宽高
            double x = ZoomCanvas.ActualWidth;
            double y = ZoomCanvas.ActualHeight;

            // 鸟瞰图和实际内容宽高的比例
            double scaleX = x / w;
            double scaleY = y / h;

            // 整体缩放比例（取比例值叫小的作为整体缩放比例）
            scale = (scaleX < scaleY) ? scaleX : scaleY;

            // 橡皮圈需要增加的偏移量（ 实际缩放比例和整体缩放比例的误差 / 2 ）
            xOffset = (x - scale * w) / 2;
            yOffset = (y - scale * h) / 2;

            // 主界面内容视区宽高（不超过缩放后RootCanvas宽高）
            double viewportWidth = ZoomControl.ViewportWidth > w ? w : ZoomControl.ViewportWidth;
            double viewportHeight = ZoomControl.ViewportHeight > h ? h : ZoomControl.ViewportHeight;

            ViewPortSize = new Size(viewportWidth, viewportHeight);
        }
    }
}
