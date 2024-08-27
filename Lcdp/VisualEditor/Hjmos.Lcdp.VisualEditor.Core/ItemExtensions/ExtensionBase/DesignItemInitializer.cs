namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// "DefaultInitializer"只在新对象中被调用，这个Initializer在每个设计项的实例中被调用
    /// </summary>
    [ExtensionServer(typeof(NeverApplyExtensionsExtensionServer))]
    public abstract class DesignItemInitializer : Extension
    {
        /// <summary>初始化设计项</summary>
        public abstract void InitializeDesignItem(DesignItem item);
    }
}