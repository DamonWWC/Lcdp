using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 在选定的UI元素周围绘制虚线。
    /// </summary>
    [ExtensionFor(typeof(UIElement))]
    public class SelectedElementRectangleExtension : SelectionAdornerProvider
    {
        public SelectedElementRectangleExtension()
        {
            Rectangle selectionRect = new()
            {
                SnapsToDevicePixels = true,
                Stroke = new SolidColorBrush(Color.FromRgb(0x47, 0x47, 0x47)),
                StrokeThickness = 1.5,
                IsHitTestVisible = false
            };

            RelativePlacement placement = new(HorizontalAlignment.Stretch, VerticalAlignment.Stretch)
            {
                XOffset = -1,
                YOffset = -1,
                WidthOffset = 2,
                HeightOffset = 2
            };

            this.AddAdorners(placement, selectionRect);
        }
    }
}
