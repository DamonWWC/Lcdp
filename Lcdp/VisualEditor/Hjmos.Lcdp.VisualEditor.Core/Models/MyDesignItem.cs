using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using Hjmos.Lcdp.VisualEditor.Core.XamlDom;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// DesignItem将组件与服务系统和设计器连接起来。 
    /// </summary>
    public class MyDesignItem : DesignItem
    {
        /// <summary>设计项所包装的实例</summary>
        private readonly object _obj;
        /// <summary>设计器上下文</summary>
        private readonly MyDesignContext _designContext;
        /// <summary>设计项属性的集合</summary>
        private readonly MyModelPropertyCollection _properties;
        private UIElement _view;
        /// <summary>设计项所有属性描述的集合</summary>
        internal IEnumerable<PropertyDescriptor> PropertyDescriptorCollection { get; }


        public MyDesignItem(object obj, MyDesignContext designContext)
        {
            _obj = obj;
            _designContext = designContext;
            // 获取设计项所有属性描述的集合
            PropertyDescriptorCollection = TypeHelper.GetPropertyDescriptorCollection(_obj as DependencyObject);



            // 获取设计项属性的集合
            _properties = new MyModelPropertyCollection(this);
        }

        public override UIElement View => _view ?? this.Component as UIElement;

        public override object Component => _obj;

        public override Type ComponentType => _obj.GetType();

        public override DesignContext Context => _designContext;

        internal MyComponentService ComponentService => _designContext._componentService;

        /// <summary>
        /// 项目在设计时被锁定
        /// </summary>
        public bool IsDesignTimeLocked
        {
            get
            {
                object locked = Properties.GetAttachedProperty(DesignTimeProperties.IsLockedProperty).ValueOnInstance;
                return locked != null && (bool)locked == true;
            }
            set
            {
                if (value)
                    Properties.GetAttachedProperty(DesignTimeProperties.IsLockedProperty).SetValue(true);
                else
                    Properties.GetAttachedProperty(DesignTimeProperties.IsLockedProperty).Reset();
            }

        }



        public override DesignItem Parent
        {
            get
            {
                DependencyObject parent = VisualTreeHelper.GetParent(_obj as DependencyObject);
                return parent is null ? null : ComponentService.GetDesignItem(parent);
            }
        }

        /// <summary>
        /// TODO：父属性一般是Children，这一个后续要精确处理一下
        /// </summary>
        public override DesignItemProperty ParentProperty => this.Parent?.Properties.GetProperty("Children");


        public override DesignItemPropertyCollection Properties => _properties;

        public override IEnumerable<DesignItemProperty> AllSetProperties => throw new NotImplementedException();

        public override string Name { get; set; }

        public override string Key { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override string ContentPropertyName => "Children"; // TODO: 应该从集合的ContentProperty特性中获取

        public override event EventHandler ParentChanged;
        public override event EventHandler NameChanged;

        public override DesignItem Clone()
        {
            throw new NotImplementedException();
        }

        public override void SetView(UIElement newView) => _view = newView;

        internal void NotifyPropertyChanged(MyModelProperty property, object oldValue, object newValue)
        {
            Debug.Assert(property != null);
            OnPropertyChanged(new PropertyChangedEventArgs(property.Name));

            ((MyComponentService)this.Services.Component).RaisePropertyChanged(property, oldValue, newValue);
        }
    }
}
