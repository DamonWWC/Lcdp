using Hjmos.Lcdp.Common;
using Hjmos.Lcdp.Helpers;
using Hjmos.Lcdp.Toolkits;
using Hjmos.Lcdp.VisualEditor.Core.Entities;
using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.Core.JsonConverters;
using Hjmos.Lcdp.VisualEditor.Core.Services;
using Hjmos.Lcdp.VisualEditor.Core.Units;
using Hjmos.Lcdp.VisualEditor.Models;
using Hjmos.Lcdp.VisualEditorServer.Entities;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using Newtonsoft.Json;
using Prism.Ioc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.Controls
{
    /// <summary>
    /// 页面容器
    /// </summary>
    public class PageShell : Grid
    {
        #region Property

        /// <summary>
        /// 平台页面API对象
        /// </summary>
        public IPageApi PageApi { get; set; }

        /// <summary>
        /// 是否自动选中图层
        /// </summary>
        public bool IsAutoSelect
        {
            get => (bool)GetValue(IsAutoSelectProperty);
            set => SetValue(IsAutoSelectProperty, value);
        }

        public static readonly DependencyProperty IsAutoSelectProperty =
            DependencyProperty.Register("IsAutoSelect", typeof(bool), typeof(PageShell), new PropertyMetadata(false));

        /// <summary>
        /// 存放图层的列表
        /// </summary>
        public List<ILayer> LayerList
        {
            get => (List<ILayer>)GetValue(LayerListProperty);
            set => SetValue(LayerListProperty, value);
        }

        public static readonly DependencyProperty LayerListProperty = DependencyProperty.Register(
            "LayerList", typeof(List<ILayer>), typeof(PageShell), new PropertyMetadata(default(List<ILayer>))
        );

        /// <summary>
        /// 页面原始数据
        /// </summary>
        public File RawData { get; set; }

        /// <summary>
        /// 页面根组件
        /// </summary>
        public Panel RootElement => this.Children[0] as Panel;
        //public Panel RootElement => this.Content as Panel;

        #endregion

        public MyComponentService _myComponentService;


        public PageShell() : this(null) { }


        public PageShell(MyComponentService myComponentService)
        {
            _myComponentService = myComponentService;

            // 避免XAML提示"未将对象引用设置到对象实例"
            if (System.ComponentModel.DesignerProperties.GetIsInDesignMode(this)) return;

            PageApi = ContainerLocator.Current.Resolve<IPageApi>();

            LayerList = new List<ILayer>();

            RootNode rootNode = new() { Guid = Guid.NewGuid(), DisplayName = "A New Page" };

            //this.Content = rootNode.GetElement();
            this.Children.Add(rootNode.GetElement());

            // 添加默认图层
            AddLayer();

            this.Loaded += delegate
            {
                // 子页面背景透明
                if (this.Parent != null && typeof(IPage).IsAssignableFrom(this.Parent.GetType()))
                {
                    (this.Children[0] as Panel).Background = Brushes.Transparent;
                    //(this.Content as Panel).Background = Brushes.Transparent;
                }
                else
                {
                    (this.Children[0] as Panel).Background = Brushes.Transparent;
                    //(this.Content as Panel).Background = Brushes.Transparent;
                }
            };
        }

        //IEventParameters parameters = new EventParameters {
        //            { "DependencyProperty", DataFieldsAttached.DataFieldsProperty },
        //            { "NewValue", DataFieldsAttached.GetDataFields(this) }
        //        };

        /// <summary>
        /// 传递给页面的参数
        /// </summary>
        public IPageParameters PageParameters
        {
            get => (IPageParameters)GetValue(PageParametersProperty);
            set => SetValue(PageParametersProperty, value);
        }

        public static readonly DependencyProperty PageParametersProperty =
            DependencyProperty.Register("PageParameters", typeof(IPageParameters), typeof(PageShell), new FrameworkPropertyMetadata(null, (d, e) =>
            {
                //// TODO: 和OnCoerceValueCallback中是重复的
                //// 传递页面上下文
                //PageService.InjectPageContext((d as PageShell).RootElement, new PageContext()
                //{
                //    PageShell = d as PageShell,
                //    Parameters = e.NewValue as PageParameters
                //});
            }));


        /// <summary>
        /// 要渲染的页面GUID
        /// </summary>
        public string PageGuid
        {
            get => (string)GetValue(PageGuidProperty);
            set => SetValue(PageGuidProperty, value);
        }

        public static readonly DependencyProperty PageGuidProperty =
            DependencyProperty.Register("PageGuid", typeof(string), typeof(PageShell), new FrameworkPropertyMetadata(string.Empty, null, OnCoerceValueCallback));


        /// <summary>
        /// Guid强制值回调
        /// 逻辑写在强制值回调中，当重新赋值相同的GUID，可以刷新页面
        /// </summary>
        private static object OnCoerceValueCallback(DependencyObject d, object baseValue)
        {
            if (baseValue == null) return string.Empty;

            if (Guid.TryParse(baseValue.ToString(), out Guid guid))
            {
                PageShell shell = d as PageShell;
                // 清空图层
                shell.LayerList = new List<ILayer>();

                // 渲染页面
                shell.LoadPage(guid.ToString());

                return baseValue;
            }

            return string.Empty;
        }


        /// <summary>
        /// 根据GUID加载页面
        /// </summary>
        /// <param name="guid">页面GUID</param>
        private async void LoadPage(string guid)
        {

            if (!Guid.TryParse(guid, out _))
            {
                MessageBox.Show("Guid字符串格式有误");
            }

            // 根据GUID获取页面
            var result = await WebApiHelper.GetAsync<Result<File>>($"pages/{guid}");

            RawData = result.Data;

            // 获取全局应用ID
            PageApi.AppId = RawData.AppId;

            // 加载全局参数
            LoadParameters(PageApi.AppId);

            // 清空画布
            if (this.RootElement != null)
            {
                this.RootElement.Children.Clear();
            }

            // 加载页面内容
            if (RawData == null || string.IsNullOrEmpty(RawData.Content))
            {
                if (Children.Count > 0)
                {
                    this.Children.Clear();
                }

                if (!PageApi.IsDesignMode)
                {
                    this.Children.Add(new RootNode() { Guid = Guid.Parse(guid), DisplayName = RawData.Name }.GetElement());
                    //this.Content = new RootNode() { Guid = Guid.Parse(guid), DisplayName = RawData.Name }.GetElement();
                }
                else
                {
                    this.Children.Add(new RootNode() { Guid = Guid.Parse(guid), DisplayName = RawData.Name }.GetElement());
                    //this.Content = new RootNode() { Guid = Guid.Parse(guid), DisplayName = RawData.Name }.GetElement(Brushes.Transparent);
                }

                // 添加默认图层
                AddLayer();
            }
            else
            {
                // 根据JSON文本获取根节点实例
                RootNode rootNode = JsonConvert.DeserializeObject<RootNode>(RawData.Content, new UnitNoteJsonConverter());

                Panel rootWidget;
                if (!PageApi.IsDesignMode)
                {
                    rootWidget = rootNode.GetElement() as Panel;
                }
                else
                {
                    rootWidget = rootNode.GetElement() as Panel;
                }

                if (Children.Count > 0)
                {
                    this.Children.Clear();
                }

                this.Children.Add(rootWidget);

                //this.Content = rootWidget;

                List<ILayer> list = new();

                // 添加图层
                if (rootWidget.Children.Count > 0)
                {
                    foreach (ILayer layer in rootWidget.Children)
                    {
                        list.Add(layer);
                    }

                    LayerList = list;
                }

                if (list.Count == 0)
                {
                    MessageBox.Show("页面中不存在图层节点，请检查页面内容");
                }

                // 当前图层
                CurrentLayer = list.Last();
            }

            // TODO: 因为LoadPage是异步的，又需要页面加载完成，所以放在这里
            // 传递页面上下文
            PageService.InjectPageContext(this.RootElement, new PageContext()
            {
                PageShell = this,
                Parameters = this.GetValue(PageParametersProperty) as PageParameters
            });


            if (_myComponentService != null)
            {
                // TODO：嵌入其他程序的时候，MyComponentService由xaml创建，是null
                _myComponentService.RegisterComponentRecursive(this.Children[0]);
                //_myComponentService.RegisterComponentRecursive(this.Content);
            }
        }


        /// <summary>
        /// 加载全局参数
        /// </summary>
        /// <param name="appid">应用ID</param>
        private async void LoadParameters(int appid)
        {
            // 从数据库加载参数
            // TODO：这里异步会导致绑定时找不到参数列表，后续处理
            List<ParameterModel> parameters = await new Service.PageService().LoadParameter(appid);
            PageApi.ParameterList.Clear();
            // 保存到全局参数列表
            parameters.ForEach(PageApi.ParameterList.Add);
        }


        #region Layer Region

        /// <summary>
        /// 当前激活的图层
        /// </summary>
        public ILayer CurrentLayer
        {
            get => (ILayer)GetValue(CurrentLayerProperty);
            set => SetValue(CurrentLayerProperty, value);
        }
        public static readonly DependencyProperty CurrentLayerProperty = DependencyProperty.Register("CurrentLayer", typeof(ILayer), typeof(PageShell));

        /// <summary>
        /// 删除图层
        /// </summary>
        public void RemoveLayer()
        {
            if (LayerList.Count() == 1)
            {
                MessageBox.Show("至少需要保留一个图层！");
                return;
            }
            string layerName = CurrentLayer.DisplayName;
            MessageBoxResult result = MessageBox.Show($"确定要删除图层【{layerName}】吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
            if (result == MessageBoxResult.OK)
            {
                // 删除图层列表项
                List<ILayer> list = LayerList.ToList();
                list.Remove(CurrentLayer);

                // 根组件
                if (this.Children[0] is not Panel root)
                //if (this.Content is not Panel root)
                {
                    MessageBox.Show("无法删除图层，请先加载页面文件");
                    return;
                }

                // 删除图层组件
                root.Children.Remove(CurrentLayer as UIElement);

                LayerList = list;
                CurrentLayer = LayerList.First();
            }
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="d"></param>
        /// <param name="layerName"></param>
        /// <returns></returns>
        public ILayer AddLayer(string type = "Grid", string layerName = "", string guid = "")
        {
            if (layerName == string.Empty)
            {
                // 自动生成图层名称
                layerName = RenderLayerName();
            }

            ILayer newLayer = null;

            if (type == "Canvas")
            {
                newLayer = new LayerCanvas()
                {
                    DisplayName = layerName,
                    // 透明时鼠标不能穿透
                    Background = Brushes.Transparent
                    // null鼠标可以穿透
                    //Background = null
                };
            }
            else if (type == "Grid")
            {
                newLayer = new LayerGrid()
                {
                    DisplayName = layerName,
                    // 透明时鼠标不能穿透
                    Background = Brushes.Transparent
                    // null鼠标可以穿透
                    //Background = null
                };
            }


            if (Guid.TryParse(guid, out _))
            {
                newLayer.Guid = Guid.Parse(guid);
            }
            else
            {
                newLayer.Guid = Guid.NewGuid();
            }

            AdaptiveHelper.AdaptiveParent(newLayer as FrameworkElement);

            // 添加图层
            List<ILayer> list = LayerList.ToList();
            list.Add(newLayer);
            LayerList = list;
            CurrentLayer = newLayer;

            // 根组件
            if (this.Children[0] is not Panel root)
            //if (this.Content is not Panel root)
            {
                MessageBox.Show("请先加载页面文件");
                return null;
            }

            if (_myComponentService != null)
            {
                // TODO：嵌入其他程序的时候，MyComponentService由xaml创建，是null
                _myComponentService.RegisterComponentForDesigner(newLayer);
            }


            root.Children.Add(newLayer as UIElement);

            return newLayer;
        }

        private string RenderLayerName()
        {
            int index;
            // 最大序号+1
            //index = Layers.Select(x => Convert.ToInt32(x.Name.Replace("Layer", ""))).Max() + 1;

            // 找到最小可用序号
            List<int> list = LayerList.Select(x => Convert.ToInt32(x.DisplayName?.Replace("Layer", ""))).ToList();
            list.Sort();

            if (list.Count == 0 || list.First() != 1)
                index = 1;
            else if (list.Last() == list.Count)
                index = list.Count + 1;
            else
                index = ((list.First() + list.Last()) * (list.Count + 1) / 2) - list.Sum();

            return $"Layer{index}";
        }

        /// <summary>
        /// 切换当前图层
        /// </summary>
        /// <param name="layerName"></param>
        public void ToggleLayer()
        {
            if (CurrentLayer is null) return;

            // 标记对应图层响应点击事件，禁用其他图层响应点击事件
            LayerList.ToList().ForEach(x => (x as UIElement).IsHitTestVisible = x.Guid == CurrentLayer.Guid);
        }

        #endregion Layer Region

        #region HitTest Region

        private readonly List<FrameworkElement> hitList = new();
        private EllipseGeometry hitArea = new();

        /// <summary>
        /// 控件选中检测事件
        /// TODO：优化检测效率
        /// </summary>
        /// <param name="action">找到控件后执行的操作</param>
        public void HitTest(Action<FrameworkElement> action = null)
        {
            if (!IsAutoSelect) return;

            // 获取鼠标点击的位置
            Point pt = Mouse.GetPosition(this);

            // 定义点击测试的范围
            hitArea = new EllipseGeometry(pt, 1.0, 1.0);
            hitList.Clear();

            // 调用点击测试的方法
            VisualTreeHelper.HitTest(this, null, new HitTestResultCallback(HitTestCallback), new GeometryHitTestParameters(hitArea));

            if (hitList.Count > 0)
            {
                LayerCanvas layer = hitList.First().TryFindParent<LayerCanvas>();
                if (layer != null)
                {
                    CurrentLayer = layer;

                    action?.Invoke(hitList.First());
                }
            }
        }

        /// <summary>
        /// 命中测试结果回调
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private HitTestResultBehavior HitTestCallback(HitTestResult result)
        {
            // 检索点击测试结果

            // 点击结果的交叉信息
            IntersectionDetail intersectionDetail = ((GeometryHitTestResult)result).IntersectionDetail;
            switch (intersectionDetail)
            {
                case IntersectionDetail.FullyContains:

                    DependencyObject node = FindComponent<IWidget>(result.VisualHit);
                    if (node != null)
                    {
                        hitList.Add(node as FrameworkElement);
                    }

                    return HitTestResultBehavior.Continue;

                case IntersectionDetail.Intersects:
                    return HitTestResultBehavior.Continue;

                case IntersectionDetail.FullyInside:
                    return HitTestResultBehavior.Continue;

                default:
                    return HitTestResultBehavior.Stop;
            }
        }

        /// <summary>
        /// 从元素自身和父级元素，查找可视元素对应的自定义组件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        /// <returns></returns>
        public static DependencyObject FindComponent<T>(DependencyObject child) where T : IWidget
        {

            if (typeof(T).IsAssignableFrom(child.GetType()))
            {
                return child;
            }
            else
            {
                DependencyObject parentObject = VisualTreeHelper.GetParent(child);
                return parentObject == null ? null : FindComponent<T>(parentObject);
            }
        }

        #endregion HitTest Region
    }
}
