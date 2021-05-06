using System;
using System.Collections.Generic;
using System.Text;
using Tool.Web.Routing;

namespace Tool.Web.Api.ApiCore
{

    /// <summary>
    /// Tool 框架自定接口，同步 （最小API）
    /// </summary>
    public interface IMinHttpApi
    {
        /// <summary>
        /// 验证用户验证操作
        /// </summary>
        /// <param name="ashxRoute">对象</param>
        /// <returns></returns>
        bool Initialize(AshxRouteData ashxRoute);

        /// <summary>
        /// 通过实现 注入实现 接口的自定义 HttpHandler 启用 HTTP Web 请求的处理。
        /// </summary>
        void Request(AshxRouteData ashxRoute, object[] _objs);

    }
}
