using Hjmos.Lcdp.EditorManagement.PageModule.Views;
using Prism.Ioc;
using Prism.Modularity;
using System.Windows;
using System.Windows.Threading;

namespace Hjmos.Lcdp.EditorManagement.PageModule
{
    public class PageModule : IModule
    {
        public void OnInitialized(IContainerProvider containerProvider)
        {
 
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<Dispatcher>(() => Application.Current.Dispatcher);

            containerRegistry.RegisterForNavigation<ModuleDirectoryView>();
            containerRegistry.RegisterForNavigation<WidgetUploadView>();
            containerRegistry.RegisterForNavigation<InterfaceManagementView>();
            containerRegistry.RegisterDialog<AddLibDialog>();
            containerRegistry.RegisterDialog<AddPageDialog>();
            containerRegistry.RegisterDialog<AddDirectoryDialog>();
            containerRegistry.RegisterDialog<WidgetListDialog>();
            containerRegistry.RegisterDialog<ModifyLibConfigDialog>();
            containerRegistry.RegisterDialog<WorkflowView>();
        }
    }
}