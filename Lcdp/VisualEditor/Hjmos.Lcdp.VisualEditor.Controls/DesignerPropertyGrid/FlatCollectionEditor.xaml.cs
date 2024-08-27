using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.DesignerPropertyGrid
{
    public partial class FlatCollectionEditor : Window
    {
        private static readonly Dictionary<Type, Type> TypeMappings = new Dictionary<Type, Type>();
        static FlatCollectionEditor()
        {
            TypeMappings.Add(typeof(ListBox), typeof(ListBoxItem));
            TypeMappings.Add(typeof(ListView), typeof(ListViewItem));
            TypeMappings.Add(typeof(ComboBox), typeof(ComboBoxItem));
            TypeMappings.Add(typeof(TabControl), typeof(TabItem));
            TypeMappings.Add(typeof(ColumnDefinitionCollection), typeof(ColumnDefinition));
            TypeMappings.Add(typeof(RowDefinitionCollection), typeof(RowDefinition));
        }

        private DesignItemProperty _itemProperty;
        private IComponentService _componentService;
        private Type _type;

        public FlatCollectionEditor()
        {
            this.InitializeComponent();

            this.Owner = Application.Current.MainWindow;
        }

        public Type GetItemsSourceType(Type t)
        {
            Type tp = t.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(ICollection<>));

            return (tp != null) ? tp.GetGenericArguments()[0] : null;
        }

        public void LoadItemsCollection(DesignItemProperty itemProperty)
        {
            _itemProperty = itemProperty;
            _componentService = _itemProperty.DesignItem.Services.Component;
            TypeMappings.TryGetValue(_itemProperty.ReturnType, out _type);

            _type = _type ?? GetItemsSourceType(_itemProperty.ReturnType);

            if (_type == null)
            {
                AddItem.IsEnabled = false;
            }

            ListBox.ItemsSource = _itemProperty.CollectionElements;
        }

        private void OnAddItemClicked(object sender, RoutedEventArgs e)
        {
            DesignItem newItem = _componentService.RegisterComponentForDesigner(Activator.CreateInstance(_type));
            _itemProperty.CollectionElements.Add(newItem);
        }

        private void OnRemoveItemClicked(object sender, RoutedEventArgs e)
        {
            var selItem = ListBox.SelectedItem as DesignItem;
            if (selItem != null)
                _itemProperty.CollectionElements.Remove(selItem);
        }

        private void OnMoveItemUpClicked(object sender, RoutedEventArgs e)
        {
            DesignItem selectedItem = ListBox.SelectedItem as DesignItem;
            if (selectedItem != null)
            {
                if (_itemProperty.CollectionElements.Count != 1 && _itemProperty.CollectionElements.IndexOf(selectedItem) != 0)
                {
                    int moveToIndex = _itemProperty.CollectionElements.IndexOf(selectedItem) - 1;
                    var itemAtMoveToIndex = _itemProperty.CollectionElements[moveToIndex];
                    _itemProperty.CollectionElements.RemoveAt(moveToIndex);
                    if ((moveToIndex + 1) < (_itemProperty.CollectionElements.Count + 1))
                        _itemProperty.CollectionElements.Insert(moveToIndex + 1, itemAtMoveToIndex);
                }
            }
        }

        private void OnMoveItemDownClicked(object sender, RoutedEventArgs e)
        {
            DesignItem selectedItem = ListBox.SelectedItem as DesignItem;
            if (selectedItem != null)
            {
                var itemCount = _itemProperty.CollectionElements.Count;
                if (itemCount != 1 && _itemProperty.CollectionElements.IndexOf(selectedItem) != itemCount)
                {
                    int moveToIndex = _itemProperty.CollectionElements.IndexOf(selectedItem) + 1;
                    if (moveToIndex < itemCount)
                    {
                        var itemAtMoveToIndex = _itemProperty.CollectionElements[moveToIndex];
                        _itemProperty.CollectionElements.RemoveAt(moveToIndex);
                        if (moveToIndex > 0)
                            _itemProperty.CollectionElements.Insert(moveToIndex - 1, itemAtMoveToIndex);
                    }
                }
            }
        }

        void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PropertyGridView.PropertyGrid.SelectedItems = ListBox.SelectedItems.Cast<DesignItem>();
        }
    }
}
