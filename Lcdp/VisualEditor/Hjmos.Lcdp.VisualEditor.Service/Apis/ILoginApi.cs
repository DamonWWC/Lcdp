using Hjmos.Lcdp.VisualEditorServer.Entities;
using Hjmos.Lcdp.VisualEditorServer.Entities.Core;
using Hjmos.Lcdp.VisualEditorServer.Entities.DTO;
using Refit;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.VisualEditor.Service.Apis
{
    /// <summary>
    /// 登录接口
    /// </summary>
    public interface ILoginApi
	{
		/// <summary>
		/// 用户登录
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[Post("/login")]
		Task<Result<User>> Login(LoginDto dto);
	}
}
