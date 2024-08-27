using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using System.Diagnostics;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Thumbs
{
    /// <summary>
    /// 调整尺寸，如果装饰元素太小，则自动消失。
    /// </summary>
    public sealed class ResizeThumb : DesignerThumb
    {

        private readonly bool checkWidth;
        private readonly bool checkHeight;

        static ResizeThumb() => DefaultStyleKeyProperty.OverrideMetadata(typeof(ResizeThumb), new FrameworkPropertyMetadata(typeof(ResizeThumb)));

        public ResizeThumb(bool checkWidth, bool checkHeight)
        {
            Debug.Assert((checkWidth && checkHeight) == false);
            this.checkWidth = checkWidth;
            this.checkHeight = checkHeight;
        }

        protected override Size ArrangeOverride(Size arrangeBounds)
        {
            if (this.Parent is AdornerPanel parent && parent.AdornedElement != null)
            {
                if (checkWidth)
                    this.ThumbVisible = PlacementOperation.GetRealElementSize(parent.AdornedElement).Width > 14;
                else if (checkHeight)
                    this.ThumbVisible = PlacementOperation.GetRealElementSize(parent.AdornedElement).Height > 14;
            }
            return base.ArrangeOverride(arrangeBounds);
        }
    }
}
