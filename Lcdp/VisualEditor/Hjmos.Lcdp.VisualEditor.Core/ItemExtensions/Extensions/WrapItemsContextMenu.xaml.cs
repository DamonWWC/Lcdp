using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public partial class WrapItemsContextMenu
    {
        private readonly DesignItem _designItem;

        public WrapItemsContextMenu(DesignItem designItem)
        {
            this._designItem = designItem;

            InitializeComponent();
        }

        private void Click_WrapInCanvas(object sender, System.Windows.RoutedEventArgs e)
        {
            ModelTools.WrapItemsNewContainer(this._designItem.Services.Selection.SelectedItems, typeof(Canvas));
        }

        private void Click_WrapInGrid(object sender, System.Windows.RoutedEventArgs e)
        {
            ModelTools.WrapItemsNewContainer(this._designItem.Services.Selection.SelectedItems, typeof(Grid));
        }
    }
}
