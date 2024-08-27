using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Core.Managers;
using Prism.Ioc;
using Prism.Mvvm;
using System;

namespace Hjmos.Lcdp.VisualEditor.Core.ViewModels
{
    public class ViewModelBase : BindableBase
    {
        /// <summary>
        /// 获取全局状态对象StateManager的实例
        /// </summary>
        public StateManager State { get; private set; } = ContainerLocator.Current.Resolve<StateManager>();

        /// <summary>
        /// 平台页面API对象
        /// </summary>
        public IPageApi PageApi { get; set; }

        public ViewModelBase()
        {
            PageApi = ContainerLocator.Current.Resolve<IPageApi>();
        }

        /// <summary>
        /// 定义附加属性改变事件
        /// </summary>
        public event Action<IEventParameters> AttachedPropertyChanged;

        /// <summary>
        /// 触发附加属性改变事件
        /// </summary>
        /// <param name="parameters"></param>
        public void RaiseAttachedPropertyChanged(IEventParameters parameters) => AttachedPropertyChanged?.Invoke(parameters);
    }
}
