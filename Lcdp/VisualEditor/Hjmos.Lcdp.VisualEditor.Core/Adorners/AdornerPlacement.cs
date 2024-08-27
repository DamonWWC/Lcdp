using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.Adorners
{
    /// <summary>
    /// 定义如何放置设计时装饰器
    /// </summary>
    public abstract class AdornerPlacement
    {
        /// <summary>
        /// 放置实例：使用与内容相同的边界将装饰器放置在内容上方
        /// </summary>
        public static readonly AdornerPlacement FillContent = new FillContentPlacement();

        /// <summary>
        /// 在指定的装饰面板上排列装饰元素。
        /// </summary>
        public abstract void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize);
    }
}
