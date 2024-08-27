using Hjmos.Lcdp.SocketServer.AppSession;
using Hjmos.Lcdp.SocketServer.Models;
using SuperSocket.SocketBase.Command;
using SuperSocket.SocketBase.Protocol;
using System;
using System.Linq;

namespace Hjmos.Lcdp.SocketServer.Commands
{
    public class Login : CommandBase<VisualEditorSession, StringRequestInfo>
    {
        public override void ExecuteCommand(VisualEditorSession session, StringRequestInfo requestInfo)
        {
            if (requestInfo.Key == "Login" && requestInfo.Parameters != null && requestInfo.Parameters.Count() == 2)
            {
                // TODO：用户应该通过数据库查询
                User user = new User();
                if (int.TryParse(requestInfo.Parameters[0], out int userId))
                {
                    user.UserId = userId;
                }
                else
                {
                    session.Send($"UserID应为整型");
                }
                user.UserName = requestInfo.Parameters[1];
                session.User = user;
                session.IsLogin = true;
                Console.WriteLine($"【{session.User.UserName}】登录成功");
                session.Send($"【{session.User.UserName}】登录成功");
            }
        }
    }
}
