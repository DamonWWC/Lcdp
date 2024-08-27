using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerControls
{
    /// <summary>
    /// A Small icon which shows up a menu containing common properties
    /// 显示包含公共属性的菜单的小图标
    /// </summary>
    public class QuickOperationMenu : Control
    {
        static QuickOperationMenu() => DefaultStyleKeyProperty.OverrideMetadata(typeof(QuickOperationMenu), new FrameworkPropertyMetadata(typeof(QuickOperationMenu)));

        public QuickOperationMenu() { }

        /// <summary>
        /// Contains Default values in the Sub menu for example "HorizontalAlignment" has "HorizontalAlignment.Stretch" as it's value.
        /// 包含子菜单中的默认值，例如"HorizontalAlignment"具有"HorizontalAlignment.Stretch"的值。
        /// </summary>
        private readonly Dictionary<MenuItem, MenuItem> _defaults = new();

        /// <summary>
        /// Is the main header menu which brings up all the menus.
        /// 显示所有菜单的主标题菜单。
        /// </summary>
        public MenuItem MainHeader { get; private set; }

        /// <summary>
        /// Add a submenu with checkable values.
        /// 添加具有可检查值的子菜单。
        /// </summary>
        /// <param name="parent">The parent menu under which to add.</param>
        /// <param name="enumValues">All the values of an enum to be showed in the menu</param>
        /// <param name="defaultValue">The default value out of all the enums.</param>
        /// <param name="setValue">The presently set value out of the enums</param>
        public void AddSubMenuCheckable(MenuItem parent, Array enumValues, string defaultValue, string setValue)
        {
            foreach (object enumValue in enumValues)
            {
                MenuItem menuItem = new() { Header = enumValue.ToString(), IsCheckable = true };
                parent.Items.Add(menuItem);
                if (enumValue.ToString() == defaultValue)
                    _defaults.Add(parent, menuItem);
                if (enumValue.ToString() == setValue)
                    menuItem.IsChecked = true;
            }
        }

        /// <summary>
        /// 在主标题中添加一个菜单
        /// </summary>
        /// <param name="menuItem">添加的菜单</param>
        public void AddSubMenuInTheHeader(MenuItem menuItem)
        {
            if (MainHeader != null)
                MainHeader.Items.Add(menuItem);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            if (Template.FindName("MainHeader", this) is MenuItem mainHeader)
            {
                MainHeader = mainHeader;
            }
        }

        /// <summary>
        /// 检查菜单项并使其具有排他性。如果勾选，则选择默认菜单项。
        /// </summary>
        /// <param name="parent">子菜单的父项</param>
        /// <param name="clickedOn">点击的菜单项</param>
        /// <returns>如果可检查菜单项被切换，或新选中的菜单项被切换，则返回默认值</returns>
        public string UncheckChildrenAndSelectClicked(MenuItem parent, MenuItem clickedOn)
        {
            _defaults.TryGetValue(parent, out MenuItem defaultMenuItem);
            if (IsAnyItemChecked(parent))
            {
                foreach (object item in parent.Items)
                {
                    if (item is MenuItem menuItem) menuItem.IsChecked = false;
                }
                clickedOn.IsChecked = true;
                return (string)clickedOn.Header;
            }
            else
            {
                if (defaultMenuItem != null)
                {
                    defaultMenuItem.IsChecked = true;
                    return (string)defaultMenuItem.Header;
                }
            }
            return null;
        }

        /// <summary>
        /// 在子菜单中检查某个项是否已被选中
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private bool IsAnyItemChecked(MenuItem parent)
        {
            bool check = false;
            if (parent.HasItems)
            {
                foreach (object item in parent.Items)
                {
                    if (item is MenuItem menuItem && menuItem.IsChecked)
                        check = true;
                }
            }
            return check;
        }
    }
}
