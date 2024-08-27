using Hjmos.Lcdp.VisualEditor.Controls.Adorners;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Controls.Extensions
{
    /// <summary>
    /// 对鼠标悬停时的组件应用扩展
    /// </summary>
    public class MouseOverExtensionServer : DefaultExtensionServer
    {
        private DesignItem _lastItem = null;

        /// <summary>
        /// 在扩展服务初始化并设置了Context属性之后调用
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            if (this.Services.GetService<IDesignPanel>() is FrameworkElement panel)
            {
                ((FrameworkElement)this.Services.DesignPanel).PreviewMouseMove += MouseOverExtensionServer_PreviewMouseMove;
                ((FrameworkElement)this.Services.DesignPanel).MouseLeave += MouseOverExtensionServer_MouseLeave;
                Services.Selection.SelectionChanged += OnSelectionChanged;
            }
        }

        private void OnSelectionChanged(object sender, DesignItemCollectionEventArgs e) => ReapplyExtensions(e.Items);

        private void MouseOverExtensionServer_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_lastItem != null)
            {
                DesignItem oldLastItem = _lastItem;
                _lastItem = null;
                ReapplyExtensions(new[] { oldLastItem });
            }
        }

        private void MouseOverExtensionServer_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            DesignItem element = null;
            VisualTreeHelper.HitTest((FrameworkElement)this.Services.DesignPanel,
                potentialHitTestTarget =>
                {
                    if (potentialHitTestTarget is IAdornerLayer)
                        return HitTestFilterBehavior.ContinueSkipSelfAndChildren;

                    if (Extension.GetDisableMouseOverExtensions(potentialHitTestTarget))
                        return HitTestFilterBehavior.ContinueSkipSelfAndChildren;

                    DesignItem item = this.Services.Component.GetDesignItem(potentialHitTestTarget);
                    if (item == null)
                        return HitTestFilterBehavior.ContinueSkipSelf;

                    if (element == null || item.Parent == element)
                    {
                        element = item;
                        return HitTestFilterBehavior.Continue;
                    }

                    DesignItem par = item.Parent;
                    while (par != null)
                    {
                        if (par.Parent == element)
                        {
                            element = item;
                            return HitTestFilterBehavior.Continue;
                        }
                        par = par.Parent;
                    }

                    return HitTestFilterBehavior.Stop;
                },
                result => HitTestResultBehavior.Stop,
                new PointHitTestParameters(e.GetPosition((FrameworkElement)this.Services.DesignPanel)));

            DesignItem oldLastItem = _lastItem;
            _lastItem = element;
            if (oldLastItem != null && oldLastItem != element)
                ReapplyExtensions(new[] { oldLastItem, element });
            else
            {
                ReapplyExtensions(new[] { element });
            }
        }

        /// <summary>
        /// 获取项是否被选中
        /// </summary>
        public override bool ShouldApplyExtensions(DesignItem extendedItem) => extendedItem == _lastItem && !Services.Selection.IsComponentSelected(extendedItem);
    }
}
