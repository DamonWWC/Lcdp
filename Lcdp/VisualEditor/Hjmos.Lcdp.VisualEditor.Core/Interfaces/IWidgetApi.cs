using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.Interfaces
{
    /// <summary>
    /// 平台组件API接口
    /// </summary>
    public interface IWidgetApi
    {

        /// <summary>
        /// 平台页面API对象
        /// </summary>
        IPageApi PageApi { get; }

        /// <summary>
        /// 设置组件参数
        /// </summary>
        /// <param name="element">组件</param>
        /// <param name="node">页面节点</param>
        void SetOption(FrameworkElement element, IUnitNode node);

        /// <summary>
        /// 组件基础初始化工作
        /// </summary>
        /// <param name="element"></param>
        void BaseInit(FrameworkElement element);
    }
}
