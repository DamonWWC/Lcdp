using Hjmos.Lcdp.VisualEditor.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;

namespace Hjmos.Lcdp.VisualEditor.Core.DesignerPropertyGrid
{
    public interface IPropertyGrid
    {
        IEnumerable<DesignItem> SelectedItems { get; set; }
        Dictionary<MemberDescriptor, PropertyNode> NodeFromDescriptor { get; }
        DesignItem SingleItem { get; }
        string Name { get; set; }
        string OldName { get; }
        bool IsNameCorrect { get; set; }
        bool ReloadActive { get; }
        event EventHandler AggregatePropertiesUpdated;
        event PropertyChangedEventHandler PropertyChanged;
    }

    public class PropertyGrid : INotifyPropertyChanged, IPropertyGrid
    {
        public PropertyGrid()
        {
            Categories = new CategoriesCollection();
            Categories.Add(specialCategory);
            Categories.Add(popularCategory);
            Categories.Add(otherCategory);
            Categories.Add(attachedCategory);

            Events = new PropertyNodeCollection();
        }

        Category specialCategory = new Category("Special");
        Category popularCategory = new Category("Popular");
        Category otherCategory = new Category("Other");
        Category attachedCategory = new Category("Attached");

        Dictionary<MemberDescriptor, PropertyNode> nodeFromDescriptor = new Dictionary<MemberDescriptor, PropertyNode>();
        public Dictionary<MemberDescriptor, PropertyNode> NodeFromDescriptor { get { return nodeFromDescriptor; } }
        public event EventHandler AggregatePropertiesUpdated;
        public CategoriesCollection Categories { get; private set; }
        public PropertyNodeCollection Events { get; private set; }

        private PropertyGridGroupMode _groupMode;

        public PropertyGridGroupMode GroupMode
        {
            get { return _groupMode; }
            set
            {
                if (_groupMode != value)
                {
                    _groupMode = value;

                    RaisePropertyChanged("GroupMode");

                    Reload();
                }
            }
        }

        PropertyGridTab currentTab;

        public PropertyGridTab CurrentTab
        {
            get
            {
                return currentTab;
            }
            set
            {
                currentTab = value;
                RaisePropertyChanged("CurrentTab");
                RaisePropertyChanged("NameBackground");
            }
        }

