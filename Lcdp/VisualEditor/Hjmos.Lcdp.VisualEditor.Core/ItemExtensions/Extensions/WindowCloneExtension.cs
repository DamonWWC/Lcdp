using Hjmos.Lcdp.VisualEditor.Core.DesignerControls;
using System;
using System.Diagnostics;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// A <see cref="CustomInstanceFactory"/> for <see cref="Window"/>
    /// (and derived classes, unless they specify their own <see cref="CustomInstanceFactory"/>).
    /// </summary>
    [ExtensionFor(typeof(Window))]
    public class WindowCloneExtension : CustomInstanceFactory
    {
        /// <summary>
        /// 用于创建<see cref="WindowClone"/>的实例。
        /// </summary>
        public override object CreateInstance(Type type, params object[] arguments)
        {
            Debug.Assert(arguments.Length == 0);
            return new WindowClone();
        }
    }
}
