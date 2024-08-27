using Hjmos.Lcdp.VisualEditor.Core.Interface;
using System;
using System.ComponentModel;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions
{
    /// <summary>
    /// A special kind of extension that is used to create instances of objects when loading XAML inside the designer.
    /// 一种特殊类型的扩展，用于在设计器中加载组件时创建对象实例。
    /// </summary>
    /// <remarks>
    /// Cider中的CustomInstanceFactory: https://docs.microsoft.com/en-us/archive/blogs/jnak/cider-item-creation
    /// </remarks>
    [ExtensionServer(typeof(NeverApplyExtensionsExtensionServer))]
    public class CustomInstanceFactory : Extension
    {
        /// <summary>
        /// 获取使用Activator.CreateInstance创建实例的默认实例工厂
        /// </summary>
        public static readonly CustomInstanceFactory DefaultInstanceFactory = new();

        protected CustomInstanceFactory() { }

        /// <summary>
        /// 创建指定类型的实例，将指定的参数传递给其构造函数。  
        /// </summary>
        public virtual object CreateInstance(Type type, params object[] arguments)
        {
            object instance = Activator.CreateInstance(type, arguments);

            // TODO：这里是没有自定义实例化工厂时的默认实例化

            // 初始化组件
            if (instance is IWidget widget)
            {
                widget.Init();
            }
            
            if (instance is UIElement uiElement)
                DesignerProperties.SetIsInDesignMode(uiElement, true);
            return instance;
        }
    }
}
