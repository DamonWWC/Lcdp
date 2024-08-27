using SuperSocket.ClientEngine;
using System;
using System.Net;
using System.Text;
using System.Windows;

namespace Hjmos.Lcdp.MessageCenter.Views
{

    public partial class MainWindow : Window
    {
        private AsyncTcpSession asyncTcpsession;

        public MainWindow()
        {
            InitializeComponent();

            string strIp = "127.0.0.1";
            string strPort = "3757";

            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(strIp), Convert.ToInt32(strPort));
            asyncTcpsession = new AsyncTcpSession();
            asyncTcpsession.Connect(endPoint);

            string userId = "1";
            string userName = "Controller";
            ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Login {userId} {userName}\r\n"));
            asyncTcpsession.Send(buffer);

        }

        // 弹窗
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string toId = "2";
            string msg = "fire";
            ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Msg {toId} {msg}\r\n"));
            asyncTcpsession.Send(buffer);
        }

        // 切换监测预警
        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            string toId = "2";
            string msg = "jcyj";
            ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Msg {toId} {msg}\r\n"));
            asyncTcpsession.Send(buffer);
            //_client.Send(Encoding.UTF8.GetBytes($"9999:yjjk"));
        }

        // 切换到应急资源
        private void Button3_Click(object sender, RoutedEventArgs e)
        {
            string toId = "2";
            string msg = "yjzy";
            ArraySegment<byte> buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes($"Msg {toId} {msg}\r\n"));
            asyncTcpsession.Send(buffer);
            //_client.Send(Encoding.UTF8.GetBytes($"9999:czlc"));
        }

    }
}
