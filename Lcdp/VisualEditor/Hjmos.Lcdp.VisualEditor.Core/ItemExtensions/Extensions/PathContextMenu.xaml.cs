using System.Windows.Media;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public partial class PathContextMenu
    {
        private DesignItem designItem;

        public PathContextMenu(DesignItem designItem)
        {
            this.designItem = designItem;

            InitializeComponent();
        }

        private void Click_ConvertToFigures(object sender, System.Windows.RoutedEventArgs e)
        {
            Path path = this.designItem.Component as Path;

            if (path.Data is StreamGeometry)
            {
                StreamGeometry sg = path.Data as StreamGeometry;

                PathGeometry pg = sg.GetFlattenedPathGeometry();

                DesignItem pgDes = designItem.Services.Component.RegisterComponentForDesigner(pg);
                designItem.Properties[Path.DataProperty].SetValue(pgDes);
            }
            else if (path.Data is PathGeometry)
            {
                PathGeometry pg = path.Data as PathGeometry;

                PathFigureCollection figs = pg.Figures;

                PathGeometry newPg = new PathGeometry();
                DesignItem newPgDes = designItem.Services.Component.RegisterComponentForDesigner(newPg);

                foreach (PathFigure fig in figs)
                {
                    newPgDes.Properties[PathGeometry.FiguresProperty].CollectionElements.Add(FigureToDesignItem(fig));
                }

                designItem.Properties[Path.DataProperty].SetValue(newPg);
            }

        }

        private DesignItem FigureToDesignItem(PathFigure pf)
        {
            DesignItem pfDes = designItem.Services.Component.RegisterComponentForDesigner(new PathFigure());

            pfDes.Properties[PathFigure.StartPointProperty].SetValue(pf.StartPoint);
            pfDes.Properties[PathFigure.IsClosedProperty].SetValue(pf.IsClosed);

            foreach (PathSegment s in pf.Segments)
            {
                pfDes.Properties[PathFigure.SegmentsProperty].CollectionElements.Add(SegmentToDesignItem(s));
            }
            return pfDes;
        }

        private DesignItem SegmentToDesignItem(PathSegment s)
        {
            DesignItem sDes = designItem.Services.Component.RegisterComponentForDesigner(s.Clone());

            if (!s.IsStroked)
                sDes.Properties[PathSegment.IsStrokedProperty].SetValue(s.IsStroked);
            if (s.IsSmoothJoin)
                sDes.Properties[PathSegment.IsSmoothJoinProperty].SetValue(s.IsSmoothJoin);

            if (s is LineSegment lineSegment)
            {
                sDes.Properties[LineSegment.PointProperty].SetValue(lineSegment.Point);
            }
            else if (s is QuadraticBezierSegment qbSegment)
            {
                sDes.Properties[QuadraticBezierSegment.Point1Property].SetValue(qbSegment.Point1);
                sDes.Properties[QuadraticBezierSegment.Point2Property].SetValue(qbSegment.Point2);
            }
            else if (s is BezierSegment bSegment)
            {
                sDes.Properties[BezierSegment.Point1Property].SetValue(bSegment.Point1);
                sDes.Properties[BezierSegment.Point2Property].SetValue(bSegment.Point2);
                sDes.Properties[BezierSegment.Point3Property].SetValue(bSegment.Point3);
            }
            else if (s is ArcSegment arcSegment)
            {
                sDes.Properties[ArcSegment.PointProperty].SetValue(arcSegment.Point);
                sDes.Properties[ArcSegment.IsLargeArcProperty].SetValue(arcSegment.IsLargeArc);
                sDes.Properties[ArcSegment.RotationAngleProperty].SetValue(arcSegment.RotationAngle);
                sDes.Properties[ArcSegment.SizeProperty].SetValue(arcSegment.Size);
                sDes.Properties[ArcSegment.SweepDirectionProperty].SetValue(arcSegment.SweepDirection);
            }
            else if (s is PolyLineSegment polyLineSegment)
            {
                sDes.Properties[PolyLineSegment.PointsProperty].SetValue(polyLineSegment.Points);
            }
            else if (s is PolyQuadraticBezierSegment pqbSegment)
            {
                sDes.Properties[PolyQuadraticBezierSegment.PointsProperty].SetValue(pqbSegment.Points);
            }
            else if (s is PolyBezierSegment pbSegment)
            {
                sDes.Properties[PolyBezierSegment.PointsProperty].SetValue(pbSegment.Points);
            }
            return sDes;
        }
    }
}

