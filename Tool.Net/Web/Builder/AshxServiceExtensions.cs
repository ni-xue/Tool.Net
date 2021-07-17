using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using Tool.Web;
using Tool.Web.Api.ApiCore;
using Tool.Web.Builder;
using Tool.Web.Routing;

namespace Microsoft.Extensions.DependencyInjection //Tool.Web.Builder
{
    /// <summary>
    /// Ashx 核心对象扩展类
    /// </summary>
    public static class AshxServiceExtensions
    {
        /// <summary>
        /// 启动Ashx路由模式
        /// </summary>
        /// <param name="services"></param>
        /// <returns>IAshxBuilder</returns>
        public static IAshxBuilder AddAshx(this IServiceCollection services) 
        {
            return services.AddAshx(null);
        }

        /// <summary>
        /// 启动Ashx路由模式
        /// </summary>
        /// <param name="services">注册对象</param>
        /// <param name="action">选择委托</param>
        /// <returns>IAshxBuilder</returns>
        public static IAshxBuilder AddAshx(this IServiceCollection services, Action<AshxOptions> action)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            AshxBuilder builder = new(services);

            services.Add(new ServiceDescriptor(builder.GetType(), builder));//services.Add, 

            var sd = services.AddOptions<AshxOptions>();
            if (action != null)
            {
                sd.Configure(action);
            }

            return builder;
        }

        ///// <summary>
        ///// 注入IAshxBuilder 是否采用异步线程池，启动路由
        ///// </summary>
        ///// <param name="ashxBuilder">Ashx框架对象</param>
        ///// <param name="IsAsync">采用路由方式</param>
        ///// <returns>IAshxBuilder</returns>
        //public static IAshxBuilder SetAsync(this IAshxBuilder ashxBuilder, bool IsAsync)
        //{
        //    if (ashxBuilder == null)
        //    {
        //        throw new ArgumentNullException(nameof(ashxBuilder));
        //    }

        //    ashxBuilder.IsAsync = IsAsync;

        //    return ashxBuilder;
        //}

        /// <summary>
        /// 注入HttpContext对象，实现静态获取
        /// </summary>
        /// <param name="ashxBuilder">Ashx框架对象</param>
        /// <returns>IAshxBuilder</returns>
        public static IAshxBuilder AddHttpContext(this IAshxBuilder ashxBuilder)
        {
            if (ashxBuilder == null)
            {
                throw new ArgumentNullException(nameof(ashxBuilder));
            }

            ashxBuilder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();//Type.GetType("Microsoft.AspNetCore.Http.HttpContextAccessor")

            //ashxBuilder.Services.AddSingleton(typeof(IHttpContextAccessor), Type.GetType("Microsoft.AspNetCore.Http.HttpContextAccessor, Microsoft.AspNetCore.Http"));

            ashxBuilder.IsHttpContext = true;

            return ashxBuilder;
        }

        /// <summary>
        /// 将用户自定义的数据注册进服务。
        /// </summary>
        /// <param name="services">Ashx框架对象</param>
        /// <param name="_obj">注册的数据</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddObject(this IServiceCollection services, object _obj)
        {
            return services.AddObject(_obj.GetType(), _obj);
        }

        /// <summary>
        /// 将用户自定义的数据注册进服务。
        /// </summary>
        /// <param name="services">Ashx框架对象</param>
        /// <param name="_obj">注册的数据</param>
        /// <typeparam name="T"></typeparam>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddObject<T>(this IServiceCollection services, T _obj)
        {
            return services.AddObject(typeof(T), _obj);
        }

        /// <summary>
        /// 将用户自定义的数据注册进服务。
        /// </summary>
        /// <param name="services">主服务</param>
        /// <param name="type">注册对象类型</param>
        /// <param name="_obj">注册的数据</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddObject(this IServiceCollection services, Type type, object _obj)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            services.Add(new ServiceDescriptor(type, _obj));//services.Add, 

            return services;
        }

        /// <summary>
        /// 设置系统默认配置的<see cref="FormOptions"/>对象属性
        /// </summary>
        /// <param name="app">IServiceCollection框架对象</param>
        /// <param name="action">用于属性配置的方法</param>
        /// <returns>IServiceCollection框架对象</returns>
        public static IServiceCollection SetFormOptions(this IServiceCollection app, Action<FormOptions> action)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }

            app.Configure(action);

            return app;
        }
    }
}
