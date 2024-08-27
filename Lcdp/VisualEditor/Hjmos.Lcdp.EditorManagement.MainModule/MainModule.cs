using Hjmos.Lcdp.EditorManagement.MainModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Hjmos.Lcdp.EditorManagement.MainModule
{
    public class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("LeftMenuTreeRegion", typeof(TreeMenuView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<TreeMenuView>();
            containerRegistry.RegisterForNavigation<MainMgtView>();
        }
    }
}