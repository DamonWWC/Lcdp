using Hjmos.Lcdp.VisualEditorServer.Entities;
using System.Collections.Generic;

namespace Hjmos.Lcdp.VisualEditorServer.IService
{
    public interface IMenuService : IBaseService
    {
        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <returns></returns>
        List<Menu> GetMenusByUserId(int userId);
    }
}
