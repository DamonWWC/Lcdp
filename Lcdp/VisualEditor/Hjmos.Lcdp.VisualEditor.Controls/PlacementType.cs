namespace Hjmos.Lcdp.VisualEditor.Controls
{
    /// <summary>
    /// 描述如何完成放置。
    /// </summary>
    public sealed class PlacementType
    {
        /// <summary>
        /// 放置是通过移动内部点来完成的(例如在路径上，直线上，…)
        /// </summary>
        public static readonly PlacementType MovePoint = Register("MovePoint");

        /// <summary>
        /// 放置是通过在拖放操作中调整元素的大小来完成的。
        /// </summary>
        public static readonly PlacementType Resize = Register("Resize");

        /// <summary>
        /// 放置是通过拖放操作中移动一个元素来完成的。
        /// </summary>
        public static readonly PlacementType Move = Register("Move");

        /// <summary>
        /// 放置是通过移动元素来完成的，例如通过键盘!
        /// </summary>
        public static readonly PlacementType MoveAndIgnoreOtherContainers = Register("MoveAndIgnoreOtherContainers");

        /// <summary>
		/// 将元素添加到容器的指定位置。
		/// 将工具箱项拖动到设计图面时使用AddItem。
        /// </summary>

        public static readonly PlacementType AddItem = Register("AddItem");

        /// <summary>
        /// 不是“真正的”放置，而是删除元素。
        /// </summary>
        public static readonly PlacementType Delete = Register("Delete");

        /// <summary>
        /// 从Cliboard插入
        /// </summary>
        public static readonly PlacementType PasteItem = Register("PasteItem");

        /// <summary>
        /// 通过DragDropFromOutside插入
        /// </summary>
        public static readonly PlacementType DragDropFromOutside = Register("DragDropFromOutside");

        private readonly string _name;

        private PlacementType(string name) => _name = name;

        /// <summary>
        /// 创建一个新的唯一的PlacementKind。
        /// </summary>
        /// <param name="name">从ToString()调用返回的名称。注意，两个相同名称的PlacementTypes是不相等的!</param>
        public static PlacementType Register(string name) => new(name);

        /// <summary>
        /// 获取用于注册此PlacementType的名称。
        /// </summary>
        public override string ToString() => _name;
    }
}
