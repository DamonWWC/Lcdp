using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System.Collections.Generic;
using System.Windows.Input;

namespace Hjmos.Lcdp.EditorManagement.MainModule.Models
{
    public class MenuItemModel : BindableBase
    {
        #region Property

        /// <summary>
        /// 菜单图标
        /// </summary>
        public string MenuIcon { get; set; }

        /// <summary>
        /// 菜单标题
        /// </summary>
        public string MenuHeader { get; set; }

        /// <summary>
        /// 菜单关联的视图
        /// </summary>
        public string TargetView { get; set; }

        /// <summary>
        /// 是否展开
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }
        private bool _isExpanded;

        /// <summary>
        /// 子菜单列表
        /// </summary>
        public List<MenuItemModel> Children { get; set; }

        #endregion

        #region Command

        /// <summary>
        /// 打开菜单关联的视图命令
        /// </summary>
        public ICommand OpenViewCommand
        {
            get
            {
                if (_openViewCommand == null)
                    _openViewCommand = new DelegateCommand<MenuItemModel>(model =>
                    {
                        if ((model.Children == null || model.Children.Count == 0) && !string.IsNullOrEmpty(model.TargetView))
                            _regionManager.RequestNavigate("MainMgtContentRegion", model.TargetView);
                        else
                            IsExpanded = !IsExpanded;
                    });
                return _openViewCommand;
            }
        }
        private ICommand _openViewCommand;

        #endregion


        private readonly IRegionManager _regionManager;

        public MenuItemModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
    }
}
