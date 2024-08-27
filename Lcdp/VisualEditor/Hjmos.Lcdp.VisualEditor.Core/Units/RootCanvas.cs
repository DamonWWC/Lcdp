using Hjmos.Lcdp.VisualEditor.Core.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Enums;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.Units
{
    public class RootCanvas : Canvas, IRoot
    {

        public Guid Guid { get; set; }

        /// <summary>
        /// 页面API对象
        /// </summary>
        public IPageApi PageApi { get; set; }

        /// <summary>
        /// 平台组件API对象
        /// </summary>
        public IWidgetApi WidgetApi { get; set; }

        public string DisplayName { get; set; }

        public RootCanvas()
        {
            PageApi = ContainerLocator.Current.Resolve<IPageApi>();
            WidgetApi = ContainerLocator.Current.Resolve<IWidgetApi>();

            // 默认透明背景
            Background = Brushes.Transparent;

            // 允许拖拽
            AllowDrop = true;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public IUnit Init()
        {
            WidgetApi.BaseInit(this);

            // 运行时环境
            if (!PageApi.IsDesignMode)
            {
                // 清除宽高，让画布自适应嵌入的程序
                // TODO：改成程序控制按分辨率还是需要自适应
                this.ClearValue(WidthProperty);
                this.ClearValue(HeightProperty);
            }
            else
            {
                // 设置画布默认宽高
                // TODO：增加页面分辨率配置
                this.Width = 1920d;
                this.Height = 1080d;
                // 自适应宽高
                //AdaptiveHelper.AdaptiveParent(this);
            }

            return this;
        }

        /// <summary>
        /// 获取组件节点
        /// </summary>
        /// <returns></returns>
        public IUnitNode GetNode()
        {
            IUnitNode node = new RootNode()
            {
                Child = new List<IUnitNode>(),
                NodeType = NodeType.Root,
                ElementType = this.GetType().FullName,
                Guid = this.Guid == Guid.Empty ? Guid.NewGuid() : this.Guid,
                DisplayName = this.DisplayName
            };

            //// 获取参数配置
            //node.Options = DependencyObjectHelper.GetPropertyDescribers(this);

            // 获取子组件
            foreach (UIElement item in Children)
            {
                if ((item as IUnit).GetNode() is UnitNode childNode)
                {
                    node.Child.Add(childNode);
                }
            }

            return node;
        }

        /// <summary>
        /// 设置组件参数
        /// </summary>
        /// <param name="node">页面节点</param>
        public void SetOption(IUnitNode node)
        {

        }
    }
}
