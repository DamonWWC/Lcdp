﻿using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public class RasterPlacementBehavior : DefaultPlacementBehavior
    {
        private Canvas surface;
        private AdornerPanel adornerPanel;
        private bool rasterDrawn = false;
        private int raster = 5;

        public override void BeginPlacement(PlacementOperation operation)
        {
            base.BeginPlacement(operation);

            if (ExtendedItem.Services.DesignPanel is DesignPanel designPanel)
                raster = designPanel.RasterWidth;

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
            }
        }

        void DeleteSurface()
        {
            rasterDrawn = false;
            if (surface != null)
            {
                ExtendedItem.Services.DesignPanel.Adorners.Remove(adornerPanel);
                adornerPanel = null;
                surface = null;
            }
        }

        public override void BeforeSetPosition(PlacementOperation operation)
        {
            base.BeforeSetPosition(operation);
            if (surface == null) return;

            if (ExtendedItem.Services.DesignPanel is not DesignPanel designPanel || !designPanel.UseRasterPlacement)
                return;

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                surface.Children.Clear();
                rasterDrawn = false;
                return;
            }

            DrawRaster();

            Rect bounds = operation.PlacedItems[0].Bounds;
            bounds.Y = ((int)bounds.Y / raster) * raster;
            bounds.X = ((int)bounds.X / raster) * raster;
            bounds.Width = Convert.ToInt32((bounds.Width / raster)) * raster;
            bounds.Height = Convert.ToInt32((bounds.Height / raster)) * raster;
            operation.PlacedItems[0].Bounds = bounds;
        }

        public override Point PlacePoint(Point point)
        {
            if (surface == null)
                return base.PlacePoint(point);

            if (ExtendedItem.Services.DesignPanel is not DesignPanel designPanel || !designPanel.UseRasterPlacement)
                return base.PlacePoint(point);

            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                surface.Children.Clear();
                rasterDrawn = false;
                return base.PlacePoint(point);
            }

            DrawRaster();

            point.Y = ((int)point.Y / raster) * raster;
            point.X = ((int)point.X / raster) * raster;

            return point;
        }

        private void DrawRaster()
        {
            if (!rasterDrawn)
            {
                rasterDrawn = true;

                double w = ModelTools.GetWidth(ExtendedItem.View);
                double h = ModelTools.GetHeight(ExtendedItem.View);
                DoubleCollection dash = new DoubleCollection() { 1, raster - 1 };
                for (int i = 0; i <= h; i += raster)
                {
                    Line line = new()
                    {
                        X1 = 0,
                        Y1 = i,
                        X2 = w,
                        Y2 = i,
                        StrokeThickness = 1,
                        Stroke = Brushes.Black,
                        StrokeDashArray = dash,
                    };
                    surface.Children.Add(line);
                }
            }
        }
    }
}
