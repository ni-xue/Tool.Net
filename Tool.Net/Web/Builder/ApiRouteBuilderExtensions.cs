using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Web.Builder
{
    /// <summary>
    /// Ashx 路由模式扩展
    /// </summary>
    public static class ApiRouteBuilderExtensions
    {

        /// <summary>
        /// 创建与AshxApi有关的路由
        /// </summary>
        /// <param name="routeBuilder">路由核心对象</param>
        /// <param name="name">路由规则名称</param>
        /// <param name="areaName">区域名称，可为null</param>
        /// <param name="template">路由的规则</param>
        /// <returns>路由核心对象</returns>
        public static IRouteBuilder MapApiRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string areaName,
            string template)
        {
            return MapApiRoute(routeBuilder, name, areaName, controller: null, template);
        }

        /// <summary>
        /// 创建与AshxApi有关的路由
        /// </summary>
        /// <param name="routeBuilder">路由核心对象</param>
        /// <param name="name">路由规则名称</param>
        /// <param name="areaName">区域名称，可为null</param>
        /// <param name="controller">控制器名称，可为null</param>
        /// <param name="template">路由的规则</param>
        /// <returns>路由核心对象</returns>
        public static IRouteBuilder MapApiRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string areaName,
            string controller,
            string template)
        {
            return MapApiRoute(routeBuilder, name, areaName, controller, action: null, template);
        }

        /// <summary>
        /// 创建与AshxApi有关的路由
        /// </summary>
        /// <param name="routeBuilder">路由核心对象</param>
        /// <param name="name">路由规则名称</param>
        /// <param name="areaName">区域名称，可为null</param>
        /// <param name="controller">控制器名称，可为null</param>
        /// <param name="action">方法名称，可为null</param>
        /// <param name="template">路由的规则</param>
        /// <returns>路由核心对象</returns>
        public static IRouteBuilder MapApiRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string areaName,
            string controller,
            string action,
            string template)
        {
            return MapApiRoute(routeBuilder, name, areaName, controller, action, template, defaults: null, constraints: null, dataTokens: null);
        }

        /// <summary>
        /// 创建与AshxApi有关的路由
        /// </summary>
        /// <param name="routeBuilder">路由核心对象</param>
        /// <param name="name">路由规则名称</param>
        /// <param name="areaName">区域名称，可为null</param>
        /// <param name="controller">控制器名称，可为null</param>
        /// <param name="action">方法名称，可为null</param>
        /// <param name="template">路由的规则</param>
        /// <param name="defaults">包含路由参数默认值的对象。对象的属性表示，默认值的名称和值，可为null。</param>
        /// <param name="constraints">包含路由约束的对象。对象的属性表示，约束的名称和值，可为null。</param>
        /// <param name="dataTokens">包含路由的数据标记的对象。对象的属性表示，数据标记的名称和值，可为null。</param>
        /// <returns>路由核心对象</returns>
        public static IRouteBuilder MapApiRoute(
            this IRouteBuilder routeBuilder,
            string name,
            string areaName,
            string controller,
            string action,
            string template,
            object defaults,
            object constraints,
            object dataTokens)
        {
            if (routeBuilder == null)
            {
                throw new ArgumentNullException(nameof(routeBuilder));
            }

            var defaultsDictionary = new RouteValueDictionary(defaults);
            if (!string.IsNullOrEmpty(areaName)) defaultsDictionary["area"] = defaultsDictionary["area"] ?? areaName;

            if (!string.IsNullOrEmpty(controller)) defaultsDictionary["controller"] = defaultsDictionary["controller"] ?? controller;

            if (!string.IsNullOrEmpty(action)) defaultsDictionary["action"] = defaultsDictionary["action"] ?? action;

            var constraintsDictionary = new RouteValueDictionary(constraints);
            if (!string.IsNullOrEmpty(areaName)) constraintsDictionary["area"] = constraintsDictionary["area"] ?? new StringRouteConstraint(areaName);

            if (!string.IsNullOrEmpty(controller)) constraintsDictionary["controller"] = constraintsDictionary["controller"] ?? new StringRouteConstraint(controller);

            if (!string.IsNullOrEmpty(action)) constraintsDictionary["action"] = constraintsDictionary["action"] ?? new StringRouteConstraint(action);

            routeBuilder.MapRoute(name, template, defaultsDictionary, constraintsDictionary, dataTokens);

            //MapAreaRoute(routeBuilder, name, areaName, template, defaults: null, constraints: null, dataTokens: null);
            return routeBuilder;
        }
    }
}
