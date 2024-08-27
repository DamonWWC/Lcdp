using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditorServer.Entities
{
    public class User
    {
        public int Id { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 头像
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 年龄
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 状态标记（0：删除，1：有效）
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// 用户可见菜单列表
        /// </summary>
        public List<Menu> Menus { get; set; }
    }
}
