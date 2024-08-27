using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Models;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;

namespace Hjmos.Lcdp.EditorManagement.PageModule.ViewModels
{
    public class AddDirectoryDialogViewModel : BindableBase, IDialogAware
    {
        #region Property

        public string Title => "新建应用";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        /// <summary>
        /// 应用名称
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
        /// 确认新建应用命令
        /// </summary>
        public ICommand ConfirmCommand { get; private set; }

        #endregion

        private IDialogParameters _parameters;

        public void OnDialogOpened(IDialogParameters parameters) => _parameters = parameters;

        private readonly IModuleService _moduleService;

        public AddDirectoryDialogViewModel(IModuleService moduleService)
        {
            _moduleService = moduleService;

            // 确认新建应用命令
            ConfirmCommand = new DelegateCommand<object>(async o =>
            {
                FileModel parent = _parameters.GetValue<FileModel>("CurrentPath");
                int id = parent is null ? 0 : parent.Id;

                bool result = await _moduleService.AddDirectory(id, FileName);
                if (result)
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
            });
        }
    }
}
