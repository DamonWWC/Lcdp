using System.Reflection;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionServer(typeof(PrimarySelectionExtensionServer))]
    [ExtensionFor(typeof(object))]
    [Extension(Order = 10)]
    public class DefaultCommandsContextMenuExtension : SelectionAdornerProvider
    {
        DesignPanel panel;
        ContextMenu contextMenu;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            contextMenu = new DefaultCommandsContextMenu(ExtendedItem);
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
