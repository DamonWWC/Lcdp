namespace Hjmos.Lcdp.VisualEditorServer.Entities
{
    public class Role
    {
        public int Id { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 状态标记（0：删除，1：有效）
        /// </summary>
        public int Status { get; set; }
    }
}
