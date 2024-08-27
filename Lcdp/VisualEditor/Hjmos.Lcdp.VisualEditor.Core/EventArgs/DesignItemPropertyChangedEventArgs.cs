namespace Hjmos.Lcdp.VisualEditor.Core
{
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
}
