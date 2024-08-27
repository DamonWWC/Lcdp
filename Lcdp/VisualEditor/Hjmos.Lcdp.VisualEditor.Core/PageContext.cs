using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Core.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 页面上下文，基于事件传递
    /// TODO：参考NavigationAware、NavigationContext
    /// </summary>
    public class PageContext
    {
        /// <summary>
        /// 页面容器的引用
        /// </summary>
        public PageShell PageShell { get; set; }

        /// <summary>
        /// 页面参数
        /// </summary>
        public PageParameters Parameters { get; set; }
    }
}
