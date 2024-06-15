using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Utils.ThreadQueue
{
    /// <summary>
    /// 一个(公共线程安全)消息队列任务模型
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class ActionQueue<T, TResult>
    {
        /// <summary>
        /// 分配的任务队列线程完成后最大保留时间
        /// </summary>
        public const int WaitTimeout = 60 * 1000;

        private static volatile int _islock = 0;

        private static readonly ConcurrentQueue<WaitAction<T, TResult>> queue;
        private static readonly ManualResetEventSlim ResetEven;

        private static Task task = null;

        /// <summary>
        /// 注册完成任务后触发的事件
        /// </summary>
        public static event Action<WaitAction<T, TResult>> ContinueWith;

        /// <summary>
        /// 表示当前事件是否已经注册
        /// </summary>
        public static bool IsContinueWith => ContinueWith != null;

        static ActionQueue()
        {
            _islock = new();
            queue = new();
            ResetEven = new ManualResetEventSlim(false, 100);
        }

        private static void PerformTask()
        {
            if (Interlocked.CompareExchange(ref _islock, 1, 0) == 1) { ResetEven.Set(); return; }

            if (task == null || task.IsCompleted)
            {
                task = ObjectExtension.RunTask(gettask, TaskCreationOptions.LongRunning).ContinueWith(finish);
                //task = Task.Factory.StartNew(gettask, TaskCreationOptions.LongRunning).ContinueWith(finish);
            }

            async Task gettask()
            {
            //System.Threading.Thread.CurrentThread.Name ??= "公共队列任务线程";
            A:
                while (!queue.IsEmpty && queue.TryDequeue(out WaitAction<T, TResult> waitAction))
                {
                    if (waitAction != null && !waitAction.IsCompleted)
                    {
                        using (waitAction)
                        {
                            try
                            {
                                await waitAction.Run();
                                ContinueWith?.Invoke(waitAction);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                if (ResetEven.Wait(WaitTimeout)) { ResetEven.Reset(); goto A; }
            }

            void finish(Task i)
            {
                i.Dispose();
                Interlocked.Exchange(ref _islock, 0);
            }
        }

        /// <summary>
        /// 添加无返回值任务
        /// </summary>
        /// <param name="action">任务</param>
        /// <param name="state">参数</param>
        /// <returns>获取<see cref="WaitAction{T, TResult}"/>对象</returns>
        public static WaitAction<T, TResult> Add(Action<T> action, T state)
        {
            WaitAction<T, TResult> waitAction = new(action, state);
            queue.Enqueue(waitAction);
            PerformTask();
            return waitAction;
        }

        /// <summary>
        /// 添加有返回值任务
        /// </summary>
        /// <param name="func">任务</param>
        /// <param name="state">参数</param>
        /// <returns>获取<see cref="WaitAction{T, TResult}"/>对象</returns>
        public static WaitAction<T, TResult> Add(Func<T, ValueTask<TResult>> func, T state)
        {
            WaitAction<T, TResult> waitAction = new(func, state);
            queue.Enqueue(waitAction);
            PerformTask();
            return waitAction;
        }

        /// <summary>
        /// 添加队列任务
        /// </summary>
        /// <param name="waitaction">任务对象</param>
        public static void Add(WaitAction<T, TResult> waitaction)
        {
            queue.Enqueue(waitaction);
            PerformTask();
        }
    }
}
