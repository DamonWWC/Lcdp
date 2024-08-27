using Hjmos.Lcdp.VisualEditor.Core.Services;
using Hjmos.Lcdp.VisualEditor.Core.XamlDom;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.OutlineView
{

    public abstract class OutlineNodeBase : INotifyPropertyChanged, IOutlineNode
    {
        protected abstract void UpdateChildren();
        protected abstract void UpdateChildrenCollectionChanged(NotifyCollectionChangedEventArgs e);
        //Used to check if element can enter other containers
        protected static PlacementType DummyPlacementType;

        private bool _collectionWasChanged;

        private string _name;

        public Visibility IsVisualNode => this.DesignItem.Component is Visual ? Visibility.Visible : Visibility.Collapsed;

        protected OutlineNodeBase(DesignItem designItem)
        {
            DesignItem = designItem;

            bool hidden = false;
            try
            {
                hidden = Equals(designItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).ValueOnInstance, true);
            }
            catch (Exception)
            { }
            if (hidden)
            {
                _isDesignTimeVisible = false;
            }

            bool locked = false;
            try
            {
                locked = Equals(designItem.Properties.GetAttachedProperty(DesignTimeProperties.IsLockedProperty).ValueOnInstance, true);
            }
            catch (Exception)
            { }
            if (locked)
            {
                _isDesignTimeLocked = true;
            }

            //TODO

            DesignItem.NameChanged += new EventHandler(DesignItem_NameChanged);

            if (DesignItem.ContentProperty != null && DesignItem.ContentProperty.IsCollection)
            {
                DesignItem.ContentProperty.CollectionElements.CollectionChanged += CollectionElements_CollectionChanged;
                DesignItem.PropertyChanged += new PropertyChangedEventHandler(DesignItem_PropertyChanged);
            }
            else
            {
                DesignItem.PropertyChanged += new PropertyChangedEventHandler(DesignItem_PropertyChanged);
            }
        }

        protected OutlineNodeBase(string name) => _name = name;

        public DesignItem DesignItem { get; set; }

        public virtual ServiceContainer Services => this.DesignItem.Services;

        public ISelectionService SelectionService => DesignItem?.Services.Selection;

        private bool isExpanded = true;

        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        private bool isSelected;

        public bool IsSelected
        {
            get => isSelected;
            set
            {
                if (isSelected != value && SelectionService != null)
                {
                    isSelected = value;
                    SelectionService.SetSelectedComponents(new[] { DesignItem },
                                                           value ? SelectionTypes.Add : SelectionTypes.Remove);
                    RaisePropertyChanged("IsSelected");
                }
            }
        }

        private bool _isDesignTimeVisible = true;

        public bool IsDesignTimeVisible
        {
            get => _isDesignTimeVisible;
            set
            {
                _isDesignTimeVisible = value;

                RaisePropertyChanged("IsDesignTimeVisible");

                if (!value)
                    DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).SetValue(true);
                else
                    DesignItem.Properties.GetAttachedProperty(DesignTimeProperties.IsHiddenProperty).Reset();
            }
        }

        private bool _isDesignTimeLocked = false;

        public bool IsDesignTimeLocked
        {
            get => _isDesignTimeLocked;
            set
            {
                _isDesignTimeLocked = value;
                ((MyDesignItem)DesignItem).IsDesignTimeLocked = _isDesignTimeLocked;

                RaisePropertyChanged("IsDesignTimeLocked");
            }
        }

        public ObservableCollection<IOutlineNode> Children { get; } = new ObservableCollection<IOutlineNode>();

        public string Name => !string.IsNullOrEmpty(_name) ? _name : DesignItem.Services.GetService<IOutlineNodeNameService>().GetOutlineNodeName(DesignItem);

        private void DesignItem_NameChanged(object sender, EventArgs e) => RaisePropertyChanged("Name");

        private void DesignItem_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_collectionWasChanged)
            {
                UpdateChildren();
            }
            _collectionWasChanged = false;
        }

        private void CollectionElements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _collectionWasChanged = true;
            UpdateChildrenCollectionChanged(e);
        }

        public bool CanInsert(IEnumerable<IOutlineNode> nodes, IOutlineNode after, bool copy)
        {
            IPlacementBehavior placementBehavior = DesignItem.GetBehavior<IPlacementBehavior>();
            if (placementBehavior == null)
                return false;
            PlacementOperation operation = PlacementOperation.Start(nodes.Select(node => node.DesignItem).ToArray(), DummyPlacementType);
            if (operation != null)
            {
                bool canEnter = placementBehavior.CanEnterContainer(operation, true);
                operation.Abort();
                return canEnter;
            }
            return false;
        }

        public virtual void Insert(IEnumerable<IOutlineNode> nodes, IOutlineNode after, bool copy)
        {
            //using (var moveTransaction = DesignItem.Context.OpenGroup("Item moved in outline view", nodes.Select(n => n.DesignItem).ToList()))
            //{
            if (copy)
            {
                nodes = nodes.Select(n => OutlineNode.Create(n.DesignItem.Clone())).ToList();
            }
            else
            {
                foreach (IOutlineNode node in nodes)
                {
                    node.DesignItem.Remove();
                }
            }

            int index = after == null ? 0 : Children.IndexOf(after) + 1;

            DesignItemProperty content = DesignItem.ContentProperty;
            if (content.IsCollection)
            {
                foreach (var node in nodes)
                {
                    content.CollectionElements.Insert(index++, node.DesignItem);
                }
            }
            else
            {
                content.SetValue(nodes.First().DesignItem);
            }
            //moveTransaction.Commit();
            //}
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
