using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.Windows.Controls;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 处理面板中选择的多个控件。
    /// </summary>
    [ExtensionFor(typeof(Panel))]
    public class PanelSelectionHandler : BehaviorExtension, IHandlePointerToolMouseDown
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.ExtendedItem.AddBehavior(typeof(IHandlePointerToolMouseDown), this);
        }

        public void HandleSelectionMouseDown(IDesignPanel designPanel, MouseButtonEventArgs e, DesignPanelHitTestResult result)
        {
            if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left))
            {
                e.Handled = true;
                // 这一句会显示拖拽框、或者给组件附加装饰层
                new RangeSelectionGesture(result.ModelHit).Start(designPanel, e);
            }
        }
    }
}
