using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionServer(typeof(PrimarySelectionButOnlyWhenMultipleSelectedExtensionServer))]
    [ExtensionFor(typeof(UIElement))]
    [Extension(Order = 52)]
    public class WrapItemsContextMenuExtension : SelectionAdornerProvider
    {
        DesignPanel panel;
        ContextMenu contextMenu;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            contextMenu = new WrapItemsContextMenu(ExtendedItem);
            panel = ExtendedItem.Context.Services.DesignPanel as DesignPanel;
            if (panel != null)
                panel.AddContextMenu(contextMenu, this.GetType().GetCustomAttribute<ExtensionAttribute>().Order);
        }

        protected override void OnRemove()
        {
            if (panel != null)
                panel.RemoveContextMenu(contextMenu);

            base.OnRemove();
        }
    }
}
