namespace Hjmos.Lcdp.VisualEditorServer.Entities.Core
{
    /// <summary>
    /// HTTP响应体
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Result<T>
    {
        public int Code { get; set; }
        public string Msg { get; set; }

        public T Data { get; set; }

        public Result() { }

        private Result(T data)
        {
            this.Code = CodeMsg.Success.Code;
            this.Msg = CodeMsg.Success.Msg;
            this.Data = data;
        }

        private Result(CodeMsg codeMsg)
        {
            if (codeMsg != null)
            {
                this.Code = codeMsg.Code;
                this.Msg = codeMsg.Msg;
            }
        }

        /// <summary>
        /// 访问成功时调用
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static Result<T> Success(T data)
        {
            return new Result<T>(data);
        }

        /// <summary>
        /// 访问失败时调用
        /// </summary>
        /// <param name="codeMsg"></param>
        /// <returns></returns>
        public static Result<T> Error(CodeMsg codeMsg)
        {
            return new Result<T>(codeMsg);
        }

        /// <summary>
        /// 访问失败时调用
        /// </summary>
        /// <param name="codeMsg"></param>
        /// <returns></returns>
        public static Result<T> Error(int code, string msg, T data = default(T))
        {
            return new Result<T>() { Code = code, Msg = msg, Data = data };
        }
    }
}
