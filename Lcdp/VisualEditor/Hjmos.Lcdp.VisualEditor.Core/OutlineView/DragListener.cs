﻿using System;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.OutlineView
{
    public class DragListener
    {
        public DragListener(FrameworkElement target)
        {
            this.target = target;
            target.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(MouseButtonDown), true);
            target.PreviewMouseMove += MouseMove;
            target.PreviewMouseLeftButtonUp += MouseLeftButtonUp;
        }

        public event MouseButtonEventHandler DragStarted;

        FrameworkElement target;
        Point startPoint;
        bool ready;
        MouseButtonEventArgs args;

        void MouseButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && Mouse.Captured == null)
            {
                ready = true;
                startPoint = e.GetPosition(target);
                args = e;
                target.CaptureMouse();
            }
        }

        void MouseMove(object sender, MouseEventArgs e)
        {
            if (ready)
            {
                var currentPoint = e.GetPosition(target);
                if (Math.Abs(currentPoint.X - startPoint.X) >= SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(currentPoint.Y - startPoint.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    ready = false;
                    if (DragStarted != null)
                    {
                        DragStarted(this, args);
                    }
                }
            }
        }

        void MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ready = false;
            target.ReleaseMouseCapture();
        }
    }
}
