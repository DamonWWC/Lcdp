using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using System.Linq;
using Unity;

namespace Hjmos.Lcdp.VisualEditor.Core.ViewModels
{
    /// <summary>
    /// 窗口内容基类
    /// </summary>
    public class PageViewModelBase : BindableBase, INavigationAware
    {
        #region Property

        /// <summary>
        /// 标题
        /// </summary>
        public string PageTitle { get; set; } = string.Empty;

        /// <summary>
        /// 指示是否允许关闭页签
        /// </summary>
        public bool IsCanClose { get; set; } = true;

        /// <summary>
        /// 页签Uri
        /// </summary>
        public string NavUri { get; set; }

        /// <summary>
        /// 搜索框文本
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }
        private string _searchText;

        #endregion

        #region Command

        /// <summary>
        /// 页签关闭命令
        /// </summary>
        public DelegateCommand<string> CloseCommand { get; private set; }

        /// <summary>
        /// 窗体刷新命令
        /// </summary>
        public DelegateCommand RefreshCommand { get; set; }

        #endregion

        public bool IsNavigationTarget(NavigationContext navigationContext) => true;

        public void OnNavigatedFrom(NavigationContext navigationContext) { }

        public void OnNavigatedTo(NavigationContext navigationContext) => NavUri = navigationContext.Uri.ToString();


        private readonly IEventAggregator _ea;

        public PageViewModelBase(IRegionManager regionManager, IUnityContainer unityContainer, IEventAggregator ea) : this(regionManager, unityContainer)
        {
            _ea = ea;
        }

        public PageViewModelBase(IRegionManager regionManager, IUnityContainer unityContainer)
        {
            // 页签关闭命令
            CloseCommand = new DelegateCommand<string>(param =>
            {
                var obj = unityContainer.Registrations.FirstOrDefault(v => v.Name == NavUri);
                string name = obj.MappedToType.Name;
                if (string.IsNullOrEmpty(name)) return;
                IRegion region = regionManager.Regions["MainMgtContentRegion"];
                object view = region.Views.FirstOrDefault(v => v.GetType().Name == name);
                if (view != null) region.Remove(view);
            });

            // 窗体刷新命令
            RefreshCommand = new DelegateCommand(Refresh);
        }

        // 窗体刷新虚方法
        public virtual void Refresh() { }
    }
}
