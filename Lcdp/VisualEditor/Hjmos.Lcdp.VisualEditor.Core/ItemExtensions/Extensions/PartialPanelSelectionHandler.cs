using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public class PartialPanelSelectionHandler : BehaviorExtension, IHandlePointerToolMouseDown
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();
            this.ExtendedItem.AddBehavior(typeof(IHandlePointerToolMouseDown), this);
        }

        #region IHandlePointerToolMouseDown

        public void HandleSelectionMouseDown(IDesignPanel designPanel, MouseButtonEventArgs e, DesignPanelHitTestResult result)
        {
            if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left))
            {
                e.Handled = true;
                new PartialRangeSelectionGesture(result.ModelHit).Start(designPanel, e);
            }
        }

        #endregion
    }
}
