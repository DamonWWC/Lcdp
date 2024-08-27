using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    public partial class WrapItemsContextMenu
    {
        private DesignItem designItem;

        public WrapItemsContextMenu(DesignItem designItem)
        {
            this.designItem = designItem;

            InitializeComponent();
        }

        private void Click_WrapInCanvas(object sender, System.Windows.RoutedEventArgs e)
        {
            ModelTools.WrapItemsNewContainer(this.designItem.Services.Selection.SelectedItems, typeof(Canvas));
        }

        void Click_WrapInGrid(object sender, System.Windows.RoutedEventArgs e)
        {
            ModelTools.WrapItemsNewContainer(this.designItem.Services.Selection.SelectedItems, typeof(Grid));
        }
    }
}
