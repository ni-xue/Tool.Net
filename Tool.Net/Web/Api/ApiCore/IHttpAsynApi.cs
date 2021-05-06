//using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Web.Api.ApiCore
{
    /// <summary>
    /// Tool 框架自定接口，异步
    /// </summary>
    public interface IHttpAsynApi : IHttpApi
    {
        /// <summary>
        /// 启动对 HTTP 处理程序的异步调用。
        /// </summary>
        /// <param name="_objs">处理该请求所需的数据。</param>
        /// <returns>包含有关线程状态信息的 Task。</returns>
        Task TaskRequest(object[] _objs);

        /// <summary>
        /// 完成后任务
        /// </summary>
        /// <param name="task">任务信息</param>
        void ContinueWith(Task task);
    }
}
