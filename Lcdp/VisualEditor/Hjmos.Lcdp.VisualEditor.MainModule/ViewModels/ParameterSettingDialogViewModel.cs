using Hjmos.Lcdp.VisualEditor.Core.ViewModels;
using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Models;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class ParameterSettingDialogViewModel : ViewModelBase, IDialogAware
    {
        #region Property

        public string Title => "参数设置";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }


        /// <summary>
        /// 变量列表
        /// </summary>
        public ObservableCollection<ParameterModel> ParameterList
        {
            get => _parameterList ??= PageApi.ParameterList;
            set => SetProperty(ref _parameterList, value);
        }
        private ObservableCollection<ParameterModel> _parameterList;

        #endregion

        /// <summary>
        /// 添加参数命令
        /// </summary>
        public ICommand AddCommand { get; private set; }

        public void OnDialogOpened(IDialogParameters parameters) { }

        private readonly IDialogService _dialogService;
        private readonly IPageService _pageService;

        public ParameterSettingDialogViewModel(IDialogService dialogService, IPageService pageService)
        {
            _dialogService = dialogService;
            _pageService = pageService;

            // 打开上传组件窗口命令
            AddCommand = new DelegateCommand(() =>
            {
                if (PageApi.AppId <= 0)
                {
                    MessageBox.Show("请先打开一个模板");
                    return;
                }
                _dialogService.ShowDialog("AddParameterDialog", null, d =>
                {
                    // 刷新
                    LoadParameters();
                });
            });

        }

        private async void LoadParameters()
        {
            List<ParameterModel> parameters = await _pageService.LoadParameter(PageApi.AppId);
            PageApi.ParameterList.Clear();
            foreach (ParameterModel item in parameters)
            {

                item.DeleteCommand = new DelegateCommand<ParameterModel>(async p =>
                    {
                        bool result = await _pageService.Delete(p);
                        if (result)
                        {
                            // 关闭窗体，刷新父目录
                            LoadParameters();
                        }
                    });


                PageApi.ParameterList.Add(item);
            }
        }
    }
}