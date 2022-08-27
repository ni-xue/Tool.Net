using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Web.Api.ApiCore;
using Tool.Web.Routing;

namespace Tool.Web.Api
{
    /// <summary>
    /// 针对于新版Ashx路由模式，的同步，异步，支持
    /// <para>该控制器，相对于Mvc的控制器轻，应有功能都有，可自由扩展。</para>
    /// <para>方便实现，您最想实现的效果，最大的优点还是因为他轻量级。</para>
    /// <example>   Api 方法创建示例：
    /// <code>
    ///   public <see cref="void"/> GetApi(<see cref="string"/> context)  => Json(new { msg = "路由版本api。" });
    /// </code>
    /// </example>
    /// <example>Api 方法创建示例：(异步实现)
    /// <code>
    ///   public async <see cref="Task"/> GetTaskApi(<see cref="string"/> context) => await JsonAsync(new { msg = "路由版本api。" });
    /// </code>
    /// </example>
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class ApiAshx : IHttpAsynApi, IHttpApi, IAshxAction
    {
        //private ISession session { get; set; }

        //private HttpContext context { get; set; }

        //private HttpRequest request { get; set; }

        //private HttpResponse response { get; set; }

        private AshxRouteData routeData { get; set; }

        /// <summary>
        /// 获取当前请求的接口唯一ID
        /// </summary>
        public string ApiKey => RouteData.Key;

        /// <summary>
        /// 路由模式
        /// </summary>
        public AshxRouteData RouteData { get { return routeData; } }

        /// <summary>
        /// 为当前 HTTP 请求获取 <see cref="ISession" /> 对象。
        /// </summary>
        public ISession Session { get { return routeData.HttpContext.Session; } }

        /// <summary>
        /// 获取当前 请求获取 <see cref="HttpContext" /> 对象。
        /// </summary>
        public HttpContext Context { get { return routeData.HttpContext; } }

        /// <summary>
        /// 获取当前 HTTP 请求的 <see cref="HttpRequest" /> 对象。
        /// </summary>
        public HttpRequest Request { get { return routeData.HttpContext.Request; } }

        /// <summary>
        /// 获取当前 HTTP 响应的 <see cref="HttpResponse" /> 对象。
        /// </summary>
        public HttpResponse Response { get { return routeData.HttpContext.Response; } }

        /// <summary>
        /// 启用到输出 HTTP 内容主体的二进制输出。
        /// </summary>
        /// <returns>表示输出 HTTP 内容主体的原始内容的 <see cref="Stream"/>。</returns>
        /// <exception cref="Exception">Body 不可用。</exception>
        public Stream ResponseBody { get { return Response.Body; } }

        /// <summary>
        /// 获取客户端发送的 Cookie 的集合。
        /// </summary>
        /// <returns>表示客户端的 Cookie 变量的 System.Web.HttpCookieCollection 对象。</returns>
        public IRequestCookieCollection Cookies { get { return Request.Cookies; } }

        /// <summary>
        /// 获取传入的 HTTP 实体主体的内容。
        /// </summary>
        /// <returns>表示传入的 HTTP 内容主体的内容的 System.IO.Stream 对象。</returns>
        public Stream RequestBody { get { return Request.Body; } }

        /// <summary>
        /// 获取当前输入流中的字节数。
        /// </summary>
        /// <returns>输入流中的字节数。</returns>
        public long TotalBytes { get { return Request.Body.Length; } }

        /// <summary>
        /// 获取客户端使用的 HTTP 数据传输方法（如 GET、POST 或 HEAD）。
        /// </summary>
        /// <returns>客户端使用的 HTTP 数据传输方法。</returns>
        public string HttpMethod { get { return Request.Method; } }

        /// <summary>
        /// 获取或设置传入请求的 MIME 内容类型。(ContentType)
        /// </summary>
        /// <returns>表示传入请求的 MIME 内容类型的字符串，例如，“text/html”。 其他常见 MIME 类型包括“audio.wav”、“image/gif”和“application/pdf”。</returns>
        public string ContentType { get { return Request.ContentType; } set { Request.ContentType = value; } }

        /// <summary>
        /// 指定客户端发送的内容长度（以字节计）。
        /// </summary>
        /// <returns>客户端发送的内容的长度（以字节为单位）。</returns>
        public long? ContentLength { get { return Request.ContentLength; } }

        /// <summary>
        /// 当链接真实有效时被执行，默认返回成功。（该方法是用于给使用者重写的）
        /// </summary>
        /// <param name="ashx">当前可以被调起的接口信息</param>
        /// <returns>当前请求会根据返回状态决定是否继续执行接口方法</returns>
        protected virtual bool Initialize(Ashx ashx)
        {
            return true;
        }

        /// <summary>
        /// 隐性实现
        /// </summary>
        /// <param name="ashx"></param>
        /// <returns></returns>
        bool IHttpApi.Initialize(Ashx ashx)
        {
            return Initialize(ashx);
        }

        /// <summary>
        /// 将客户端重定向到新的 URL。 指定新的 URL 并指定当前页的执行是否应终止。
        /// </summary>
        /// <param name="url">目标的位置。</param>
        /// <param name="endResponse">指示当前页的执行是否应终止。</param>
        /// <exception cref="System.ArgumentNullException">url 为 null。</exception>
        /// <exception cref="System.ArgumentException">url 包含换行符。</exception>
        /// <exception cref="Exception">在发送了 HTTP 头之后尝试重定向。</exception>
        /// <exception cref="System.ApplicationException">页请求是回调的结果。</exception>
        public void Redirect(string url, bool endResponse)
        {
            if (Response != null)
            {
                Response.Clear();
                Response.Redirect(url, endResponse);
            }
            else
            {
                throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
            }
        }

        /// <summary>
        /// 将请求重定向到新 URL 并指定该新 URL。
        /// </summary>
        /// <param name="url">目标位置。</param>
        /// <exception cref="Exception">在发送了 HTTP 头之后尝试重定向。</exception>
        public void Redirect(string url)
        {
            if (Response != null)
            {
                Response.Clear();
                Response.Redirect(url);
            }
            else
            {
                throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
            }
        }

        /// <summary>
        /// 设置输出类型
        /// </summary>
        /// <param name="contentType">类型</param>
        private void SetContentType(string contentType)
        {
            try
            {
                Response.ContentType = string.Concat(contentType, "; charset=utf-8");
            }
            catch (Exception e)
            {
                if (!e.Message.Equals("服务器无法在发送 HTTP 标头之后设置内容类型。"))
                {
                    throw new Exception("无法处理的异常！", e);
                }
            }

            //string IsFlush;
            //try
            //{
            //    IsFlush = response.Headers.Get("IsFlush");
            //}
            //catch (Exception e)
            //{
            //    if (!e.Message.Equals("服务器无法在发送 HTTP 标头之后设置内容类型。"))
            //    {
            //        throw e;
            //    }
            //    IsFlush = "false";
            //}

            //if (IsFlush != "true")
            //{
            //    response.ContentType = contentType;
            //}
        }

        /// <summary>
        /// Json 格式输出，将 System.Object 写入 HTTP 响应流。 
        /// </summary>
        /// <param name="obj">要写入 HTTP 输出流的 System.Object。</param>
        public void Json(object obj)
        {
            Json(obj, null);
            //if (Response != null)
            //{
            //    SetContentType("application/json");
            //    string json = string.Empty;
            //    if (obj != null)
            //    {
            //        if (obj.GetType() != typeof(string))
            //        {
            //            json = obj.ToJson();
            //        }
            //        else
            //        {
            //            json = obj.ToString();
            //        }
            //    }

            //    Response.Write(json);
            //}
            //else
            //{
            //    throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
            //}
        }

        /// <summary>
        /// Json 格式输出，将 System.Object 写入 HTTP 响应流。 
        /// </summary>
        /// <param name="obj">要写入 HTTP 输出流的 System.Object。</param>
        /// <param name="JsonOptions">Json 特殊格式输出</param>
        public void Json(object obj, System.Text.Json.JsonSerializerOptions JsonOptions)
        {
            JsonAsync(obj, JsonOptions).Wait();
        }

        /// <summary>
        /// Json 格式输出，将 System.Object 写入 HTTP 响应流。 
        /// </summary>
        /// <param name="obj">要写入 HTTP 输出流的 System.Object。</param>
        public async Task JsonAsync(object obj)
        {
            await JsonAsync(obj, null);
        }

        /// <summary>
        /// Json 格式输出，将 System.Object 写入 HTTP 响应流。 
        /// </summary>
        /// <param name="obj">要写入 HTTP 输出流的 System.Object。</param>
        /// <param name="JsonOptions">Json 特殊格式输出</param>
        public async Task JsonAsync(object obj, System.Text.Json.JsonSerializerOptions JsonOptions)
        {
            if (Response != null)
            {
                SetContentType("application/json");
                string json = string.Empty;
                if (obj != null)
                {
                    if (obj.GetType() != typeof(string))
                    {
                        json = obj.ToJson(JsonOptions ?? routeData.JsonOptions);
                    }
                    else
                    {
                        json = obj.ToString();
                    }
                }

                await Response.WriteAsync(json);
            }
            else
            {
                throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
            }
        }

        /// <summary>
        /// 将 System.Object 写入 HTTP 响应流。
        /// </summary>
        /// <param name="obj">要写入 HTTP 输出流的 System.Object。</param>
        public void Write(object obj)
        {
            WriteAsync(obj).Wait();
        }

        /// <summary>
        /// 将 System.Object 写入 HTTP 响应流。
        /// </summary>
        /// <param name="obj">要写入 HTTP 输出流的 System.Object。</param>
        public async Task WriteAsync(object obj)
        {
            if (Response != null)
            {
                await Response.WriteAsync(obj);
            }
            else
            {
                throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
            }
        }

        /// <summary>
        /// 将一个字符串写入 HTTP 响应输出流。
        /// </summary>
        /// <param name="test">要写入 HTTP 输出流的字符串。</param>
        public void Write(string test)
        {
            WriteAsync(test).Wait();
        }

        /// <summary>
        /// 将一个字符串写入 HTTP 响应输出流。
        /// </summary>
        /// <param name="test">要写入 HTTP 输出流的字符串。</param>
        public async Task WriteAsync(string test)
        {
            if (Response != null)
            {
                await Response.WriteAsync(test);
            }
            else
            {
                throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
            }
        }

        /// <summary>
        /// 将 System.Object 写入 HTTP 响应流。
        /// </summary>
        /// <param name="obj">要写入 HTTP 输出流的 System.Object。</param>
        /// <param name="contentType">返回客户端的类型</param>
        public void Write(object obj, WriteType contentType)
        {
            WriteAsync(obj, contentType).Wait();

            //if (Response != null)
            //{
            //    if (obj == null)
            //    {
            //        obj = string.Empty;
            //    }
            //    bool isstr = obj.GetType() == typeof(string);

            //    switch (contentType)
            //    {
            //        case WriteType.Html:
            //            SetContentType("text/html");
            //            Response.Write(obj.ToString());
            //            break;
            //        case WriteType.Json:
            //            SetContentType("application/json");
            //            Response.Write(isstr ? obj.ToString() : obj.ToJson());
            //            break;
            //        case WriteType.Xml:
            //            SetContentType("application/xml");
            //            Response.Write(obj.ToString());
            //            break;
            //        case WriteType.Text:
            //            SetContentType("text/plain");
            //            Response.Write(obj.ToString());
            //            break;
            //    }
            //}
            //else
            //{
            //    throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
            //}
        }

        /// <summary>
        /// 将 System.Object 写入 HTTP 响应流。
        /// </summary>
        /// <param name="obj">要写入 HTTP 输出流的 System.Object。</param>
        /// <param name="contentType">返回客户端的类型</param>
        public async Task WriteAsync(object obj, WriteType contentType)
        {
            if (Response != null)
            {
                if (obj == null)
                {
                    obj = string.Empty;
                }
                bool isstr = obj.GetType() == typeof(string);

                switch (contentType)
                {
                    case WriteType.Html:
                        SetContentType("text/html");
                        await Response.WriteAsync(obj.ToString());
                        break;
                    case WriteType.Json:
                        SetContentType("application/json");
                        await Response.WriteAsync(isstr ? obj.ToString() : obj.ToJson(routeData.JsonOptions));
                        break;
                    case WriteType.Xml:
                        SetContentType("application/xml");
                        await Response.WriteAsync(obj.ToString());
                        break;
                    case WriteType.Text:
                        SetContentType("text/plain");
                        await Response.WriteAsync(obj.ToString());
                        break;
                }
            }
            else
            {
                throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
            }
        }

        //

        ///// <summary>
        ///// 将指定文件的内容作为文件块直接写入 HTTP 响应输出流。
        ///// </summary>
        ///// <param name="filename">要写入 HTTP 输出的文件名。</param>
        ///// <exception cref="System.ArgumentNullException">filename 参数为 null。</exception>
        //public void WriteFile(string filename)
        //{
        //    if (Response != null)
        //    {
        //        Response.WriteFile(filename);
        //    }
        //    else
        //    {
        //        throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
        //    }
        //}

        ///// <summary>
        ///// 将指定文件的内容作为内存块直接写入 HTTP 响应输出流。
        ///// </summary>
        ///// <param name="filename">要写入内存块的文件名。</param>
        ///// <param name="readIntoMemory">指示是否将把文件写入内存块。</param>
        ///// <exception cref="System.ArgumentNullException">filename 参数为 null。</exception>
        //public void WriteFile(string filename, bool readIntoMemory)
        //{
        //    if (Response != null)
        //    {
        //        Response.WriteFile(filename, readIntoMemory);
        //    }
        //    else
        //    {
        //        throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
        //    }
        //}

        ///// <summary>
        ///// 将指定的文件直接写入 HTTP 响应输出流。
        ///// </summary>
        ///// <param name="filename">要写入 HTTP 输出流的文件名。</param>
        ///// <param name="offset">文件中将开始进行写入的字节位置。</param>
        ///// <param name="size">要写入输出流的字节数。</param>
        ///// <exception cref="System.Web.HttpException">offset 小于 0。 - 或 - size 大于文件大小减去 offset。</exception>
        ///// <exception cref="System.ArgumentNullException">filename 参数为 null。</exception>
        //public void WriteFile(string filename, long offset, long size)
        //{
        //    if (Response != null)
        //    {
        //        Response.WriteFile(filename, offset, size);
        //    }
        //    else
        //    {
        //        throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
        //    }
        //}

        ///// <summary>
        ///// 将指定的文件直接写入 HTTP 响应输出流。
        ///// </summary>
        ///// <param name="fileHandle">要写入 HTTP 输出流的文件的文件句柄。</param>
        ///// <param name="offset">文件中将开始进行写入的字节位置。</param>
        ///// <param name="size">要写入输出流的字节数。</param>
        ///// <exception cref="System.ArgumentNullException">fileHandler 为 null。</exception>
        ///// <exception cref="System.Web.HttpException">offset 小于 0。 - 或 - size 大于文件大小减去 offset。</exception>
        //public void WriteFile(IntPtr fileHandle, long offset, long size)
        //{
        //    if (Response != null)
        //    {
        //        Response.WriteFile(fileHandle, offset, size);
        //    }
        //    else
        //    {
        //        throw new System.Exception("请在{Ashx注入后}调用方法，使用。");
        //    }
        //}


        void IHttpApi.SetRouteData(AshxRouteData ashxRoute)
        {
            this.routeData = ashxRoute;

            //this.context = routeData.HttpContext;

            //this.session = context.Session;

            //this.request = context.Request;

            //this.response = context.Response;
        }


        //AshxRouteData IHttpApi.RouteData => throw new NotImplementedException();

        //string IHttpApi.ApiKey => throw new NotImplementedException();


        /// <summary>
        /// 同步请求创建（开始）
        /// </summary>
        /// <param name="_objs">源数据</param>
        void IHttpApi.Request(object[] _objs)
        {
            Ashx ashx = this.RouteData.GetAshx;
            try
            {
                void action() => ashx.Action.VoidExecute(this, _objs); //AshxExtension.Invoke(ashx.Method, this);

                action();

                OnResult(ashx);
            }
            catch (ThreadAbortException)
            {
                // This type of exception occurs as a result of Response.Redirect(), but we special-case so that
                // the filters don't see this as an error.
                throw;
            }
            catch (Exception ex)
            {
                AshxException exception = new(ashx, ex, _objs) { ExceptionHandled = true };
                AshxException(exception);
                IsException(exception);
            }
            //context.ApplicationInstance.CompleteRequest();
        }

        async Task IHttpAsynApi.TaskRequest(object[] _objs)
        {
            Ashx ashx = this.RouteData.GetAshx;
            try
            {
                if (ashx.IsOnAshxEvent)
                {
                    //OnAshxEvent onAshxEvent = ashx.Action.Execute(this, _objs) as OnAshxEvent;

                    OnAshxEvent func() => ashx.Action.Execute(this, _objs) as OnAshxEvent;
                    OnAshxEvent onAshxEvent = func();
                    await RequestAsyncEvent(onAshxEvent);
                }
                else
                {
                    //Func<Task> func = () => { return ashx.Action.Execute(this, _objs) as Task; };

                    async Task func() => await (ashx.Action.Execute(this, _objs) as Task); //ThreadLocal AsyncLocal

                    await func(); //AshxExtension.Invoke(ashx.Method, this, _objs as object[]) as Task; //
                }

                OnResult(ashx);
            }
            //catch (ThreadAbortException)
            //{
            //    // This type of exception occurs as a result of Response.Redirect(), but we special-case so that
            //    // the filters don't see this as an error.
            //    throw;
            //}
            //catch (AshxException ex)
            //{
            //    AshxException(new AshxException(ashx, ex) { ExceptionHandled = true });
            //}
            catch (Exception ex)
            {
                AshxException exception = new(ashx, ex, _objs) { ExceptionHandled = true };
                AshxException(exception);
                await IsTaskException(exception);
            }
        }

        private async Task RequestAsyncEvent(OnAshxEvent onAshxEvent)
        {
            if (onAshxEvent.IsFlush)
            {
                //response.Headers.Add("IsFlush", "true");
                this.Response.ContentType = onAshxEvent.ContentType;
                //this.response.HeaderEncoding = Encoding.UTF8;
                //this.response.Charset = "utf-8";
                await this.Response.FlushAsync();
            }
            //else
            //{
            //    response.Headers.Add("IsFlush", "false");
            //}

            if (StaticData.AshxEvents.TryRemove(onAshxEvent.GuId, out OnAshxEvent onAshxEvent1))
            {
                onAshxEvent1.OnAshx = OnAshxEventState.OnlyID;
                onAshxEvent1.ManualReset?.Set();
            }

            StaticData.AshxEvents.TryAdd(onAshxEvent.GuId, onAshxEvent);

            Task task = Task.Run(() =>
            {
                onAshxEvent.ManualReset = new ManualResetEvent(false);

                if (!onAshxEvent.ManualReset.WaitOne(onAshxEvent.DelayTime))
                {
                    StaticData.AshxEvents.TryRemove(onAshxEvent.GuId, out _);
                    onAshxEvent.OnAshx = OnAshxEventState.Timeout;
                }

                using (onAshxEvent)
                {
                    onAshxEvent.Revive();
                }
            });

            await task;
        }

        private static void IsException(AshxException ex)
        {
            if (!ex.ExceptionHandled)
            {
                throw ex;
            }
        }

        private static async Task IsTaskException(AshxException ex)
        {
            if (ex.ExceptionHandled)
            {
                await Task.CompletedTask;
            }
            else
            {
                await Task.FromException(ex);
            }
        }

        /// <summary>
        /// 当前API接口发生异常时触发
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        protected virtual void AshxException(AshxException ex)
        {
            ex.ExceptionHandled = false;
            //response.Clear();
            //Context.AddError(new HttpException(500, "接口异常。", ex));
            //throw new HttpException(500, "接口异常。", ex);
            //throw ex;
        }

        /// <summary>
        /// 在请求正常完成结束时触发
        /// </summary>
        /// <param name="ashx">获取接口信息</param>
        /// <returns></returns>
        protected virtual void OnResult(Ashx ashx)
        {

        }

        void IHttpAsynApi.ContinueWith(Task task)
        {
            if (task.IsFaulted)
            {
                if (task.Exception.GetBaseException() is AshxException ashxException && !ashxException.ExceptionHandled) throw ashxException;

                //AshxException(new AshxException(RouteData.GetAshx, new Exception("发生异常，异步处理过程中发生异常。", task.Exception.GetBaseException())) { ExceptionHandled = true });
            }
            if (!task.IsCompleted)
            {
                throw new Exception("发生异常，异步处理未完成。"); //AshxException(new AshxException(RouteData.GetAshx, new Exception("发生异常，异步处理未完成。")) { ExceptionHandled = true });
            }
        }

        //IAsyncResult IHttpAsynApi.BeginRequest(AsyncCallback cb, object extraData)
        //{
        //    return TaskAsyncHelper.BeginTask(() => this.RequestAsync(), cb, extraData);
        //}

        //void IHttpAsynApi.EndRequest(IAsyncResult result)
        //{
        //    TaskWrapperAsyncResult taskWrapperAsync = result as TaskWrapperAsyncResult;

        //    if (taskWrapperAsync.Task.IsFaulted)
        //    {
        //        AshxException(new AshxException(RouteData.GetAshx, new Exception("发生异常，异步处理过程中发生异常。", taskWrapperAsync.Task.Exception.GetBaseException())) { ExceptionHandled = true });
        //    }
        //    if (!taskWrapperAsync.Task.IsCompleted)
        //    {
        //        AshxException(new AshxException(RouteData.GetAshx, new Exception("发生异常，异步处理未完成。")) { ExceptionHandled = true });
        //    }

        //    //context.ApplicationInstance.CompleteRequest();
        //    TaskAsyncHelper.EndTask(result);
        //}

        void IDisposable.Dispose()
        {
            Dispose();
            GC.SuppressFinalize(this);
        }

        private void Dispose()
        {
            routeData = null;
        }
    }
}
