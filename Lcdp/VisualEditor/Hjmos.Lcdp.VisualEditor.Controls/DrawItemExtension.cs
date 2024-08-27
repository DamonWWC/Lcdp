using System;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 容器元素实现的行为接口，以支持调整大小绘制新元素
    /// </summary>
    public interface IDrawItemExtension
    {
        /// <summary>
        /// 返回是否可以绘制指定的类型
        /// </summary>
        /// <param name="createItemType">要检查的类型</param>
        /// <returns>如果可以绘制指定的类型，则为True，否则为false。</returns>
        bool CanItemBeDrawn(Type createItemType);

        /// <summary>
        /// 开始绘制
        /// </summary>
        /// <param name="clickedOn">要绘制的设计项</param>
        /// <param name="createItemType">组件类型</param>
        /// <param name="panel">用来绘制的设计面板</param>
        /// <param name="e">初始化绘制操作的<see cref="MouseEventArgs"/>参数</param>
        /// <param name="createItemCallback">用于创建设计项的回调函数</param>
        void StartDrawItem(DesignItem clickedOn, Type createItemType, IDesignPanel panel, MouseEventArgs e, Action<DesignItem> drawItemCallback);
    }
}
