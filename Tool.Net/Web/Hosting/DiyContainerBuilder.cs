using Microsoft.AspNetCore.Html;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Tool.Web.Hosting
{
    /// <summary>
    /// Diy容器数据
    /// </summary>
    public class DiyContainerBuilder
    {
        private readonly Utils.IocCore iocCore;

        /// <summary>
        /// 初始化
        /// </summary>
        public DiyContainerBuilder() 
        {
            iocCore = Utils.IocHelper.IocCore;
        }

        /// <summary>
        /// 将默认的依赖注入的对象填入新的容器
        /// </summary>
        /// <param name="services">服务</param>
        public void Populate(IServiceCollection services) 
        {
            foreach (var item in services)
            {
                iocCore.Services.Add(item);
            }
        }

        /// <summary>
        /// 完成服务模型注册
        /// </summary>
        /// <returns></returns>
        public IServiceProvider Build() 
        {
            iocCore.Build();
            return iocCore.Provider;
        }
    }
}
