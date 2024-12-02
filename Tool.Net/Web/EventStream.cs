using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;
using System;

namespace Tool.Web
{
    /// <summary>
    /// HTTP协议的事件流（EventStream）简称SSE 接口
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public interface IEventStream
    {
        /// <summary>
        /// 告知断开后多久重连
        /// </summary>
        public int Retry { get; }

        /// <summary>
        /// 输出的编码格式
        /// </summary>
        public Encoding ContentEncoding { get; }

        /// <summary>
        /// 最后一次接收到的事件的标识符
        /// </summary>
        public int LastEventID { get; }

        /// <summary>
        /// 开始挂起输出流，直到<see cref="Func{EventStream, Task}"/> 函数完成为止。
        /// </summary>
        /// <returns>任务</returns>
        Task ExecuteResultAsync();
    }

    /// <summary>
    /// SSE服务器事件流
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class EventStream : IEventStream
    {
        /// <summary>
        /// 最后一次接收到的事件的标识符
        /// </summary>
        public int LastEventID
        {
            get;
            set;
        }

        /// <summary>
        /// 输出的编码格式
        /// </summary>
        public Encoding ContentEncoding { get; set; }

        /// <summary>
        /// 告知断开后多久重连
        /// </summary>
        public int Retry { get; set; }

        /// <summary>
        /// 连接对象
        /// </summary>
        public HttpContext Context { get; }

        private readonly Func<EventStream, Task> func;

        /// <summary>
        /// SSE服务器事件流
        /// </summary>
        /// <param name="func">流回复任务</param>
        /// <param name="context">连接对象</param>
        /// <param name="retry">指定浏览器重新发起连接的时间间隔</param>
        public EventStream(Func<EventStream, Task> func, HttpContext context, int retry = 3 * 1000)
        {
            if (func is null) throw new ArgumentNullException(nameof(func));
            if (context is null) throw new ArgumentNullException(nameof(context));
            this.ContentEncoding = Encoding.UTF8;
            this.Retry = retry;
            this.func = func;
            Context = context;
        }

        /// <summary>
        /// 开始挂起输出流，直到<see cref="Func{EventStream, Task}"/> 函数完成为止。
        /// </summary>
        /// <returns>任务</returns>
        public async Task ExecuteResultAsync()
        {
            if (Context.Request.Headers.TryGetValue("Last-Event-ID", out var value) && int.TryParse(value, out int id))
            {
                LastEventID = id;
            }

            Context.Response.ContentType = "text/event-stream; charset=utf-8";
#if NET6_0_OR_GREATER
            Context.Response.Headers.CacheControl = "no-cache";
            Context.Response.Headers.KeepAlive = "timeout=5";
#else
            Context.Response.Headers["Cache-Control"] = "no-cache";
            Context.Response.Headers["Keep-Alive"] = "timeout=5";
#endif
            Context.Response.StatusCode = 200;
            await Context.Response.FlushAsync();

            //心跳
            //var pong = $"Server Pong {DateTime.Now:yyyy:MM:dd HH:mm:ss.fff}";

            //TaskHelper.LongRunning(() =>
            //{
            //    ServerSent(Encoding.UTF8.GetBytes($": {SerializeHelper.Serialize(pong)}\n\n"));
            //}, 1000);

            //断开重连时长
            await ServerSent(Encoding.UTF8.GetBytes($"retry: {Retry}\n\n"));

            await func(this);
        }

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="t"></param>
        /// <param name="event"></param>
        /// <param name="id"></param>
        public async Task ServerSent<T>(T t, string @event = "message", string id = "") where T : class
        {
            if (string.IsNullOrEmpty(id)) id = (LastEventID++).ToString();
            if (t != null)
                await ServerSent(ContentEncoding?.GetBytes($"id: {id?.Trim()}\nevent: {@event?.Trim()}\ndata: {t.ToJson()}\n\n"));
        }

        /// <summary>
        /// 发送通知
        /// </summary>
        /// <param name="content"></param>
        private async Task ServerSent(byte[] content)
        {
            await Context?.Response.WriteAsync(content);
            await Context?.Response.FlushAsync();
        }
    }
}