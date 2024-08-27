using System.ComponentModel;

namespace Hjmos.Lcdp.VisualEditor.Core.Enums
{
    /// <summary>
    /// 排列方向
    /// </summary>
    public enum ArrangeDirection
    {
        [Description("顶部对齐")]
        Top,
        [Description("垂直居中")]
        VerticalMiddle,
        [Description("底部对齐")]
        Bottom,
        [Description("左对齐")]
        Left,
        [Description("水平居中")]
        HorizontalMiddle,
        [Description("右对齐")]
        Right,
    }
}
