using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Core.OutlineView
{
    public class DragTreeViewItem : TreeViewItem
    {
        ContentPresenter part_header;

        static DragTreeViewItem() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DragTreeViewItem), new FrameworkPropertyMetadata(typeof(DragTreeViewItem)));

        public DragTreeViewItem()
        {
            Loaded += new RoutedEventHandler(DragTreeViewItem_Loaded);
            Unloaded += new RoutedEventHandler(DragTreeViewItem_Unloaded);
        }

        private void DragTreeViewItem_Loaded(object sender, RoutedEventArgs e)
        {
            ParentTree = this.GetVisualAncestors().OfType<DragTreeView>().FirstOrDefault();
            if (ParentTree != null)
            {
                ParentTree.ItemAttached(this);
                ParentTree.FilterChanged += ParentTree_FilterChanged;
            }
        }

        private void ParentTree_FilterChanged(string obj)
        {
            var v = ParentTree.ShouldItemBeVisible(this);
            if (v)
                part_header.Visibility = Visibility.Visible;
            else
                part_header.Visibility = Visibility.Collapsed;
        }

        private void DragTreeViewItem_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ParentTree != null)
            {
                ParentTree.ItemDetached(this);
                ParentTree.FilterChanged -= ParentTree_FilterChanged;
            }
            ParentTree = null;
        }


        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            part_header = GetTemplateChild("PART_Header") as ContentPresenter;
        }

        public new static readonly DependencyProperty IsSelectedProperty =
            Selector.IsSelectedProperty.AddOwner(typeof(DragTreeViewItem), new FrameworkPropertyMetadata(OnIsSelectedChanged));

        public new bool IsSelected
        {
            get { return (bool)GetValue(IsSelectedProperty); }
            set { SetValue(IsSelectedProperty, value); }
        }

        public static void OnIsSelectedChanged(DependencyObject s, DependencyPropertyChangedEventArgs e)
        {
            if (s is DragTreeViewItem el && el.IsSelected)
                el.BringIntoView();
        }

        public static readonly DependencyProperty IsDragHoverProperty =
            DependencyProperty.Register("IsDragHover", typeof(bool), typeof(DragTreeViewItem));

        public bool IsDragHover
        {
            get { return (bool)GetValue(IsDragHoverProperty); }
            set { SetValue(IsDragHoverProperty, value); }
        }

        internal ContentPresenter HeaderPresenter => (ContentPresenter)Template.FindName("PART_Header", this);

        public static readonly DependencyProperty LevelProperty =
            DependencyProperty.Register("Level", typeof(int), typeof(DragTreeViewItem));

        public int Level
        {
            get { return (int)GetValue(LevelProperty); }
            set { SetValue(LevelProperty, value); }
        }

        public DragTreeView ParentTree { get; private set; }

        protected override void OnVisualParentChanged(DependencyObject oldParent)
        {
            base.OnVisualParentChanged(oldParent);

            if (ItemsControlFromItemContainer(this) is DragTreeViewItem parentItem) Level = parentItem.Level + 1;
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == IsSelectedProperty)
            {
                if (ParentTree != null)
                {
                    ParentTree.ItemIsSelectedChanged(this);
                }
            }
        }

        protected override DependencyObject GetContainerForItemOverride() => new DragTreeViewItem();

        protected override bool IsItemItsOwnContainerOverride(object item) => item is DragTreeViewItem;

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.Source is ToggleButton || e.Source is ItemsPresenter) return;
            ParentTree.ItemMouseDown(this);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (ParentTree != null)
            {
                ParentTree.ItemMouseUp(this);
            }
        }
    }
}
