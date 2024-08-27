using Hjmos.Lcdp.VisualEditor.Core.Units;
using Hjmos.Lcdp.VisualEditor.Core.Enums;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.Entities
{
    /// <summary>
    /// 表示页面的根节点
    /// </summary>
    public class RootNode : UnitNode, IRootNode
    {
        public RootNode() { }

        /// <summary>
        /// 名称
        /// </summary>
        public string DisplayName { get; set; } = "Undefined";

        /// <summary>
        /// 获取根组件实例
        /// </summary>
        /// <returns></returns>
        public override UIElement GetElement()
        {
            RootCanvas root = new()
            {
                Guid = this.Guid,
                DisplayName = this.DisplayName
            };

            // 初始化根画布
            root.Init();

            // 渲染图层和子组件
            if (this.Child != null)
            {
                this.Child.ToList().ForEach(x =>
                {
                    if (x.NodeType == NodeType.Layer)
                    {
                        root.Children.Add(x.GetElement());
                    }
                });
            }

            return root;
        }

        /// <summary>
        /// 获取根组件实例
        /// </summary>
        /// <param name="backgroundBrush">根组件背景颜色</param>
        /// <returns></returns>
        public UIElement GetElement(SolidColorBrush backgroundBrush)
        {
            Panel element = this.GetElement() as Panel;
            element.Background = backgroundBrush;
            return element;
        }
    }
}
