using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Utils.ThreadQueue
{

    /// <summary>
    /// 一个消息队列任务模型（异步处理任务）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class TaskOueue<T, V>
    {
        //private readonly object _lock = new();

        private readonly TaskCreationOptions _creationOptions;

        private volatile int _islock = new();

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
        /// <param name="func">队列处理的函数</param>
        public TaskOueue(Func<T, V> func) : this(func, TaskCreationOptions.LongRunning)
        {
        }

        /// <summary>
        /// 创建执行需要的函数
        /// </summary>
        /// <param name="func">队列处理的函数</param>
        /// <param name="creationOptions">异步线程属性</param>
        public TaskOueue(Func<T, V> func, TaskCreationOptions creationOptions)
        {
            this.func = func;
            _creationOptions = creationOptions;
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

        //int a, b;
        private void PerformTask()
        {
            if (Interlocked.Exchange(ref _islock, 1) == 1) return;
            //lock (_lock)
            //{

            if (task == null)
            {
                //Debug.WriteLine("1:{0}", Interlocked.Increment(ref a));
                task = Task.Factory.StartNew(gettask, _creationOptions).ContinueWith(finish);
                //await task;
                //task.Dispose();
                //task = null;
                //Interlocked.Exchange(ref _islock, 0);

                //Debug.WriteLine("2:{0}", Interlocked.Increment(ref b));
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
                //System.Threading.Thread.CurrentThread.Name ??= "独立队列任务线程";
                byte retry = 0;
                A:
                while (!queue.IsEmpty && queue.TryDequeue(out T obj))
                {
                    Exception ex;
                    V val;
                    try
                    {
                        val = func(obj);
                        ex = default;
                    }
                    catch (Exception _e)
                    {
                        val = default;
                        ex = _e;
                    }

                    try
                    {
                        ContinueWith?.Invoke(obj, val, ex);
                    }
                    catch (Exception)
                    {
                    }
                    retry = 0;
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

                //Debug.WriteLine("2:{0}", Interlocked.Increment(ref b));
            }

            //}
        }
    }
}
