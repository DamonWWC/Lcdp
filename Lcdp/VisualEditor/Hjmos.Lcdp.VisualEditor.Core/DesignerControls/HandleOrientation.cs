namespace Hjmos.Lcdp.VisualEditor.Core.DesignerControls
{
    /// <summary>
    /// 指定手柄方向
    /// 手柄的旋转是右方向、顺时针方向的
    /// </summary>
    public enum HandleOrientation
    {
        /// <summary>指示边距句柄是向左的，并相对于<see cref="Right"/>旋转180度</summary>
        Left = 180,
        /// <summary>指示边距手柄是向上的，并相对于<see cref="Right"/>旋转270度</summary>
        Top = 270,
        /// <summary>指示边距手柄是向右的</summary>
        Right = 0,
        /// <summary>指示边距手柄是向左的，并相对于<see cref="Right"/>旋90度</summary>
        Bottom = 90
    }
}
