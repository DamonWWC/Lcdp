using Hjmos.Lcdp.VisualEditorServer.Entities;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.VisualEditor.IService
{
    public interface ILoginService
    {
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        Task<User> Login(string userName, string password);
    }
}
