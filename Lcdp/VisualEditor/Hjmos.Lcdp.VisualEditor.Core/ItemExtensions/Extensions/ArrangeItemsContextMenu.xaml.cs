using Hjmos.Lcdp.VisualEditor.Core.Enums;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public partial class ArrangeItemsContextMenu
    {
        private readonly DesignItem designItem;

        public ArrangeItemsContextMenu(DesignItem designItem)
        {
            this.designItem = designItem;

            this.InitializeComponent();
        }

        private void Click_ArrangeLeft(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.Left);
        }

        private void Click_ArrangeHorizontalCentered(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.HorizontalMiddle);
        }

        private void Click_ArrangeRight(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.Right);
        }

        private void Click_ArrangeTop(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.Top);
        }

        private void Click_ArrangeVerticalCentered(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.VerticalMiddle);
        }

        private void Click_ArrangeBottom(object sender, RoutedEventArgs e)
        {
            ModelTools.ArrangeItems(this.designItem.Services.Selection.SelectedItems, ArrangeDirection.Bottom);
        }
    }
}
