using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    /// <summary>
    /// 仅在最小拖动距离后开始拖动的鼠标手势的基类
    /// </summary>
    public abstract class ClickOrDragMouseGesture : MouseGestureBase
    {
        protected Point _startPoint;
        protected bool _hasDragStarted;
        protected IInputElement _positionRelativeTo;

        // const double MinimumDragDistance = 3;

        protected sealed override void OnStarted(MouseButtonEventArgs e)
        {
            Debug.Assert(_positionRelativeTo != null);
            _hasDragStarted = false;
            _startPoint = e.GetPosition(_positionRelativeTo);
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_hasDragStarted)
            {
                Vector v = e.GetPosition(_positionRelativeTo) - _startPoint;
                if (Math.Abs(v.X) >= SystemParameters.MinimumHorizontalDragDistance || Math.Abs(v.Y) >= SystemParameters.MinimumVerticalDragDistance)
                {
                    _hasDragStarted = true;

                    OnDragStarted(e);
                }
            }
        }

        protected override void OnStopped() => _hasDragStarted = false;

        protected virtual void OnDragStarted(MouseEventArgs e) { }
    }
}
