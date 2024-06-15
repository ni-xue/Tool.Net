using System.Threading.Tasks;
using System.Threading;
using System;

namespace Tool.Utils.ThreadQueue
{

    /// <summary>
    /// 创建任务执行对象
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class WaitAction<T, TResult> : IDisposable
    {
        /// <summary>
        /// Wait函数最大等待时长 -1 无限制等待
        /// </summary>
        public int WaitTimeout { get; set; } = 120000;

        /// <summary>
        /// 任务完成情况
        /// </summary>
        public bool IsCompleted { get; private set; }

        /// <summary>
        /// 是否发生异常
        /// </summary>
        public bool IsException { get; private set; }

        /// <summary>
        /// 任务执行中发生的异常
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// 是否正在调用Wait函数
        /// </summary>
        public bool IsWait { get; private set; }

        /// <summary>
        /// 任务完成后的结果
        /// </summary>
        public TResult Result { get; private set; }

        /// <summary>
        /// 需要的参数
        /// </summary>
        public T State { get; }

        private WaitAction(T state)
        {
            this.WaitHandle = new(false, 100);
            this.State = state;
        }

        /// <summary>
        /// 创建有返回结果的任务
        /// </summary>
        /// <param name="func">任务</param>
        /// <param name="state">参数</param>
        public WaitAction(Func<T, ValueTask<TResult>> func, T state) : this(state)
        {
            this.Func = func;
        }

        /// <summary>
        /// 创建无返回结果的任务
        /// </summary>
        /// <param name="action">任务</param>
        /// <param name="state">参数</param>
        public WaitAction(Action<T> action, T state) : this(delegate (T obj)
        {
            action(obj);
            return default;
        }, state)
        {
        }

        /// <summary>
        /// 其他线程中可用等待获取的任务结果
        /// </summary>
        /// <returns>返回成功失败</returns>
        public bool Wait()
        {
            if (this.IsCompleted) return true;
            if (this.IsWait) throw new Exception("当前任务已经处于等待状态！");
            this.IsWait = true;
            if (this.WaitHandle.Wait(this.WaitTimeout)) return true;
            else
            {
                this.Exception = new Exception("当前等待超时，稍后依然会完成！");
                this.IsException = true;
            }
            return this.IsWait = false;
        }

        /// <summary>
        /// 启动已就绪的任务
        /// </summary>
        public async ValueTask Run()
        {
            if (IsCompleted) return;
            if (this.Func != null)
            {
                try
                {
                    this.Result = await this.Func(this.State);
                    this.Exception = null;
                    this.IsException = false;
                }
                catch (Exception exception)
                {
                    this.Exception = exception;
                    this.IsException = true;
                }
            }
            this.IsCompleted = true;
            if (this.IsWait) this.WaitHandle.Set();
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Dispose()
        {
            this.WaitHandle.Dispose();
            this.Result = default;
            GC.SuppressFinalize(this);
        }

        ///// <summary>
        ///// 回收吧
        ///// </summary>
        //~WaitAction()
        //{
        //    this.Dispose();
        //}

        /// <summary>
        /// 执行的任务
        /// </summary>
        private Func<T, ValueTask<TResult>> Func { get; }

        /// <summary>
        /// 任务的事件
        /// </summary>
        private ManualResetEventSlim WaitHandle { get; }
    }
}
