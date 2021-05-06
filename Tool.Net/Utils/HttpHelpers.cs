using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Utils
{
    /// <summary>
    /// 提供部分的API请求访问类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class HttpHelpers
    {
        /// <summary>
        /// 默认编码格式
        /// </summary>
        public static Encoding DefaultEncoding { get; set; } = Encoding.UTF8;

        /// <summary>
        /// 毫秒
        /// </summary>
        public static int Timeout { get; set; } = 5 * 1000;

        /// <summary>
        /// GET 方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="queryString"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static Stream Get(string url, string queryString = null, CookieContainer cc = null, string refer = null)
        {
            try
            {
                url = string.Format("{0}?{1}", url, queryString);
                var r = CreateHttpWebRequest(url);
                r.Referer = refer;
                r.CookieContainer = cc;
                r.Method = "GET";
                return r.GetResponse().GetResponseStream();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// GET 方式获取响应流(异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="queryString"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<Stream> GetAsync(string url, string queryString = null, CookieContainer cc = null, string refer = null)
        {
            try
            {
                //System.Net.Http.IHttpClientFactorySystem.Net.Http.HttpClient

                url = string.Format("{0}?{1}", url, queryString);
                var r = CreateHttpWebRequest(url);
                r.Referer = refer;
                r.CookieContainer = cc;
                r.Method = "GET";

                WebResponse response = await r.GetResponseAsync();
                return response.GetResponseStream();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// GET 方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static Stream Get(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            return Get(url, FormatData(data), cc, refer);
        }

        /// <summary>
        /// GET 方式获取响应流(异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<Stream> GetAsync(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            return await GetAsync(url, FormatData(data), cc, refer);
        }
        

        /// <summary>
        /// GET 方式获取响应流 返回实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static T Get<T>(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            try
            {
                var s = FormatData(data);

                using var _StreamReader = new StreamReader(Get(url, s, cc, refer), DefaultEncoding);
                return _StreamReader.ReadToEnd().Json<T>();
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
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<T> GetAsync<T>(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            try
            {
                var s = FormatData(data);

                using var _StreamReader = new StreamReader(await GetAsync(url, s, cc, refer), DefaultEncoding);
                return _StreamReader.ReadToEnd().Json<T>();
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
        /// <param name="queryString"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string GetString(string url, string queryString = null, CookieContainer cc = null,
            string refer = null)
        {
            using var _StreamReader = new StreamReader(Get(url, queryString, cc, refer));
            return _StreamReader.ReadToEnd();
        }

        /// <summary>
        /// GET 方式获取响应流 返回字符串 (异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="queryString"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(string url, string queryString = null, CookieContainer cc = null,
            string refer = null)
        {
            using var _StreamReader = new StreamReader(await GetAsync(url, queryString, cc, refer));
            return _StreamReader.ReadToEnd();
        }

        /// <summary>
        /// GET 方式获取响应流 返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string GetString(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            using var _StreamReader = new StreamReader(Get(url, FormatData(data), cc, refer));
            return _StreamReader.ReadToEnd();
        }

        /// <summary>
        /// GET 方式获取响应流 返回字符串 (异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<string> GetStringAsync(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            using var _StreamReader = new StreamReader(await GetAsync(url, FormatData(data), cc, refer));
            return _StreamReader.ReadToEnd();
        }


        /// <summary>
        /// POST 方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static Stream Post(string url, string data = null, CookieContainer cc = null, string refer = null)
        {
            try
            {
                var r = CreateHttpWebRequest(url);
                r.Timeout = Timeout;
                r.Method = "POST";
                r.Referer = refer;
                r.CookieContainer = cc;
                r.ContentType = "application/x-www-form-urlencoded";
                if (data != null)
                {
                    var bs = DefaultEncoding.GetBytes(data);
                    r.ContentLength = bs.Length;
                    var stream = r.GetRequestStream();
                    stream.Write(bs, 0, bs.Length);
                    stream.Flush();
                    stream.Close();
                }
                else
                {
                    r.ContentLength = 0;
                }

                //var sd = r.GetResponse() as HttpWebResponse;

                //if (sd.StatusCode != HttpStatusCode.OK)
                //{

                //}

                //return sd.GetResponseStream();

                return r.GetResponse().GetResponseStream();
            }
            catch (WebException)
            {
                throw;
            }
        }

        /// <summary>
        /// POST 方式获取响应流(异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<Stream> PostAsync(string url, string data = null, CookieContainer cc = null, string refer = null)
        {
            try
            {
                var r = CreateHttpWebRequest(url);
                r.Timeout = Timeout;
                r.Method = "POST";
                r.Referer = refer;
                r.CookieContainer = cc;
                r.ContentType = "application/x-www-form-urlencoded";
                if (data != null)
                {
                    var bs = DefaultEncoding.GetBytes(data);
                    r.ContentLength = bs.Length;
                    var stream = r.GetRequestStream();
                    stream.Write(bs, 0, bs.Length);
                    stream.Flush();
                    stream.Close();
                }
                else
                {
                    r.ContentLength = 0;
                }

                //var sd = r.GetResponse() as HttpWebResponse;

                //if (sd.StatusCode != HttpStatusCode.OK)
                //{

                //}

                //return sd.GetResponseStream();

                WebResponse response = await r.GetResponseAsync();
                return response.GetResponseStream();
            }
            catch (WebException)
            {
                throw;
            }
        }

        /// <summary>
        /// POST 方式获取响应流
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static Stream Post(string url, IDictionary<string, object> data = null, CookieContainer cc = null, string refer = null)
        {
            var s = FormatData(data);
            return Post(url, s, cc, refer);
        }

        /// <summary>
        /// POST 方式获取响应流(异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<Stream> PostAsync(string url, IDictionary<string, object> data = null, CookieContainer cc = null, string refer = null)
        {
            var s = FormatData(data);
            return await PostAsync(url, s, cc, refer);
        }

        /// <summary>
        /// POST 方式获取响应流  返回字符串
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string PostString(string url, string data = null, CookieContainer cc = null, string refer = null)
        {
            try
            {
                using var _StreamReader = new StreamReader(Post(url, data, cc, refer), DefaultEncoding);
                return _StreamReader.ReadToEnd();
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
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<string> PostStringAsync(string url, string data = null, CookieContainer cc = null, string refer = null)
        {
            try
            {
                using var _StreamReader = new StreamReader(await PostAsync(url, data, cc, refer), DefaultEncoding);
                return _StreamReader.ReadToEnd();
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
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static string PostString(string url, IDictionary<string, object> data,
            CookieContainer cc = null, string refer = null)
        {
            var s = FormatData(data);
            using var _StreamReader = new StreamReader(Post(url, s, cc, refer), DefaultEncoding);
            return _StreamReader.ReadToEnd();
        }

        /// <summary>
        /// POST 方式获取响应流  返回字符串 (异步获取)
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<string> PostStringAsync(string url, IDictionary<string, object> data,
            CookieContainer cc = null, string refer = null)
        {
            var s = FormatData(data);
            using var _StreamReader = new StreamReader(await PostAsync(url, s, cc, refer), DefaultEncoding);
            return _StreamReader.ReadToEnd();
        }

        /// <summary>
        /// POST 方式获取响应流  返回实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static T Post<T>(string url, string data = null, CookieContainer cc = null, string refer = null)
        {
            try
            {
                using var _StreamReader = new StreamReader(Post(url, data, cc, refer), DefaultEncoding);
                return _StreamReader.ReadToEnd().Json<T>();
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
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string url, string data = null, CookieContainer cc = null, string refer = null)
        {
            try
            {
                using var _StreamReader = new StreamReader(await PostAsync(url, data, cc, refer), DefaultEncoding);
                return _StreamReader.ReadToEnd().Json<T>();
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
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static T Post<T>(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            var s = FormatData(data);
            return Post<T>(url, s, cc, refer);
        }

        /// <summary>
        /// POST 方式获取响应流  返回实体 (异步获取)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static async Task<T> PostAsync<T>(string url, IEnumerable<KeyValuePair<string, object>> data = null,
            CookieContainer cc = null, string refer = null)
        {
            var s = FormatData(data);
            return await PostAsync<T>(url, s, cc, refer);
        }

        /// <summary>
        /// HEAD 方式获取响应的状态
        /// </summary>
        /// <param name="url"></param>
        /// <param name="cc"></param>
        /// <param name="refer"></param>
        /// <returns></returns>
        public static HttpStatusCode HeadHttpCode(string url, /*string data = null,*/// <param name="data"></param>
            CookieContainer cc = null, string refer = null)
        {
            try
            {
                var r = CreateHttpWebRequest(url);
                r.Timeout = Timeout;
                r.Method = "HEAD";
                r.Referer = refer;
                r.CookieContainer = cc;
                return (r.GetResponse() as HttpWebResponse).StatusCode;
            }
            catch
            {
                return HttpStatusCode.ExpectationFailed;
            }
        }

        /// <summary>
        /// 根据http链接获取<see cref="HttpWebRequest"/>对象
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private static HttpWebRequest CreateHttpWebRequest(string url)
        {
            var r = WebRequest.Create(url) as HttpWebRequest;
            //r.Headers["X-Requested-With"] = "XMLHttpRequest";
            //var s = "Mozilla/5.0 (Windows NT 6.2; WOW64) AppleWebKit/537.1 (KHTML, like Gecko) Maxthon/4.1.2.4000 Chrome/26.0.1410.43 Safari/537.1";
            //r.Headers["User-Agent"] = s;
            return r;
        }

        /// <summary>
        /// 返回 Format 表单提交
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static string FormatData(IEnumerable<KeyValuePair<string, object>> data)
        {
            var s = data == null
                ? null
                : string.Join("&",
                    data.Select(d => string.Format("{0}={1}", d.Key, Uri.EscapeDataString(Convert.ToString(d.Value)))));
            return s;
        }
    }
}
