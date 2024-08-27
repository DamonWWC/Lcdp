using System.Collections.ObjectModel;

namespace Hjmos.Lcdp.VisualEditor.Controls.Adorners
{
    /// <summary>
    /// 装饰器面板集合
    /// </summary>
    internal sealed class AdornerPanelCollectionAdapter : Collection<AdornerPanel>
    {
        private readonly AdornerProvider _provider;

        internal AdornerPanelCollectionAdapter(AdornerProvider provider) => _provider = provider;

        /// <summary>
        /// 添加装饰器面板
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void InsertItem(int index, AdornerPanel item)
        {
            base.InsertItem(index, item);
            _provider.OnAdornerAdd(item);
        }

        /// <summary>
        /// 删除装饰器面板
        /// </summary>
        /// <param name="index"></param>
        protected override void RemoveItem(int index)
        {
            _provider.OnAdornerRemove(base[index]);
            base.RemoveItem(index);
        }

        /// <summary>
        /// 替换装饰器面板
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        protected override void SetItem(int index, AdornerPanel item)
        {
            _provider.OnAdornerRemove(base[index]);
            base.SetItem(index, item);
            _provider.OnAdornerAdd(item);
        }

        /// <summary>
        /// 移除所有装饰器面板
        /// </summary>
        protected override void ClearItems()
        {
            foreach (AdornerPanel v in this)
            {
                _provider.OnAdornerRemove(v);
            }
            base.ClearItems();
        }
    }
}
