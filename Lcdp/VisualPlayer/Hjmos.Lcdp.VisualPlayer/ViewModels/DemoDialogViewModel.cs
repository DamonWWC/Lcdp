using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;

namespace Hjmos.Lcdp.VisualPlayer.ViewModels
{
    public class DemoDialogViewModel : BindableBase, IDialogAware
    {
        #region Property

        public string Title => "示例弹窗";

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;

        public void OnDialogClosed() { }


        #endregion

        public DemoDialogViewModel() { }

        private IDialogParameters _parameters;

        public void OnDialogOpened(IDialogParameters parameters) => _parameters = parameters;
    }
}
