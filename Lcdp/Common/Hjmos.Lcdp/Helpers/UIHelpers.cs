using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Hjmos.Lcdp.Helpers
{
    /// <summary>
    /// UI相关的帮助方法
    /// </summary>
    public static class UIHelpers
    {
        /// <summary>
        /// 获取父元素。从哪棵树检索父元素取决于参数。
        /// </summary>
        /// <param name="child">要获取父元素的子元素</param>
        /// <param name="searchCompleteVisualTree">如果为真，则返回可视树中的父元素，如果为假，则根据子类型从另一棵树中检索父元素</param>
        /// <returns>父元素，根据参数从可视树、逻辑树或其他树检索</returns>
        public static DependencyObject GetParentObject(this DependencyObject child, bool searchCompleteVisualTree)
        {
            if (child == null) return null;

            if (!searchCompleteVisualTree)
            {
                if (child is ContentElement contentElement)
                {
                    DependencyObject parent = ContentOperations.GetParent(contentElement);
                    if (parent != null) return parent;

                    return contentElement is FrameworkContentElement fce ? fce.Parent : null;
                }

                if (child is FrameworkElement frameworkElement)
                {
                    DependencyObject parent = frameworkElement.Parent;
                    if (parent != null) return parent;
                }
            }

            return VisualTreeHelper.GetParent(child);
        }

        /// <summary>
        /// 获取指定类型的第一个父元素。从哪棵树检索父节点取决于参数
        /// </summary>
        /// <param name="child">要获取父元素的子元素</param>
        /// <param name="searchCompleteVisualTree">如果为真，则返回可视树中的父元素，如果为假，则根据子类型从另一棵树中检索父元素</param>
        /// <returns>父元素，根据参数从可视树、逻辑树或其他树检索，如果没有找到指定类型的父元素，则为空</returns>
        public static T TryFindParent<T>(this DependencyObject child, bool searchCompleteVisualTree = false) where T : DependencyObject
        {
            DependencyObject parentObject = GetParentObject(child, searchCompleteVisualTree);

            if (parentObject == null) return null;

            T parent = parentObject as T;
            return parent ?? TryFindParent<T>(parentObject);
        }

        /// <summary>
        /// 返回在可视树中找到的指定类型的第一个子元素
        /// </summary>
        /// <param name="parent">开始搜索的父元素</param>
        /// <returns>在可视树中找到指定类型的第一个子元素，如果没有找到指定类型的父元素，则为空</returns>
        public static T TryFindChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t)
                {
                    return t;
                }
                child = TryFindChild<T>(child);
                if (child != null)
                {
                    return (T)child;
                }
            }
            return null;
        }

        /// <summary>
        /// 返回在可视树中指定类型的所有元素
        /// </summary>
        /// <param name="parent">开始搜索的父元素</param>
        /// <returns>在可视树中找到指定类型的所有子元素，如果没有找到指定类型的父元素，则为空</returns>
        public static IEnumerable<T> TryFindAllChild<T>(this DependencyObject parent) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child is T t)
                {
                    yield return t;
                }

                IEnumerable<T> grandChilds = TryFindAllChild<T>(child);

                foreach (var grandChild in grandChilds)
                {
                    yield return grandChild;
                }
            }
        }

        /// <summary>
        /// 返回在可视树中找到的指定类型和指定名称的第一个子元素。
        /// </summary>
        /// <param name="parent">开始搜索的父元素。</param>
        /// <param name="childName">要查找的子元素的名称，空字符串或null表示仅查看类型的。</param>
        /// <returns>匹配指定类型和名称的第一个子元素，如果没有匹配，则为空。</returns>
        public static T TryFindChild<T>(this DependencyObject parent, string childName) where T : DependencyObject
        {
            if (parent == null) return null;
            T foundChild = null;
            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childrenCount; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (!(child is T t))
                {
                    foundChild = TryFindChild<T>(child, childName);
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    if (child is FrameworkElement frameworkElement && frameworkElement.Name == childName)
                    {
                        foundChild = t;
                        break;
                    }
                }
                else
                {
                    foundChild = t;
                    break;
                }
            }
            return foundChild;
        }

        /// <summary>
        /// 返回指定类型的第一个祖先
        /// </summary>
        public static T FindAncestor<T>(DependencyObject current) where T : DependencyObject
        {
            current = GetVisualOrLogicalParent(current);

            while (current != null)
            {
                if (current is T t)
                {
                    return t;
                }
                current = GetVisualOrLogicalParent(current);
            }

            return null;
        }

        private static DependencyObject GetVisualOrLogicalParent(DependencyObject obj)
        {
            if (obj is Visual || obj is Visual3D)
            {
                return VisualTreeHelper.GetParent(obj);
            }
            return LogicalTreeHelper.GetParent(obj);
        }
    }
}
