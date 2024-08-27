using Hjmos.Lcdp.VisualEditorServer.ICommon;
using Hjmos.Lcdp.VisualEditorServer.IService;

namespace Hjmos.Lcdp.VisualEditorServer.Service
{
    public class LoginService : BaseService, ILoginService
    {
        public LoginService(IConnectionFactory contextFactory) : base(contextFactory) { }
    }
}
