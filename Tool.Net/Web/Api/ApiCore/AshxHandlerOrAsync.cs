using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tool.Web.Routing;

namespace Tool.Web.Api.ApiCore
{
    /// <summary>
    /// 同步Ashx的请求实现或异步Ashx的请求实现
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class AshxHandlerOrAsync
    {
        /// <summary>
        /// 当前Ashx版本号
        /// </summary>
        public const string AshxVersion = "3.9.0"; // static readonly 

        /// <summary>
        /// Ashx路由模式的表头 同步
        /// </summary>
        public const string AshxVersionHeaderName = "X-AshxApi-Version"; // static readonly 

        /// <summary>
        /// Ashx路由模式的表头 异步
        /// </summary>
        public const string AshxVersionHeaderAsyncName = "X-AshxApi-Async-Version";

        /// <summary>
        /// Ashx路由模式的表头 同步（极小）
        /// </summary>
        public const string MinAshxVersionHeaderName = "X-MinApi-Version"; // static readonly 

        /// <summary>
        /// Ashx路由模式的表头 异步 （极小）
        /// </summary>
        public const string MinAshxVersionHeaderAsyncName = "X-MinApi-Async-Version";

        /// <summary>
        /// 验证请求结果
        /// </summary>
        /// <param name="httpHandler"></param>
        /// <param name="RouteData"></param>
        /// <param name="_objs"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        internal static bool Initialize(IHttpApi httpHandler, AshxRouteData RouteData, out object[] _objs, out Exception error)
        {
            httpHandler.SetRouteData(RouteData);
            RouteData.HttpContext.Response.AppendHeader(RouteData.GetAshx.IsTask ? AshxVersionHeaderAsyncName : AshxVersionHeaderName, AshxVersion);
            if (httpHandler.Initialize(RouteData.GetAshx))
            {
                error = GetApiObj(RouteData, out _objs);
                return true;
            }
            _objs = null;
            error = null;
            return false;
        }

        /// <summary>
        /// 验证请求结果
        /// </summary>
        /// <param name="httpHandler"></param>
        /// <param name="RouteData"></param>
        /// <param name="_objs"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        internal static bool MinInitialize(IMinHttpApi httpHandler, AshxRouteData RouteData, out object[] _objs, out Exception error)
        {
            RouteData.HttpContext.Response.AppendHeader(RouteData.GetAshx.IsTask ? MinAshxVersionHeaderAsyncName : MinAshxVersionHeaderName, AshxVersion);
            if (httpHandler.Initialize(RouteData))
            {
                error = GetApiObj(RouteData, out _objs);
                return true;
            }
            _objs = null;
            error = null;
            return false;
        }

        ///// <summary>
        ///// 获取参数
        ///// </summary>
        ///// <param name="RouteData"></param>
        ///// <param name="_objs"></param>
        ///// <returns></returns>
        //internal static bool GetObj(AshxRouteData RouteData, out object[] _objs)
        //{
        //    Ashx ashx = RouteData.GetAshx;
        //    if (ashx.Parameters.Length > 0)
        //    {
        //        bool obj(out object[] _objs)
        //        {
        //            int length = ashx.Parameters.Length;
        //            _objs = new object[length];
        //            _objs = AshxExtension.GetParameterObjs(ashx, RouteData.HttpContext.Request, 0, length, _objs, out bool isException);
        //            return isException;
        //        }
        //        return obj(out _objs);
        //    }
        //    _objs = default;
        //    return false;
        //}

        /// <summary>
        /// 获取参数
        /// </summary>
        /// <param name="RouteData"></param>
        /// <param name="_objs"></param>
        /// <returns></returns>
        internal static Exception GetApiObj(AshxRouteData RouteData, out object[] _objs)
        {
            Ashx ashx = RouteData.GetAshx;
            int length = ashx.Parameters.Length;
            if (length > 0)
            {
                Exception Obj(out object[] _objs)
                {
                    int index = 0;
                    _objs = new object[length];
                    if (ashx.IsMinApi && ashx.Parameters[index].ParameterType == typeof(HttpContext))
                    {
                        _objs[0] = RouteData.HttpContext;
                        index++;
                    }
                    _objs = AshxExtension.GetParameterObjs(ashx, RouteData.HttpContext.Request, index, length, _objs, out Exception error);
                    return error;
                }
                return Obj(out _objs);
            }
            _objs = default;
            return null;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="httpHandler">构造的对应实现</param>
        /// <param name="ashxRoute">路由信息对象</param>
        /// <param name="objs">数据</param>
        internal static Task StartMinAshx(IMinHttpApi httpHandler, AshxRouteData ashxRoute, object[] objs)
        {
            //httpHandler.SetRouteData(RouteData);
            //RouteData.HttpContext.Response.AppendHeader(AshxVersionHeaderName, AshxVersion);
            httpHandler.Request(ashxRoute, objs);
            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化(异步)
        /// </summary>
        /// <param name="ashxRoute">路由信息对象</param>
        /// <param name="httpHandler">构造的对应实现</param>
        /// <param name="objs">数据</param>
        internal static async Task StartMinAsyncAshx(IMinHttpAsynApi httpHandler, AshxRouteData ashxRoute, object[] objs)
        {
            var task = httpHandler.TaskRequest(ashxRoute, objs);
            await task;
            httpHandler.ContinueWith(task, ashxRoute);

            //await httpHandler.TaskRequest(ashxRoute, objs)
            //    .ContinueWith((task) =>
            //    {
            //        httpHandler.ContinueWith(task, ashxRoute);
            //    });
            //    Task.Run(() =>
            //{
            //        //httpHandler.SetRouteData(RouteData);
            //        //RouteData.HttpContext.Response.AppendHeader(AshxVersionHeaderAsyncName, AshxVersion);
            //        using (httpHandler)
            //    {
            //        ;

            //            //    if (taskWrapperAsync.Task.IsFaulted)
            //            //    {
            //            //        AshxException(new AshxException(RouteData.GetAshx, new Exception("发生异常，异步处理过程中发生异常。", taskWrapperAsync.Task.Exception.GetBaseException())) { ExceptionHandled = true });
            //            //    }
            //            //    if (!taskWrapperAsync.Task.IsCompleted)
            //            //    {
            //            //        AshxException(new AshxException(RouteData.GetAshx, new Exception("发生异常，异步处理未完成。")) { ExceptionHandled = true });
            //            //    }
            //        }
            //});
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="httpHandler">构造的对应实现</param>
        /// <param name="objs">数据</param>
        internal static Task StartAshx(IHttpApi httpHandler, object[] objs)
        {
            //httpHandler.SetRouteData(RouteData);
            //RouteData.HttpContext.Response.AppendHeader(AshxVersionHeaderName, AshxVersion);
            using (httpHandler)
            {
                httpHandler.Request(objs);
            }

            return Task.CompletedTask;
        }

        /// <summary>
        /// 初始化(异步)
        /// </summary>
        /// <param name="httpHandler">构造的对应实现</param>
        /// <param name="objs">数据</param>
        internal static async Task StartAsyncAshx(IHttpAsynApi httpHandler, object[] objs)
        {
            using (httpHandler)
            {
                var task = httpHandler.TaskRequest(objs);
                await task;
                httpHandler.ContinueWith(task);
            }

            //await httpHandler.TaskRequest(objs)
            //    .ContinueWith((task) =>
            //    {
            //        using (httpHandler)
            //        {
            //            httpHandler.ContinueWith(task);
            //        }
            //    });
            //    Task.Run(() =>
            //{
            //        //httpHandler.SetRouteData(RouteData);
            //        //RouteData.HttpContext.Response.AppendHeader(AshxVersionHeaderAsyncName, AshxVersion);
            //        using (httpHandler)
            //    {
            //        ;

            //            //    if (taskWrapperAsync.Task.IsFaulted)
            //            //    {
            //            //        AshxException(new AshxException(RouteData.GetAshx, new Exception("发生异常，异步处理过程中发生异常。", taskWrapperAsync.Task.Exception.GetBaseException())) { ExceptionHandled = true });
            //            //    }
            //            //    if (!taskWrapperAsync.Task.IsCompleted)
            //            //    {
            //            //        AshxException(new AshxException(RouteData.GetAshx, new Exception("发生异常，异步处理未完成。")) { ExceptionHandled = true });
            //            //    }
            //        }
            //});
        }

        /// <summary>
        /// 设置失败，请求错误状态，自定义输出结果
        /// </summary>
        /// <param name="HttpContext"></param>
        /// <param name="ContentType"></param>
        /// <param name="test"></param>
        /// <param name="StatusCode"></param>
        /// <returns></returns>
        public static async Task CustomOutput(HttpContext HttpContext, string ContentType, string test, int StatusCode)
        {
            //await Task.Run(() =>
            //{
            //    HttpContext.Response.StatusCode = StatusCode;
            //    HttpContext.Response.ContentType = string.Concat(ContentType, ";charset=utf-8");
            //    HttpContext.Response.Write(string.Concat("{\"code\":", StatusCode, "\"msg\":\"", test, "\"}"));
            //});

            HttpContext.Response.StatusCode = StatusCode;
            HttpContext.Response.ContentType = string.Concat(ContentType, ";charset=utf-8");
            await HttpContext.Response.WriteAsync(string.Concat("{\"code\":", StatusCode, ",\"msg\":\"", test, "\"}"));
        }

        //IAsyncResult IHttpAsyncHandler.BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
        //{
        //    context.Response.AppendHeader(AshxVersionHeaderName, AshxVersion);
        //    return httpApi.BeginRequest(context, cb, extraData); //extraData
        //}

        //void IHttpAsyncHandler.EndProcessRequest(IAsyncResult result)
        //{
        //    using (httpApi)
        //    {
        //        httpApi.EndRequest(result);
        //    }
        //}

    }
}
