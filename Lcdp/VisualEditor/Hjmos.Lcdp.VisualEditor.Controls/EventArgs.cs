using System;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditor.Controls
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

    /// <summary>
    /// 指定组件和属性作为参数的事件参数。
    /// </summary>
    public class DesignItemPropertyChangedEventArgs : DesignItemEventArgs
    {

        public DesignItemPropertyChangedEventArgs(DesignItem item, DesignItemProperty itemProperty) : base(item) => ItemProperty = itemProperty;

        public DesignItemPropertyChangedEventArgs(DesignItem item, DesignItemProperty itemProperty, object oldValue, object newValue) : this(item, itemProperty)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        /// <summary>
        /// 受事件影响的属性
        /// </summary>
        public DesignItemProperty ItemProperty { get; }

        /// <summary>
        /// 之前的值
        /// </summary>
        public object OldValue { get; }

        /// <summary>
        /// 新值
        /// </summary>
        public object NewValue { get; }
    }

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
