using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions
{
    /// <summary>
    /// 创建派生自<see cref="DefaultExtension"/>的扩展服务的基类。
    /// </summary>
    public abstract class DefaultExtensionServer : ExtensionServer
    {
        /// <summary>
        /// 创建DefaultExtension的一个实例并在其上调用OnInitialize。
        /// </summary>
        public override Extension CreateExtension(Type extensionType, DesignItem extendedItem)
        {
            DefaultExtension ext = (DefaultExtension)Activator.CreateInstance(extensionType);
            ext.InitializeDefaultExtension(extendedItem);
            return ext;
        }

        /// <summary>
        /// 调用DefaultExtension上的OnRemove()。
        /// </summary>
        public override void RemoveExtension(Extension extension)
        {
            DefaultExtension defaultExtension = extension as DefaultExtension;
            Debug.Assert(defaultExtension != null);
            defaultExtension.CallOnRemove();
        }

        /// <summary>
        /// 当一组项的ShouldApplyExtensions失效时将引发此事件。
        /// </summary>
        public override event EventHandler<DesignItemCollectionEventArgs> ShouldApplyExtensionsInvalidated;

        /// <summary>
        /// 为指定的设计项集合引发ShouldApplyExtensionsInvalidated事件。
        /// </summary>
        protected void ReapplyExtensions(ICollection<DesignItem> items)
        {
            // 注释这个方法后，组件就没有装饰层了，组件从这个方法开始附加装饰层
            // 鼠标在设计界面移动也会触发这个方法，比较难调试，这个方法执行是跳到ExtensionManager.GetExtensionServer中的一个匿名委托，里面执行的是ExtensionManager.ReapplyExtensions方法
            ShouldApplyExtensionsInvalidated?.Invoke(this, new DesignItemCollectionEventArgs(items));
        }

        internal sealed class Permanent : DefaultExtensionServer
        {
            public override bool ShouldApplyExtensions(DesignItem extendedItem) => true;
        }

        // special extension server like 'permanent' - skips applying extensions if there is no design panel (e.g. in designer unit tests).
        // 特殊的扩展服务器，如“永久”-跳过应用扩展，如果没有设计面板(例如在设计器单元测试)。  
        internal sealed class PermanentWithDesignPanel : DefaultExtensionServer
        {
            public override bool ShouldApplyExtensions(DesignItem extendedItem) => Services.GetService(typeof(IDesignPanel)) != null;
        }
    }
}
