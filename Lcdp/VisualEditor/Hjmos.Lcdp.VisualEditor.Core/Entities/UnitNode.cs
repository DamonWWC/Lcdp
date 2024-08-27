using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Hjmos.Lcdp.VisualEditor.Core.Enums;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Core.Proxy;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Core.Entities
{
    /// <summary>
    /// 表示页面中的一个节点
    /// </summary>
    public class UnitNode : IUnitNode
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))] // Enum默认序列化成数值，使用Converter转换成字符串
        public NodeType NodeType { get; set; }

        /// <summary>
        /// 节点关联元素的Type
        /// </summary>
        public string ElementType { get; set; }

        /// <summary>
        /// 关联对象的参数配置
        /// </summary>
        public Dictionary<string, string> Options { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        public List<IUnitNode> Child { get; set; }

        /// <summary>
        /// 子部件
        /// </summary>
        public List<IUnitNode> Part { get; set; }

        /// <summary>
        /// 获取组件实例
        /// </summary>
        /// <returns></returns>
        public virtual UIElement GetElement()
        {

            if (NodeType is NodeType.Widget or NodeType.Region)
            {
                // 获取组件类型
                Type type = Type.GetType(ElementType);
                if (type == null) throw new Exception($"无法加载组件类型‘{ ElementType }’，请检查程序集是否存在。");

                // 获取组件上的特性
                WidgetAttribute att = type.GetCustomAttribute<WidgetAttribute>();
                if (att == null) throw new Exception($"组件{ElementType}上不存在Widget特性。");

                // 获取组件实例
                FrameworkElement element;
                if (att.RenderAsSample && ContainerLocator.Current.Resolve<IPageApi>().IsDesignMode)
                {
                    // 获取样例
                    element = ContainerLocator.Current.Resolve(Type.GetType(att.SampleFullName)) as FrameworkElement;
                    (element as ISample).WidgetType = type;
                }
                else
                {
                    // 获取真实组件
                    element = ContainerLocator.Current.Resolve(type) as FrameworkElement;
                }

                // 组件初始化、组件属性赋值
                WidgetProxy widgetProxy = new(element as IWidget);
                widgetProxy.Init().SetOption(this);

                // 遍历子组件（子页面不用遍历）
                if (Child?.Count > 0 && NodeType != NodeType.PageContainer)
                {
                    Child.ToList().ForEach(x => (element as Panel).Children.Add(x.GetElement()));
                }

                // 遍历子部件
                if (Part?.Count > 0 && NodeType != NodeType.PageContainer)
                {
                    Part.ToList().ForEach(x =>
                    {
                        // TODO: 是否有必要通过name来检索
                        string name = x.Options["name"];

                        // 获取子部件的引用
                        UIElement part = element.FindName(name) as UIElement;

                        // 设置子部件的参数
                        if (part is IUnit partUnit)
                        {
                            partUnit.SetOption(x);
                        }
                    });
                }

                return element;
            }

            return null;
        }
    }
}
