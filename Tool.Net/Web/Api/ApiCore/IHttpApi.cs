using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tool.Web.Routing;

namespace Tool.Web.Api.ApiCore
{
    /// <summary>
    /// Tool 框架自定接口，同步
    /// </summary>
    public interface IHttpApi : IDisposable
    {
        /// <summary>
        /// 初始化绑定路由规则，必须第一个调用
        /// </summary>
        /// <param name="ashxRoute">路由规则信息</param>
        void SetRouteData(AshxRouteData ashxRoute);

        /// <summary>
        /// 验证用户验证操作
        /// </summary>
        /// <param name="ashx">对象</param>
        /// <returns></returns>
        bool Initialize(Ashx ashx);

        /// <summary>
        /// 通过实现 注入实现 接口的自定义 HttpHandler 启用 HTTP Web 请求的处理。
        /// </summary>
        void Request(object[] _objs);

        ///// <summary>
        ///// 路由
        ///// </summary>
        //AshxRouteData RouteData { get; }

        ///// <summary>
        ///// 获取当前请求的接口唯一ID
        ///// </summary>
        //string ApiKey { get; }
    }
}
