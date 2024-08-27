using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;

namespace Hjmos.Lcdp.EditorManagement.PageModule.ViewModels
{
    public class AddPageDialogViewModel : BindableBase, IDialogAware
    {
        #region Property

        public string Title => "新建页面";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        /// <summary>
        /// 页面名称
        /// </summary>
        public string FileName
        {
            get => _fileName;
            set => SetProperty(ref _fileName, value);
        }
        private string _fileName;

        #endregion

        #region Command

        /// <summary>
        /// 确认新建页面命令
        /// </summary>
        public ICommand ConfirmCommand { get; private set; }

        #endregion


        private IDialogParameters _parameters;

        public void OnDialogOpened(IDialogParameters parameters) => _parameters = parameters;

        private readonly IModuleService _moduleService;

        public AddPageDialogViewModel(IModuleService moduleService)
        {
            _moduleService = moduleService;

            // 确认新建页面命令
            ConfirmCommand = new DelegateCommand<object>(async o =>
            {
                bool result = await _moduleService.AddPage(_parameters.GetValue<FileModel>("CurrentPath").Id, FileName);
                if (result)
                {
                    // TODO：关闭窗体，刷新父目录
                }
            });
        }
    }
}
