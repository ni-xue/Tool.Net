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

        public async Task RouteAsync(RouteContext context)
        {
            AshxRouteData _routeData = AshxBuilder.Filter(context);

            if (AshxBuilder.Options.IsAsync)
            {
                await Task.Run(async () => await IHttpHandlerAshxExecute(_routeData));
            }
            else
            {
                await IHttpHandlerAshxExecute(_routeData);
            }

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
        internal Task IHttpHandlerAshxExecute(AshxRouteData _routeData)
        {
            string key = _routeData.Controller; ////string text = requestContext.RouteData.GetRequiredString("controller");$"{Registration.Segment}{text}";

            if (AshxBuilder.RouteDefaults.TryGetValue(key, out AshxExtension extension))
            {
                if (_routeData.Area != string.Empty && !_routeData.Area.Equals(extension.AshxType.Namespace))
                {
                    throw _routeData.HttpContext.AddHttpException(404, string.Format(System.Globalization.CultureInfo.CurrentCulture, "找不到该路由的控制器:“{0}” ，URL: {1}", new object[] { key, _routeData.HttpContext.Request.Path }));
                }

                Ashx ashx;
                void isMethod()
                {
                    if (extension.IsMethod(_routeData.Action, out ashx))
                    {
                        _routeData.GetAshx = ashx;
                        //SessionStateBehavior sessionState = (SessionStateBehavior)(int)ashx.IsSession;//Enum.Parse(typeof(SessionStateBehavior),);
                        //requestContext.HttpContext.SetSessionStateBehavior(sessionState);
                    }
                    else
                    {
                        _routeData.IsAshx = false;
                        //requestContext.HttpContext.SetSessionStateBehavior(SessionStateBehavior.Disabled);

                        //在控制器“TaskApi”上未找到可执行方法“SellerInf
                        throw _routeData.HttpContext.AddHttpException(404, string.Format(System.Globalization.CultureInfo.CurrentCulture, "在控制器“{0}”上未找到可执行方法“{1}”，URL: {2}", new object[] { key, _routeData.Action, _routeData.HttpContext.Request.Path }));
                    }
                }

                isMethod();

                _routeData.SetKey();

                bool isOptions() 
                {
                    if (AshxExtension.CrossDomain(_routeData.HttpContext.Response, _routeData.GetAshx) && _routeData.HttpContext.Request.Method.EqualsNotCase("OPTIONS"))
                    {
                        _routeData.Handler = async (_) => await Task.CompletedTask;
                        Info(_routeData);
                        return true;
                    }
                    return false;
                }

                if (isOptions()) return Task.CompletedTask;

                if (!AshxExtension.HttpMethod(_routeData.HttpContext.Request.Method, ashx.State))
                {
                    _routeData.Handler = async (HttpContext i) => await AshxHandlerOrAsync.CustomOutput(i, "application/json", "请求类型错误！", 403);
                    return Task.CompletedTask;
                }

                if (extension.IsMin)
                {
                    Task minapi()
                    {
                        IMinHttpAsynApi handler = extension.MinHttpAsynApi;

                        if (AshxHandlerOrAsync.MinInitialize(handler, _routeData))
                        {
                            if (AshxHandlerOrAsync.GetMinObj(_routeData, out object[] _objs))
                            {
                                _routeData.Handler = async (HttpContext i) => await AshxHandlerOrAsync.CustomOutput(i, "application/json", "框架错误，请查看日志文件！", 500);
                                return Task.CompletedTask;
                            }

                            Info(_routeData);

                            if (ashx.IsTask)
                            {
                                //AshxAsyncHandler ashxAsync = new AshxAsyncHandler(handler as IHttpAsynApi, _routeData);
                                _routeData.Handler = async (HttpContext i) => await AshxHandlerOrAsync.StartMinAsyncAshx(handler, _routeData, _objs);
                                //return Task.CompletedTask;
                            }
                            else
                            {
                                //AshxHandler ashxHandler = new AshxHandler(handler as IHttpApi, _routeData);
                                _routeData.Handler = async (HttpContext i) => await AshxHandlerOrAsync.StartMinAshx(handler, _routeData, _objs);
                            }

                            return Task.CompletedTask;
                        }
                        else
                        {
                            //因用户取消逻辑
                            _routeData.Handler = async (HttpContext i) => await Task.CompletedTask;
                            return Task.CompletedTask;
                        }
                    }

                    return minapi();
                }
                else
                {
                    Task ashxapi()
                    {
                        IHttpAsynApi handler;
                        try
                        {
                            handler = extension.NewClassAshx(AshxBuilder.Application.ApplicationServices);
                        }
                        catch (Exception e)
                        {
                            throw _routeData.HttpContext.AddHttpException(404, string.Format(System.Globalization.CultureInfo.CurrentCulture, "在创建时发生异常，应该是使用有参构造函数，URL: {0}", new object[] { _routeData.HttpContext.Request.Path }), e);
                        }

                        if (AshxHandlerOrAsync.Initialize(handler, _routeData))//初始加载
                        {
                            if (AshxHandlerOrAsync.GetObj(_routeData, out object[] _objs))
                            {
                                _routeData.Handler = async (HttpContext i) => await AshxHandlerOrAsync.CustomOutput(i, "application/json", "框架错误，请查看日志文件！", 500);
                                return Task.CompletedTask;
                            }

                            Info(_routeData);

                            if (ashx.IsTask)
                            {
                                //AshxAsyncHandler ashxAsync = new AshxAsyncHandler(handler as IHttpAsynApi, _routeData);
                                _routeData.Handler = async (HttpContext i) => await AshxHandlerOrAsync.StartAsyncAshx(handler, _objs);
                                //return Task.CompletedTask;
                            }
                            else
                            {
                                //AshxHandler ashxHandler = new AshxHandler(handler as IHttpApi, _routeData);
                                _routeData.Handler = async (HttpContext i) => await AshxHandlerOrAsync.StartAshx(handler, _objs);
                            }
                            return Task.CompletedTask;
                        }
                        else
                        {
                            //因用户取消逻辑
                            _routeData.Handler = async (HttpContext i) => await Task.CompletedTask;
                            return Task.CompletedTask;
                        }
                    }

                    return ashxapi();
                }
            }
            //if (AshxBuilder._segment.Equals("/false/"))
            //{
            //    throw _routeData.HttpContext.AddHttpException(404, string.Format(System.Globalization.CultureInfo.CurrentCulture, "找不到该路由的控制器:“{0}” ，URL: {1}，如果的确有这个路由，可能是MVC模式的路由，出现这种情况是因为您使用了：RegisterAllAreas(string controller, string action) 这个模式是专门针对没有MVC工程的项目使用的，请更换。", new object[] { key, requestContext.HttpContext.Request.Path }));
            //}
            throw _routeData.HttpContext.AddHttpException(404, string.Format(System.Globalization.CultureInfo.CurrentCulture, "找不到该路由的控制器:“{0}” ，URL: {1}", new object[] { key, _routeData.HttpContext.Request.Path }));
        }

        private void Info(AshxRouteData _routeData)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                void info()
                {
                    string ApiName;

                    if (_routeData.IsAshx && _routeData.GetAshx.IsMinApi)
                    {
                        ApiName = "MinApi";
                    }
                    else
                    {
                        ApiName = "ApiAshx";
                    }

                    Logger.LogInformation("调用 {ApiName} 控制器操作所用内容\n\tHTTP 方法: {Method}\n\t路径: {Path}\n\t控制器: {Controller}\n\t操作: {Action}\n\t请求 ID: {Key}",
                        ApiName,
                        _routeData.HttpContext.Request.Method,
                        _routeData.HttpContext.Request.Path,
                        _routeData.Controller,
                        _routeData.Action,
                        _routeData.Key
                        );
                }

                info();

                //Logger.LogInformation($@"调用 {ApiName} 控制器操作所用内容:
                //HTTP 方法: {_routeData.HttpContext.Request.Method}
                //路径: {_routeData.HttpContext.Request.Path}
                //控制器: {_routeData.Controller}
                //操作: {_routeData.Action}
                //请求 ID: {_routeData.Key.Value} ", ApiName);
            }
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
