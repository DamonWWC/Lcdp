using System.Collections.Generic;
using System.Collections.Specialized;


namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 实现INotifyCollectionChanged的IList
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IObservableList<T> : IList<T>, INotifyCollectionChanged { }
}