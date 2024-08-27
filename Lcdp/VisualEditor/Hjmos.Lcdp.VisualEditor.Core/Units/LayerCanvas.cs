using Hjmos.Lcdp.VisualEditor.Core.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Enums;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.Units
{
    public class LayerCanvas : Canvas, ILayer
    {
        public int ZIndex { get; set; }

        public string DisplayName { get; set; }

        public Guid Guid { get; set; }

        /// <summary>
        /// 页面API对象
        /// </summary>
        public IPageApi PageApi { get; set; }

        /// <summary>
        /// 平台组件API对象
        /// </summary>
        public IWidgetApi WidgetApi { get; set; }

        public LayerCanvas()
        {
            PageApi = ContainerLocator.Current.Resolve<IPageApi>();
            WidgetApi = ContainerLocator.Current.Resolve<IWidgetApi>();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public IUnit Init()
        {
            WidgetApi.BaseInit(this);

            return this;
        }

        /// <summary>
        /// 获取组件节点
        /// </summary>
        /// <returns></returns>
        public IUnitNode GetNode()
        {
            IUnitNode node = new LayerNode()
            {
                Child = new List<IUnitNode>(),
                DisplayName = this.DisplayName,
                NodeType = NodeType.Layer,
                ElementType = this.GetType().FullName,
                Guid = this.Guid == Guid.Empty ? Guid.NewGuid() : this.Guid
            };

            //// 获取参数配置
            //node.Options = DependencyObjectHelper.GetPropertyDescribers(this);

            // 获取子组件
            foreach (UIElement item in Children)
            {
                IUnitNode childNode = (item as IUnit).GetNode();

                if (childNode != null)
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
