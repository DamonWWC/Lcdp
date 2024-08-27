using Hjmos.Lcdp.SocketServer.AppSession;
using SuperSocket.SocketBase;
using System;

namespace Hjmos.Lcdp.SocketServer.AppServer
{
    public class VisualEditorServer : AppServer<VisualEditorSession>
    {
        /// <summary>
        /// 有新的会话连接时触发
        /// </summary>
        /// <param name="session"></param>
        protected override void OnNewSessionConnected(VisualEditorSession session)
        {
            Console.WriteLine("新的会话连接成功");
            base.OnNewSessionConnected(session);
        }

        /// <summary>
        /// 会话关闭时触发
        /// </summary>
        /// <param name="session"></param>
        /// <param name="reason"></param>
        protected override void OnSessionClosed(VisualEditorSession session, CloseReason reason)
        {
            base.OnSessionClosed(session, reason);
        }

    }
}
