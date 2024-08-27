using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Input;
using Unity;

namespace Hjmos.Lcdp.EditorManagement.PageModule.ViewModels
{
    public class WorkflowViewModel : PageViewModelBase
    {

        public ICommand StartCommand { get; private set; }

        public ICommand FireCommand { get; private set; }

        public ICommand PopCommand { get; private set; }

        public ICommand EndCommand { get; private set; }

        public ObservableCollection<KeyValuePair<string, string>> EventList { get; set; } = new ObservableCollection<KeyValuePair<string, string>>();

        public WorkflowViewModel(
             IRegionManager regionManager,
             IUnityContainer unityContainer,
             IEventAggregator ea
         ) : base(regionManager, unityContainer, ea)
        {
            PageTitle = "应急处置流程";
            StartCommand = new DelegateCommand<ListBox>(list =>
            {
                EventList.Clear();
            });

            EndCommand = new DelegateCommand<ListBox>(list =>
            {
                EventList.Clear();
            });


            FireCommand = new DelegateCommand<ListBox>(list =>
            {
                EventList.Clear();

                EventList.Add(new KeyValuePair<string, string>("step 1：切换到应急指挥界面", "step 1：切换到应急指挥界面"));

                list.SelectedIndex = 0;
            });


            PopCommand = new DelegateCommand<ListBox>(list =>
            {
                EventList.Clear();

                EventList.Add(new KeyValuePair<string, string>("step 1：弹出视频窗口", "step 1：弹出视频窗口"));

                list.SelectedIndex = 0;
            });

        }
    }
}
