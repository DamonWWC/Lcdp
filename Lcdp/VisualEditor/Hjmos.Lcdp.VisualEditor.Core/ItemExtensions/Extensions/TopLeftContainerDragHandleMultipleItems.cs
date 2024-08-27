using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using Hjmos.Lcdp.VisualEditor.Core.Controls;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 为元素显示的拖动手柄
    /// </summary>
    [ExtensionServer(typeof(PrimarySelectionButOnlyWhenMultipleSelectedExtensionServer))]
    [ExtensionFor(typeof(FrameworkElement))]
    public class TopLeftContainerDragHandleMultipleItems : AdornerProvider
    {
        /// <summary/>
        public TopLeftContainerDragHandleMultipleItems()
        { }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            ContainerDragHandle rect = new();

            rect.PreviewMouseDown += delegate (object sender, MouseButtonEventArgs e)
            {
                //Services.Selection.SetSelectedComponents(new DesignItem[] { this.ExtendedItem }, SelectionTypes.Auto);
                new DragMoveMouseGesture(this.ExtendedItem, false).Start(this.ExtendedItem.Services.DesignPanel, e);
                e.Handled = true;
            };

            ICollection<DesignItem> items = this.ExtendedItem.Services.Selection.SelectedItems;

            double minX = 0;
            double minY = 0;
            double maxX = 0;
            double maxY = 0;

            foreach (DesignItem di in items)
            {
                Point relativeLocation = di.View.TranslatePoint(new Point(0, 0), this.ExtendedItem.View);

                minX = minX < relativeLocation.X ? minX : relativeLocation.X;
                minY = minY < relativeLocation.Y ? minY : relativeLocation.Y;
                maxX = maxX > relativeLocation.X + ((FrameworkElement)di.View).ActualWidth ? maxX : relativeLocation.X + ((FrameworkElement)di.View).ActualWidth;
                maxY = maxY > relativeLocation.Y + ((FrameworkElement)di.View).ActualHeight ? maxY : relativeLocation.Y + ((FrameworkElement)di.View).ActualHeight;
            }

            Rectangle rect2 = new()
            {
                Width = maxX - minX + 4,
                Height = maxY - minY + 4,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection() { 2, 2 },
            };

            RelativePlacement p = new(HorizontalAlignment.Left, VerticalAlignment.Top)
            {
                XOffset = minX - 3,
                YOffset = minY - 3
            };

            RelativePlacement p2 = new(HorizontalAlignment.Left, VerticalAlignment.Top)
            {
                XOffset = minX + rect2.Width - 2,
                YOffset = minY + rect2.Height - 2
            };

            AddAdorner(p, AdornerOrder.Background, rect);
            AddAdorner(p2, AdornerOrder.Background, rect2);
        }
    }
}
