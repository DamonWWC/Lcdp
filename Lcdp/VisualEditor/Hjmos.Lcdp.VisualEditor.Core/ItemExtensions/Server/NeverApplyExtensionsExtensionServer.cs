using System;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 从不应用其扩展的扩展服务—用于特殊的扩展，如CustomInstanceFactory
    /// </summary>
    internal sealed class NeverApplyExtensionsExtensionServer : ExtensionServer
    {
        public override bool ShouldApplyExtensions(DesignItem extendedItem) => false;

        public override Extension CreateExtension(Type extensionType, DesignItem extendedItem) => throw new NotImplementedException();

        public override void RemoveExtension(Extension extension) => throw new NotImplementedException();

        /// <summary>
        /// 因为事件永远不会引发，所以我们不需要存储事件处理程序
        /// </summary>
        public override event EventHandler<DesignItemCollectionEventArgs> ShouldApplyExtensionsInvalidated
        {
            add { }
            remove { }
        }
    }
}
