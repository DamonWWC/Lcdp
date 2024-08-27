using Hjmos.Lcdp.Common;
using System;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditor.Core.Interfaces
{
    /// <summary>
    /// 组件接口
    /// </summary>
    public interface IWidget : IUnit
    {
        /// <summary>
        /// 用来关联全局参数的组件字段列表
        /// </summary>
        List<string> WidgetFieldList { get; set; }

        /// <summary>
        /// 定义消息接收事件
        /// </summary>
        event Action<object> MessageReceived;

        /// <summary>
        /// 附加属性改变事件
        /// </summary>
        event Action<IEventParameters> AttachedPropertyChanged;

        /// <summary>
        /// 加载完成事件
        /// </summary>
        event Action WidgetLoaded;

        /// <summary>
        /// 触发附加属性改变事件
        /// </summary>
        /// <param name="parameters"></param>
        void RaiseAttachedPropertyChanged(IEventParameters parameters);

        /// <summary>
        /// 初始化全局变量绑定关系
        /// </summary>
        void InitParameterBinding();

        /// <summary>
        /// 设置数据
        /// </summary>
        void SetData();

        /// <summary>
        /// 渲染数据
        /// </summary>
        void Render();
    }
}
