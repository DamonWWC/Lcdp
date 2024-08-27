﻿using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    public partial class RightClickContextMenu
    {
        private readonly DesignItem designItem;

        public RightClickContextMenu(DesignItem designItem)
        {
            this.designItem = designItem;

            InitializeComponent();
        }

        private void Click_BringToFront(object sender, RoutedEventArgs e)
        {
            if ((designItem.ParentProperty == null) || !designItem.ParentProperty.IsCollection)
                return;

            var collection = this.designItem.ParentProperty.CollectionElements;
            collection.Remove(this.designItem);
            collection.Add(this.designItem);
        }

        void Click_SendToBack(object sender, RoutedEventArgs e)
        {
            if ((designItem.ParentProperty == null) || !designItem.ParentProperty.IsCollection)
                return;

            IObservableList<DesignItem> collection = this.designItem.ParentProperty.CollectionElements;
            collection.Remove(this.designItem);
            collection.Insert(0, this.designItem);
        }

        void Click_Backward(object sender, RoutedEventArgs e)
        {
            if ((designItem.ParentProperty == null) || !designItem.ParentProperty.IsCollection)
                return;

            var collection = this.designItem.ParentProperty.CollectionElements;
            var idx = collection.IndexOf(this.designItem);
            collection.RemoveAt(idx);
            collection.Insert((--idx < 0 ? 0 : idx), this.designItem);
        }

        void Click_Forward(object sender, RoutedEventArgs e)
        {
            if ((designItem.ParentProperty == null) || !designItem.ParentProperty.IsCollection)
                return;

            var collection = this.designItem.ParentProperty.CollectionElements;
            var idx = collection.IndexOf(this.designItem);
            collection.RemoveAt(idx);
            var cnt = collection.Count;
            collection.Insert((++idx > cnt ? cnt : idx), this.designItem);
        }
    }
}
