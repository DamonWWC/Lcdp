using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.Extensions
{
    /// <summary>
    /// UIElement静态扩展类
    /// </summary>
    public static class UIElementExtension
    {
        /// <summary>
        /// 获取Canvas附加属性Top的值
        /// </summary>
        public static double GetCanvasTop(this UIElement element) => double.IsNaN(Canvas.GetTop(element)) ? 0 : Canvas.GetTop(element);

        /// <summary>
        /// 获取Canvas附加属性Left的值
        /// </summary>
        public static double GetCanvasLeft(this UIElement element) => double.IsNaN(Canvas.GetLeft(element)) ? 0 : Canvas.GetLeft(element);

        /// <summary>
        /// 获取Canvas附加属性Right的值
        /// </summary>
        public static double GetCanvasRight(this UIElement element) => double.IsNaN(Canvas.GetRight(element)) ? 0 : Canvas.GetRight(element);

        /// <summary>
        /// 获取Canvas附加属性Bottom的值
        /// </summary>
        public static double GetCanvasBottom(this UIElement element) => double.IsNaN(Canvas.GetBottom(element)) ? 0 : Canvas.GetBottom(element);
    }
}
