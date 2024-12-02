using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Utils
{
    /// <summary>
    /// 用于提供全局支持的 TOC 对象
    /// 
    /// <para>服务生命周期</para>
    /// <para>在Microsoft依赖项注入框架中，我们可以使用三种生命周期注册服务，分别是单例（Singleton）、瞬时（Transient）、作用域（Scoped），在上面的代码中，
    /// 我使用了AddSingleton()来注册服务。</para>
    /// <para>使用Singleton服务的优点是我们不会创建多个服务实例，只会创建一个实例，保存到DI容器中，直到程序退出，这不仅效率高，而且性能高，但是有一个要注意的点，
    /// 如果在多线程中使用了Singleton,要考虑线程安全的问题，保证它不会有冲突。</para>
    /// <para>瞬时（Transient）和单例（Singleton）模式是相反的，每次使用时，DI容器都是创建一个新的实例。</para>
    /// <para>作用域（Scoped），在一个作用域内，会使用同一个实例，像EF Core的DbContext上下文就被注册为作用域服务。</para>
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class IocHelper
    {
        /// <summary>
        /// 提供全局，使用的 依赖注入 服务
        /// </summary>
        public static IocCore IocCore { get; }

        static IocHelper() 
        {
            IocCore = new();
        }

        /// <summary>
        /// 获取一个全新的 IOC 容器对象
        /// </summary>
        /// <returns>IOC 容器对象</returns>
        public static IocCore NewIoc() 
        {
            return new();
        }
    }

    /// <summary>
    /// 提供一个完全独立的 Ioc 容器
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class IocCore : IAsyncDisposable, IDisposable
    {
        /// <summary>
        /// 注册服务存储对象
        /// </summary>
        private readonly ServiceCollection _services;

        /// <summary>
        /// 获取服务对象
        /// </summary>
        private ServiceProvider _provider;

        /// <summary>
        /// 初始化
        /// </summary>
        public IocCore()
        {
            //使用ServiceCollaction对象的扩展方法进行注册服务
            _services = new();

            //Build();
            //ServiceCollectionContainerBuilderExtensions
        }

        /// <summary>
        /// 创建用于获取服务对象
        /// <para>调用该函数，将会释放掉原本的服务</para>
        /// </summary>
        public void Build() 
        {
            Dispose();
            _provider = Services.BuildServiceProvider(new ServiceProviderOptions { ValidateOnBuild = true });
        }

        /// <summary>
        /// 清空所有已注册的服务
        /// </summary>
        public void RemoveAll() 
        {
            _services.Clear();
        }

        /// <summary>
        /// 异步释放相关资源
        /// </summary>
        /// <returns></returns>
        public ValueTask DisposeAsync()
        {
            if (_provider != null)
            {   GC.SuppressFinalize(this);
                return _provider.DisposeAsync();
            }
            return ValueTask.CompletedTask;
        }

        /// <summary>
        /// 释放相关资源
        /// </summary>
        public void Dispose()
        {
            if (_provider != null)
            {
                GC.SuppressFinalize(this);
                _provider.Dispose();
            }
        }

        /// <summary>
        /// 提供用于添加对象服务
        /// </summary>
        public IServiceCollection Services => _services;

        /// <summary>
        /// 提供用于获取注入对象的服务
        /// </summary>
        public IServiceProvider Provider => _provider ?? throw new Exception("请先调用Build()，函数提供服务。");
    }
}
