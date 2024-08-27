using Hjmos.Lcdp.VisualEditorServer.Entities;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using Hjmos.Lcdp.VisualEditorServer.Entities.DTO;
using Hjmos.Lcdp.VisualEditorServer.ICommon;
using Hjmos.Lcdp.VisualEditorServer.IService;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Linq;

namespace Hjmos.Lcdp.VisualEditorServer.WebAPI.Controllers
{
    /// <summary>
    /// 登录
    /// </summary>
    [Route("v1/login")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v1")]
    public class LoginController : Controller
    {

        private readonly ILoginService _loginService;
        private readonly IMenuService _menuService;
        private readonly IUtils _utils;

        public LoginController(ILoginService loginService, IUtils utils, IMenuService menuService)
        {
            _loginService = loginService;
            _menuService = menuService;
            _utils = utils;
        }

        [HttpPost]
        [SwaggerResponse(200, Type = typeof(Result<User>))]
        public IActionResult Index(LoginDto userInfo)
        {

            string userName = userInfo.Name;
            string password = userInfo.Password;

            string password_md5 = _utils.GetMD5Str(_utils.GetMD5Str(password) + "|" + userName);

            var users = _loginService.Query<User>(u => u.Name == userName && u.Password == password_md5);

            if (users?.Count() > 0)
            {
                User user = users.First();

                // 获取用户菜单
                user.Menus = _menuService.GetMenusByUserId(user.Id);

                return Ok(Result<User>.Success(user));
            }
            else
            {
                return Ok(Result<User>.Error(CodeMsg.BadRequest));
            }
        }

    }
}
