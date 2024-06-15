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
        /// <summary>
        /// 日志
        /// </summary>
        private static readonly EnumEventQueue _messageQueue;

        /// <summary>
        /// TCP连接公共的事件线程
        /// </summary>
        private readonly Thread _logthread;

        /// <summary>
        /// 事件拦截器
        /// </summary>
        private readonly ConcurrentDictionary<Enum, bool> _IsEnumOns;

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

        static EnumEventQueue()
        {
            _messageQueue = new EnumEventQueue();
        }

        private EnumEventQueue()
        {
            _que = new ConcurrentQueue<IGetQueOnEnum>();
            _mre = new ManualResetEvent(false);
            _IsEnumOns = new ConcurrentDictionary<Enum, bool>();

            _logthread = new Thread(TaskOnComplete)
            {
                Name = "Net事件线程",
                IsBackground = true,
                Priority = ThreadPriority.Lowest //false https://blog.csdn.net/snakorse/article/details/43888847
            };

            _logthread.Start();
        }

        /// <summary>
        /// 实现单例,不建议直接调用。
        /// </summary>
        private static EnumEventQueue Instance => _messageQueue;

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
        public IGetQueOnEnum Complete<T>(in UserKey key, T enAction, CompletedEvent<T> action) where T : Enum
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
                    if (action is not null && IsEnumOn(enAction))
                    {
                        IGetQueOnEnum onEnum = new GetQueOnEnum<T>(key, enAction, action);
                        _que.Enqueue(onEnum);//Completed?.Invoke(key, enAction)
                        _mre.Set();//启动
                        return onEnum;
                    }
                }
                //}
            }
            catch (Exception e)
            {
                Log.Error("TCP事件通道异常", e);
            }
            return IGetQueOnEnum.Success;
        }

        private bool IsEnumOn(Enum enAction)
        {
            if (_IsEnumOns.IsEmpty) return true;
            if (_IsEnumOns.TryGetValue(enAction, out bool value)) return value;
            return true;
        }

        internal static IGetQueOnEnum OnComplete<T>(in UserKey key, T enAction, CompletedEvent<T> action) where T : Enum
        {
            return Instance.Complete(in key, enAction, action);//Unsafe.NullRef<T>();var a = ref Unsafe.AsRef<EnServer>(ref age1);
        }

        /// <summary>
        /// 设置开启或关闭不想收到的消息事件
        /// </summary>
        /// <param name="enAction">屏蔽不触发的事件状态（<see cref="EnClient"/> | <see cref="EnServer"/>）</param>
        /// <param name="state">等于true时生效，将关闭一切的相关事件</param>
        /// <returns>返回是否设置成功</returns>
        public static bool OnInterceptor(Enum enAction, bool state)
        {
            if (enAction is EnClient or EnServer) //(typeof(EnClient) == enAction.GetType() || typeof(EnServer) == enAction.GetType())
            {
                //if (!Instance._IsEnumOns.TryAdd(enAction, state))
                //{
                //    Instance._IsEnumOns[enAction] = state;
                //}

                _ = Instance._IsEnumOns.AddOrUpdate(enAction, state, (_, _) => state);
                return true;
            }
            else
            {
                throw new Exception("并不是认可的枚举！");
            }
        }
    }
}
