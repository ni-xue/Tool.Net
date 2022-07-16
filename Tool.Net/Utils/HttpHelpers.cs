using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
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

            _HttpClient = new(new SocketsHttpHandler() { UseCookies = false, AutomaticDecompression = DecompressionMethods.All, SslOptions = new System.Net.Security.SslClientAuthenticationOptions { RemoteCertificateValidationCallback = (a, b, c, d) => true } }, true);   //HttpClientHandler
        }

        /// <summary>
        /// 自动回收
        /// </summary>
        ~HttpHelpers() { _HttpClient.Dispose(); }

        /// <summary>
        /// GET 方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static Stream Get(string url, string cookies = null, Uri refer = null)
        {
            try
            {
                using var requestMessage = CreateHttpRequestMessage(HttpMethod.Get, url);

                requestMessage.Headers.Referrer = refer;

                if (!string.IsNullOrWhiteSpace(cookies)) requestMessage.Headers.TryAddWithoutValidation("Cookie", cookies);

                using var http = _HttpClient.Send(requestMessage);

                //http.EnsureSuccessStatusCode();

                MemoryStream memoryStream = new();
                http.Content.CopyTo(memoryStream, null, CancellationToken.None);
                memoryStream.Position = 0;
                return memoryStream;

                //var stream = http.Content.ReadAsStream();

                //byte[] buffer = new byte[stream.Length];

                //stream.Read(buffer);

                //MemoryStream memoryStream = new(buffer);

                //return memoryStream;
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
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<Stream> GetAsync(string url, string cookies = null, Uri refer = null)
        {
            try
            {
                using var requestMessage = CreateHttpRequestMessage(HttpMethod.Get, url);

                requestMessage.Headers.Referrer = refer;

                if (!string.IsNullOrWhiteSpace(cookies)) requestMessage.Headers.TryAddWithoutValidation("Cookie", cookies);

                using var http = await _HttpClient.SendAsync(requestMessage);

                //http.EnsureSuccessStatusCode();

                MemoryStream memoryStream = new();
                await http.Content.CopyToAsync(memoryStream, null, CancellationToken.None);
                memoryStream.Position = 0;
                return memoryStream;

                //var stream = await http.Content.ReadAsStreamAsync();

                //byte[] buffer = new byte[stream.Length];

                //await stream.ReadAsync(buffer);

                //MemoryStream memoryStream = new(buffer);

                //return memoryStream;
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
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static T GetJson<T>(string url, string cookies = null, Uri refer = null)
        {
            try
            {
                var _json = GetString(url, cookies, refer);
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
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<T> GetJsonAsync<T>(string url, string cookies = null, Uri refer = null)
        {
            try
            {
                var _json = await GetStringAsync(url, cookies, refer);
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
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string GetString(string url, string cookies = null, Uri refer = null)
        {
            var result = Get(url, cookies, refer);

            if (result == null)
            {
                return null; //throw new Exception("请求失败。");
            }

            using var _StreamReader = new StreamReader(result, DefaultEncoding ?? Encoding.Default);
            return _StreamReader.ReadToEnd();
        }

        /// <summary>
        /// GET 方式获取响应流 返回字符串 (异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(string url, string cookies = null, Uri refer = null)
        {
            var result = await GetAsync(url, cookies, refer);

            if (result == null)
            {
                return null; //throw new Exception("请求失败。");
            }

            using var _StreamReader = new StreamReader(result, DefaultEncoding ?? Encoding.Default);
            return await _StreamReader.ReadToEndAsync();
        }

        /// <summary>
        /// POST 方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static Stream Post(string url, IEnumerable<KeyValuePair<string, string>> data, string cookies = null, Uri refer = null)
        {
            try
            {
                using var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, url);

                requestMessage.Headers.Referrer = refer;

                if (!string.IsNullOrWhiteSpace(cookies)) requestMessage.Headers.TryAddWithoutValidation("Cookie", cookies);

                if (data != null) requestMessage.Content = new FormUrlEncodedContent(data);

                using var http = _HttpClient.Send(requestMessage);

                //http.EnsureSuccessStatusCode();

                MemoryStream memoryStream = new();
                http.Content.CopyTo(memoryStream, null, CancellationToken.None);
                memoryStream.Position = 0;
                return memoryStream;

                //var stream = http.Content.ReadAsStream();

                //byte[] buffer = new byte[stream.Length];

                //stream.Read(buffer);

                //MemoryStream memoryStream = new(buffer);

                //return memoryStream;
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
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<Stream> PostAsync(string url, IEnumerable<KeyValuePair<string, string>> data, string cookies = null, Uri refer = null)
        {
            try
            {
                using var requestMessage = CreateHttpRequestMessage(HttpMethod.Post, url);

                requestMessage.Headers.Referrer = refer;

                if (!string.IsNullOrWhiteSpace(cookies)) requestMessage.Headers.TryAddWithoutValidation("Cookie", cookies);

                if (data != null) requestMessage.Content = new FormUrlEncodedContent(data);

                using var http = await _HttpClient.SendAsync(requestMessage);

                //http.EnsureSuccessStatusCode();

                MemoryStream memoryStream = new();
                await http.Content.CopyToAsync(memoryStream, null, CancellationToken.None);
                memoryStream.Position = 0;
                return memoryStream;

                //var stream = await http.Content.ReadAsStreamAsync();

                //byte[] buffer = new byte[stream.Length];

                //await stream.ReadAsync(buffer);

                //MemoryStream memoryStream = new(buffer);

                //return memoryStream;
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
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static Stream Post(string url, IDictionary<string, string> data = null, string cookies = null)
        {
            return Post(url, data, cookies, null);
        }

        /// <summary>
        /// POST 方式获取响应流(异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cookies"></param>
        /// <returns></returns>
        public static async Task<Stream> PostAsync(string url, IDictionary<string, string> data = null, string cookies = null)
        {
            return await PostAsync(url, data, cookies, null);
        }

        /// <summary>
        /// POST 方式获取响应流  返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data">字符串拼接的数据</param>
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string PostString(string url, string data = null, string cookies = null, Uri refer = null)
        {
            try
            {
                var s = FormatData(data);
                return PostString(url, s, cookies, refer);
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
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<string> PostStringAsync(string url, string data = null, string cookies = null, Uri refer = null)
        {
            try
            {
                var s = FormatData(data);
                return await PostStringAsync(url, s, cookies, refer);
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// POST 方式获取响应流  返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string PostString(string url, IDictionary<string, string> data, string cookies = null, Uri refer = null)
        {
            var result = Post(url, data, cookies, refer);

            if (result == null)
            {
                return null; //throw new Exception("请求失败。");
            }
            using var _StreamReader = new StreamReader(result, DefaultEncoding ?? Encoding.Default);
            return _StreamReader.ReadToEnd();
        }

        /// <summary>
        /// POST 方式获取响应流  返回字符串 (异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<string> PostStringAsync(string url, IDictionary<string, string> data, string cookies = null, Uri refer = null)
        {
            var result = await PostAsync(url, data, cookies, refer);

            if (result == null)
            {
                return null; //throw new Exception("请求失败。");
            }
            using var _StreamReader = new StreamReader(result, DefaultEncoding ?? Encoding.Default);
            return await _StreamReader.ReadToEndAsync();
        }

        /// <summary>
        /// POST 方式获取响应流  返回实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static T PostJson<T>(string url, string data = null, string cookies = null, Uri refer = null)
        {
            try
            {
                var _json = PostString(url, data, cookies, refer);
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
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<T> PostJsonAsync<T>(string url, string data = null, string cookies = null, Uri refer = null)
        {
            try
            {
                var _json = await PostStringAsync(url, data, cookies, refer);
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
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static T PostJson<T>(string url, IDictionary<string, string> data = null, string cookies = null, Uri refer = null)
        {
            return PostString(url, data, cookies, refer).Json<T>();
        }

        /// <summary>
        /// POST 方式获取响应流  返回实体 (异步获取)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<T> PostJsonAsync<T>(string url, IDictionary<string, string> data = null, string cookies = null, Uri refer = null)
        {
            var _str = await PostStringAsync(url, data, cookies, refer);
            return _str.Json<T>();
        }

        /// <summary>
        /// HEAD 方式获取响应的状态
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cookies"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static HttpStatusCode HeadHttpCode(string url, /*string data = null,*/// <param name="data"></param>
            string cookies = null, Uri refer = null)
        {
            try
            {
                using var requestMessage = CreateHttpRequestMessage(HttpMethod.Head, url);

                requestMessage.Headers.Referrer = refer;

                if (!string.IsNullOrWhiteSpace(cookies)) requestMessage.Headers.TryAddWithoutValidation("Cookie", cookies);

                using var http = _HttpClient.Send(requestMessage);

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

        private static HttpRequestMessage CreateHttpRequestMessage(HttpMethod method, string url)
        {
            return new(method, url);
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
        /// 高效解析http表单类文本 
        /// </summary>
        /// <param name="query">待解析的http表单值</param>
        /// <returns></returns>
        public static IDictionary<string, string> FormatData(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return default;
            var querys = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseNullableQuery(query);

            Dictionary<string, string> result = new();

            foreach (var pair in querys)
            {
                result.Add(pair.Key, pair.Value);
            }

            return result;
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
