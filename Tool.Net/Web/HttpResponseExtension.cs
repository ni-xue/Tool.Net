using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Web
{
    /// <summary>
    /// 对HttpResponse进行升级
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class HttpResponseExtension
    {

        /// <summary>
        /// 将 HTTP 头添加到输出流。
        /// </summary>
        /// <param name="response">对象</param>
        /// <param name="name">要添加到输出流的 HTTP 头的名称。</param>
        /// <param name="value">要追加到头中的字符串。</param>
        /// <exception cref="System.NotSupportedException">已发送的 HTTP 标头之后追加标头。</exception>
        public static void AppendHeader(this HttpResponse response, string name, string value)
        {
            response.Headers.Add(name.StringEncode(), value.StringEncode());
        }

        /// <summary>
        /// 添加 Cookie 信息
        /// </summary>
        /// <param name="response">对象</param>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void AppendCookie(this HttpResponse response, string key, string value)
        {
            response.Cookies.Append(key, value);
        }

        /// <summary>
        /// 输出HTTP流
        /// </summary>
        /// <param name="response">对象</param>
        /// <param name="obj">输出内容</param>
        public static void Write(this HttpResponse response, object obj)
        {
            response.Write(obj.ToString());
        }

        /// <summary>
        /// 输出HTTP流
        /// </summary>
        /// <param name="response">对象</param>
        /// <param name="obj">输出内容</param>
        public static async Task WriteAsync(this HttpResponse response, object obj)
        {
            await response.WriteAsync(obj.ToString());
        }

        /// <summary>
        /// 输出HTTP流
        /// </summary>
        /// <param name="response">对象</param>
        /// <param name="txt">输出内容</param>
        public static void Write(this HttpResponse response, string txt)
        {
            response.Write(txt.ToBytes(Encoding.UTF8));
        }

        /// <summary>
        /// 输出HTTP流
        /// </summary>
        /// <param name="response">对象</param>
        /// <param name="txt">输出内容</param>
        public static async Task WriteAsync(this HttpResponse response, string txt)
        {
            await response.WriteAsync(txt.ToBytes(Encoding.UTF8));
        }

        /// <summary>
        /// 输出HTTP流
        /// </summary>
        /// <param name="response">对象</param>
        /// <param name="bytes">数据流</param>
        public static void Write(this HttpResponse response, byte[] bytes)
        {
            response.Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 输出HTTP流
        /// </summary>
        /// <param name="response">对象</param>
        /// <param name="bytes">数据流</param>
        public static async Task WriteAsync(this HttpResponse response, byte[] bytes)
        {
           await response.WriteAsync(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// 输出HTTP流
        /// </summary>
        /// <param name="response">对象</param>
        /// <param name="bytes">数据流</param>
        /// <param name="offset">读取开始位置</param>
        /// <param name="count">读取数量</param>
        public static void Write(this HttpResponse response, byte[] bytes, int offset, int count)
        {
            response.WriteAsync(bytes, offset, count).Wait();
        }

        /// <summary>
        /// 输出HTTP流
        /// </summary>
        /// <param name="response">对象</param>
        /// <param name="bytes">数据流</param>
        /// <param name="offset">读取开始位置</param>
        /// <param name="count">读取数量</param>
        public static async Task WriteAsync(this HttpResponse response, byte[] bytes, int offset, int count)
        {
            await response.Body.WriteAsync(bytes.AsMemory(offset, count));
        }

        /// <summary>
        /// 向客户端发送当前所有缓冲的输出。(里面实现的异步方式)
        /// </summary>
        /// <param name="response">对象</param>
        public static void Flush(this HttpResponse response)
        {
            response.Body.FlushAsync().Wait();
        }

        /// <summary>
        /// 向客户端发送当前所有缓冲的输出。(里面实现的异步方式)
        /// </summary>
        /// <param name="response">对象</param>
        public static async Task FlushAsync(this HttpResponse response)
        {
            await response.Body.FlushAsync();
        }
    }
}
