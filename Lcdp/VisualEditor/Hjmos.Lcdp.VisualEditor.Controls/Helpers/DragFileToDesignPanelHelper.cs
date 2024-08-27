using Hjmos.Lcdp.VisualEditor.Controls.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Interface;
using Prism.Ioc;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.Controls.Helpers
{
    /// <summary>
    /// 拖拽助手
    /// </summary>
    public class DragFileToDesignPanelHelper
	{
        //protected ChangeGroup ChangeGroup;

        //MoveLogic moveLogic;
        //Point createPoint;

        //Func<DesignContext, DragEventArgs, DesignItem[]> _createItems;
        DesignSurface _designSurface;
        PageShell _pageShell;

        /// <summary>
        /// 为设计界面配置拖拽助手
        /// </summary>
        /// <param name="designSurface">设计界面</param>
        /// <param name="createItems">创建元素的委托</param>
        /// <returns></returns>
        public static DragFileToDesignPanelHelper Install(DesignSurface designSurface)
        {
            DragFileToDesignPanelHelper helper = new DragFileToDesignPanelHelper
            {
                //_createItems = createItems,
                _designSurface = designSurface
            };
            helper._designSurface.AllowDrop = true;
            //helper._designSurface.DragOver += helper.DesignSurface_DragOver;
            helper._designSurface.Drop += helper.DesignSurface_Drop;
            //helper._designSurface.DragLeave += helper.DesignSurface_DragLeave;

            return helper;
        }

        /// <summary>
        /// 移除拖拽助手
        /// </summary>
        public void Remove()
        {
            //_designSurface.DragOver -= DesignSurface_DragOver;
            _designSurface.Drop -= DesignSurface_Drop;
            //_designSurface.DragLeave -= DesignSurface_DragLeave;
        }

        void DesignSurface_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (!e.Data.GetDataPresent(typeof(WidgetItem)) || _designSurface.PageShell.RootElement == null) return;

                WidgetItem widgetItem = e.Data.GetData(typeof(WidgetItem)) as WidgetItem;

                FrameworkElement element;
                if (widgetItem.RenderAsSample)
                {
                    if (Type.GetType(widgetItem.SampleFullName) is null)
                    {
                        MessageBox.Show("找不到样例组件的类型");
                    }
                    element = ContainerLocator.Current.Resolve(Type.GetType(widgetItem.SampleFullName)) as FrameworkElement;
                    (element as ISample).WidgetType = widgetItem.WidgetType;
                }
                else
                {
                    element = ContainerLocator.Current.Resolve(widgetItem.WidgetType) as FrameworkElement;
                }

                // 设置组件默认宽高
                // TODO：改成根据布局面板自适应
                element.Width = widgetItem.DefaultWidth;
                element.Height = widgetItem.DefaultHeight;

                // 初始化组件
                (element as IWidget).Init();

                // 设置组件摆放位置
                Point p = e.GetPosition(_pageShell.RootElement);
                Canvas.SetTop(element, p.Y);
                Canvas.SetLeft(element, p.X);

                //// 添加组件到画布
                //_designSurface.AddChild(element);

                e.Effects = DragDropEffects.Copy;
                e.Handled = true;
            }
            catch (Exception x)
            {

            }
        }
    }
}
