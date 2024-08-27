using System;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 管理工具选择的服务。
    /// </summary>
    public interface IToolService
    {
        /// <summary>为DesignPanel设置自定义HitTestFilterCallback</summary>
        HitTestFilterCallback DesignPanelHitTestFilterCallback { set; }

        /// <summary>
        /// 获取“指针”工具
        /// 指针工具是选择和移动元素的默认工具
        /// </summary>
        ITool PointerTool { get; }

        /// <summary>获取/设置当前选定的工具</summary>
        ITool CurrentTool { get; set; }

        /// <summary>当前工具更改事件</summary>
        event EventHandler CurrentToolChanged;
    }
}
