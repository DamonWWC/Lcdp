using System;

namespace Hjmos.Lcdp.VisualEditor.Core.Adorners
{
    /// <summary>
    /// 描述装饰器在Z-Layer上的位置
    /// </summary>
    public struct AdornerOrder : IComparable<AdornerOrder>, IEquatable<AdornerOrder>
    {
        /// <summary>装饰器在背景层</summary>
        public static readonly AdornerOrder Background = new(100);

        /// <summary>装饰器在内容层</summary>
        public static readonly AdornerOrder Content = new(200);

        /// <summary>装饰者在前景的后面，但在内容的上面。这个图层用于灰色效果</summary>
        public static readonly AdornerOrder BehindForeground = new(280);

        /// <summary>装饰器在前景层</summary>
        public static readonly AdornerOrder Foreground = new(300);

        /// <summary>装饰者在前景层之前</summary>
        public static readonly AdornerOrder BeforeForeground = new(400);

        private readonly int _order;

        internal AdornerOrder(int _order) => this._order = _order;

        public override int GetHashCode() => _order.GetHashCode();

        public override bool Equals(object obj) => obj is AdornerOrder order && this == order;

        public bool Equals(AdornerOrder other) => _order == other._order;

        /// <summary><see cref="AdornerOrder"/>与另一个AdornerOrder比较</summary>
        public int CompareTo(AdornerOrder other) => _order.CompareTo(other._order);

        #region 运算符重载

        public static bool operator ==(AdornerOrder leftHandSide, AdornerOrder rightHandSide) => leftHandSide._order == rightHandSide._order;

        public static bool operator !=(AdornerOrder leftHandSide, AdornerOrder rightHandSide) => leftHandSide._order != rightHandSide._order;

        public static bool operator <(AdornerOrder leftHandSide, AdornerOrder rightHandSide) => leftHandSide._order < rightHandSide._order;

        public static bool operator <=(AdornerOrder leftHandSide, AdornerOrder rightHandSide) => leftHandSide._order <= rightHandSide._order;

        public static bool operator >(AdornerOrder leftHandSide, AdornerOrder rightHandSide) => leftHandSide._order > rightHandSide._order;

        public static bool operator >=(AdornerOrder leftHandSide, AdornerOrder rightHandSide) => leftHandSide._order >= rightHandSide._order;

        #endregion
    }
}
