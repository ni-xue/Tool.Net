using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Tool.Web.Api;

namespace Tool.Web.Routing
{
    /// <summary>
    /// 封装有关路由的信息。
    /// </summary>
    public class AshxRouteData
    {
        /// <summary>
        /// 获取有关 路由 HTTP 请求的信息。
        /// </summary>
        internal virtual RouteContext RouteContext { get; }

        /// <summary>
        /// 获取有关 HTTP 请求的信息。
        /// </summary>
        /// <returns>一个对象，该对象包含有关 HTTP 请求的信息。</returns>
        public virtual HttpContext HttpContext { get; }

        /// <summary>
        /// 获取有关所请求路由的信息。
        /// </summary>
        /// <returns>一个对象，该对象包含有关所请求路由的信息。</returns>
        public virtual RouteData GetRouteData { get; }

        /// <summary>
        /// 表示当前 注册的 默认 Json 条件
        /// </summary>
        internal virtual System.Text.Json.JsonSerializerOptions JsonOptions { get; }

        /// <summary>
        /// 获取有关所请求路由的信息。
        /// </summary>
        /// <returns>一个对象，该对象包含有关所请求路由的信息。</returns>
        public virtual IServiceProvider Service { get { return HttpContext.RequestServices; } }

        /// <summary>
        /// 使用指定路由和路由处理程序初始化 <see cref="AshxRouteData"/> 类的新实例。
        /// </summary>
        /// <param name="routeContext">封装与所定义路由匹配的 HTTP 请求的相关信息。</param>
        /// <param name="jsonOptions">json配置对象</param>
        public AshxRouteData(RouteContext routeContext, System.Text.Json.JsonSerializerOptions jsonOptions)
        {
            this.JsonOptions = jsonOptions;

            this.RouteContext = routeContext;
            this.GetRouteData = routeContext.RouteData;
            this.HttpContext = routeContext.HttpContext;

            this.HttpContext.Request.RouteValues = this.GetRouteData.Values;
            //this.Service = this.HttpContext.RequestServices;
            this.IsAshx = true;
            try
            {
                this.Area = GetRequiredString("area");
                this.Controller = GetRequiredString("controller");
                this.Action = GetRequiredString("action");
            }
            catch (Exception)
            {
                throw HttpContext.AddHttpException(404, string.Format(System.Globalization.CultureInfo.CurrentCulture, "找不到该路由的控制器,，URL: {0}", new object[] { this.HttpContext.Request.Path }));
            }
        }

        /// <summary>
        /// 注册api唯一值
        /// </summary>
        internal void SetKey()
        {
            //string TraceId = this.HttpContext.TraceIdentifier, controller = this.Controller, action = this.Action;
            //() => string.Concat(StringExtension.GetGuid(), "/", controller, "/", action)
            //this.Key = new Lazy<string>(() => string.Concat(TraceId, "/", controller, "/", action));//$"{StringExtension.GetGuid(false)}/{this.Controller}/{this.Action}";
            string key = string.Concat(this.HttpContext.TraceIdentifier, "|", DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss:fffff"));
            this.Key = key;
        }

        /// <summary>
        /// 每次请求生成的Id
        /// </summary>
        public string Key { get; private set; }

        ///// <summary>
        ///// 每次请求生成的Id
        ///// </summary>
        //public Lazy<string> Key { get; set; }

        /// <summary>
        /// 当前的控制器的命名空间
        /// </summary>
        public string Area { get; }

        /// <summary>
        /// 当前的控制器
        /// </summary>
        public string Controller { get; }

        /// <summary>
        /// 当前的方法名
        /// </summary>
        public string Action { get; }

        /// <summary>
        /// 获取当前对象的 Ashx
        /// </summary>
        public Ashx GetAshx { get; internal set; }

        /// <summary>
        /// 获取当前对象的 Ashx 是否为空
        /// </summary>
        public bool IsAshx { get; internal set; }

        ///// <summary>
        ///// 给当前的 Ashx 赋值
        ///// </summary>
        ///// <param name="ashx"></param>
        //internal void SetAshx(Ashx ashx)
        //{
        //    this.IsAshx = true;
        //    this.GetAshx = ashx;
        //}

        /// <summary>
        /// 获取在 ASP.NET 路由确定路由是否匹配请求时，传递到路由处理程序但未使用的自定义值的集合。
        /// </summary>
        /// <returns>一个包含自定义值的对象。</returns>
        public RouteValueDictionary DataTokens => GetRouteData.DataTokens;

        /// <summary>
        /// 获取或设置表示路由的对象。
        /// </summary>
        /// <returns>一个表示路由定义的对象。</returns>
        public IList<IRouter> Routers => GetRouteData.Routers;

        /// <summary>
        /// 获取或设置处理所请求路由的对象。
        /// </summary>
        /// <returns>一个处理路由请求的对象。</returns>
        public RequestDelegate Handler { get { return RouteContext.Handler; } set { RouteContext.Handler = value; } }

        /// <summary>
        /// 获取路由的 URL 参数值和默认值的集合。
        /// </summary>
        /// <returns>一个对象，其中包含根据 URL 和默认值分析得出的值。</returns>
        public RouteValueDictionary Values => GetRouteData.Values;

        /// <summary>
        /// 使用指定标识符检索值。
        /// </summary>
        /// <param name="valueName">要检索的值的键。</param>
        /// <returns>其键与 valueName 匹配的 System.Web.Routing.RouteData.Values 属性中的元素。</returns>
        /// <exception cref="System.InvalidOperationException">valueName 的值不存在。</exception>
        public string GetRequiredString(string valueName)
        {
            return GetRequired(valueName)?.ToString() ?? string.Empty;// GetRouteData.GetRequiredString(valueName);
        }

        /// <summary>
        /// 使用指定标识符检索值。
        /// </summary>
        /// <param name="valueName">要检索的值的键。</param>
        /// <returns>其键与 valueName 匹配的 System.Web.Routing.RouteData.Values 属性中的元素。</returns>
        /// <exception cref="System.InvalidOperationException">valueName 的值不存在。</exception>
        public object GetRequired(string valueName)
        {
            if (GetRouteData.Values.TryGetValue(valueName, out object value))
            {
                return value;
            }
            return null;// GetRouteData.GetRequiredString(valueName);
        }

        /// <summary>
        /// 获取一个新的 JsonSerializerOptions 对象 原对象来源于 AddAshx 时注册值
        /// </summary>
        /// <returns>新的 JsonSerializerOptions 对象</returns>
        public System.Text.Json.JsonSerializerOptions GetNewJsonOptions() 
        {
            return new System.Text.Json.JsonSerializerOptions(JsonOptions ?? throw new ArgumentNullException(nameof(JsonOptions), "您未在 AddAshx 的时候注册 JsonSerializerOptions 对象！"));
        }

        ///// <summary>
        ///// 获取服务（在 System.IServiceProvider 从中检索服务对象。）
        ///// </summary>
        ///// <typeparam name="T">要获取的服务对象的类型。</typeparam>
        ///// <returns>类型为 T 或 null 的服务对象（如果没有此类服务）。</returns>
        //public T GetService<T>() 
        //{
        //    return Service.GetService<T>();
        //}
    }
}
