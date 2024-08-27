using System;

namespace Hjmos.Lcdp.VisualEditor.Core.Attributes
{
    /// <summary>
    /// 此特性标记对象中数据字段的类型
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DataFieldsAttribute : Attribute
    {

        public DataFieldsAttribute(Type type) => Type = type;

        /// <summary>
        /// 数据面板上的字段对象类型
        /// </summary>
        public Type Type { get; set; }

    }
}
