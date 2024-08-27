using Hjmos.Lcdp.VisualEditor.Core.Services;
using System;
using System.Diagnostics;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 具有无参数构造函数并使用OnInitialize方法初始化的扩展的基类。
    /// </summary>
    public class DefaultExtension : Extension
    {
        private DesignItem _extendedItem;

        /// <summary>获取由BehaviorExtension扩展的设计项</summary>
        public DesignItem ExtendedItem => _extendedItem ?? throw new InvalidOperationException("不能访问BehaviorExtension.ExtendedItem: 属性尚未初始化。请将依赖于ExtendedItem的初始化逻辑移动到OnInitialized方法中。");

        /// <summary>获取扩展项的设计上下文。 “Context”相当于“ExtendedItem.Context”</summary>
        public DesignContext Context => ExtendedItem.Context;

        /// <summary>获取扩展项的服务容器。“Services”等同于“ExtendedItem.Services”</summary>
        public ServiceContainer Services => ExtendedItem.Services;

        /// <summary>
        /// 在设置ExtendedItem之后调用
        /// 重写此方法以将您的行为注册到项目中
        /// </summary>
        protected virtual void OnInitialized() { }

        /// <summary>在删除扩展时调用</summary>
        protected virtual void OnRemove() { }

        internal void CallOnRemove() => OnRemove();

        internal void InitializeDefaultExtension(DesignItem extendedItem)
        {
            Debug.Assert(_extendedItem == null);
            Debug.Assert(extendedItem != null);

            _extendedItem = extendedItem;
            OnInitialized();
        }
    }
}
