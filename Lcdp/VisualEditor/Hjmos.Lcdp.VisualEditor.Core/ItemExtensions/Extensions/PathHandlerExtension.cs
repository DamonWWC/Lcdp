﻿using Hjmos.Lcdp.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.Controls;
using Hjmos.Lcdp.VisualEditor.Core.Thumbs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionFor(typeof(Path))]
    public class PathHandlerExtension : LineExtensionBase, IKeyDown, IKeyUp
    {
        enum PathPartConvertType
        {
            insertPoint,
            ToLineSegment,
            ToBezierSegment,
            ToQuadricBezierSegment,
            ToArcSegment,
        }

        // 路径上一个可修改的点
        protected class PathPoint : INotifyPropertyChanged
        {
            public PathPoint(Point point, object @object, object parentObject, Action<Point> setLambda, Action save, Shape shape)
            {
                this._point = point;
                this._setLambda = setLambda;
                this.Object = @object;
                this.ParentObject = parentObject;
                this._shape = shape;
                this._save = save;
            }

            private Point _point;
            readonly Action<Point> _setLambda;
            readonly Action _save;
            readonly Shape _shape;

            public void Commit() => _save();

            public Point Point
            {
                get { return _point; }
                set
                {
                    if (_setLambda != null)
                    {
                        _point = value;
                        _setLambda(value);
                        if (PropertyChanged != null)
                        {
                            PropertyChanged(this, new PropertyChangedEventArgs("Point"));
                            PropertyChanged(this, new PropertyChangedEventArgs("TranslatedPoint"));
                        }
                    }
                }
            }

            public Point TranslatedPoint
            {
                get
                {
                    if (_shape.RenderedGeometry.Transform == null)
                        return Point;
                    return _shape.RenderedGeometry.Transform.Transform(Point);
                }
                set
                {
                    if (_shape.RenderedGeometry.Transform == null)
                        Point = value;
                    else
                        Point = _shape.RenderedGeometry.Transform.Inverse.Transform(value);
                }
            }

            public PathPoint ParentPathPoint { get; set; }

            public Point ReferencePoint { get; private set; }

            public object Object { get; private set; }

            public object ParentObject { get; private set; }

            public int PolyLineIndex { get; set; }

            public PathPoint TargetPathPoint { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;
        }

        // A Thumb wich displays the Point
        protected class PathThumb : PointThumb
        {
            public PathThumb(int index, PathPoint pathpoint) : base()
            {
                this.Index = index;
                this.PathPoint = pathpoint;
                var bnd = new Binding("TranslatedPoint") { Source = this.PathPoint, Mode = BindingMode.OneWay };
                this.SetBinding(PointProperty, bnd);
            }

            public int Index { get; set; }

            public PathPoint PathPoint { get; set; }
        }

        // A Converter for the RealtiveTo Point (on PolyLineSegment, ...)
        protected class RelativeToPointConverter : IValueConverter
        {
            readonly PathPoint pathPoint;

            public RelativeToPointConverter(PathPoint pathPoint)
            {
                this.pathPoint = pathPoint;
            }
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                var pt = (Point)value;
                return pt - new Vector(pathPoint.TranslatedPoint.X, pathPoint.TranslatedPoint.Y);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private readonly Dictionary<int, Point> _selectedPoints = new Dictionary<int, Point>();
#pragma warning disable 0414 // For future use, disable Warning CS0414: The field is assigned but its value is never used
        private bool _isDragging;
#pragma warning restore 0414
        ZoomControl _zoom;

        private List<PathPoint> pathPoints = null;

        protected virtual Control[] BuildMenu(PathPoint pathpoint)
        {
            var menuList = new List<Control>();
            MenuItem menuItem = null;

            if (pathpoint.TargetPathPoint == null && (pathpoint.Object is LineSegment || pathpoint.Object is PolyLineSegment || pathpoint.Object is BezierSegment || pathpoint.Object is QuadraticBezierSegment || pathpoint.Object is ArcSegment))
            {
                menuItem = new MenuItem() { Header = "insert Point", HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                menuItem.Click += (s, e) => ConvertPart(((DependencyObject)s).TryFindParent<PathThumb>(), PathPartConvertType.insertPoint);
                menuList.Add(menuItem);
                menuItem = new MenuItem() { Header = "to Line Segment", HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                menuItem.Click += (s, e) => ConvertPart(((DependencyObject)s).TryFindParent<PathThumb>(), PathPartConvertType.ToLineSegment);
                menuList.Add(menuItem);
                menuItem = new MenuItem() { Header = "to Bezier Segment", HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                menuItem.Click += (s, e) => ConvertPart(((DependencyObject)s).TryFindParent<PathThumb>(), PathPartConvertType.ToBezierSegment);
                menuList.Add(menuItem);
                menuItem = new MenuItem() { Header = "to Quadric Bezier Segment", HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                menuItem.Click += (s, e) => ConvertPart(((DependencyObject)s).TryFindParent<PathThumb>(), PathPartConvertType.ToQuadricBezierSegment);
                menuList.Add(menuItem);
                menuItem = new MenuItem() { Header = "to Arc Segment", HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                menuItem.Click += (s, e) => ConvertPart(((DependencyObject)s).TryFindParent<PathThumb>(), PathPartConvertType.ToArcSegment);
                menuList.Add(menuItem);
                menuList.Add(new Separator());
                menuItem = new MenuItem() { Header = "is Stroked", IsChecked = ((PathSegment)pathpoint.Object).IsStroked, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                menuItem.Click += (s, e) => ChangeIsStroked(((DependencyObject)s).TryFindParent<PathThumb>(), (MenuItem)s);
                menuList.Add(menuItem);
                menuItem = new MenuItem() { Header = "is Smooth Join", IsChecked = ((PathSegment)pathpoint.Object).IsSmoothJoin, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                menuList.Add(menuItem);
            }
            if (pathpoint.Object is ArcSegment)
            {
                menuItem = new MenuItem() { Header = "is large Arc", IsChecked = ((ArcSegment)pathpoint.Object).IsLargeArc, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                menuList.Add(menuItem);
                menuItem = new MenuItem() { Header = "Rotation Angle", IsChecked = true, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                menuList.Add(menuItem);
                menuItem = new MenuItem() { Header = "Clockwise SweepDirection", IsChecked = ((ArcSegment)pathpoint.Object).SweepDirection == SweepDirection.Clockwise, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                menuList.Add(menuItem);
            }

            if (pathpoint.Object is PathFigure)
            {
                menuItem = new MenuItem() { Header = "is Closed", IsChecked = ((PathFigure)pathpoint.Object).IsClosed, HorizontalContentAlignment = HorizontalAlignment.Left, VerticalContentAlignment = VerticalAlignment.Center };
                menuItem.Click += (s, e) => ChangeIsClosed(((DependencyObject)s).TryFindParent<PathThumb>(), (MenuItem)s);
                menuList.Add(menuItem);
            }

            if (!menuList.Any())
                return null;

            return menuList.ToArray();
        }

        #region thumb methods
        protected virtual PathThumb CreateThumb(PlacementAlignment alignment, Cursor cursor, int index, PathPoint pathpoint, Transform transform)
        {
            var designerThumb = new PathThumb(index, pathpoint) { Cursor = cursor };
            designerThumb.OperationMenu = BuildMenu(pathpoint);

            designerThumb.InnerRenderTransform = ((Transform)transform.Inverse);

            if (pathpoint.TargetPathPoint != null)
            {
                designerThumb.IsEllipse = true;
                designerThumb.Foreground = Brushes.Blue;

                var bnd = new Binding("TranslatedPoint") { Source = pathpoint.TargetPathPoint, Mode = BindingMode.OneWay, Converter = new RelativeToPointConverter(pathpoint) };
                designerThumb.SetBinding(PathThumb.RelativeToPointProperty, bnd);
            }

            AdornerPanel.SetPlacement(designerThumb, designerThumb.AdornerPlacement);
            adornerPanel.Children.Add(designerThumb);

            DragListener drag = new DragListener(designerThumb);
            drag.Transform = transform;

            WeakEventManager<DesignerThumb, MouseButtonEventArgs>.AddHandler(designerThumb, "PreviewMouseLeftButtonDown", ResizeThumbOnMouseLeftButtonUp);

            //drag.MouseDown += drag_MouseDown;
            drag.Started += Drag_Started;
            drag.Changed += Drag_Changed;
            drag.Completed += Drag_Completed;
            return designerThumb;
        }

        private void ConvertPart(PathThumb senderThumb, PathPartConvertType convertType)
        {
            if (senderThumb.PathPoint.ParentObject is PathFigure)
            {
                var pathFigure = senderThumb.PathPoint.ParentObject as PathFigure;
                var pathSegment = senderThumb.PathPoint.Object as PathSegment;

                var idx = pathFigure.Segments.IndexOf(pathSegment);

                var point = senderThumb.PathPoint.Point;

                if (pathSegment is PolyLineSegment)
                {
                    var poly = pathSegment as PolyLineSegment;
                    var lst = poly.Points.Take(senderThumb.PathPoint.PolyLineIndex);
                    var lst2 = poly.Points.Skip(senderThumb.PathPoint.PolyLineIndex + 1);
                    var p = poly.Points[senderThumb.PathPoint.PolyLineIndex];
                    pathFigure.Segments.RemoveAt(idx);
                    var p1 = new PolyLineSegment();
                    p1.Points.AddRange(lst);
                    pathFigure.Segments.Insert(idx, p1);
                    pathSegment = new LineSegment() { Point = p };
                    pathFigure.Segments.Insert(idx + 1, pathSegment);
                    var p2 = new PolyLineSegment();
                    p2.Points.AddRange(lst2);
                    pathFigure.Segments.Insert(idx + 2, p2);
                    idx++;
                }
                else if (pathSegment is PolyBezierSegment)
                {
                    // TODO:
                }
                else if (pathSegment is PolyQuadraticBezierSegment)
                {
                    // TODO:
                }

                pathFigure.Segments.RemoveAt(idx);

                var midp = senderThumb.PathPoint.ParentPathPoint.Point - ((senderThumb.PathPoint.ParentPathPoint.Point - point) / 2);

                PathSegment newSegment = null;
                switch (convertType)
                {
                    case PathPartConvertType.ToBezierSegment:
                        newSegment = new BezierSegment() { Point1 = midp - new Vector(40, 40), Point2 = midp + new Vector(-40, 40), Point3 = point };
                        break;
                    case PathPartConvertType.ToQuadricBezierSegment:
                        newSegment = new QuadraticBezierSegment() { Point1 = point - new Vector(40, 40), Point2 = point };
                        break;
                    case PathPartConvertType.ToArcSegment:
                        newSegment = new ArcSegment() { Point = point, Size = new Size(20, 20) };
                        break;
                    case PathPartConvertType.insertPoint:
                        pathFigure.Segments.Insert(idx, pathSegment);
                        newSegment = new LineSegment() { Point = midp, };
                        break;
                    default:
                        newSegment = new LineSegment() { Point = point };
                        break;
                }

                pathFigure.Segments.Insert(idx, newSegment);
                operation = PlacementOperation.Start(extendedItemArray, PlacementType.MovePoint);
                CommitOperation();
            }

            this.ExtendedItem.ReapplyAllExtensions();
        }

        private void ChangeIsClosed(PathThumb senderThumb, MenuItem menuItem)
        {
            var figure = senderThumb.PathPoint.Object as PathFigure;
            figure.IsClosed = !figure.IsClosed;
            menuItem.IsChecked = figure.IsClosed;
        }

        private void ChangeIsStroked(PathThumb senderThumb, MenuItem menuItem)
        {
            var segment = senderThumb.PathPoint.Object as PathSegment;
            segment.IsStroked = !segment.IsStroked;
            menuItem.IsChecked = segment.IsStroked;
        }

        private void ResetThumbs()
        {
            foreach (FrameworkElement rt in adornerPanel.Children)
            {
                if (rt is DesignerThumb)
                    (rt as DesignerThumb).IsPrimarySelection = true;
            }
            _selectedPoints.Clear();
        }

        private void SelectThumb(PathThumb mprt)
        {
            CommitOrAbortKeyboardOperation();

            List<PathPoint> points = GetPoints();
            Point p = points[mprt.Index].TranslatedPoint;
            _selectedPoints.Add(mprt.Index, p);

            mprt.IsPrimarySelection = false;
        }

        #endregion

        #region eventhandlers

        private void ResizeThumbOnMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            // 获取当前手柄
            if (sender is PathThumb mprt)
            {
                // 如果没有按键盘的CTRL键，并且之前没有选择点，清除选择
                if (!_selectedPoints.ContainsKey(mprt.Index) & !Keyboard.IsKeyDown(Key.LeftCtrl) & !Keyboard.IsKeyDown(Key.RightCtrl))
                {
                    ResetThumbs();
                }
                // 添加选中的Thumb，如果按CTRL，这可以是多边形中的所有点
                if (!_selectedPoints.ContainsKey(mprt.Index))
                    SelectThumb(mprt);
                _isDragging = false;

            }
        }

        // TODO: 从这里删除所有的隐藏/显示扩展
        protected void Drag_Started(DragListener drag)
        {
            // 获取当前手柄
            if (drag.Target is PathThumb)
            {
                SetOperation();
            }
        }

        private bool _isKeyboardMoveing;

        private void SetOperation()
        {
            DesignPanel designPanel = ExtendedItem.Services.DesignPanel as DesignPanel;
            _zoom = designPanel.TryFindParent<ZoomControl>();

            CommitOrAbortKeyboardOperation();

            //Move a Virtual Design Item arround... (for Snaplines, raster, ...)
            //And Resfresh the Points after Positioning that Item!
            operation = PlacementOperation.Start(extendedItemArray, PlacementType.MovePoint);
            _isResizing = true;
        }

        private void CommitOrAbortKeyboardOperation()
        {
            if (operation != null)
            {
                if (!_isKeyboardMoveing)
                {
                    PlacementOperation op = operation;
                    operation = null;
                    op.Abort();
                }
                else
                    CommitOperation();
            }

            _isKeyboardMoveing = false;
        }

        private void CommitOperation()
        {
            if (operation != null)
            {
                foreach (int i in _selectedPoints.Keys)
                {
                    pathPoints[i].Commit();
                }

                PlacementOperation op = operation;
                operation = null;
                op.Commit();

                _isKeyboardMoveing = false;

                this.ExtendedItem.ReapplyExtension(typeof(PathHandlerExtension));
            }
            _isResizing = false;
        }

        protected void Drag_Changed(DragListener drag)
        {
            if (drag.Target is PathThumb mprt)
            {
                _isDragging = true;
                MovePoints(drag.Delta.X, drag.Delta.Y);
            }
        }

        protected void Drag_Completed(DragListener drag)
        {
            if (drag.Target is PathThumb mprt)
            {
                if (operation != null && drag.IsCanceled)
                {
                    operation.Abort();
                }
                else if (drag.IsCanceled)
                {
                    //changeGroup.Abort();
                }
                CommitOperation();
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            pathPoints = GetPoints();

            resizeThumbs = new List<DesignerThumb>();

            Transform transform = this.ExtendedItem.GetCompleteAppliedTransformationToView();

            for (int i = 0; i < pathPoints.Count; i++)
            {
                CreateThumb(PlacementAlignment.BottomRight, Cursors.Cross, i, pathPoints[i], transform);
            }

            Invalidate();

            ResetThumbs();
            _isDragging = false;

            extendedItemArray[0] = ExtendedItem;
            ExtendedItem.PropertyChanged += OnPropertyChanged;
            resizeBehavior = PlacementOperation.GetPlacementBehavior(extendedItemArray);
            UpdateAdornerVisibility();

            this.ExtendedItem.Services.Selection.PrimarySelectionChanged += ExtendedItem_Services_Selection_PrimarySelectionChanged;
        }

        protected override void OnRemove()
        {
            this.ExtendedItem.Services.Selection.PrimarySelectionChanged -= ExtendedItem_Services_Selection_PrimarySelectionChanged;
            base.OnRemove();
        }

        private void ExtendedItem_Services_Selection_PrimarySelectionChanged(object sender, EventArgs e)
        {
            if (operation != null && _isKeyboardMoveing)
            {
                List<DesignItem> newSelection = this.ExtendedItem.Services.Selection.SelectedItems.ToList();
                CommitOrAbortKeyboardOperation();
                this.ExtendedItem.Services.Selection.SetSelectedComponents(newSelection);
            }
        }

        #endregion

        private List<PathPoint> GetPoints() => GetPoints(this.ExtendedItem);

        protected List<PathPoint> GetPoints(DesignItem designItem)
        {
            List<PathPoint> retVal = new();
            AddGeometryPoints(retVal, ((Path)designItem.View).Data, ((Path)designItem.View), () => designItem.Properties["Data"].SetValue(((Path)designItem.View).Data.ToString()));

            return retVal;
        }

        protected void AddGeometryPoints(List<PathPoint> list, Geometry geometry, Shape shape, Action saveDesignItem)
        {
            if (geometry is CombinedGeometry)
            {
                CombinedGeometry g = geometry as CombinedGeometry;
                DesignItem d = ExtendedItem.Services.Component.GetDesignItem(g);
                if (d != null)
                {
                    DesignItem dClosure = d;
                    saveDesignItem = () =>
                    {
                        dClosure.Properties["Geometry1"].SetValue(((CombinedGeometry)dClosure.Component).Geometry1);
                        dClosure.Properties["Geometry2"].SetValue(((CombinedGeometry)dClosure.Component).Geometry2);
                    };
                }
                AddGeometryPoints(list, g.Geometry1, shape, saveDesignItem);
                AddGeometryPoints(list, g.Geometry2, shape, saveDesignItem);
            }
            else if (geometry is GeometryGroup)
            {
                GeometryGroup gg = geometry as GeometryGroup;
                foreach (Geometry g in gg.Children.ToList())
                {
                    AddGeometryPoints(list, g, shape, saveDesignItem);
                }
            }
            else if (geometry is StreamGeometry)
            {
                StreamGeometry sg = geometry as StreamGeometry;
                PathGeometry pg = sg.GetFlattenedPathGeometry().Clone();
                DesignItem d = ExtendedItem.Services.Component.GetDesignItem(sg);
                if (d != null)
                {
                    DesignItem dClosure = d;
                    saveDesignItem = () =>
                    {
                        dClosure.ParentProperty.SetValue(Geometry.Parse(new GeometryConverter().ConvertToInvariantString(pg)));
                    };
                }

                if (d != null)
                {
                    if (!d.ParentProperty.IsCollection)
                        ((DependencyObject)d.ParentProperty.DesignItem.Component).SetValue(d.ParentProperty.DependencyProperty, pg);
                    else
                    {
                        GeometryGroup collection = ((DependencyObject)d.ParentProperty.DesignItem.Component) as GeometryGroup;
                        int i = collection.Children.IndexOf(geometry);
                        collection.Children.RemoveAt(i);
                        collection.Children.Insert(i, pg);
                    }
                }
                else
                    ((Path)shape).Data = pg;
                AddGeometryPoints(list, pg, shape, saveDesignItem);
            }
            else if (geometry is PathGeometry)
            {
                PathGeometry g = geometry as PathGeometry;
                DesignItem d = ExtendedItem.Services.Component.GetDesignItem(g);
                if (d != null)
                {
                    DesignItem dClosure = d;
                    saveDesignItem = () => { dClosure.Properties["Figures"].SetValue(((PathGeometry)dClosure.Component).Figures); };
                }
                if (geometry != null)
                {
                    foreach (PathFigure figure in g.Figures)
                    {
                        DesignItem dd = ExtendedItem.Services.Component.GetDesignItem(figure);
                        if (dd != null)
                        {
                            DesignItem ddClosure = dd;
                            saveDesignItem = () =>
                            {
                                ddClosure.Properties["StartPoint"].SetValue(((PathFigure)ddClosure.Component).StartPoint);
                            };
                        }
                        list.Add(new PathPoint(figure.StartPoint, figure, null, (p) => figure.StartPoint = p, saveDesignItem, shape));
                        foreach (PathSegment s in figure.Segments)
                        {
                            PathPoint parentp = list.Last();
                            if (s is LineSegment lineSegment)
                            {
                                DesignItem ds = ExtendedItem.Services.Component.GetDesignItem(s);
                                if (ds != null)
                                {
                                    DesignItem dsClosure = ds;
                                    saveDesignItem = () =>
                                    {
                                        dsClosure.Properties["Point"].SetValue(((LineSegment)dsClosure.Component).Point);
                                    };
                                }
                                list.Add(new PathPoint(lineSegment.Point, s, figure, (p) => lineSegment.Point = p, saveDesignItem, shape) { ParentPathPoint = parentp });
                            }
                            else if (s is PolyLineSegment)
                            {
                                DesignItem ds = ExtendedItem.Services.Component.GetDesignItem(s);
                                if (ds != null)
                                {
                                    DesignItem dsClosure = ds;
                                    saveDesignItem = () => dsClosure.Properties["Points"].SetValue(((PolyLineSegment)dsClosure.Component).Points);
                                }
                                PolyLineSegment poly = s as PolyLineSegment;
                                for (int n = 0; n < poly.Points.Count; n++)
                                {
                                    int closure_n = n;
                                    list.Add(new PathPoint(poly.Points[closure_n], s, figure, (p) => poly.Points[closure_n] = p, saveDesignItem, shape) { PolyLineIndex = closure_n, ParentPathPoint = parentp });
                                    parentp = list.Last();
                                }
                            }
                            else if (s is BezierSegment bezierSegment)
                            {
                                DesignItem ds = ExtendedItem.Services.Component.GetDesignItem(s);
                                if (ds != null)
                                {
                                    DesignItem dsClosure = ds;
                                    saveDesignItem = () =>
                                    {
                                        dsClosure.Properties["Point1"].SetValue(((BezierSegment)dsClosure.Component).Point1);
                                        dsClosure.Properties["Point2"].SetValue(((BezierSegment)dsClosure.Component).Point2);
                                        dsClosure.Properties["Point3"].SetValue(((BezierSegment)dsClosure.Component).Point3);
                                    };
                                }

                                PathPoint pathp = new PathPoint(bezierSegment.Point3, s, figure, (p) => bezierSegment.Point3 = p, saveDesignItem, shape) { ParentPathPoint = parentp };
                                PathPoint previous = list.Last();
                                list.Add(new PathPoint(bezierSegment.Point1, s, figure, (p) => bezierSegment.Point1 = p, saveDesignItem, shape) { TargetPathPoint = previous });
                                list.Add(new PathPoint(bezierSegment.Point2, s, figure, (p) => bezierSegment.Point2 = p, saveDesignItem, shape) { TargetPathPoint = pathp });
                                list.Add(pathp);
                            }
                            else if (s is PolyBezierSegment)
                            {
                                DesignItem ds = ExtendedItem.Services.Component.GetDesignItem(s);
                                if (ds != null)
                                {
                                    DesignItem dsClosure = ds;
                                    saveDesignItem = () => dsClosure.Properties["Points"].SetValue(((PolyBezierSegment)dsClosure.Component).Points);
                                }
                                PolyBezierSegment poly = s as PolyBezierSegment;
                                for (int n = 0; n < poly.Points.Count; n += 3)
                                {
                                    int closure_n = n;
                                    PathPoint previous = list.Last();
                                    PathPoint pathp = new PathPoint(poly.Points[closure_n + 2], s, figure, (p) => poly.Points[closure_n + 2] = p, saveDesignItem, shape) { PolyLineIndex = closure_n + 2, ParentPathPoint = parentp };
                                    list.Add(new PathPoint(poly.Points[closure_n], s, figure, (p) => poly.Points[closure_n] = p, saveDesignItem, shape) { PolyLineIndex = closure_n, TargetPathPoint = previous });
                                    list.Add(new PathPoint(poly.Points[closure_n + 1], s, figure, (p) => poly.Points[closure_n + 1] = p, saveDesignItem, shape) { PolyLineIndex = closure_n + 1, TargetPathPoint = pathp });
                                    list.Add(pathp);
                                    parentp = pathp;
                                }
                            }
                            else if (s is QuadraticBezierSegment qbSegment)
                            {
                                DesignItem ds = ExtendedItem.Services.Component.GetDesignItem(s);
                                if (ds != null)
                                {
                                    DesignItem dsClosure = ds;
                                    saveDesignItem = () =>
                                    {
                                        dsClosure.Properties["Point1"].SetValue(((QuadraticBezierSegment)dsClosure.Component).Point1);
                                        dsClosure.Properties["Point2"].SetValue(((QuadraticBezierSegment)dsClosure.Component).Point2);
                                    };
                                }
                                PathPoint pathp = new(qbSegment.Point2, s, figure, (p) => qbSegment.Point2 = p, saveDesignItem, shape) { ParentPathPoint = parentp };
                                list.Add(new PathPoint(qbSegment.Point1, s, figure, (p) => qbSegment.Point1 = p, saveDesignItem, shape) { TargetPathPoint = pathp });
                                list.Add(pathp);
                            }
                            else if (s is PolyQuadraticBezierSegment)
                            {
                                DesignItem ds = ExtendedItem.Services.Component.GetDesignItem(s);
                                if (ds != null)
                                {
                                    DesignItem dsClosure = ds;
                                    saveDesignItem = () => dsClosure.Properties["Points"].SetValue(((PolyQuadraticBezierSegment)dsClosure.Component).Points);
                                }
                                PolyQuadraticBezierSegment poly = s as PolyQuadraticBezierSegment;
                                for (int n = 0; n < poly.Points.Count; n += 2)
                                {
                                    int closure_n = n;
                                    PathPoint pathp = new(poly.Points[closure_n + 1], s, figure, (p) => poly.Points[closure_n + 1] = p, saveDesignItem, shape) { PolyLineIndex = closure_n + 1, ParentPathPoint = parentp };
                                    list.Add(new PathPoint(poly.Points[closure_n], s, figure, (p) => poly.Points[closure_n] = p, saveDesignItem, shape) { PolyLineIndex = closure_n, TargetPathPoint = pathp });
                                    list.Add(pathp);
                                    parentp = pathp;
                                }
                            }
                            else if (s is ArcSegment arcSegment)
                            {
                                DesignItem ds = ExtendedItem.Services.Component.GetDesignItem(s);
                                if (ds != null)
                                {
                                    DesignItem dsClosure = ds;
                                    saveDesignItem = () =>
                                    {
                                        dsClosure.Properties["Size"].SetValue(((ArcSegment)dsClosure.Component).Size);
                                    };
                                }
                                ArcSegment arc = arcSegment;
                                PathPoint pathp = new PathPoint(arc.Point, s, figure, (p) => arc.Point = p, saveDesignItem, shape) { ParentPathPoint = parentp };
                                list.Add(new PathPoint(arc.Point - new Vector(arc.Size.Width, arc.Size.Height), s, figure, (p) => arc.Size = new Size(Math.Abs(arc.Point.X - p.X), Math.Abs(arc.Point.Y - p.Y)), saveDesignItem, shape) { TargetPathPoint = pathp });
                                list.Add(pathp);
                            }
                        }
                    }
                }
            }
            else if (geometry is RectangleGeometry)
            {
                RectangleGeometry g = geometry as RectangleGeometry;
                list.Add(new PathPoint(g.Rect.TopLeft, geometry, null, null, saveDesignItem, shape)); //(p) => g.Rect.Left = p.X));
                list.Add(new PathPoint(g.Rect.TopRight, geometry, null, null, saveDesignItem, shape)); //(p) => g.Rect.Width = p.X));
                list.Add(new PathPoint(g.Rect.BottomLeft, geometry, null, null, saveDesignItem, shape)); //(p) => g.Rect.Top = p.Y));
                list.Add(new PathPoint(g.Rect.BottomRight, geometry, null, null, saveDesignItem, shape)); //(p) => g.Rect.Height = p.Y));
            }
            else if (geometry is EllipseGeometry)
            {
                EllipseGeometry g = geometry as EllipseGeometry;
                DesignItem d = ExtendedItem.Services.Component.GetDesignItem(g);
                if (d != null)
                {
                    DesignItem dClosure = d;
                    saveDesignItem = () => dClosure.Properties["Center"].SetValue(((EllipseGeometry)dClosure.Component).Center);
                }
                list.Add(new PathPoint(g.Center, geometry, null, (p) => g.Center = p, saveDesignItem, shape));
            }
            else if (geometry is LineGeometry)
            {
                LineGeometry g = geometry as LineGeometry;
                DesignItem d = ExtendedItem.Services.Component.GetDesignItem(g);
                if (d != null)
                {
                    DesignItem dClosure = d;
                    saveDesignItem = () =>
                    {
                        dClosure.Properties["StartPoint"].SetValue(((LineGeometry)dClosure.Component).StartPoint);
                        dClosure.Properties["EndPoint"].SetValue(((LineGeometry)dClosure.Component).EndPoint);
                    };
                }
                list.Add(new PathPoint(g.StartPoint, geometry, null, (p) => g.StartPoint = p, saveDesignItem, shape));
                list.Add(new PathPoint(g.EndPoint, geometry, null, (p) => g.EndPoint = p, saveDesignItem, shape));
            }
        }

        private void MovePoints(double displacementX, double displacementY, bool useContainerBehavior = true)
        {
            Vector relativeTo = new(operation.PlacedItems[0].Bounds.TopLeft.X, operation.PlacedItems[0].Bounds.TopLeft.Y);

            // 迭代所有选定的点
            foreach (int i in _selectedPoints.Keys)
            {
                Point p = pathPoints[i].TranslatedPoint;

                // x and y is calculated from the currentl point
                double x = _selectedPoints[i].X + displacementX;
                double y = _selectedPoints[i].Y + displacementY;

                p.X = x;
                p.Y = y;

                if (useContainerBehavior)
                    p = operation.CurrentContainerBehavior.PlacePoint(p + relativeTo) - relativeTo;

                pathPoints[i].TranslatedPoint = p;
            }
        }

        #region IKeyDown

        public bool InvokeDefaultAction => _selectedPoints.Count == 0 || _selectedPoints.Count == pathPoints.Count - 1;

        private int _movingDistanceX;
        private int _movingDistanceY;
        public void KeyDownAction(object sender, KeyEventArgs e)
        {
            if (_selectedPoints.Count > 0)
            {
                if (IsArrowKey(e.Key))
                {
                    if (operation == null)
                    {
                        SetOperation();
                        _isKeyboardMoveing = true;
                        _movingDistanceX = 0;
                        _movingDistanceY = 0;
                    }
                }

                int dx1 = (e.Key == Key.Left) ? Keyboard.IsKeyDown(Key.LeftShift) ? -10 : -1 : 0;
                int dy1 = (e.Key == Key.Up) ? Keyboard.IsKeyDown(Key.LeftShift) ? -10 : -1 : 0;
                int dx2 = (e.Key == Key.Right) ? Keyboard.IsKeyDown(Key.LeftShift) ? +10 : +1 : 0;
                int dy2 = (e.Key == Key.Down) ? Keyboard.IsKeyDown(Key.LeftShift) ? +10 : +1 : 0;

                _movingDistanceX += dx1 + dx2;
                _movingDistanceY += dy1 + dy2;

                if (operation != null)
                    MovePoints(_movingDistanceX, _movingDistanceY, false);
            }
        }

        public void KeyUpAction(object sender, KeyEventArgs e)
        {
            //			if (IsArrowKey(e.Key))
            //				CommitOperation();
        }

        private bool IsArrowKey(Key key) => key is Key.Left or Key.Right or Key.Up or Key.Down;

        #endregion
    }
}
