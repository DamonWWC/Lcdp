using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.ComponentModel;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    /// <summary>
    /// 管理选定组件的集合和主要选择。  
    /// 当组件的选择状态发生变化时，通知组件及其附加的DesignSite。
    /// </summary>
    internal sealed class DefaultSelectionService : ISelectionService, INotifyPropertyChanged
    {
        private readonly HashSet<DesignItem> _selectedComponents = new();

        public bool IsComponentSelected(DesignItem component) => _selectedComponents.Contains(component);

        public ICollection<DesignItem> SelectedItems => _selectedComponents.ToArray();

        public DesignItem PrimarySelection { get; private set; }

        public int SelectionCount => _selectedComponents.Count;

        public event EventHandler SelectionChanging;
        public event EventHandler<DesignItemCollectionEventArgs> SelectionChanged;
        public event EventHandler PrimarySelectionChanging;
        public event EventHandler PrimarySelectionChanged;

        public void SetSelectedComponents(ICollection<DesignItem> components)
        {
            SetSelectedComponents(components, SelectionTypes.Replace);
        }

        // 使用指定的组件和selectionType修改当前选择。
        // 在这个方法内会给组件加装饰层
        public void SetSelectedComponents(ICollection<DesignItem> components, SelectionTypes selectionType)
        {
            if (components == null)
                components = SharedInstances.EmptyDesignItemArray;

            if (components.Contains(null))
                throw new ArgumentException("Cannot select 'null'.");

            var prevSelectedItems = _selectedComponents.ToArray();

            if (SelectionChanging != null)
                SelectionChanging(this, EventArgs.Empty);

            DesignItem newPrimarySelection = PrimarySelection;

            if (selectionType == SelectionTypes.Auto)
            {
                if (Keyboard.Modifiers == ModifierKeys.Control)
                    selectionType = SelectionTypes.Toggle; // Ctrl pressed: toggle selection   按下Ctrl键:切换选择
                else if ((Keyboard.Modifiers & ~ModifierKeys.Control) == ModifierKeys.Shift)
                    selectionType = SelectionTypes.Add; // Shift or Ctrl+Shift pressed: add to selection   按下Shift或Ctrl+Shift:添加到选区
                else
                    selectionType = SelectionTypes.Primary; // otherwise: change primary selection   否则:更改初选
            }

            if ((selectionType & SelectionTypes.Primary) == SelectionTypes.Primary)
            {
                // change primary selection to first new component
                // 将主选择更改为第一个新组件
                newPrimarySelection = null;
                foreach (DesignItem obj in components)
                {
                    newPrimarySelection = obj;
                    break;
                }

                selectionType &= ~SelectionTypes.Primary;
                if (selectionType == 0)
                {
                    // if selectionType was only Primary, and components has only one item that changes the primary selection was changed to an already-selected item, then we keep the current selection.
                    // otherwise, we replace it
                    // 如果selectionType仅为Primary，且组件只有一个将主选择更改为已选中项的项，则保留当前选择。
                    if (components.Count == 1 && IsComponentSelected(newPrimarySelection) && prevSelectedItems.Length == 1)
                    {
                        // keep selectionType = 0 -> don't change the selection
                    }
                    else
                    {
                        selectionType = SelectionTypes.Replace;
                    }
                }
            }

            HashSet<DesignItem> componentsToNotifyOfSelectionChange = new HashSet<DesignItem>();
            switch (selectionType)
            {
                case SelectionTypes.Add:
                    // add to selection and notify if required
                    // 添加到选择中，并在需要时通知
                    foreach (DesignItem obj in components)
                    {
                        if (_selectedComponents.Add(obj))
                            componentsToNotifyOfSelectionChange.Add(obj);
                    }
                    break;
                case SelectionTypes.Remove:
                    // remove from selection and notify if required
                    // 从选择中删除，并在需要时通知
                    foreach (DesignItem obj in components)
                    {
                        if (_selectedComponents.Remove(obj))
                            componentsToNotifyOfSelectionChange.Add(obj);
                    }
                    break;
                case SelectionTypes.Replace:
                    // notify all old components:
                    // 通知所有旧组件:
                    componentsToNotifyOfSelectionChange.AddRange(_selectedComponents);
                    // set _selectedCompontents to new components
                    _selectedComponents.Clear();
                    foreach (DesignItem obj in components)
                    {
                        _selectedComponents.Add(obj);
                        // notify the new components
                        // 通知新组件
                        componentsToNotifyOfSelectionChange.Add(obj);
                    }
                    break;
                case SelectionTypes.Toggle:
                    // toggle selection and notify
                    // 切换选择和通知
                    foreach (DesignItem obj in components)
                    {
                        if (_selectedComponents.Contains(obj))
                        {
                            _selectedComponents.Remove(obj);
                        }
                        else
                        {
                            _selectedComponents.Add(obj);
                        }
                        componentsToNotifyOfSelectionChange.Add(obj);
                    }
                    break;
                case 0:
                    break;
                default:
                    throw new NotSupportedException("The selection type " + selectionType + " is not supported");
            }

            if (!IsComponentSelected(newPrimarySelection))
            {
                // primary selection is not selected anymore - change primary selection to any other selected component
                // 主选择不再被选择-将主选择更改为任何其他被选择的组件
                newPrimarySelection = null;
                foreach (DesignItem obj in _selectedComponents)
                {
                    newPrimarySelection = obj;
                    break;
                }
            }

            // Primary selection has changed:
            // 初选已经改变:
            if (newPrimarySelection != PrimarySelection)
            {
                componentsToNotifyOfSelectionChange.Add(PrimarySelection);
                componentsToNotifyOfSelectionChange.Add(newPrimarySelection);
                PrimarySelectionChanging?.Invoke(this, EventArgs.Empty);
                PrimarySelection = newPrimarySelection;
                // 注释这个方法后，组件就没有装饰层了，组件从这个方法开始附加装饰层
                PrimarySelectionChanged?.Invoke(this, EventArgs.Empty);
                RaisePropertyChanged("PrimarySelection");
            }

            if (!_selectedComponents.SequenceEqual(prevSelectedItems))
            {
                SelectionChanged?.Invoke(this, new DesignItemCollectionEventArgs(componentsToNotifyOfSelectionChange));
                RaisePropertyChanged("SelectedItems");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        #endregion
    }
}