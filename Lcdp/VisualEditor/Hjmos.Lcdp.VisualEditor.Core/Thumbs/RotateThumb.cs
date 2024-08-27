using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.Thumbs
{
    public class RotateThumb : DesignerThumb
    {
        static RotateThumb() => DefaultStyleKeyProperty.OverrideMetadata(typeof(RotateThumb), new FrameworkPropertyMetadata(typeof(RotateThumb)));

        public RotateThumb() => ThumbVisible = true;
    }
}
