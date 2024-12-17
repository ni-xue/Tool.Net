using System.Threading.Tasks;
using System.Threading;
using System;
using System.Runtime.CompilerServices;

namespace Tool.Utils.TaskHelper
{
    /// <summary>
    /// 允许自定义触发异步任务
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class TaskWithTimeout : TaskCompletionSource, IDisposable
    {
        private readonly CancellationTokenSource cts;

        /// <summary>
        /// 是否被取消
        /// </summary>
        public bool IsCancellationRequested => cts.IsCancellationRequested;

        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout {  get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="timeout">超时时间</param>
        public TaskWithTimeout(TimeSpan timeout) : this(timeout, null) { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <param name="state">带传递值</param>
        public TaskWithTimeout(TimeSpan timeout, object state) : this(timeout, state, TaskCreationOptions.None) { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <param name="creationOptions">任务枚举类型</param>
        public TaskWithTimeout(TimeSpan timeout, TaskCreationOptions creationOptions) : this(timeout, null, creationOptions) {  }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <param name="state">带传递值</param>
        /// <param name="creationOptions">任务枚举类型</param>
        public TaskWithTimeout(TimeSpan timeout, object state, TaskCreationOptions creationOptions) : base(state, creationOptions) 
        {
            Timeout = timeout;
            cts = new CancellationTokenSource(Timeout);
            cts.Token.UnsafeRegister((state) =>
            {
                TrySetCanceled();
            }, state);
        }

        /// <summary>
        /// 返回任务调度器
        /// </summary>
        /// <returns></returns>
        public TaskAwaiter GetAwaiter() 
        {
            return Task.GetAwaiter();
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        public void Dispose()
        {
            cts.Dispose();
            GC.SuppressFinalize(this);
        }

        #region 执行可以设置超时时间的任务

        /// <summary>
        /// 公共任务，执行时允许设置超时时间
        /// </summary>
        /// <typeparam name="T">返回泛型</typeparam>
        /// <param name="taskFactory">任务</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>任务</returns>
        /// <exception cref="TimeoutException">超时错误</exception>
        public static async Task<T> RunWithTimeout<T>(Func<Task<T>> taskFactory, TimeSpan timeout)
        {
            var tcs = new TaskCompletionSource<T>();
            using var cts = new CancellationTokenSource(timeout);

            // Listen to cancellation
            cts.Token.Register(() =>
            {
                tcs.TrySetCanceled();
            });

            try
            {
                // Start the provided task
                var task = taskFactory();
                // Wait for either the task to complete or the timeout
                var completedTask = await Task.WhenAny(task, tcs.Task);

                if (completedTask == tcs.Task)
                {
                    throw new TimeoutException("The operation timed out.");
                }

                // Return the result of the completed task
                return await task;
            }
            catch (Exception ex) when (!(ex is TimeoutException))
            {
                // Propagate non-timeout exceptions
                tcs.TrySetException(ex);
                throw;
            }
        }

        /// <summary>
        /// 公共任务，执行时允许设置超时时间（带取消模式的）
        /// </summary>
        /// <typeparam name="T">返回泛型</typeparam>
        /// <param name="taskFactory">任务</param>
        /// <param name="timeout">超时时间</param>
        /// <returns>任务</returns>
        /// <exception cref="TimeoutException">超时错误</exception>
        public static async Task<T> RunWithTimeout<T>(Func<CancellationToken, Task<T>> taskFactory, TimeSpan timeout)
        {
            using var cts = new CancellationTokenSource(timeout);

            try
            {
                // Pass the cancellation token to the task factory
                var task = taskFactory(cts.Token);

                // Wait for either the task to complete or timeout
                return await Task.WhenAny(task, Task.Delay(timeout, cts.Token)) == task
                    ? await task
                    : throw new TimeoutException("The operation timed out.");
            }
            catch (OperationCanceledException) when (cts.Token.IsCancellationRequested)
            {
                throw new TimeoutException("The operation timed out.");
            }
        }

        #endregion

    }

    /// <summary>
    /// 允许自定义触发异步任务
    /// </summary>
    /// <typeparam name="T">返回值</typeparam>
    /// <remarks>代码由逆血提供支持</remarks>
    public class TaskWithTimeout<T> : TaskCompletionSource<T>, IDisposable
    {
        private readonly CancellationTokenSource cts;

        /// <summary>
        /// 是否被取消
        /// </summary>
        public bool IsCancellationRequested => cts.IsCancellationRequested;

        /// <summary>
        /// 超时时间
        /// </summary>
        public TimeSpan Timeout { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="timeout">超时时间</param>
        public TaskWithTimeout(TimeSpan timeout) : this(timeout, null) { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <param name="state">带传递值</param>
        public TaskWithTimeout(TimeSpan timeout, object state) : this(timeout, state, TaskCreationOptions.None) { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <param name="creationOptions">任务枚举类型</param>
        public TaskWithTimeout(TimeSpan timeout, TaskCreationOptions creationOptions) : this(timeout, null, creationOptions) { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="timeout">超时时间</param>
        /// <param name="state">带传递值</param>
        /// <param name="creationOptions">任务枚举类型</param>
        public TaskWithTimeout(TimeSpan timeout, object state, TaskCreationOptions creationOptions) : base(state, creationOptions)
        {
            Timeout = timeout;
            cts = new CancellationTokenSource(Timeout);
            cts.Token.UnsafeRegister((state) =>
            {
                TrySetCanceled();
            }, state);
        }

        /// <summary>
        /// 返回任务调度器
        /// </summary>
        /// <returns><see cref="TaskAwaiter{T}"/></returns>
        public TaskAwaiter<T> GetAwaiter()
        {
            return Task.GetAwaiter();
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        public void Dispose()
        {
            cts.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
