using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;
using Tool.Web.Api.ApiCore;
using Tool.Web.Routing;
using System.Threading;
using Tool.Utils;

namespace Tool.Web.Api
{
    /// <summary>
    /// 针对于新版Ashx路由模式，的同步，异步，支持（此Api为最轻量级，请严格遵循实现写法）
    /// <para>给你一个快的理由，因为该路由接口类在启动路由时就已经创建，中途调用，无需创建新的实例，达到最大性能优化。</para>
    /// <para>同时请注意您这个Api类对象的生命周期，将伴随着整个Web应用程序一致，也就意味着类中的所有对象将不是安全的，请合理声明类变量使用。</para>
    /// <example>   Api 方法创建示例：
    /// <code>
    ///   public <see cref="IApiOut"/> GetApi(<see cref="HttpContext"/> context)  => ApiOut.Json(new { msg = "最小，路由版本api。" });
    /// </code>
    /// </example>
    /// <example>Api 方法创建示例：(异步实现)
    /// <code>
    ///   public async <see cref="Task{IApiOut}"/> GetTaskApi(<see cref="HttpContext"/> context) => await ApiOut.JsonAsync(new { msg = "最小，路由版本api。" });
    /// </code>
    /// </example>
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class MinApi : IMinHttpApi, IMinHttpAsynApi, IAshxAction
    {
        /// <summary>
        /// 当链接真实有效时被执行，默认返回成功。（该方法是用于给使用者重写的）
        /// </summary>
        /// <param name="ashxRoute">当前请求的<see cref="AshxRouteData"/>路由，包含全部详情信息</param>
        /// <returns>返回输出结果，当为null的时候，表示继续执行，不为空执行输出结果。</returns>
        protected virtual IApiOut Initialize(AshxRouteData ashxRoute)//, out IApiOut aipOut        /// <param name="aipOut">用于在返回false，输出结果的对象</param>当前请求会根据返回状态决定是否继续执行接口方法
        {
            //aipOut = null;
            return null;
        }

        /// <summary>
        /// 当前API接口发生异常时触发
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <returns></returns>
        protected virtual IApiOut AshxException(AshxException ex)
        {
            ex.ExceptionHandled = false;
            //response.Clear();
            //Context.AddError(new HttpException(500, "接口异常。", ex));
            //throw new HttpException(500, "接口异常。", ex);
            return null;
        }

        /// <summary>
        /// 在请求正常完成结束时触发
        /// </summary>
        /// <param name="ashxRoute">获取接口信息</param>
        /// <returns></returns>
        protected virtual void OnResult(AshxRouteData ashxRoute)
        {

        }

        private static void IsException(AshxException ex, AshxRouteData ashxRoute, IApiOut aipOut)
        {
            if (ex.ExceptionHandled)
            {
                aipOut?.HttpOutput(ashxRoute).Wait();
            }
            else
            {
                throw ex;
            }
        }

        private static async Task IsTaskException(AshxException ex, AshxRouteData ashxRoute, IApiOut aipOut)
        {
            if (ex.ExceptionHandled)
            {
                if (aipOut == null)
                {
                    await Task.CompletedTask;
                }
                else
                {
                    await aipOut?.HttpOutput(ashxRoute);//await Task.CompletedTask;
                }
            }
            else
            {
                await Task.FromException(ex);
            }
        }

        bool IMinHttpApi.Initialize(AshxRouteData ashxRoute)
        {
            IApiOut aipOut;
            bool isapi = (aipOut = Initialize(ashxRoute)) == null;//Initialize(ashxRoute, out IApiOut aipOut);
            if (!isapi)
            {
                aipOut?.HttpOutput(ashxRoute).Wait();
            }
            return isapi;//(ashxRoute.HttpContext, ashxRoute.GetAshx);
        }

        void IMinHttpApi.Request(AshxRouteData ashxRoute, object[] _objs)
        {
            Ashx ashx = ashxRoute.GetAshx;
            try
            {
                Func<IApiOut> func = () => { return ashx.Action.Execute(this, _objs) as IApiOut; }; //AshxExtension.Invoke(ashx.Method, this);

                IApiOut aipOut = func();

                aipOut?.HttpOutput(ashxRoute).Wait();

                OnResult(ashxRoute);
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
                IApiOut aipOut = AshxException(exception);
                IsException(exception, ashxRoute, aipOut);
            }
        }

        async Task IMinHttpAsynApi.TaskRequest(AshxRouteData ashxRoute, object[] _objs)
        {
            Ashx ashx = ashxRoute.GetAshx;
            try
            {
                //Func<Task<IApiOut>> func = () => { return ashx.Action.Execute(this, _objs) as Task<IApiOut>; };
                //IApiOut aipOut = await func();

                //System.Runtime.CompilerServices.AsyncTaskMethodBuilder
                //System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object> asyncTaskMethodBuilder = System.Runtime.CompilerServices.AsyncTaskMethodBuilder<object>.Create();
                //Task task = func(); //AsyncStateMachineBox
                //Utils.Data.DictionaryExtension.ToDictionary(task);
                //Type type = task.GetType();
                //var field = type.GetType().GetField("Result", System.Reflection.BindingFlags.Public);

                async Task<IApiOut> func() => await (Task<IApiOut>)ashx.Action.Execute(this, _objs);
                //{
                //    return await (Task<IApiOut>)ashx.Action.Execute(this, _objs);

                //    //Task <IApiOut> task = ashx.Action.Execute(this, _objs) as Task<IApiOut>;
                //    //return await task;
                //} //Task

                //object _task = func();// dynamic，Task

                //Task.WaitAll<object>((func() as Task<object>));

                //for (int i = 0; i < 1000000; i++)
                //{
                //    IApiOut aipOut1 = _task.GetValue("Result") as IApiOut; //aipOut1.Result as IApiOut;
                //}

                //var _r = _task.GetPropertieFind("Result");
                //for (int i = 0; i < 1000000; i++)
                //{
                //    IApiOut aipOut1 = _task.GetValue(_r) as IApiOut; //aipOut1.Result as IApiOut;
                //}

                //Task<object> s = Task.FromResult<object>(100);

                //for (int i = 0; i < 100000000; i++)
                //{
                //    object aipOut1 = s.Result; //aipOut1.Result as IApiOut;
                //}

                //dynamic d = _task;
                //for (int i = 0; i < 100000000; i++)
                //{
                //    IApiOut aipOut1 = d.Result; //aipOut1.Result as IApiOut;
                //}

                IApiOut aipOut = await func();//_task.GetValue("Result") as IApiOut; //aipOut1.Result as IApiOut;
                await aipOut?.HttpOutput(ashxRoute);

                OnResult(ashxRoute);
            }
            //catch (ThreadAbortException ex)
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
                //AshxException(new AshxException(ashx, ex) { ExceptionHandled = true });
                AshxException exception = new(ashx, ex, _objs) { ExceptionHandled = true };
                IApiOut aipOut = AshxException(exception);
                await IsTaskException(exception, ashxRoute, aipOut);
            }

            //return await Task<IAipOut>.FromResult<IAipOut>(default);
        }

