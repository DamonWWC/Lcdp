using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Hjmos.Lcdp.VisualEditor.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Unity;

namespace Hjmos.Lcdp.EditorManagement.PageModule.ViewModels
{
    public class InterfaceManagementViewModel : PageViewModelBase
    {
        #region Property

        /// <summary>
        /// 接口列表
        /// </summary>
        public ObservableCollection<InterfaceModel> Interfaces { get; set; } = new ObservableCollection<InterfaceModel>();

        #endregion

        #region Command

        /// <summary>
        /// 添加接口命令
        /// </summary>
        public ICommand AddCommand { get; private set; }

        #endregion

        private readonly IUnityContainer _unityContainer;
        private readonly IDialogService _dialogService;

        public InterfaceManagementViewModel(
            IRegionManager regionManager,
            IUnityContainer unityContainer,
            IEventAggregator ea,
            IDialogService dialogService
        ) : base(regionManager, unityContainer, ea)
        {
            _unityContainer = unityContainer;
            _dialogService = dialogService;
            PageTitle = "接口管理";

            Interfaces.Clear();

            // 加载接口列表
            Task.Run(new Action(() =>
            {
                var libs = new List<InterfaceModel>() { new InterfaceModel { Index = "1", Name = "接口1", Address = "demo/basic_1" }, new InterfaceModel { Index = "2", Name = "接口2", Address = "demo/basic_2" } };
                _unityContainer.Resolve<Dispatcher>().Invoke(() =>
                {
                    libs.ToList().ForEach(f =>
                    {
                        Interfaces.Add(f);
                    });
                });
            }));

            // 打开上传组件窗口命令
            AddCommand = new DelegateCommand(() =>
            {
                _dialogService.ShowDialog("AddInterfaceDialog");
            });
        }
    }
}
