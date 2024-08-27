using System;
using System.ComponentModel;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 表示DesignItem的属性
    /// 通过DesignItemProperty类完成的所有更改都在底层模型中表示(例如XAML)。
    /// 所有这些更改都记录在当前运行的设计器事务中 (<see cref="ChangeGroup"/>)，支持撤销/重做。
    /// 警告:直接对控件实例所做的更改可能不会反映在模型中。
    /// </summary>
    public abstract class DesignItemProperty : INotifyPropertyChanged
    {
        /// <summary>获取属性名称</summary>
        public abstract string Name { get; protected set; }

        /// <summary>获取属性的返回类型</summary>
        public abstract Type ReturnType { get; }

        /// <summary>获取声明属性的类型</summary>
        public abstract Type DeclaringType { get; }

        /// <summary>获取属性的类别</summary>
        public abstract string Category { get; }

        /// <summary>获取用于将属性值和字符串互相转换的类型转换器</summary>
        public virtual TypeConverter TypeConverter => TypeDescriptor.GetConverter(this.ReturnType);

        /// <summary>获取属性是否表示集合</summary>
        public abstract bool IsCollection { get; }

        /// <summary>获取属性是否表示事件</summary>
        public abstract bool IsEvent { get; }

        /// <summary>获取元素的集合</summary>
        public abstract IObservableList<DesignItem> CollectionElements { get; }

        /// <summary>获取属性的值。如果未设置该值，或者该值被设置为一个原始值，则该属性返回null</summary>
        public abstract DesignItem Value { get; }

        /// <summary>
        /// 获取属性的字符串值。
        /// 如果该值未被设置，或者该值被设置为非原始值(即由<see cref="DesignItem"/>表示，通过<see cref="Value"/>属性可访问)，该属性将返回null。
        /// </summary>
        public abstract string TextValue { get; }

        /// <summary>
        /// 当属性的值发生变化时将被引发，(通过调用<see cref="SetValue"/>或<see cref="Reset"/>)。
        /// </summary>
        public abstract event EventHandler ValueChanged;

        /// <summary>
        /// 在<see cref="ValueOnInstance"/>属性更改时引发。
        /// 如果不经过设计器基础结构就更改了值，则不会引发此事件。
        /// </summary>
        public abstract event EventHandler ValueOnInstanceChanged;

        /// <summary>获取/设置设计实例上的属性值。如果未设置属性，则返回默认值</summary>
        public abstract object ValueOnInstance { get; }

        /// <summary>获取设计实例上的属性值。如果未设置属性，则返回默认值</summary>
        public abstract object DesignerValue { get; }

        /// <summary>设置属性的值</summary>
        public abstract void SetValue(object value);

        /// <summary>获取是否在设计项上设置了属性</summary>
        public abstract bool IsSet { get; }

        /// <summary>IsSet属性改变事件</summary>
        public abstract event EventHandler IsSetChanged;

        /// <summary>将属性值重置为默认值，可能将其从属性列表中删除</summary>
        public abstract void Reset();

        /// <summary>获取父设计项</summary>
        public abstract DesignItem DesignItem { get; }

        /// <summary>获取依赖项属性，如果此属性不表示依赖项属性，则为空</summary>
        public abstract DependencyProperty DependencyProperty { get; }

        /// <summary>如果此属性被认为是“高级”且应在属性网格中默认隐藏，则获取</summary>
        public virtual bool IsAdvanced => false;

        /// <summary>
        /// 获取属性的完整名称(DeclaringType.FullName + "." + Name)。
        /// </summary>
        public string FullName => DeclaringType.FullName + "." + Name;

        /// <summary>
        /// 获取值或ValueOnInstance的视图
        /// (例如，如果Content属性是复杂对象，则它有一个DesignItem；如果它只是文本，则它只有ValueOnInstance)
        /// </summary>		
        public object ValueOnInstanceOrView => Value == null ? ValueOnInstance : Value.View;

        /// <summary>
        /// 获取依赖项属性的完整名称。如果属性不是依赖属性，则返回正常的FullName。
        /// </summary>
        public string DependencyFullName => DependencyProperty != null ? DependencyProperty.GetFullName() : FullName;

        /// <summary>
        /// 当属性值发生变化时将引发
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 为指定的属性引发属性更改事件
        /// </summary>
        /// <param name="propertyName"></param>
        protected virtual void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}