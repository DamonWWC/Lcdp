using Hjmos.Lcdp.VisualEditor.Core.DesignerPropertyGrid;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public class MyComponentPropertyService : IComponentPropertyService
    {
        public IEnumerable<MemberDescriptor> GetAvailableEvents(DesignItem designItem)
        {
            //We don't want to show events for our design items, so probably throw an exception here?
            // 我们不想为我们的设计项目显示事件，所以可能在这里抛出一个异常?  
            IEnumerable<EventDescriptor> retVal = TypeHelper.GetAvailableEvents(designItem.ComponentType);
            return retVal;
        }

        public IEnumerable<MemberDescriptor> GetAvailableProperties(DesignItem designItem)
        {
            IEnumerable<PropertyDescriptor> retVal = TypeHelper.GetAvailableProperties(designItem.Component);

            retVal = retVal.Where(c => c.Name == "Foreground" || c.Name == "MyStringProperty");

            return retVal;
        }

        public IEnumerable<MemberDescriptor> GetCommonAvailableProperties(IEnumerable<DesignItem> designItems)
        {
            IEnumerable<PropertyDescriptor> retVal = TypeHelper.GetCommonAvailableProperties(designItems.Select(t => t.Component));

            return retVal;
        }
    }
}
