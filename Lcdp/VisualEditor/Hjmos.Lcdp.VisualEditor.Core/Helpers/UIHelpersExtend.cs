using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.Helpers
{
    public static class UIHelpers
    {
        /// <summary>
        /// 返回在可视树中指定类型的所有元素
        /// </summary>
        /// <param name="parent">开始搜索的父元素</param>
        /// <returns>在可视树中找到指定类型的所有子元素，如果没有找到指定类型的父元素，则为空</returns>
        public static IEnumerable<T> TryFindAllChildWidget<T>(this DependencyObject parent) where T : IWidget
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t)
                {
                    yield return t;
                }

                IEnumerable<T> grandChilds;

                if (child is ContentControl contentControl && contentControl.Content is UIElement content)
                {
                    // TODO: 为什么首次加载不到ContentControl中的控件，再次加载就行？难道是注册了Name的原因？
                    // 特殊处理下ContentControl
                    grandChilds = content.TryFindAllChildWidget<T>();
                }
                else
                {
                    grandChilds = child.TryFindAllChildWidget<T>();
                }

                foreach (var grandChild in grandChilds)
                {
                    yield return grandChild;
                }
            }
        }
    }
}
