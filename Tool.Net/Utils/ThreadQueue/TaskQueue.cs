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
    /// 一个消息队列任务模型（异步处理任务·线程安全）
    /// <list type="table">警告：在队列执行任务中，注册任务是很傻逼的行为，这回导致无限期死锁，是死锁，死锁。</list>
    /// <list type="table">但是并不代表不能注册，但是如业务需要注册，请不要直接 等待任务 结果 await 等行为，这才是导致死锁发生的祸因。</list>
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class TaskQueue
    {
        private class TaskModel : IDisposable
        {
            private readonly ManualResetEventSlim ResetEven;
            private readonly ActionDelegate.ActionDispatcher<object, object> actionDispatcher;

            protected Task taskResult;

            public object CallClass { get; }

            public object[] Args { get; }

            public TaskModel(object callClass, Delegate @delegate, object[] args)
            {
                var dispatcher = GetOrAdd(@delegate);
                if (!dispatcher.IsTask)
                {
                    throw new Exception("加入的任务非异步任务，不包含 Task/Task<?> 或 ValueTask/ValueTask<?> 返回值！");
                }
                actionDispatcher = dispatcher as ActionDelegate.ActionDispatcher<object, object>;
                if (actionDispatcher is null)
                {
                    throw new Exception("无法识别当前任务！");
                }
                CallClass = callClass;
                if (args.GetType() == typeof(object[]))
                {
                    Args = args;
                }
                else
                {
                    Args = new[] { args };
                }
                ResetEven = new ManualResetEventSlim(false, 100);
            }

            public async Task<Task> Invoke()
            {
                Task task;
                //ActionDelegate.ActionDispatcher<object, Task> actionDispatcher = new(Delegate.Method);
                if (actionDispatcher.IsVoid)
                {
                    task = actionDispatcher.VoidExecuteAsync(CallClass, Args);
                }
                else
                {
                    task = actionDispatcher.ExecuteAsync(CallClass, Args);
                }
                await task;
                return task;
            }

            public virtual async Task WaitAsync()
            {
                await Task.Yield();
                ResetEven.Wait();
                await taskResult;
            }

            public void OkResult(Task taskResult, Exception ex)
            {
                this.taskResult = ex is null ? taskResult : Task.FromException(ex);
                ResetEven.Set();
            }

            void IDisposable.Dispose()
            {
                ResetEven.Dispose();
                GC.SuppressFinalize(this);
            }
        }

        private class TaskModel<T> : TaskModel
        {
            public TaskModel(object callClass, Delegate @delegate, object[] args) : base(callClass, @delegate, args)
            {
            }

            public override async Task<T> WaitAsync()
            {
                await base.WaitAsync();
                if (taskResult is Task<object> result && result.Result is T obj)
                {
                    return obj;
                }
                return default;
            }
        }


        private static readonly ConcurrentDictionary<Delegate, ActionDelegate.IActionDispatcher> otherEvents;
        private static readonly TaskQueue<TaskModel, Task> taskQueue;

        /// <summary>
        /// 当前活动的任务数
        /// </summary>
        public static int Count => taskQueue.Count;

        /// <summary>
        /// 累计已有的任务数
        /// </summary>
        public static ulong TotalCount => taskQueue.TotalCount;

        /// <summary>
        /// 累计已完成的任务数
        /// </summary>
        public static ulong CompleteCount => taskQueue.CompleteCount;

        static TaskQueue()
        {
            otherEvents = new();
            taskQueue = new(OnFunc);
            taskQueue.ContinueWith += TaskQueue_ContinueWith;
        }

        private static void TaskQueue_ContinueWith(TaskModel arg1, Task arg2, Exception arg3)
        {
            arg1.OkResult(arg2, arg3);
        }

        private static async ValueTask<Task> OnFunc(TaskModel state)
        {
            return await state.Invoke();
        }

        /// <summary>
        /// 添加一个新的任务（他会排队一个一个完成）
        /// </summary>
        /// <param name="callClass">调用类信息</param>
        /// <param name="action">任务</param>
        /// <param name="args">任务需要的参数</param>
        /// <remarks>可等待的结果</remarks>
        public static async Task Enqueue(object callClass, Delegate action, params object[] args)
        {
            using var task = new TaskModel(callClass, action, args);
            taskQueue.Add(task);
            await task.WaitAsync();
        }

        /// <summary>
        /// 添加一个新的任务（他会排队一个一个完成）
        /// </summary>
        /// <param name="callClass">调用类信息</param>
        /// <param name="func">任务</param>
        /// <param name="args">任务需要的参数</param>
        /// <remarks>可等待的结果</remarks>
        public static async Task<T> Enqueue<T>(object callClass, Delegate func, params object[] args)
        {
            using var task = new TaskModel<T>(callClass, func, args);
            taskQueue.Add(task);
            return await task.WaitAsync();
        }

        /// <summary>
        /// 添加一个新的任务（他会排队一个一个完成）
        /// </summary>
        /// <param name="action">任务</param>
        /// <param name="args">任务需要的参数</param>
        /// <remarks>可等待的结果</remarks>
        public static async Task StaticEnqueue(Delegate action, params object[] args)
        {
            await Enqueue(null, action, args);
        }

        /// <summary>
        /// 添加一个新的任务（他会排队一个一个完成）
        /// </summary>
        /// <param name="func">任务</param>
        /// <param name="args">任务需要的参数</param>
        /// <remarks>可等待的结果</remarks>
        public static async Task<T> StaticEnqueue<T>(Delegate func, params object[] args)
        {
            return await Enqueue<T>(null, func, args);
        }

        /// <summary>
        /// 获取一个可以动态传递的委托（可复用保证线程安全）
        /// </summary>
        /// <param name="func">任务需要的参数</param>
        /// <remarks><see cref="ActionDelegate.IActionDispatcher"/></remarks>
        public static ActionDelegate.IActionDispatcher GetOrAdd(Delegate func)
        {
            return otherEvents.GetOrAdd(func, (action) => { return new ActionDelegate.ActionDispatcher(func.Method); });
        }
    }

    /// <summary>
    ///  一个消息队列任务模型（异步处理任务·线程安全）
    /// </summary>
    /// <typeparam name="T">传入对象</typeparam>
    /// <remarks>代码由逆血提供支持</remarks>
    public class TaskQueue<T>
    {
        private readonly TaskQueue<T, object> taskOueue;
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
        public TaskQueue(Func<T, ValueTask> func) : this(func, TaskCreationOptions.LongRunning)
        {
        }

        /// <summary>
        /// 创建执行需要的函数
        /// </summary>
        /// <param name="func">队列处理的函数</param>
        /// <param name="creationOptions">异步线程属性</param>
        public TaskQueue(Func<T, ValueTask> func, TaskCreationOptions creationOptions)
        {
            this.func = func;
            taskOueue = new TaskQueue<T, object>(OnFunc, creationOptions);
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
    public class TaskQueue<T, TResult>
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
        private ulong totalCount = 0;

        /// <summary>
        /// 注册完成任务后触发的事件
        /// </summary>
        public event Action<T, TResult, Exception> ContinueWith;

        /// <summary>
        /// 累计已完成的任务数
        /// </summary>
        public ulong CompleteCount => totalCount - (ulong)queue.Count;

        /// <summary>
        /// 当前活动的任务数
        /// </summary>
        public int Count => queue.Count;

        /// <summary>
        /// 累计已有的任务数
        /// </summary>
        public ulong TotalCount => totalCount;

        /// <summary>
        /// 表示当前事件是否已经注册
        /// </summary>
        public bool IsContinueWith => ContinueWith != null;

        /// <summary>
        /// 创建执行需要的函数
        /// </summary>
        /// <param name="func">队列处理的函数</param>
        public TaskQueue(Func<T, ValueTask<TResult>> func) : this(func, TaskCreationOptions.LongRunning)
        {
        }

        /// <summary>
        /// 创建执行需要的函数
        /// </summary>
        /// <param name="func">队列处理的函数</param>
        /// <param name="creationOptions">异步线程属性</param>
        public TaskQueue(Func<T, ValueTask<TResult>> func, TaskCreationOptions creationOptions)
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
            totalCount.Increment();
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
