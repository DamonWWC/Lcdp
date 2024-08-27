using Hjmos.Lcdp.VisualEditor.Core.Controls;
using Hjmos.Lcdp.VisualEditor.Models;
using System.Collections.ObjectModel;

namespace Hjmos.Lcdp.VisualEditor.Core.Interfaces
{
    /// <summary>
    /// 平台页面API接口
    /// </summary>
    public interface IPageApi
    {
        /// <summary>
        /// 应用ID
        /// </summary>
        int AppId { get; set; }

        /// <summary>
        /// 是否设计时模式
        /// </summary>
        bool IsDesignMode { get; set; }

        /// <summary>
        /// 变量列表
        /// </summary>
        ObservableCollection<ParameterModel> ParameterList { get; set; }

        /// <summary>
        /// 设置变量值
        /// </summary>
        /// <param name="variableName">变量名</param>
        /// <param name="newValue">新变量值</param>
        /// <param name="widgetGuid">组件Guid</param>
        void SetParameter(string variableName, string newValue, string widgetGuid);

        /// <summary>
        /// 获取变量值
        /// </summary>
        /// <param name="name">变量名</param>
        string GetParameter(string name);
    }
}
