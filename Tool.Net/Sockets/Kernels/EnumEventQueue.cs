using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Utils;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 一个TCP连接公共的事件消息体
    /// </summary>
    public class EnumEventQueue
    {
        private readonly struct EventState
        {
            public EventState(bool IsEvent, bool IsQueue)
            {
                this.IsEvent = IsEvent;
                this.IsQueue = IsQueue;
            }

            public bool IsEvent { get; }

            public bool IsQueue { get; }
        }

        /// <summary>
        /// Net事件核心
        /// </summary>
        private static readonly Lazy<EnumEventQueue> _messageQueue = new(() => new EnumEventQueue());

        /// <summary>
        /// TCP连接公共的事件线程
        /// </summary>
        private readonly Thread _eventthread;

        /// <summary>
        /// 事件拦截器
        /// </summary>
        private readonly ConcurrentDictionary<Enum, EventState> _IsEnumOns;

        /**
         * 信号
         */
        private readonly ManualResetEvent _mre;

        /**
         * 事件消息Queue
         */
        private readonly ConcurrentQueue<IGetQueOnEnum> _que;

        /**
         * 当前锁
         */
        //private static readonly object _lockobj = new();

        private EnumEventQueue()
        {
            _que = new ConcurrentQueue<IGetQueOnEnum>();
            _mre = new ManualResetEvent(false);
            _IsEnumOns = new ConcurrentDictionary<Enum, EventState>();
            _IsEnumOns.TryAdd(EnClient.Connect, new EventState(true, false));
            _IsEnumOns.TryAdd(EnClient.Close, new EventState(true, false));
            _IsEnumOns.TryAdd(EnClient.Fail, new EventState(true, false));

            _IsEnumOns.TryAdd(EnServer.Connect, new EventState(true, false));
            _IsEnumOns.TryAdd(EnServer.ClientClose, new EventState(true, false));

            _IsEnumOns.TryAdd(EnServer.Create, new EventState(true, false));
            _IsEnumOns.TryAdd(EnServer.Close, new EventState(true, false));
            _IsEnumOns.TryAdd(EnServer.Fail, new EventState(true, false));

            _eventthread = new Thread(TaskOnComplete)
            {
                Name = "Net事件线程",
                IsBackground = true,
                Priority = ThreadPriority.Lowest //false https://blog.csdn.net/snakorse/article/details/43888847
            };

#if NET6_0_OR_GREATER
            _eventthread.UnsafeStart();
#else
            _eventthread.Start();
#endif
        }

        /// <summary>
        /// 实现单例,不建议直接调用。
        /// </summary>
        private static EnumEventQueue Instance => _messageQueue.Value;

        private async void TaskOnComplete()
        {
            try
            {
                while (true)
                {
                    // 等待信号通知
                    _mre.WaitOne();

                    // 判断是否有内容需要执行的事件 从列队中获取内容，并删除列队中的内容
                    while (!_que.IsEmpty && _que.TryDequeue(out IGetQueOnEnum getQueOn))//_que.Count > 0
                    {
                        await getQueOn.Completed();
                    }

                    // 重新设置信号
                    _mre.Reset();
                    //Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                Log.Fatal("连接事件线程崩溃：", ex, "Log/Tool");
            }
        }

        /// <summary>
        /// 调用TCP事件队列线程
        /// </summary>
        /// <param name="key">IP</param>
        /// <param name="enAction">事件枚举</param>
        /// <param name="action">委托事件</param>
        public ValueTask<IGetQueOnEnum> Complete<T>(in UserKey key, T enAction, CompletedEvent<T> action) where T : Enum
        {
            try
            {
                //lock (_lockobj)
                //{
                if (!_mre.SafeWaitHandle.IsClosed)
                {
                    //if (!_logthread.IsAlive)
                    //{
                    //    _logthread.Start();
                    //}
                    if (action is not null && IsEnumOn(enAction, out bool IsQueue))
                    {
                        GetQueOnEnum<T> onEnum = new(key, enAction, action);
                        if (IsQueue)
                        {
                            _que.Enqueue(onEnum);//Completed?.Invoke(key, enAction)
                            _mre.Set();//启动
                            return ValueTask.FromResult(onEnum as IGetQueOnEnum);
                        }
                        else
                        {
                            return onEnum.WaitAsync();
                        }
                    }
                }
                //}
            }
            catch (Exception e)
            {
                Log.Error("Net事件通道异常", e);
            }
            return ValueTask.FromResult(IGetQueOnEnum.Success);
        }

        private bool IsEnumOn(Enum enAction, out bool IsQueue)
        {
            if (_IsEnumOns.IsEmpty is false && _IsEnumOns.TryGetValue(enAction, out EventState value)) { IsQueue = value.IsQueue; return value.IsEvent; }
            IsQueue = true;
            return IsQueue;
        }

        internal static ValueTask<IGetQueOnEnum> OnComplete<T>(in UserKey key, T enAction, CompletedEvent<T> action) where T : Enum
        {
            return Instance.Complete(in key, enAction, action);//Unsafe.NullRef<T>();var a = ref Unsafe.AsRef<EnServer>(ref age1);
        }

        /// <summary>
        /// 设置开启或关闭不想收到的消息事件
        /// </summary>
        /// <param name="enAction">屏蔽不触发的事件状态（<see cref="EnClient"/> | <see cref="EnServer"/>）</param>
        /// <param name="state">等于true时生效，将关闭一切的相关事件</param>
        /// <returns>返回是否设置成功</returns>
        /// /// <exception cref="Exception"></exception>
        public static bool OnInterceptor(Enum enAction, bool state)
        {
            if (enAction is EnClient or EnServer) //(typeof(EnClient) == enAction.GetType() || typeof(EnServer) == enAction.GetType())
            {
                _ = Instance._IsEnumOns.AddOrUpdate(enAction, enAction =>
                {
                    return new EventState(state, true);
                },
                (enAction, eventState) =>
                {
                    return new EventState(state, eventState.IsQueue);
                });
                return true;
            }
            else
            {
                throw new Exception("并不是认可的枚举！");
            }
        }

        /// <summary>
        /// 设置将，特定事件，载入或不载入，队列池
        /// <list type="table">注意系统默认，提供了合理方案，如果需要更改，请仔细理解设计模式后，改动。</list>
        /// </summary>
        /// <param name="enAction">事件状态（<see cref="EnClient"/> | <see cref="EnServer"/>）</param>
        /// <param name="state">true时载入队列，false时由当前线程处理</param>
        /// <returns>返回是否设置成功</returns>
        /// <exception cref="Exception"></exception>
        public static bool OnIsQueue(Enum enAction, bool state)
        {
            if (enAction is EnClient or EnServer) //(typeof(EnClient) == enAction.GetType() || typeof(EnServer) == enAction.GetType())
            {
                _ = Instance._IsEnumOns.AddOrUpdate(enAction, enAction =>
                {
                    return new EventState(true, state);
                },
                (enAction, eventState) =>
                {
                    return new EventState(eventState.IsEvent, state);
                });
                return true;
            }
            else
            {
                throw new Exception("并不是认可的枚举！");
            }
        }
    }
}
