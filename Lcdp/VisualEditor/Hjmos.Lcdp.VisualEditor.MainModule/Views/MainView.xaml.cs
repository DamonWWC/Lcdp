using Hjmos.Lcdp.VisualEditor.Core.Managers;
using Prism.Ioc;
using System.Windows.Controls;

namespace Hjmos.Lcdp.VisualEditor.MainModule.Views
{
    public partial class MainView : UserControl
    {
        public MainView()
        {
            InitializeComponent();

            // 在全局保留当前设计界面的引用
            ContainerLocator.Current.Resolve<StateManager>().CurrentDesignSurface = designSurface;
        }
    }
}
