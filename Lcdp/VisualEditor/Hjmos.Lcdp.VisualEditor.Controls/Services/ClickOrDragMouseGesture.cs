using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls.Services
{
    /// <summary>
    /// Base class for mouse gestures that should start dragging only after a minimum drag distance.
    /// 仅在最小拖动距离后开始拖动的鼠标手势的基类。
    /// </summary>
    public abstract class ClickOrDragMouseGesture : MouseGestureBase
    {
        protected Point startPoint;
        protected bool hasDragStarted;
        protected IInputElement positionRelativeTo;

        const double MinimumDragDistance = 3;

        protected sealed override void OnStarted(MouseButtonEventArgs e)
        {
            Debug.Assert(positionRelativeTo != null);
            hasDragStarted = false;
            startPoint = e.GetPosition(positionRelativeTo);
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!hasDragStarted)
            {
                Vector v = e.GetPosition(positionRelativeTo) - startPoint;
                if (Math.Abs(v.X) >= SystemParameters.MinimumHorizontalDragDistance
                    || Math.Abs(v.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    hasDragStarted = true;

                    OnDragStarted(e);
                }
            }
        }

        protected override void OnStopped()
        {
            hasDragStarted = false;
        }

        protected virtual void OnDragStarted(MouseEventArgs e) { }
    }
}
