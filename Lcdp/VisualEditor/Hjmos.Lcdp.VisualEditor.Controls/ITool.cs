using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 描述一种可以处理设计界面上输入的工具。
    /// 仿照这个网址上的描述：http://urbanpotato.net/Default.aspx/document/2300  网址已经失效了
    /// </summary>
    public interface ITool
    {
        /// <summary>
        /// 获取工具使用的光标
        /// </summary>
        Cursor Cursor { get; }

        /// <summary>
        /// 激活工具，将其事件处理程序附加到设计面板
        /// </summary>
        void Activate(IDesignPanel designPanel);

        /// <summary>
        /// 禁用该工具，将其事件处理程序从设计面板分离
        /// </summary>
        void Deactivate(IDesignPanel designPanel);
    }
}
