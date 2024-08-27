namespace Hjmos.Lcdp.VisualEditorServer.Entities.DTO
{
    /// <summary>
    /// 登录DTO
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; } = "admin";

        /// <summary>
        /// 登录密码
        /// </summary>
        public string Password { get; set; }
    }
}
