using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public partial class UnwrapItemContextMenu
    {
        private readonly DesignItem designItem;

        public UnwrapItemContextMenu(DesignItem designItem)
        {
            this.designItem = designItem;

            this.InitializeComponent();
        }

        void Click_Unwrap(object sender, RoutedEventArgs e) => ModelTools.UnwrapItemsFromContainer(this.designItem.Services.Selection.PrimarySelection);
    }
}
