using System;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 指定组件作为参数的事件参数。
    /// </summary>
    public class DesignItemEventArgs : EventArgs
    {
        public DesignItemEventArgs(DesignItem item) => Item = item;

        /// <summary>
        /// 受事件影响的组件
        /// </summary>
        public DesignItem Item { get; }
    }
}