        string filter;

        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                filter = value;
                Reload();
                RaisePropertyChanged("Filter");
            }
        }

        DesignItem singleItem;

        public DesignItem SingleItem
        {
            get
            {
                return singleItem;
            }
            private set
            {
                if (singleItem != null)
                {
                    singleItem.NameChanged -= singleItem_NameChanged;
                }
                singleItem = value;
                if (singleItem != null)
                {
                    singleItem.NameChanged += singleItem_NameChanged;
                }
                RaisePropertyChanged("SingleItem");
                RaisePropertyChanged("Name");
                RaisePropertyChanged("IsNameEnabled");
                IsNameCorrect = true;
            }
        }

        void singleItem_NameChanged(object sender, EventArgs e)
        {
            RaisePropertyChanged("Name");
        }

        public string OldName
        {
            get; private set;
        }

        public string Name
        {
            get
            {
                if (SingleItem != null)
                {
                    return SingleItem.Name;
                }
                return null;
            }
            set
            {
                if (SingleItem != null)
                {
                    try
                    {
                        if (string.IsNullOrEmpty(value))
                        {
                            OldName = null;
                            SingleItem.Name = null;
                        }
                        else
                        {
                            OldName = SingleItem.Name;
                            SingleItem.Name = value;
                        }
                        IsNameCorrect = true;
                    }
                    catch
                    {
                        IsNameCorrect = false;
                    }
                    RaisePropertyChanged("Name");
                }
            }
        }

        bool isNameCorrect = true;

        public bool IsNameCorrect
        {
            get
            {
                return isNameCorrect;
            }
            set
            {
                isNameCorrect = value;
                RaisePropertyChanged("IsNameCorrect");
            }
        }

        public bool IsNameEnabled
        {
            get
            {
                return SingleItem != null;
            }
        }

        IList<DesignItem> selectedItems;

        public IEnumerable<DesignItem> SelectedItems
        {
            get
            {
                return selectedItems;
            }
            set
            {
                if (value == null)
                    selectedItems = null;
                else
                    selectedItems = value.ToList();
                RaisePropertyChanged("SelectedItems");
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(
                    delegate
                    {
                        Reload();
                    }));
            }
        }

        public void ClearFilter()
        {
            Filter = null;
        }

        volatile bool reloadActive;
        public bool ReloadActive
        {
            get { return reloadActive; }
        }

        private void Reload()
        {
            reloadActive = true;
            try
            {
                Clear();

                if (selectedItems == null || selectedItems.Count == 0) return;
                if (selectedItems.Count == 1) SingleItem = selectedItems[0];

                foreach (MemberDescriptor md in GetDescriptors())
                {
                    if (PassesFilter(md.Name))
                        AddNode(md);
                }
            }
            finally
            {
                reloadActive = false;
                if (AggregatePropertiesUpdated != null)
                    AggregatePropertiesUpdated(this, EventArgs.Empty);
            }
        }

        private void Clear()
        {
            foreach (Category c in Categories)
            {
                c.IsVisible = false;
                foreach (PropertyNode p in c.Properties)
                {
                    p.IsVisible = false;
                }
            }

            foreach (PropertyNode e in Events)
            {
                e.IsVisible = false;
            }

            SingleItem = null;
        }

        private List<MemberDescriptor> GetDescriptors()
        {
            List<MemberDescriptor> list = new List<MemberDescriptor>();
            IComponentPropertyService service = (SingleItem ?? SelectedItems.First()).Services.GetService<IComponentPropertyService>();

            if (SelectedItems.Count() == 1)
            {
                // TODO：添加列的时候，这里会报错
                list.AddRange(service.GetAvailableProperties(SingleItem));
                list.AddRange(service.GetAvailableEvents(SingleItem));

            }
            else
            {
                list.AddRange(service.GetCommonAvailableProperties(SelectedItems));
            }

            return list;
        }

        bool PassesFilter(string name)
        {
            if (string.IsNullOrEmpty(Filter)) return true;
            for (int i = 0; i < name.Length; i++)
            {
                if (i == 0 || char.IsUpper(name[i]))
                {
                    if (string.Compare(name, i, Filter, 0, Filter.Length, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void AddNode(MemberDescriptor md)
        {
            DesignItemProperty[] designProperties = SelectedItems.Select(item => item.Properties.GetProperty(md)).ToArray();
            if (!Metadata.IsBrowsable(designProperties[0])) return;

            if (nodeFromDescriptor.TryGetValue(md, out PropertyNode node))
            {
                node.Load(designProperties);
            }
            else
            {
                node = new PropertyNode();
                node.Load(designProperties);
                if (node.IsEvent)
                {
                    Events.AddSorted(node);
                }
                else
                {
                    Category cat = PickCategory(node);
                    cat.Properties.AddSorted(node);
                    node.Category = cat;
                }
                nodeFromDescriptor[md] = node;
            }
            node.IsVisible = true;
            if (node.Category != null)
                node.Category.IsVisible = true;
        }

        Category PickCategory(PropertyNode node)
        {
            if (Metadata.IsPopularProperty(node.FirstProperty)) return popularCategory;
            if (node.FirstProperty.IsAttachedDependencyProperty()) return attachedCategory;
            string typeName = node.FirstProperty.DeclaringType.FullName;
            if (typeName.StartsWith("System.Windows.", StringComparison.Ordinal) || typeName.StartsWith("Hjmos.Lcdp.VisualEditor.Core.", StringComparison.Ordinal))
                return otherCategory;
            return specialCategory;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }

    public class CategoriesCollection : SortedObservableCollection<Category, string>
    {
        public CategoriesCollection() : base(n => n.Name) { }
    }

    public enum PropertyGridGroupMode
    {
        GroupByPopularCategorys,
        GroupByCategorys,
        Ungrouped,
    }

    public enum PropertyGridTab
    {
        Properties,
        Events
    }
}
