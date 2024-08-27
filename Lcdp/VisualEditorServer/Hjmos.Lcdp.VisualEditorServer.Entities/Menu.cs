namespace Hjmos.Lcdp.VisualEditorServer.Entities
{
    public class Menu
    {
        public int Id { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// 关联视图
        /// </summary>
        public string TargetView { get; set; }

        /// <summary>
        /// 父ID
        /// </summary>
        public int ParentId { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int Sort { get; set; }

        /// <summary>
        /// 菜单类型（0：叶子结点，1：分支结点）
        /// </summary>
        public int MenuType { get; set; }

        /// <summary>
        /// 状态标记（0：删除，1：有效）
        /// </summary>
        public int Status { get; set; }
    }
}
