using System;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerPropertyGrid
{
    /// <summary>
    /// Attribute to specify that the decorated class is a editor for the specified property.
    /// 该特性指定装饰类是指定属性的编辑器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class PropertyEditorAttribute : Attribute
    {
        readonly Type propertyDeclaringType;
        readonly string propertyName;

        /// <summary>
        /// Creates a new PropertyEditorAttribute that specifies that the decorated class is a editor
        /// for the "<paramref name="propertyDeclaringType"/>.<paramref name="propertyName"/>".
        /// 创建一个新的PropertyEditorAttribute，它指定装饰类是propertyDeclaringType.propertyName的编辑器
        /// </summary>
        public PropertyEditorAttribute(Type propertyDeclaringType, string propertyName)
        {
            if (propertyDeclaringType == null)
                throw new ArgumentNullException("propertyDeclaringType");
            if (propertyName == null)
                throw new ArgumentNullException("propertyName");
            this.propertyDeclaringType = propertyDeclaringType;
            this.propertyName = propertyName;
        }

        /// <summary>
        /// Gets the type that declares the property that the decorated editor supports.
        /// 获取声明装饰编辑器支持的属性的类型。
        /// </summary>
        public Type PropertyDeclaringType
        {
            get { return propertyDeclaringType; }
        }

        /// <summary>
        /// Gets the name of the property that the decorated editor supports.
        /// 获取装饰编辑器支持的属性的名称。
        /// </summary>
        public string PropertyName
        {
            get { return propertyName; }
        }
    }
}
