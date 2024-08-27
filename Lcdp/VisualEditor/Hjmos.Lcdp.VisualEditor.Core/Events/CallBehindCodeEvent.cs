using Hjmos.Lcdp.Common;
using Prism.Events;

namespace Hjmos.Lcdp.VisualEditor.Core.Events
{
    /// <summary>
    /// 用于组件和ViewModel传递消息
    /// </summary>
    public class CallBehindCodeEvent : PubSubEvent<IEventParameters> { }
}
