using Hjmos.Lcdp.VisualEditor.Core.DesignerControls;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
    [ExtensionFor(typeof(UIElement))]
    [Extension(Order = 50)]
    public class WrapItemContextMenuExtension : SelectionAdornerProvider
    {
        DesignPanel panel;
        ContextMenu contextMenu;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (!(ExtendedItem.View is WindowClone))
            {
                contextMenu = new WrapItemContextMenu(ExtendedItem);
                panel = ExtendedItem.Context.Services.DesignPanel as DesignPanel;
                if (panel != null)
                    panel.AddContextMenu(contextMenu, this.GetType().GetCustomAttribute<ExtensionAttribute>().Order);
            }
        }

        protected override void OnRemove()
        {
            if (panel != null)
                panel.RemoveContextMenu(contextMenu);

            base.OnRemove();
        }
    }
}
