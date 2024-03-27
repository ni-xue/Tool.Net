using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 用于长连接的心跳模型
    /// </summary>
    public class KeepAlive
    {
        /// <summary>
        /// 获取持久连接协议
        /// </summary>
        /// <returns></returns>
        public static byte[] KeepAliveObj => new byte[] { 123, 1, 2, 3, 2, 1, 123 };

        /// <summary>
        /// 创建心跳对象
        /// </summary>
        /// <param name="TimeInterval">心跳频率</param>
        /// <param name="OnStart">心跳触发器内部捕获了异常</param>
        public KeepAlive(byte TimeInterval, Func<Task> OnStart)
        {
            if (TimeInterval == 0)
            {
                throw new ArgumentException("TimeInterval 值必须>0！", nameof(TimeInterval));
            }

            StopTime = new System.Diagnostics.Stopwatch();
            this.TimeInterval = TimeInterval * 1000;
            this.OnStart = OnStart ?? throw new ArgumentNullException(nameof(OnStart), "OnStart 不能为空！");
            HeartBeatStart();//Timer
        }

        /// <summary>
        /// 开始心跳事件
        /// </summary>
        private Func<Task> OnStart { get; }

        /// <summary>
        /// 间隔时间，不能小于1秒
        /// </summary>
        public int TimeInterval { get; }

        /// <summary>
        /// 用于记录心跳间隔
        /// </summary>
        private readonly System.Diagnostics.Stopwatch StopTime;

        private readonly CancellationTokenRegistration cancellationTokenRegistration;

        private bool OnClose = false;

        private async void HeartBeatStart()
        {
            //Task.Factory.StartNew(() =>
            //{
            //Thread.CurrentThread.Name ??= "心跳线程";
            while (!OnClose)
            {
                ResetTime();
                //System.Threading.Thread.Sleep(TimeInterval);
                await Task.Delay(TimeInterval, CancellationToken.None); //CancellationTokenSource

                if (StopTime.ElapsedMilliseconds >= TimeInterval)
                {
                    try
                    {
                        await OnStart();
                    }
                    catch (Exception)
                    {

                    }
                }
            }
            //}, TaskCreationOptions.LongRunning);

            //Task.Run();
        }

        /// <summary>
        /// 重置计数器
        /// </summary>
        public void ResetTime()
        {
            cancellationTokenRegistration.Unregister();
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
