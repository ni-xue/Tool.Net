using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Utils
{
    /// <summary>
    /// 提供部分的API请求访问类 (内置调用接口 替换为 HttpClient)
    /// 注意此类下所有函数调用皆无异常抛出，但为了方便问题排查，增加异常相关日志
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class HttpHelpers
    {
        /// <summary>
        /// 相关请求异常日志输出位置
        /// </summary>
        public const string LogFilePath = "Log/HttpHelpers/";

        private static readonly HttpClient _HttpClient;

        /// <summary>
        /// Http请求处理程序
        /// </summary>
        public static HttpMessageHandler HttpHandler { get; set; }

        /// <summary>
        /// 默认编码格式
        /// </summary>
        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 等待毫秒
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">指定的超时值小于或等于零，并且不是系统超时值。穿线。超时。无穷大跨度。</exception>
        /// <exception cref="InvalidOperationException">已在当前实例上启动操作。</exception>
        /// <exception cref="ObjectDisposedException">当前实例已被释放。</exception>
        public static double Timeout { get { return _HttpClient.Timeout.TotalMilliseconds; } set { _HttpClient.Timeout = TimeSpan.FromMilliseconds(value); } } // = 5 * 1000;

        static HttpHelpers()
        {
            HttpHandler = CreateHttpHandler();
            _HttpClient = new(HttpHandler, true);   //HttpClientHandler
        }

        private static SocketsHttpHandler CreateHttpHandler()
        {
            return new SocketsHttpHandler() { UseCookies = false, AutomaticDecompression = DecompressionMethods.All, SslOptions = new System.Net.Security.SslClientAuthenticationOptions { RemoteCertificateValidationCallback = (a, b, c, d) => true } };
        }

        /// <summary>
        /// 自动回收
        /// </summary>
        ~HttpHelpers() { _HttpClient.Dispose(); }

        /// <summary>
        /// GET 方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static Stream Get(string url, Func<HttpRequestHeaders, HttpContent> onheaders = null)
        {
            try
            {
                using var requestMessage = CreateHttpRequestMessage(HttpMethod.Get, url);

                var content = onheaders?.Invoke(requestMessage.Headers);
                if (content != null) requestMessage.Content = content;

                using var http = Send(requestMessage);

                return GetMemory(http.Content);
            }
            catch (Exception ex)
            {
                Log.Error("Get", ex, LogFilePath);
                return default;
            }
        }

        /// <summary>
        /// GET 方式获取响应流(异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static async Task<Stream> GetAsync(string url, Func<HttpRequestHeaders, HttpContent> onheaders = null)
        {
            try
            {
                using var requestMessage = CreateHttpRequestMessage(HttpMethod.Get, url);

                var content = onheaders?.Invoke(requestMessage.Headers);
                if (content != null) requestMessage.Content = content;

                using var http = await SendAsync(requestMessage);

                return await GetMemoryAsync(http.Content);
            }
            catch (Exception ex)
            {
                Log.Error("GetAsync", ex, LogFilePath);
                return default;
            }
        }

        /// <summary>
        /// GET 方式获取响应流 返回实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static T GetJson<T>(string url, Func<HttpRequestHeaders, HttpContent> onheaders = null)
        {
            try
            {
                var _json = GetString(url, onheaders);
                return _json.Json<T>();
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// GET 方式获取响应流 返回实体 (异步获取)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static async Task<T> GetJsonAsync<T>(string url, Func<HttpRequestHeaders, HttpContent> onheaders = null)
        {
            try
            {
                var _json = await GetStringAsync(url, onheaders);
                return _json.Json<T>();
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// GET 方式获取响应流 返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static string GetString(string url, Func<HttpRequestHeaders, HttpContent> onheaders = null)
        {
            var result = Get(url, onheaders);
            return GetString(result);
        }

        /// <summary>
        /// GET 方式获取响应流 返回字符串 (异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(string url, Func<HttpRequestHeaders, HttpContent> onheaders = null)
        {
            var result = await GetAsync(url, onheaders);
            return await GetStringAsync(result);
        }

        /// <summary>
        /// POST 方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static Stream Post(string url, Func<HttpRequestHeaders, HttpContent> onheaders = null)
        {
            try
            {
                using var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, url);

                var content = onheaders?.Invoke(requestMessage.Headers);
                if (content != null) requestMessage.Content = content;

                using var http = Send(requestMessage);

                return GetMemory(http.Content);
            }
            catch (Exception ex)
            {
                Log.Error("Post", ex, LogFilePath);
                return default;
            }
        }

        /// <summary>
        /// POST 方式获取响应流(异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static async Task<Stream> PostAsync(string url, Func<HttpRequestHeaders, HttpContent> onheaders = null)
        {
            try
            {
                using var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, url);

                var content = onheaders?.Invoke(requestMessage.Headers);
                if (content != null) requestMessage.Content = content;

                using var http = await SendAsync(requestMessage);

                return await GetMemoryAsync(http.Content);

            }
            catch (Exception ex)
            {
                Log.Error("PostAsync", ex, LogFilePath);
                return default;
            }
        }

        /// <summary>
        /// POST 方式获取响应流  返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static string PostString(string url, Func<HttpRequestHeaders, HttpContent> onheaders = null)
        {
            var result = Post(url, onheaders);
            return GetString(result);
        }

        /// <summary>
        /// POST 方式获取响应流  返回字符串 (异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static async Task<string> PostStringAsync(string url, Func<HttpRequestHeaders, HttpContent> onheaders = null)
        {
            var result = await PostAsync(url, onheaders);
            return await GetStringAsync(result);
        }


        /// <summary>
        /// POST 方式获取响应流  返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static string PostString(string url, IDictionary<string, string> data, Action<HttpRequestHeaders> onheaders = null)
        {
            var result = Post(url, data, onheaders);
            return GetString(result);
        }

        /// <summary>
        /// POST 方式获取响应流  返回字符串 (异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static async Task<string> PostStringAsync(string url, IDictionary<string, string> data, Action<HttpRequestHeaders> onheaders = null)
        {
            var result = await PostAsync(url, data, onheaders);
            return await GetStringAsync(result);
        }

        /// <summary>
        /// POST 方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static Stream Post(string url, IEnumerable<KeyValuePair<string, string>> data, Action<HttpRequestHeaders> onheaders = null)
        {
            try
            {
                return Post(url, ops => { onheaders(ops); return data is null ? null : BodyForm(data); });
            }
            catch (Exception ex)
            {
                Log.Error("Post", ex, LogFilePath);
                return default;
            }
        }

        /// <summary>
        /// POST 方式获取响应流(异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static async Task<Stream> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> data, Action<HttpRequestHeaders> onheaders = null)
        {
            try
            {
                return await PostAsync(url, ops => { onheaders(ops); return data is null ? null : BodyForm(data); });
            }
            catch (Exception ex)
            {
                Log.Error("PostAsync", ex, LogFilePath);
                return default;
            }
        }

        /// <summary>
        /// POST 方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static Stream Post(string url, IDictionary<string, string> data = null, Action<HttpRequestHeaders> onheaders = null)
        {
            return Post(url, data as IEnumerable<KeyValuePair<string, string>>, onheaders);
        }

        /// <summary>
        /// POST 方式获取响应流(异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static async Task<Stream> PostAsync(string url, IDictionary<string, string> data = null, Action<HttpRequestHeaders> onheaders = null)
        {
            return await PostAsync(url, data as IEnumerable<KeyValuePair<string, string>>, onheaders);
        }

        /// <summary>
        /// POST 方式获取响应流  返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data">字符串拼接的数据</param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static string PostString(string url, string data = null, Action<HttpRequestHeaders> onheaders = null)
        {
            try
            {
                var s = FormatData(data);
                return PostString(url, s, onheaders);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// POST 方式获取响应流  返回字符串 (异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static async Task<string> PostStringAsync(string url, string data = null, Action<HttpRequestHeaders> onheaders = null)
        {
            try
            {
                var s = FormatData(data);
                return await PostStringAsync(url, s, onheaders);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// POST 方式获取响应流  返回实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static T PostJson<T>(string url, string data = null, Action<HttpRequestHeaders> onheaders = null)
        {
            try
            {
                var _json = PostString(url, data, onheaders);
                return _json.Json<T>();
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// POST 方式获取响应流  返回实体 (异步获取)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static async Task<T> PostJsonAsync<T>(string url, string data = null, Action<HttpRequestHeaders> onheaders = null)
        {
            try
            {
                var _json = await PostStringAsync(url, data, onheaders);
                return _json.Json<T>();
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// POST 方式获取响应流  返回实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static T PostJson<T>(string url, IDictionary<string, string> data = null, Action<HttpRequestHeaders> onheaders = null)
        {
            return PostString(url, data, onheaders).Json<T>();
        }

        /// <summary>
        /// POST 方式获取响应流  返回实体 (异步获取)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static async Task<T> PostJsonAsync<T>(string url, IDictionary<string, string> data = null, Action<HttpRequestHeaders> onheaders = null)
        {
            var _str = await PostStringAsync(url, data, onheaders);
            return _str.Json<T>();
        }

        /// <summary>
        /// HEAD 方式获取响应的状态
        /// </summary>
        /// <param name="url"></param>
        /// <param name="onheaders"></param>
        /// <returns></returns>
        public static HttpStatusCode HeadHttpCode(string url, Func<HttpRequestHeaders, HttpContent> onheaders = null)
        {
            try
            {
                using var requestMessage = CreateHttpRequestMessage(HttpMethod.Head, url);

                var content = onheaders?.Invoke(requestMessage.Headers);
                if (content != null) requestMessage.Content = content;

                using var http = Send(requestMessage);

                return http.StatusCode;
            }
            catch (Exception ex)
            {
                Log.Error("HeadHttpCode", ex, LogFilePath);
                return HttpStatusCode.ExpectationFailed;
            }
        }

        ///// <summary>
        ///// 根据http链接获取<see cref="HttpWebRequest"/>对象
        ///// </summary>
        ///// <param name="url"></param>
        ///// <returns></returns>
        //private static HttpWebRequest CreateHttpWebRequest(string url)
        //{
        //    var r = WebRequest.Create(url) as HttpWebRequest;
        //    //r.Headers["X-Requested-With"] = "XMLHttpRequest";
        //    //var s = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Maxthon/4.1.2.4000 Chrome/26.0.1410.43 Safari/537.1";
        //    //r.Headers["User-Agent"] = s;
        //    return r;
        //}

        //requestMessage.Headers.Referrer = refer;
        //if (!string.IsNullOrWhiteSpace(cookies)) requestMessage.Headers.TryAddWithoutValidation("Cookie", cookies);

        //var stream = await http.Content.ReadAsStreamAsync();
        //byte[] buffer = new byte[stream.Length];
        //await stream.ReadAsync(buffer);
        //MemoryStream memoryStream = new(buffer);
        //return memoryStream;

        //http.EnsureSuccessStatusCode();

        /// <summary>
        /// 获取可用的连接
        /// </summary>
        /// <param name="method"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string url)
        {
            return new(method, url);
        }

        /// <summary>
        /// 异步获取请求结果
        /// </summary>
        /// <param name="requestMessage">请求信息</param>
        /// <returns></returns>
        public static async Task<HttpResponseMessage> SendAsync(HttpRequestMessage requestMessage)
        {
            return await _HttpClient.SendAsync(requestMessage);
        }

        /// <summary>
        /// 获取请求结果
        /// </summary>
        /// <param name="requestMessage">请求信息</param>
        /// <returns></returns>
        public static HttpResponseMessage Send(HttpRequestMessage requestMessage)
        {
            return _HttpClient.Send(requestMessage);
        }

        /// <summary>
        /// 获取内存流
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static async Task<Stream> GetMemoryAsync(HttpContent content)
        {
            MemoryStream memoryStream = new();
            using (content) await content.CopyToAsync(memoryStream, null, CancellationToken.None);
            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>
        /// 获取内存流
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static Stream GetMemory(HttpContent content)
        {
            MemoryStream memoryStream = new();
            using (content) content.CopyTo(memoryStream, null, CancellationToken.None);
            memoryStream.Position = 0;
            return memoryStream;
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static string GetString(Stream result)
        {
            if (result == null)
            {
                return null; //throw new Exception("请求失败。");
            }
            using var _StreamReader = new StreamReader(result, DefaultEncoding ?? Encoding.Default);
            return _StreamReader.ReadToEnd();
        }

        /// <summary>
        /// 获取字符串
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(Stream result)
        {
            if (result == null)
            {
                return null; //throw new Exception("请求失败。");
            }
            using var _StreamReader = new StreamReader(result, DefaultEncoding ?? Encoding.Default);
            return await _StreamReader.ReadToEndAsync();
        }

        ///// <summary>
        ///// 返回 Format 表单提交
        ///// </summary>
        ///// <param name="data"></param>
        ///// <returns></returns>
        //internal static string FormatData(IEnumerable<KeyValuePair<string, string>> data)
        //{
        //    var s = data == null
        //        ? null
        //        : string.Join("&",
        //            data.Select(d => string.Format("{0}={1}", d.Key, Uri.EscapeDataString(d.Value))));
        //    return s;
        //}

        /// <summary>
        /// 根据字典高效组装成以转义的字符串
        /// </summary>
        /// <param name="data">字典对象</param>
        /// <returns></returns>
        public static string QueryString(IDictionary<string, string> data)
        {
            return data is null
                 ? string.Empty
                 : QueryHelpers.AddQueryString(string.Empty, data);
        }

        /// <summary>
        /// 高效解析http表单类文本 
        /// </summary>
        /// <param name="query">待解析的http表单值</param>
        /// <returns></returns>
        public static IDictionary<string, string> FormatData(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return default;
            var querys = QueryHelpers.ParseNullableQuery(query);

            Dictionary<string, string> result = new(querys.Count);

            foreach (var pair in querys)
            {
                result.Add(pair.Key, pair.Value);
            }

            return result;
        }

        /// <summary>
        /// 生成请求体
        /// </summary>
        /// <param name="data"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static HttpContent BodyString(string data, string ContentType = "application/json")
        {
            HttpContent content = new ByteArrayContent(data.ToBytes(encoding: DefaultEncoding));
            content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            return content;
        }

        /// <summary>
        /// 生成请求体
        /// </summary>
        /// <param name="data"></param>
        /// <param name="bufferSize"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static HttpContent BodyStream(Stream data, int bufferSize = -1, string ContentType = "application/json")
        {
            HttpContent content = new StreamContent(data, bufferSize == -1 ? (int)data.Length : bufferSize);
            content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            return content;
        }

        /// <summary>
        /// 生成请求体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        public static HttpContent BodyForm<T>(T data, string ContentType = "application/x-www-form-urlencoded") where T : IEnumerable<KeyValuePair<string, string>>
        {
            HttpContent content = new FormUrlEncodedContent(data);
            content.Headers.ContentType = new MediaTypeHeaderValue(ContentType);
            return content;
        }

        //public static IDictionary<string, string> FormatData(string query)
        //{
        //    if (string.IsNullOrWhiteSpace(query)) return default;
        //    var nameValue = System.Web.HttpUtility.ParseQueryString(query);//弃用的

        //    Dictionary<string, string> result = new();

        //    foreach (string val in nameValue)
        //    {
        //        result.Add(val, nameValue.Get(val));
        //    }

        //    return result;
        //}
    }
}
