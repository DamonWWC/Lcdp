using System;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditor.Core
{

    /// <summary>
    /// 指定组件作为参数的事件参数。
    /// </summary>
    public class DesignItemCollectionEventArgs : EventArgs
    {

        public DesignItemCollectionEventArgs(ICollection<DesignItem> items) => Items = items;

        /// <summary>
        /// 受事件影响的组件
        /// </summary>
        public ICollection<DesignItem> Items { get; }
    }
}
