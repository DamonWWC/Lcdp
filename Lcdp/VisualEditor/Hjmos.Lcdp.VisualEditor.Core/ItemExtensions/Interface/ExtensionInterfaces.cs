using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 如果要在DesignPanel上通知控件按下键事件，则可以实现该接口
    /// </summary>
    public interface IKeyDown
    {
        /// <summary>
        /// 在特定控件上按下键要执行的操作  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void KeyDownAction(object sender, KeyEventArgs e);

        /// <summary>
        /// 如果该控件想要抑制默认的DesignPanel动作，让它返回false
        /// </summary>
        bool InvokeDefaultAction { get; }
    }

    /// <summary>
    /// 如果要在DesignPanel上对控件发出KeyUp事件警报，则可以实现该接口  
    /// </summary>
    public interface IKeyUp
    {
        /// <summary>
        /// 在特定的控制下，在keyup上执行的操作  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void KeyUpAction(object sender, KeyEventArgs e);
    }


}
