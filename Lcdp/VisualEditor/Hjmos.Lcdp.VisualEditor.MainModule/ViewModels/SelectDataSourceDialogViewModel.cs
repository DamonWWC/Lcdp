using Hjmos.Lcdp.VisualEditor.Models;
using Hjmos.Lcdp.VisualEditorServer.Entities.DTO;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Unity;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class SelectDataSourceDialogViewModel : BindableBase, IDialogAware
    {
        #region Property

        public string Title => "选择数据源";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        /// <summary>
        /// 选中的项
        /// </summary>
        public InterfaceModel SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }
        private InterfaceModel _selectedItem;

        /// <summary>
        /// 接口列表
        /// </summary>
        public ObservableCollection<InterfaceModel> Interfaces { get; set; } = new ObservableCollection<InterfaceModel>();

        #endregion

        #region Command

        /// <summary>
        /// 确认选择命令
        /// </summary>
        public ICommand ConfirmCommand { get; private set; }

        #endregion

        private IDialogParameters _parameters;

        public void OnDialogOpened(IDialogParameters parameters) => _parameters = parameters;

        private readonly IUnityContainer _unityContainer;

        public SelectDataSourceDialogViewModel(IUnityContainer unityContainer)
        {
            _unityContainer = unityContainer;

            Interfaces.Clear();

            // 加载接口列表
            Task.Run(new Action(() =>
            {
                // TODO：从数据库读取
                var libs = new List<InterfaceModel>() { new InterfaceModel { Index = "1", Name = "接口1", Address = "demo/basic_1" }, new InterfaceModel { Index = "2", Name = "接口2", Address = "demo/basic_2" } };
                _unityContainer.Resolve<Dispatcher>().Invoke(() =>
                {
                    libs.ToList().ForEach(f => Interfaces.Add(f));
                });
            }));

            // 确认选择命令
            ConfirmCommand = new DelegateCommand<object>(o =>
            {
                InterfaceDTO datasource = new InterfaceDTO
                {
                    Id = SelectedItem.Id,
                    Index = SelectedItem.Index,
                    Name = SelectedItem.Name,
                    Address = SelectedItem.Address
                };

                // 窗口传参
                _parameters = new DialogParameters { { "DataSource", datasource } };

                // TODO：关闭窗体，刷新父目录
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, _parameters));

            });
        }
    }
}
