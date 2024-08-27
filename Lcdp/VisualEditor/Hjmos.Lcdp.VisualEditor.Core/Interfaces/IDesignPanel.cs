using Hjmos.Lcdp.VisualEditor.Core.Adorners;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 设计面板的接口。设计面板是包含设计元素的UIElement，负责处理鼠标和键盘事件。  
    /// </summary>
    public interface IDesignPanel : IInputElement
    {
        /// <summary>
        /// 设置一个自定义的过滤器回调，这样可以过滤掉任何元素  
        /// </summary>
        HitTestFilterCallback CustomHitTestFilterBehavior { get; set; }

        /// <summary>
        /// DesignPanel使用的设计上下文
        /// </summary>
        DesignContext Context { get; }

        /// <summary>
        /// 设计内容对于命中测试是否可见
        /// </summary>
        bool IsContentHitTestVisible { get; set; }

        /// <summary>
        /// 装饰层对于命中测试是否可见  
        /// </summary>
        bool IsAdornerLayerHitTestVisible { get; set; }

        /// <summary>
        /// 获取显示在设计面板上的装饰器列表
        /// </summary>
        ICollection<AdornerPanel> Adorners { get; }

        /// <summary>
        /// 在设计界面上执行命中测试
        /// </summary>
        DesignPanelHitTestResult HitTest(Point mousePosition, bool testAdorners, bool testDesignSurface, HitTestType hitTestType);

        /// <summary>
        /// 在设计界面上执行命中测试，为每个匹配触发<paramref name="callback"/>。 当回调返回true时，命中测试继续进行。 
        /// </summary>
        void HitTest(Point mousePosition, bool testAdorners, bool testDesignSurface, Predicate<DesignPanelHitTestResult> callback, HitTestType hitTestType);

        // 以下成员在<see cref="IInputElement"/> 中缺失，但在DesignPanel中被支持:  

        /// <summary>
        /// Occurs when a mouse button is pressed.
        /// </summary>
        event MouseButtonEventHandler MouseDown;

        /// <summary>
        /// Occurs when a mouse button is released.
        /// </summary>
        event MouseButtonEventHandler MouseUp;

        /// <summary>
        /// Occurs when a drag operation enters the design panel.
        /// </summary>
        event DragEventHandler DragEnter;

        /// <summary>
        /// Occurs when a drag operation is over the design panel.
        /// </summary>
        event DragEventHandler DragOver;

        /// <summary>
        /// Occurs when a drag operation leaves the design panel.
        /// </summary>
        event DragEventHandler DragLeave;

        /// <summary>
        /// Occurs when an element is dropped on the design panel.
        /// </summary>
        event DragEventHandler Drop;
    }
}
