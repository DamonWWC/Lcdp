using System;

namespace Hjmos.Lcdp.VisualEditor.Core.Attributes
{
    /// <summary>
    /// 自定义特性，标记对象保存到页面时，不使用Converter，直接转换为JSON
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ConvertToJsonAttribute : Attribute
    {

    }
}
