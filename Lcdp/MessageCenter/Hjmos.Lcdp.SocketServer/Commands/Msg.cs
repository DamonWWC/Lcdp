using Hjmos.Lcdp.SocketServer.AppSession;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System.Linq;

namespace Hjmos.Lcdp.SocketServer.Commands
{
    public class Msg : CommandBase<VisualEditorSession, StringRequestInfo>
    {
        public override void ExecuteCommand(VisualEditorSession session, StringRequestInfo requestInfo)
        {
            if (requestInfo.Parameters != null && requestInfo.Parameters.Count() == 2)
            {
                // 需要找到消息接收方ID 
                string toId = requestInfo.Parameters[0];
                string msg = requestInfo.Parameters[1];
                VisualEditorSession toSession = session.AppServer.GetAllSessions().FirstOrDefault(s => s.User.UserId.ToString() == toId);
                toSession.Send($"{msg}");
            }
        }
    }
}
