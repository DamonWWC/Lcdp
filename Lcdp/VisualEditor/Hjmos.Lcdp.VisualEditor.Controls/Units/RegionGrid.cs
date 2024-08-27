using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.VisualEditor.Controls.Attached;
using Hjmos.Lcdp.VisualEditor.Controls.DataFields;
using Hjmos.Lcdp.VisualEditor.Controls.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Hjmos.Lcdp.VisualEditor.Core.Enums;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Interface;
using Hjmos.Lcdp.VisualEditor.Models;
using Newtonsoft.Json;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls.Units
{
    /// <summary>
    /// 分区域Grid组件
    /// </summary>
    [Widget(DisplayName = "RegionGrid", Category = "Function", DefaultWidth = 300d, DefaultHeight = 300d)]
    [DataFields(typeof(RegionGridDataFields))]
    public class RegionGrid : Grid, IRegion
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

        /// <summary>
        /// 用来关联全局参数的组件字段列表
        /// </summary>
        public List<string> WidgetFieldList { get; set; }

        /// <summary>
        /// 定义附加属性改变事件
        /// </summary>
        public event Action<IEventParameters> AttachedPropertyChanged;

        /// <summary>
        /// 定义消息接收事件
        /// </summary>
        public event Action<object> MessageReceived;

        /// <summary>
        /// 表格行高数组
        /// </summary>
        public string Rows
        {
            get => (string)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(string), typeof(RegionGrid), new FrameworkPropertyMetadata(string.Empty, delegate { }, (d, v) =>
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
            DependencyProperty.Register("Columns", typeof(string), typeof(RegionGrid), new FrameworkPropertyMetadata(string.Empty, delegate { }, (d, v) =>
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

        public RegionGrid()
        {
            PageApi = ContainerLocator.Current.Resolve<IPageApi>();
            WidgetApi = ContainerLocator.Current.Resolve<IWidgetApi>();

            // 背景透明
            Background = Brushes.Transparent;
            //// 显示表格虚线
            //ShowGridLines = true;

            //Rows = 1;
            //Columns = 1;

            //// 注册拖拽事件
            //Drop += Grid_Drop;
        }


        // 组件拖拽到表格上触发事件
        private void Grid_Drop(object sender, DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(typeof(WidgetItem))) return;

            WidgetItem widgetItem = e.Data.GetData(typeof(WidgetItem)) as WidgetItem;

            // 实例化组件
            FrameworkElement element;
            if (widgetItem.RenderAsSample)
            {
                element = ContainerLocator.Current.Resolve(Type.GetType(widgetItem.SampleFullName)) as FrameworkElement;
                (element as ISample).WidgetType = widgetItem.WidgetType;
            }
            else
                element = ContainerLocator.Current.Resolve(widgetItem.WidgetType) as FrameworkElement;

            // 清理组件宽高数据，使组件铺满单元格
            element.ClearValue(WidthProperty);
            element.ClearValue(HeightProperty);
            // 设置为NaN属性面板会渲染为0，改为ClearValue
            //element.Width = double.NaN;
            //element.Height = double.NaN;

            // 初始化组件
            (element as IWidget).Init();

            SetRowSpan(element, 1); // 跨行数
            SetColumnSpan(element, 1); // 跨列数


            #region 根据鼠标位置，判断组件要放置的单元格

            int selectedColumnIndex = -1, selectedRowIndex = -1;

            // 鼠标位置
            Point position = e.GetPosition(this);

            double temp = position.X;

            // 遍历单元格宽度，直到超过鼠标横坐标
            for (int i = 0; i < ColumnDefinitions.Count; i++)
            {
                ColumnDefinition colDef = ColumnDefinitions[i];
                temp -= colDef.ActualWidth;
                if (temp <= -1)
                {
                    selectedColumnIndex = i;
                    break;
                }
            }

            temp = position.Y;

            // 遍历单元格高度，直到超过鼠标纵坐标
            for (int i = 0; i < RowDefinitions.Count; i++)
            {
                RowDefinition rowDef = RowDefinitions[i];
                temp -= rowDef.ActualHeight;
                if (temp <= -1)
                {
                    selectedRowIndex = i;
                    break;
                }
            }

            #endregion

            // 设置行列附加属性
            element.SetValue(RowProperty, selectedRowIndex);
            element.SetValue(ColumnProperty, selectedColumnIndex);

            Children.Add(element);

            e.Effects = DragDropEffects.Copy;

            e.Handled = true;
        }

        /// <summary>
        /// 触发附加属性改变事件
        /// </summary>
        /// <param name="parameters"></param>
        public void RaiseAttachedPropertyChanged(IEventParameters parameters) => AttachedPropertyChanged?.Invoke(parameters);

        /// <summary>
        /// 初始化
        /// </summary>
        public IUnit Init()
        {
            WidgetApi.BaseInit(this);

            return this;
        }

        /// <summary>
        /// 设置组件参数
        /// </summary>
        /// <param name="node">页面节点</param>
        public void SetOption(IUnitNode node)
        {
            WidgetApi.SetOption(this, node);
        }

        public void SetData()
        {
            throw new NotImplementedException();
        }

        public void Render()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 获取组件节点
        /// </summary>
        /// <returns></returns>
        public IUnitNode GetNode()
        {
            UnitNode node = new()
            {
                Child = new List<IUnitNode>(),
                NodeType = NodeType.Region,
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
                    node.Child.Add(childNode as UnitNode);
                }
            }

            return node;
        }

        public void InitParameterBinding()
        {
            throw new NotImplementedException();
        }
    }
}
