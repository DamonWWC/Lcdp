using Hjmos.Lcdp.VisualEditor.IService;
using Hjmos.Lcdp.VisualEditor.Service.Apis;
using Hjmos.Lcdp.VisualEditorServer.Entities;
using Hjmos.Lcdp.VisualEditorServer.Entities.DTO;
using Refit;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.VisualEditor.Service
{
    public class LoginService : ILoginService
    {
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        public async Task<User> Login(string userName, string password)
        {
            var result = await RestService.For<ILoginApi>(RestConfig.BaseUrl).Login(new LoginDto() { Name = userName, Password = password });

            if (result.Code == 200 && result.Data != null)
            {
                return result.Data;
            }
            return null;
        }
    }
}
