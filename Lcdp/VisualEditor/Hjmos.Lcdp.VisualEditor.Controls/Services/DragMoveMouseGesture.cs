using System.Diagnostics;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls.Services
{
    /// <summary>
	/// 用于在容器内或容器之间移动元素的鼠标手势
	/// 属于PointerTool
    /// </summary>
    public sealed class DragMoveMouseGesture : ClickOrDragMouseGesture
    {
        bool isDoubleClick;
        bool setSelectionIfNotMoving;
        MoveLogic moveLogic;

        public DragMoveMouseGesture(DesignItem clickedOn, bool isDoubleClick, bool setSelectionIfNotMoving = false)
        {
            Debug.Assert(clickedOn != null);

            this.isDoubleClick = isDoubleClick;
            this.setSelectionIfNotMoving = setSelectionIfNotMoving;
            this.positionRelativeTo = clickedOn.Services.DesignPanel;

            moveLogic = new MoveLogic(clickedOn);
        }

        protected override void OnDragStarted(MouseEventArgs e)
        {
            moveLogic.Start(startPoint);
        }

        protected override void OnMouseMove(object sender, MouseEventArgs e)
        {
            base.OnMouseMove(sender, e); // call OnDragStarted if min. drag distace is reached
            moveLogic.Move(e.GetPosition(positionRelativeTo));
        }

        protected override void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (!hasDragStarted)
            {
                if (isDoubleClick)
                {
                    // user made a double-click
                    Debug.Assert(moveLogic.Operation == null);
                    moveLogic.HandleDoubleClick();
                }
                else if (setSelectionIfNotMoving)
                {
                    services.Selection.SetSelectedComponents(new DesignItem[] { moveLogic.ClickedOn }, SelectionTypes.Auto);
                }
            }
            moveLogic.Stop();
            Stop();
        }

        protected override void OnStopped()
        {
            moveLogic.Cancel();
        }
    }
}
