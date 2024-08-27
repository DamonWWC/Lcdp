using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Hjmos.Lcdp.VisualEditor.Core.ItemExtensions
{
    /// <summary>
    /// 允许在网格上排列行/列
    /// </summary>
    [ExtensionFor(typeof(Grid))]
    [ExtensionServer(typeof(LogicalOrExtensionServer<PrimarySelectionExtensionServer, PrimarySelectionParentExtensionServer>))]
    public class GridAdornerProvider : AdornerProvider
    {
        /// <summary>
        /// 行分割器放置信息类
        /// </summary>
        private sealed class RowSplitterPlacement : AdornerPlacement
        {
            private readonly RowDefinition row;
            public RowSplitterPlacement(RowDefinition row) => this.row = row;

            public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
            {
                adorner.Arrange(new Rect(-(GridRailAdorner.RailSize + GridRailAdorner.RailDistance),
                                         row.Offset - GridRailAdorner.SplitterWidth / 2,
                                         GridRailAdorner.RailSize + GridRailAdorner.RailDistance + adornedElementSize.Width,
                                         GridRailAdorner.SplitterWidth));
            }
        }

        /// <summary>
        /// 列分割器放置信息类
        /// </summary>
        private sealed class ColumnSplitterPlacement : AdornerPlacement
        {
            private readonly ColumnDefinition column;
            public ColumnSplitterPlacement(ColumnDefinition column) { this.column = column; }

            public override void Arrange(AdornerPanel panel, UIElement adorner, Size adornedElementSize)
            {
                adorner.Arrange(new Rect(column.Offset - GridRailAdorner.SplitterWidth / 2,
                                         -(GridRailAdorner.RailSize + GridRailAdorner.RailDistance),
                                         GridRailAdorner.SplitterWidth,
                                         GridRailAdorner.RailSize + GridRailAdorner.RailDistance + adornedElementSize.Height));
            }
        }

        /// <summary>装饰器面板</summary>
        private readonly AdornerPanel adornerPanel = new();
        /// <summary>上方和左侧的条状轨道装饰器</summary>
        private GridRailAdorner topBar, leftBar;

        protected override void OnInitialized()
        {
            // 左侧条状轨道装饰器
            leftBar = new GridRailAdorner(ExtendedItem, adornerPanel, Orientation.Vertical);
            // 上方条状轨道装饰器
            topBar = new GridRailAdorner(ExtendedItem, adornerPanel, Orientation.Horizontal);

            // 提供摆放信息的类
            // 左对齐
            RelativePlacement rp = new(HorizontalAlignment.Left, VerticalAlignment.Stretch);
            // X轴偏移量：-轨道边距
            rp.XOffset -= GridRailAdorner.RailDistance;
            // 指定装饰器的位置
            AdornerPanel.SetPlacement(leftBar, rp);

            // 顶部对齐
            rp = new RelativePlacement(HorizontalAlignment.Stretch, VerticalAlignment.Top);
            // Y轴偏移量：-轨道边距
            rp.YOffset -= GridRailAdorner.RailDistance;
            // 指定装饰器的位置
            AdornerPanel.SetPlacement(topBar, rp);

            // 轨道装饰器添加到装饰器面板
            adornerPanel.Children.Add(leftBar);
            adornerPanel.Children.Add(topBar);

            // 添加到基类的装饰面板集合
            this.Adorners.Add(adornerPanel);

            // 根据已有的行/列创建分割器
            CreateSplitter();

            this.ExtendedItem.PropertyChanged += OnPropertyChanged;

            base.OnInitialized();
        }

        protected override void OnRemove()
        {
            this.ExtendedItem.PropertyChanged -= OnPropertyChanged;
            base.OnRemove();
        }

        /// <summary>行列改变的时候，重新创建分割器</summary>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName is "RowDefinitions" or "ColumnDefinitions")
            {
                // 根据已有的行/列创建分割器
                CreateSplitter();
            }
        }

        /// <summary>保存现有的分割器集合</summary>
        private readonly List<GridSplitterAdorner> splitterList = new();

        /// <summary>
        /// 用于确保异步分割器创建只排队一次的标志
        /// </summary>
        private bool requireSplitterRecreation;

        /// <summary>
        /// 根据已有的行/列创建分割器
        /// </summary>
        private void CreateSplitter()
        {
            if (requireSplitterRecreation) return;

            requireSplitterRecreation = true;

            // 延迟创建分割器，以防止在对集合进行多次更改时重新创建不必要的分割器。
            // 它还确保在添加分割器时初始化新行/列的Offset属性。
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Loaded, // Loaded = after layout, but before input
                (Action)delegate
                {
                    requireSplitterRecreation = false;

                    // 清除原有的分割器
                    splitterList.ForEach(s => adornerPanel.Children.Remove(s));
                    splitterList.Clear();

                    Grid grid = this.ExtendedItem.Component as Grid;

                    // 创建行分割器
                    IList<DesignItem> col = this.ExtendedItem.Properties["RowDefinitions"].CollectionElements;
                    for (int i = 1; i < grid.RowDefinitions.Count; i++)
                    {
                        RowDefinition row = grid.RowDefinitions[i];
                        // 实例化行分割器
                        GridRowSplitterAdorner splitter = new GridRowSplitterAdorner(leftBar, this.ExtendedItem, col[i - 1], col[i]);
                        // 设置摆放位置
                        AdornerPanel.SetPlacement(splitter, new RowSplitterPlacement(row));
                        // 附加到装饰器面板
                        adornerPanel.Children.Add(splitter);
                        // 保存到分割器集合
                        splitterList.Add(splitter);
                    }

                    // 创建列分割器
                    col = this.ExtendedItem.Properties["ColumnDefinitions"].CollectionElements;
                    for (int i = 1; i < grid.ColumnDefinitions.Count; i++)
                    {
                        ColumnDefinition column = grid.ColumnDefinitions[i];
                        // 实例化列分割器
                        GridColumnSplitterAdorner splitter = new GridColumnSplitterAdorner(topBar, this.ExtendedItem, col[i - 1], col[i]);
                        // 设置摆放位置
                        AdornerPanel.SetPlacement(splitter, new ColumnSplitterPlacement(column));
                        // 附加到装饰器面板
                        adornerPanel.Children.Add(splitter);
                        // 保存到分割器集合
                        splitterList.Add(splitter);
                    }
                }
            );
        }
    }
}
