using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    [ExtensionServer(typeof(PrimarySelectionExtensionServer))]
    [ExtensionFor(typeof(Path))]
    [Extension(Order = 70)]
    public class PathContextMenuExtension : SelectionAdornerProvider
    {
        DesignPanel panel;
        ContextMenu contextMenu;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            contextMenu = new PathContextMenu(ExtendedItem);
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
