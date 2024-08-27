using Hjmos.Lcdp.ILoger;
using Hjmos.Lcdp.VisualEditor.Core.Managers;
using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditorServer.Entities;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System.Windows;
using System.Windows.Input;

namespace Hjmos.Lcdp.VisualEditor.ViewModels
{
    public class LoginViewModel : BindableBase
    {
        #region Properties

        /// <summary>
        /// 指示正在登陆
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }
        bool _isLoading;

        /// <summary>
        /// 正在登陆提示信息
        /// </summary>
        public string LoadingMessage
        {
            get => _loadingMessage;
            set => SetProperty(ref _loadingMessage, value);
        }
        string _loadingMessage;

        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName
        {
            get => _userName;
            set => SetProperty(ref _userName, value);
        }
        string _userName;

        /// <summary>
        /// 密码
        /// </summary>
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        string _password;

        /// <summary>
        /// 登陆异常信息
        /// </summary>
        public string ErrorMsg
        {
            get => _errorMsg;
            set => SetProperty(ref _errorMsg, value);
        }
        string _errorMsg;

        #endregion

        /// <summary>
        /// 登陆命令
        /// </summary>
        public ICommand LoginCommand { get; private set; }

        private readonly ILoginService _loginService;
        private readonly ILogHelper _logHelper;

        public LoginViewModel(ILoginService loginService, ILogHelper logHelper)
        {
            _loginService = loginService;
            _logHelper = logHelper;

            // 登陆命令
            LoginCommand = new DelegateCommand<object>(OnLogin);

            UserName = "Admin";
            Password = "123456";
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="obj"></param>
        private async void OnLogin(object obj)
        {
            ErrorMsg = string.Empty;

            if (string.IsNullOrEmpty(UserName))
            {
                ErrorMsg = "请输入用户名";
                return;
            }

            if (string.IsNullOrEmpty(Password))
            {
                ErrorMsg = "请输入密码";
                return;
            }

            IsLoading = true;
            LoadingMessage = "正在登录";

            // 获取登陆信息
            User user = await _loginService.Login(UserName, Password);

            var state = ContainerLocator.Current.Resolve<StateManager>();

            if (user != null)
            {
                _logHelper.Info(this, "登陆成功");
                // 保存用户信息到全局
                state.CurrentUser = user;

                (obj as Window).DialogResult = true;
            }
            else
            {
                ErrorMsg = "用户名或密码错误，请重新输入";
                IsLoading = false;
            }
        }
    }
}