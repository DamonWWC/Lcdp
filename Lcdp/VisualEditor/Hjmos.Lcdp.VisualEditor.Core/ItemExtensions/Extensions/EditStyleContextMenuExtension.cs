﻿using System.Reflection;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
    [ExtensionFor(typeof(Control))]
    [Extension(Order = 31)]
    public class EditStyleContextMenuExtension : PrimarySelectionAdornerProvider
    {
        private DesignPanel _designPanel;
        private ContextMenu _contextMenu;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _contextMenu = new EditStyleContextMenu(ExtendedItem);
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
