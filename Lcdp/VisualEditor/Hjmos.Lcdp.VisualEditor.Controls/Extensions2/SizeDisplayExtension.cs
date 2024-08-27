using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.DesignerControls;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    /// <summary>
    /// 在主选择上显示高度/宽度
    /// </summary>
    [ExtensionFor(typeof(UIElement))]
    public class SizeDisplayExtension : PrimarySelectionAdornerProvider
    {
        public HeightDisplay HeightDisplay { get; private set; }

        public WidthDisplay WidthDisplay { get; private set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (this.ExtendedItem != null)
            {
                RelativePlacement placementHeight = new RelativePlacement(HorizontalAlignment.Right, VerticalAlignment.Stretch);
                placementHeight.XOffset = 10;
                HeightDisplay = new HeightDisplay();
                HeightDisplay.DataContext = this.ExtendedItem.Component;

                RelativePlacement placementWidth = new RelativePlacement(HorizontalAlignment.Stretch, VerticalAlignment.Bottom);
                placementWidth.YOffset = 10;
                WidthDisplay = new WidthDisplay();
                WidthDisplay.DataContext = this.ExtendedItem.Component;

                this.AddAdorners(placementHeight, HeightDisplay);
                this.AddAdorners(placementWidth, WidthDisplay);
                HeightDisplay.Visibility = Visibility.Hidden;
                WidthDisplay.Visibility = Visibility.Hidden;
            }
        }
    }
}
