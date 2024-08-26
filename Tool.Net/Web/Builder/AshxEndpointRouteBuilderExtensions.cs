using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool.Utils;
using Tool.Web;
using Tool.Web.Api;
using Tool.Web.Api.ApiCore;
using Tool.Web.Builder;
using Tool.Web.Routing;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 终结点模式扩展
    /// </summary>
    public static class AshxEndpointRouteBuilderExtensions
    {
        /// <summary>
        /// 启动终结点（默认版）
        /// </summary>
        /// <param name="endpoints">对象</param>
        /// <returns></returns>
        public static AshxActionEndpointConventionBuilder MapAshxs(this IEndpointRouteBuilder endpoints)
        {
            return endpoints.MapAshxs(routes =>
            {
                routes.MapApiRoute(template: "{controller=Home}/{action}/{id?}");
            });
        }

        /// <summary>
        /// 启动终结点
        /// </summary>
        /// <param name="endpoints">对象</param>
        /// <param name="configureRoutes">要生成的路由委托</param>
        /// <returns></returns>
        public static AshxActionEndpointConventionBuilder MapAshxs(this IEndpointRouteBuilder endpoints, Action<EndpointDataSource> configureRoutes)
        {
            if (endpoints == null)
            {
                throw new ArgumentNullException(nameof(endpoints));
            }
            if (configureRoutes == null)
            {
                throw new ArgumentNullException(nameof(configureRoutes));
            }

            AshxBuilder builder;
            try
            {
                builder = endpoints.ServiceProvider.GetRequiredService<AshxBuilder>();
            }
            catch (Exception ex)
            {
                if (ex.Message.Equals("No service for type 'Tool.Web.AshxBuilder' has been registered."))
                {
                    throw new Exception("您未调用 .AddAshx(); 方法完成注册Ashx，无法创建路由！");
                }
                throw new Exception("注册Ashx路由服务异常！", ex);
            }

            var options = endpoints.ServiceProvider.GetRequiredService<IOptions<AshxOptions>>();

            if (!options.Value.EnableEndpointRouting)
            {
                throw new Exception("您当前采用的是路由模式，请使用 app.UseAshx()！ 若要使用当前模式，请将 EnableEndpointRouting 值设置为 true");
            }
            options.Value.JsonOptions ??= AshxOptions.JsonOptionsDefault;

            builder.Application = endpoints.GetValue("ApplicationBuilder") as IApplicationBuilder;// endpoints.CreateApplicationBuilder();

            builder.Options = options.Value;

            ILoggerFactory loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();

            builder.Logger = loggerFactory.CreateLogger("Tool.Api");

            builder.Logger.LogWarning("您好，当前您正在使用的是公测版本3.0的终结点模式，目前已是最终版本，如使用过程中发现任何问题，都可以邮件告知！");

            //创建默认mvc处理类 
            //RouteBuilder为RouterMiddleware中间件创建所需的Router对象

            //endpoints.ServiceProvider.GetService(typeof(IConfiguration)) as IConfiguration
            var ashxEndpoint = builder.AshxEndpoint.InitialLoad(configureRoutes);

            endpoints.DataSources.Add(ashxEndpoint);

            //配置MVC路由的回调
            //configureRoutes(routes); //Microsoft.AspNetCore.Routing.Route

            if (builder.IsHttpContext)
            {
                HttpContextExtension.Accessor = endpoints.ServiceProvider.GetRequiredService<IHttpContextAccessor>();
            }

            return new(ashxEndpoint);
        }

    }

    /// <summary>
    /// 终结点扩展对象
    /// </summary>
    public sealed class AshxActionEndpointConventionBuilder : IEndpointConventionBuilder
    {
        private readonly AshxEndpointDataSource AshxEndpoint;
        internal AshxActionEndpointConventionBuilder(AshxEndpointDataSource ashxEndpoint)
        {
            AshxEndpoint = ashxEndpoint;
        }

        /// <summary>
        /// 回调已经生成的终结点信息
        /// </summary>
        /// <param name="convention"></param>
        public void Add(Action<EndpointBuilder> convention)
        {
            AshxEndpoint._convention = convention;
        }
    }

    /// <summary>
    /// Ashx终结点核心模块
    /// </summary>
    public class AshxEndpointDataSource : EndpointDataSource, IDisposable
    {
        /// <summary>
        /// 获取现有终结点
        /// </summary>
        public override IReadOnlyList<Endpoint> Endpoints
        {
            get
            {
                return GetEndpoints();
            }
        }

        private Action<AshxEndpointDataSource> _configureRoutes;

        private readonly List<RouteEndpointBuilder> _endpointBuilders;

        internal Action<EndpointBuilder> _convention;

        private readonly AshxBuilder _AshxBuilder;//IEndpointRouteBuilder EndpointRouteBuilder;

        internal AshxEndpointDataSource(AshxBuilder builder)
        {
            _AshxBuilder = builder;

            //_configureRoutes = configureRoutes;

            _endpointBuilders = new();

            //if (_configureRoutes == null) throw new ArgumentNullException(nameof(configureRoutes));

            // 获取当前动态配置的路由模式
            //_AshxBuilder.RegisterAshxRoute(this);

            //_configureRoutes(this);
        }

        /// <summary>
        /// 初始化完成终结点规则
        /// </summary>
        /// <param name="configureRoutes"></param>
        /// <returns></returns>
        internal AshxEndpointDataSource InitialLoad(Action<AshxEndpointDataSource> configureRoutes)
        {
            _configureRoutes = configureRoutes;

            if (_configureRoutes == null) throw new ArgumentNullException(nameof(configureRoutes));

            // 获取当前动态配置的路由模式
            _AshxBuilder.RegisterAshxRoute(this);
            //Logger.LogWarning("动态路由模块暂不处理！[AshxRoute]");

            _configureRoutes(this);

            return this;
        }

        /// <summary>
        /// 获取操作事件
        /// </summary>
        /// <returns></returns>
        public override IChangeToken GetChangeToken()
        {
            return Microsoft.Extensions.FileProviders.NullChangeToken.Singleton; //_configuration.GetReloadToken();
        }

        private const string area = "area";
        private const string controller = "controller";
        private const string action = "action";

        private static RouteValueDictionary GetRouteValue(object _controller = null, object _action = null, object _area = null)
        {
            var _defaultsDictionary = new RouteValueDictionary();
            if (_controller is not null) _defaultsDictionary.TryAdd(controller, _controller);
            if (_action is not null) _defaultsDictionary.TryAdd(action, _action);
            if (_area is not null) _defaultsDictionary.TryAdd(area, _area);
            return _defaultsDictionary;
        }

        private static string ParameterPart(RoutePatternParameterPart route) => $"{{{route.Name}{(route.IsOptional ? "?" : null) ?? (route.Default is not null ? $"={route.Default}" : null)}}}";

        private static void RemovePattern(IReadOnlyList<RoutePatternPathSegment> PathSegments, string name, ref List<RoutePatternPathSegment> routes)
        {
            routes ??= PathSegments.ToList();
            foreach (var item in PathSegments)
            {
                if (item.Parts[0] is RoutePatternParameterPart route)
                {
                    if (route.Name == name)
                    {
                        routes.Remove(item);
                    }
                }
            }
        }

        private static string CreateTemplate(List<RoutePatternPathSegment> routes)
        {
            StringBuilder builder = new();
            foreach (var item in routes)
            {
                foreach (var item0 in item.Parts)
                {
                    if (item0 is RoutePatternParameterPart route)
                    {
                        builder.Append('/')
                        .Append('{')
                        .Append(route.Name)
                        .Append((route.IsOptional ? "?" : null) ?? (route.Default is not null ? $"={route.Default}" : null))
                        .Append('}');
                    }
                    else if (item0 is RoutePatternLiteralPart route1)
                    {
                        builder.Append('/').Append(route1.Content);
                    }
                }
            }
            //if (builder[1].Equals('{'))
            //{
            //   return builder.ToString(1, builder.Length - 1);
            //}
            return builder.ToString();
        }

        private void AddRoute(RoutePattern _routePattern, int order, Ashx ashx, RouteValueDictionary constraintsDictionary, object dataTokens)
        {
            string DisplayName = $"Ashx:{_routePattern.RawText}";
            if (EndpointExists(DisplayName)) throw new Exception($"{DisplayName}，路由规则重复设置！");
            var routeEndpointBuilder = new RouteEndpointBuilder(requestDelegate: InitialLoadAsync, routePattern: _routePattern, order: order)
            {
                DisplayName = DisplayName
            };

            //routeEndpointBuilder.Metadata.Add(extension);
            routeEndpointBuilder.Metadata.Add(ashx);
            //routeEndpointBuilder.Metadata.Add(new { areaName });
            routeEndpointBuilder.Metadata.Add(constraintsDictionary);
            routeEndpointBuilder.Metadata.Add(new RouteValueDictionary(dataTokens));

            _endpointBuilders.Add(routeEndpointBuilder);
        }

        private IReadOnlyDictionary<string, AshxExtension> GetRouteDefaults(string areaName)
        {
            if (string.IsNullOrEmpty(areaName))
            {
                if (_AshxBuilder.RouteDefaults.Any()) return _AshxBuilder.RouteDefaults;
                throw new Exception("当前项目中，未存在任何可用的控制器及接口！");
            }
            else
            {
                Dictionary<string, AshxExtension> keys = new(StringComparer.OrdinalIgnoreCase);
                string _areaName = $"{areaName}.";
                foreach (var pair in _AshxBuilder.RouteDefaults)
                {
                    AshxExtension extension = pair.Value;
                    if (extension.AshxType.FullName.StartsWith(_areaName)) keys.Add(pair.Key, extension);
                }
                if (keys.Count != 0) return keys;
                throw new Exception("在指定的命名空间内，未找到任何控制器及接口！");
            }
        }

        /// <summary>
        /// 生成终结点规则
        /// </summary>
        /// <param name="template"></param>
        /// <param name="areaName"></param>
        /// <param name="defaultsDictionary"></param>
        /// <param name="constraintsDictionary"></param>
        /// <param name="dataTokens"></param>
        public void MapRoute(string template, string areaName, RouteValueDictionary defaultsDictionary, RouteValueDictionary constraintsDictionary, object dataTokens)
        {
            if (string.IsNullOrWhiteSpace(template)) throw new ArgumentNullException(nameof(template), "路由规则不能为空！");
            var _routePattern = RoutePatternFactory.Parse(template);
            var controllerpart = _routePattern.GetParameter(controller);
            var actionpart = _routePattern.GetParameter(action);
            if (defaultsDictionary.TryGetValue(controller, out var defaults0) && controllerpart is not null)
            {
                throw new Exception($"已设置了，固定的{{{controller}={defaults0}}}，路由中不能存在{ParameterPart(controllerpart)}");
            }
            if (defaultsDictionary.TryGetValue(action, out var defaults1) && actionpart is not null)
            {
                throw new Exception($"已设置了，固定的{{{action}={defaults1}}}，路由中不能存在{ParameterPart(actionpart)}");
            }
            var RouteDefaults = GetRouteDefaults(areaName);

            List<RoutePatternPathSegment> routes = null;
            if (controllerpart is not null && actionpart is not null)
            {
                if (controllerpart?.Default is not null && actionpart?.Default is not null)
                {
                    if (RouteDefaults.TryGetValue(String(controllerpart.Default), out var extension) && extension.Ashxes.TryGetValue(String(actionpart.Default), out var ashx))
                    {
                        //string _template = template.Replace(ParameterPart(controllerpart), "");
                        //_template = _template.Replace(ParameterPart(actionpart), "").Replace("//", "/");

                        RemovePattern(_routePattern.PathSegments, controller, ref routes);
                        RemovePattern(_routePattern.PathSegments, action, ref routes);

                        string _template = CreateTemplate(routes);
                        AddRoute(RoutePatternFactory.Parse(_template, GetRouteValue(_area: areaName, _controller: controllerpart.Default, _action: actionpart.Default), null, null), 50, ashx, constraintsDictionary, dataTokens);
                    }
                    else
                    {
                        throw new Exception($"{template} 路由规则默认信息无效！");
                    }
                }
                //else if (controllerpart?.Default is not null && defaults1 is not null)
                //{
                //    if (RouteDefaults.TryGetValue(String(controllerpart.Default), out var extension) && extension.Ashxes.TryGetValue(String(defaults1), out var ashx))
                //    {
                //        var _defaultsDictionary = new RouteValueDictionary();
                //        _defaultsDictionary.TryAdd(action, defaults1);

                //        RemovePattern(_routePattern.PathSegments, controller, ref routes);

                //        string _template = CreateTemplate(routes);
                //        AddRoute(RoutePatternFactory.Parse(_template, GetRouteValue(_controller: controllerpart.Default), null, GetRouteValue(_action: defaults1)), 30, extension, ashx, constraintsDictionary, dataTokens);
                //    }
                //    else
                //    {
                //        throw new Exception($"{template} 路由规则默认信息无效！");
                //    }
                //}
                //else if (actionpart?.Default is not null && defaults0 is not null)
                //{
                //    if (RouteDefaults.TryGetValue(String(defaults0), out var extension) && extension.Ashxes.TryGetValue(String(actionpart.Default), out var ashx))
                //    {
                //        RemovePattern(_routePattern.PathSegments, action, ref routes);

                //        string _template = CreateTemplate(routes);
                //        AddRoute(RoutePatternFactory.Parse(_template, GetRouteValue(_action: actionpart.Default), null, GetRouteValue(_controller: defaults0)), 30, extension, ashx, constraintsDictionary, dataTokens);
                //    }
                //    else
                //    {
                //        throw new Exception($"{template} 路由规则默认信息无效！");
                //    }
                //}

                foreach (var extensions in RouteDefaults)
                {
                    AshxExtension extension = extensions.Value;

                    foreach (var _keys in extension.Ashxes)
                    {
                        string _template = template.Replace(ParameterPart(controllerpart), extensions.Key).Replace(ParameterPart(actionpart), _keys.Key);//$"{{{controller}={extensions.Key}}}" $"{{{action}={_keys.Key}}}"
                        AddRoute(RoutePatternFactory.Parse(_template, GetRouteValue(_area: areaName, _controller: extensions.Key, _action: _keys.Key), null, null), 40, _keys.Value, constraintsDictionary, dataTokens);
                    }
                }
            }
            else if (controllerpart is not null && defaults1 is not null)
            {
                bool isok = false;
                if (RouteDefaults.TryGetValue(String(controllerpart.Default), out var extension) && extension.Ashxes.TryGetValue(String(defaults1), out var ashx))
                {
                    RemovePattern(_routePattern.PathSegments, controller, ref routes);
                    string _template = CreateTemplate(routes);
                    AddRoute(RoutePatternFactory.Parse(_template, GetRouteValue(_area: areaName, _controller: controllerpart.Default, _action: defaults1), null, null), 30, ashx, constraintsDictionary, dataTokens);
                    isok = true;
                }

                foreach (var extensions in RouteDefaults)
                {
                    extension = extensions.Value;
                    if (extension.Ashxes.TryGetValue(String(defaults1), out ashx))
                    {
                        string _template = template.Replace(ParameterPart(controllerpart), extensions.Key);
                        AddRoute(RoutePatternFactory.Parse(_template, GetRouteValue(_area: areaName, _controller: extensions.Key, _action: defaults1), null, null), 20, ashx, constraintsDictionary, dataTokens);
                        isok = true;
                    }
                }

                if (!isok)
                {
                    throw new Exception($"无适配的该路由({template})的控制器接口！");
                }
            }
            else if (actionpart is not null && defaults0 is not null)
            {
                if (RouteDefaults.TryGetValue(String(defaults0), out var extension))
                {
                    if (extension.Ashxes.TryGetValue(String(actionpart.Default), out var ashx))
                    {
                        RemovePattern(_routePattern.PathSegments, action, ref routes);
                        string _template = CreateTemplate(routes);
                        AddRoute(RoutePatternFactory.Parse(_template, GetRouteValue(_area: areaName, _controller: defaults0, _action: actionpart.Default), null, null), 30, ashx, constraintsDictionary, dataTokens);
                    }
                    else
                    {
                        throw new Exception($"{template} 路由规则默认信息无效！");
                    }

                    foreach (var _keys in extension.Ashxes)
                    {
                        string _template = template.Replace(ParameterPart(actionpart), _keys.Key);// $"{{{action}?}}"
                        AddRoute(RoutePatternFactory.Parse(_template, GetRouteValue(_area: areaName, _controller: defaults0, _action: _keys.Key), null, null), 20, _keys.Value, constraintsDictionary, dataTokens);
                    }
                }
                else
                {
                    throw new Exception($"{template}，无{{{defaults0}}}控制器， 路由无效！");
                }
            }
            else
            {
                if (defaults0 is not null && defaults1 is not null)
                {
                    if (RouteDefaults.TryGetValue(String(defaults0), out var extension) && extension.Ashxes.TryGetValue(String(defaults1), out var ashx))
                    {
                        AddRoute(RoutePatternFactory.Parse(template, GetRouteValue(_area: areaName, _controller: defaults0, _action: defaults1), null, null), 10, ashx, constraintsDictionary, dataTokens);
                        return;
                    }
                }
                throw new Exception($"{template}，路由无效，或不支持！");
            }

            //foreach (var keys in _AshxBuilder.RouteDefaults)
            //{
            //    if (true)
            //    {
            //        string DisplayName = $"Ashx:{keys.Value.AshxType.FullName}.default";

            //        if (EndpointExists(DisplayName)) continue;

            //        //var _routePattern1 = RoutePatternFactory.Parse(template);//"{controller=" + keys.Key + "}/{action}/{id?}"
            //        //_routePattern1.Defaults.TryGetValue("controller", out object controller);
            //        //_routePattern1.Defaults.TryGetValue("action", out object action);

            //        if (keys.Key.EqualsNotCase(_controller?.ToString()) && keys.Value.Ashxes.TryGetValue(action?.ToString(), out var ashx))
            //        {
            //            string _template = template;
            //            foreach (var item in _routePattern.Parameters)
            //            {
            //                _template = _template.Replace($"{{{item.Name}{(item.IsOptional ? "?" : $"={item.Default}")}}}", "").Replace("//", "/");
            //            }
            //            bool iscontroller = false;
            //        A:
            //            if (iscontroller)
            //            {
            //                DisplayName += $".{_controller}";
            //                _template = $"{_template}{_controller}";
            //            }

            //            //string _template = $"{_routePattern1.PathSegments}"; //template.Replace($"{{controller={controller}}}/", controller).Replace($"{{action={action}}}", "");
            //            var routePattern = RoutePatternFactory.Parse(_template, defaultsDictionary, null, null); //RoutePatternFactory.Pattern(null, parameterPolicies: null, _routePattern1.PathSegments);//_defaultsDictionary

            //            var routeEndpointBuilder = new RouteEndpointBuilder(requestDelegate: InitialLoadAsync, routePattern: routePattern, order: 0)
            //            {
            //                DisplayName = DisplayName
            //            };

            //            routeEndpointBuilder.Metadata.Add(keys.Value);
            //            routeEndpointBuilder.Metadata.Add(ashx);
            //            routeEndpointBuilder.Metadata.Add(new { areaName });
            //            routeEndpointBuilder.Metadata.Add(constraintsDictionary);
            //            routeEndpointBuilder.Metadata.Add(new RouteValueDictionary(dataTokens));

            //            _endpointBuilders.Add(routeEndpointBuilder);

            //            if (!iscontroller) { iscontroller = true; goto A; }
            //        }
            //    }

            //    foreach (var _keys in keys.Value.Ashxes)
            //    {
            //        var _defaultsDictionary = new RouteValueDictionary();
            //        _defaultsDictionary.TryAdd(controller, keys.Key);
            //        _defaultsDictionary.TryAdd(action, _keys.Key);

            //        if (!string.IsNullOrWhiteSpace(areaName) && !areaName.Equals(keys.Value.AshxType.Namespace)) continue;

            //        string DisplayName = $"Ashx:{keys.Value.AshxType.FullName}.{_keys.Key}()";

            //        if (EndpointExists(DisplayName)) continue;

            //        //var _routePattern1 = RoutePatternFactory.Parse($"{{controller={keys.Key}}}/{{action={_keys.Key}}}");//"{controller=" + keys.Key + "}/{action}/{id?}"

            //        var routePattern = RoutePatternFactory.Parse(template, defaultsDictionary, null, _defaultsDictionary); //RoutePatternFactory.Pattern(null, parameterPolicies: null, _routePattern1.PathSegments);//_defaultsDictionary

            //        var routeEndpointBuilder = new RouteEndpointBuilder(requestDelegate: InitialLoadAsync, routePattern: routePattern, order: 1)
            //        {
            //            DisplayName = DisplayName
            //        };

            //        routeEndpointBuilder.Metadata.Add(keys.Value);
            //        routeEndpointBuilder.Metadata.Add(_keys.Value);
            //        routeEndpointBuilder.Metadata.Add(new { areaName });
            //        routeEndpointBuilder.Metadata.Add(constraintsDictionary);
            //        routeEndpointBuilder.Metadata.Add(new RouteValueDictionary(dataTokens));

            //        _endpointBuilders.Add(routeEndpointBuilder);
            //    }
            //}

            //string GetPattern(ref System.Text.StringBuilder sb, IEnumerable<RoutePatternPathSegment> segments)
            //{
            //    if (sb == null)
            //    {
            //        sb = new System.Text.StringBuilder();
            //    }

            //    //RoutePatternWriter.WriteString(sb, segments);
            //    var rawPattern = sb.ToString();
            //    sb.Length = 0;

            //    return rawPattern;
            //}

            static string String(object obj)
            {
                if (obj is null) return string.Empty;
                return obj.ToString();
            }
        }

        private bool EndpointExists(string displayName)
        {
            return _endpointBuilders.Exists(match => match.DisplayName == displayName);
        }

        private ReadOnlyCollection<Endpoint> GetEndpoints()
        {
            var _endpoints = new List<Endpoint>();
            foreach (var routeEndpoint in _endpointBuilders)
            {
                _convention?.Invoke(routeEndpoint);
                _endpoints.Add(routeEndpoint.Build());
            }
            return _endpoints.AsReadOnly();
        }

        private async Task InitialLoadAsync(HttpContext context)
        {
            //RouteTemplate routeTemplate = new(endpoint.RoutePattern);

            //TemplateMatcher templateMatcher = new(routeTemplate, null);

            AshxRouteData _routeData = _AshxBuilder.Filter(context); //IRoutingFeature
            AshxEndpointDataSource.AshxLoad(_routeData);
            await _routeData.Handler(context);
        }

        private static void AshxLoad(AshxRouteData routeData)
        {
            var context = routeData.HttpContext;
            var endpoint = context.Features.Get<Http.Features.IEndpointFeature>()?.Endpoint as RouteEndpoint;
            var ashx = endpoint.Metadata.GetMetadata<Ashx>();

            routeData.GetAshx = ashx;
        }

        void IDisposable.Dispose()
        {
            _endpointBuilders.Clear();
            _convention = null;
            GC.SuppressFinalize(this);
        }

        //private void EnsureRequiredValuesInDefaults(
        //    IDictionary<string, string> routeValues,
        //    RouteValueDictionary defaults,
        //    IEnumerable<RoutePatternPathSegment> segments)
        //{
        //    foreach (var kvp in routeValues)
        //    {
        //        if (kvp.Value != null)
        //        {
        //            defaults[kvp.Key] = kvp.Value;
        //        }
        //    }
        //}
    }
}
