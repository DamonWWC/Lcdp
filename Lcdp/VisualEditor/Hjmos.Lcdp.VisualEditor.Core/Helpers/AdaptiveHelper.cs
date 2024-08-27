using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Hjmos.Lcdp.VisualEditor.Core.Helpers
{
    /// <summary>
    /// 组件自适应帮助类
    /// </summary>
    public class AdaptiveHelper
    {
        /// <summary>
        /// 自适应父面板
        /// </summary>
        public static void AdaptiveParent(FrameworkElement element)
        {
            // 清理组件宽高数据，使组件自适应容器
            element.ClearValue(FrameworkElement.WidthProperty);
            element.ClearValue(FrameworkElement.HeightProperty);

            if (element.Parent != null && typeof(Grid).IsAssignableFrom(element.Parent.GetType()))
            {
                element.HorizontalAlignment = HorizontalAlignment.Stretch;
                element.VerticalAlignment = VerticalAlignment.Stretch;
                //// 清理组件宽高数据，使组件自适应容器
                //element.ClearValue(Panel.WidthProperty);
                //element.ClearValue(Panel.HeightProperty);
            }
            else
            {
                Binding heightBinding = new();
                Binding widthBinding = new();

                heightBinding.RelativeSource = new RelativeSource()
                {
                    Mode = RelativeSourceMode.FindAncestor,
                    AncestorType = typeof(Panel)
                };
                heightBinding.Path = new PropertyPath("ActualHeight");

                widthBinding.RelativeSource = new RelativeSource()
                {
                    Mode = RelativeSourceMode.FindAncestor,
                    AncestorType = typeof(Panel)
                };
                widthBinding.Path = new PropertyPath("ActualWidth");

                element.SetBinding(FrameworkElement.HeightProperty, heightBinding);
                element.SetBinding(FrameworkElement.WidthProperty, widthBinding);

                if (element.Parent != null && typeof(Canvas).IsAssignableFrom(element.Parent.GetType()))
                {
                    element.SetValue(Canvas.LeftProperty, 0d);
                    element.SetValue(Canvas.TopProperty, 0d);
                }
            }
        }
    }
}
