using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{

    /// <summary>
    /// 排列设计项的上下文菜单（左对齐、右对齐等）
    /// </summary>
    [ExtensionServer(typeof(PrimarySelectionButOnlyWhenMultipleSelectedExtensionServer))]
    [ExtensionFor(typeof(UIElement))]
    [Extension(Order = 30)]
    public class ArrangeItemsContextMenuExtension : SelectionAdornerProvider
    {
        private DesignPanel _designPanel;
        private ContextMenu _contextMenu;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _contextMenu = new ArrangeItemsContextMenu(ExtendedItem);
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
