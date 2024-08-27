using Hjmos.Lcdp.VisualEditor.Core.Managers;
using Hjmos.Lcdp.VisualEditor.MainModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace Hjmos.Lcdp.VisualEditor.MainModule
{
    public class MainModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
            IRegionManager regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("ToolbarRegion", typeof(ToolbarView));
            regionManager.RegisterViewWithRegion("LeftSideRegion", typeof(LeftSideView));
            regionManager.RegisterViewWithRegion("RightSideRegion", typeof(RightSideView));
            regionManager.RegisterViewWithRegion("MainRegion", typeof(MainView));
            regionManager.RegisterViewWithRegion("StatusBarRegion", typeof(StatusBarView));
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterDialog<SelectDataSourceDialog>();
            containerRegistry.RegisterDialog<ParameterSettingDialog>();
            containerRegistry.RegisterDialog<WidgetParamterDialog>();
            containerRegistry.RegisterDialog<AddParameterDialog>();
            containerRegistry.RegisterDialog<JsonDataSourceDialog>();
            containerRegistry.RegisterDialog<CustomXamlDialog>();
            containerRegistry.RegisterDialog<LoadedCodeDialog>();
            containerRegistry.RegisterForNavigation<EditorView>();
        }
    }
}