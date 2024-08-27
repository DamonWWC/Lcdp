using Hjmos.Lcdp.VisualEditor.Models;
using System;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 变量管理中介
    /// </summary>
    public class ParameterMediator
    {
        /// <summary>
        /// TODO：Action<ParameterModel>考虑改为EventHandler，返回变量的新值和旧值
        /// </summary>
        private readonly Dictionary<string, Action<ParameterModel>> _watchList = new();

        /// <summary>
        /// 变量改变事件订阅
        /// </summary>
        /// <param name="parameterName">变量名称</param>
        /// <param name="action">回调函数</param>
        public void Subscribe(string parameterName, Action<ParameterModel> action)
        {
            if (_watchList.ContainsKey(parameterName))
            {
                _watchList[parameterName] += action;
            }
            else
            {
                _watchList.Add(parameterName, action);
            }

        }

        /// <summary>
        /// 变量改变事件发布
        /// </summary>
        /// <param name="parameterModel">变量</param>
        public void Publish(ParameterModel parameterModel)
        {
            if (_watchList.ContainsKey(parameterModel.Name))
            {
                _watchList[parameterModel.Name]?.Invoke(parameterModel);
            }
        }
    }
}
