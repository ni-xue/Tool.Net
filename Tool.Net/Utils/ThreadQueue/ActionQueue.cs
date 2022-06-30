using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Utils.ThreadQueue
{
    /// <summary>
    /// 一个(公共)消息队列任务模型
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class ActionQueue
    {
        //private static readonly object _lock = new();

        private static volatile int _islock;

        private static readonly ConcurrentQueue<WaitAction> queue;

        private static Task task = null;

        /// <summary>
        /// 注册完成任务后触发的事件
        /// </summary>
        public static event Action<WaitAction> ContinueWith;

        /// <summary>
        /// 表示当前事件是否已经注册
        /// </summary>
        public static bool IsContinueWith => ContinueWith != null;

        static ActionQueue()
        {
            _islock = new();
            queue = new();
        }

        private static void PerformTask()
        {
            if (Interlocked.Exchange(ref _islock, 1) == 1) return;
            //lock (ActionQueue._lock)
            //{
            if (task == null)
            {
                task = Task.Factory.StartNew(gettask, TaskCreationOptions.LongRunning).ContinueWith(finish);
            }
            else
            {
                if (task.Status is TaskStatus.RanToCompletion or TaskStatus.Faulted)
                {
                    task = null;
                    Interlocked.Exchange(ref _islock, 0);
                }
            }

            void gettask()
            {
                //System.Threading.Thread.CurrentThread.Name ??= "公共队列任务线程";
                byte retry = 0;
                A:
                while (!queue.IsEmpty && queue.TryDequeue(out WaitAction waitAction))
                {
                    if (waitAction != null && !waitAction.IsCompleted)
                    {
                        using (waitAction)
                        {
                            try
                            {
                                waitAction.Run();
                                ContinueWith?.Invoke(waitAction);
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

                while (retry < 200)
                {
                    Thread.Sleep(100);
                    retry++;
                    goto A;
                }
            }

            void finish(Task i)
            {
                i.Dispose();
                task = null;
                Interlocked.Exchange(ref _islock, 0);
            }
            //}
        }

        /// <summary>
        /// 添加无返回值任务
        /// </summary>
        /// <param name="action">任务</param>
        /// <param name="state">参数</param>
        /// <returns>获取<see cref="WaitAction"/>对象</returns>
        public static WaitAction Add(Action<object> action, object state)
        {
            WaitAction waitAction = new(action, state);
            ActionQueue.queue.Enqueue(waitAction);
            ActionQueue.PerformTask();
            return waitAction;
        }

        /// <summary>
        /// 添加有返回值任务
        /// </summary>
        /// <param name="func">任务</param>
        /// <param name="state">参数</param>
        /// <returns>获取<see cref="WaitAction"/>对象</returns>
        public static WaitAction Add(Func<object, object> func, object state)
        {
            WaitAction waitAction = new(func, state);
            ActionQueue.queue.Enqueue(waitAction);
            ActionQueue.PerformTask();
            return waitAction;
        }

        /// <summary>
        /// 添加队列任务
        /// </summary>
        /// <param name="waitaction">任务对象</param>
        public static void Add(WaitAction waitaction)
        {
            ActionQueue.queue.Enqueue(waitaction);
            ActionQueue.PerformTask();
        }
    }
}
