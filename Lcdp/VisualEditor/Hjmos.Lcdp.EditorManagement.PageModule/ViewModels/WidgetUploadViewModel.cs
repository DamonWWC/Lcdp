using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Models;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using Unity;

namespace Hjmos.Lcdp.EditorManagement.PageModule.ViewModels
{
    public class WidgetUploadViewModel : PageViewModelBase
    {
        #region Property

        /// <summary>
        /// 组件类库列表
        /// </summary>
        public ObservableCollection<WidgetLibModel> Libs { get; set; } = new ObservableCollection<WidgetLibModel>();

        #endregion

        #region Command

        /// <summary>
        /// 打开上传组件窗口命令
        /// </summary>
        public ICommand UploadCommand { get; private set; }

        #endregion

        private readonly IUnityContainer _unityContainer;
        private readonly IWidgetService _widgetService;
        private readonly IDialogService _dialogService;

        public WidgetUploadViewModel(
            IRegionManager regionManager,
            IUnityContainer unityContainer,
            IEventAggregator ea,
            IWidgetService widgetService,
            IDialogService dialogService
        ) : base(regionManager, unityContainer, ea)
        {
            _unityContainer = unityContainer;
            _widgetService = widgetService;
            _dialogService = dialogService;
            PageTitle = "插件管理";

            Libs.Clear();

            // 加载组件类库列表
            Task.Run(new Action(async () =>
            {
                var libs = await _widgetService.GetLibs();
                _unityContainer.Resolve<Dispatcher>().Invoke(() =>
                {
                    libs.ToList().ForEach(f =>
                    {
                        f.ConfigCommand = new DelegateCommand<object>(o =>
                        {
                            DialogParameters param = new DialogParameters();
                            param.Add("Id", (o as WidgetLibModel).Id);
                            param.Add("Name", (o as WidgetLibModel).Name);
                            _dialogService.ShowDialog("WidgetListDialog", param, new Action<IDialogResult>(result =>
                            {
                                if (result != null && result.Result == ButtonResult.OK)
                                {
                                    MessageBox.Show("配置完成", "提示");
                                }
                            }));
                        });
                        Libs.Add(f);
                    });
                });
            }));

            // 打开上传组件窗口命令
            UploadCommand = new DelegateCommand(() => _dialogService.ShowDialog("AddLibDialog"));

        }
    }
}
