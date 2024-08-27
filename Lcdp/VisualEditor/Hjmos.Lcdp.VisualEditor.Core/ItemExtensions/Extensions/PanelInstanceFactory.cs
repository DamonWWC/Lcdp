using System;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 用于创建Panel实例的实例工厂。
    /// 将面板的笔刷设置为透明笔刷，并修改面板的类型描述符，以便当使用透明笔刷时，属性值报告为空，将笔刷设置为空实际上恢复了透明笔刷。
    /// </summary>
    [ExtensionFor(typeof(Panel))]
    public sealed class PanelInstanceFactory : CustomInstanceFactory
    {
        private readonly Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);

        /// <summary>
        /// 创建指定类型的实例，并将指定的参数传递给其构造函数。
        /// </summary>
        public override object CreateInstance(Type type, params object[] arguments)
        {
            object instance = base.CreateInstance(type, arguments);
            if (instance is Panel panel)
            {
                if (panel.Background == null)
                {
                    panel.Background = _transparentBrush;
                }
                TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(TypeDescriptor.GetProvider(panel), "Background", _transparentBrush);
                TypeDescriptor.AddProvider(provider, panel);
            }
            return instance;
        }
    }

    [ExtensionFor(typeof(HeaderedContentControl))]
    public sealed class HeaderedContentControlInstanceFactory : CustomInstanceFactory
    {
        readonly Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);

        /// <summary>
        /// 创建指定类型的实例，并将指定的参数传递给其构造函数。
        /// </summary>
        public override object CreateInstance(Type type, params object[] arguments)
        {
            object instance = base.CreateInstance(type, arguments);
            if (instance is Control control)
            {
                if (control.Background == null)
                {
                    control.Background = _transparentBrush;
                }
                TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(TypeDescriptor.GetProvider(control), "Background", _transparentBrush);
                TypeDescriptor.AddProvider(provider, control);
            }
            return instance;
        }
    }

    [ExtensionFor(typeof(ItemsControl))]
    public sealed class TransparentControlsInstanceFactory : CustomInstanceFactory
    {
        readonly Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);

        /// <summary>
        /// 创建指定类型的实例，并将指定的参数传递给其构造函数。
        /// </summary>
        public override object CreateInstance(Type type, params object[] arguments)
        {
            object instance = base.CreateInstance(type, arguments);
            if (instance is Control control && (type == typeof(ItemsControl)))
            {
                if (control.Background == null)
                {
                    control.Background = _transparentBrush;
                }

                TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(TypeDescriptor.GetProvider(control), "Background", _transparentBrush);
                TypeDescriptor.AddProvider(provider, control);
            }
            return instance;
        }
    }

    [ExtensionFor(typeof(Border))]
    public sealed class BorderInstanceFactory : CustomInstanceFactory
    {
        readonly Brush _transparentBrush = new SolidColorBrush(Colors.Transparent);

        /// <summary>
        /// 创建指定类型的实例，并将指定的参数传递给其构造函数。
        /// </summary>
        public override object CreateInstance(Type type, params object[] arguments)
        {
            object instance = base.CreateInstance(type, arguments);
            if (instance is Border panel)
            {
                if (panel.Background == null)
                {
                    panel.Background = _transparentBrush;
                }
                TypeDescriptionProvider provider = new DummyValueInsteadOfNullTypeDescriptionProvider(TypeDescriptor.GetProvider(panel), "Background", _transparentBrush);
                TypeDescriptor.AddProvider(provider, panel);
            }
            return instance;
        }
    }
}
