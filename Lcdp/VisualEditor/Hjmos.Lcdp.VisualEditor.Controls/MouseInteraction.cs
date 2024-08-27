using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    // Interfaces for mouse interaction on the design surface.
    // 设计界面上的鼠标交互界面。

    /// <summary>
    /// 由元素实现的行为接口，用于处理它们的鼠标按下事件。
    /// </summary>
    public interface IHandlePointerToolMouseDown
    {
        /// <summary>
        /// 调用以处理鼠标点击事件
        /// </summary>
        void HandleSelectionMouseDown(IDesignPanel designPanel, MouseButtonEventArgs e, DesignPanelHitTestResult result);
    }
}
