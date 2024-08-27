using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.DesignerControls;
using Hjmos.Lcdp.VisualEditor.Controls.DesignerPropertyGrid;
using Hjmos.Lcdp.VisualEditor.Controls.DesignerPropertyGrid.Editors;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions2
{
    /// <summary>
    /// 扩展设计器的快速操作菜单
    /// </summary>
    [ExtensionServer(typeof(OnlyOneItemSelectedExtensionServer))]
    [ExtensionFor(typeof(FrameworkElement))]
    public class QuickOperationMenuExtension : PrimarySelectionAdornerProvider
    {
        private QuickOperationMenu _menu;
        private KeyBinding _keyBinding;

        protected override void OnInitialized()
        {
            base.OnInitialized();
            _menu = new QuickOperationMenu();
            _menu.Loaded += OnMenuLoaded;
            _menu.RenderTransform = (Transform)this.ExtendedItem.GetCompleteAppliedTransformationToView().Inverse;
            RelativePlacement placement = new(HorizontalAlignment.Right, VerticalAlignment.Top) { XOffset = 7, YOffset = 3.5 };
            this.AddAdorners(placement, _menu);

            RelayCommand command = new(delegate
            {
                _menu.MainHeader.IsSubmenuOpen = true; _menu.MainHeader.Focus();
            });
            _keyBinding = new KeyBinding(command, Key.Enter, ModifierKeys.Alt);
            if (this.ExtendedItem.Services.GetService(typeof(IKeyBindingService)) is IKeyBindingService kbs)
                kbs.RegisterBinding(_keyBinding);
        }

        private void OnMenuLoaded(object sender, EventArgs e)
        {
            if (_menu.MainHeader != null)
                _menu.MainHeader.Click += MainHeaderClick;

            int menuItemsAdded = 0;
            var view = this.ExtendedItem.View;

            if (view != null)
            {
                string setValue;
                if (view is ItemsControl)
                {
                    _menu.AddSubMenuInTheHeader(new MenuItem() { Header = "编辑项" });
                }

                if (view is Grid)
                {
                    _menu.AddSubMenuInTheHeader(new MenuItem() { Header = "编辑行" });

                    _menu.AddSubMenuInTheHeader(new MenuItem() { Header = "编辑列" });
                }

                if (view is StackPanel)
                {
                    var ch = new MenuItem() { Header = "改变方向" };
                    _menu.AddSubMenuInTheHeader(ch);
                    setValue = this.ExtendedItem.Properties[StackPanel.OrientationProperty].ValueOnInstance.ToString();
                    _menu.AddSubMenuCheckable(ch, Enum.GetValues(typeof(Orientation)), Orientation.Vertical.ToString(), setValue);
                    _menu.MainHeader.Items.Add(new Separator());
                    menuItemsAdded++;
                }

                if (this.ExtendedItem.Parent != null && this.ExtendedItem.Parent.View is DockPanel)
                {
                    var sda = new MenuItem() { Header = "停靠" };
                    _menu.AddSubMenuInTheHeader(sda);
                    setValue = this.ExtendedItem.Properties.GetAttachedProperty(DockPanel.DockProperty).ValueOnInstance.ToString();
                    _menu.AddSubMenuCheckable(sda, Enum.GetValues(typeof(Dock)), Dock.Left.ToString(), setValue);
                    _menu.MainHeader.Items.Add(new Separator());
                    menuItemsAdded++;
                }

                var ha = new MenuItem() { Header = "水平对齐" };
                _menu.AddSubMenuInTheHeader(ha);
                setValue = this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].ValueOnInstance.ToString();
                _menu.AddSubMenuCheckable(ha, Enum.GetValues(typeof(HorizontalAlignment)), HorizontalAlignment.Stretch.ToString(), setValue);
                menuItemsAdded++;

                var va = new MenuItem() { Header = "垂直对齐" };
                _menu.AddSubMenuInTheHeader(va);
                setValue = this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].ValueOnInstance.ToString();
                _menu.AddSubMenuCheckable(va, Enum.GetValues(typeof(VerticalAlignment)), VerticalAlignment.Stretch.ToString(), setValue);
                menuItemsAdded++;
            }

            if (menuItemsAdded == 0)
            {
                OnRemove();
            }
        }

        private void MainHeaderClick(object sender, RoutedEventArgs e)
        {
            MenuItem clickedOn = e.Source as MenuItem;
            if (clickedOn != null)
            {
                MenuItem parent = clickedOn.Parent as MenuItem;
                if (parent != null)
                {

                    if ((string)clickedOn.Header == "编辑项")
                    {
                        CollectionEditor editor = new();
                        ItemsControl itemsControl = this.ExtendedItem.View as ItemsControl;
                        if (itemsControl != null)
                            editor.LoadItemsCollection(this.ExtendedItem);
                        editor.Show();
                    }

                    if ((string)clickedOn.Header == "编辑行")
                    {
                        FlatCollectionEditor editor = new();
                        Grid gd = this.ExtendedItem.View as Grid;
                        if (gd != null)
                            editor.LoadItemsCollection(this.ExtendedItem.Properties["RowDefinitions"]);
                        editor.Show();
                    }

                    if ((string)clickedOn.Header == "编辑列")
                    {
                        FlatCollectionEditor editor = new();
                        if (this.ExtendedItem.View is Grid)
                            editor.LoadItemsCollection(this.ExtendedItem.Properties["ColumnDefinitions"]);
                        editor.Show();
                    }

                    if (parent.Header is string header)
                    {
                        if (header == "改变方向")
                        {
                            var value = _menu.UncheckChildrenAndSelectClicked(parent, clickedOn);
                            if (value != null)
                            {
                                var orientation = Enum.Parse(typeof(Orientation), value);
                                if (orientation != null)
                                    this.ExtendedItem.Properties[StackPanel.OrientationProperty].SetValue(orientation);
                            }
                        }

                        if (header == "停靠")
                        {
                            var value = _menu.UncheckChildrenAndSelectClicked(parent, clickedOn);
                            if (value != null)
                            {
                                var dock = Enum.Parse(typeof(Dock), value);
                                if (dock != null)
                                    this.ExtendedItem.Properties.GetAttachedProperty(DockPanel.DockProperty).SetValue(dock);
                            }
                        }

                        if (header == "水平对齐")
                        {
                            var value = _menu.UncheckChildrenAndSelectClicked(parent, clickedOn);
                            if (value != null)
                            {
                                var ha = Enum.Parse(typeof(HorizontalAlignment), value);
                                if (ha != null)
                                    this.ExtendedItem.Properties[FrameworkElement.HorizontalAlignmentProperty].SetValue(ha);
                            }
                        }

                        if (header == "垂直对齐")
                        {
                            var value = _menu.UncheckChildrenAndSelectClicked(parent, clickedOn);
                            if (value != null)
                            {
                                var va = Enum.Parse(typeof(VerticalAlignment), value);
                                if (va != null)
                                    this.ExtendedItem.Properties[FrameworkElement.VerticalAlignmentProperty].SetValue(va);
                            }
                        }
                    }
                }
            }
        }

        protected override void OnRemove()
        {
            base.OnRemove();
            _menu.Loaded -= OnMenuLoaded;
            var kbs = this.ExtendedItem.Services.GetService(typeof(IKeyBindingService)) as IKeyBindingService;
            if (kbs != null)
                kbs.DeregisterBinding(_keyBinding);
        }
    }
}
