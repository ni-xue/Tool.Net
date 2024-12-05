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
    /// 一个Net连接公共的事件消息体
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class EnumEventQueue
    {
        /// <summary>
        /// Net事件核心
        /// </summary>
        private static readonly Lazy<EnumEventQueue> _messageQueue = new(() => new());

        /// <summary>
        /// Net连接公共的事件线程
        /// </summary>
        private readonly Thread _eventthread;

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

        #region 事件拦截器公共枚举

        internal EnClient noEnClient;
        internal EnClient noQueueEnClient;

        internal EnServer noEnServer;
        internal EnServer noQueueEnServer;

        #endregion

        private EnumEventQueue()
        {
            _que = new();
            _mre = new(false);

            noEnClient = new EnClient();
            noEnServer = new EnServer();

            noQueueEnClient = EnClient.Connect | EnClient.Close | EnClient.Fail | EnClient.Reconnect;
            noQueueEnServer = EnServer.Create | EnServer.Close | EnServer.Fail | EnServer.Connect | EnServer.ClientClose;

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
        internal static EnumEventQueue Instance => _messageQueue.Value;

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
        /// <param name="isQueue">是否采用队列处理</param>
        /// <param name="action">委托事件</param>
        public ValueTask<IGetQueOnEnum> Complete<T>(in UserKey key, T enAction, bool isQueue, CompletedEvent<T> action) where T : Enum
        {
            if (action is not null)
            {
                GetQueOnEnum<T> onEnum = new(key, enAction, action);
                if (isQueue)
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

                            _que.Enqueue(onEnum);//Completed?.Invoke(key, enAction)
                            _mre.Set();//启动
                            return ValueTask.FromResult<IGetQueOnEnum>(onEnum);
                        }
                        //}
                    }
                    catch (Exception e)
                    {
                        Log.Error("Net事件通道异常", new Exception("自动容错，已将任务移交回当前线程执行，确保业务流程正常进行", e));
                    }
                }
                return onEnum.WaitAsync();
            }
            return IGetQueOnEnum.SuccessAsync;
        }

        internal static ValueTask<IGetQueOnEnum> OnComplete<T>(in UserKey key, T enAction, bool isQueue, CompletedEvent<T> action) where T : Enum
        {
            return Instance.Complete(in key, enAction, isQueue, action);//Unsafe.NullRef<T>();var a = ref Unsafe.AsRef<EnServer>(ref age1);
        }

        /// <summary>
        /// 设置开启或关闭不想收到的消息事件（当前设置仅在 <see cref="INetworkCore"/> 接口，相关构造对象还未创建之前设置有效，是这些通信的公共默认配置）
        /// </summary>
        /// <param name="enClient"><see cref="EnClient"/></param>
        /// <param name="state">等于true时生效，false将关闭一切的相关事件</param>
        /// <returns>返回true时表示设置成功！</returns>
        public static bool OnInterceptor(EnClient enClient, bool state)
        {
            bool isno = (Instance.noEnClient & enClient) != 0;
            if (state)
            {
                if (!isno) return false;
                Instance.noEnClient &= enClient;
            }
            else
            {
                if (isno) return false;
                Instance.noEnClient |= enClient;
            }
            return true;
        }

        /// <summary>
        /// 设置开启或关闭不想收到的消息事件（当前设置仅在 <see cref="INetworkCore"/> 接口，相关构造对象还未创建之前设置有效，是这些通信的公共默认配置）
        /// </summary>
        /// <param name="enServer"><see cref="EnServer"/></param>
        /// <param name="state">等于true时生效，false将关闭一切的相关事件</param>
        /// <returns>返回true时表示设置成功！</returns>
        public static bool OnInterceptor(EnServer enServer, bool state)
        {
            bool isno = (Instance.noEnServer & enServer) != 0;
            if (state)
            {
                if (!isno) return false;
                Instance.noEnServer &= enServer;
            }
            else
            {
                if (isno) return false;
                Instance.noEnServer |= enServer;
            }
            return true;
        }

        /// <summary>
        /// 设置将<see cref="EnClient"/>事件，载入或不载入，队列池（当前设置仅在 <see cref="INetworkCore"/> 接口，相关构造对象还未创建之前设置有效，是这些通信的公共默认配置）
        /// </summary>
        /// <param name="enClient"><see cref="EnClient"/></param>
        /// <param name="state">等于true时，事件由队列线程完成，false时交由任务线程自行完成</param>
        /// <returns>返回true时表示设置成功！</returns>
        public static bool OnIsQueue(EnClient enClient, bool state)
        {
            bool isno = (Instance.noQueueEnClient & enClient) != 0;
            if (state)
            {
                if (!isno) return false;
                Instance.noQueueEnClient &= enClient;
            }
            else
            {
                if (isno) return false;
                Instance.noQueueEnClient |= enClient;
            }
            return true;
        }

        /// <summary>
        /// 设置将<see cref="EnServer"/>事件，载入或不载入，队列池（当前设置仅在 <see cref="INetworkCore"/> 接口，相关构造对象还未创建之前设置有效，是这些通信的公共默认配置）
        /// </summary>
        /// <param name="enServer"><see cref="EnServer"/></param>
        /// <param name="state">等于true时，事件由队列线程完成，false时交由任务线程自行完成</param>
        /// <returns>返回true时表示设置成功！</returns>
        public static bool OnIsQueue(EnServer enServer, bool state)
        {
            bool isno = (Instance.noQueueEnServer & enServer) != 0;
            if (state)
            {
                if (!isno) return false;
                Instance.noQueueEnServer &= enServer;
            }
            else
            {
                if (isno) return false;
                Instance.noQueueEnServer |= enServer;
            }
            return true;
        }
    }
}
