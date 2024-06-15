using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Tool.Web.Api;
using Tool.Web.Api.ApiCore;

namespace Tool.Web.Routing
{
    internal class AshxRouteHandler : IRouter
    {
        //public AshxRouteHandler(IActionInvokerFactory actionInvokerFactory, IActionSelector actionSelector, DiagnosticListener diagnosticListener, ILoggerFactory loggerFactory)
        //{

        //}

        internal AshxBuilder AshxBuilder { get; }

        internal ILogger Logger => AshxBuilder.Logger;

        /// <summary>初始化类的新实例。<see cref="AshxRouteHandler" /></summary>
        public AshxRouteHandler(AshxBuilder AshxBuilder)
        {
            this.AshxBuilder = AshxBuilder;
        }

        /// <summary>
        /// 无需实现
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public VirtualPathData GetVirtualPath(VirtualPathContext context)
        {
            return null;
        }

        public Task RouteAsync(RouteContext context)
        {
            AshxRouteData _routeData = AshxBuilder.Filter(context);
            IHttpHandlerAshxExecute(_routeData);
            return Task.CompletedTask;

            //HttpContextExtension.Current
            //context.Handler
            //return Task.Run(() =>
            //{
            //    AshxRouteData ashxRoute = new AshxRouteData(context);

            //    ashxRoute.Handler = async (HttpContext i) =>
            //    {
            //        i.Response.ContentType = "text/html;charset=utf-8;";
            //        HttpContextExtension.Current.Response.Write("我是由注入获取的Current\n\t");

            //        i.Response.Write("Hello World! 我是Ashx.Core,由逆血提供技术支持！");
            //        await Task.CompletedTask;
            //    };

            //    //context.HttpContext.Abort();
            //    //context.HttpContext.Response.Body.Write(Encoding.UTF8.GetBytes("我是你爹"));
            //    //return Task.CompletedTask;
            //});
            //throw new NotImplementedException();
        }

        /// <summary>
        /// （默认是内部调用）调用此方法，可以对 当前路由的信息 Application_Start() 方法中调用 AshxRegistration.RegisterAllAreas();
        /// </summary>
        internal void IHttpHandlerAshxExecute(AshxRouteData _routeData)
        {
            string key = _routeData.Controller; ////string text = requestContext.RouteData.GetRequiredString("controller");$"{Registration.Segment}{text}";

            if (AshxBuilder.RouteDefaults.TryGetValue(key, out AshxExtension extension))
            {
                if (string.IsNullOrEmpty(_routeData.Area) || extension.AshxType.FullName.StartsWith($"{_routeData.Area}."))
                {
                    if (extension.IsMethod(_routeData.Action, out Ashx ashx))
                    {
                        _routeData.GetAshx = ashx;
                        return;
                    }
                    else
                    {
                        _routeData.IsAshx = false;
                        throw _routeData.HttpContext.AddHttpException(404, "在控制器“{0}”上未找到可执行方法“{1}”，URL: {2}", new object[] { key, _routeData.Action, _routeData.HttpContext.Request.Path });
                    }
                } //extension.AshxType.FullName.StartsWith($"{areaName}.")  //_routeData.Area.Equals(extension.AshxType.Namespace)
            }
            throw _routeData.HttpContext.AddHttpException(404, "找不到该路由的控制器:“{0}” ，URL: {1}", new object[] { key, _routeData.HttpContext.Request.Path });
        }
    }
}

//#if BEBUG #endif

//Debug.WriteLine($@"调用 MinAshx 控制器操作所用内容:
//                  HTTP 方法: {_routeData.HttpContext.Request.Method}
//                  路径: {_routeData.HttpContext.Request.Path}
//                  控制器: {_routeData.Controller}
//                  操作: {_routeData.Action}
//                  请求 ID: {_routeData.Key.Value} ");

//Debug.WriteLine($@"调用 Ashx 控制器操作所用内容:
//                  HTTP 方法: {_routeData.HttpContext.Request.Method}
//                  路径: {_routeData.HttpContext.Request.Path}
//                  控制器: {_routeData.Controller}
//                  操作: {_routeData.Action}
//                  请求 ID: {_routeData.Key.Value} ");
