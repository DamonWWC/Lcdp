using System;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 指定被修饰的类是使用指定扩展服务的扩展。
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public sealed class ExtensionServerAttribute : Attribute
    {
        /// <summary>使用此扩展设计的项的类型</summary>
        public Type ExtensionServerType { get; }

        public ExtensionServerAttribute(Type extensionServerType)
        {
            if (extensionServerType == null)
                throw new ArgumentNullException("extensionServerType");
            if (!typeof(ExtensionServer).IsAssignableFrom(extensionServerType))
                throw new ArgumentException("extensionServerType必须继承自ExtensionServer");
            if (extensionServerType.GetConstructor(new Type[0]) == null)
                throw new ArgumentException("extensionServerType必须有一个无参构造函数");

            ExtensionServerType = extensionServerType;
        }
    }
}
