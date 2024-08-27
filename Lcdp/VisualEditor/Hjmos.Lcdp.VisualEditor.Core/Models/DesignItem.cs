using Hjmos.Lcdp.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.ItemExtensions;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// DesignItem将组件与服务系统和设计器连接起来。 
    /// 相当于Cider的ModelItem。
    /// </summary>
    public abstract class DesignItem : INotifyPropertyChanged
    {
        /// <summary>拖放后的初始位置</summary>
        public Point Position { get; set; }

        /// <summary>获取DesignItem所包装的组件</summary>
        public abstract object Component { get; }

        /// <summary>
        /// 获取DesignItem包装的组件的类型
        /// 如果CustomInstanceFactory使用不同的类型创建对象(例如ComponentType=Window，但Component.GetType()=WindowClone)，则此值可能与Component.GetType()不同。  
        /// </summary>
        public abstract Type ComponentType { get; }

        /// <summary>获取组件使用的视图</summary>
        public abstract UIElement View { get; }

        /// <summary>
        /// 设置组件的视图
        /// </summary>
        /// <param name="newView"></param>
        public abstract void SetView(UIElement newView);

        /// <summary>获取设计上下文</summary>
        public abstract DesignContext Context { get; }

        /// <summary>获取父设计项</summary>
        public abstract DesignItem Parent { get; }

        /// <summary>父项更改事件</summary>
        public abstract event EventHandler ParentChanged;

        /// <summary>获取将此DesignItem用作值的属性</summary>
        public abstract DesignItemProperty ParentProperty { get; }

        /// <summary>获取设计项上设置的属性</summary>
        public abstract DesignItemPropertyCollection Properties { get; }

        /// <summary>获取设计项上设置的属性</summary>
        public abstract IEnumerable<DesignItemProperty> AllSetProperties { get; }

        /// <summary>获取/设置设计项的名称</summary>
        public abstract string Name { get; set; }

        /// <summary>获取/设置设计项上的“x:Key”属性的值</summary>
        public abstract string Key { get; set; }

        /// <summary>名称改变事件</summary>
        public abstract event EventHandler NameChanged;

        /// <summary>
        /// 获取一个实例，该实例为最常用的设计器提供方便的属性
        /// </summary>
        public ServiceContainer Services
        {
            [DebuggerStepThrough]
            get => this.Context.Services;
        }

        ///// <summary>
        ///// 打开一个用于批处理多个更改的新更改组。
        ///// ChangeGroups作为事务工作，用于支持撤消/重做系统。
        ///// 注意:ChangeGroup适用于整个<see cref="DesignContext"/>，而不仅仅是这个项目! 
        ///// </summary>
        //public ChangeGroup OpenGroup(string changeGroupTitle)
        //{
        //	return this.Context.OpenGroup(changeGroupTitle, new DesignItem[] { this });
        //}

        #region Extensions support

        [DebuggerDisplay("ExtensionEntry - {Extension} / {Server}")]
        private struct ExtensionEntry
        {
            internal readonly Extension Extension;
            internal readonly ExtensionServer Server;

            public ExtensionEntry(Extension extension, ExtensionServer server)
            {
                this.Extension = extension;
                this.Server = server;
            }
        }

        ExtensionServer[] _extensionServers;
        bool[] _extensionServerIsApplied;
        readonly List<ExtensionEntry> _extensions = new();

        /// <summary>
        /// 获取为设计项注册的扩展
        /// </summary>
        public IEnumerable<Extension> Extensions => _extensions.Select(x => x.Extension).ToList();

        internal void SetExtensionServers(ExtensionManager extensionManager, ExtensionServer[] extensionServers)
        {
            Debug.Assert(_extensionServers == null);
            Debug.Assert(extensionServers != null);

            _extensionServers = extensionServers;
            _extensionServerIsApplied = new bool[extensionServers.Length];

            for (int i = 0; i < _extensionServers.Length; i++)
            {
                bool shouldApply = _extensionServers[i].ShouldApplyExtensions(this);
                if (shouldApply != _extensionServerIsApplied[i])
                {
                    _extensionServerIsApplied[i] = shouldApply;
                    ApplyUnapplyExtensionServer(extensionManager, shouldApply, _extensionServers[i]);
                }
            }
        }

        /// <summary>
        /// 重新设置设计项的扩展服务
        /// </summary>
        /// <param name="extensionManager"></param>
        /// <param name="server"></param>
        internal void ReapplyExtensionServer(ExtensionManager extensionManager, ExtensionServer server)
        {
            Debug.Assert(_extensionServers != null);

            for (int i = 0; i < _extensionServers.Length; i++)
            {
                if (_extensionServers[i] == server)
                {
                    bool shouldApply = server.ShouldApplyExtensions(this);

                    if (server.ShouldBeReApplied() && shouldApply && shouldApply == _extensionServerIsApplied[i])
                    {
                        _extensionServerIsApplied[i] = false;
                        ApplyUnapplyExtensionServer(extensionManager, false, server);
                    }

                    if (shouldApply != _extensionServerIsApplied[i])
                    {
                        _extensionServerIsApplied[i] = shouldApply;
                        ApplyUnapplyExtensionServer(extensionManager, shouldApply, server);
                    }
                }
            }
        }

        private void ApplyUnapplyExtensionServer(ExtensionManager extensionManager, bool shouldApply, ExtensionServer server)
        {
            if (shouldApply)
            {
                // 添加扩展
                foreach (Extension ext in extensionManager.CreateExtensions(server, this))
                {
                    _extensions.Add(new ExtensionEntry(ext, server));
                }
            }
            else
            {
                // 移除扩展
                _extensions.RemoveAll(
                    entry =>
                    {
                        if (entry.Server == server)
                        {
                            server.RemoveExtension(entry.Extension);
                            return true;
                        }

                        return false;
                    });
            }
        }

        /// <summary>
        /// 移除指定的扩展
        /// </summary>
        public void RemoveExtension(Extension extension)
        {
            var hasExtension = this._extensions.Any(x => x.Extension.GetType() == extension.GetType());

            if (hasExtension)
            {
                var extensionEntry = this._extensions.FirstOrDefault(x => x.Extension.GetType() == extension.GetType());
                //_extensions.Remove(extensionEntry);
                extensionEntry.Server.RemoveExtension(extensionEntry.Extension);
            }
        }

        /// <summary>
        /// 重新应用所有扩展
        /// </summary>
        public void ReapplyAllExtensions()
        {
            ExtensionManager manager = this.Services.GetService<ExtensionManager>();
            List<ExtensionServer> servers = _extensions.GroupBy(entry => entry.Server).Select(grp => grp.First().Server).ToList();

            foreach (ExtensionServer server in servers)
            {
                ApplyUnapplyExtensionServer(manager, false, server);
                ApplyUnapplyExtensionServer(manager, true, server);
            }
        }

        /// <summary>
        /// 重新应用指定的扩展
        /// </summary>
        public void ReapplyExtension(Type extensionType)
        {
            ExtensionManager manager = this.Services.GetService<ExtensionManager>();
            List<ExtensionServer> servers = _extensions.GroupBy(entry => entry.Server).Select(grp => grp.First().Server).ToList();

            foreach (ExtensionServer server in servers)
            {
                _extensions.RemoveAll(
                    entry =>
                    {
                        if (entry.Server == server && entry.Extension.GetType() == extensionType)
                        {
                            server.RemoveExtension(entry.Extension);
                            return true;
                        }

                        return false;
                    });

                foreach (Extension ext in manager.CreateExtensions(server, this, extensionType))
                {
                    _extensions.Add(new ExtensionEntry(ext, server));
                }
            }
        }

        #endregion

        #region Manage behavior
        private readonly Dictionary<Type, object> _behaviorObjects = new();

        /// <summary>
        /// 向此设计项添加行为扩展对象。
        /// </summary>
        public void AddBehavior(Type behaviorInterface, object behaviorImplementation)
        {
            if (behaviorInterface == null)
                throw new ArgumentNullException("behaviorInterface");
            if (behaviorImplementation == null)
                throw new ArgumentNullException("behaviorImplementation");
            if (!behaviorInterface.IsInstanceOfType(behaviorImplementation))
                throw new ArgumentException("behaviorImplementation must implement bevahiorInterface", "behaviorImplementation");

            _behaviorObjects[behaviorInterface] = behaviorImplementation;
        }

        /// <summary>
        /// 从设计项获取行为扩展对象
        /// </summary>
        /// <returns>行为对象，如果未找到则为空。</returns>
        public T GetBehavior<T>() where T : class
        {
            _behaviorObjects.TryGetValue(typeof(T), out object obj);
            return (T)obj;
        }
        #endregion

        /// <summary>
        /// 当DesignItem上的模型属性发生更改时，将引发此事件。
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// 引发<see cref="PropertyChanged"/>事件。
        /// </summary>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        /// <summary>
        /// 获取内容属性(包含逻辑子属性的属性)的名称
        /// </summary>
        public abstract string ContentPropertyName { get; }

        /// <summary>
        /// 获取内容属性(包含逻辑子元素的属性)
        /// </summary>
        public DesignItemProperty ContentProperty => ContentPropertyName is null ? null : Properties[ContentPropertyName];

        /// <summary>
        /// 从其父属性/集合中移除此设计项
        /// </summary>
        public void Remove()
        {
            if (ParentProperty != null)
            {
                if (ParentProperty.IsCollection)
                {
                    ParentProperty.CollectionElements.Remove(this);
                }
                else
                {
                    ParentProperty.Reset();
                }
            }
        }

        /// <summary>
        /// 创建此设计项的副本。
        /// </summary>
        public abstract DesignItem Clone();

        /// <summary>
        /// 获取一个<see cref="Transform"/>，它表示应用于项的视图的所有转换。
        /// </summary>
        public Transform GetCompleteAppliedTransformationToView()
        {
            TransformGroup retVal = new();
            Visual view = this.View;
            while (view != null)
            {
                FrameworkElement frameworkElement = view as FrameworkElement;

                if (frameworkElement != null && frameworkElement.LayoutTransform != null)
                    retVal.Children.Add(frameworkElement.LayoutTransform);

                if (frameworkElement != null && frameworkElement.RenderTransform != null)
                    retVal.Children.Add(frameworkElement.RenderTransform);

                if (view is ContainerVisual visual && visual.Transform != null)
                {
                    retVal.Children.Add(visual.Transform);
                }
                view = view.TryFindParent<Visual>(true);
            }

            return retVal;
        }

        public int DepthLevel
        {
            get
            {
                int j = 0;
                var x = this.Parent;
                while (x != null)
                {
                    j++;
                    x = x.Parent;
                }
                return j;
            }
        }
    }
}
