using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tool.Web.Routing;

namespace Tool.Web.Api.ApiCore
{
    /// <summary>
    /// Tool 框架自定接口，异步（最小API）
    /// </summary>
    public interface IMinHttpAsynApi : IMinHttpApi
    {
        /// <summary>
        /// 启动对 HTTP 处理程序的异步调用。
        /// </summary>
        /// <param name="ashxRoute">路由信息对象</param>
        /// <param name="_objs">处理该请求所需的数据。</param>
        /// <returns>包含有关线程状态信息的 Task。</returns>
        Task TaskRequest(AshxRouteData ashxRoute, object[] _objs);

        /// <summary>
        /// 完成后任务
        /// </summary>
        /// <param name="task">任务信息</param>
        /// <param name="ashxRoute">路由信息对象</param>
        void ContinueWith(Task task, AshxRouteData ashxRoute);
    }
}
