using Hjmos.Lcdp.VisualEditor.Core.Interfaces;
using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class AddParameterDialogViewModel : BindableBase, IDialogAware
    {

        #region Property

        public string Title => "参数设置";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        /// <summary>
        /// 参数
        /// </summary>
        public ParameterModel Parameter
        {
            get => _parameter;
            set => SetProperty(ref _parameter, value);
        }
        private ParameterModel _parameter = new() { Value = string.Empty };

        public IList<ParameterRange> RangeList
        {
            get => _rangeList;
            set => SetProperty(ref _rangeList, value);
        }
        private IList<ParameterRange> _rangeList = new List<ParameterRange>() { ParameterRange.App, ParameterRange.Page };

        #endregion

        /// <summary>
        /// 确认命令
        /// </summary>
        public ICommand ConfirmCommand { get; private set; }

        public void OnDialogOpened(IDialogParameters parameters) { }

        private readonly IPageService _pageService;

        ///平台页面API对象
        private readonly IPageApi _pageApi;

        public AddParameterDialogViewModel(IPageService templateService, IPageApi pageApi)
        {
            _pageService = templateService;
            _pageApi = pageApi;

            Parameter.AppId = _pageApi.AppId;

            // 确认命令
            ConfirmCommand = new DelegateCommand<object>(async o =>
            {
                bool result = await _pageService.Add(Parameter);
                if (result)
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
            });
        }
    }
}
