using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 平台组件API
    /// TODO：梳理API中不开放给用户的方法，放到单独的类
    /// </summary>
    public class WidgetApi : IWidgetApi
    {

        /// <summary>
        /// 平台页面API对象
        /// </summary>
        public IPageApi PageApi { get; }

        public WidgetApi(IPageApi pageApi) => PageApi = pageApi;

        /// <summary>
        /// 基础初始化工作
        /// </summary>
        /// <param name="element"></param>
        public void BaseInit(FrameworkElement element)
        {
            Type type = element.GetType();

            // 初始化组件
            if (typeof(IWidget).IsAssignableFrom(type) && !typeof(IGroup).IsAssignableFrom(type))
            {
                // 组件添加一些常用属性（1、为了属性在属性面板显示；2、序列化还原附加属性，赋值前需要给控件添加附加属性）
                WidgetPropertyHelper.InitGeneralProperty(element);

                //// 组件注册鼠标事件
                //if (PageApi.IsDesignMode)
                //{
                //    // 注册鼠标事件
                //    element.PreviewMouseDown += StateManager.Instance.ChildPreviewMouseDownEvent;
                //}
            }

            if (typeof(IRegion).IsAssignableFrom(type))
            {
                // 组件添加一些常用属性（1、为了属性在属性面板显示；2、序列化还原附加属性，赋值前需要给控件添加附加属性）
                WidgetPropertyHelper.InitGeneralProperty(element);
            }

            // 初始化组件容器
            if (!PageApi.IsDesignMode && typeof(IGroup).IsAssignableFrom(type))
            {
                // 允许鼠标穿透
                (element as Panel).Background = null;
            }

        }


        /// <summary>
        /// 设置组件参数
        /// </summary>
        /// <param name="element">组件</param>
        /// <param name="node">页面节点</param>
        public void SetOption(FrameworkElement element, IUnitNode node)
        {
            (element as IUnit).Guid = node.Guid;
            TypeHelper.SetValue(element, node.Options);
        }
    }
}
