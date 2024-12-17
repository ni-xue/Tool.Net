using Microsoft.Extensions.DependencyInjection;
using Tool;
using Tool.Web.Hosting;

namespace Microsoft.Extensions.Hosting
{
    /// <summary>
    /// HostBuilder扩展
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class HostBuilderExtensions
    {
        /// <summary>
        /// 依赖注入（采用自定义模式，替换掉 默认的容器）
        /// <list type="table">自动注册<see cref="ObjectExtension"/>.Services</list>
        /// <list type="table">自动注册<see cref="ObjectExtension"/>.Provider</list>
        /// </summary>
        /// <param name="builder">信息</param>
        /// <returns><see cref="IHostBuilder"/></returns>
        public static IHostBuilder UseDiyServiceProvider(this IHostBuilder builder)
        {
            return builder.UseServiceProviderFactory(new DiyServiceProviderFactory());
        }
    }
}
