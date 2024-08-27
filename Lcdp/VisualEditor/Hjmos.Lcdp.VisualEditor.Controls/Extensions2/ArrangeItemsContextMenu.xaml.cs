using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    public partial class ArrangeItemsContextMenu
    {
        private DesignItem designItem;

        public ArrangeItemsContextMenu(DesignItem designItem)
        {
            this.designItem = designItem;

            this.InitializeComponent();
        }

        void Click_ArrangeLeft(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.Left);
        }

        void Click_ArrangeHorizontalCentered(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.HorizontalMiddle);
        }

        void Click_ArrangeRight(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.Right);
        }

        void Click_ArrangeTop(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.Top);
        }

        void Click_ArrangeVerticalCentered(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.VerticalMiddle);
        }

        void Click_ArrangeBottom(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.Bottom);
        }
    }
}
