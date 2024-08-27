using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class LoadedCodeDialogViewModel : BindableBase, IDialogAware
    {

        #region Property

        public string Title => "参数设置";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        /// <summary>
        /// LoadedCode
        /// </summary>
        public string LoadedCode
        {
            get => _loadedCode;
            set => SetProperty(ref _loadedCode, value);
        }
        private string _loadedCode = string.Empty;

        #endregion

        /// <summary>
        /// 确认命令
        /// </summary>
        public ICommand ConfirmCommand { get; private set; }

        private IDialogParameters _parameters;

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _parameters = parameters;

            LoadedCode = _parameters.GetValue<string>("LoadedCode");

        }

        public LoadedCodeDialogViewModel()
        {
            // 确认命令
            ConfirmCommand = new DelegateCommand<object>(o =>
            {
                // 窗口传参
                _parameters = new DialogParameters { { "LoadedCode", LoadedCode } };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, _parameters));
            });
        }
    }
}
