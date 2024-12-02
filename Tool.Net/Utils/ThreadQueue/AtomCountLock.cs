using System;
using System.Diagnostics;
using System.Threading;

namespace Tool.Utils.ThreadQueue
{
    /// <summary>
    /// 一个原子计数锁，可以确保多线程下，可调用区域内只能调用固定次数
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    [DebuggerDisplay("阈值={InitialCount}, 当前值={CurrentCount}")]
    public class AtomCountLock : IDisposable
    {
        //全部暂停锁
        private readonly ManualResetEvent _event;

        /// <summary>
        /// 最大阈值
        /// </summary>
        public uint InitialCount { get; }

        /// <summary>
        /// 当前调用次数
        /// </summary>
        public uint CurrentCount => _currentCount;

        private volatile uint _currentCount;
        //private readonly ManualResetEventSlim _event;  

        /// <summary>
        /// 初始化一个原子锁
        /// </summary>
        /// <param name="initialCount">指定最大可用数量</param>
        public AtomCountLock(uint initialCount)
        {
            _event = new ManualResetEvent(false);
            InitialCount = initialCount;
        }

        /// <summary>
        /// 达到最大阈值，只上锁不计数
        /// </summary>
        /// <returns></returns>
        public bool Wait()
        {
            //Interlocked.Increment(ref _currentCount);
            if (Interlocked.Increment(ref _currentCount) >= InitialCount)
            {
                Interlocked.Exchange(ref _currentCount, InitialCount);
                if (!_event.WaitOne())
                {
                    
                }
                return Wait();
            }
            //Interlocked.Increment(ref _currentCount);
            return true;
        }

        /// <summary>
        /// 没调用一次可获得一次调用计数
        /// </summary>
        /// <returns></returns>
        public bool Set()
        {
            if (Interlocked.Decrement(ref _currentCount) >= InitialCount)
            {
                return _event.Set();
            }
            else
            {
                return _event.Reset();
            }
        }

        /// <summary>
        /// 将所有计数清空，将重新获得调用计数
        /// </summary>
        /// <returns></returns>
        public bool Reset()
        {
            if (Interlocked.Exchange(ref _currentCount, 0u) >= InitialCount)
            {
                return _event.Set();
            }
            else
            {
                _event.Reset();
            }
            return false;
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        public void Dispose()
        {
            _event.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
