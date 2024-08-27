using System;

namespace Hjmos.Lcdp.VisualEditor.Core.Interfaces
{
    /// <summary>
    /// 所有组件的接口
    /// </summary>
    public interface IUnit
    {

        /// <summary>
        /// 唯一标识
        /// </summary>
        Guid Guid { get; set; }

        /// <summary>
        /// 平台页面API对象
        /// </summary>
        IPageApi PageApi { get; set; }

        /// <summary>
        /// 平台组件API对象
        /// </summary>
        IWidgetApi WidgetApi { get; set; }

        /// <summary>
        ///  初始化组件
        /// </summary>
        IUnit Init();

        /// <summary>
        /// 设置组件参数
        /// </summary>
        /// <param name="node">页面节点</param>
        void SetOption(IUnitNode node);

        /// <summary>
        /// 获取组件节点
        /// </summary>
        IUnitNode GetNode();

    }
}
