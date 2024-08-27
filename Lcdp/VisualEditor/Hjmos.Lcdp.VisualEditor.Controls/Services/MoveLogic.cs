using System;
using System.Collections.Generic;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Services
{
    /// <summary>
    /// 鼠标移动的逻辑类
    /// </summary>
    internal class MoveLogic
    {
        public MoveLogic(DesignItem clickedOn)
        {
            // 鼠标点击的组件（或者点击测试到的组件）
            this.ClickedOn = clickedOn;

            // 通过DesignItem获取设计界面上下文的服务，通过服务返回选定组件的集合。
            selectedItems = clickedOn.Services.Selection.SelectedItems;

            // 如果选定组件的集合不包含当前点击的组件
            if (!selectedItems.Contains(clickedOn))
                // 赋值一个空的静态DesignItem数组
                selectedItems = SharedInstances.EmptyDesignItemArray;
        }

        /// <summary>设计界面上选定组件的集合的副本</summary>
        private ICollection<DesignItem> selectedItems;

        /// <summary>起始坐标</summary>
        private Point startPoint;

        /// <summary>鼠标点击的组件项</summary>
        public DesignItem ClickedOn { get; private set; }

        /// <summary>放置操作</summary>
        public PlacementOperation Operation { get; private set; }

        public IDesignPanel DesignPanel => ClickedOn.Services.DesignPanel;

        /// <summary>
        /// 放置组件到指定的位置
        /// </summary>
        /// <param name="p"></param>
        public void Start(Point p)
        {
            startPoint = p;
            IPlacementBehavior behavior = PlacementOperation.GetPlacementBehavior(selectedItems);
            if (behavior != null && behavior.CanPlace(selectedItems, PlacementType.Move, PlacementAlignment.TopLeft))
            {
                List<DesignItem> sortedSelectedItems = new(selectedItems);
                sortedSelectedItems.Sort(ModelTools.ComparePositionInModelFile);
                selectedItems = sortedSelectedItems;
                Operation = PlacementOperation.Start(selectedItems, PlacementType.Move);
            }
        }

        /// <summary>
        /// 移动组件到指定的位置
        /// </summary>
        /// <param name="p"></param>
        public void Move(Point point)
        {
            if (Operation != null)
            {
                // 试着转换容器
                if (Operation.CurrentContainerBehavior.CanLeaveContainer(Operation))
                {
                    ChangeContainerIfPossible(point);
                }

                Vector vector;

                if (Operation.CurrentContainer.View != null && this.DesignPanel is UIElement designPanel)
                {
                    vector = designPanel.TranslatePoint(point, Operation.CurrentContainer.View) - designPanel.TranslatePoint(startPoint, Operation.CurrentContainer.View);
                }
                else
                {
                    vector = point - startPoint;
                }

                foreach (PlacementInformation info in Operation.PlacedItems)
                {
                    info.Bounds = new Rect(info.OriginalBounds.Left + Math.Round(vector.X, PlacementInformation.BoundsPrecision),
                                           info.OriginalBounds.Top + Math.Round(vector.Y, PlacementInformation.BoundsPrecision),
                                           info.OriginalBounds.Width,
                                           info.OriginalBounds.Height);
                }
                Operation.CurrentContainerBehavior.BeforeSetPosition(Operation);
                foreach (PlacementInformation info in Operation.PlacedItems)
                {
                    Operation.CurrentContainerBehavior.SetPosition(info);
                }
            }
        }

        public void Stop()
        {
            if (Operation != null)
            {
                Operation.Commit();
                Operation = null;
            }
        }

        public void Cancel()
        {
            if (Operation != null)
            {
                Operation.Abort();
                Operation = null;
            }
        }

        // Perform hit testing on the design panel and return the first model that is not selected
        // 在设计面板上执行命中测试，并返回未选中的第一个模型  
        private DesignPanelHitTestResult HitTestUnselectedModel(Point p)
        {
            DesignPanelHitTestResult result = DesignPanelHitTestResult.NoHit;
            ISelectionService selection = ClickedOn.Services.Selection;

            DesignPanel.HitTest(p, false, true, delegate (DesignPanelHitTestResult r)
            {
                if (r.ModelHit == null)
                    return true; // continue hit testing 继续命中测试
                if (selection.IsComponentSelected(r.ModelHit))
                    return true; // continue hit testing 继续命中测试
                result = r;
                return false; // finish hit testing  停止命中测试
            }, HitTestType.Default);

            return result;
        }

        private bool ChangeContainerIfPossible(Point p)
        {
            DesignPanelHitTestResult result = HitTestUnselectedModel(p);
            if (result.ModelHit == null) return false;
            if (result.ModelHit == Operation.CurrentContainer) return false;

            // check that we don't move an item into itself:
            // 检查我们没有移动一个项目到它本身:
            DesignItem tmp = result.ModelHit;
            while (tmp != null)
            {
                if (tmp == ClickedOn) return false;
                tmp = tmp.Parent;
            }

            IPlacementBehavior b = result.ModelHit.GetBehavior<IPlacementBehavior>();
            if (b != null && b.CanEnterContainer(Operation, false))
            {
                Operation.ChangeContainer(result.ModelHit);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 处理双击操作
        /// </summary>
        public void HandleDoubleClick()
        {
            if (selectedItems.Count == 1)
            {
                IEventHandlerService ehs = ClickedOn.Services.GetService<IEventHandlerService>();
                if (ehs != null)
                {
                    DesignItemProperty defaultEvent = ehs.GetDefaultEvent(ClickedOn);
                    if (defaultEvent != null)
                    {
                        ehs.CreateEventHandler(defaultEvent);
                    }
                }
            }
        }
    }
}
