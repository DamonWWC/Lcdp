namespace Hjmos.Lcdp.VisualEditorServer.Entities
{
    /// <summary>
    /// 用户与角色对应关系
    /// </summary>
    public class UserRole
    {
        public int Id { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 角色ID
        /// </summary>
        public int RoleId { get; set; }
    }
}
