using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public partial class WrapItemContextMenu
    {
        private readonly DesignItem designItem;

        public WrapItemContextMenu(DesignItem designItem)
        {
            this.designItem = designItem;

            InitializeComponent();
        }

        private void Click_WrapInViewbox(object sender, System.Windows.RoutedEventArgs e)
        {
            ModelTools.WrapItemsNewContainer(this.designItem.Services.Selection.SelectedItems, typeof(Viewbox));
        }
    }
}
