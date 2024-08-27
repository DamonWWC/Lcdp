using System;
using System.Collections.Generic;
namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions
{
    /// <summary>
    /// 指定被修饰类是指定项类型的WPF扩展
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public sealed class ExtensionForAttribute : Attribute
    {
        private Type _overrideExtension;
        private readonly List<Type> _overrideExtensions = new();

        /// <summary>获取使用此扩展设计的项的类型</summary>
        public Type DesignedItemType { get; }

        /// <summary>获取/设置此扩展要覆盖的另一个扩展的类型</summary>
        public Type[] OverrideExtensions
        {
            get => _overrideExtensions.ToArray();
            set => _overrideExtensions.AddRange(value);
        }

        /// <summary>获取/设置此扩展要覆盖的另一个扩展的类型</summary>
        public Type OverrideExtension
        {
            get => _overrideExtension;
            set
            {
                _overrideExtension = value;
                if (value != null)
                {
                    if (!typeof(Extension).IsAssignableFrom(value))
                    {
                        throw new ArgumentException("OverrideExtension must specify the type of an Extension.");
                    }
                    if (!_overrideExtensions.Contains(value))
                        _overrideExtensions.Add(value);
                }
            }
        }

        public ExtensionForAttribute(Type designedItemType) => DesignedItemType = designedItemType ?? throw new ArgumentNullException("designedItemType");
    }
}
