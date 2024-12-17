using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tool.Web;
using Tool.Web.Api.ApiCore;
using Tool.Web.Builder;
using Tool.Web.Routing;

namespace Microsoft.AspNetCore.Builder //Tool.Web.Builder
{
    /// <summary>
    /// Ashx 核心路由 扩展类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class AshxApplicationBuilderExtensions
    {
        /// <summary>
        /// Ashx路由
        /// </summary>
        /// <param name="app"><see cref="IApplicationBuilder"/></param>
        /// <param name="configureRoutes"><see cref="IRouteBuilder"/></param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseAshx(this IApplicationBuilder app, Action<IRouteBuilder> configureRoutes)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (configureRoutes == null)
            {
                throw new ArgumentNullException(nameof(configureRoutes));
            }

            AshxBuilder builder;
            try
            {
                builder = app.ApplicationServices.GetRequiredService<AshxBuilder>();
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("No service for type 'Tool.Web.AshxBuilder' has been registered."))
                {
                    throw new Exception("您未调用 .AddAshx(); 方法完成注册Ashx，无法创建路由！");
                }
                throw new Exception("注册Ashx路由服务异常！", ex);
            }

            var options = app.ApplicationServices.GetRequiredService<IOptions<AshxOptions>>();

            if (options.Value.EnableEndpointRouting)
            {
                throw new Exception("您当前采用的是终结点模式，请使用 app.UseEndpoints()！ 若要使用当前模式，请将 EnableEndpointRouting 值设置为 false");
            }
            options.Value.JsonOptions ??= AshxOptions.JsonOptionsDefault;

            builder.Application = app;

            builder.Options = options.Value;

            // ILoggerFactory loggerFactory
            ILoggerFactory loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

            builder.Logger = loggerFactory.CreateLogger("Tool.Api");

            //创建默认mvc处理类 
            //RouteBuilder为RouterMiddleware中间件创建所需的Router对象
            var routes = new RouteBuilder(app)
            {
                DefaultHandler = builder.AshxRoute,
            };

            // 获取当前动态配置的路由模式
            builder.RegisterAshxRoute(routes); 

            //配置MVC路由的回调
            configureRoutes(routes); //Microsoft.AspNetCore.Routing.Route

            if (builder.IsHttpContext)
            {
                HttpContextExtension.Accessor = app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
            }

            //CreateAttributeMegaRoute:返回一个IRouter 主要是用来处理 RouteAttribute 标记的Action,
            //routes.Routes.Insert(0, AttributeRouting.CreateAttributeMegaRoute(app.ApplicationServices));
            //使用制定的路由将路由中间件田间到制applicationbuilder

            return app.UseRouter(routes.Build());
        }

        /// <summary>
        /// 注入 捕获全局异常 对象
        /// </summary>
        /// <param name="app">IApplicationBuilder框架对象</param>
        /// <param name="action">需要提供处理这些异常的委托函数</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseExceptionHandler(this IApplicationBuilder app, Func<HttpContext, Exception, Task> action)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            //app.UseExceptionHandler(a => a.Run(async context =>
            //{
            //    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
            //    var exception = feature.Error;
            //    await Task.Run(() => action(context, exception));
            //}));

            ExceptionHandlerOptions options = new()
            {
                ExceptionHandler = async (context) => 
                {
                    var feature = context.Features.Get<IExceptionHandlerPathFeature>();
                    //var exception = feature.Error;
                    await action?.Invoke(context, feature?.Error);
                    //await Task.CompletedTask; 
                }
            };

            app.UseExceptionHandler(options);

            return app;
        }

        /// <summary>
        /// 获取 已经注册的 服务或数据
        /// </summary>
        /// <param name="app">IApplicationBuilder框架对象</param>
        /// <typeparam name="T">获取的服务类型</typeparam>
        /// <returns>IApplicationBuilder</returns>
        public static T GetObject<T>(this IApplicationBuilder app)
        {
            return (T)app.GetObject(typeof(T));
        }

        /// <summary>
        /// 获取 已经注册的 服务或数据
        /// </summary>
        /// <param name="app">IApplicationBuilder框架对象</param>
        /// <param name="type">获取的服务类型</param>
        /// <returns>IApplicationBuilder</returns>
        public static object GetObject(this IApplicationBuilder app, Type type)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.ApplicationServices.GetService(type);
        }

        /// <summary>
        /// 注入HttpContext对象，实现静态获取
        /// </summary>
        /// <param name="app">IApplicationBuilder框架对象</param>
        /// <returns>IHttpContextAccessor</returns>
        public static IHttpContextAccessor AddHttpContext(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            return app.ApplicationServices.GetRequiredService<IHttpContextAccessor>();
        }

        /// <summary>
        /// 注册 IgnoreUrl （忽略请求地址）
        /// </summary>
        /// <param name="app">IApplicationBuilder框架对象</param>
        /// <param name="urls">需要忽略的地址，必填项</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseIgnoreUrl(this IApplicationBuilder app, params string[] urls)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (urls == null)
            {
                throw new NullReferenceException("IgnoreUrl:没有需要忽略的Url！");
            }
            for (int i = 0; i < urls.Length; i++)
            {
                IgnoreUrlMiddleware.Urls.Add(PathString.FromUriComponent(urls[i][0].Equals('/') ? urls[i] : $"/{urls[i]}"));
            }

            return app.UseMiddleware(typeof(IgnoreUrlMiddleware));
        }

    }
}
