using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Markup;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    public sealed class MyComponentService : IComponentService
    {
        private readonly MyDesignContext _context;

        public MyComponentService(MyDesignContext context) => _context = context;

        public event EventHandler<DesignItemEventArgs> ComponentRegisteredAndAddedToContainer;
        public event EventHandler<DesignItemEventArgs> ComponentRegistered;
        public event EventHandler<DesignItemEventArgs> ComponentRemoved;
        public event EventHandler<DesignItemPropertyChangedEventArgs> PropertyChanged;

        #region IdentityEqualityComparer
        private sealed class IdentityEqualityComparer : IEqualityComparer<object>
        {
            internal static readonly IdentityEqualityComparer Instance = new();

            int IEqualityComparer<object>.GetHashCode(object obj) => RuntimeHelpers.GetHashCode(obj);

            bool IEqualityComparer<object>.Equals(object x, object y) => x == y;
        }
        #endregion

        // 待办事项:这不能是一个字典，因为没有办法取消注册组件
        // 然而，这并不重要，因为如果我们不限制撤销堆栈，我们的设计项将在设计师的生命周期内一直存在。
        private readonly Dictionary<object, MyDesignItem> _sites = new(IdentityEqualityComparer.Instance);

        public DesignItem GetDesignItem(object component) => GetDesignItem(component, false);

        public DesignItem GetDesignItem(object component, bool findByView)
        {
            if (component == null)
                throw new ArgumentNullException("component");

            _sites.TryGetValue(component, out MyDesignItem site);

            if (findByView)
            {
                site ??= _sites.Values.FirstOrDefault(x => Equals(x.View, component));
            }
            return site;
        }

        public DesignItem RegisterComponentForDesigner(object component) => RegisterComponentForDesigner(null, component);

        public DesignItem RegisterComponentForDesigner(DesignItem parent, object component)
        {
            if (component == null)
            {
                component = new NullExtension();
            }
            else if (component is Type type)
            {
                component = new TypeExtension(type);
            }

            //XamlObject parentXamlObject = null;
            //if (parent != null)
            //    parentXamlObject = ((MyDesignItem)parent).XamlObject;

            MyDesignItem item = new(component, _context);
            _context.Services.ExtensionManager.ApplyDesignItemInitializers(item);

            if (component is not string)
                _sites.Add(component, item);
            ComponentRegistered?.Invoke(this, new DesignItemEventArgs(item));
            return item;
        }

        public DesignItem RegisterComponentForDesignerRecursiveUsingXaml(object component) => throw new NotImplementedException();

        public void SetDefaultPropertyValues(DesignItem designItem)
        {
            var values = Metadata.GetDefaultPropertyValues(designItem.ComponentType);
            if (values != null)
            {
                foreach (var value in values)
                {
                    designItem.Properties[value.Key].SetValue(value.Value);
                }
            }
        }

        /// <summary>
        /// registers components from an existing XAML tree
        /// </summary>
        internal void RaiseComponentRegisteredAndAddedToContainer(DesignItem obj) => ComponentRegisteredAndAddedToContainer?.Invoke(this, new DesignItemEventArgs(obj));

        /// <summary>
        /// registers components from an existing XAML tree
        /// 
        /// 注册来自现有XAML树的组件
        /// </summary>
        internal MyDesignItem RegisterXamlComponentRecursive(object obj)
        {
            if (obj == null) return null;


            MyDesignItem site = new(obj, _context);
            _context.Services.ExtensionManager.ApplyDesignItemInitializers(site);

            _sites.Add(site.Component, site);

            ComponentRegistered?.Invoke(this, new DesignItemEventArgs(site));

            return site;
        }

        /// <summary>
        /// 递归注册组件（TODO:模仿RegisterXamlComponentRecursive写的）
        /// </summary>
        internal MyDesignItem RegisterComponentRecursive(object obj)
        {
            if (obj == null) return null;

            if (_sites.ContainsKey(obj)) return null;

            MyDesignItem site = new(obj, _context);
            _context.Services.ExtensionManager.ApplyDesignItemInitializers(site);


            _sites.Add(site.Component, site);

            ComponentRegistered?.Invoke(this, new DesignItemEventArgs(site));

            // 递归注册子组件  TODO：暂时乱写的，后续确定这样行不行，需不需要进一步优化
            if (typeof(Panel).IsAssignableFrom(obj.GetType()))
            {
                Panel panel = obj as Panel;
                foreach (object item in panel.Children)
                {
                    RegisterComponentRecursive(item);
                }
            }

            return site;
        }

        /// <summary>
        /// 触发属性改变事件
        /// </summary>
        internal void RaisePropertyChanged(MyModelProperty property, object oldValue, object newValue)
        {
            PropertyChanged?.Invoke(this, new DesignItemPropertyChangedEventArgs(property.DesignItem, property, oldValue, newValue));
        }
        /// <summary>
        /// 触发RaiseComponentRemoved事件
        /// </summary>
        internal void RaiseComponentRemoved(DesignItem item)
        {
            this.ComponentRemoved?.Invoke(this, new DesignItemEventArgs(item));
        }

    }
}
