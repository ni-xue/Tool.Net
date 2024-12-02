using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Web.Builder
{
    /// <summary>
    /// URL 拦截器中间件
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class IgnoreUrlMiddleware
    {
        private RequestDelegate Next { get; }

        private ILogger Logger { get; set; }

        /// <summary>
        /// 现有的拦截器配置
        /// </summary>
        public static HashSet<PathString> Urls { get; } = new HashSet<PathString>();

        /// <summary>
        /// 注册 URL 拦截器
        /// </summary>
        /// <param name="next"></param>
        /// <param name="loggerFactory"></param>
        public IgnoreUrlMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)//, object[] urls
        {
            Next = next;
            Logger = loggerFactory.CreateLogger("IgnoreUrl");
           // Urls = urls;
        }

        /// <summary>
        /// 处理每次请求 验证是否需要拦截
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task InvokeAsync(HttpContext context)//, RequestDelegate next/// <param name="next"></param>
        {
            if (context.Request.Path.HasValue && !context.Request.Path.Equals("/"))
            {
                //if (Urls.Contains(context.Request.Path))
                //{
                //    //Urls.Comparer.Equals("1", "1");
                //}

                PathString _IgnoreUrl = Urls.FirstOrDefault(predicate =>
                {
                    return context.Request.Path.StartsWithSegments(predicate, StringComparison.OrdinalIgnoreCase);
                });

                if (_IgnoreUrl.HasValue)
                {
                    context.Response.StatusCode = 404;
                    Info(context.Request.Path.Value, _IgnoreUrl);
                    return;
                    //Urls.Comparer.Equals("1", "1");
                }

                //foreach (string url in Urls)
                //{
                //    if (context.Request.Path.Value.StartsWith(url, StringComparison.OrdinalIgnoreCase))
                //    {
                //        context.Response.StatusCode = 404;
                //        await Task.CompletedTask;
                //        Info(context.Request.Path.Value, url);
                //    }
                //}
            }
            await Next?.Invoke(context);
        }

        private void Info(string url, string _url)
        {
            if (Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation("已拦截请求Url: {url} ,拦截规则: {_url}。", url, _url);
            }
        }
    }
}
