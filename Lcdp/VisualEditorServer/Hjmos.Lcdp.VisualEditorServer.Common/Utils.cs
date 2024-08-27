using Hjmos.Lcdp.VisualEditorServer.ICommon;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Hjmos.Lcdp.VisualEditorServer.Common
{
    /// <summary>
    /// 帮助类
    /// </summary>
    public class Utils : IUtils
    {
        /// <summary>
        /// 获取字符串MD5
        /// </summary>
        /// <param name="strInput"></param>
        /// <returns></returns>
        public string GetMD5Str(string strInput)
        {
            if (string.IsNullOrEmpty(strInput)) return string.Empty;

            byte[] result = Encoding.Default.GetBytes(strInput);
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] output = md5.ComputeHash(result);
            return BitConverter.ToString(output).Replace("-", string.Empty);
        }
    }
}
