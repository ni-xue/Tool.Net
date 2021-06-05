using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Tool.Utils;

namespace Tool.Sockets.SupportCode
{
    /// <summary>
    /// 一个TCP连接公共的事件消息体
    /// </summary>
    public class TcpEventQueue
    {
        /// <summary>
        /// 日志
        /// </summary>
        private static TcpEventQueue _messageQueue;

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
        private readonly ConcurrentQueue<GetQueOnEnum> _que;

        /// <summary>
        /// 当前锁
        /// </summary>
        private static readonly object _lockobj = new object();

        private TcpEventQueue()
        {
            _que = new ConcurrentQueue<GetQueOnEnum>();
            _mre = new ManualResetEvent(false);
            _IsEnumOns = new ConcurrentDictionary<Enum, bool>();

            _logthread = new Thread(new ThreadStart(TaskOnComplete))
            {
                Name = "Tcp事件线程",
                IsBackground = true,
                Priority = ThreadPriority.Lowest //false https://blog.csdn.net/snakorse/article/details/43888847
            };
        }

        /// <summary>
        /// 实现单例,不建议直接调用。
        /// </summary>
        private static TcpEventQueue Instance
        {
            get
            {
                if (_messageQueue == null)
                {
                    lock (_lockobj)
                    {
                        if (_messageQueue == null)
                        {
                            _messageQueue = new TcpEventQueue();
                        }
                    }
                }
                return _messageQueue;
            }
        }

        private void TaskOnComplete()
        {
            while (true)
            {
                // 等待信号通知
                _mre.WaitOne();

                // 判断是否有内容需要执行的事件 从列队中获取内容，并删除列队中的内容
                while (!_que.IsEmpty && _que.TryDequeue(out GetQueOnEnum getQueOn))//_que.Count > 0
                {
                    getQueOn.Completed();
                }

                // 重新设置信号
                _mre.Reset();
                Thread.Sleep(1);
            }
        }

        /// <summary>
        /// 调用TCP事件队列线程
        /// </summary>
        /// <param name="key">IP</param>
        /// <param name="enAction">事件枚举</param>
        /// <param name="action">委托事件</param>
        public void Complete(string key, Enum enAction, object action)
        {
            try
            {
                lock (_lockobj)
                {
                    if (!_mre.SafeWaitHandle.IsClosed)
                    {
                        if (!_logthread.IsAlive)
                        {
                            _logthread.Start();
                        }
                        if (!_IsEnumOns.IsEmpty && _IsEnumOns.TryGetValue(enAction, out bool value) && value) return; //_IsEnumOns.Count > 0 

                        _que.Enqueue(new GetQueOnEnum(key, enAction, action));//Completed?.Invoke(key, enAction)
                        _mre.Set();//启动
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error("TCP事件通道异常", e);
            }
        }

        internal static void OnComplete(string key, Enum enAction, object action)
        {
            Instance.Complete(key, enAction, action);
        }

        /// <summary>
        /// 设置开启或关闭不想收到的消息事件
        /// </summary>
        /// <param name="enAction">屏蔽不触发的事件状态（<see cref="EnClient"/>|<see cref="EnServer"/>）</param>
        /// <param name="state">等于true时生效，将关闭一切的相关事件</param>
        /// <returns>返回是否设置成功</returns>
        public static bool OnInterceptor(Enum enAction, bool state) 
        {
            if (typeof(EnClient) == enAction.GetType() || typeof(EnServer) == enAction.GetType())
            {
                if (!Instance._IsEnumOns.TryAdd(enAction, state))
                {
                    Instance._IsEnumOns[enAction] = state;
                }
                return true;
            }
            else
            {
                throw new Exception("并不是认可的枚举！");
            }
        }
    }
}
