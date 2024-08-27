using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Adorners
{
    internal sealed class FillContentPlacement : AdornerPlacement
    {
        public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize) => adorner.Arrange(new Rect(adornedElementSize));
    }
}
