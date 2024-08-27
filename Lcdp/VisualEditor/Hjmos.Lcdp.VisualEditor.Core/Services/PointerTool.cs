using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    internal sealed class PointerTool : ITool
    {
        internal static readonly PointerTool Instance = new();

        public Cursor Cursor => null;

        public void Activate(IDesignPanel designPanel) => designPanel.MouseDown += OnMouseDown;

        public void Deactivate(IDesignPanel designPanel) => designPanel.MouseDown -= OnMouseDown;

        void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            IDesignPanel designPanel = (IDesignPanel)sender;
            DesignPanelHitTestResult result = designPanel.HitTest(e.GetPosition(designPanel), false, true, HitTestType.ElementSelection);
            if (result.ModelHit != null)
            {
                // 点击Grid的时候，b返回PanelSelectionHandler，点击组件的时候返回null
                IHandlePointerToolMouseDown b = result.ModelHit.GetBehavior<IHandlePointerToolMouseDown>();
                if (b != null)
                {
                    // 处理鼠标点击事件
                    // 执行到这一句，如果点击了Grid，会调用到继承子IHandlePointerToolMouseDown的PanelSelectionHandler
                    b.HandleSelectionMouseDown(designPanel, e, result);
                }
                if (!e.Handled)
                {
                    if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left))
                    {
                        e.Handled = true;
                        // 这里返回Hjmos.Lcdp.VisualEditor.Core.Services.DefaultSelectionService
                        ISelectionService selectionService = designPanel.Context.Services.Selection;
                        bool setSelectionIfNotMoving = false;
                        if (selectionService.IsComponentSelected(result.ModelHit))
                        {
                            setSelectionIfNotMoving = true;
                            // There might be multiple components selected. We might have
                            // to set the selection to only the item clicked on
                            // (or deselect the item clicked on if Ctrl is pressed),
                            // but we should do so only if the user isn't performing a drag operation.
                            // 可能会选择多个组件。我们可能不得不将选择设置为只选中所点击的项目(或者在按下Ctrl时取消选中所点击的项目)，但我们应该只在用户不执行拖动操作时才这样做。
                        }
                        else
                        {
                            // 这一句给组件附加装饰层
                            selectionService.SetSelectedComponents(new DesignItem[] { result.ModelHit }, SelectionTypes.Auto);
                        }
                        if (selectionService.IsComponentSelected(result.ModelHit))
                        {
                            // 这里控制组件的拖拽移动，没有这一句，组件有装饰但是不能移动
                            new DragMoveMouseGesture(result.ModelHit, e.ClickCount == 2, setSelectionIfNotMoving).Start(designPanel, e);
                        }
                    }
                }
            }
        }
    }
}
