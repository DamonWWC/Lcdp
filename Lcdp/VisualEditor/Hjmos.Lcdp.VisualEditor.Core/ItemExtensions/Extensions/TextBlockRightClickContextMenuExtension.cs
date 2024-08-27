using System.Reflection;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
    [ExtensionFor(typeof(TextBlock))]
    [Extension(Order = 40)]
    public class TextBlockRightClickContextMenuExtension : PrimarySelectionAdornerProvider
    {
        private DesignPanel _designPanel;
        private ContextMenu _contextMenu;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _contextMenu = new TextBlockRightClickContextMenu(ExtendedItem);
            _designPanel = ExtendedItem.Context.Services.DesignPanel as DesignPanel;
            if (_designPanel != null)
                _designPanel.AddContextMenu(_contextMenu, this.GetType().GetCustomAttribute<ExtensionAttribute>().Order);
        }

        protected override void OnRemove()
        {
            if (_designPanel != null)
                _designPanel.RemoveContextMenu(_contextMenu);

            base.OnRemove();
        }
    }
}
