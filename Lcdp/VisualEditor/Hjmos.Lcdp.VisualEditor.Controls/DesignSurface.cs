using Hjmos.Lcdp.VisualEditor.Controls.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    [TemplatePart(Name = "PART_DesignContent", Type = typeof(ContentControl))]
    [TemplatePart(Name = "PART_Zoom", Type = typeof(ZoomControl))]
    public class DesignSurface : ContentControl, INotifyPropertyChanged
    {
        #region Property

        /// <summary>
        /// TODO: 应该是画布缩放控制的
        /// </summary>
        public ZoomControl ZoomControl { get; private set; }

        /// <summary>
        /// 应用容器
        /// </summary>
        public PageShell PageShell
        {
            get => (PageShell)GetValue(PageShellProperty);
            set => SetValue(PageShellProperty, value);
        }

        public static readonly DependencyProperty PageShellProperty = DependencyProperty.Register("PageShell", typeof(PageShell), typeof(DesignSurface));

        public ScrollViewer ScrollViewer
        {
            get => (ScrollViewer)GetValue(ScrollViewerProperty);
            set => SetValue(ScrollViewerProperty, value);
        }

        public static readonly DependencyProperty ScrollViewerProperty =
            DependencyProperty.Register("ScrollViewer", typeof(ScrollViewer), typeof(DesignSurface));

        #endregion

        public DesignPanel DesignPanel { get; private set; }

        private ContentControl _partDesignContent;
        private readonly Border _sceneContainer;

        static DesignSurface() => DefaultStyleKeyProperty.OverrideMetadata(typeof(DesignSurface), new FrameworkPropertyMetadata(typeof(DesignSurface)));

        public DesignSurface()
        {
            //Propertygrid should show no inherited Datacontext!
            this.DataContext = null;

            this.AddCommandHandler(ApplicationCommands.Undo, Undo, CanUndo);
            this.AddCommandHandler(ApplicationCommands.Redo, Redo, CanRedo);
            this.AddCommandHandler(ApplicationCommands.Copy, Copy, CanCopyOrCut);
            this.AddCommandHandler(ApplicationCommands.Cut, Cut, CanCopyOrCut);
            this.AddCommandHandler(ApplicationCommands.Delete, Delete, CanDelete);
            this.AddCommandHandler(ApplicationCommands.Paste, Paste, CanPaste);
            this.AddCommandHandler(ApplicationCommands.SelectAll, SelectAll, CanSelectAll);

            this.AddCommandHandler(Commands.AlignTopCommand, () => ModelTools.ArrangeItems(this.DesignContext.Services.Selection.SelectedItems, ArrangeDirection.Top), () => this.DesignContext.Services.Selection.SelectedItems.Count() > 1);
            this.AddCommandHandler(Commands.AlignMiddleCommand, () => ModelTools.ArrangeItems(this.DesignContext.Services.Selection.SelectedItems, ArrangeDirection.VerticalMiddle), () => this.DesignContext.Services.Selection.SelectedItems.Count() > 1);
            this.AddCommandHandler(Commands.AlignBottomCommand, () => ModelTools.ArrangeItems(this.DesignContext.Services.Selection.SelectedItems, ArrangeDirection.Bottom), () => this.DesignContext.Services.Selection.SelectedItems.Count() > 1);
            this.AddCommandHandler(Commands.AlignLeftCommand, () => ModelTools.ArrangeItems(this.DesignContext.Services.Selection.SelectedItems, ArrangeDirection.Left), () => this.DesignContext.Services.Selection.SelectedItems.Count() > 1);
            this.AddCommandHandler(Commands.AlignCenterCommand, () => ModelTools.ArrangeItems(this.DesignContext.Services.Selection.SelectedItems, ArrangeDirection.HorizontalMiddle), () => this.DesignContext.Services.Selection.SelectedItems.Count() > 1);
            this.AddCommandHandler(Commands.AlignRightCommand, () => ModelTools.ArrangeItems(this.DesignContext.Services.Selection.SelectedItems, ArrangeDirection.Right), () => this.DesignContext.Services.Selection.SelectedItems.Count() > 1);

            this.AddCommandHandler(Commands.RotateLeftCommand, () => ModelTools.ApplyTransform(this.DesignContext.Services.Selection.PrimarySelection, new RotateTransform(-90), true, this.DesignContext.RootItem == this.DesignContext.Services.Selection.PrimarySelection ? LayoutTransformProperty : RenderTransformProperty), () => this.DesignContext.Services.Selection.PrimarySelection != null);
            this.AddCommandHandler(Commands.RotateRightCommand, () => ModelTools.ApplyTransform(this.DesignContext.Services.Selection.PrimarySelection, new RotateTransform(90), true, this.DesignContext.RootItem == this.DesignContext.Services.Selection.PrimarySelection ? LayoutTransformProperty : RenderTransformProperty), () => this.DesignContext.Services.Selection.PrimarySelection != null);

            this.AddCommandHandler(Commands.StretchToSameWidthCommand, () => ModelTools.StretchItems(this.DesignContext.Services.Selection.SelectedItems, StretchDirection.Width), () => this.DesignContext.Services.Selection.SelectedItems.Count() > 1);
            this.AddCommandHandler(Commands.StretchToSameHeightCommand, () => ModelTools.StretchItems(this.DesignContext.Services.Selection.SelectedItems, StretchDirection.Height), () => this.DesignContext.Services.Selection.SelectedItems.Count() > 1);


            _sceneContainer = new Border() { AllowDrop = false, UseLayoutRounding = true };
            _sceneContainer.SetValue(TextOptions.TextFormattingModeProperty, TextFormattingMode.Ideal);

            DesignPanel = new DesignPanel() { Child = _sceneContainer, DesignSurface = this };

            InitializeDesigner();
        }


        /// <summary>
        /// 获取活动的设计上下文
        /// </summary>
        public DesignContext DesignContext { get; private set; }

        /// <summary>
        /// 是否能滚动到可见区域
        /// </summary>
        private bool _enableBringIntoView = false;

        private FocusNavigator _focusNav;

        /// <summary>
        /// 初始化设计器
        /// </summary>
        private void InitializeDesigner()
        {

            // 用于加载一些上下文需要的初始化设置。
            MyLoadSettings loadSettings = new();
            loadSettings.DesignerAssemblies.Add(this.GetType().Assembly);

            // 自定义注册服务，注册一个实现了IDesignPanel的上下文服务
            // IDesignPanel是包含设计元素的UIElement，负责处理鼠标和键盘事件。  
            loadSettings.CustomServiceRegisterFunctions.Add(context => context.Services.AddService(typeof(IDesignPanel), DesignPanel));

            MyDesignContext context = new(loadSettings);

            DesignContext = context;


            DesignPanel.Context = context;
            DesignPanel.ClearContextMenu();

            if (context.RootItem != null)
            {
                _sceneContainer.Child = context.RootItem.View;


            }

            if (context.RootItem.View is PageShell pageShell)
            {
                this.PageShell = pageShell;
            }

            //// TODO：在设计图面上支持撤消/重做操作的服务。
            //context.Services.RunWhenAvailable<UndoService>(
            //    undoService => undoService.UndoStackChanged += delegate
            //    {
            //        CommandManager.InvalidateRequerySuggested();
            //    }
            //);

            //注册当前选择改变事件
            context.Services.Selection.SelectionChanged += delegate { CommandManager.InvalidateRequerySuggested(); };

            // 我们希望提供我们自己的IComponentPropertyService实现，因此我们只返回我们想要的属性。
            // 这可能在MyDesignerModel类中更好，但必须等到DesignContext创建之后。
            context.Services.AddOrReplaceService(typeof(IComponentPropertyService), new MyComponentPropertyService());

            context.Services.AddService(typeof(IKeyBindingService), new DesignerKeyBindings(this));
            _focusNav = new FocusNavigator(this);
            _focusNav.Start();

            OnPropertyChanged("DesignContext");
        }

        public override void OnApplyTemplate()
        {
            _partDesignContent = Template.FindName("PART_DesignContent", this) as ContentControl;
            _partDesignContent.Content = DesignPanel;
            _partDesignContent.RequestBringIntoView += PartDesignContent_RequestBringIntoView;

            ZoomControl = Template.FindName("PART_Zoom", this) as ZoomControl;
            OnPropertyChanged("ZoomControl");

            base.OnApplyTemplate();
        }

        /// <summary>
        /// 此事件向父ScrollViewer（或派生类）指示引发RequestBringIntoView事件的元素应在可滚动区域内可见
        /// TODO: 后续仔细了解一下这个内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PartDesignContent_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            if (!_enableBringIntoView)
                e.Handled = true;
            _enableBringIntoView = false;
        }

        #region Commands

        public bool CanUndo()
        {
            UndoService undoService = GetService<UndoService>();
            return undoService != null && undoService.CanUndo;
        }

        public void Undo()
        {
            UndoService undoService = GetService<UndoService>();
            IUndoAction action = undoService.UndoActions.First();
            Debug.WriteLine("Undo " + action.Title);
            undoService.Undo();
            DesignContext.Services.Selection.SetSelectedComponents(GetLiveElements(action.AffectedElements));
        }

        public bool CanRedo()
        {
            UndoService undoService = GetService<UndoService>();
            return undoService != null && undoService.CanRedo;
        }

        public void Redo()
        {
            UndoService undoService = GetService<UndoService>();
            IUndoAction action = undoService.RedoActions.First();
            Debug.WriteLine("Redo " + action.Title);
            undoService.Redo();
            DesignContext.Services.Selection.SetSelectedComponents(GetLiveElements(action.AffectedElements));
        }

        public bool CanCopyOrCut()
        {
            ISelectionService selectionService = GetService<ISelectionService>();
            if (selectionService != null)
            {
                if (selectionService.SelectedItems.Count == 0)
                    return false;
                if (selectionService.SelectedItems.Count == 1 && selectionService.PrimarySelection == DesignContext.RootItem)
                    return false;
            }
            return true;
        }

        public void Copy()
        {
            ISelectionService selectionService = GetService<ISelectionService>();
            if (DesignContext is MyDesignContext myContext && selectionService != null)
            {
                //myContext.XamlEditAction.Copy(selectionService.SelectedItems);
            }
        }

        public void Cut()
        {
            ISelectionService selectionService = GetService<ISelectionService>();
            if (DesignContext is MyDesignContext myContext && selectionService != null)
            {
                //myContext.XamlEditAction.Cut(selectionService.SelectedItems);
            }
        }

        public bool CanDelete()
        {
            return DesignContext != null && ModelTools.CanDeleteComponents(DesignContext.Services.Selection.SelectedItems);
        }

        public void Delete()
        {
            if (DesignContext != null)
            {
                ModelTools.DeleteComponents(DesignContext.Services.Selection.SelectedItems);
            }
        }

        public bool CanPaste()
        {
            ISelectionService selectionService = GetService<ISelectionService>();
            if (selectionService != null && selectionService.SelectedItems.Count != 0)
            {
                try
                {
                    string xaml = Clipboard.GetText(TextDataFormat.Xaml);
                    if (xaml != "" && xaml != " ")
                        return true;
                }
                catch (Exception)
                {
                }
            }
            return false;
        }

        public void Paste()
        {
            if (DesignContext is MyDesignContext myContext)
            {
                //myContext.XamlEditAction.Paste();
            }
        }

        public bool CanSelectAll() => DesignContext != null;

        //TODO: Do not select layout root
        public void SelectAll()
        {
            DesignItem[] items = Descendants(DesignContext.RootItem).Where(item => ModelTools.CanSelectComponent(item)).ToArray();
            DesignContext.Services.Selection.SetSelectedComponents(items);
        }

        // 取消所有组件选中状态
        public void UnselectAll() => DesignContext.Services.Selection.SetSelectedComponents(null);

        //TODO: Share with Outline / PlacementBehavior
        //待办事项:与Outline / PlacementBehavior分享
        public static IEnumerable<DesignItem> DescendantsAndSelf(DesignItem item)
        {
            yield return item;
            foreach (DesignItem child in Descendants(item))
            {
                yield return child;
            }
        }

        public static IEnumerable<DesignItem> Descendants(DesignItem item)
        {
            if (item.ContentPropertyName != null)
            {
                DesignItemProperty content = item.ContentProperty;
                if (content.IsCollection)
                {
                    foreach (DesignItem child in content.CollectionElements)
                    {
                        foreach (DesignItem child2 in DescendantsAndSelf(child))
                        {
                            yield return child2;
                        }
                    }
                }
                else
                {
                    if (content.Value != null)
                    {
                        foreach (DesignItem child2 in DescendantsAndSelf(content.Value))
                        {
                            yield return child2;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// 过滤元素列表，删除不属于xaml文档的所有元素(例如，因为它们被删除了)
        /// </summary>
        /// <param name="items"></param>
        /// <returns></returns>
        static List<DesignItem> GetLiveElements(ICollection<DesignItem> items)
        {
            List<DesignItem> result = new(items.Count);
            foreach (DesignItem item in items)
            {
                if (ModelTools.IsInDocument(item) && ModelTools.CanSelectComponent(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }

        T GetService<T>() where T : class
        {
            if (DesignContext != null)
                return DesignContext.Services.GetService<T>();
            else
                return null;
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
