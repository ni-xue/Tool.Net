using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Tool.Web.Session;

namespace Microsoft.Extensions.DependencyInjection //Tool.Web.Session
{
    /// <summary>
    /// AsSession服务的注册帮助类
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class SessionServiceExtensions
    {
        /// <summary>
        /// 添加AsSession，暂时可以不需要写，目前还没实现长连接数据绑定，采用内存处理。
        /// </summary>
        /// <param name="services"></param>
        /// <param name="action">必须实现的条件</param>
        /// <returns>IServiceCollection</returns>
        public static IServiceCollection AddDiySession(this IServiceCollection services, Action<DiySessionOptions> action)
        {
            //services.Add(ServiceDescriptor.Transient((IServiceProvider service) =>
            //{
            //    service.GetService(typeof(AsSession)); return default(AsSession);
            //}));

            var sd = services.AddOptions<DiySessionOptions>();
            if (action != null)
            {
                sd.Configure(action);
            }

            return services;
        }

        /// <summary>
        /// 注册AsSession
        /// </summary>
        /// <param name="app">IApplicationBuilder框架对象</param>
        /// <returns>IApplicationBuilder</returns>
        public static IApplicationBuilder UseDiySession(this IApplicationBuilder app)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            var options = app.ApplicationServices.GetRequiredService<Microsoft.Extensions.Options.IOptions<DiySessionOptions>>();

            DiySessionOptions sessionOptions = options.Value;

            if (sessionOptions.TypeDiySession is null)
            {
                throw new Exception("您未调用 .AddDiySession(); 方法完成注册DiySession，无法创建DiySession！");
            }

            if (string.IsNullOrWhiteSpace(sessionOptions.SessionName))
            {
                throw new NullReferenceException("SessionName为空，无法创建DiySession！");
            }

            return app.UseMiddleware(typeof(DiySessionMiddleware), options.Value);
        }

        
    }
}
