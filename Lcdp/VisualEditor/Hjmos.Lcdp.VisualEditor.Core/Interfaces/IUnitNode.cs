using Hjmos.Lcdp.VisualEditor.Core.Enums;
using System;
using System.Collections.Generic;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core.Interfaces
{
    /// <summary>
    /// 所有页面节点的接口
    /// </summary>
    /// <typeparam name="T">子节点的类型</typeparam>
    public interface IUnitNode
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        Guid Guid { get; set; }

        /// <summary>
        /// 节点类型
        /// </summary>
        NodeType NodeType { get; set; }

        /// <summary>
        /// 节点关联元素的Type
        /// </summary>
        string ElementType { get; set; }

        /// <summary>
        /// 关联对象的参数配置
        /// </summary>
        Dictionary<string, string> Options { get; set; }

        /// <summary>
        /// 子节点
        /// </summary>
        List<IUnitNode> Child { get; set; }

        /// <summary>
        /// 组件内部的组件
        /// </summary>
        List<IUnitNode> Part { get; set; }

        /// <summary>
        /// 获取组件实例
        /// </summary>
        UIElement GetElement();
    }
}
