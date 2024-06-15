using System.Runtime.Versioning;
using System.Threading;
using System;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.ObjectPool;

namespace Tool.Sockets.Kernels
{
    //LeakTrackingObjectPool ConditionalWeakTable
    /// <summary>
    /// 表示线程同步事件，收到信号时，必须手动重置该事件。此类是 System.Threading.ManualResetEvent 的轻量替代项。
    /// </summary>
    public class AutoResetEventSlim : IDisposable
    {
        private object latchLock = new object();
        // 0 = unset, 1 = set.
        private int m_state = 0;
        private readonly EventWaitHandle m_eventObj;

        public AutoResetEventSlim() : this(false) { }

        public AutoResetEventSlim(bool initialState) : this(initialState, 35) { }

        public AutoResetEventSlim(bool initialState, int spinCount)
        {
            m_eventObj = new EventWaitHandle(initialState, EventResetMode.AutoReset);
            SpinCount = spinCount;
        }

        public bool IsSet { get; private set; }

        /// <summary>
        /// 自旋次数
        /// </summary>
        public int SpinCount { get; }

        public WaitHandle WaitHandle => m_eventObj;

        public void Dispose()
        {
            WaitHandle.Dispose();
            GC.SuppressFinalize(this);
        }

        public bool Reset()
        {
            lock (latchLock)
            {
                bool isreset = m_eventObj.Reset();
                if (isreset)
                {
                    m_state = 0;
                    IsSet = false;
                }
                return isreset;
            }
        }

        public void Set()
        {
            lock (latchLock)
            {
                m_state = 1;
                IsSet = m_eventObj.Set();
            }
        }

        [UnsupportedOSPlatform("browser")]
        public void Wait() { Wait(Timeout.Infinite); }

        [UnsupportedOSPlatform("browser")]
        public bool Wait(int millisecondsTimeout)
        {
            SpinWait spinner = new SpinWait();
            System.Diagnostics.Stopwatch watch = default;
            int multiple = 0;

            while (m_state == 0)
            {
                watch ??= System.Diagnostics.Stopwatch.StartNew();

                if ((multiple * 10) + spinner.Count < SpinCount)
                {
                    if (spinner.NextSpinWillYield)
                    {
                        multiple++;
                        spinner = new SpinWait();
                    }
                    spinner.SpinOnce(Timeout.Infinite);
                }
                else
                {
                    long realTimeout = millisecondsTimeout - watch.ElapsedMilliseconds;
                    if (realTimeout <= 0 || !m_eventObj.WaitOne((int)realTimeout))
                    {
                        return false;
                    }
                }
            }

            Interlocked.Exchange(ref m_state, 0);
            return true;
        }

        public override string ToString()
        {
            return $"IsSet = {IsSet}";
        }
    }
}
