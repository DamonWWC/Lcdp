using System;

namespace Hjmos.Lcdp.VisualEditor.Core.Interfaces
{
    /// <summary>
    /// 样例组件
    /// </summary>
    public interface ISample : IWidget
    {
        /// <summary>
        /// 运行时真正的组件类型
        /// </summary>
        Type WidgetType { get; set; }
    }
}
