using System.Diagnostics;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    /// <summary>
	/// 用于在容器内或容器之间移动元素的鼠标手势
	/// 属于PointerTool
    /// </summary>
    public sealed class DragMoveMouseGesture : ClickOrDragMouseGesture
    {
        private readonly bool _isDoubleClick;
        private readonly bool _setSelectionIfNotMoving;
        private readonly MoveLogic _moveLogic;

        public DragMoveMouseGesture(DesignItem clickedOn, bool isDoubleClick, bool setSelectionIfNotMoving = false)
        {
            Debug.Assert(clickedOn != null);

            _isDoubleClick = isDoubleClick;
            _setSelectionIfNotMoving = setSelectionIfNotMoving;
            _positionRelativeTo = clickedOn.Services.DesignPanel;

            _moveLogic = new MoveLogic(clickedOn);
        }

        protected override void OnDragStarted(MouseEventArgs e) => _moveLogic.Start(_startPoint);

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            // 如果达到最小拖动距离，调用OnDragStarted
            base.OnMouseMove(sender, e);
            _moveLogic.Move(e.GetPosition(_positionRelativeTo));
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!_hasDragStarted)
            {
                if (_isDoubleClick)
                {
                    // 用户双击
                    Debug.Assert(_moveLogic.Operation == null);
                    _moveLogic.HandleDoubleClick();
                }
                else if (_setSelectionIfNotMoving)
                {
                    services.Selection.SetSelectedComponents(new DesignItem[] { _moveLogic.ClickedOn }, SelectionTypes.Auto);
                }
            }
            _moveLogic.Stop();
            Stop();
        }

        protected override void OnStopped() => _moveLogic.Cancel();
    }
}
