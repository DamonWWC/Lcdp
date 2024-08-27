using System;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Adorners
{
    // 我们必须支持"http://myfun.spaces.live.com/blog/cns!AC1291870308F748!242.entry"中解释的不同坐标空间

    /// <summary>
    /// 为不同类型的相对摆放提供属性的摆放类。
    /// </summary>
    public sealed class RelativePlacement : AdornerPlacement
    {
        /// <summary>
        /// 创建一个新的RelativePlacement实例。默认实例是一个大小为零的装饰器，必须自己设置一些属性来定义位置。
        /// </summary>
        public RelativePlacement() { }

        /// <summary>
        /// 从指定的水平和垂直对齐方式创建一个新的RelativePlacement实例。
        /// </summary>
        public RelativePlacement(HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            switch (horizontal)
            {
                case HorizontalAlignment.Left:
                    WidthRelativeToDesiredWidth = 1;
                    XRelativeToAdornerWidth = -1;
                    break;
                case HorizontalAlignment.Right:
                    WidthRelativeToDesiredWidth = 1;
                    XRelativeToContentWidth = 1;
                    break;
                case HorizontalAlignment.Center:
                    WidthRelativeToDesiredWidth = 1;
                    XRelativeToContentWidth = 0.5;
                    XRelativeToAdornerWidth = -0.5;
                    break;
                case HorizontalAlignment.Stretch:
                    WidthRelativeToContentWidth = 1;
                    break;
            }
            switch (vertical)
            {
                case VerticalAlignment.Top:
                    HeightRelativeToDesiredHeight = 1;
                    YRelativeToAdornerHeight = -1;
                    break;
                case VerticalAlignment.Bottom:
                    HeightRelativeToDesiredHeight = 1;
                    YRelativeToContentHeight = 1;
                    break;
                case VerticalAlignment.Center:
                    HeightRelativeToDesiredHeight = 1;
                    YRelativeToContentHeight = 0.5;
                    YRelativeToAdornerHeight = -0.5;
                    break;
                case VerticalAlignment.Stretch:
                    HeightRelativeToContentHeight = 1;
                    break;
            }
        }

        /// <summary>
        /// Gets/Sets the width of the adorner as factor relative to the desired adorner width.
        /// 获取/设置装饰器的宽度为相对于所需装饰器宽度的因子
        /// </summary>
        public double WidthRelativeToDesiredWidth { get; set; }

        /// <summary>
        /// Gets/Sets the height of the adorner as factor relative to the desired adorner height.
        /// 获取/设置装饰器的高度作为相对于所需的装饰器高度的因子
        /// </summary>
        public double HeightRelativeToDesiredHeight { get; set; }

        /// <summary>
        /// Gets/Sets the width of the adorner as factor relative to the width of the adorned item.
        /// 获取/设置装饰器的宽度为相对于装饰项的宽度的因子
        /// </summary>
        public double WidthRelativeToContentWidth { get; set; }

        /// <summary>
        /// Gets/Sets the height of the adorner as factor relative to the height of the adorned item.
        /// 获取/设置装饰器的高度作为相对于装饰器的高度的因子
        /// </summary>
        public double HeightRelativeToContentHeight { get; set; }

        /// <summary>
        /// Gets/Sets an offset that is added to the adorner width for the size calculation.
        /// 获取/设置添加到装饰器宽度用于大小计算的偏移量
        /// </summary>
        public double WidthOffset { get; set; }

        /// <summary>
        /// Gets/Sets an offset that is added to the adorner height for the size calculation.
        /// 获取/设置添加到装饰器高度用于大小计算的偏移量
        /// </summary>
        public double HeightOffset { get; set; }

        private Size CalculateSize(UIElement adorner, Size adornedElementSize)
        {
            double width = Math.Max(WidthOffset + WidthRelativeToDesiredWidth * adorner.DesiredSize.Width + WidthRelativeToContentWidth * adornedElementSize.Width, 0);
            double height = Math.Max(HeightOffset + HeightRelativeToDesiredHeight * adorner.DesiredSize.Height + HeightRelativeToContentHeight * adornedElementSize.Height, 0);
            return new Size(width, height);
        }

        /// <summary>
        /// Gets/Sets an offset that is added to the adorner position.
        /// 获取/设置添加到装饰器位置的偏移量
        /// </summary>
        public double XOffset { get; set; }

        /// <summary>
        /// Gets/Sets an offset that is added to the adorner position.
        /// 获取/设置添加到装饰器位置的偏移量。
        /// </summary>
        public double YOffset { get; set; }

        /// <summary>
        /// Gets/Sets the left border of the adorner element as factor relative to the width of the adorner.
        /// 获取/设置装饰器元素的左边框为相对于装饰器宽度的因子。
        /// </summary>
        public double XRelativeToAdornerWidth { get; set; }

        /// <summary>
        /// Gets/Sets the top border of the adorner element as factor relative to the height of the adorner.
        /// 获取/设置装饰器元素的上边框为相对于装饰器高度的因子。
        /// </summary>
        public double YRelativeToAdornerHeight { get; set; }

        /// <summary>
        /// Gets/Sets the left border of the adorner element as factor relative to the width of the adorned content.
        /// 获取/设置装饰器元素的左边框作为与装饰内容宽度相关的因子。
        /// </summary>
        public double XRelativeToContentWidth { get; set; }

        /// <summary>
        /// Gets/Sets the top border of the adorner element as factor relative to the height of the adorned content.
        /// 获取/设置装饰器元素的上边框作为相对于装饰内容的高度的因子。
        /// </summary>
        public double YRelativeToContentHeight { get; set; }

        private Point CalculatePosition(Size adornedElementSize, Size adornerSize)
        {
            double x = XOffset + XRelativeToAdornerWidth * adornerSize.Width + XRelativeToContentWidth * adornedElementSize.Width;
            double y = YOffset + YRelativeToAdornerHeight * adornerSize.Height + YRelativeToContentHeight * adornedElementSize.Height;
            return new Point(x, y);
        }

        /// <summary>
        /// 在指定的装饰面板上排列装饰元素
        /// </summary>
        public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
        {
            Size adornerSize = CalculateSize(adorner, adornedElementSize);
            adorner.Arrange(new Rect(CalculatePosition(adornedElementSize, adornerSize), adornerSize));
        }
    }
}