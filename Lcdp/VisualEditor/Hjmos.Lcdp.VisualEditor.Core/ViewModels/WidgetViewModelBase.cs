using Hjmos.Lcdp.Common;
using Prism.Mvvm;
using System;

namespace Hjmos.Lcdp.VisualEditor.Core.ViewModels
{
    public class WidgetViewModelBase : BindableBase
    {
        /// <summary>
        /// 定义附加属性改变事件
        /// </summary>
        public event Action<IEventParameters> AttachedPropertyChanged;

        /// <summary>
        /// 触发附加属性改变事件
        /// </summary>
        /// <param name="parameters">事件参数</param>
        public void RaiseAttachedPropertyChanged(IEventParameters parameters) => AttachedPropertyChanged?.Invoke(parameters);
    }
}
