using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Collections.ObjectModel;
using Tool.Web.Api;
using Tool.Web.Api.ApiCore;
using Tool.Web.Routing;
using Tool.Utils.Data;
using Microsoft.AspNetCore.Builder;
using Tool.Web.Builder;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Routing;

namespace Tool.Web
{
    internal class AshxBuilder : IAshxBuilder
    {
        /// <summary>
        /// 子一级路由对象
        /// </summary>
        internal readonly IReadOnlyDictionary<string, AshxExtension> RouteDefaults = null;

        /// <summary>
        /// 核心日志对象
        /// </summary>
        internal ILogger Logger { get; set; }

        /// <summary>
        /// 路由对象 （旧模式）
        /// </summary>
        internal AshxRouteHandler AshxRoute { get; }

        /// <summary>
        /// 终结点对象 （新模式）
        /// </summary>
        internal AshxEndpointDataSource AshxEndpoint { get; }

        /// <summary>
        /// 当前对象注册所需要选项对象
        /// </summary>

        internal AshxOptions Options { get; set; }

        /// <summary>
        /// API项目目录
        /// </summary>
        private string Namespace = string.Empty;

        //public IReadOnlyDictionary<string, string> Dictionary { get; } = new ReadOnlyDictionary<string, string>(new Dictionary<string, string>
        //{
        //    { "1", "111" },
        //    { "2", "222" },
        //    { "3", "333" },
        //});
        //var dictionary = (Dictionary<string, string>)Dictionary; // possible
        //dictionary.Add("4", "444"); // possible
        //var sda = (Dictionary<string, AshxExtension>)RouteDefaults;
        //sda.TryAdd("s", null);

        /// <summary>
        /// 启动Ashx核心对象
        /// </summary>
        /// <param name="Services"></param>
        public AshxBuilder(IServiceCollection Services)
        {
            this.Services = Services;

            AshxRoute = new AshxRouteHandler(this);//ReadOnlyDictionary

            AshxEndpoint = new AshxEndpointDataSource(this);

            Dictionary<string, AshxExtension> keys = GetAssembly();

            RouteDefaults = keys.AsReadOnly(); //new ReadOnlyDictionary<string, AshxExtension>(new Dictionary<string, AshxExtension>(StringComparer.OrdinalIgnoreCase)); // new RouteValueDictionary();
        }

        private Dictionary<string, AshxExtension> GetAssembly()
        {
            Dictionary<string, AshxExtension> _RouteDefaults = new(StringComparer.OrdinalIgnoreCase);

            Assembly assembly = GetHttpApplicationAssembly();

            Type[] types = assembly.GetTypes();

            Namespace = assembly.FullName.Split(',')[0];

            foreach (Type type in types)
            {
                if (typeof(ApiAshx).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                {
                    string typeFullName = type.FullName.Split('.')[0];

                    if (typeFullName.Equals(Namespace))
                    {
                        string url = type.Name.ToLower(); //$"{Registration.Segment}{ type.Name }".ToLower();

                        AshxExtension extension = new(type, false);

                        _RouteDefaults.TryAdd(url, extension);
                    }
                }
                else if (typeof(MinApi).IsAssignableFrom(type) && type.IsClass && !type.IsAbstract)
                {
                    string typeFullName = type.FullName.Split('.')[0];

                    if (typeFullName.Equals(Namespace))
                    {
                        string url = type.Name.ToLower(); //$"{Registration.Segment}{ type.Name }".ToLower();

                        AshxExtension extension = new(type, true);

                        _RouteDefaults.TryAdd(url, extension);
                    }
                }
            }

            return _RouteDefaults;
        }

        private Assembly GetHttpApplicationAssembly()
        {
            Assembly Iassembly = this.GetType().Assembly;//获取自己

            Assembly assembly = null;

            var Types = GetTypes();

            foreach (Type type in Types)
            {
                if (!type.Assembly.Equals(Iassembly))//&& typeof(HttpApplication).IsAssignableFrom(type)
                {
                    assembly = type.Assembly;
                    break;
                }
            }

            return assembly;
        }

        private static Type[] GetTypes()
        {
            var Sources = new System.Diagnostics.StackTrace().GetFrames();

            Type[] types = new Type[Sources.Length];

            for (int i = 0; i < Sources.Length; i++)
            {
                types[i] = Sources[i].GetMethod().ReflectedType;
            }
            return types;
        }

        internal void RegisterAshxRoute(object builder) 
        {
            foreach (var keyValue in RouteDefaults)
            {
                var extension = keyValue.Value;

                AshxRouteAttribute ashxRoute = Attribute.GetCustomAttribute(extension.AshxType, typeof(AshxRouteAttribute)).ToVar<AshxRouteAttribute>();

                if (ashxRoute != null)
                {
                    ashxRoute.Name ??= keyValue.Key;

                    if (Options.EnableEndpointRouting)//ashxRoute.Name, 
                    {
                        ((Microsoft.AspNetCore.Routing.EndpointDataSource)builder).MapApiRoute(areaName: extension.AshxType.Namespace, controller: keyValue.Key, template: ashxRoute.Template);
                    }
                    else
                    {
                        ((Microsoft.AspNetCore.Routing.IRouteBuilder)builder).MapApiRoute(ashxRoute.Name, areaName: extension.AshxType.Namespace, controller: keyValue.Key, template: ashxRoute.Template);
                    }
                }

                foreach (var ashx in extension.Ashxes)
                {
                    var _ashx = ashx.Value;
                    AshxRouteAttribute ashxRoute1 = Attribute.GetCustomAttribute(_ashx.Method, typeof(AshxRouteAttribute)).ToVar<AshxRouteAttribute>();
                    if (ashxRoute1 != null)
                    {
                        ashxRoute1.Name ??= string.Concat(keyValue.Key, '-', ashx.Key);

                        if (Options.EnableEndpointRouting)//ashxRoute1.Name,
                        {
                            ((Microsoft.AspNetCore.Routing.EndpointDataSource)builder).MapApiRoute(areaName: extension.AshxType.Namespace, controller: keyValue.Key, action: ashx.Key, template: ashxRoute1.Template);
                        }
                        else
                        {
                            ((Microsoft.AspNetCore.Routing.IRouteBuilder)builder).MapApiRoute(ashxRoute1.Name, areaName: extension.AshxType.Namespace, controller: keyValue.Key, action: ashx.Key, template: ashxRoute1.Template);
                        }
                    }
                }
            }
        }

        internal AshxRouteData Filter(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var routeData = context.GetRouteData() ?? throw new ArgumentNullException("无法获取到，请求信息的路由配置信息！");

            //var routeData = context.Features.Get<IRoutingFeature>()?.RouteData ?? throw new ArgumentNullException("无法获取到，请求信息的路由配置信息！"); ;

            var routeContext = new RouteContext(context) { RouteData = routeData };

            return Filter(routeContext);
        }

        internal AshxRouteData Filter(Microsoft.AspNetCore.Routing.RouteContext context)
        {
            //AshxRouteData _routeData = new AshxRouteData(requestContext);, System.Text.Json.JsonSerializerOptions jsonOptions
            //context.
            return new AshxRouteData(context, Options.JsonOptions);
        }

        /// <summary>
        /// 容器对象
        /// </summary>
        public IServiceCollection Services { get; }

        /// <summary>
        /// 提供整个请求服务者（依赖注入）
        /// </summary>
        public IApplicationBuilder Application { get; set; }

        /// <summary>
        /// 是否注入HttpContext对象
        /// </summary>
        public bool IsHttpContext { get; set; }
    }
}
