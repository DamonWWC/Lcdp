using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    [ExtensionFor(typeof(Panel))]
    [ExtensionFor(typeof(Border))]
    [ExtensionFor(typeof(ContentControl))]
    [ExtensionFor(typeof(Viewbox))]
    public class PanelMove : PermanentAdornerProvider
    {
        protected override void OnInitialized()
        {
            base.OnInitialized();

            AdornerPanel adornerPanel = new();
            PanelMoveAdorner adorner = new(ExtendedItem);
            AdornerPanel.SetPlacement(adorner, AdornerPlacement.FillContent);
            adornerPanel.Children.Add(adorner);
            Adorners.Add(adornerPanel);
        }
    }
}
