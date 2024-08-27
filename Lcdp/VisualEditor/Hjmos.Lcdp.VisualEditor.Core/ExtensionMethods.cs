using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    public static class ExtensionMethods
    {
        public static double Coerce(this double value, double min, double max) => Math.Max(Math.Min(value, max), min);

        public static void AddRange<T>(this ICollection<T> col, IEnumerable<T> items)
        {
            foreach (T item in items)
            {
                col.Add(item);
            }
        }

        private static bool IsVisual(DependencyObject d) => d is Visual or Visual3D;

        /// <summary>
        /// 获取可视化树中的所有祖先(包括本身)。
        /// 如果<paramref name="visual"/>为空或者不是visual，返回一个空列表。
        /// </summary>
        public static IEnumerable<DependencyObject> GetVisualAncestors(this DependencyObject visual)
        {
            if (IsVisual(visual))
            {
                while (visual != null)
                {
                    yield return visual;
                    visual = VisualTreeHelper.GetParent(visual);
                }
            }
        }

        public static void AddCommandHandler(this UIElement element, ICommand command, Action execute) => AddCommandHandler(element, command, execute, null);

        public static void AddCommandHandler(this UIElement element, ICommand command, Action execute, Func<bool> canExecute)
        {
            CommandBinding cb = new(command);
            if (canExecute != null)
            {
                cb.CanExecute += delegate (object sender, CanExecuteRoutedEventArgs e)
                {
                    e.CanExecute = canExecute();
                    e.Handled = true;
                };
            }
            cb.Executed += delegate (object sender, ExecutedRoutedEventArgs e)
            {
                execute();
                e.Handled = true;
            };
            element.CommandBindings.Add(cb);
        }

        /// <summary>
        /// 用PlacementInformation.BoundsPrecision的位数，舍入矩形位置和大小
        /// </summary>
        public static Rect Round(this Rect rect)
        {
            return new Rect(
                Math.Round(rect.X, PlacementInformation.BoundsPrecision),
                Math.Round(rect.Y, PlacementInformation.BoundsPrecision),
                Math.Round(rect.Width, PlacementInformation.BoundsPrecision),
                Math.Round(rect.Height, PlacementInformation.BoundsPrecision)
            );
        }

        /// <summary>
        /// 获取指定成员描述器的设计项属性
        /// </summary>
        public static DesignItemProperty GetProperty(this DesignItemPropertyCollection properties, MemberDescriptor md)
        {
            DesignItemProperty prop = null;

            if (md is PropertyDescriptor pd)
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(pd);
                if (dpd != null)
                {
                    prop = dpd.IsAttached ? properties.GetAttachedProperty(dpd.DependencyProperty) : properties.GetProperty(dpd.DependencyProperty);
                }
            }

            if (prop == null)
            {
                prop = properties[md.Name];
            }

            return prop;
        }

        /// <summary>
        /// 获取指定的设计项属性是否为附加属性
        /// </summary>
        public static bool IsAttachedDependencyProperty(this DesignItemProperty property)
        {
            if (property.DependencyProperty != null)
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(property.DependencyProperty, property.DesignItem.ComponentType);
                return dpd.IsAttached;
            }

            return false;
        }
    }
}
