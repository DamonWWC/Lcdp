using System;

namespace Hjmos.Lcdp.Exceptions
{
    /// <summary>
    /// 描述WebAPI异常
    /// </summary>
    public class ApiException : Exception
    {
        /// <summary>
        /// 异常状态码
        /// </summary>
        public int StatusCode { get; set; }

        /// <summary>
        /// 异常内容
        /// </summary>
        public string Content { get; set; }
    }
}
