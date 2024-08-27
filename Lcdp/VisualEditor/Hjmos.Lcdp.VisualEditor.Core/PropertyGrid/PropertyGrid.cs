using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Interactivity;
using HandyControl.Tools.Extension;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core
{

    //[TemplatePart(Name = ElementSearchBar, Type = typeof(SearchBar))]
    [TemplatePart(Name = ElementItemsControl, Type = typeof(ItemsControl))]
    public class PropertyGrid : Control
    {
        private const string ElementItemsControl = "PART_ItemsControl";

        //private const string ElementSearchBar = "PART_SearchBar";

        private ItemsControl _itemsControl;

        private ICollectionView _dataView;

        //private SearchBar _searchBar;

        private string _searchKey;

        public PropertyGrid()
        {
            CommandBindings.Add(new CommandBinding(ControlCommands.SortByCategory, SortByCategory, (s, e) => e.CanExecute = ShowSortButton));
            CommandBindings.Add(new CommandBinding(ControlCommands.SortByName, SortByName, (s, e) => e.CanExecute = ShowSortButton));
        }

        public virtual PropertyResolver PropertyResolver { get; } = new PropertyResolver();

        public static readonly RoutedEvent SelectedObjectChangedEvent =
            EventManager.RegisterRoutedEvent("SelectedObjectChanged", RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<object>), typeof(PropertyGrid));

        public event RoutedPropertyChangedEventHandler<object> SelectedObjectChanged
        {
            add => AddHandler(SelectedObjectChangedEvent, value);
            remove => RemoveHandler(SelectedObjectChangedEvent, value);
        }

        public static readonly DependencyProperty SelectedObjectProperty = DependencyProperty.Register(
            "SelectedObject", typeof(object), typeof(PropertyGrid), new PropertyMetadata(default, OnSelectedObjectChanged));

        private static void OnSelectedObjectChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PropertyGrid ctl = (PropertyGrid)d;
            ctl.OnSelectedObjectChanged(e.OldValue, e.NewValue);
        }

        public object SelectedObject
        {
            get => GetValue(SelectedObjectProperty);
            set => SetValue(SelectedObjectProperty, value);
        }

        protected virtual void OnSelectedObjectChanged(object oldValue, object newValue)
        {
            UpdateItems(newValue);
            RaiseEvent(new RoutedPropertyChangedEventArgs<object>(oldValue, newValue, SelectedObjectChangedEvent));
        }

        public static readonly DependencyProperty DescriptionProperty = DependencyProperty.Register(
            "Description", typeof(string), typeof(PropertyGrid), new PropertyMetadata(default(string)));

        public string Description
        {
            get => (string)GetValue(DescriptionProperty);
            set => SetValue(DescriptionProperty, value);
        }

        public static readonly DependencyProperty MaxTitleWidthProperty = DependencyProperty.Register(
            "MaxTitleWidth", typeof(double), typeof(PropertyGrid), new PropertyMetadata(ValueBoxes.Double0Box));

        public double MaxTitleWidth
        {
            get => (double)GetValue(MaxTitleWidthProperty);
            set => SetValue(MaxTitleWidthProperty, value);
        }

        public static readonly DependencyProperty MinTitleWidthProperty = DependencyProperty.Register(
            "MinTitleWidth", typeof(double), typeof(PropertyGrid), new PropertyMetadata(ValueBoxes.Double0Box));

        public double MinTitleWidth
        {
            get => (double)GetValue(MinTitleWidthProperty);
            set => SetValue(MinTitleWidthProperty, value);
        }

        public static readonly DependencyProperty ShowSortButtonProperty = DependencyProperty.Register(
            "ShowSortButton", typeof(bool), typeof(PropertyGrid), new PropertyMetadata(ValueBoxes.TrueBox));

        public bool ShowSortButton
        {
            get => (bool)GetValue(ShowSortButtonProperty);
            set => SetValue(ShowSortButtonProperty, ValueBoxes.BooleanBox(value));
        }

        public override void OnApplyTemplate()
        {
            //if (_searchBar != null)
            //{
            //    _searchBar.SearchStarted -= SearchBar_SearchStarted;
            //}

            base.OnApplyTemplate();

            _itemsControl = GetTemplateChild(ElementItemsControl) as ItemsControl;
            //_searchBar = GetTemplateChild(ElementSearchBar) as SearchBar;

            //if (_searchBar != null)
            //{
            //    _searchBar.SearchStarted += SearchBar_SearchStarted;
            //}

            UpdateItems(SelectedObject);
        }

        private void UpdateItems(object obj)
        {
            if (_itemsControl == null) return;

            if (obj == null)
            {
                _itemsControl.ItemsSource = null;
                return;
            }

            IEnumerable<PropertyDescriptor> descriptors = TypeDescriptor.GetProperties(obj, new Attribute[] { new PropertyFilterAttribute(PropertyFilterOptions.SetValues) }).OfType<PropertyDescriptor>();

            // TODO：不需要的属性可以用特性来配置
            // 过滤属性面板不需要的属性
            List<string> filter = new()
            {
                "Background",
                "RenderSize",
                "Content",
                "XmlAttributeProperties.XmlnsDictionary",
                "NameScope.NameScope",
                "ViewModelLocator.AutoWireViewModel",
                "DataContext",
                "JsonAttached.Json",
                "ParameterMappingAttached.ParameterMapping",
                "DataSource",
                "DimensionArray",
                "Attached.",
                "CustomXaml"
            };
            // 特殊包含需要的属性
            List<string> withoutFilter = new()
            {
                "AdaptiveAttached.Adaptive"
            };



            IEnumerable<PropertyItem> propertyItems = descriptors.Where(item => PropertyResolver.ResolveIsBrowsable(item))
                .Where(item => !PropertyResolver.ResolveIsReadOnly(item))
                .Where(item => withoutFilter.Contains(item.DisplayName) || !filter.Contains(item.DisplayName))
                .Select(CreatePropertyItem).Do(item => item.InitElement());

            _dataView = CollectionViewSource.GetDefaultView(propertyItems);

            // 按分组排序
            //SortByCategory(null, null);

            // 按字母排序
            SortByName(null, null);
            _itemsControl.ItemsSource = _dataView;
        }

        private void SortByCategory(object sender, ExecutedRoutedEventArgs e)
        {
            if (_dataView == null) return;

            using (_dataView.DeferRefresh())
            {
                _dataView.GroupDescriptions.Clear();
                _dataView.SortDescriptions.Clear();
                _dataView.SortDescriptions.Add(new SortDescription(PropertyItem.CategoryProperty.Name, ListSortDirection.Ascending));
                _dataView.SortDescriptions.Add(new SortDescription(PropertyItem.DisplayNameProperty.Name, ListSortDirection.Ascending));
                _dataView.GroupDescriptions.Add(new PropertyGroupDescription(PropertyItem.CategoryProperty.Name));
            }
        }

        private void SortByName(object sender, ExecutedRoutedEventArgs e)
        {
            if (_dataView == null) return;

            using (_dataView.DeferRefresh())
            {
                _dataView.GroupDescriptions.Clear();
                _dataView.SortDescriptions.Clear();
                _dataView.SortDescriptions.Add(new SortDescription(PropertyItem.PropertyNameProperty.Name, ListSortDirection.Ascending));
            }
        }

        private void SearchBar_SearchStarted(object sender, FunctionEventArgs<string> e)
        {
            if (_dataView == null) return;

            _searchKey = e.Info;
            if (string.IsNullOrEmpty(_searchKey))
            {
                foreach (UIElement item in _dataView)
                {
                    item.Show();
                }
            }
            else
            {
                foreach (PropertyItem item in _dataView)
                {
                    item.Show(item.PropertyName.ToLower().Contains(_searchKey) || item.DisplayName.ToLower().Contains(_searchKey));
                }
            }
        }

        protected virtual PropertyItem CreatePropertyItem(PropertyDescriptor propertyDescriptor)
        {
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(propertyDescriptor);

            return new PropertyItem()
            {
                Category = PropertyResolver.ResolveCategory(propertyDescriptor),
                DisplayName = PropertyResolver.ResolveDisplayName(propertyDescriptor),
                Description = PropertyResolver.ResolveDescription(propertyDescriptor),
                IsReadOnly = PropertyResolver.ResolveIsReadOnly(propertyDescriptor),
                DefaultValue = PropertyResolver.ResolveDefaultValue(propertyDescriptor),
                Editor = PropertyResolver.ResolveEditor(propertyDescriptor),
                Value = SelectedObject,
                SourceProperty = dpd?.DependencyProperty,
                PropertyName = propertyDescriptor.Name,
                PropertyType = propertyDescriptor.PropertyType,
                PropertyTypeName = $"{propertyDescriptor.PropertyType.Namespace}.{propertyDescriptor.PropertyType.Name}"
            };
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            TitleElement.SetTitleWidth(this, new GridLength(Math.Max(MinTitleWidth, Math.Min(MaxTitleWidth, ActualWidth / 3))));
        }
    }
}
