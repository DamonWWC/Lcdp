using Hjmos.Lcdp.VisualEditor.Controls.Units;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Interface;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls.Entities
{
    /// <summary>
    /// 表示页面中的一个图层节点
    /// </summary>
    public class LayerNode : UnitNode, ILayerNode
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        /// 层叠
        /// </summary>
        public int ZIndex { get; set; }

        /// <summary>
        /// 获取图层组件实例
        /// </summary>
        /// <returns></returns>
        public override UIElement GetElement()
        {

            ILayer layer;
            // TODO:这里要根据ElementType去反射
            if (this.ElementType.Contains("Grid"))
            {
                layer = new LayerGrid()
                {
                    DisplayName = this.DisplayName,
                    // 透明时鼠标不能穿透
                    Background = Brushes.Transparent
                    // null鼠标可以穿透
                    //Background = null
                };
            }
            else {
                layer = new LayerCanvas()
                {
                    DisplayName = this.DisplayName,
                    // 透明时鼠标不能穿透
                    Background = Brushes.Transparent
                    // null鼠标可以穿透
                    //Background = null
                };
            }



            // 初始化根画布
            layer.Init().SetOption(this); ;


            layer.Guid = this.Guid == Guid.Empty ? Guid.NewGuid() : this.Guid;


            AdaptiveHelper.AdaptiveParent(layer as FrameworkElement);

            // 渲染图层和子组件
            this.Child.ToList().ForEach(x =>
            {
                (layer as Panel).Children.Add(x.GetElement());
            });

            return layer as UIElement;
        }
    }
}
