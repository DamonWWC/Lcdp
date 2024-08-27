using System;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Core
{
    /// <summary>
    /// 水平对齐/垂直对齐的组合
    /// </summary>
    public struct PlacementAlignment : IEquatable<PlacementAlignment>
    {

        /// <summary>获取水平组件</summary>
        public HorizontalAlignment Horizontal { get; }

        /// <summary>获取垂直组件</summary>
        public VerticalAlignment Vertical { get; }

        /// <summary>
        /// 创建PlacementAlignment结构的新实例
        /// </summary>
        public PlacementAlignment(HorizontalAlignment horizontal, VerticalAlignment vertical)
        {
            if (horizontal == HorizontalAlignment.Stretch)
                throw new ArgumentException("Strech is not a valid value", "horizontal");
            if (vertical == VerticalAlignment.Stretch)
                throw new ArgumentException("Strech is not a valid value", "vertical");

            this.Horizontal = horizontal;
            this.Vertical = vertical;
        }

        #region Equals and GetHashCode implementation
        /// <summary>Compares this to <paramref name="obj"/>.</summary>
        public override bool Equals(object obj)
        {
            if (obj is PlacementAlignment)
                return Equals((PlacementAlignment)obj); // use Equals method below
            else
                return false;
        }

        /// <summary>Compares this to <paramref name="other"/>.</summary>
        public bool Equals(PlacementAlignment other)
        {
            return this.Horizontal == other.Horizontal && this.Vertical == other.Vertical;
        }

        /// <summary>
        /// Gets the hash code.
        /// </summary>
        public override int GetHashCode()
        {
            unchecked
            {
                return Horizontal.GetHashCode() * 27 + Vertical.GetHashCode();
            }
        }

        /// <summary>Compares <paramref name="lhs"/> to <paramref name="rhs"/>.</summary>
        public static bool operator ==(PlacementAlignment lhs, PlacementAlignment rhs)
        {
            return lhs.Equals(rhs);
        }

        /// <summary>Compares <paramref name="lhs"/> to <paramref name="rhs"/>.</summary>
        public static bool operator !=(PlacementAlignment lhs, PlacementAlignment rhs)
        {
            return !(lhs.Equals(rhs));
        }
        #endregion


        /// <summary>Top left</summary>
        public static readonly PlacementAlignment TopLeft = new(HorizontalAlignment.Left, VerticalAlignment.Top);
        /// <summary>Top center</summary>
        public static readonly PlacementAlignment Top = new(HorizontalAlignment.Center, VerticalAlignment.Top);
        /// <summary>Top right</summary>
        public static readonly PlacementAlignment TopRight = new(HorizontalAlignment.Right, VerticalAlignment.Top);
        /// <summary>Center left</summary>
        public static readonly PlacementAlignment Left = new(HorizontalAlignment.Left, VerticalAlignment.Center);
        /// <summary>Center center</summary>
        public static readonly PlacementAlignment Center = new(HorizontalAlignment.Center, VerticalAlignment.Center);
        /// <summary>Center right</summary>
        public static readonly PlacementAlignment Right = new(HorizontalAlignment.Right, VerticalAlignment.Center);
        /// <summary>Bottom left</summary>
        public static readonly PlacementAlignment BottomLeft = new(HorizontalAlignment.Left, VerticalAlignment.Bottom);
        /// <summary>Bottom center</summary>
        public static readonly PlacementAlignment Bottom = new(HorizontalAlignment.Center, VerticalAlignment.Bottom);
        /// <summary>Bottom right</summary>
        public static readonly PlacementAlignment BottomRight = new(HorizontalAlignment.Right, VerticalAlignment.Bottom);
    }
}
