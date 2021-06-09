using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Utils.ThreadQueue
{
    /// <summary>
    /// 一个(公共)消息队列任务模型
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class ActionQueue
    {
        private static void PerformTask()
        {
            lock (ActionQueue._lock)
            {
                if (ActionQueue.task == null)
                {
                    ActionQueue.task = Task.Factory.StartNew(delegate ()
                    {
                        System.Threading.Thread.CurrentThread.Name = "队列任务线程";

                        while (ActionQueue.queue.Count > 0)
                        {
                            WaitAction waitAction = ActionQueue.queue.Dequeue();
                            if (waitAction != null)
                            {
                                if (!waitAction.IsCompleted)
                                {
                                    using (waitAction)
                                    {
                                        waitAction.Run();
                                        ContinueWith?.Invoke(waitAction);
                                    }
                                }
                                
                            }
                        }
                    }, TaskCreationOptions.LongRunning).ContinueWith(delegate (Task i)
                    {
                        ActionQueue.task = null;
                    });
                }
            }
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

        private static readonly object _lock = new();

        private static readonly Queue<WaitAction> queue = new();

        private static Task task = null;

        /// <summary>
        /// 注册完成任务后触发的事件
        /// </summary>
        public static event Action<WaitAction> ContinueWith;
    }
}
