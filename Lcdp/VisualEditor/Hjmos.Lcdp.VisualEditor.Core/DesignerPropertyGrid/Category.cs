using System.ComponentModel;

namespace Hjmos.Lcdp.VisualEditor.Core.DesignerPropertyGrid
{
    /// <summary>
    /// View-Model class for a property grid category.
    /// 属性网格类别的视图-模型类。  
    /// </summary>
    public class Category : INotifyPropertyChanged
    {

        public Category(string name)
        {
            Name = name;
            Properties = new PropertyNodeCollection();
            //MoreProperties = new ObservableCollection<PropertyNode>();
        }

        public string Name { get; private set; }
        public PropertyNodeCollection Properties { get; private set; }
        //public ObservableCollection<PropertyNode> MoreProperties { get; private set; }

        bool isExpanded = true;

        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                isExpanded = value;
                RaisePropertyChanged("IsExpanded");
            }
        }

        //bool showMore;
        //internal bool ShowMoreByFilter;

        //public bool ShowMore {
        //    get {
        //        return showMore;
        //    }
        //    set {
        //        showMore = value;
        //        RaisePropertyChanged("ShowMore");
        //    }
        //}

        bool isVisible;

        public bool IsVisible
        {
            get => isVisible;
            set
            {
                isVisible = value;
                RaisePropertyChanged("IsVisible");
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        void RaisePropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}
