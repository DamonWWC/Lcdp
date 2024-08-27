using Hjmos.Lcdp.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.DesignerPropertyGrid.Editors.FormatedTextEditor;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public partial class TextBlockRightClickContextMenu
    {
        private DesignItem designItem;

        public TextBlockRightClickContextMenu(DesignItem designItem)
        {
            this.designItem = designItem;

            InitializeComponent();
        }

        private void Click_EditFormatedText(object sender, RoutedEventArgs e)
        {
            Window dlg = new()
            {
                Content = new FormatedTextEditor(designItem),
                Width = 440,
                Height = 200,
                WindowStyle = WindowStyle.ToolWindow,
                Owner = ((DesignPanel)designItem.Context.Services.DesignPanel).TryFindParent<Window>(),
            };

            dlg.ShowDialog();
        }
    }
}
