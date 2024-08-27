using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Controls.Services
{
    /// <summary>
    /// 创建组件的工具。
    /// </summary>
    public class CreateComponentTool : ITool
    {
        private MoveLogic _moveLogic;
        private Point _createPoint;

        /// <summary>创建一个新的CreateComponentTool实例</summary>
        public CreateComponentTool(Type componentType) => ComponentType = componentType ?? throw new ArgumentNullException("componentType");

        /// <summary>获取要创建的组件的类型</summary>
        public Type ComponentType { get; }

        public Cursor Cursor => Cursors.Cross;

        public void Activate(IDesignPanel designPanel)
        {
            designPanel.MouseDown += OnMouseDown;
            designPanel.DragOver += DesignPanel_DragOver;
            designPanel.Drop += DesignPanel_Drop;
            designPanel.DragLeave += DesignPanel_DragLeave;
        }

        public void Deactivate(IDesignPanel designPanel)
        {
            designPanel.MouseDown -= OnMouseDown;
            designPanel.DragOver -= DesignPanel_DragOver;
            designPanel.Drop -= DesignPanel_Drop;
            designPanel.DragLeave -= DesignPanel_DragLeave;
        }

        /// <summary>
        /// 鼠标覆盖设计面板时触发的方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DesignPanel_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                IDesignPanel designPanel = (IDesignPanel)sender;

                e.Effects = DragDropEffects.Copy;

                e.Handled = true;
                Point p = e.GetPosition(designPanel);

                if (_moveLogic == null)
                {
                    if (e.Data.GetData(this.GetType()) != this) return;
                    // TODO: dropLayer in designPanel
                    designPanel.IsAdornerLayerHitTestVisible = false;
                    DesignPanelHitTestResult result = designPanel.HitTest(p, false, true, HitTestType.Default);

                    if (result.ModelHit != null)
                    {
                        designPanel.Focus();

                        #region 这一段似乎是用来一次创建多个设计项的，但是暂时没有实现，所有返回都是null
                        //var items = CreateItemsWithPosition(designPanel.Context, e.GetPosition(result.ModelHit.View));
                        //if (items != null)
                        //{
                        //    if (AddItemsWithDefaultSize(result.ModelHit, items))
                        //    {
                        //        _moveLogic = new MoveLogic(items[0]);

                        //        foreach (DesignItem designItem in items)
                        //        {
                        //            if (designPanel.Context.Services.Component is MyComponentService service)
                        //            {
                        //                service.RaiseComponentRegisteredAndAddedToContainer(designItem);
                        //            }
                        //        }
                        //        _createPoint = p;
                        //        // We'll keep the ChangeGroup open as long as the moveLogic is active.
                        //        // 我们将保持ChangeGroup打开，只要moveLogic是活动的。
                        //    }
                        //    else
                        //    {
                        //        // Abort the ChangeGroup created by the CreateItem() call.
                        //        // 终止由CreateItem()调用创建的ChangeGroup。
                        //        //ChangeGroup.Abort();
                        //    }
                        //}
                        //else
                        //{

                        #endregion

                        // 根据鼠标位置，在设计面板上创建设计项实例
                        DesignItem createdItem = CreateItemWithPosition(designPanel.Context, e.GetPosition(result.ModelHit.View));

                        if (AddItemsWithDefaultSize(result.ModelHit, new[] { createdItem }))
                        {
                            _moveLogic = new MoveLogic(createdItem);

                            if (designPanel.Context.Services.Component is MyComponentService service)
                            {
                                service.RaiseComponentRegisteredAndAddedToContainer(createdItem);
                            }

                            _createPoint = p;
                            // 我们将保持ChangeGroup打开，只要moveLogic是活动的。
                        }
                        else
                        {
                            // 终止由CreateItem()调用创建的ChangeGroup。
                            //ChangeGroup.Abort();
                        }
                        //}
                    }
                }
                else if ((_moveLogic.ClickedOn.View as FrameworkElement).IsLoaded)
                {
                    if (_moveLogic.Operation == null)
                    {
                        _moveLogic.Start(_createPoint);
                    }
                    else
                    {
                        _moveLogic.Move(p);
                    }
                }
            }
            catch (Exception x)
            {
                //DragDropExceptionHandler.RaiseUnhandledException(x);
            }
        }

        private void DesignPanel_Drop(object sender, DragEventArgs e)
        {
            try
            {
                if (_moveLogic != null)
                {
                    _moveLogic.Stop();
                    if (_moveLogic.ClickedOn.Services.Tool.CurrentTool is CreateComponentTool)
                    {
                        _moveLogic.ClickedOn.Services.Tool.CurrentTool = _moveLogic.ClickedOn.Services.Tool.PointerTool;
                    }
                    _moveLogic.DesignPanel.IsAdornerLayerHitTestVisible = true;
                    _moveLogic = null;
                    //ChangeGroup.Commit();

                    e.Handled = true;
                }
            }
            catch (Exception x)
            {
                //DragDropExceptionHandler.RaiseUnhandledException(x);
            }
        }

        private void DesignPanel_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                if (_moveLogic != null)
                {
                    _moveLogic.Cancel();
                    _moveLogic.ClickedOn.Services.Selection.SetSelectedComponents(null);
                    _moveLogic.DesignPanel.IsAdornerLayerHitTestVisible = true;
                    _moveLogic = null;
                    //ChangeGroup.Abort();

                }
            }
            catch (Exception x)
            {
                //DragDropExceptionHandler.RaiseUnhandledException(x);
            }
        }

        /// <summary>
        /// 调用以创建CreateComponentTool使用的项。  
        /// </summary>
        protected virtual DesignItem CreateItemWithPosition(DesignContext context, Point position)
        {
            DesignItem item = CreateItem(context);
            item.Position = position;
            return item;
        }

        /// <summary>
        /// 调用以创建CreateComponentTool使用的项。  
        /// </summary>
        protected virtual DesignItem CreateItem(DesignContext context)
        {
            //if (ChangeGroup == null)
            //    ChangeGroup = context.RootItem.OpenGroup("Add Control");
            DesignItem item = CreateItem(context, ComponentType);
            return item;
        }

        protected virtual DesignItem[] CreateItemsWithPosition(DesignContext context, Point position)
        {
            DesignItem[] items = CreateItems(context);
            if (items != null)
            {
                foreach (var designItem in items)
                {
                    designItem.Position = position;
                }
            }

            return items;
        }

        protected virtual DesignItem[] CreateItems(DesignContext context) => null;

        /// <summary>
        /// 调用以创建CreateComponentTool使用的项
        /// </summary>
        public static DesignItem CreateItem(DesignContext context, Type type)
        {
            object newInstance = context.Services.ExtensionManager.CreateInstanceWithCustomInstanceFactory(type, null);
            DesignItem item = context.Services.Component.RegisterComponentForDesigner(newInstance);
            context.Services.Component.SetDefaultPropertyValues(item);
            context.Services.ExtensionManager.ApplyDefaultInitializers(item);
            return item;
        }

        /// <summary>
        /// 用来设置绘制项的属性
        /// </summary>
        protected virtual void SetPropertiesForDrawnItem(DesignItem designItem) { }

        public static bool AddItemWithCustomSizePosition(DesignItem container, Type createdItem, Size size, Point position)
        {
            CreateComponentTool cct = new(createdItem);
            return AddItemsWithCustomSize(container, new[] { cct.CreateItem(container.Context) }, new[] { new Rect(position, size) });
        }

        public static bool AddItemWithDefaultSize(DesignItem container, Type createdItem, Size size)
        {
            CreateComponentTool cct = new CreateComponentTool(createdItem);
            return AddItemsWithCustomSize(container, new[] { cct.CreateItem(container.Context) }, new[] { new Rect(new Point(0, 0), size) });
        }

        /// <summary>
        /// 用默认大小添加绘制项
        /// </summary>
        /// <param name="container"></param>
        /// <param name="createdItems"></param>
        /// <returns></returns>
        internal static bool AddItemsWithDefaultSize(DesignItem container, DesignItem[] createdItems)
        {
            return AddItemsWithCustomSize(container, createdItems, createdItems.Select(x => new Rect(x.Position, ModelTools.GetDefaultSize(x))).ToList());
        }

        /// <summary>
        /// 用自定义大小添加绘制项
        /// </summary>
        /// <param name="container"></param>
        /// <param name="createdItems"></param>
        /// <param name="positions"></param>
        /// <returns></returns>
        internal static bool AddItemsWithCustomSize(DesignItem container, DesignItem[] createdItems, IList<Rect> positions)
        {
            PlacementOperation operation = null;

            while (operation == null && container != null)
            {
                operation = PlacementOperation.TryStartInsertNewComponents(container, createdItems, positions, PlacementType.AddItem);

                if (operation != null)
                    break;

                try
                {
                    if (container.Parent != null)
                    {
                        Point rel = container.View.TranslatePoint(new Point(0, 0), container.Parent.View);
                        for (int index = 0; index < positions.Count; index++)
                        {
                            positions[index] = new Rect(new Point(positions[index].X + rel.X, positions[index].Y + rel.Y), positions[index].Size);
                        }
                    }
                }
                catch (Exception) { }

                container = container.Parent;
            }

            if (operation != null)
            {
                container.Services.Selection.SetSelectedComponents(createdItems);
                operation.Commit();
                return true;
            }
            else
            {
                return false;
            }
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left && MouseGestureBase.IsOnlyButtonPressed(e, MouseButton.Left))
            {
                e.Handled = true;
                IDesignPanel designPanel = (IDesignPanel)sender;
                DesignPanelHitTestResult result = designPanel.HitTest(e.GetPosition(designPanel), false, true, HitTestType.Default);
                if (result.ModelHit != null)
                {
                    IEnumerable<IDrawItemExtension> darwItemBehaviors = result.ModelHit.Extensions.OfType<IDrawItemExtension>();
                    IDrawItemExtension drawItembehavior = darwItemBehaviors.FirstOrDefault(x => x.CanItemBeDrawn(ComponentType));
                    if (drawItembehavior != null && drawItembehavior.CanItemBeDrawn(ComponentType))
                    {
                        drawItembehavior.StartDrawItem(result.ModelHit, ComponentType, designPanel, e, SetPropertiesForDrawnItem);
                    }
                    else
                    {
                        IPlacementBehavior placementBehavior = result.ModelHit.GetBehavior<IPlacementBehavior>();
                        if (placementBehavior != null)
                        {
                            DesignItem createdItem = CreateItem(designPanel.Context);

                            new CreateComponentMouseGesture(result.ModelHit, createdItem).Start(designPanel, e);
                            //// CreateComponentMouseGesture now is responsible for the changeGroup created by CreateItem()
                            //ChangeGroup = null;
                        }
                    }
                }
            }
        }
    }
}
