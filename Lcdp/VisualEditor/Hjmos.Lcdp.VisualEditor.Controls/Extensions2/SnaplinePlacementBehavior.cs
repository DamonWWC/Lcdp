using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    public class SnaplinePlacementBehavior : RasterPlacementBehavior
    {
        public static bool GetDisableSnaplines(DependencyObject obj) => (bool)obj.GetValue(DisableSnaplinesProperty);

        public static void SetDisableSnaplines(DependencyObject obj, bool value) => obj.SetValue(DisableSnaplinesProperty, value);

        public static readonly DependencyProperty DisableSnaplinesProperty =
            DependencyProperty.RegisterAttached("DisableSnaplines", typeof(bool), typeof(SnaplinePlacementBehavior), new PropertyMetadata(false));

        private AdornerPanel adornerPanel;
        private Canvas surface;
        private List<Snapline> horizontalMap;
        private List<Snapline> verticalMap;
        private double? baseline;

        public static double SnaplineMargin { get; set; } = 8;
        public static double SnaplineAccuracy { get; set; } = 5;

        public override void BeginPlacement(PlacementOperation operation)
        {
            base.BeginPlacement(operation);
            CreateSurface(operation);
        }

        public override void EndPlacement(PlacementOperation operation)
        {
            base.EndPlacement(operation);
            DeleteSurface();
        }

        public override void EnterContainer(PlacementOperation operation)
        {
            base.EnterContainer(operation);
            CreateSurface(operation);
        }

        public override void LeaveContainer(PlacementOperation operation)
        {
            base.LeaveContainer(operation);
            DeleteSurface();
        }

        public override Point PlacePoint(Point point)
        {
            if (surface == null)
                return base.PlacePoint(point);

            DesignPanel designPanel = ExtendedItem.Services.DesignPanel as DesignPanel;
            if (designPanel is null || !designPanel.UseSnaplinePlacement)
                return base.PlacePoint(point); ;

            surface.Children.Clear();
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
                return base.PlacePoint(point); ;

            Rect bounds = new(point.X, point.Y, 0, 0);

            List<Snapline> horizontalInput = new();
            List<Snapline> verticalInput = new();

            AddLines(bounds, 0, false, horizontalInput, verticalInput, null);
            if (baseline.HasValue)
            {
                double textOffset = bounds.Top + baseline.Value;
                horizontalInput.Add(new Snapline() { Group = 1, Offset = textOffset, Start = bounds.Left, End = bounds.Right });
            }


            Point newPoint = base.PlacePoint(point);
            if (Snap(horizontalInput, horizontalMap, SnaplineAccuracy, out List<Snapline> drawLines, out double delta))
            {
                foreach (var d in drawLines)
                {
                    DrawLine(d.Start, d.Offset + d.DrawOffset, d.End, d.Offset + d.DrawOffset);
                }

                point.Y += delta;
            }
            else
                point.Y = newPoint.Y;

            if (Snap(verticalInput, verticalMap, SnaplineAccuracy, out drawLines, out delta))
            {
                foreach (Snapline d in drawLines)
                {
                    DrawLine(d.Offset + d.DrawOffset, d.Start, d.Offset + d.DrawOffset, d.End);
                }

                point.X += delta;
            }
            else
                point.X = newPoint.X;


            return point;
        }

        public override void BeforeSetPosition(PlacementOperation operation)
        {
            base.BeforeSetPosition(operation);
            if (surface == null) return;

            if (ExtendedItem.Services.DesignPanel is not DesignPanel designPanel || !designPanel.UseSnaplinePlacement)
                return;

            surface.Children.Clear();
            if (Keyboard.IsKeyDown(Key.LeftCtrl)) return;

            Rect bounds = Rect.Empty;
            foreach (PlacementInformation item in operation.PlacedItems)
            {
                bounds.Union(item.Bounds);
            }

            List<Snapline> horizontalInput = new List<Snapline>();
            List<Snapline> verticalInput = new List<Snapline>();
            PlacementInformation info = operation.PlacedItems[0];

            if (operation.Type == PlacementType.Resize)
            {
                AddLines(bounds, 0, false, horizontalInput, verticalInput, info.ResizeThumbAlignment);
            }
            else
            {
                AddLines(bounds, 0, false, horizontalInput, verticalInput, null);
                if (baseline.HasValue)
                {
                    double textOffset = bounds.Top + baseline.Value;
                    horizontalInput.Add(new Snapline() { Group = 1, Offset = textOffset, Start = bounds.Left, End = bounds.Right });
                }
            }

            // debug
            //foreach (var t in horizontalMap.Concat(horizontalInput)) {
            //    surface.Children.Add(new Line() { X1 = t.Start, X2 = t.End, Y1 = t.Offset, Y2 = t.Offset, Stroke = Brushes.Black });
            //}
            //foreach (var t in verticalMap.Concat(verticalInput)) {
            //    surface.Children.Add(new Line() { X1 = t.Offset, X2 = t.Offset, Y1 = t.Start , Y2 = t.End, Stroke = Brushes.Black });
            //}
            //return;


            if (Snap(horizontalInput, horizontalMap, SnaplineAccuracy, out List<Snapline> drawLines, out double delta))
            {

                if (operation.Type == PlacementType.Resize)
                {
                    if (info.ResizeThumbAlignment != null && info.ResizeThumbAlignment.Value.Vertical == VerticalAlignment.Top)
                    {
                        bounds.Y += delta;
                        bounds.Height = Math.Max(0, bounds.Height - delta);
                        if (operation.CurrentContainer.Services.GetService<OptionService>().SnaplinePlacementRoundValues)
                        {
                            bounds.Y = Math.Round(bounds.Y);
                            bounds.Height = Math.Round(bounds.Height);
                        }
                    }
                    else
                    {
                        bounds.Height = Math.Max(0, bounds.Height + delta);

                        if (operation.CurrentContainer.Services.GetService<OptionService>().SnaplinePlacementRoundValues)
                        {
                            bounds.Height = Math.Round(bounds.Height);
                        }
                    }
                    info.Bounds = bounds;
                }
                else
                {
                    foreach (PlacementInformation item in operation.PlacedItems)
                    {
                        Rect r = item.Bounds;
                        r.Y += delta;
                        if (operation.CurrentContainer.Services.GetService<OptionService>().SnaplinePlacementRoundValues)
                        {
                            r.Y = Math.Round(r.Y);
                        }
                        item.Bounds = r;
                    }
                }

                foreach (Snapline d in drawLines)
                {
                    DrawLine(d.Start, d.Offset + d.DrawOffset, d.End, d.Offset + d.DrawOffset);
                }
            }

            if (Snap(verticalInput, verticalMap, SnaplineAccuracy, out drawLines, out delta))
            {

                if (operation.Type == PlacementType.Resize)
                {
                    if (info.ResizeThumbAlignment != null && info.ResizeThumbAlignment.Value.Horizontal == HorizontalAlignment.Left)
                    {
                        bounds.X += delta;
                        bounds.Width = Math.Max(0, bounds.Width - delta);
                        if (operation.CurrentContainer.Services.GetService<OptionService>().SnaplinePlacementRoundValues)
                        {
                            bounds.X = Math.Round(bounds.X);
                            bounds.Width = Math.Round(bounds.Width);
                        }
                    }
                    else
                    {
                        bounds.Width = Math.Max(0, bounds.Width + delta);
                        if (operation.CurrentContainer.Services.GetService<OptionService>().SnaplinePlacementRoundValues)
                        {
                            bounds.Width = Math.Round(bounds.Width);
                        }
                    }
                    info.Bounds = bounds;
                }
                else
                {
                    foreach (PlacementInformation item in operation.PlacedItems)
                    {
                        Rect r = item.Bounds;
                        r.X += delta;
                        if (operation.CurrentContainer.Services.GetService<OptionService>().SnaplinePlacementRoundValues)
                        {
                            r.X = Math.Round(r.X);
                        }
                        item.Bounds = r;
                    }
                }

                foreach (Snapline d in drawLines)
                {
                    DrawLine(d.Offset + d.DrawOffset, d.Start, d.Offset + d.DrawOffset, d.End);
                }
            }
        }

        private void CreateSurface(PlacementOperation operation)
        {
            if (ExtendedItem.Services.GetService<IDesignPanel>() != null)
            {

                surface = new Canvas();
                adornerPanel = new AdornerPanel();
                adornerPanel.SetAdornedElement(ExtendedItem.View, ExtendedItem);
                AdornerPanel.SetPlacement(surface, AdornerPlacement.FillContent);
                adornerPanel.Children.Add(surface);
                ExtendedItem.Services.DesignPanel.Adorners.Add(adornerPanel);

                BuildMaps(operation);

                if (operation.Type != PlacementType.Resize && operation.PlacedItems.Count == 1)
                {
                    baseline = GetBaseline(operation.PlacedItems[0].Item.View);
                }
            }
        }

        private IEnumerable<DesignItem> AllDesignItems(DesignItem designItem = null)
        {
            if (designItem == null && this.ExtendedItem.Services.DesignPanel is DesignPanel)
                designItem = this.ExtendedItem.Services.DesignPanel.Context.RootItem;

            if (designItem?.ContentProperty != null)
            {
                if (designItem.ContentProperty.IsCollection)
                {
                    foreach (DesignItem collectionElement in designItem.ContentProperty.CollectionElements)
                    {
                        if (collectionElement != null)
                            yield return collectionElement;

                        foreach (DesignItem el in AllDesignItems(collectionElement))
                        {
                            if (el != null)
                                yield return el;
                        }
                    }
                }
                else if (designItem.ContentProperty.Value != null)
                {
                    yield return designItem.ContentProperty.Value;

                    foreach (DesignItem el in AllDesignItems(designItem.ContentProperty.Value))
                    {
                        if (el != null)
                            yield return el;
                    }
                }
            }
        }

        /// <summary>
        /// Method Returns the DesignItems for wich Snaplines are created
        /// </summary>
        /// <param name="operation"></param>
        /// <returns></returns>
        protected IEnumerable<DesignItem> GetSnapToDesignItems(PlacementOperation operation) => AllDesignItems();

        private void BuildMaps(PlacementOperation operation)
        {
            horizontalMap = new List<Snapline>();
            verticalMap = new List<Snapline>();

            Rect containerRect = new(0, 0, ModelTools.GetWidth(ExtendedItem.View), ModelTools.GetHeight(ExtendedItem.View));
            if (SnaplineMargin > 0)
            {
                AddLines(containerRect, -SnaplineMargin, false);
            }

            AddLines(containerRect, 0, false);

            AddContainerSnaplines(containerRect, horizontalMap, verticalMap);

            if (!CanPlace(operation.PlacedItems.Select(x => x.Item), operation.Type, PlacementAlignment.Center))
                return;

            foreach (DesignItem item in GetSnapToDesignItems(operation).Except(operation.PlacedItems.Select(f => f.Item)).Where(x => x.View != null && !GetDisableSnaplines(x.View)))
            {
                if (item != null)
                {
                    Rect bounds = GetPositionRelativeToContainer(operation, item);

                    AddLines(bounds, 0, false);
                    if (SnaplineMargin > 0)
                    {
                        AddLines(bounds, SnaplineMargin, true);
                    }
                    AddBaseline(item, bounds, horizontalMap);
                }
            }
        }

        protected virtual void AddContainerSnaplines(Rect containerRect, List<Snapline> horizontalMap, List<Snapline> verticalMap) { }

        private void AddLines(Rect r, double inflate, bool requireOverlap) => AddLines(r, inflate, requireOverlap, horizontalMap, verticalMap, null);

        private void AddLines(Rect r, double inflate, bool requireOverlap, List<Snapline> h, List<Snapline> v, PlacementAlignment? filter)
        {
            if (r != Rect.Empty)
            {
                Rect r2 = r;
                r2.Inflate(inflate, inflate);

                if (filter == null || filter.Value.Vertical == VerticalAlignment.Top)
                    h.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Top - 1, Start = r.Left, End = r.Right });
                if (filter == null || filter.Value.Vertical == VerticalAlignment.Bottom)
                    h.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Bottom - 1, Start = r.Left, End = r.Right });
                if (filter == null || filter.Value.Horizontal == HorizontalAlignment.Left)
                    v.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Left - 1, Start = r.Top, End = r.Bottom });
                if (filter == null || filter.Value.Horizontal == HorizontalAlignment.Right)
                    v.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Right - 1, Start = r.Top, End = r.Bottom });

                if (filter == null)
                {
                    h.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Top + Math.Abs((r2.Top - r2.Bottom) / 2) - 1, DrawOffset = 1, Start = r.Left, End = r.Right });
                    v.Add(new Snapline() { RequireOverlap = requireOverlap, Offset = r2.Left + Math.Abs((r2.Left - r2.Right) / 2) - 1, DrawOffset = 1, Start = r.Top, End = r.Bottom });
                }
            }
        }

        private void AddBaseline(DesignItem item, Rect bounds, List<Snapline> list)
        {
            double? baseline = GetBaseline(item.View);
            if (baseline.HasValue)
            {
                double textOffset = item.View.TranslatePoint(new Point(0, baseline.Value), ExtendedItem.View).Y;
                list.Add(new Snapline() { Group = 1, Offset = textOffset, Start = bounds.Left, End = bounds.Right });
            }
        }

        private void DeleteSurface()
        {
            if (surface != null)
            {
                ExtendedItem.Services.DesignPanel.Adorners.Remove(adornerPanel);
                adornerPanel = null;
                surface = null;
                horizontalMap = null;
                verticalMap = null;
            }
        }

        private void DrawLine(double x1, double y1, double x2, double y2)
        {
            if (double.IsInfinity(x1) || double.IsNaN(x1) || double.IsInfinity(y1) || double.IsNaN(y1) || double.IsInfinity(x2) || double.IsNaN(x2) || double.IsInfinity(y2) || double.IsNaN(y2))
                return;

            Line line1 = new()
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                StrokeThickness = 1,
                Stroke = Brushes.White
            };
            surface.Children.Add(line1);

            Line line2 = new()
            {
                X1 = x1,
                Y1 = y1,
                X2 = x2,
                Y2 = y2,
                StrokeThickness = 1,
                Stroke = Brushes.Orange,
                StrokeDashArray = new DoubleCollection(new double[] { 5, 2 }),
                StrokeDashOffset = x1 + y1  // fix dashes
            };
            surface.Children.Add(line2);
        }

        //TODO: GlyphRun must be used
        private static double? GetBaseline(UIElement element)
        {
            if (element is TextBox textBox)
            {
                double r = textBox.GetRectFromCharacterIndex(0).Bottom;
                return textBox.TranslatePoint(new Point(0, r), element).Y;
            }
            if (element is TextBlock textBlock)
                return textBlock.TranslatePoint(new Point(0, textBlock.ActualHeight), element).Y;

            return null;
        }

        private static bool Snap(List<Snapline> input, List<Snapline> map, double accuracy, out List<Snapline> drawLines, out double delta)
        {
            delta = double.MaxValue;
            drawLines = null;

            foreach (Snapline inputLine in input)
            {
                foreach (Snapline mapLine in map)
                {
                    if (Math.Abs(mapLine.Offset - inputLine.Offset) <= accuracy)
                    {
                        if (!inputLine.RequireOverlap && !mapLine.RequireOverlap || Math.Max(inputLine.Start, mapLine.Start) < Math.Min(inputLine.End, mapLine.End))
                        {
                            if (mapLine.Group == inputLine.Group)
                                delta = mapLine.Offset - inputLine.Offset;
                        }
                    }
                }
            }

            if (delta == double.MaxValue) return false;
            Dictionary<double, Snapline> offsetDict = new();

            foreach (Snapline inputLine in input)
            {
                inputLine.Offset += delta;
                foreach (Snapline mapLine in map)
                {
                    if (inputLine.Offset == mapLine.Offset)
                    {
                        double offset = mapLine.Offset;
                        if (!offsetDict.TryGetValue(offset, out Snapline drawLine))
                        {
                            drawLine = new Snapline
                            {
                                Start = double.MaxValue,
                                End = double.MinValue,
                                DrawOffset = mapLine.DrawOffset
                            };
                            offsetDict[offset] = drawLine;
                        }
                        drawLine.Offset = offset;
                        drawLine.Start = Math.Min(drawLine.Start, Math.Min(inputLine.Start, mapLine.Start));
                        drawLine.End = Math.Max(drawLine.End, Math.Max(inputLine.End, mapLine.End));
                    }
                }
            }

            drawLines = offsetDict.Values.ToList();
            return true;
        }

        [DebuggerDisplay("Snapline: {Offset}")]
        protected class Snapline
        {
            public double Offset;
            public double Start;
            public double End;
            public bool RequireOverlap;
            public int Group;

            public double DrawOffset = 0;
        }
    }
}
