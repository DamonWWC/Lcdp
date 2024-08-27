using System;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions
{
    /// <summary>
    /// 属性指定扩展的属性。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class ExtensionAttribute : Attribute
    {
        /// <summary>获取或设置使用扩展的顺序</summary>
        public int Order { get; set; }
    }
}
