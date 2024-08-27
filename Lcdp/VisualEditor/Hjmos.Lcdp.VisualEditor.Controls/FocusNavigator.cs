using Hjmos.Lcdp.VisualEditor.Controls.DesignerControls;
using System.Linq;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 管理焦点/主要选择使用TAB下的树导航和Shift+Tab上的树导航
    /// </summary>
    internal class FocusNavigator
    {
        /*
		 *  焦点导航器不涉及逻辑焦点或键盘焦点的概念，因为除了DesignPanel之外，没有什么是聚焦在设计器上的。
		 *  它只是改变了设计器上呈现的元素层次之间的主要选择。  
		 */

        private readonly DesignSurface _surface;
        private KeyBinding _tabBinding;
        private KeyBinding _shiftTabBinding;

        public FocusNavigator(DesignSurface surface) => _surface = surface;

        /// <summary>
        /// 在设计界面上启动导航器并添加绑定。  
        /// </summary>
        public void Start()
        {
            RelayCommand tabFocus = new(this.MoveFocusForward, this.CanMoveFocusForward);
            RelayCommand shiftTabFocus = new(this.MoveFocusBack, this.CanMoveFocusBack);
            _tabBinding = new KeyBinding(tabFocus, new KeyGesture(Key.Tab));
            _shiftTabBinding = new KeyBinding(shiftTabFocus, new KeyGesture(Key.Tab, ModifierKeys.Shift));
            IKeyBindingService kbs = _surface.DesignContext.Services.GetService(typeof(IKeyBindingService)) as IKeyBindingService;
            if (kbs != null)
            {
                kbs.RegisterBinding(_tabBinding);
                kbs.RegisterBinding(_shiftTabBinding);
            }
        }

        /// <summary>
        /// De-register the bindings from the Design Surface
        /// </summary>
        public void End()
        {
            if (_surface.DesignContext.Services.GetService(typeof(IKeyBindingService)) is IKeyBindingService kbs)
            {
                kbs.DeregisterBinding(_tabBinding);
                kbs.DeregisterBinding(_shiftTabBinding);
            }
        }

        /// <summary>
        /// Moves the Foucus down the tree.
        /// </summary>        
        private void MoveFocusForward()
        {
            DesignSurface designSurface = _surface;
            if (designSurface != null)
            {
                DesignContext context = designSurface.DesignContext;
                ISelectionService selection = context.Services.Selection;
                DesignItem item = selection.PrimarySelection;
                selection.SetSelectedComponents(selection.SelectedItems, SelectionTypes.Remove);
                if (item != GetLastElement())
                {
                    if (item.ContentProperty != null)
                    {
                        if (item.ContentProperty.IsCollection)
                        {
                            if (item.ContentProperty.CollectionElements.Count != 0)
                            {
                                if (ModelTools.CanSelectComponent(item.ContentProperty.CollectionElements.First()))
                                    selection.SetSelectedComponents(new DesignItem[] { item.ContentProperty.CollectionElements.First() }, SelectionTypes.Primary);
                                else
                                    SelectNextInPeers(item);
                            }
                            else
                                SelectNextInPeers(item);
                        }
                        else if (item.ContentProperty.Value != null)
                        {
                            if (ModelTools.CanSelectComponent(item.ContentProperty.Value))
                                selection.SetSelectedComponents(new DesignItem[] { item.ContentProperty.Value }, SelectionTypes.Primary);
                            else
                                SelectNextInPeers(item);
                        }
                        else
                        {
                            SelectNextInPeers(item);
                        }
                    }
                    else
                    {
                        SelectNextInPeers(item);
                    }
                }
                else
                { //if the element was last element move focus to the root element to keep a focus cycle.
                    selection.SetSelectedComponents(new DesignItem[] { context.RootItem }, SelectionTypes.Primary);
                }
            }
        }

        /// <summary>
        /// Checks if focus navigation should be for down-the-tree be done.
        /// </summary>
        private bool CanMoveFocusForward()
        {
            DesignSurface designSurface = _surface;
            if (designSurface != null)
                if (Keyboard.FocusedElement == designSurface.DesignPanel)
                    return true;
            return false;
        }

        /// <summary>
        /// Moves focus up-the-tree.
        /// </summary>        
        private void MoveFocusBack()
        {
            DesignSurface designSurface = _surface;
            if (designSurface != null)
            {
                DesignContext context = designSurface.DesignContext;
                ISelectionService selection = context.Services.Selection;
                DesignItem item = selection.PrimarySelection;
                if (item != context.RootItem)
                {
                    if (item.Parent != null && item.Parent.ContentProperty.IsCollection)
                    {
                        int index = item.Parent.ContentProperty.CollectionElements.IndexOf(item);
                        if (index != 0)
                        {
                            if (ModelTools.CanSelectComponent(item.Parent.ContentProperty.CollectionElements.ElementAt(index - 1)))
                                selection.SetSelectedComponents(new DesignItem[] { item.Parent.ContentProperty.CollectionElements.ElementAt(index - 1) }, SelectionTypes.Primary);
                        }
                        else
                        {
                            if (ModelTools.CanSelectComponent(item.Parent))
                                selection.SetSelectedComponents(new DesignItem[] { item.Parent }, SelectionTypes.Primary);
                        }

                    }
                    else
                    {
                        if (ModelTools.CanSelectComponent(item.Parent))
                            selection.SetSelectedComponents(new DesignItem[] { item.Parent }, SelectionTypes.Primary);
                    }
                }
                else
                {// if the element was root item move focus again to the last element.
                    selection.SetSelectedComponents(new DesignItem[] { GetLastElement() }, SelectionTypes.Primary);
                }
            }
        }

        /// <summary>
        /// Checks if focus navigation for the up-the-tree should be done.
        /// </summary>
        private bool CanMoveFocusBack()
        {
            var designSurface = _surface;
            if (designSurface != null)
                if (Keyboard.FocusedElement == designSurface.DesignPanel)
                    return true;
            return false;
        }

        /// <summary>
        /// Gets the last element in the element hierarchy.
        /// </summary>        
        DesignItem GetLastElement()
        {
            DesignItem item = _surface.DesignContext.RootItem;
            while (item != null && item.ContentProperty != null)
            {
                if (item.ContentProperty.IsCollection)
                {
                    if (item.ContentProperty.CollectionElements.Count != 0)
                    {
                        if (ModelTools.CanSelectComponent(item.ContentProperty.CollectionElements.Last()))
                            item = item.ContentProperty.CollectionElements.Last();
                        else
                            break;
                    }
                    else
                        break;
                }
                else
                {
                    if (item.ContentProperty.Value != null)
                        item = item.ContentProperty.Value;
                    else
                        break;
                }
            }
            return item;
        }

        /// <summary>
        /// Select the next element in the element collection if <paramref name="item"/> parent's had it's content property as collection.
        /// </summary>        
        private void SelectNextInPeers(DesignItem item)
        {
            ISelectionService selection = _surface.DesignContext.Services.Selection;
            if (item.Parent != null && item.Parent.ContentProperty != null)
            {
                if (item.Parent.ContentProperty.IsCollection)
                {
                    int index = item.Parent.ContentProperty.CollectionElements.IndexOf(item);
                    if (index != item.Parent.ContentProperty.CollectionElements.Count)
                        selection.SetSelectedComponents(new DesignItem[] { item.Parent.ContentProperty.CollectionElements.ElementAt(index + 1) }, SelectionTypes.Primary);
                }
            }
        }
    }
}
