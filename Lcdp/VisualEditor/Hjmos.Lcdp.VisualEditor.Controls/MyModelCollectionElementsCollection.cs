using Hjmos.Lcdp.VisualEditor.Controls.Services;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 表示设计项的一个集合属性中的集合
    /// </summary>
    internal sealed class MyModelCollectionElementsCollection : IObservableList<DesignItem>, INotifyCollectionChanged
    {
        readonly MyModelProperty _modelProperty;
        readonly MyDesignContext _context;
        private readonly IList _collection;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public MyModelCollectionElementsCollection(MyModelProperty modelProperty)
        {
            _modelProperty = modelProperty;
            _context = (MyDesignContext)modelProperty.DesignItem.Context;
            _collection = _modelProperty.ValueOnInstance as IList;
        }

        public int Count => _collection.Count;

        public bool IsReadOnly => false;

        public void Add(DesignItem item) => Insert(this.Count, item);

        public void Clear()
        {
            while (this.Count > 0)
            {
                RemoveAt(this.Count - 1);
            }

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        public bool Contains(DesignItem item)
        {
            MyDesignItem xitem = CheckItemNoException(item);
            if (xitem != null)
                throw new NotImplementedException();
            //return property.CollectionElements.Contains(xitem.XamlObject);
            else
                return false;
        }

        public int IndexOf(DesignItem item)
        {
            // TODO: 这里要返回索引，给Tab的时候用
            MyDesignItem xitem = CheckItemNoException(item);
            if (xitem != null)
                return _collection.IndexOf(xitem.Component);
            else
                return -1;
        }

        public void CopyTo(DesignItem[] array, int arrayIndex)
        {
            for (int i = 0; i < this.Count; i++)
            {
                array[arrayIndex + i] = this[i];
            }
        }

        public bool Remove(DesignItem item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return false;

            RemoveAt(index);

            return true;
        }

        public IEnumerator<DesignItem> GetEnumerator()
        {
            if (_collection != null)
            {
                foreach (object item in _collection)
                {
                    yield return new MyDesignItem(item, _context);
                }
            }


            //foreach (XamlPropertyValue val in property.CollectionElements)
            //{
            //    var item = GetItem(val);
            //    if (item != null)
            //        yield return item;
            //}
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        //DesignItem GetItem(XamlPropertyValue val)
        //{
        //    if (val is XamlObject)
        //    {
        //        return context._componentService.GetDesignItem(((XamlObject)val).Instance);
        //    }
        //    else
        //    {
        //        return null; //	throw new NotImplementedException();
        //    }
        //}

        private MyDesignItem CheckItem(DesignItem item)
        {
            if (item == null)
                throw new ArgumentNullException("item");
            if (item.Context != _modelProperty.DesignItem.Context)
                throw new ArgumentException("The item must belong to the same context as this collection", "item");
            MyDesignItem xitem = item as MyDesignItem;
            Debug.Assert(xitem != null);
            return xitem;
        }

        private MyDesignItem CheckItemNoException(DesignItem item) => item as MyDesignItem;

        public DesignItem this[int index]
        {
            get => new MyDesignItem(_collection[index], _context);
            set
            {
                RemoveAt(index);
                Insert(index, value);
            }
        }

        public void Insert(int index, DesignItem item) => Execute(new InsertAction(this, index, CheckItem(item)));

        public void RemoveAt(int index) => Execute(new RemoveAtAction(this, index, (MyDesignItem)this[index]));

        internal ITransactionItem CreateResetTransaction() => new ResetAction(this);

        private void Execute(ITransactionItem item)
        {
            UndoService undoService = _context.Services.GetService<UndoService>();
            if (undoService != null)
                undoService.Execute(item);
            else
                item.Do();
        }

        private void RemoveInternal(int index, MyDesignItem item)
        {
            //RemoveFromNamescopeRecursive(item);

            // TODO:这里要删除设计项，我暂时只能拿到实例
            //Debug.Assert(property.CollectionElements[index] == item.XamlObject);
            //property.CollectionElements.RemoveAt(index);
            _collection.RemoveAt(index);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
        }

        private void InsertInternal(int index, MyDesignItem item)
        {
            _collection.Insert(index, item.Component);

            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));

            //AddToNamescopeRecursive(item);
        }

        private static void RemoveFromNamescopeRecursive(MyDesignItem designItem)
        {
            throw new NotImplementedException();
            //NameScopeHelper.NameChanged(designItem.XamlObject, designItem.Name, null);

            foreach (var p in designItem.Properties)
            {
                if (p.Value != null)
                {
                    RemoveFromNamescopeRecursive((MyDesignItem)p.Value);
                }
                else if (p.IsCollection && p.CollectionElements != null)
                {
                    foreach (var c in p.CollectionElements)
                    {
                        RemoveFromNamescopeRecursive((MyDesignItem)c);
                    }
                }
            }
        }

        private static void AddToNamescopeRecursive(MyDesignItem designItem)
        {
            //NameScopeHelper.NameChanged(designItem.XamlObject, null, designItem.Name);

            foreach (var p in designItem.Properties)
            {
                if (p.Value != null)
                {
                    AddToNamescopeRecursive((MyDesignItem)p.Value);
                }
                else if (p.IsCollection && p.CollectionElements != null)
                {
                    foreach (var c in p.CollectionElements)
                    {
                        AddToNamescopeRecursive((MyDesignItem)c);
                    }
                }
            }
        }

        private sealed class InsertAction : ITransactionItem
        {
            private readonly MyModelCollectionElementsCollection collection;
            private readonly int index;
            private readonly MyDesignItem item;

            public InsertAction(MyModelCollectionElementsCollection collection, int index, MyDesignItem item)
            {
                this.collection = collection;
                this.index = index;
                this.item = item;
            }

            public ICollection<DesignItem> AffectedElements => new DesignItem[] { item };

            public string Title => "Insert into collection";

            public void Do()
            {
                collection.InsertInternal(index, item);
                collection._modelProperty.MyDesignItem.NotifyPropertyChanged(collection._modelProperty, null, item);
            }

            public void Undo()
            {
                collection.RemoveInternal(index, item);
                collection._modelProperty.MyDesignItem.NotifyPropertyChanged(collection._modelProperty, item, null);
            }

            public bool MergeWith(ITransactionItem other) => false;
        }

        private sealed class RemoveAtAction : ITransactionItem
        {
            private readonly MyModelCollectionElementsCollection collection;
            private readonly int index;
            private readonly MyDesignItem item;

            public RemoveAtAction(MyModelCollectionElementsCollection collection, int index, MyDesignItem item)
            {
                this.collection = collection;
                this.index = index;
                this.item = item;
            }

            public ICollection<DesignItem> AffectedElements => new DesignItem[] { collection._modelProperty.DesignItem };

            public string Title => "Remove from collection";

            public void Do()
            {
                collection.RemoveInternal(index, item);
                collection._modelProperty.MyDesignItem.NotifyPropertyChanged(collection._modelProperty, item, null);
            }

            public void Undo()
            {
                collection.InsertInternal(index, item);
                collection._modelProperty.MyDesignItem.NotifyPropertyChanged(collection._modelProperty, null, item);
            }

            public bool MergeWith(ITransactionItem other) => false;
        }

        private sealed class ResetAction : ITransactionItem
        {
            private readonly MyModelCollectionElementsCollection collection;
            private readonly MyDesignItem[] items;

            public ResetAction(MyModelCollectionElementsCollection collection)
            {
                this.collection = collection;

                items = new MyDesignItem[collection.Count];
                for (int i = 0; i < collection.Count; i++)
                {
                    items[i] = (MyDesignItem)collection[i];
                }
            }

            #region ITransactionItem implementation

            public void Do()
            {
                for (int i = items.Length - 1; i >= 0; i--)
                {
                    collection.RemoveInternal(i, items[i]);
                }
                collection._modelProperty.MyDesignItem.NotifyPropertyChanged(collection._modelProperty, items, null);
            }
            public void Undo()
            {
                for (int i = 0; i < items.Length; i++)
                {
                    collection.InsertInternal(i, items[i]);
                }
                collection._modelProperty.MyDesignItem.NotifyPropertyChanged(collection._modelProperty, null, items);
            }
            public bool MergeWith(ITransactionItem other)
            {
                return false;
            }

            #endregion

            #region IUndoAction implementation

            public ICollection<DesignItem> AffectedElements => new DesignItem[] { collection._modelProperty.DesignItem };

            public string Title => "Reset collection";

            #endregion
        }
    }
}
