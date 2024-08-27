using Hjmos.Lcdp.VisualEditor.Controls.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Enums;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Interface;
using Newtonsoft.Json;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.Units
{
    public class LayerGrid : Grid, ILayer
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

        public LayerGrid()
        {
            PageApi = ContainerLocator.Current.Resolve<IPageApi>();
            WidgetApi = ContainerLocator.Current.Resolve<IWidgetApi>();
        }

        /// <summary>
        /// 表格行高数组
        /// </summary>
        public string Rows
        {
            get => (string)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(string), typeof(LayerGrid), new FrameworkPropertyMetadata(string.Empty, delegate { }, (d, v) =>
            {
                if (string.IsNullOrEmpty(v as string)) return string.Empty;

                Grid region = d as Grid;

                // 定义表格行数
                List<GridLength> rowLengths = JsonConvert.DeserializeObject<List<GridLength>>(v as string);
                region.RowDefinitions.Clear();

                foreach (GridLength rowLength in rowLengths)
                {
                    region.RowDefinitions.Add(new RowDefinition() { Height = rowLength });
                }

                return v;
            }));

        /// <summary>
        /// 表格列宽数组
        /// </summary>
        public string Columns
        {
            get => (string)GetValue(ColumnsProperty);
            set => SetValue(ColumnsProperty, value);
        }

        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(string), typeof(LayerGrid), new FrameworkPropertyMetadata(string.Empty, delegate { }, (d, v) =>
            {
                if (string.IsNullOrEmpty(v as string)) return string.Empty;

                Grid region = d as Grid;

                // 定义表格行数
                List<GridLength> columnLengths = JsonConvert.DeserializeObject<List<GridLength>>(v as string);
                region.ColumnDefinitions.Clear();

                foreach (GridLength columnLength in columnLengths)
                {
                    region.ColumnDefinitions.Add(new ColumnDefinition() { Width = columnLength });
                }

                return v;
            }));

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

            // 获取参数配置
            node.Options = DependencyObjectHelper.GetPropertyDescribers(this);

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
            WidgetApi.SetOption(this, node);
        }
    }
}
