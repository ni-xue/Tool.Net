using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tool.Sockets.SupportCode
{
    /// <summary>
    /// 持久连接对象
    /// </summary>
    public class KeepAlive
    {
        /// <summary>
        /// 创建心跳对象
        /// </summary>
        /// <param name="TimeInterval">心跳频率</param>
        /// <param name="OnStart">心跳触发器</param>
        public KeepAlive(byte TimeInterval, Action OnStart) 
        {
            if (TimeInterval == 0)
            {
                throw new ArgumentException("TimeInterval 值必须>0！", nameof(TimeInterval));
            }

            StopTime = new System.Diagnostics.Stopwatch();
            this.TimeInterval = TimeInterval * 1000;
            this.OnStart = OnStart ?? throw new ArgumentNullException(nameof(OnStart), "OnStart 不能为空！");
            HeartBeatStart();
        }

        /// <summary>
        /// 开始心跳事件
        /// </summary>
        internal Action OnStart { get; }

        /// <summary>
        /// 间隔时间，不能小于1秒
        /// </summary>
        public int TimeInterval { get; }

        /// <summary>
        /// 用于记录心跳间隔
        /// </summary>
        private readonly System.Diagnostics.Stopwatch StopTime;

        private bool OnClose = false;

        private void HeartBeatStart() 
        {
            Task.Run(() =>
            {
                while (!OnClose)
                {
                    ResetTime();
                    System.Threading.Thread.Sleep(TimeInterval);

                    if (StopTime.ElapsedMilliseconds >= TimeInterval)
                    {
                        OnStart();
                    }
                }
            });
        }

        /// <summary>
        /// 重置计数器
        /// </summary>
        public void ResetTime() 
        {
            StopTime.Restart();
        }

        /// <summary>
        /// 关闭心跳系统
        /// </summary>
        public void Close() 
        {
            OnClose = true;
            ResetTime();
        }
    }
}
