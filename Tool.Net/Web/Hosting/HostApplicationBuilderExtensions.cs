namespace Microsoft.Extensions.Hosting
{

#if NET8_0_OR_GREATER

    /// <summary>
    /// 服务扩展
    /// </summary>
    public static class HostApplicationBuilderExtensions
    {
        /// <summary>
        /// 替换原服务容器
        /// </summary>
        /// <param name="builder">服务核心</param>
        public static void UseDiyServiceProvider(this Microsoft.Extensions.Hosting.IHostApplicationBuilder builder)
        {
            builder.ConfigureContainer(UseDiyServiceProvider(), (builder) => { });

            static Tool.Web.Hosting.DiyServiceProviderFactory UseDiyServiceProvider()
            {
                return new Tool.Web.Hosting.DiyServiceProviderFactory();
            }
        }
    }

#endif
}
