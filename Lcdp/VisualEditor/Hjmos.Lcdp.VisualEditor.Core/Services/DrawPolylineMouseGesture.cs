using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    sealed class DrawPolylineMouseGesture : ClickOrDragMouseGesture
    {
        //private ChangeGroup changeGroup;
        private readonly DesignItem _newLine;
        private new Point _startPoint;
        private Point? _lastAdded;
        private Matrix _matrix;

        public DrawPolylineMouseGesture(DesignItem newLine, IInputElement relativeTo, Transform transform)
        {
            _newLine = newLine;
            _positionRelativeTo = relativeTo;
            //this.changeGroup = changeGroup;
            this._matrix = transform.Value;
            _matrix.Invert();

            _startPoint = Mouse.GetPosition(null);
        }

        protected override void OnPreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            base.OnPreviewMouseLeftButtonDown(sender, e);
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            //if (changeGroup == null)
            //    return;
            var delta = _matrix.Transform(e.GetPosition(null) - _startPoint);
            var diff = delta;
            if (_lastAdded.HasValue)
            {
                diff = new Vector(_lastAdded.Value.X - delta.X, _lastAdded.Value.Y - delta.Y);
            }
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (Math.Abs(diff.X) > Math.Abs(diff.Y))
                {
                    delta.Y = 0;
                    if (_newLine.View is Polyline polylineY && polylineY.Points.Count > 1)
                    {
                        delta.Y = polylineY.Points.Reverse().Skip(1).First().Y;
                    }
                    else if (_newLine.View is Polygon polygonY && polygonY.Points.Count > 1)
                    {
                        delta.Y = polygonY.Points.Reverse().Skip(1).First().Y;
                    }
                }
                else
                {
                    delta.X = 0;
                    if (_newLine.View is Polyline polylineX && polylineX.Points.Count > 1)
                    {
                        delta.X = polylineX.Points.Reverse().Skip(1).First().X;
                    }
                    else if (_newLine.View is Polygon polygonX && polygonX.Points.Count > 1)
                    {
                        delta.X = polygonX.Points.Reverse().Skip(1).First().X;
                    }
                }
            }
            var point = new Point(delta.X, delta.Y);

            if (_newLine.View is Polyline)
            {
                if (((Polyline)_newLine.View).Points.Count <= 1)
                    ((Polyline)_newLine.View).Points.Add(point);
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                    ((Polyline)_newLine.View).Points.RemoveAt(((Polyline)_newLine.View).Points.Count - 1);
                if (((Polyline)_newLine.View).Points.Last() != point)
                    ((Polyline)_newLine.View).Points.Add(point);
            }
            else
            {
                if (((Polygon)_newLine.View).Points.Count <= 1)
                    ((Polygon)_newLine.View).Points.Add(point);
                if (Mouse.LeftButton != MouseButtonState.Pressed)
                    ((Polygon)_newLine.View).Points.RemoveAt(((Polygon)_newLine.View).Points.Count - 1);
                if (((Polygon)_newLine.View).Points.Last() != point)
                    ((Polygon)_newLine.View).Points.Add(point);
            }
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //if (changeGroup == null)
            //    return;

            var delta = _matrix.Transform(e.GetPosition(null) - _startPoint);
            var diff = delta;
            if (_lastAdded.HasValue)
            {
                diff = new Vector(_lastAdded.Value.X - delta.X, _lastAdded.Value.Y - delta.Y);
            }
            if (Keyboard.IsKeyDown(Key.LeftAlt) || Keyboard.IsKeyDown(Key.RightAlt))
            {
                if (Math.Abs(diff.X) > Math.Abs(diff.Y))
                {
                    delta.Y = 0;
                    if (_newLine.View is Polyline polylineY && polylineY.Points.Count > 1)
                    {
                        delta.Y = polylineY.Points.Reverse().Skip(1).First().Y;
                    }
                    else if (_newLine.View is Polygon polygonY && polygonY.Points.Count > 1)
                    {
                        delta.Y = polygonY.Points.Reverse().Skip(1).First().Y;
                    }
                }
                else
                {
                    delta.X = 0;
                    if (_newLine.View is Polyline polylineX && polylineX.Points.Count > 1)
                    {
                        delta.X = polylineX.Points.Reverse().Skip(1).First().X;
                    }
                    else if (_newLine.View is Polygon polygonX && polygonX.Points.Count > 1)
                    {
                        delta.X = polygonX.Points.Reverse().Skip(1).First().X;
                    }
                }
            }
            var point = new Point(delta.X, delta.Y);
            _lastAdded = point;

            if (_newLine.View is Polyline polyline)
                polyline.Points.Add(point);
            else
                ((Polygon)_newLine.View).Points.Add(point);
        }

        protected override void OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            base.OnMouseDoubleClick(sender, e);

            if (_newLine.View is Polyline polyline)
            {
                polyline.Points.RemoveAt(polyline.Points.Count - 1);
                _newLine.Properties[Polyline.PointsProperty].SetValue(new PointCollectionConverter().ConvertToInvariantString(((Polyline)_newLine.View).Points));
            }
            else
            {
                ((Polygon)_newLine.View).Points.RemoveAt(((Polygon)_newLine.View).Points.Count - 1);
                _newLine.Properties[Polygon.PointsProperty].SetValue(new PointCollectionConverter().ConvertToInvariantString(((Polygon)_newLine.View).Points));

            }

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
            //if (services.Tool.CurrentTool is CreateComponentTool)
            //{
            //    services.Tool.CurrentTool = services.Tool.PointerTool;
            //}

            _newLine.ReapplyAllExtensions();

            base.OnStopped();
        }

    }
}
