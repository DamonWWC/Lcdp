using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using System.Reflection;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
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
