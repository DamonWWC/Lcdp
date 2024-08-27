using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.MainModule.ViewModels
{
    public class CustomXamlDialogViewModel : BindableBase, IDialogAware
    {

        #region Property

        public string Title => "参数设置";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }

        /// <summary>
        /// Xaml
        /// </summary>
        public string CustomXaml
        {
            get => _customXaml;
            set => SetProperty(ref _customXaml, value);
        }
        private string _customXaml = string.Empty;

        #endregion

        /// <summary>
        /// 确认命令
        /// </summary>
        public ICommand ConfirmCommand { get; private set; }

        private IDialogParameters _parameters;

        public void OnDialogOpened(IDialogParameters parameters)
        {
            _parameters = parameters;

            CustomXaml = _parameters.GetValue<string>("CustomXaml");
        }

        public CustomXamlDialogViewModel()
        {
            // 确认命令
            ConfirmCommand = new DelegateCommand<object>(o =>
            {
                // 窗口传参
                _parameters = new DialogParameters { { "CustomXaml", CustomXaml } };
                RequestClose?.Invoke(new DialogResult(ButtonResult.OK, _parameters));
            });
        }
    }
}
