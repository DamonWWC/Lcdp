using System;

namespace Hjmos.Lcdp.VisualEditor.Core.Attributes
{
    /// <summary>
    /// 此特性标记的对象，表示其真实的类型保持在字段TypeName中
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class TypeNameMarkAttribute : Attribute
    {

    }
}
