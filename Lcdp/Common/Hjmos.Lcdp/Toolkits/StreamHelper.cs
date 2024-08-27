using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.Toolkits
{
    /// <summary>
    /// Stream帮助类
    /// </summary>
    public class StreamHelper
    {
        /// <summary>
        /// 将实体对象写入到流
        /// </summary>
        /// <param name="value"></param>
        /// <param name="stream"></param>
        public static void SerializeEntityIntoStream(object value, Stream stream)
        {
            using (StreamWriter streamWriter = new StreamWriter(stream, new UTF8Encoding(false), 1024, true))
            using (JsonTextWriter jsonTextWriter = new JsonTextWriter(streamWriter) { Formatting = Formatting.None })
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Serialize(jsonTextWriter, value);
                jsonTextWriter.Flush();
            }
        }

        /// <summary>
        /// 将流转换为实体对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static T DeserializeEntityFromStream<T>(Stream stream)
        {
            if (stream == null || stream.CanRead == false) return default;

            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonTextReader jsonTextReader = new JsonTextReader(streamReader))
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                T result = jsonSerializer.Deserialize<T>(jsonTextReader);
                return result;
            }
        }

        /// <summary>
        /// 将流转换成字符串
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static async Task<string> StreamToStringAsync(Stream stream)
        {
            string content = string.Empty;

            if (stream != null)
                using (StreamReader sr = new StreamReader(stream))
                    content = await sr.ReadToEndAsync();

            return content;
        }
    }
}
