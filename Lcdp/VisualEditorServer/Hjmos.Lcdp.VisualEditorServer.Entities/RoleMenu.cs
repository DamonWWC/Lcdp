namespace Hjmos.Lcdp.VisualEditorServer.Entities
{
    /// <summary>
    /// 用户与角色对应关系
    /// </summary>
    public class RoleMenu
    {
        public int Id { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        public int MenuId { get; set; }
    }
}
