using Hjmos.Lcdp.VisualEditor.Core.Helpers;
using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// TODO：这个PageService和数据服务层的PageService重名了
    /// </summary>
    public static class PageService
    {
        /// <summary>
        /// TODO: 考虑改成Prism事件
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="pageParameters"></param>
        public static void InjectPageContext(DependencyObject obj, PageContext pageContext)
        {
            List<IWidget> children = obj.TryFindAllChildWidget<IWidget>().ToList();

            if (children == null) return;

            foreach (var child in children)
            {
                if ((child as FrameworkElement).DataContext is IPageAware viewModel)
                {
                    viewModel.OnPageLoaded(pageContext);
                }
            }
        }
    }
}
