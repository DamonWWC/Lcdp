using Hjmos.Lcdp.Exceptions;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Hjmos.Lcdp.Toolkits
{
    /// <summary>
    /// WebApi操作帮助类
    /// </summary>
    public sealed class WebApiHelper
    {
        private static readonly HttpClient _httpClient;

        private WebApiHelper() { }

        static WebApiHelper()
        {
            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri("http://101.33.246.96:5000/v1/"),

                // 默认30秒超时
                Timeout = new TimeSpan(0, 0, 30)
            };

            _httpClient.DefaultRequestHeaders.Clear();
            // 通知服务端保持长连接
            _httpClient.DefaultRequestHeaders.Connection.Add("keep-alive");
            _httpClient.DefaultRequestHeaders.Add("keep-alive", "timeout=600");
        }

        #region GET POST PUT DELETE

        /// <summary>
        /// 异步Get请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static async Task<string> GetAsync(string url, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            using (var response = await _httpClient.SendAsync(request, cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();
                var content = await StreamHelper.StreamToStringAsync(stream);
                if (response.IsSuccessStatusCode)
                    return content;
                else
                {
                    throw new ApiException
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = content
                    };
                }
            }
        }

        /// <summary>
        /// 泛型异步Get请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(HttpMethod.Get, url))
            using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
            {
                var stream = await response.Content.ReadAsStreamAsync();

                if (response.IsSuccessStatusCode)
                    return StreamHelper.DeserializeEntityFromStream<T>(stream);

                string content = await StreamHelper.StreamToStringAsync(stream);
                throw new ApiException
                {
                    StatusCode = (int)response.StatusCode,
                    Content = content
                };
            }
        }

        /// <summary>
        /// 异步Post请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="model">请求体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url, object model = default, CancellationToken cancellationToken = default) => await RequestAsync(url, model, HttpMethod.Post, cancellationToken);

        /// <summary>
        /// 异步泛型Post请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="model">请求体</param>
        /// <param name="method">请求方式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string url, object model = default, CancellationToken cancellationToken = default) => await RequestAsync<T>(url, model, HttpMethod.Post, cancellationToken);

        /// <summary>
        /// 异步Put请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="model">请求体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static async Task<string> PutAsync(string url, object model = default, CancellationToken cancellationToken = default) => await RequestAsync(url, model, HttpMethod.Put, cancellationToken);

        /// <summary>
        /// 异步泛型Put请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="model">请求体</param>
        /// <param name="method">请求方式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static async Task<T> PutAsync<T>(string url, object model = default, CancellationToken cancellationToken = default) => await RequestAsync<T>(url, model, HttpMethod.Put, cancellationToken);

        /// <summary>
        /// 异步Delete请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="model">请求体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static async Task<string> DeleteAsync(string url, object model = default, CancellationToken cancellationToken = default) => await RequestAsync(url, model, HttpMethod.Delete, cancellationToken);

        /// <summary>
        /// 异步泛型Delete请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="model">请求体</param>
        /// <param name="method">请求方式</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        public static async Task<T> DeleteAsync<T>(string url, object model = default, CancellationToken cancellationToken = default) => await RequestAsync<T>(url, model, HttpMethod.Delete, cancellationToken);

        #endregion

        #region Request

        /// <summary>
        /// 异步请求
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="model">请求体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        private static async Task<string> RequestAsync(string url, object model, HttpMethod method, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(method, url))
            using (var httpContent = CreateHttpContent(model))
            {
                request.Content = httpContent;

                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    Stream stream = await response.Content.ReadAsStreamAsync();

                    string content = await StreamHelper.StreamToStringAsync(stream);
                    if (response.IsSuccessStatusCode)
                        return content;
                    else
                    {
                        throw new ApiException
                        {
                            StatusCode = (int)response.StatusCode,
                            Content = content
                        };
                    }
                }
            }
        }


        /// <summary>
        /// 异步泛型请求
        /// </summary>
        /// <typeparam name="T">返回值类型</typeparam>
        /// <param name="url">请求地址</param>
        /// <param name="model">请求体</param>
        /// <param name="cancellationToken">取消令牌</param>
        /// <returns></returns>
        private static async Task<T> RequestAsync<T>(string url, object model, HttpMethod method, CancellationToken cancellationToken = default)
        {
            using (var request = new HttpRequestMessage(method, url))
            using (var httpContent = CreateHttpContent(model))
            {
                request.Content = httpContent;

                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    Stream stream = await response.Content.ReadAsStreamAsync();

                    if (response.IsSuccessStatusCode)
                        return StreamHelper.DeserializeEntityFromStream<T>(stream);

                    string content = await StreamHelper.StreamToStringAsync(stream);
                    throw new ApiException
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = content
                    };
                }
            }
        }

        #endregion

        #region Upload the File

        /// <summary>
        /// 异步上传文件
        /// </summary>
        /// <param name="url"></param>
        /// <param name="model">Post请求体</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<T> UploadFileAsync<T>(string url, string fileName, string path, object model, CancellationToken cancellationToken = default(CancellationToken))
        {

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            using (var httpContent = new MultipartFormDataContent())
            {
                FileStream fileStream = new FileStream(path, FileMode.Open);
                httpContent.Add(new StreamContent(fileStream, (int)fileStream.Length), "file", fileName);
                httpContent.Add(new StringContent(JsonConvert.SerializeObject(model)), "model");

                request.Content = httpContent;

                using (var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken))
                {
                    Stream stream = await response.Content.ReadAsStreamAsync();

                    if (response.IsSuccessStatusCode)
                        return StreamHelper.DeserializeEntityFromStream<T>(stream);

                    string content = await StreamHelper.StreamToStringAsync(stream);
                    throw new ApiException
                    {
                        StatusCode = (int)response.StatusCode,
                        Content = content
                    };
                }
            }
        }

        #endregion

        #region Other Methods

        private static HttpContent CreateHttpContent(object content)
        {
            HttpContent httpContent = null;

            if (content != null)
            {
                MemoryStream ms = new MemoryStream();
                StreamHelper.SerializeEntityIntoStream(content, ms);
                ms.Seek(0, SeekOrigin.Begin);
                httpContent = new StreamContent(ms);
                httpContent.Headers.ContentType = new MediaTypeHeaderValue("application/json;charset=unicode");
            }

            return httpContent;
        }

        #endregion

    }
}
