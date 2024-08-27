using Hjmos.Lcdp.VisualEditor.Core.Events;
using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace Hjmos.Lcdp.VisualEditor.ViewModels
{
    public class NonClientAreaContentViewModel : ViewModelBase
    {
        /// <summary>
        /// 选项卡索引
        /// </summary>
        public int CheckedIndex
        {
            get => _checkedIndex;
            set => SetProperty(ref _checkedIndex, value);
        }
        private int _checkedIndex = 1;


        private readonly IRegionManager _regionManager;

        public DelegateCommand<string> NavigateCommand { get; private set; }

        public NonClientAreaContentViewModel(IRegionManager regionManager, IEventAggregator ea)
        {
            _regionManager = regionManager;

            // 默认显示编辑器
            _regionManager.RequestNavigate("ContentRegion", "EditorView");

            NavigateCommand = new DelegateCommand<string>(Navigate);

            // 显示编辑器
            ea.GetEvent<SwitchPageEvent>().Subscribe(p => { CheckedIndex = 1; });
        }

        private void Navigate(string navigatePath)
        {
            if (navigatePath != null)
            {
                _regionManager.RequestNavigate("ContentRegion", navigatePath);
            }
        }
    }
}