namespace Hjmos.Lcdp.VisualEditorServer.Entities.Core
{
    /// <summary>
    /// HTTP状态码
    /// </summary>
    public class CodeMsg
    {
        public int Code { get; private set; }
        public string Msg { get; private set; }


        // 请求成功
        public static CodeMsg Success = new CodeMsg(200, "Success");
        
        // 请求验证失败
        public static CodeMsg BadRequest = new CodeMsg(400, "Bad Request");

        // 未通过身份验证
        public static CodeMsg Unauthorized = new CodeMsg(401, "Unauthorized");

        // 已通过身份验证，但不允许访问资源
        public static CodeMsg Forbidden = new CodeMsg(403, "Forbidden");

        // 没有找到资源
        public static CodeMsg NotFound = new CodeMsg(405, "Not Found");

        // 服务器内部错误
        public static CodeMsg Error = new CodeMsg(500, "Error");

        // 网关路由异常，表示访问不到上游服务器
        public static CodeMsg BadGateway = new CodeMsg(502, "Bad Gateway");

        // 服务不可用：服务器过载、系统某些功能故障等
        public static CodeMsg ServiceUnavailable = new CodeMsg(503, "Service Unavailable");



        private CodeMsg() { }

        public CodeMsg(int code, string msg)
        {
            this.Code = code;
            this.Msg = msg;
        }

        public override string ToString()
        {
            return "CodeMsg [Code=" + Code + ", Msg=" + Msg + "]";
        }

    }
}