        void IMinHttpAsynApi.ContinueWith(Task task, AshxRouteData ashxRoute)
        {
            if (task.IsFaulted)
            {
                if (task.Exception.GetBaseException() is AshxException ashxException && !ashxException.ExceptionHandled) throw ashxException;

                //AshxException ashxException = task.Exception.GetBaseException() as AshxException;
                //if (ashxException != null && !ashxException.ExceptionHandled) throw ashxException;
                //AshxException(new AshxException(ashxRoute.GetAshx, new Exception("发生异常，异步处理过程中发生异常。", task.Exception.GetBaseException())) { ExceptionHandled = true });
            }
            else if (!task.IsCompleted)
            {
                throw new Exception("发生异常，异步处理未完成。"); //AshxException(new AshxException(ashxRoute.GetAshx, ) { ExceptionHandled = true });
            }
            //else
            //{
            //    IAipOut aipOut = task.Result;
            //    aipOut?.HttpOutput(ashxRoute.HttpContext).Wait();
            //}
        }
    }

    /// <summary>
    /// <see cref="MinApi"/> Api 返回结果接口，用于实现各种返回输出
    /// </summary>
    public interface IApiOut
    {
        ///// <summary>
        ///// 输出类型
        ///// </summary>
        //string ContentType { get; set; }

        ///// <summary>
        ///// HTTP 返回 Code
        ///// </summary>
        //int StatusCode { get; set; }

        /// <summary>
        /// 系统回调，获取输出结果函数
        /// </summary>
        /// <param name="ashxRoute">包含所有有效信息</param>
        /// <returns></returns>
        Task HttpOutput(AshxRouteData ashxRoute);
    }

    /// <summary>
    /// 系统默认 <see cref="MinApi"/> Api输出结果 抽象类，用于普通返回值，特殊返回值建议您自己实现。
    /// </summary>
    public abstract class ApiOut : IApiOut
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        protected ApiOut()
        {

        }

        ///// <summary>
        ///// 输出类型
        ///// </summary>
        //public WriteType ContentType { get; set; }

        ///// <summary>
        ///// 输出结果数据
        ///// </summary>
        //public abstract byte[] Bytes { get; set; }

        /// <summary>
        /// 输出类型
        /// </summary>
        public abstract string ContentType { get; set; }

        /// <summary>
        /// HTTP 返回 Code
        /// </summary>
        public abstract int StatusCode { get; set; }

        /// <summary>
        /// 系统回调，用于完成该请求任务的输出
        /// </summary>
        /// <param name="ashxRoute">包含所有有效信息</param>
        /// <returns>异步任务</returns>
        public abstract Task ExecuteOutAsync(AshxRouteData ashxRoute);

        //public abstract void HttpOutput(AshxRouteData ashxRoute);

        async Task IApiOut.HttpOutput(AshxRouteData ashxRoute)
        {
            ashxRoute.HttpContext.Response.StatusCode = StatusCode;
            ashxRoute.HttpContext.Response.ContentType = string.Concat(ContentType, "; charset=utf-8");
            await ExecuteOutAsync(ashxRoute);
            //HttpContext context = ashxRoute.HttpContext;
            //context.Response.StatusCode = StatusCode; //Microsoft.AspNetCore.Http.DefaultHttpContext
            ////byte[] bytes;
            ////string contentType;
            ////switch (ContentType)
            ////{
            ////    case WriteType.Html:
            ////        contentType = "text/html";
            ////        //bytes = Data.ToString().ToBytes(Encoding.UTF8);
            ////        break;
            ////    case WriteType.Json:
            ////        contentType = "application/json";
            ////        //bytes = Data.ToJson().ToBytes(Encoding.UTF8);
            ////        break;
            ////    case WriteType.Xml:
            ////        contentType = "application/xml";
            ////        //bytes = Data.ToString().ToBytes(Encoding.UTF8);
            ////        break;
            ////    case WriteType.Text:
            ////        contentType = "text/plain";
            ////        //bytes = Data.ToString().ToBytes(Encoding.UTF8);
            ////        break;
            ////    default:
            ////        throw new Exception("出现意外输出类型！终止返回！");
            ////}
            //string contentType = ContentType switch
            //{
            //    WriteType.Html => "text/html",
            //    WriteType.Json => "application/json",
            //    WriteType.Xml => "application/xml",
            //    WriteType.Text => "text/plain",
            //    _ => throw new Exception("出现意外输出类型！终止返回！"),
            //};
            //context.Response.ContentType = string.Concat(contentType, ";charset=utf-8");
            //await context.Response.WriteAsync(Bytes, 0, Bytes.Length);
        }

        /// <summary>
        /// 向客户端返回JSON数据
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <returns>输出对象</returns>
        public static JsonOut Json(object obj)
        {
            return new JsonOut(obj);// { Bytes = obj.ToJson().ToBytes(Encoding.UTF8), StatusCode = 200, ContentType = WriteType.Json };
        }

        /// <summary>
        /// 异步向客户端返回JSON数据
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <returns>输出对象</returns>
        public static async Task<JsonOut> JsonAsync(object obj)
        {
            return await Task.FromResult(Json(obj));
        }

        /// <summary>
        /// 向客户端返回JSON数据
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="JsonOptions">Json 转换条件</param>
        /// <returns>输出对象</returns>
        public static JsonOut Json(object obj, System.Text.Json.JsonSerializerOptions JsonOptions)
        {
            return new JsonOut(obj, JsonOptions);// { Bytes = obj.ToJson().ToBytes(Encoding.UTF8), StatusCode = 200, ContentType = WriteType.Json };
        }

        /// <summary>
        /// 异步向客户端返回JSON数据
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <param name="JsonOptions">Json 转换条件</param>
        /// <returns>输出对象</returns>
        public static async Task<JsonOut> JsonAsync(object obj, System.Text.Json.JsonSerializerOptions JsonOptions)
        {
            return await Task.FromResult(Json(obj, JsonOptions));
        }

        /// <summary>
        /// 向客户端返回Text数据
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <returns>输出对象</returns>
        public static WriteOut Write(object obj)
        {
            return new WriteOut(obj.ToString());// { Bytes = obj.ToString().ToBytes(Encoding.UTF8), StatusCode = 200, ContentType = WriteType.Text };
        }

        /// <summary>
        /// 异步向客户端返回Text数据
        /// </summary>
        /// <param name="obj">源数据</param>
        /// <returns>输出对象</returns>
        public static async Task<WriteOut> WriteAsync(object obj)
        {
            return await Task.FromResult(Write(obj));
        }

        /// <summary>
        /// 向客户端返回页面（默认模式路径：wwwroot文件夹下，加上控制器名和接口名：/Views/Api/Get.html）
        /// </summary>
        /// <returns>输出对象</returns>
        public static ViewOut View()
        {
            //string filePath;
            //string directory = AppContext.BaseDirectory;
            //filePath = directory + "ApiView\\";

            return new ViewOut(true, null);// View(filePath);
        }

        /// <summary>
        /// 向客户端返回页面
        /// </summary>
        /// <param name="viewName">源数据(是wwwroot文件夹下面的相对路径)，不支持绝对路径</param>
        /// <returns>输出对象</returns>
        public static ViewOut View(string viewName)
        {
            return new ViewOut(false, viewName);// View(filePath);
            //string filePath;
            //if (System.IO.File.Exists(viewName))
            //{
            //    filePath = viewName;
            //}
            //else
            //{
            //    string directory = AppContext.BaseDirectory.Replace("\\", "/");
            //    filePath = $"{directory}/{viewName}";
            //}
            //if (System.IO.File.Exists(filePath))
            //{
            //    byte[] obj = System.IO.File.ReadAllBytes(filePath);
            //    return new ApiOut() { Bytes = obj, StatusCode = 200, ContentType = WriteType.Html };
            //}
            //throw new Exception("找不到页面:“" + viewName + "”,无法显示！");
        }

        /// <summary>
        /// 向客户端返回页面
        /// </summary>
        /// <param name="pathName">源数据(是wwwroot文件夹下面的相对路径)，不支持绝对路径 的文件夹名称 支持多重文件 不能包含文件</param>
        /// <returns>输出对象</returns>
        public static ViewOut PathView(string pathName) 
        {
            return new ViewOut(true, pathName);
        }

        /// <summary>
        /// 异步向客户端返回页面（默认模式路径：ApiView文件夹下，加上控制器名和接口名：/Views/Api/Get.html）
        /// </summary>
        /// <returns>输出对象</returns>
        public static async Task<ViewOut> ViewAsync()
        {
            return await Task.FromResult(View());
        }

        /// <summary>
        /// 异步向客户端返回页面
        /// </summary>
        /// <param name="viewName">源数据(是wwwroot文件夹下面的相对路径)，不支持绝对路径</param>
        /// <returns>输出对象</returns>
        public static async Task<ViewOut> ViewAsync(string viewName)
        {
            return await Task.FromResult(View(viewName));
        }

        /// <summary>
        /// 异步向客户端返回页面
        /// </summary>
        /// <param name="pathName">源数据(是wwwroot文件夹下面的相对路径)，不支持绝对路径 的文件夹名称 支持多重文件 不能包含文件</param>
        /// <returns>输出对象</returns>
        public static async Task<ViewOut> PathViewAsync(string pathName)
        {
            return await Task.FromResult(PathView(pathName));
        }

        /// <summary>
        /// 向客户端返回下载的资源文件
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <param name="fileStream">文件流对象</param>
        /// <returns>输出对象</returns>
        public static FileOut File(string name, System.IO.Stream fileStream)
        {
            return new FileOut(name, fileStream);
        }

        /// <summary>
        /// 异步向客户端返回下载的资源文件
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <param name="fileStream">文件流对象</param>
        /// <returns>输出对象</returns>
        public static async Task<FileOut> FileAsync(string name, System.IO.Stream fileStream)
        {
            return await Task.FromResult(File(name, fileStream));
        }

        /// <summary>
        /// 向客户端返回下载的资源文件
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <param name="bytes">文件字节流</param>
        /// <returns>输出对象</returns>
        public static FileOut File(string name, byte[] bytes)
        {
            return new FileOut(name, bytes);
        }

        /// <summary>
        /// 异步向客户端返回下载的资源文件
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <param name="bytes">文件字节流</param>
        /// <returns>输出对象</returns>
        public static async Task<FileOut> FileAsync(string name, byte[] bytes)
        {
            return await Task.FromResult(File(name, bytes));
        }

        /// <summary>
        /// 向客户端返回重定向的URl
        /// </summary>
        /// <param name="url">跳转的URL</param>
        /// <returns>输出对象</returns>
        public static RedirectOut Redirect(string url)
        {
            return new RedirectOut(url);
        }

        /// <summary>
        /// 异步向客户端返回重定向的URl
        /// </summary>
        /// <param name="url">跳转的URL</param>
        /// <returns>输出对象</returns>
        public static async Task<RedirectOut> RedirectAsync(string url)
        {
            return await Task.FromResult(Redirect(url));
        }

    }

    /// <summary>
    /// 系统默认 <see cref="ApiOut"/> 输出对象的实现类，JSON格式处理
    /// </summary>
    public class JsonOut : ApiOut
    {
        /// <summary>
        /// 创建Json输出对象
        /// </summary>
        /// <param name="data">可被序列化的数据源</param>
        public JsonOut(object data)
        {
            Data = data;
            ContentType = "application/json";
            StatusCode = 200;
        }

        /// <summary>
        /// 创建Json输出对象
        /// </summary>
        /// <param name="data">可被序列化的数据源</param>
        /// <param name="jsonOptions">Json 转换条件</param>
        public JsonOut(object data, System.Text.Json.JsonSerializerOptions jsonOptions) : this(data)
        {
            JsonOptions = jsonOptions;
        }

        /// <summary>
        /// 输出结果数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// Json 转换条件
        /// </summary>
        public System.Text.Json.JsonSerializerOptions JsonOptions { get; set; }

        /// <summary>
        /// 输出类型
        /// </summary>
        public override string ContentType { get; set; }

        /// <summary>
        /// HTTP 返回 Code
        /// </summary>
        public override int StatusCode { get; set; }

        /// <summary>
        /// 实现JSON格式的输出
        /// </summary>
        /// <param name="ashxRoute">当前请求对象</param>
        /// <returns></returns>
        public override async Task ExecuteOutAsync(AshxRouteData ashxRoute)
        {
            string json;
            if (Data.GetType() == typeof(string))
            {
                json = Data.ToString();
            }
            else
            {
                json = Data.ToJson(JsonOptions ?? ashxRoute.JsonOptions);
            }
            byte[] bytes = json.ToBytes(Encoding.UTF8);
            await ashxRoute.HttpContext.Response.WriteAsync(bytes);
        }
    }

    /// <summary>
    /// 系统默认 <see cref="ApiOut"/> 输出对象的实现类，文本格式处理
    /// </summary>
    public class WriteOut : ApiOut
    {
        /// <summary>
        /// 向客户端输出文本数据
        /// </summary>
        /// <param name="text">文本内容</param>
        public WriteOut(string text)
        {
            Text = text;
            ContentType = "text/plain";
            StatusCode = 200;
        }

        /// <summary>
        /// 输出结果数据
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 输出类型
        /// </summary>
        public override string ContentType { get; set; }

        /// <summary>
        /// HTTP 返回 Code
        /// </summary>
        public override int StatusCode { get; set; }

        /// <summary>
        /// 实现文本格式的输出
        /// </summary>
        /// <param name="ashxRoute">当前请求对象</param>
        /// <returns></returns>
        public override async Task ExecuteOutAsync(AshxRouteData ashxRoute)
        {
            await ashxRoute.HttpContext.Response.WriteAsync(Text);
        }
    }

    /// <summary>
    /// 系统默认 <see cref="ApiOut"/> 输出对象的实现类，视图页面输出处理
    /// </summary>
    public class ViewOut : ApiOut
    {
        /// <summary>
        /// 创建输出视图的实现类
        /// </summary>
        /// <param name="isView">是否使用系统格式视图</param>
        /// <param name="viewName">路径或名字</param>
        public ViewOut(bool isView, string viewName)
        {
            if (viewName is not null)
            {
                //System.IO.Path.GetFullPath
                if (viewName.Length > 1 && viewName[1].Equals(':'))
                {
                    throw new Exception("viewName结果必须是相对路径的值。");
                }
            }
            IsView = isView;
            ViewName = viewName;
            ContentType = "text/html";
            StatusCode = 200;
        }

        /// <summary>
        /// 表示地址
        /// </summary>
        public string ViewName { get; set; }

        /// <summary>
        /// 是否采用默认定位地址模式？
        /// </summary>
        public bool IsView { get; set; }

        /// <summary>
        /// 输出类型
        /// </summary>
        public override string ContentType { get; set; }

        /// <summary>
        /// HTTP 返回 Code
        /// </summary>
        public override int StatusCode { get; set; }

        /// <summary>
        /// 实现页面内容的输出（采用异步IO读取）
        /// </summary>
        /// <param name="ashxRoute">当前请求对象</param>
        /// <returns></returns>
        public override async Task ExecuteOutAsync(AshxRouteData ashxRoute)
        {
            Microsoft.AspNetCore.Hosting.IWebHostEnvironment env = ashxRoute.HttpContext.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
            Microsoft.Extensions.FileProviders.IFileInfo view;

            if (env.WebRootPath is null)
            {
                string webRootPath = env.ContentRootPath + "\\wwwroot";
                System.IO.Directory.CreateDirectory(webRootPath + "\\Views");
                env.WebRootPath = webRootPath;
                env.WebRootFileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(webRootPath);
            }

            //string filePath = "";
            if (IsView)
            {
                //string directory = AppContext.BaseDirectory;
                //filePath = string.Concat(directory, "ApiView\\", ashxRoute.Controller, '\\', ashxRoute.Action, ".html");

                if (string.IsNullOrEmpty(ViewName))
                {
                    ViewName = $"Views\\{ashxRoute.Controller}\\{ashxRoute.Action}.html";
                }
                else
                {
                    ViewName = $"Views\\{ashxRoute.Controller}\\{ViewName}\\{ashxRoute.Action}.html";
                }

                view = env.WebRootFileProvider.GetFileInfo(ViewName);
            }
            else
            {
                view = env.WebRootFileProvider.GetFileInfo(ViewName);

                //if (ViewName != null && ViewName[1].Equals(':'))
                //{
                //    filePath = ViewName;
                //}
                //else
                //{
                //    string directory = AppContext.BaseDirectory;
                //    filePath = string.Concat(directory, ViewName);// '\\' $"{directory}/{ViewName}";
                //}

                //System.IO.File.Exists(@"D:\Nixue工作室\Tool.Net\WebTestApp\wwwroot\"+ViewName);

                //System.IO.File.OpenRead(@"D:\Nixue工作室\Tool.Net\WebTestApp\wwwroot\" + ViewName);

            }
            //if (System.IO.File.Exists(filePath))
            //{
            //    filePath = viewName;
            //}
            //else
            //{
            //    string directory = AppContext.BaseDirectory.Replace("\\", "/");
            //    filePath = $"{directory}/{viewName}";
            //}
            if (view.Exists)
            {
                //byte[] obj = System.IO.File.ReadAllBytes(filePath);
                //byte[] obj = await System.IO.File.ReadAllBytesAsync(filePath);
                //await ashxRoute.HttpContext.Response.WriteAsync(obj);

                System.IO.Stream streamview = view.CreateReadStream();
                System.IO.Stream writeStream = ashxRoute.HttpContext.Response.Body;

                //long len = (1024 * 50) > view.Length ? view.Length : (1024 * 50);

                //byte[] slice = new byte[len];

                //long seekiength = 0;
                //do
                //{
                //    int i = await streamview.ReadAsync(slice.AsMemory(0, slice.Length));
                //    await writeStream.WriteAsync(slice.AsMemory(0, i));
                //    await writeStream.FlushAsync();
                //    seekiength += i;
                //    Array.Clear(slice, 0, i);
                //} while (view.Length > seekiength);

                //await streamview.DisposeAsync();

                await HttpContextExtension.StreamMove(streamview, writeStream, 1024 * 50);
            }
            else
            {
                throw new Exception("找不到页面:“(相对路径)wwwroot\\" + ViewName + "”,无法显示！");
            }
        }
    }

    /// <summary>
    /// 系统默认 <see cref="ApiOut"/> 输出对象的实现类，文件输出处理
    /// </summary>
    public class FileOut : ApiOut
    {
        /// <summary>
        /// 资源流对象
        /// </summary>
        public System.IO.Stream FileStream { get; set; }

        /// <summary>
        /// 下载文件名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 下载的文件格式
        /// </summary>
        public override string ContentType { get; set; } = "application/octet-stream";

        /// <summary>
        /// 下载的状态码
        /// </summary>
        public override int StatusCode { get; set; } = 200;

        /// <summary>
        /// 初始化构造
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <param name="fileStream">文件流对象</param>
        public FileOut(string name, System.IO.Stream fileStream)
        {
            this.Name = name;
            this.FileStream = fileStream;
        }

        /// <summary>
        /// 初始化构造
        /// </summary>
        /// <param name="name">文件名称</param>
        /// <param name="bytes">文件字节流</param>
        public FileOut(string name, byte[] bytes)
        {
            this.Name = name;
            this.FileStream = new System.IO.MemoryStream(bytes, false); // fileStream;
        }

        /// <summary>
        /// 下载核心任务对象
        /// </summary>
        /// <param name="ashxRoute">核心传输对象</param>
        /// <returns>返回任务</returns>
        public override async Task ExecuteOutAsync(AshxRouteData ashxRoute)
        {
            ashxRoute.HttpContext.Response.Headers.Add("Connection", "Keep-Alive");
            ashxRoute.HttpContext.Response.Headers.Add("Content-Length", FileStream.Length.ToString());
            ashxRoute.HttpContext.Response.Headers.Add("Content-Disposition", "attachment;filename=" + Name);

            //await ashxRoute.HttpContext.Response.Body.WriteAsync(Body.AsMemory(0, Body.Length));

            //long len = (1024 * 512) > FileStream.Length ? FileStream.Length : (1024 * 512);

            //byte[] slice = new byte[1024 * 512];
            //byte[] slice = new byte[len];

            //long seekiength = 0;
            //do
            //{
            //    int i = await FileStream.ReadAsync(slice.AsMemory(0, slice.Length));
            //    await writeStream.WriteAsync(slice.AsMemory(0, i));
            //    await writeStream.FlushAsync();
            //    seekiength += i;
            //    Array.Clear(slice, 0, i);
            //} while (FileStream.Length > seekiength);

            //await FileStream.DisposeAsync();

            System.IO.Stream writeStream = ashxRoute.HttpContext.Response.Body;

            await HttpContextExtension.StreamMove(FileStream, writeStream, 1024 * 512);
        }
    }

    /// <summary>
    /// 系统默认 <see cref="ApiOut"/> 跳转地址的实现类，跳转地址302
    /// </summary>
    public class RedirectOut : ApiOut
    {
        /// <summary>
        /// 目标位置
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 重定向表头
        /// </summary>
        public override string ContentType { get; set; } = "application/x-www-form-urlencoded";

        /// <summary>
        /// 重定向状态码
        /// </summary>
        public override int StatusCode { get; set; } = 302;

        /// <summary>
        /// 初始化构造
        /// </summary>
        /// <param name="url">目标位置</param>
        public RedirectOut(string url)
        {
            this.Url = url;
        }

        /// <summary>
        /// 重定向核心任务函数
        /// </summary>
        /// <param name="ashxRoute">核心传输对象</param>
        /// <returns>返回任务</returns>
        public override async Task ExecuteOutAsync(AshxRouteData ashxRoute)
        {
            ashxRoute.HttpContext.Response.Redirect(Url);

            await Task.CompletedTask;
        }
    }
}
