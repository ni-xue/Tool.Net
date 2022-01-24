using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.AspNetCore.Routing.Template;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Tool;
using Tool.Utils;
using Tool.Web;
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

            //if (options.Value.IsException)
            //{
            //    //app.ApplicationServices.GetRequiredService<I>();
            //}
            //Microsoft.AspNetCore.Routing.DefaultEndpointRouteBuilder
            builder.Application = endpoints.GetValue("ApplicationBuilder") as IApplicationBuilder;// endpoints.CreateApplicationBuilder();

            builder.Options = options.Value;

            ILoggerFactory loggerFactory = endpoints.ServiceProvider.GetRequiredService<ILoggerFactory>();

            builder.Logger = loggerFactory.CreateLogger("Tool.Api");

            builder.Logger.LogWarning("您好，当前您正在使用的是公测版本的终结点模式，目前还不支持自定义路由，如使用过程中发现任何问题，都可以邮件告知！");

            //创建默认mvc处理类 
            //RouteBuilder为RouterMiddleware中间件创建所需的Router对象

            //endpoints.ServiceProvider.GetService(typeof(IConfiguration)) as IConfiguration
            var ashxEndpoint = builder.AshxEndpoint.InitialLoad(configureRoutes);

            endpoints.DataSources.Add(ashxEndpoint);

            //var routes = new RouteBuilder(app)
            //{
            //    DefaultHandler = builder.AshxRoute,
            //};

            // 获取当前动态配置的路由模式
            //builder.RegisterAshxRoute(routes);

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

        internal ILogger Logger => _AshxBuilder.Logger;

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
            //_AshxBuilder.RegisterAshxRoute(this);
            Logger.LogWarning("动态路由模块暂不处理！[AshxRoute]");

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
            foreach (var keys in _AshxBuilder.RouteDefaults)
            {
                if (true)
                {
                    string DisplayName = $"Ashx:{keys.Value.AshxType.FullName}.default";

                    if (EndpointExists(DisplayName)) continue;

                    var _routePattern1 = RoutePatternFactory.Parse(template);//"{controller=" + keys.Key + "}/{action}/{id?}"
                    _routePattern1.Defaults.TryGetValue("controller",out object controller);
                    _routePattern1.Defaults.TryGetValue("action", out object action);

                    if (keys.Key.EqualsNotCase(controller.ToString()) && keys.Value.Ashxes.TryGetValue(action.ToString(), out var ashx))
                    {
                        string _template = template;
                        foreach (var item in _routePattern1.Parameters)
                        {
                            _template = _template.Replace($"{{{item.Name}{(item.IsOptional ? "?" : $"={item.Default}")}}}", "").Replace("//", "/");
                        }
                        bool iscontroller = false;
                    A:
                        if (iscontroller)
                        {
                            DisplayName += $".{controller}";
                            _template = $"{_template}{controller}";
                        }

                        //string _template = $"{_routePattern1.PathSegments}"; //template.Replace($"{{controller={controller}}}/", controller).Replace($"{{action={action}}}", "");
                        var routePattern = RoutePatternFactory.Parse(_template, defaultsDictionary, null, null); //RoutePatternFactory.Pattern(null, parameterPolicies: null, _routePattern1.PathSegments);//_defaultsDictionary

                        var routeEndpointBuilder = new RouteEndpointBuilder(requestDelegate: InitialLoadAsync, routePattern: routePattern, order: 0)
                        {
                            DisplayName = DisplayName
                        };

                        routeEndpointBuilder.Metadata.Add(keys.Value);
                        routeEndpointBuilder.Metadata.Add(ashx);
                        routeEndpointBuilder.Metadata.Add(new RouteValueDictionary(dataTokens));
                        routeEndpointBuilder.Metadata.Add(new { areaName });

                        _endpointBuilders.Add(routeEndpointBuilder);

                        if (!iscontroller) { iscontroller = true; goto A; }
                    }
                }

                foreach (var _keys in keys.Value.Ashxes)
                {
                    var _defaultsDictionary = new RouteValueDictionary();
                    _defaultsDictionary.TryAdd("controller", keys.Key);
                    _defaultsDictionary.TryAdd("action", _keys.Key);

                    if (!string.IsNullOrWhiteSpace(areaName) && !areaName.Equals(keys.Value.AshxType.Namespace)) continue;

                    string DisplayName = $"Ashx:{keys.Value.AshxType.FullName}.{_keys.Key}()";

                    if (EndpointExists(DisplayName)) continue;

                    //var _routePattern1 = RoutePatternFactory.Parse($"{{controller={keys.Key}}}/{{action={_keys.Key}}}");//"{controller=" + keys.Key + "}/{action}/{id?}"

                    var routePattern = RoutePatternFactory.Parse(template, defaultsDictionary, null, _defaultsDictionary); //RoutePatternFactory.Pattern(null, parameterPolicies: null, _routePattern1.PathSegments);//_defaultsDictionary

                    var routeEndpointBuilder = new RouteEndpointBuilder(requestDelegate: InitialLoadAsync, routePattern: routePattern, order: 1)
                    {
                        DisplayName = DisplayName
                    };

                    routeEndpointBuilder.Metadata.Add(keys.Value);
                    routeEndpointBuilder.Metadata.Add(_keys.Value);
                    routeEndpointBuilder.Metadata.Add(new RouteValueDictionary(dataTokens));
                    routeEndpointBuilder.Metadata.Add(new { areaName });

                    _endpointBuilders.Add(routeEndpointBuilder);
                }
            }

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
        }

        private bool EndpointExists(string displayName)
        {
            return _endpointBuilders.Exists(match => match.DisplayName == displayName);
        }

        private IReadOnlyList<Endpoint> GetEndpoints()
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

            if (_AshxBuilder.Options.IsAsync)
            {
                await Task.Run(async () => await AshxLoadAsync(_routeData));
            }
            else
            {
                await AshxLoadAsync(_routeData);
            }
        }

        private async Task AshxLoadAsync(AshxRouteData routeData)
        {
            var context = routeData.HttpContext;
            var endpoint = context.Features.Get<Http.Features.IEndpointFeature>()?.Endpoint as RouteEndpoint;

            var ashxExtension = endpoint.Metadata.GetMetadata<AshxExtension>();

            var ashx = endpoint.Metadata.GetMetadata<Tool.Web.Api.Ashx>();

            routeData.GetAshx = ashx;

            routeData.SetKey();

            bool isOptions()
            {
                if (AshxExtension.CrossDomain(routeData.HttpContext.Response, routeData.GetAshx) && routeData.HttpContext.Request.Method.EqualsNotCase("OPTIONS"))
                {
                    Info(routeData);
                    return true;
                }
                return false;
            }

            if (isOptions()) return;

            if (!AshxExtension.HttpMethod(routeData.HttpContext.Request.Method, ashx.State))
            {
                //routeData.Handler = async (HttpContext i) => await AshxHandlerOrAsync.CustomOutput(i, "application/json", "请求类型错误！", 403);
                await AshxHandlerOrAsync.CustomOutput(routeData.HttpContext, "application/json", "请求类型错误！", 403);
                return;
            }

            if (routeData.GetAshx.IsMinApi)
            {
                async Task minapi()
                {
                    IMinHttpAsynApi handler = ashxExtension.MinHttpAsynApi;

                    if (AshxHandlerOrAsync.MinInitialize(handler, routeData))
                    {
                        if (AshxHandlerOrAsync.GetMinObj(routeData, out object[] _objs))
                        {
                            await AshxHandlerOrAsync.CustomOutput(routeData.HttpContext, "application/json", "框架错误，请查看日志文件！", 500);
                        }
                        else
                        {
                            Info(routeData);

                            if (ashx.IsTask)
                            {
                                await AshxHandlerOrAsync.StartMinAsyncAshx(handler, routeData, _objs);
                            }
                            else
                            {
                                await AshxHandlerOrAsync.StartMinAshx(handler, routeData, _objs);
                            }
                        }
                    }
                }

                await minapi();
            }
            else
            {
                async Task ashxapi()
                {
                    IHttpAsynApi handler;
                    try
                    {
                        handler = ashxExtension.NewClassAshx(context.RequestServices);
                    }
                    catch (Exception e)
                    {
                        throw routeData.HttpContext.AddHttpException(404, string.Format(System.Globalization.CultureInfo.CurrentCulture, "在创建时发生异常，应该是使用有参构造函数，URL: {0}", new object[] { routeData.HttpContext.Request.Path }), e);
                    }

                    if (AshxHandlerOrAsync.Initialize(handler, routeData))//初始加载
                    {
                        if (AshxHandlerOrAsync.GetObj(routeData, out object[] _objs))
                        {
                            await AshxHandlerOrAsync.CustomOutput(routeData.HttpContext, "application/json", "框架错误，请查看日志文件！", 500);
                        }
                        else
                        {
                            Info(routeData);

                            if (ashx.IsTask)
                            {
                                await AshxHandlerOrAsync.StartAsyncAshx(handler, _objs);
                            }
                            else
                            {
                                await AshxHandlerOrAsync.StartAshx(handler, _objs);
                            }
                        }
                    }
                }

                await ashxapi();
            }

        }

        void IDisposable.Dispose()
        {
            _endpointBuilders.Clear();
            _convention = null;
        }

        private void EnsureRequiredValuesInDefaults(
            IDictionary<string, string> routeValues,
            RouteValueDictionary defaults,
            IEnumerable<RoutePatternPathSegment> segments)
        {
            foreach (var kvp in routeValues)
            {
                if (kvp.Value != null)
                {
                    defaults[kvp.Key] = kvp.Value;
                }
            }
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
            }
        }
    }
}
