using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public class PropertyItemsControl : ListBox
    {
        protected override bool IsItemItsOwnContainerOverride(object item) => item is PropertyItem;

        public PropertyItemsControl()
        {
#if !NET40
            VirtualizingPanel.SetIsVirtualizingWhenGrouping(this, true);
            VirtualizingPanel.SetScrollUnit(this, ScrollUnit.Pixel);


#endif
        }
    }
}
