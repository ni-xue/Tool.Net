using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Web.Builder
{
    /// <summary>
    /// Ashx核心标准接口
    /// </summary>
    public interface IAshxBuilder
    {
        /// <summary>
        /// 容器核心对象
        /// </summary>
        IServiceCollection Services
        {
            get;
        }

        /// <summary>
        /// 是否注入HttpContext对象
        /// </summary>
        bool IsHttpContext 
        {
            get;
            set;
        }

        /// <summary>
        /// 提供整个请求服务者（依赖注入）
        /// </summary>
        IApplicationBuilder Application
        {
            get;
            set;
        }
    }
}
