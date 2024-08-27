using System;
using System.Diagnostics;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions
{
    /// <summary>
    /// ExtensionServer管理特定扩展类型的扩展的创建和删除。
    /// 对于给定的DesignContext, ExtensionServer只创建一次。
    /// ExtensionServer可以处理由服务引发的事件，而不必注销其事件处理程序，因为ExtensionServer在DesignContext的生命周期内运行。
    /// </summary>
    public abstract class ExtensionServer
    {
        private DesignContext _context;

        /// <summary>
        /// 获取创建此扩展服务的上下文
        /// </summary>
        public DesignContext Context
        {
            [DebuggerStepThrough]
            get
            {
                string message = "无法访问 ExtensionServer.Context: 属性尚未初始化。请将依赖于Context的初始化逻辑移动到OnInitialized方法中。";
                return _context ?? throw new InvalidOperationException(message);
            }
        }

        /// <summary>
        /// 获取当前上下文的服务容器。“x.Services”等价于“x.Context.Services”。
        /// </summary>
        public ServiceContainer Services
        {
            [DebuggerStepThrough]
            get => this.Context.Services;
        }

        /// <summary>
        /// 初始化扩展服务（在扩展服务初始化后，并且上下文已设置后调用）
        /// </summary>
        /// <param name="context"></param>
        internal void InitializeExtensionServer(DesignContext context)
        {
            Debug.Assert(this._context == null);
            Debug.Assert(context != null);

            this._context = context;
            OnInitialized();
        }

        /// <summary>
        /// 在扩展服务初始化后，并且<see cref="Context"/>属性已设置时调用。
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>
        /// 获取扩展管理器是否应该将来自此服务的扩展应用到指定项。
        /// 由ExtensionManager调用。
        /// </summary>
        public abstract bool ShouldApplyExtensions(DesignItem extendedItem);

        /// <summary>
        /// 返回是否应该重新应用扩展服务(例如，对于多选扩展服务)。
        /// </summary>
        public virtual bool ShouldBeReApplied() => false;

        /// <summary>
        /// 创建指定类型的扩展。
        /// 由ExtensionManager调用。
        /// </summary>
        public abstract Extension CreateExtension(Type extensionType, DesignItem extendedItem);

        /// <summary>
        /// 在从设计项中删除扩展之前调用此方法，因为不应该再应用该扩展。
        /// 由ExtensionManager调用。
        /// </summary>
        public abstract void RemoveExtension(Extension extension);

        /// <summary>
        /// 当一个设计项集合的ShouldApplyExtensions失效时将引发此事件。
        /// </summary>
        public abstract event EventHandler<DesignItemCollectionEventArgs> ShouldApplyExtensionsInvalidated;
    }
}
