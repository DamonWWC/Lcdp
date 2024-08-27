using Hjmos.Lcdp.Common;
using Prism.Events;

namespace Hjmos.Lcdp.VisualEditor.Core.Events
{
    /// <summary>
    /// 用于通知编辑器加载新页面
    /// </summary>
    public class SwitchPageEvent : PubSubEvent<IEventParameters> { }
}
