using Hjmos.Lcdp.VisualEditorServer.ICommon;
using Hjmos.Lcdp.VisualEditorServer.IService;
using System.Collections.Generic;
using System.Linq;
using Hjmos.Lcdp.VisualEditorServer.Entities;

namespace Hjmos.Lcdp.VisualEditorServer.Service
{
    public class MenuService : BaseService, IMenuService
    {
        public MenuService(IConnectionFactory contextFactory) : base(contextFactory) { }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        public List<Menu> GetMenusByUserId(int userId)
        {
            // 获取所有角色
            var roles = (from ur in Context.Set<UserRole>()
                         join role in Context.Set<Role>() on ur.RoleId equals role.Id
                         where ur.UserId == userId && role.Status == 1
                         select ur.RoleId).ToList();

            // 获取所有菜单权限
            var menus = from menu in Context.Set<Menu>()
                        join role_menu in Context.Set<RoleMenu>()
                        on menu.Id equals role_menu.MenuId
                        where roles.Contains(role_menu.RoleId) && menu.Status == 1
                        select menu;

            return menus.ToList();
        }
    }
}
