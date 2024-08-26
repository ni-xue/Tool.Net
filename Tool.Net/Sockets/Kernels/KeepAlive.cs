using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// 获取完整心跳协议（Tcp）
        /// </summary>
        public static Memory<byte> TcpKeepObj { get; } = new byte[] { 40, 7, 0, 0, 0, 41, 123, 1, 2, 3, 2, 1, 123 };

        /// <summary>
        /// 获取持久连接协议
        /// </summary>
        /// <returns></returns>
        public static Memory<byte> KeepAliveObj { get; } = new byte[] { 123, 1, 2, 3, 2, 1, 123 };

        /// <summary>
        /// 检查率
        /// </summary>
        public int TimeDelay { get; }

        /// <summary>
        /// 创建心跳对象
        /// </summary>
        /// <param name="TimeInterval">心跳频率</param>
        /// <param name="OnStart">心跳触发器内部捕获了异常</param>
        public KeepAlive(byte TimeInterval, Func<Task> OnStart): this(TimeInterval * 1000, OnStart) {}

        internal KeepAlive(int TimeInterval, Func<Task> OnStart)
        {
            if (TimeInterval < 100)
            {
                throw new ArgumentException("TimeInterval 值必须>0！", nameof(TimeInterval));
            }
            TimeDelay = TimeInterval < 1000 ? 10 : 100; //检查频率
            ElapsedTicks = DateTime.UtcNow.Ticks / 10000;
            this.TimeInterval = TimeInterval;
            this.OnStart = OnStart ?? throw new ArgumentNullException(nameof(OnStart), "OnStart 不能为空！");
            //Task.Factory.StartNew(HeartBeatStart, TaskCreationOptions.LongRunning);//Timer
            ObjectExtension.RunTask(HeartBeatStart, TaskCreationOptions.LongRunning);
            //HeartBeatStart().
        }

        /// <summary>
        /// 开始心跳事件
        /// </summary>
        private Func<Task> OnStart { get; }

        /// <summary>
        /// 间隔时间，不能小于1秒
        /// </summary>
        public long TimeInterval { get; }

        private long MaxDelay => TimeInterval + TimeDelay;

        private bool OnClose = false;

        /// <summary>
        /// 逝去的时间
        /// </summary>
        private long ElapsedTicks;

        /// <summary>
        /// 时间差
        /// </summary>
        private long TimeDifference => DateTime.UtcNow.Ticks / 10000 - ElapsedTicks;

        /// <summary>
        /// 判断是否满足心跳事件
        /// </summary>
        private bool IsTimeInterval => TimeDifference >= TimeInterval;

        private async Task HeartBeatStart()
        {
            //Task.Factory.StartNew(() =>
            //{
            //Thread.CurrentThread.Name ??= "心跳线程";
            while (!OnClose)
            {
                //Debug.WriteLine("{0} - {1}", ObjectExtension.Thread.Name, ObjectExtension.Thread.ManagedThreadId);
                await Task.Delay(TimeDelay);//固定异步切片100毫秒一次
                //Debug.WriteLine("{0} - {1}", ObjectExtension.Thread.Name, ObjectExtension.Thread.ManagedThreadId);
                if (IsTimeInterval)
                {
                    try
                    {
                        await OnStart();
                    }
                    catch
                    {

                    }
                    finally
                    {
                        ResetTime();
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
            var diff = TimeDifference;
            if (diff > MaxDelay) diff -= TimeDelay;
            //long obj = diff, obj1 = diff - MaxDelay; Debug.WriteLine($"时差：{obj} {obj1} {diff}");
            Interlocked.Add(ref ElapsedTicks, diff);
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
