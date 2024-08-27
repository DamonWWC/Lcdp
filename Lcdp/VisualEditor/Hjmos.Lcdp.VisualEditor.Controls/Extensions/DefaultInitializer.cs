namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions
{
    /// <summary>
    /// 用默认值初始化新控件的扩展的基类。
    /// </summary>
    [ExtensionServer(typeof(NeverApplyExtensionsExtensionServer))]
    public abstract class DefaultInitializer : Extension
    {
        /// <summary>
        /// 将设计项初始化为默认值。
        /// </summary>
        public abstract void InitializeDefaults(DesignItem item);
    }
}
