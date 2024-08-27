using Hjmos.Lcdp.SocketServer.Models;
using SuperSocket.SocketBase;

namespace Hjmos.Lcdp.SocketServer.AppSession
{
    public class VisualEditorSession : AppSession<VisualEditorSession>
    {
        // 登录用户
        public User User { get; set; }

        // 是否已登录
        public bool IsLogin { get; set; } = false;
    }
}
