using System.Windows;
using System.Windows.Controls.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    /// <summary>
    /// 渲染变换原点的Thumb
    /// </summary>
    public class RenderTransformOriginThumb : Thumb
    {
        static RenderTransformOriginThumb() => DefaultStyleKeyProperty.OverrideMetadata(typeof(RenderTransformOriginThumb), new FrameworkPropertyMetadata(typeof(RenderTransformOriginThumb)));
    }
}
