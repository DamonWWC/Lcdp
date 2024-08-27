using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Unity;

namespace Hjmos.Lcdp.EditorManagement.PageModule.ViewModels
{
    public class WidgetListDialogViewModel : BindableBase, IDialogAware
    {
        #region Property

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string _title = "组件列表";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        /// <summary>
        /// 组件列表
        /// </summary>
        public ObservableCollection<WidgetModel> Widgets { get; set; } = new ObservableCollection<WidgetModel>();

        #endregion

        private readonly IUnityContainer _unityContainer;
        private readonly IWidgetService _widgetService;
        private readonly IDialogService _dialogService;

        public WidgetListDialogViewModel(IUnityContainer unityContainer, IWidgetService widgetService, IDialogService dialogService)
        {
            _unityContainer = unityContainer;
            _widgetService = widgetService;
            _dialogService = dialogService;
        }

        private IDialogParameters _parameters;
        public void OnDialogOpened(IDialogParameters parameters)
        {
            _parameters = parameters;

            if (_parameters.ContainsKey("Name"))
            {
                Title = $"组件列表[{_parameters.GetValue<string>("Name")}]";
            }

            if (!_parameters.ContainsKey("Id")) return;
            Widgets.Clear();

            // 加载组件类库列表
            Task.Run(new Action(async () =>
            {
                var widgets = await _widgetService.GetWidgets(_parameters.GetValue<int>("Id"));
                _unityContainer.Resolve<Dispatcher>().Invoke(() =>
                {
                    widgets.ToList().ForEach(f =>
                    {
                        f.ConfigCommand = new DelegateCommand<object>(o =>
                        {
                            DialogParameters param = new DialogParameters
                            {
                                { "Id", (o as WidgetModel).Id },
                                { "Name", (o as WidgetModel).Name }
                            };
                            _dialogService.ShowDialog("ModifyLibConfigDialog", param, new Action<IDialogResult>(result =>
                            {
                                if (result != null && result.Result == ButtonResult.OK)
                                {
                                    MessageBox.Show("配置完成", "提示");
                                }
                            }));
                        });
                        Widgets.Add(f);
                    });
                });
            }));

        }
    }
}
