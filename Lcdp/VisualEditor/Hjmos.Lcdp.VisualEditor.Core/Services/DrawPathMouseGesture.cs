using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    internal sealed class DrawPathMouseGesture : ClickOrDragMouseGesture
    {
        //private ChangeGroup changeGroup;
        private readonly DesignItem _newLine;
        private Point _point;
        private readonly PathFigure _figure;
        private readonly DesignItem _geometry;
        private Matrix _matrix;

        public DrawPathMouseGesture(PathFigure figure, DesignItem newLine, IInputElement relativeTo, Transform transform)
        {
            _newLine = newLine;
            _positionRelativeTo = relativeTo;
            //this.changeGroup = changeGroup;
            _figure = figure;
            _matrix = transform.Value;
            _matrix.Invert();
            _point = Mouse.GetPosition(null);
            _geometry = newLine.Properties[Path.DataProperty].Value;
        }

        protected override void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnPreviewMouseLeftButtonDown(sender, e);
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            var delta = _matrix.Transform(e.GetPosition(null) - _point);
            var point = new Point(Math.Round(delta.X, 0), Math.Round(delta.Y, 0));

            LineSegment segment = _figure.Segments.LastOrDefault() as LineSegment;
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                if (segment == null || segment.Point != point)
                {
                    _figure.Segments.Add(new LineSegment(point, false));
                    segment = _figure.Segments.Last() as LineSegment;
                }
            }

            segment.Point = point;
            DesignItemProperty prop = _geometry.Properties[PathGeometry.FiguresProperty];
            prop.SetValue(prop.TypeConverter.ConvertToInvariantString(_figure));
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            var delta = _matrix.Transform(e.GetPosition(null) - _point);
            var point = new Point(Math.Round(delta.X, 0), Math.Round(delta.Y, 0));

            _figure.Segments.Add(new LineSegment(point, false));
            var prop = _geometry.Properties[PathGeometry.FiguresProperty];
            prop.SetValue(prop.TypeConverter.ConvertToInvariantString(_figure));
        }

        protected override void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(sender, e);

            _figure.Segments.RemoveAt(_figure.Segments.Count - 1);
            DesignItemProperty prop = _geometry.Properties[PathGeometry.FiguresProperty];
            prop.SetValue(prop.TypeConverter.ConvertToInvariantString(_figure));

            //if (changeGroup != null)
            //{
            //    changeGroup.Commit();
            //    changeGroup = null;
            //}

            Stop();
        }

        protected override void OnStopped()
        {
            //if (changeGroup != null)
            //{
            //    changeGroup.Abort();
            //    changeGroup = null;
            //}
            if (services.Tool.CurrentTool is CreateComponentTool)
            {
                services.Tool.CurrentTool = services.Tool.PointerTool;
            }

            _newLine.ReapplyAllExtensions();

            base.OnStopped();
        }

    }
}
