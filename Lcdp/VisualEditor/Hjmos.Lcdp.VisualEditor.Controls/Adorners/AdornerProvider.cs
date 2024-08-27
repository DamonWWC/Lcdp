using Hjmos.Lcdp.VisualEditor.Controls.Extensions;
using System.Collections.ObjectModel;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls.Adorners
{
    /// <summary>
    /// 用于在屏幕上显示装饰器的扩展的基类。
    /// </summary>
    /// <remarks>
    /// Cider装饰器简介:https://docs.microsoft.com/en-us/archive/blogs/jnak/cider-adorners
    /// <quote>
    ///     装饰器是漂浮在设计界面上的WPF元素，并跟踪它们所装饰的元素的大小和位置。
    ///     设计器呈现给用户的所有UI，包括吸附线条、抓取手柄和网格标尺，都是由这些装饰器组成的。
    /// </quote>
    /// 关于设计时装饰器及其放置:
    /// read http://myfun.spaces.live.com/blog/cns!AC1291870308F748!240.entry
    /// and http://myfun.spaces.live.com/blog/cns!AC1291870308F748!242.entry
    /// 上面两个链接能找到博客，但是好像没有相关的文章了
    /// </remarks>
    public abstract class AdornerProvider : DefaultExtension
    {

        /// <summary>装饰器是否可见</summary>
        private bool _isVisible;

        protected AdornerProvider() => Adorners = new AdornerPanelCollectionAdapter(this);

        /// <summary>
        /// 在设置ExtendedItem之后调用，此方法显示已注册的装饰器
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();
            _isVisible = true;
            foreach (AdornerPanel adornerPanel in Adorners)
            {
                OnAdornerAdd(adornerPanel);
            }
        }

        /// <summary>
        /// 在删除扩展时调用，此方法隐藏已注册的装饰器。
        /// </summary>
        protected override void OnRemove()
        {
            base.OnRemove();
            foreach (AdornerPanel v in Adorners)
            {
                OnAdornerRemove(v);
            }
            _isVisible = false;
        }

        /// <summary>
        /// 获取此装饰器提供者显示的装饰器列表。
        /// </summary>
        public Collection<AdornerPanel> Adorners { get; }

        /// <summary>
        /// 用指定的放置信息添加一个UIElement元素作为装饰器
        /// </summary>
        protected void AddAdorner(AdornerPlacement placement, AdornerOrder order, UIElement adorner)
        {
            AdornerPanel p = new() { Order = order };
            AdornerPanel.SetPlacement(adorner, placement);
            p.Children.Add(adorner);
            this.Adorners.Add(p);
        }

        /// <summary>
        /// 用指定的放置添加几个UIElement作为装饰器。
        /// </summary>
        protected void AddAdorners(AdornerPlacement placement, params UIElement[] adorners)
        {
            AdornerPanel adornerPanel = new();
            foreach (UIElement adorner in adorners)
            {
                AdornerPanel.SetPlacement(adorner, placement);
                adornerPanel.Children.Add(adorner);
            }
            this.Adorners.Add(adornerPanel);
        }

        /// <summary>
        /// 为装饰面板设置被装饰的对象，并添加到设计面板中
        /// </summary>
        /// <param name="adornerPanel"></param>
        internal void OnAdornerAdd(AdornerPanel adornerPanel)
        {
            if (!_isVisible) return;

            // 设置装饰面板要装饰的对象
            adornerPanel.SetAdornedElement(this.ExtendedItem.View, this.ExtendedItem);
            // 从服务中获取设计面板实例
            IDesignPanel designPanel = Services.GetService<IDesignPanel>();
            // 把装饰面板添加到设计面板在装饰器集合中
            designPanel.Adorners.Add(adornerPanel);
        }

        /// <summary>
        /// 从设计面板中移除装饰面板
        /// </summary>
        /// <param name="item"></param>
        internal void OnAdornerRemove(AdornerPanel item)
        {
            if (!_isVisible) return;

            IDesignPanel designPanel = Services.GetService<IDesignPanel>();
            designPanel.Adorners.Remove(item);
        }
    }
}
