using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Media;

namespace Hjmos.Lcdp.VisualEditor.Core.Adorners
{
    /// <summary>
    /// 设计面板上的装饰面板集合
    /// </summary>
    internal sealed class AdornerPanelCollection : ICollection<AdornerPanel>, IReadOnlyCollection<AdornerPanel>
    {
        /// <summary>装饰层</summary>
        private readonly AdornerLayer _layer;

        public AdornerPanelCollection(AdornerLayer layer) => _layer = layer;

        /// <summary>装饰层子项数量（子项就是当前为可见状态的装饰器）</summary>
        public int Count => _layer.Children.Count;

        public bool IsReadOnly => false;

        public void Add(AdornerPanel item)
        {
            if (item == null) throw new ArgumentNullException("item");

            _layer.AddAdorner(item);
        }

        public void Clear() => _layer.ClearAdorners();

        public bool Contains(AdornerPanel item) => item == null ? throw new ArgumentNullException("item") : VisualTreeHelper.GetParent(item) == _layer;

        public void CopyTo(AdornerPanel[] array, int arrayIndex)
        {
            foreach (AdornerPanel panel in this)
                array[arrayIndex++] = panel;
        }

        public bool Remove(AdornerPanel item)
        {
            if (item == null) throw new ArgumentNullException("item");

            return _layer.RemoveAdorner(item);
        }

        /// <summary>
        /// 迭代集合
        /// </summary>
        /// <returns></returns>
        public IEnumerator<AdornerPanel> GetEnumerator()
        {
            foreach (AdornerPanel panel in _layer.Children)
            {
                yield return panel;
            }
        }

        /// <summary>
        /// 重写迭代器
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
