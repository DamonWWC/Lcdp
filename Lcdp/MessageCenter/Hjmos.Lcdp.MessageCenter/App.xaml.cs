using Hjmos.Lcdp.MessageCenter.Views;
using Prism.Ioc;
using System.Windows;

namespace Hjmos.Lcdp.MessageCenter
{

    public partial class App
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {

        }
    }
}
