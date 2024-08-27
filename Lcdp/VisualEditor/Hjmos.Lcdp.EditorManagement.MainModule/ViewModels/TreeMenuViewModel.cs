using Hjmos.Lcdp.EditorManagement.MainModule.Models;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Prism.Ioc;
using System.Collections.Generic;
using System.Linq;

namespace Hjmos.Lcdp.EditorManagement.MainModule.ViewModels
{

    public class TreeMenuViewModel : ViewModelBase
    {
        /// <summary>
        /// 菜单树
        /// </summary>
        public List<MenuItemModel> Menus { get; set; } = new List<MenuItemModel>();

        public TreeMenuViewModel()
        {
            // 生成菜单树
            FillMenus(Menus, 0);
        }

        /// <summary>
        /// 递归获取菜单树
        /// </summary>
        /// <param name="menus"></param>
        /// <param name="parentId"></param>
        private void FillMenus(List<MenuItemModel> menus, int parentId)
        {
            var sub = State.CurrentUser.Menus.Where(m => m.ParentId == parentId).OrderBy(o => o.Sort);

            if (sub.Any())
            {
                foreach (var item in sub)
                {
                    MenuItemModel model = ContainerLocator.Current.Resolve<MenuItemModel>();
                    model.MenuHeader = item.Header;
                    model.MenuIcon = item.Icon;
                    model.TargetView = item.TargetView;
                    menus.Add(model);

                    FillMenus(model.Children = new List<MenuItemModel>(), item.Id);
                }
            }
        }
    }
}
