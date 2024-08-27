using Hjmos.Common;
using Prism.Commands;
using Prism.Services.Dialogs;
using System.Windows;

namespace Hjmos.Lcdp.Plugins.NccControls.ViewModels.EventHandling
{
    public class MessageBoxViewModel : ViewModelBase
    {
        public MessageBoxViewModel()
        {

        }

        public override void OnDialogOpened(IDialogParameters parameters)
        {
            base.OnDialogOpened(parameters);
            if (parameters.TryGetValue("Content", out string content))
                Content = content;

            if (parameters.TryGetValue("Tips", out string tips))
                Tips = tips;

            if (parameters.TryGetValue("WindowType", out MessageBoxButton windowType))
            {
                //WindowType = windowType;
                switch (windowType)
                {
                    case MessageBoxButton.OK:
                        OKVisibility = Visibility.Visible;
                        break;
                    case MessageBoxButton.OKCancel:
                        OKVisibility = Visibility.Visible;
                        CancelVisibility = Visibility.Visible;
                        break;
                    default:
                        OKVisibility = Visibility.Visible;
                        break;
                }
            }
            else
            {
                OKVisibility = Visibility.Visible;
            }
        }

        //private MessageBoxButton _WindowType;
        ///// <summary>
        ///// 弹窗类型
        ///// </summary>
        //public MessageBoxButton WindowType
        //{
        //    get { return _WindowType; }
        //    set { SetProperty(ref _WindowType, value); }
        //}

        private Visibility _OKVisibility = Visibility.Collapsed;
        /// <summary>
        /// 确定按钮是否可见
        /// </summary>
        public Visibility OKVisibility
        {
            get { return _OKVisibility; }
            set { SetProperty(ref _OKVisibility, value); }
        }

        private Visibility _CancelVisibility = Visibility.Collapsed;
        /// <summary>
        /// 取消按钮是否可见
        /// </summary>
        public Visibility CancelVisibility
        {
            get { return _CancelVisibility; }
            set { SetProperty(ref _CancelVisibility, value); }
        }

        private string _Content;
        /// <summary>
        /// 内容
        /// </summary>
        public string Content
        {
            get { return _Content; }
            set { SetProperty(ref _Content, value); }
        }

        private string _Tips;
        /// <summary>
        /// 提示
        /// </summary>
        public string Tips
        {
            get { return _Tips; }
            set { SetProperty(ref _Tips, value); }
        }

        private DelegateCommand _ConfirmCommand;
        /// <summary>
        /// 确定命令
        /// </summary>
        public DelegateCommand ConfirmCommand =>
            _ConfirmCommand ?? (_ConfirmCommand = new DelegateCommand(ExecuteConfirmCommand));

        void ExecuteConfirmCommand()
        {
            CloseWindow(new DialogResult(ButtonResult.OK));
        }

        private DelegateCommand _CancelCommand;
        /// <summary>
        /// 取消命令
        /// </summary>
        public DelegateCommand CancelCommand =>
            _CancelCommand ?? (_CancelCommand = new DelegateCommand(ExecuteCancelCommand));

        void ExecuteCancelCommand()
        {
            CloseWindow(new DialogResult(ButtonResult.Cancel));
        }
    }
}
