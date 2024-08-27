using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Core.Controls;
using Hjmos.Lcdp.VisualEditor.Core.Events;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Models;
using Prism.Events;
using Prism.Ioc;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 平台页面API
    /// TODO：梳理API中不开放给用户的方法，放到单独的类
    /// </summary>
    public class PageApi : IPageApi
    {
        public PageApi() { }

        private readonly IEventAggregator _ea = ContainerLocator.Current.Resolve<IEventAggregator>();

        /// <summary>
        /// 应用ID
        /// </summary>
        public int AppId { get; set; } = 0;

        /// <summary>
        /// 是否设计时模式
        /// </summary>
        public bool IsDesignMode { get; set; } = false;

        /// <summary>
        /// 页面容器
        /// </summary>
        public PageShell PageShell { get; set; }

        /// <summary>
        /// 变量列表
        /// </summary>
        public ObservableCollection<ParameterModel> ParameterList { get; set; } = new();

        /// <summary>
        /// 变量管理中介器
        /// </summary>
        private readonly ParameterMediator _mediator = new();

        /// <summary>
        /// 设置变量值
        /// </summary>
        /// <param name="variableName">变量名</param>
        /// <param name="newValue">新变量值</param>
        /// <param name="widgetGuid">组件Guid</param>
        public void SetParameter(string variableName, string newValue, string widgetGuid)
        {
            if (AppId <= 0)
            {
                MessageBox.Show("请先打开一个模板");
                return;
            }

            ParameterModel model = this.ParameterList.FirstOrDefault(x => x.Name == variableName);
            if (model != null && model.Value != newValue)
            {
                string oldValue = model.Value;
                model.Value = newValue;

                // 事件传参
                IEventParameters parameters = new EventParameters {
                    { "VariableName", variableName },
                    { "WidgetGuid", widgetGuid },
                    { "OldValue", oldValue },
                    { "NewValue", newValue }
                };
                _ea.GetEvent<VariableEvent>().Publish(parameters);

            }

        }

        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <param name="name">变量名</param>
        public string GetParameter(string name)
        {
            ParameterModel model = this.ParameterList.FirstOrDefault(x => x.Name == name);
            if (model != null)
            {
                return model.Value;
            }
            return null;
        }
    }
}
