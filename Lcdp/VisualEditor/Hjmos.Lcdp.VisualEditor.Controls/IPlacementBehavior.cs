using System.Collections.Generic;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 容器元素实现的行为接口，以支持调整子元素的大小。
    /// </summary>
    public interface IPlacementBehavior
    {
        /// <summary>
        /// 获取子元素是否可以调整大小。
        /// </summary>
        bool CanPlace(IEnumerable<DesignItem> childItems, PlacementType type, PlacementAlignment position);

        /// <summary>
        /// 启动此容器的放置模式。
        /// </summary>
        void BeginPlacement(PlacementOperation operation);

        /// <summary>
        /// 结束该容器的放置模式。
        /// </summary>
        void EndPlacement(PlacementOperation operation);

        /// <summary>
        /// 获取子项的初始位置。
        /// </summary>
        Rect GetPosition(PlacementOperation operation, DesignItem child);

        /// <summary>
        /// 在为放置的项调用SetPosition之前调用。
        /// 这可能会更新放置操作的边界(例如，当对线启用时)。
        /// </summary>
        void BeforeSetPosition(PlacementOperation operation);

        /// <summary>
        /// 更新放置操作中指定的元素的位置。
        /// </summary>
        void SetPosition(PlacementInformation info);

        /// <summary>
        /// 获取是否允许为指定操作离开此容器。
        /// </summary>
        bool CanLeaveContainer(PlacementOperation operation);

        /// <summary>
        /// 从这个容器中移除已放置的子容器。
        /// </summary>
        void LeaveContainer(PlacementOperation operation);

        /// <summary>
        /// 获取是否允许指定操作进入此容器。
        /// </summary>
        bool CanEnterContainer(PlacementOperation operation, bool shouldAlwaysEnter);

        /// <summary>
        /// 让放置的子项进入这个容器。
        /// </summary>
        void EnterContainer(PlacementOperation operation);

        /// <summary>
        /// 位置坐标
        /// </summary>
        Point PlacePoint(Point point);
    }
}
