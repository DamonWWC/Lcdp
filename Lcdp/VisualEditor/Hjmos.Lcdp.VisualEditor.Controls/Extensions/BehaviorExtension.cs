namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions
{
    /// <summary>
    /// 为设计项提供行为接口的扩展的基类。
    /// 这些扩展总是被加载的。它们必须有一个无参数的构造函数。
    /// </summary>
    [ExtensionServer(typeof(DefaultExtensionServer.Permanent))]
    public class BehaviorExtension : DefaultExtension { }
}
