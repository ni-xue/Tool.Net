using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Utils.ThreadQueue
{
    /// <summary>
    ///  一个消息队列任务模型（异步处理任务·线程安全）
    /// </summary>
    /// <typeparam name="T">传入对象</typeparam>
    public class TaskOueue<T>
    {
        private readonly TaskOueue<T, object> taskOueue;
        private readonly Func<T, ValueTask> func;

        /// <summary>
        /// 注册完成任务后触发的事件
        /// </summary>
        public event Action<T, Exception> ContinueWith;

        /// <summary>
        /// 表示当前事件是否已经注册
        /// </summary>
        public bool IsContinueWith => ContinueWith != null;

        /// <summary>
        /// 创建执行需要的函数
        /// </summary>
        /// <param name="func">队列处理的函数</param>
        public TaskOueue(Func<T, ValueTask> func) : this(func, TaskCreationOptions.LongRunning)
        {
        }

        /// <summary>
        /// 创建执行需要的函数
        /// </summary>
        /// <param name="func">队列处理的函数</param>
        /// <param name="creationOptions">异步线程属性</param>
        public TaskOueue(Func<T, ValueTask> func, TaskCreationOptions creationOptions)
        {
            this.func = func;
            taskOueue = new TaskOueue<T, object>(OnFunc, creationOptions);
            taskOueue.ContinueWith += OnContinueWith;
        }

        private async ValueTask<object> OnFunc(T state) 
        {
            await func.Invoke(state);
            return default;
        }

        private void OnContinueWith(T state, object obj, Exception ex) => ContinueWith?.Invoke(state, ex);

        /// <summary>
        /// 添加一个新的任务（他会排队一个一个完成）
        /// </summary>
        /// <param name="state">任务需要的参数</param>
        public void Add(T state) => taskOueue.Add(state);
    }


    /// <summary>
    /// 一个消息队列任务模型（异步处理任务·线程安全）
    /// </summary>
    /// <typeparam name="T">传入对象</typeparam>
    /// <typeparam name="TResult">返回结果</typeparam>
    /// <remarks>代码由逆血提供支持</remarks>
    public class TaskOueue<T, TResult>
    {
        /// <summary>
        /// 分配的任务队列线程完成后最大保留时间
        /// </summary>
        public const int WaitTimeout = 60 * 1000;

        private readonly TaskCreationOptions _creationOptions;
        private readonly ConcurrentQueue<T> queue = new();
        private readonly Func<T, ValueTask<TResult>> func;
        private readonly ManualResetEventSlim ResetEven;

        private Task task = null;

        private volatile int _islock = 0;

        /// <summary>
        /// 注册完成任务后触发的事件
        /// </summary>
        public event Action<T, TResult, Exception> ContinueWith;

        /// <summary>
        /// 表示当前事件是否已经注册
        /// </summary>
        public bool IsContinueWith => ContinueWith != null;

        /// <summary>
        /// 创建执行需要的函数
        /// </summary>
        /// <param name="func">队列处理的函数</param>
        public TaskOueue(Func<T, ValueTask<TResult>> func) : this(func, TaskCreationOptions.LongRunning)
        {
        }

        /// <summary>
        /// 创建执行需要的函数
        /// </summary>
        /// <param name="func">队列处理的函数</param>
        /// <param name="creationOptions">异步线程属性</param>
        public TaskOueue(Func<T, ValueTask<TResult>> func, TaskCreationOptions creationOptions)
        {
            this.func = func;
            _creationOptions = creationOptions;
            ResetEven = new ManualResetEventSlim(false, 100);
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
            if (Interlocked.CompareExchange(ref _islock, 1, 0) == 1) { ResetEven.Set(); return; }

            Debug.WriteLine("新任务：{0}，{1}数", ObjectExtension.Thread.ManagedThreadId, queue.Count);

            if (task == null || task.IsCompleted)
            {
                //Debug.WriteLine("1:{0}", Interlocked.Increment(ref a));
                task = ObjectExtension.RunTask(gettask, _creationOptions).ContinueWith(finish);
                //task = Task.Factory.StartNew(gettask, _creationOptions); 
                //Debug.WriteLine("2:{0}", Interlocked.Increment(ref b));
            }

            async Task gettask()
            {
                //System.Threading.Thread.CurrentThread.Name ??= "独立队列任务线程";
            A:
                while (!queue.IsEmpty && queue.TryDequeue(out T obj))
                {
                    Exception ex;
                    TResult val;
                    try
                    {
                        val = await func(obj);
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
                }

                if (ResetEven.Wait(WaitTimeout)) { ResetEven.Reset(); goto A; }
            }

            void finish(Task i)
            {
                i.Dispose();
                Interlocked.Exchange(ref _islock, 0);
            }
        }
    }
}
