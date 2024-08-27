using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.Core.Services
{
    #region ISelectionService

    /// <summary>
    /// 定义如何更改选择的类型。
    /// </summary>
    [Flags]
    public enum SelectionTypes
    {
        /// <summary>没有指定选择类型</summary>
        None = 0,

        /// <summary>使用当前按下的修改键自动确定选择类型</summary>
        Auto = 1,

        /// <summary>只更改主要选择</summary>
        Primary = 2,

        /// <summary>切换选择</summary>
        Toggle = 4,

        /// <summary>添加到选择项中</summary>
        Add = 8,

        /// <summary>从选择中删除</summary>
        Remove = 0x10,

        /// <summary>替换选择</summary>
        Replace = 0x20
    }

    /// <summary>
    /// 管理选择组件。
    /// </summary>
    public interface ISelectionService
    {
        /// <summary>在当前选择将要更改时发生</summary>
        event EventHandler SelectionChanging;

        /// <summary>当前选择更改后发生</summary>
        event EventHandler<DesignItemCollectionEventArgs> SelectionChanged;

        /// <summary>在主选择将要更改时发生</summary>
        event EventHandler PrimarySelectionChanging;

        /// <summary>在主选择更改后发生</summary>
        event EventHandler PrimarySelectionChanged;

        /// <summary>获取指定组件是否已选中</summary>
        bool IsComponentSelected(DesignItem component);

        /// <summary>
        /// 获取选定组件的集合。
        /// 这是实际选择的组件集合的副本，该集合的返回副本将不会反映该选择以后的更改。  
        /// </summary>
        ICollection<DesignItem> SelectedItems { get; }

        /// <summary>
        /// 将当前所选内容替换为指定的内容
        /// </summary>
        void SetSelectedComponents(ICollection<DesignItem> components);

        /// <summary>
        /// 使用指定的组件和selectionType修改当前选择。
        /// </summary>
        void SetSelectedComponents(ICollection<DesignItem> components, SelectionTypes selectionType);

        /// <summary>
        /// 获取当前是的主选择的对象
        /// </summary>
        DesignItem PrimarySelection { get; }

        /// <summary>
        /// 获取选中对象的数量
        /// </summary>
        int SelectionCount { get; }
    }
    #endregion

    #region IComponentService
    /// <summary>
    /// 支持添加和删除组件
    /// </summary>
    public interface IComponentService
    {
        /// <summary>
        /// Gets the site of an existing, registered component.
        /// </summary>
        /// <returns>
        /// The site of the component, or null if the component is not registered.
        /// </returns>
        DesignItem GetDesignItem(object component);

        /// <summary>
        /// Gets the site of an existing, registered component.
        /// </summary>
        /// <returns>
        /// The site of the component, or null if the component is not registered.
        /// </returns>
        DesignItem GetDesignItem(object component, bool findByView);

        /// <summary>Registers a component for usage in the designer.</summary>
        DesignItem RegisterComponentForDesigner(object component);

        /// <summary>Registers a component for usage in the designer.</summary>
        DesignItem RegisterComponentForDesigner(DesignItem parent, object component);

        /// <summary>Registers a component for usage in the designer.</summary>
        DesignItem RegisterComponentForDesignerRecursiveUsingXaml(object component);

        /// <summary>Called when a component is registered and added to a container.</summary>
        event EventHandler<DesignItemEventArgs> ComponentRegisteredAndAddedToContainer;

        /// <summary>Event raised whenever a component is registered</summary>
        event EventHandler<DesignItemEventArgs> ComponentRegistered;

        /// <summary>Event raised whenever a component is removed</summary>
        event EventHandler<DesignItemEventArgs> ComponentRemoved;

        /// <summary>Property Changed</summary>
        event EventHandler<DesignItemPropertyChangedEventArgs> PropertyChanged;

        /// <summary> Set's default Property Values as defined in Metadata </summary>
        void SetDefaultPropertyValues(DesignItem designItem);
    }
    #endregion

    #region IViewService
    /// <summary>
    /// Service for getting the view for a model or the model for a view.
    /// </summary>
    public abstract class ViewService
    {
        /// <summary>
        /// Gets the model represented by the specified view element.
        /// </summary>
        public abstract DesignItem GetModel(DependencyObject view);

        /// <summary>
        /// Gets the view for the specified model item.
        /// This is equivalent to using <c>model.View</c>.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public DependencyObject GetView(DesignItem model)
        {
            if (model == null)
                throw new ArgumentNullException("model");
            return model.View;
        }
    }
    #endregion

    #region IComponentPropertyService
    /// <summary>
    /// Used to get properties for a Design Item.
    /// </summary>
    public interface IComponentPropertyService
    {
        /// <summary>
        /// Get all Properties for a DesignItem
        /// </summary>
        /// <param name="designItem"></param>
        /// <returns></returns>
        IEnumerable<MemberDescriptor> GetAvailableProperties(DesignItem designItem);

        /// <summary>
        /// Get all possible Events for a DesignItem
        /// </summary>
        /// <param name="designItem"></param>
        /// <returns></returns>
        IEnumerable<MemberDescriptor> GetAvailableEvents(DesignItem designItem);

        /// <summary>
        /// Get all Properties for multiple Design Items 
        /// </summary>
        /// <param name="designItems"></param>
        /// <returns></returns>
        IEnumerable<MemberDescriptor> GetCommonAvailableProperties(IEnumerable<DesignItem> designItems);
    }
    #endregion

    #region IPropertyDescriptionService
    /// <summary>
    /// Used to get a description for properties.
    /// </summary>
    public interface IPropertyDescriptionService
    {
        /// <summary>
        /// Gets a WPF object representing a graphical description of the property.
        /// </summary>
        object GetDescription(DesignItemProperty designProperty);
    }
    #endregion

    #region IOutlineNodeNameService
    /// <summary>
    /// Used to get a description for the Outline Node.
    /// </summary>
    public interface IOutlineNodeNameService
    {
        /// <summary>
        /// Gets a the Name for display in the Ouline Node.
        /// </summary>
        string GetOutlineNodeName(DesignItem designItem);
    }
    #endregion

    #region IErrorService
    /// <summary>
    /// Service for showing error UI.
    /// </summary>
    public interface IErrorService
    {
        /// <summary>
        /// Shows an error tool tip.
        /// </summary>
        void ShowErrorTooltip(FrameworkElement attachTo, UIElement errorElement);
    }
    #endregion

    #region IEventHandlerService
    /// <summary>
    /// Service for providing the designer with information about available event handlers.
    /// </summary>
    public interface IEventHandlerService
    {
        /// <summary>
        /// Creates an event handler for the specified event.
        /// </summary>
        void CreateEventHandler(DesignItemProperty eventProperty);

        /// <summary>
        /// Gets the default event of the specified design item.
        /// </summary>
        DesignItemProperty GetDefaultEvent(DesignItem item);
    }
    #endregion

    #region ITopLevelWindowService
    /// <summary>
    /// Represents a top level window.
    /// </summary>
    public interface ITopLevelWindow
    {
        /// <summary>
        /// Sets child.Owner to the top level window.
        /// </summary>
        void SetOwner(Window child);

        /// <summary>
        /// Activates the window.
        /// </summary>
        bool Activate();
    }

    /// <summary>
    /// Provides a method to get the top-level-window of any UIElement.
    /// If the WPF Designer is hosted inside a Windows.Forms application, the hosting environment
    /// should specify a ITopLevelWindowService implementation that works with <b>both</b> WPF and Windows.Forms
    /// top-level-windows.
    /// </summary>
    public interface ITopLevelWindowService
    {
        /// <summary>
        /// Gets the top level window that contains the specified element.
        /// </summary>
        ITopLevelWindow GetTopLevelWindow(UIElement element);
    }
    #endregion

    #region IKeyBindingService

    /// <summary>
    /// 处理设计器中所有键绑定的服务。
    /// </summary>
    public interface IKeyBindingService
    {
        /// <summary>
        /// 获取要应用绑定的对象
        /// </summary>
        object Owner { get; }

        /// <summary>
        /// 注册绑定
        /// </summary>
        /// <param name="binding">要应用的绑定</param>
        void RegisterBinding(KeyBinding binding);

        /// <summary>
        /// 取消绑定
        /// </summary>
        /// <param name="binding">要应用的绑定</param>
        void DeregisterBinding(KeyBinding binding);

        /// <summary>
        /// 获取对应手势的绑定，否则返回null。
        /// </summary>
        /// <param name="gesture">The keyboard gesture requested.</param>
        KeyBinding GetBinding(KeyGesture gesture);
    }

    #endregion
}
