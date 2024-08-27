using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using Hjmos.Lcdp.VisualEditor.Controls.UIExtensions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    public sealed class DesignPanel : Decorator, IDesignPanel, INotifyPropertyChanged
    {
        #region Hit Testing

        private readonly List<DesignItem> hitTestElements = new();
        private readonly List<DesignItem> skippedHitTestElements = new();


        /// <summary>
        /// 这个元素总是被击中(除非HitTestVisible被设置为false)
        /// </summary>
        private sealed class EatAllHitTestRequests : UIElement
        {
            protected override GeometryHitTestResult HitTestCore(GeometryHitTestParameters hitTestParameters) => new GeometryHitTestResult(this, IntersectionDetail.FullyContains);

            protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters) => new PointHitTestResult(this, hitTestParameters.HitPoint);
        }

        /// <summary>
        /// 运行命中测试
        /// </summary>
        /// <param name="reference">要进行命中测试的视觉对象</param>
        /// <param name="point">命中测试的坐标位置</param>
        /// <param name="filterCallback">表示命中测试筛选回调值的方法。</param>
        /// <param name="resultCallback">表示命中测试结果回调值的方法。</param>
        private void RunHitTest(Visual reference, Point point, HitTestFilterCallback filterCallback, HitTestResultCallback resultCallback)
        {
            if (!Keyboard.IsKeyDown(Key.LeftAlt))
            {
                hitTestElements.Clear();
                skippedHitTestElements.Clear();
            }

            // 调用点击测试的方法
            // 第4个参数为要对其执行命中测试的参数值。参数可以传一个形状的范围，这里是传一个点的形状
            VisualTreeHelper.HitTest(reference, filterCallback, resultCallback, new PointHitTestParameters(point));
        }

        /// <summary>
        /// 命中测试筛选回调值的方法。
        /// </summary>
        /// <param name="potentialHitTestTarget">可能的命中目标</param>
        /// <param name="hitTestType"></param>
        /// <returns></returns>
        private HitTestFilterBehavior FilterHitTestInvisibleElements(DependencyObject potentialHitTestTarget, HitTestType hitTestType)
        {
            if (potentialHitTestTarget is UIElement element)
            {
                if (!(element.IsHitTestVisible && element.Visibility == Visibility.Visible))
                {
                    return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                }

                MyDesignItem designItem = Context.Services.Component.GetDesignItem(element) as MyDesignItem;

                if (hitTestType == HitTestType.ElementSelection)
                {
                    if (Keyboard.IsKeyDown(Key.LeftAlt))
                    {
                        if (designItem != null)
                        {
                            if (skippedHitTestElements.LastOrDefault() == designItem || (hitTestElements.Contains(designItem) && !skippedHitTestElements.Contains(designItem)))
                            {
                                skippedHitTestElements.Remove(designItem);
                                return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                            }
                        }
                    }
                }
                else
                {
                    hitTestElements.Clear();
                    skippedHitTestElements.Clear();
                }

                if (designItem != null && designItem.IsDesignTimeLocked)
                {
                    return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
                }

                if (designItem != null && !hitTestElements.Contains(designItem))
                {
                    hitTestElements.Add(designItem);
                    skippedHitTestElements.Add(designItem);
                }
            }

            return HitTestFilterBehavior.Continue;
        }

        /// <summary>
        /// Performs a custom hit testing lookup for the specified mouse event args.
        /// 为指定的鼠标事件参数执行自定义命中测试查找。
        /// </summary>
        /// <param name="mousePosition">鼠标位置</param>
        /// <param name="testAdorners"></param> 测试点击RootElement的时候，这里是false
        /// <param name="testDesignSurface"></param> 测试点击RootElement的时候，这里是true
        /// <param name="hitTestType"></param>
        /// <returns></returns>
        public DesignPanelHitTestResult HitTest(Point mousePosition, bool testAdorners, bool testDesignSurface, HitTestType hitTestType)
        {
            // 一个结构，用来描述命中测试的结果
            DesignPanelHitTestResult result = DesignPanelHitTestResult.NoHit;

            // 执行命中测试
            HitTest(mousePosition, testAdorners, testDesignSurface, r => { result = r; return false; }, hitTestType);

            return result;
        }

        /// <summary>
        /// 在设计面上执行一个命中测试，为每个匹配触发<paramref name="callback"/>。
        /// 当回调返回true时，命中测试继续进行。
        /// </summary>
        public void HitTest(Point mousePosition, bool testAdorners, bool testDesignSurface, Predicate<DesignPanelHitTestResult> callback, HitTestType hitTestType)
        {
            if (mousePosition.X < 0 || mousePosition.Y < 0 || mousePosition.X > this.RenderSize.Width || mousePosition.Y > this.RenderSize.Height)
            {
                return;
            }

            // 首先尝试在装饰层上进行命中测试。

            bool continueHitTest = true;

            // 如果有自定义的命中测试过滤方法，就用自定义的，没有就用默认的
            HitTestFilterCallback filterBehavior = CustomHitTestFilterBehavior ?? (x => FilterHitTestInvisibleElements(x, hitTestType));
            CustomHitTestFilterBehavior = null;

            if (testAdorners)
            {
                RunHitTest(AdornerLayer, mousePosition, filterBehavior,
                    // 表示命中测试结果回调值的方法。每命中一个元素就会回调一次
                    result =>
                    {
                        if (result != null && result.VisualHit != null && result.VisualHit is Visual visual)
                        {

                            // 一个结构，用来描述命中测试的结果
                            DesignPanelHitTestResult customResult = new(visual);
                            // result.VisualHit表示所有命中的视觉对象
                            DependencyObject obj = result.VisualHit;

                            // 如果有命中，并且命中的不是装饰层
                            while (obj != null && obj != AdornerLayer)
                            {
                                // 被击中的装饰器的装饰面板。
                                if (obj is AdornerPanel adorner)
                                {
                                    // 保存被击中的装饰器的装饰面板。
                                    customResult.AdornerHit = adorner;
                                }
                                // 获取父元素
                                obj = VisualTreeHelper.GetParent(obj);
                            }
                            continueHitTest = callback(customResult);
                            return continueHitTest ? HitTestResultBehavior.Continue : HitTestResultBehavior.Stop;
                        }
                        else
                        {
                            return HitTestResultBehavior.Continue;
                        }
                    });
            }

            if (continueHitTest && testDesignSurface)
            {
                RunHitTest(this.Child, mousePosition, filterBehavior,
                    result =>
                    {
                        if (result != null && result.VisualHit != null && result.VisualHit is Visual)
                        {
                            DesignPanelHitTestResult customResult = new((Visual)result.VisualHit);

                            ViewService viewService = Context.Services.View;
                            DependencyObject obj = result.VisualHit;

                            while (obj != null)
                            {
                                if ((customResult.ModelHit = viewService.GetModel(obj)) != null)
                                    break;
                                obj = VisualTreeHelper.GetParent(obj);
                            }
                            if (customResult.ModelHit == null)
                            {
                                customResult.ModelHit = Context.RootItem;
                            }

                            continueHitTest = callback(customResult);
                            return continueHitTest ? HitTestResultBehavior.Continue : HitTestResultBehavior.Stop;
                        }
                        else
                        {
                            return HitTestResultBehavior.Continue;
                        }
                    }
                );
            }
        }
        #endregion

        #region Fields + Constructor

        private readonly EatAllHitTestRequests _eatAllHitTestRequests;

        public DesignPanel()
        {
            this.Focusable = true;
            // 根元素没有设置宽高的时候，这里可以设置一点边距，把设计界面和其他面板分隔
            //this.Margin = new Thickness(16);
            DesignerProperties.SetIsInDesignMode(this, true);

            _eatAllHitTestRequests = new EatAllHitTestRequests();
            // 当用户与设计面板进行交互时，确保设计面板有焦点
            _eatAllHitTestRequests.MouseDown += delegate { Focus(); };
            // 允许拖放
            _eatAllHitTestRequests.AllowDrop = true;
            // 创建一个装饰器图层，用来显示AdornerPanel的内容（TODO：这个不是系统自带的AdornerLayer，后续改造一下）
            AdornerLayer = new AdornerLayer(this);

            this.PreviewKeyUp += DesignPanel_KeyUp;
            this.PreviewKeyDown += DesignPanel_KeyDown;
        }
        #endregion

        #region Properties

        public DesignSurface DesignSurface { get; internal set; }

        // 设置自定义HitTestFilterCallbak
        public HitTestFilterCallback CustomHitTestFilterBehavior { get; set; }

        public AdornerLayer AdornerLayer { get; }

        /// <summary>
        /// 获取/设置设计上下文。
        /// </summary>
        public DesignContext Context { get; set; }

        public ICollection<AdornerPanel> Adorners => AdornerLayer.Adorners;

        /// <summary>
        /// 获取/设置设计内容是否对命中测试可见。
        /// </summary>
        public bool IsContentHitTestVisible
        {
            get => !_eatAllHitTestRequests.IsHitTestVisible;
            set => _eatAllHitTestRequests.IsHitTestVisible = !value;
        }

        /// <summary>
        /// 获取/设置装饰层是否对命中测试可见。
        /// </summary>
        public bool IsAdornerLayerHitTestVisible
        {
            get => AdornerLayer.IsHitTestVisible;
            set => AdornerLayer.IsHitTestVisible = value;
        }

        /// <summary>
        /// 启用/禁用吸附放置
        /// </summary>
        private bool _useSnaplinePlacement = true;
        public bool UseSnaplinePlacement
        {
            get => _useSnaplinePlacement;
            set
            {
                if (_useSnaplinePlacement != value)
                {
                    _useSnaplinePlacement = value;
                    OnPropertyChanged(nameof(UseSnaplinePlacement));
                }
            }
        }

        /// <summary>
        /// 启用/禁用栅格放置
        /// </summary>
        private bool _useRasterPlacement = false;
        public bool UseRasterPlacement
        {
            get => _useRasterPlacement;
            set
            {
                if (_useRasterPlacement != value)
                {
                    _useRasterPlacement = value;
                    OnPropertyChanged(nameof(UseRasterPlacement));
                }
            }
        }

        /// <summary>
        /// 当使用栅格放置时设置栅格的with
        /// </summary>
        private int _rasterWidth = 5;
        public int RasterWidth
        {
            get => _rasterWidth;
            set
            {
                if (_rasterWidth != value)
                {
                    _rasterWidth = value;
                    OnPropertyChanged(nameof(RasterWidth));
                }
            }
        }

        #endregion

        #region Visual Child Management

        public override UIElement Child
        {
            get => base.Child;
            set
            {
                if (base.Child == value)
                    return;
                if (value == null)
                {
                    // Child is being set from some value to null

                    // remove _adornerLayer and _eatAllHitTestRequests
                    RemoveVisualChild(AdornerLayer);
                    RemoveVisualChild(_eatAllHitTestRequests);
                }
                else if (base.Child == null)
                {
                    // Child is being set from null to some value
                    AddVisualChild(AdornerLayer);
                    AddVisualChild(_eatAllHitTestRequests);
                }
                base.Child = value;
            }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (base.Child != null)
            {
                if (index == 0)
                    return base.Child;
                else if (index == 1)
                    return _eatAllHitTestRequests;
                else if (index == 2)
                    return AdornerLayer;
            }
            return base.GetVisualChild(index);
        }

        protected override int VisualChildrenCount => base.Child != null ? 3 : base.VisualChildrenCount;

        protected override Size MeasureOverride(Size constraint)
        {
            Size result = base.MeasureOverride(constraint);
            if (this.Child != null)
            {
                AdornerLayer.Measure(constraint);
                _eatAllHitTestRequests.Measure(constraint);
            }
            return result;
        }

        protected override Size ArrangeOverride(Size arrangeSize)
        {
            Size result = base.ArrangeOverride(arrangeSize);
            if (this.Child != null)
            {
                Rect r = new(new Point(0, 0), arrangeSize);
                AdornerLayer.Arrange(r);
                _eatAllHitTestRequests.Arrange(r);
            }
            return result;
        }
        #endregion

        private PlacementOperation placementOp;
        Dictionary<PlacementInformation, int> dx = new();
        Dictionary<PlacementInformation, int> dy = new();

        /// <summary>
        /// If interface implementing class sets this to false defaultkeyaction will be
        /// 如果接口实现类将此设置为false，则defaultkeyaction将为
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        private bool InvokeDefaultKeyDownAction(Extension e)
        {
            IKeyDown keyDown = e as IKeyDown;
            if (keyDown != null)
            {
                return keyDown.InvokeDefaultAction;
            }

            return true;
        }

        /// <summary>
        /// 松开某个键触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DesignPanel_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key is Key.Left or Key.Right or Key.Up or Key.Down)
            {
                e.Handled = true;

                if (placementOp != null)
                {
                    placementOp.Commit();
                    placementOp = null;
                    dx.Clear();
                    dy.Clear();
                }
            }

            // 如果底层对象实现了IKeyUp接口，则将键事件传递给底层对象
            // 这个调用需要在这里，在placementOp.Commit()之后。
            // 如果底层对象有自己的操作，则需要先提交该操作
            foreach (DesignItem di in Context.Services.Selection.SelectedItems.Reverse())
            {
                foreach (Extension ext in di.Extensions)
                {
                    if (ext is IKeyUp keyUp)
                    {
                        keyUp.KeyUpAction(sender, e);
                    }
                }
            }
        }

        /// <summary>
        /// 按下某个键触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DesignPanel_KeyDown(object sender, KeyEventArgs e)
        {

            // 如果底层对象实现了IKeyUp接口，则将键事件传递给底层对象
            // 这个调用需要在这里，在placementOp.Commit()之后。
            // 如果底层对象有自己的操作，则需要先提交该操作
            foreach (DesignItem di in Context.Services.Selection.SelectedItems)
            {
                foreach (Extension ext in di.Extensions)
                {
                    if (ext is IKeyDown keyDown)
                    {
                        keyDown.KeyDownAction(sender, e);
                    }
                }
            }

            if (e.Key is Key.Left or Key.Right or Key.Up or Key.Down)
            {

                e.Handled = true;

                PlacementType placementType = Keyboard.IsKeyDown(Key.LeftCtrl) ? PlacementType.Resize : PlacementType.MoveAndIgnoreOtherContainers;

                if (placementOp != null && placementOp.Type != placementType)
                {
                    placementOp.Commit();
                    placementOp = null;
                    dx.Clear();
                    dy.Clear();
                }

                if (placementOp == null)
                {
                    // 检查是否有对象不希望调用默认操作
                    List<DesignItem> placedItems = Context.Services.Selection.SelectedItems.Where(x => x.Extensions.All(InvokeDefaultKeyDownAction)).ToList();

                    // 如果没有剩余的对象，中断
                    if (placedItems.Count < 1) return;

                    placementOp = PlacementOperation.Start(placedItems, placementType);

                    dx.Clear();
                    dy.Clear();
                }

                int odx = 0, ody = 0;
                switch (e.Key)
                {
                    case Key.Left:
                        odx = Keyboard.IsKeyDown(Key.LeftShift) ? -10 : -1;
                        break;
                    case Key.Up:
                        ody = Keyboard.IsKeyDown(Key.LeftShift) ? -10 : -1;
                        break;
                    case Key.Right:
                        odx = Keyboard.IsKeyDown(Key.LeftShift) ? 10 : 1;
                        break;
                    case Key.Down:
                        ody = Keyboard.IsKeyDown(Key.LeftShift) ? 10 : 1;
                        break;
                }

                foreach (PlacementInformation info in placementOp.PlacedItems)
                {
                    if (!dx.ContainsKey(info))
                    {
                        dx[info] = 0;
                        dy[info] = 0;
                    }
                    GeneralTransform transform = info.Item.Parent.View.TransformToVisual(this);
                    if (transform is MatrixTransform mt)
                    {
                        double angle = Math.Atan2(mt.Matrix.M21, mt.Matrix.M11) * 180 / Math.PI;
                        if (angle is > 45.0 and < 135.0)
                        {
                            dx[info] += ody * -1;
                            dy[info] += odx;
                        }
                        else if (angle < -45.0 && angle > -135.0)
                        {
                            dx[info] += ody;
                            dy[info] += odx * -1;
                        }
                        else if (angle > 135.0 || angle < -135.0)
                        {
                            dx[info] += odx * -1;
                            dy[info] += ody * -1;
                        }
                        else
                        {
                            dx[info] += odx;
                            dy[info] += ody;
                        }
                    }

                    Rect bounds = info.OriginalBounds;

                    if (placementType == PlacementType.Move || info.Operation.Type == PlacementType.MoveAndIgnoreOtherContainers)
                    {
                        info.Bounds = new Rect(bounds.Left + dx[info], bounds.Top + dy[info], bounds.Width, bounds.Height);
                    }
                    else if (placementType == PlacementType.Resize)
                    {
                        if (bounds.Width + dx[info] >= 0 && bounds.Height + dy[info] >= 0)
                        {
                            info.Bounds = new Rect(bounds.Left, bounds.Top, bounds.Width + dx[info], bounds.Height + dy[info]);
                        }
                    }

                    placementOp.CurrentContainerBehavior.SetPosition(info);
                }
            }
        }

        private static bool IsPropertySet(UIElement element, DependencyProperty d) => element.ReadLocalValue(d) != DependencyProperty.UnsetValue;

        protected override void OnQueryCursor(QueryCursorEventArgs e)
        {
            base.OnQueryCursor(e);
            if (Context != null)
            {
                Cursor cursor = Context.Services.Tool.CurrentTool.Cursor;
                if (cursor != null)
                {
                    e.Cursor = cursor;
                    e.Handled = true;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        #region ContextMenu

        private readonly Dictionary<ContextMenu, Tuple<int, List<object>>> contextMenusAndEntries = new Dictionary<ContextMenu, Tuple<int, List<object>>>();

        public Action<ContextMenu> ContextMenuHandler { get; set; }

        public void AddContextMenu(ContextMenu contextMenu) => AddContextMenu(contextMenu, int.MaxValue);

        public void AddContextMenu(ContextMenu contextMenu, int orderIndex)
        {
            contextMenusAndEntries.Add(contextMenu, new Tuple<int, List<object>>(orderIndex, new List<object>(contextMenu.Items.Cast<object>())));
            contextMenu.Items.Clear();

            UpdateContextMenu();
        }

        public void RemoveContextMenu(ContextMenu contextMenu)
        {
            contextMenusAndEntries.Remove(contextMenu);

            UpdateContextMenu();
        }

        public void ClearContextMenu()
        {
            contextMenusAndEntries.Clear();
            ContextMenu = null;
        }

        private void UpdateContextMenu()
        {
            if (this.ContextMenu != null)
            {
                this.ContextMenu.Items.Clear();
                this.ContextMenu = null;
            }

            ContextMenu contextMenu = new();

            foreach (List<object> entries in contextMenusAndEntries.Values.OrderBy(x => x.Item1).Select(x => x.Item2))
            {
                if (contextMenu.Items.Count > 0)
                    contextMenu.Items.Add(new Separator());

                foreach (object entry in entries)
                {
                    ItemsControl ctl = ((FrameworkElement)entry).TryFindParent<ItemsControl>();
                    if (ctl != null)
                        ctl.Items.Remove(entry);
                    contextMenu.Items.Add(entry);
                }
            }

            if (ContextMenuHandler != null)
                ContextMenuHandler(contextMenu);
            else
                this.ContextMenu = contextMenu;
        }

        #endregion
    }
}
