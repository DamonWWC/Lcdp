using Hjmos.Lcdp.VisualEditor.Core.Events;
using Prism.Events;
using System.Windows;

namespace Hjmos.Lcdp.VisualEditor.Views
{
    public partial class LoginView : Window
    {
        public LoginView(IEventAggregator ea)
        {
            InitializeComponent();

            // 订阅ViewModel中的窗口关闭事件
            ea.GetEvent<CloseWindowMessageEvent>().Subscribe(MessageReceived, ThreadOption.UIThread);
        }

        private void MessageReceived() => DialogResult = false;
    }
}