using Hjmos.Lcdp.Helpers;
using Hjmos.Lcdp.VisualEditor.Core;
using Hjmos.Lcdp.VisualEditor.Core.Attributes;
using Hjmos.Lcdp.VisualEditor.Core.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Core.Managers;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Prism.Commands;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class LeftSideViewModel : ViewModelBase
    {
        /// <summary>
        /// 组件面板数据源
        /// </summary>
        public ObservableCollection<WidgetItem> WidgetItemList { get; set; } = new ObservableCollection<WidgetItem>();

        #region Command

        /// <summary>
        /// 组件面板拖出控件命令
        /// </summary>
        public ICommand ControlDragCommand => new DelegateCommand<MouseButtonEventArgs>(OnControlDrag);

        /// <summary>
        /// 添加画布图层命令
        /// </summary>
        public ICommand AddCanvasLayerCommand => new DelegateCommand(() => State.PageShell.AddLayer("Canvas"));

        /// <summary>
        /// 添加网格图层命令
        /// </summary>
        public ICommand AddGridLayerCommand => new DelegateCommand(() => State.PageShell.AddLayer());

        /// <summary>
        /// 删除图层命令
        /// </summary>
        public ICommand RemoveLayerCommand => new DelegateCommand(() => State.PageShell.RemoveLayer());

        /// <summary>
        /// 图层切换命令
        /// </summary>
        public ICommand LayerSelectionChangedCommand => new DelegateCommand(() => State.PageShell.ToggleLayer());


        #endregion

        public LeftSideViewModel()
        {
            #region 加载程序集获取组件面板数据

            // 遍历插件目录下所有程序集，获取组件库列表
            foreach (Assembly assembly in StateManager.PluginList)
            {
                try
                {
                    foreach (Type item in assembly.GetExportedTypes().ToList().Where(x => x.IsClass && typeof(IWidget).IsAssignableFrom(x)))
                    {
                        // 获取组件上的特性
                        WidgetAttribute att = item.GetCustomAttribute<WidgetAttribute>();
                        if (att == null) continue;

                        // 加入组件库列表
                        WidgetItemList.Add(new WidgetItem()
                        {
                            DisplayName = att.DisplayName,
                            WidgetType = item,
                            Category = att.Category,
                            Icon = att.Icon,
                            DefaultWidth = att.DefaultWidth,
                            DefaultHeight = att.DefaultHeight,
                            RenderAsSample = att.RenderAsSample,
                            SampleFullName = att.SampleFullName
                        });
                    }
                }
                catch (Exception)
                {


                }
            }

            // 直接添加一个文本组件（废弃）
            //WidgetItemList.Add(new WidgetItem()
            //{
            //    DisplayName = "Text",
            //    WidgetType = typeof(TextBlock),
            //    Category = "Function",
            //    Icon = "\ue63c",
            //    DefaultWidth = 200d,
            //    DefaultHeight = 200d,
            //    RenderAsSample = false,
            //    SampleFullName = ""
            //});

            // 组件按Category字段分组
            ICollectionView vw = CollectionViewSource.GetDefaultView(WidgetItemList);
            vw.GroupDescriptions.Add(new PropertyGroupDescription("Category"));

            #endregion

        }

        /// <summary>
        /// 组件面板拖出控件方法
        /// </summary>
        /// <param name="selectedItem"></param>
        private void OnControlDrag(MouseButtonEventArgs e)
        {
            ListBoxItem item = (e.OriginalSource as DependencyObject).TryFindParent<ListBoxItem>();

            if (item is null) return;


            if (item != null)
            {
                WidgetItem widgetItem = item.Content as WidgetItem;

                // 获取被拖拽项对应的组件类型
                Type type = widgetItem.WidgetType;

                CreateComponentTool tool = new(type);
                if (State.CurrentDesignSurface != null)
                {
                    State.CurrentDesignSurface.DesignContext.Services.Tool.CurrentTool = tool;
                    DragDrop.DoDragDrop(item, tool, DragDropEffects.Copy);
                }
            }
        }
    }
}
