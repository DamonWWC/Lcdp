using Hjmos.Lcdp.VisualEditor.Core.Events;
using Prism.Events;
using Prism.Services.Dialogs;
using SuperSocket.ClientEngine;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Unity;

namespace Hjmos.Lcdp.VisualPlayer.Views
{
    public partial class MainWindow : Window
    {
        private readonly IEventAggregator _ea;
        private readonly IDialogService _dialogService;
        private readonly IUnityContainer _unityContainer;

        public MainWindow(IEventAggregator ea, IDialogService dialogService, IUnityContainer unityContainer)
        {
            _ea = ea;
            _dialogService = dialogService;
            _unityContainer = unityContainer;

            InitializeComponent();
            //Communication();
        }

        private AsyncTcpSession asyncTcpsession;

        public void Communication()
        {
            _ea.GetEvent<MessageCenterEvent>().Subscribe(o => { }, ThreadOption.UIThread);

            Task.Run(() =>
            {
                string strIp = "127.0.0.1";
                string strPort = "3757";

                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(strIp), Convert.ToInt32(strPort));
                asyncTcpsession = new AsyncTcpSession();
                asyncTcpsession.Connect(endPoint);
                // 登录
                string userId = "2";
                string userName = "EndPoint";
                ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Login {userId} {userName}\r\n"));
                asyncTcpsession.Send(buffer);

                asyncTcpsession.DataReceived += (o, e) =>
                {
                    // 接收消息
                    string msg = Encoding.Default.GetString(e.Data, 0, e.Data.Length);


                    if (msg.Contains("yjzy"))
                    {
                        // 广播消息
                        _ea.GetEvent<MessageCenterEvent>().Publish("yjzy");
                    }

                    if (msg.Contains("jcyj"))
                    {
                        // 广播消息
                        _ea.GetEvent<MessageCenterEvent>().Publish("jcyj");
                    }

                    if (msg.Contains("fire"))
                    {

                        // 弹窗
                        _unityContainer.Resolve<Dispatcher>().Invoke(() =>
                        {
                            _dialogService.ShowDialog("DemoDialog");
                        });
                    }
                };
            });
        }
    }
}
