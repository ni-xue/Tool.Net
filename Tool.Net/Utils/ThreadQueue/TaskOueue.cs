using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Utils.ThreadQueue
{

    /// <summary>
    /// 一个消息队列任务模型（异步处理任务）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class TaskOueue<T, V>
    {
        private readonly object _lock = new();

        private readonly ConcurrentQueue<T> queue = new();

        private Task task = null;

        private readonly Func<T, V> func;

        /// <summary>
        /// 注册完成任务后触发的事件
        /// </summary>
        public event Action<T, V, Exception> ContinueWith;

        /// <summary>
        /// 表示当前事件是否已经注册
        /// </summary>
        public bool IsContinueWith => ContinueWith != null;

        /// <summary>
        /// 创建执行需要的函数
        /// </summary>
        public TaskOueue(Func<T, V> func)
        {
            this.func = func;
        }

        /// <summary>
        /// 添加一个新的任务（他会排队一个一个完成）
        /// </summary>
        /// <param name="state">任务需要的参数</param>
        public void Add(T state)
        {
            queue.Enqueue(state);
            PerformTask();
        }

        private void PerformTask()
        {
            lock (_lock)
            {
                if (task == null)
                {
                    task = Task.Factory.StartNew(delegate ()
                    {
                        System.Threading.Thread.CurrentThread.Name ??= "独立队列任务线程";

                        while (!queue.IsEmpty && queue.TryDequeue(out T obj))
                        {
                            try
                            {
                                var val = func(obj);
                                ContinueWith?.Invoke(obj, val, default);
                            }
                            catch (Exception ex)
                            {
                                ContinueWith?.Invoke(obj, default, ex);
                            }
                        }

                        task = null;

                    }, TaskCreationOptions.LongRunning).ContinueWith(delegate (Task i)
                    {
                        i.Dispose();
                    });
                }
            }
        }
    }
}
