using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class JsonDataSourceDialogViewModel : BindableBase, IDialogAware
    {

        #region Property

        public string Title => "参数设置";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        /// <summary>
        /// JSON
        /// </summary>
        public string Json
        {
            get => _json;
            set => SetProperty(ref _json, value);
        }
        private string _json = string.Empty;

        #endregion

        /// <summary>
        /// 确认命令
        /// </summary>
        public ICommand ConfirmCommand { get; private set; }

        private IDialogParameters _parameters;

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _parameters = parameters;

            Json = _parameters.GetValue<string>("Json");

        }

        public JsonDataSourceDialogViewModel()
        {
            // 确认命令
            ConfirmCommand = new DelegateCommand<object>(o =>
            {
                // 窗口传参
                _parameters = new DialogParameters { { "Json", Json } };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, _parameters));
            });
        }
    }
}
