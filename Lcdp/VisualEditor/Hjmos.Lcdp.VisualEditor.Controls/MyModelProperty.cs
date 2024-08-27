using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Markup.Primitives;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 设计项的一个属性
    /// </summary>
    [DebuggerDisplay("MyModelProperty: {Name}")]
    internal sealed class MyModelProperty : DesignItemProperty, IEquatable<MyModelProperty>
    {
        private readonly MyDesignItem _designItem;
        private readonly MyModelCollectionElementsCollection _collectionElements;

        public MyModelProperty(MyDesignItem designItem, string propertyName)
        {
            Debug.Assert(designItem != null);

            _designItem = designItem;
            this.Name = propertyName;

            if (propertyName is "ColumnDefinitions" or "RowDefinitions" or "Children" or "Inlines")// property.IsCollection
            {
                IsCollection = true;
                _collectionElements = new MyModelCollectionElementsCollection(this);
            }


            ValueChanged += (x, y) =>
            {
                OnPropertyChanged("Value");
                OnPropertyChanged("DesignerValue");
                OnPropertyChanged("ValueOnInstanceOrView");
            };
            ValueOnInstanceChanged += (x, y) =>
            {
                OnPropertyChanged("ValueOnInstance");
                OnPropertyChanged("ValueOnInstanceOrView");
            };
        }

        public bool Equals(MyModelProperty other) => throw new NotImplementedException();

        public override string Name { get; protected set; }

        public override bool IsCollection { get; }

        public override bool IsEvent => throw new NotImplementedException();

        public override Type ReturnType
        {
            get
            {
                // TODO：获取依赖属性的值多处使用，考虑封装一下
                // 根据属性名称查找依赖属性描述器
                PropertyDescriptor descriptor = _designItem.PropertyDescriptorCollection.FirstOrDefault(x =>
                {
                    // 截取.号的最后一部分
                    int index = x.Name.LastIndexOf('.');
                    string name = x.Name.Substring(index + 1);

                    return name == Name;
                });

                return descriptor?.PropertyType;
            }
        }

        // TODO:ReturnType和DeclaringType各自什么Type？还没有自己区别，随便返回的
        // DeclaringType返回的应该是命名控件，比如Grid添加列的时候，需要一个Event的FullName：System.Windows.FrameworkContentElement.TargetUpdated，这里返回System.Windows.FrameworkContentElement
        public override Type DeclaringType
        {
            get
            {
                // TODO：获取依赖属性的值多处使用，考虑封装一下
                // 根据属性名称查找依赖属性描述器
                PropertyDescriptor descriptor = _designItem.PropertyDescriptorCollection.FirstOrDefault(x =>
                {
                    // 截取.号的最后一部分
                    int index = x.Name.LastIndexOf('.');
                    string name = x.Name.Substring(index + 1);

                    return name == Name;
                });

                return descriptor?.ComponentType;
            }
        }


        public override string Category => throw new NotImplementedException();

        public override TypeConverter TypeConverter => throw new NotImplementedException();

        public override IObservableList<DesignItem> CollectionElements => IsCollection ? _collectionElements : throw new DesignerException("Cannot access CollectionElements for non-collection properties.");

        public override DesignItem Value
        {
            get
            {
                if (ValueOnInstance != null)
                {
                    DesignItem designItem = _designItem.ComponentService.GetDesignItem(ValueOnInstance);
                    return designItem ?? new MyDesignItem(ValueOnInstance, _designItem.Context as MyDesignContext);
                }
                else
                {
                    return null;
                }

            }
        }



        public override string TextValue => throw new NotImplementedException();

        internal void SetValueOnInstance(object value) => throw new NotImplementedException();

        public override event EventHandler ValueChanged;

        public override event EventHandler ValueOnInstanceChanged;

        /// <summary>
        /// 获取/设置设计实例上的属性值。
        /// 如果未设置属性，则返回默认值。
        /// </summary>
        public override object ValueOnInstance
        {
            get
            {
                // TODO：获取依赖属性的值多处使用，考虑封装一下
                // 根据属性名称查找依赖属性描述器
                PropertyDescriptor descriptor = _designItem.PropertyDescriptorCollection.FirstOrDefault(x =>
                {
                    // 截取.号的最后一部分
                    int index = x.Name.LastIndexOf('.');
                    string name = x.Name.Substring(index + 1);

                    return name == Name;
                });

                return descriptor?.GetValue(_designItem.Component);
            }
        }

        public override object DesignerValue => throw new NotImplementedException();

        /// <summary>
        /// 获取是否在设计项上设置了属性。
        /// </summary>
        public override bool IsSet => MarkupWriter.GetMarkupObjectFor(_designItem.Component).Properties.Any(x => x.Name == Name);


        public override event EventHandler IsSetChanged;

        /// <summary>
        /// 为设计项的属性赋值
        /// </summary>
        /// <param name="value"></param>
        public override void SetValue(object value)
        {

            // TODO：获取依赖属性的值多处使用，考虑封装一下
            // 根据属性名称查找依赖属性描述器
            PropertyDescriptor descriptor = _designItem.PropertyDescriptorCollection.FirstOrDefault(x =>
            {
                // 截取.号的最后一部分
                int index = x.Name.LastIndexOf('.');
                string name = x.Name.Substring(index + 1);

                return name == Name;
            });

            // 设置新值
            descriptor?.SetValue(_designItem.Component, value);

            // TODO：这里通知的机制关注一下
            // 触发属性值改变通知
            _designItem.NotifyPropertyChanged(this, descriptor?.GetValue(_designItem.Component), value);

            //XamlPropertyValue newValue;
            //if (value == null)
            //{
            //    newValue = _property.ParentObject.OwnerDocument.CreateNullValue();
            //}
            //else
            //{
            //    XamlComponentService componentService = _designItem.ComponentService;

            //    XamlDesignItem designItem = value as XamlDesignItem;
            //    if (designItem == null) designItem = (XamlDesignItem)componentService.GetDesignItem(value);
            //    if (designItem != null)
            //    {
            //        if (designItem.Parent != null)
            //            throw new DesignerException("Cannot set value to design item that already has a parent");
            //        newValue = designItem.XamlObject;
            //    }
            //    else
            //    {
            //        XamlPropertyValue val = _property.ParentObject.OwnerDocument.CreatePropertyValue(value, _property);
            //        designItem = componentService.RegisterXamlComponentRecursive(val as XamlObject);
            //        newValue = val;
            //    }
            //}

            //UndoService undoService = _designItem.Services.GetService<UndoService>();
            //if (undoService != null)
            //    undoService.Execute(new PropertyChangeAction(this, newValue, true));
            //else
            //    SetValueInternal(newValue);
        }

        //void SetValueInternal(XamlPropertyValue newValue)
        //{
        //    var oldValue = _property.PropertyValue;
        //    _property.PropertyValue = newValue;
        //    _designItem.NotifyPropertyChanged(this, oldValue, newValue);
        //}


        /// <summary>
        /// 将属性值重置为默认值，可能将其从属性列表中删除。
        /// </summary>
        public override void Reset()
        {
            //UndoService undoService = _designItem.Services.GetService<UndoService>();
            //if (undoService != null)
            //    undoService.Execute(new PropertyChangeAction(this, null, false));
            //else

            if (this.IsSet)
            {
                object oldValue = ValueOnInstance;


                // TODO：获取依赖属性的值多处使用，考虑封装一下
                // 根据属性名称查找依赖属性描述器
                PropertyDescriptor descriptor = _designItem.PropertyDescriptorCollection.FirstOrDefault(x =>
                {
                    // 截取.号的最后一部分
                    int index = x.Name.LastIndexOf('.');
                    string name = x.Name.Substring(index + 1);

                    return name == Name;
                });

                // 设置默认值
                descriptor?.ResetValue(_designItem.Component);

                // TODO：这里通知的机制关注一下
                // 触发属性值改变通知
                _designItem.NotifyPropertyChanged(this, oldValue, descriptor?.GetValue(_designItem.Component));

            }
        }

        public override DesignItem DesignItem => _designItem;
        internal MyDesignItem MyDesignItem => _designItem;

        public override DependencyProperty DependencyProperty
        {
            get
            {
                // TODO：获取依赖属性的值多处使用，考虑封装一下
                // 根据属性名称查找依赖属性描述器
                PropertyDescriptor descriptor = _designItem.PropertyDescriptorCollection.FirstOrDefault(x =>
                {
                    // 截取.号的最后一部分
                    int index = x.Name.LastIndexOf('.');
                    string name = x.Name.Substring(index + 1);

                    return name == Name;
                });

                if (descriptor is null) return null;

                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(descriptor);
                if (dpd.DependencyProperty != null)
                {
                    return dpd.DependencyProperty;
                }

                return null;
            }
        }

        public override bool IsAdvanced => throw new NotImplementedException();
    }
}
