using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Utils.ThreadQueue
{
    /// <summary>
    /// 创建任务执行对象
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class WaitAction : IDisposable
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
        public object TResult { get; private set; }

        private WaitAction(object state)
        {
            this.WaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            this.State = state;
        }

        /// <summary>
        /// 创建有返回结果的任务
        /// </summary>
        /// <param name="func">任务</param>
        /// <param name="state">参数</param>
        public WaitAction(Func<object, object> func, object state) : this(state)
        {
            this.Func = func;
        }

        /// <summary>
        /// 创建无返回结果的任务
        /// </summary>
        /// <param name="action">任务</param>
        /// <param name="state">参数</param>
        public WaitAction(Action<object> action, object state) : this(delegate (object obj)
        {
            action(obj);
            return null;
        }, state)
        {
        }

        /// <summary>
        /// 其他线程中可用等待获取的任务结果
        /// </summary>
        /// <param name="waitAction">结果</param>
        /// <returns>返回成功失败</returns>
        public bool Wait(out WaitAction waitAction)
        {
            if (this.IsCompleted)
            {
                waitAction = this;
                return true;
            }
            if (this.IsWait)
            {
                throw new Exception("当前任务已经处于等待状态！");
            }
            this.IsWait = true;
            if (this.WaitHandle.WaitOne(this.WaitTimeout))
            {
                waitAction = this;
                return true;
            }
            else
            {
                this.Exception = new Exception("当前等待超时，稍后依然会完成！");
                this.IsException = true;
            }
            this.IsWait = false;
            waitAction = this;
            return false;
        }

        /// <summary>
        /// 启动已就绪的任务
        /// </summary>
        public void Run()
        {
            if (IsCompleted) return;
            if (this.Func != null)
            {
                try
                {
                    this.TResult = this.Func(this.State);
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
            if (this.IsWait)
            {
                this.WaitHandle.Set();
            }
        }

        /// <summary>
        /// 回收对象
        /// </summary>
        public void Dispose()
        {
            this.WaitHandle.Dispose();
            this.TResult = null;
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
        private Func<object, object> Func { get; }

        /// <summary>
        /// 需要的参数
        /// </summary>
        private object State { get; }

        /// <summary>
        /// 任务的事件
        /// </summary>
        private EventWaitHandle WaitHandle { get; }
    }
}
