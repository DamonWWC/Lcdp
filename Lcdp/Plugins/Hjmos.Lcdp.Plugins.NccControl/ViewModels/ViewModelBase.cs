using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Core.Managers;
using Prism.Mvvm;
using Prism.Unity;
using System;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.NccControl.ViewModels
{
    public class ViewModelBase : BindableBase
    {
        public StateManager State { get; private set; }


        public ViewModelBase()
        {
            var container = (Application.Current as PrismApplication).Container;
            State = container.Resolve(typeof(StateManager)) as StateManager;
        }

        /// <summary>
        /// 定义附加属性改变事件
        /// TODO：AttachedPropertyChanged中发多个if，考虑应用设计模式优化
        /// </summary>
        public event Action<IEventParameters> AttachedPropertyChanged;

        /// <summary>
        /// 触发附加属性改变事件
        /// </summary>
        /// <param name="parameters"></param>
        public void RaiseAttachedPropertyChanged(IEventParameters parameters) => AttachedPropertyChanged?.Invoke(parameters);
    }
}
