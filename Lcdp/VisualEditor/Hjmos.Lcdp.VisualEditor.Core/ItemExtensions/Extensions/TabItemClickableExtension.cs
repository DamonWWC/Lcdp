using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 使TabItems可点击
    /// </summary>
    [ExtensionFor(typeof(FrameworkElement))]
    [ExtensionServer(typeof(PrimarySelectionExtensionServer))]
    public sealed class TabItemClickableExtension : DefaultExtension
    {
        /// <summary/>
        protected override void OnInitialized()
        {
            // 当选项卡项成为主选择项时，将其设置为其父选项卡控件中的活动选项卡页
            DesignItem t = this.ExtendedItem;
            while (t != null)
            {
                if (t.Component is TabItem)
                {
                    var tabItem = (TabItem)t.Component;
                    var tabControl = tabItem.Parent as TabControl;
                    if (tabControl != null)
                    {
                        tabControl.SelectedItem = tabItem;
                    }
                }
                t = t.Parent;
            }
        }
    }
}
