using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using System.Windows;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public class PointTrackerPlacementSupport : AdornerPlacement
    {
        private readonly Shape _shape;
        private readonly PlacementAlignment _alignment;

        public int Index { get; set; }

        public PointTrackerPlacementSupport(Shape s, PlacementAlignment align, int index)
        {
            _shape = s;
            _alignment = align;
            Index = index;
        }

        /// <summary>
        /// 在指定的装饰器面板上排列装饰器元素。
        /// </summary>
        public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
        {
            Point p = new(0, 0);
            if (_shape is Line)
            {
                var s = _shape as Line;
                double x, y;

                if (_alignment == PlacementAlignment.BottomRight)
                {
                    x = s.X2;
                    y = s.Y2;
                }
                else
                {
                    x = s.X1;
                    y = s.Y1;
                }
                p = new Point(x, y);
            }
            else if (_shape is Polygon)
            {
                var pg = _shape as Polygon;
                p = pg.Points[Index];
            }
            else if (_shape is Polyline)
            {
                var pg = _shape as Polyline;
                p = pg.Points[Index];
            }

            var transform = _shape.RenderedGeometry.Transform;
            p = transform.Transform(p);

            adorner.Arrange(new Rect(p.X - 3.5, p.Y - 3.5, 7, 7));
        }
    }
}
