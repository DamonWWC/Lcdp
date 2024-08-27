using System;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.Adorners
{
    /// <summary>
    /// 管理装饰器在设计界面上的显示。
    /// 所有装饰器都放置在AdornerPanel中以支持大小和布局。
    /// AdornerPanel提供设置装饰器相对于它所装饰的控件的大小调整和重新定位的行为
    /// </summary>
    public sealed class AdornerPanel : Panel
    {
        #region Attached Property Placement

        /// <summary>
        /// 用于存储装饰器视觉效果位置的依赖项属性。
        /// </summary>
        public static readonly DependencyProperty PlacementProperty = DependencyProperty.RegisterAttached(
            "Placement", typeof(AdornerPlacement), typeof(AdornerPanel),
            new FrameworkPropertyMetadata(AdornerPlacement.FillContent, FrameworkPropertyMetadataOptions.AffectsParentMeasure)
        );

        /// <summary>
        /// 获取指定装饰器的位置。
        /// </summary>
        public static AdornerPlacement GetPlacement(UIElement adorner) => adorner == null ? throw new ArgumentNullException("adorner") : (AdornerPlacement)adorner.GetValue(PlacementProperty);

        /// <summary>
        /// 设置指定装饰器的位置。
        /// </summary>
        public static void SetPlacement(UIElement adorner, AdornerPlacement placement)
        {
            if (adorner == null) throw new ArgumentNullException("adorner");
            if (placement == null) throw new ArgumentNullException("placement");

            adorner.SetValue(PlacementProperty, placement);
        }

        #endregion


        /// <summary>
        /// 将绝对矢量转换为与<see cref="AdornerPanel" />所装饰的元素相关的矢量
        /// </summary>
        public Vector AbsoluteToRelative(Vector absolute)
        {
            return new Vector(absolute.X / ((FrameworkElement)this.AdornedElement).ActualWidth, absolute.Y / ((FrameworkElement)this.AdornedElement).ActualHeight);
        }

        /// <summary>
        /// 将与<see cref="AdornerPanel" />装饰的元素相关的向量转换为绝对向量。
        /// </summary>
        public Vector RelativeToAbsolute(Vector relative)
        {
            return new Vector(relative.X * ((FrameworkElement)this.AdornedElement).ActualWidth, relative.Y * ((FrameworkElement)this.AdornedElement).ActualHeight);
        }

        /// <summary>
        /// 获取由这个AdornerPanel装饰的元素。  
        /// </summary>
        public UIElement AdornedElement { get; private set; }

        /// <summary>
        /// 获取由此AdornerPanel装饰的设计项。  
        /// </summary>
        public DesignItem AdornedDesignItem { get; private set; }

        /// <summary>
        /// 设置AdornedElement和AdornedDesignItem属性。
        /// 此方法只能调用一次。
        /// </summary>
        public void SetAdornedElement(UIElement adornedElement, DesignItem adornedDesignItem)
        {
            if (adornedElement == null) throw new ArgumentNullException("adornedElement");

            // 当没有任何改变时忽略调用
            if (AdornedElement == adornedElement && AdornedDesignItem == adornedDesignItem) return;

            if (AdornedElement != null) throw new InvalidOperationException("已经设置了AdornedElement");

            AdornedElement = adornedElement;
            AdornedDesignItem = adornedDesignItem;
        }

        /// <summary>
        /// 获取/设置用于显示AdornerPanel相对于其他AdornerPanel的顺序。
        /// 不要在面板被添加到装饰层后更改此属性!
        /// </summary>
        public AdornerOrder Order { get; set; } = AdornerOrder.Content;

        protected override Size MeasureOverride(Size availableSize)
        {
            if (this.AdornedElement != null)
            {
                foreach (DependencyObject v in InternalChildren)
                {
                    if (v is UIElement e)
                    {
                        e.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                    }
                }

                return PlacementOperation.GetRealElementSize(this.AdornedElement);
            }
            else
            {
                return base.MeasureOverride(availableSize);
            }
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement element in base.InternalChildren)
            {
                GetPlacement(element).Arrange(this, element, finalSize);
            }
            return finalSize;
        }
    }
}
