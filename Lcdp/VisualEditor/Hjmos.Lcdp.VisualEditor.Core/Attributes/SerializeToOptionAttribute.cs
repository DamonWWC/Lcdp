using System;

namespace Hjmos.Lcdp.VisualEditor.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class SerializeToOptionAttribute : Attribute
    {
        /// <summary>
        /// 标记属性是否序列化到页面文件中
        /// </summary>
        public bool Flag { get; } = true;

        /// <summary>
        /// 标记属性是否序列化到页面文件中
        /// </summary>
        /// <param name="flag">false时不序列化</param>
        public SerializeToOptionAttribute(bool flag) => Flag = flag;
    }
}
