using Microsoft.Extensions.DependencyInjection;
using System;

namespace Tool.Web.Hosting
{
    /// <summary>
    /// Diy 依赖注入工厂
    /// </summary>
    public class DiyServiceProviderFactory : IServiceProviderFactory<DiyContainerBuilder>
    {
        private readonly DiyContainerBuilder _applicationContainerBuilder;

        /// <summary>
        /// 初始化
        /// </summary>
        public DiyServiceProviderFactory()
        {
            _applicationContainerBuilder = new DiyContainerBuilder();
        }

        /// <summary>
        /// 创建一个新的 <see cref="DiyContainerBuilder"/> 用于注册服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public DiyContainerBuilder CreateBuilder(IServiceCollection services)
        {
            _applicationContainerBuilder.Populate(services);
            return _applicationContainerBuilder;
        }

        /// <summary>
        /// 使用 <see cref="DiyContainerBuilder"/> 构建 Autofac 容器，并返回一个 <see cref="IServiceProvider"/>
        /// </summary>
        /// <param name="containerBuilder"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public IServiceProvider CreateServiceProvider(DiyContainerBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }
            var container = containerBuilder.Build();
            return container;
        }
    }
}
