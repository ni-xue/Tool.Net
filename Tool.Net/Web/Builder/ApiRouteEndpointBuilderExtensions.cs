using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.Routing.Patterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.AspNetCore.Builder
{
    /// <summary>
    /// 注册 Ashx 终结点路由的扩展模式
    /// </summary>
    public static class ApiRouteEndpointBuilderExtensions
    {
        /// <summary>
        /// 创建与AshxApi有关的路由
        /// </summary>
        /// <param name="endpointData">路由核心对象</param>
        /// <param name="template">路由的规则</param>
        /// <returns>路由核心对象</returns>
        public static EndpointDataSource MapApiRoute(
            this EndpointDataSource endpointData,
            string template)
        {
            return MapApiRoute(endpointData, null, template);
        }

        /// <summary>
        /// 创建与AshxApi有关的路由
        /// </summary>
        /// <param name="endpointData">路由核心对象</param>
        /// <param name="areaName">区域名称，可为null</param>
        /// <param name="template">路由的规则</param>
        /// <returns>路由核心对象</returns>
        public static EndpointDataSource MapApiRoute(
            this EndpointDataSource endpointData,
            string areaName,
            string template)
        {
            return MapApiRoute(endpointData, areaName, controller: null, template);
        }

        /// <summary>
        /// 创建与AshxApi有关的路由
        /// </summary>
        /// <param name="endpointData">路由核心对象</param>
        /// <param name="areaName">区域名称，可为null</param>
        /// <param name="controller">控制器名称，可为null</param>
        /// <param name="template">路由的规则</param>
        /// <returns>路由核心对象</returns>
        public static EndpointDataSource MapApiRoute(
            this EndpointDataSource endpointData,
            string areaName,
            string controller,
            string template)
        {
            return MapApiRoute(endpointData, areaName, controller, action: null, template);
        }

        /// <summary>
        /// 创建与AshxApi有关的路由
        /// </summary>
        /// <param name="endpointData">路由核心对象</param>
        /// <param name="areaName">区域名称，可为null</param>
        /// <param name="controller">控制器名称，可为null</param>
        /// <param name="action">方法名称，可为null</param>
        /// <param name="template">路由的规则</param>
        /// <returns>路由核心对象</returns>
        public static EndpointDataSource MapApiRoute(
            this EndpointDataSource endpointData,
            string areaName,
            string controller,
            string action,
            string template)
        {
            object defaults = null;
            if (controller != null && action != null)
            {
                defaults = new { controller, action };
            }
            else if (controller != null)
            {
                defaults = new { controller };
            }
            else if (action != null)
            {
                defaults = new { action };
            }

            return MapApiRoute(endpointData, areaName, template, defaults: defaults, constraints: null, dataTokens: null);
        }

        /// <summary>
        /// 创建与AshxApi有关的路由
        /// </summary>
        /// <param name="endpointData">路由核心对象</param>
        /// <param name="areaName">区域名称，可为null</param>
        /// <param name="template">路由的规则</param>
        /// <param name="defaults">包含路由参数默认值的对象。对象的属性表示，默认值的名称和值，可为null。</param>
        /// <param name="constraints">包含路由约束的对象。对象的属性表示，约束的名称和值，可为null。</param>
        /// <param name="dataTokens">包含路由的数据标记的对象。对象的属性表示，数据标记的名称和值，可为null。</param>
        /// <returns>路由核心对象</returns>
        public static EndpointDataSource MapApiRoute(
            this EndpointDataSource endpointData,
            string areaName,
            //string controller,
            //string action,
            string template,
            object defaults,
            object constraints,
            object dataTokens)
        {
            if (endpointData == null)
            {
                throw new ArgumentNullException(nameof(endpointData));
            }

            var defaultsDictionary = new RouteValueDictionary(defaults);
            //if (!string.IsNullOrEmpty(areaName)) defaultsDictionary["area"] = defaultsDictionary["area"] ?? areaName;

            //if (!string.IsNullOrEmpty(controller)) defaultsDictionary["controller"] = defaultsDictionary["controller"] ?? controller;

            //if (!string.IsNullOrEmpty(action)) defaultsDictionary["action"] = defaultsDictionary["action"] ?? action;

            var constraintsDictionary = new RouteValueDictionary(constraints);
            //if (!string.IsNullOrEmpty(areaName)) constraintsDictionary["area"] = constraintsDictionary["area"] ?? new StringRouteConstraint(areaName);

            //if (!string.IsNullOrEmpty(controller)) constraintsDictionary["controller"] = constraintsDictionary["controller"] ?? new StringRouteConstraint(controller);

            //if (!string.IsNullOrEmpty(action)) constraintsDictionary["action"] = constraintsDictionary["action"] ?? new StringRouteConstraint(action);


            //AshxEndpointDataSource

            if (endpointData is AshxEndpointDataSource dataSource)
            {
                dataSource.MapRoute(template, areaName, defaultsDictionary, constraintsDictionary, dataTokens);
            }
            else
            {
                throw new Exception("EndpointDataSource 对象 必须实现至 AshxEndpointDataSource 类，无权调用！");
            }
            return endpointData;
        }
    }
}
