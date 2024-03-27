using System.Threading;
using System;

namespace Tool.Sockets.NetFrame.Internal
{

    /**
     * 内部包
     */
    internal class ThreadObj : IDisposable
    {
        private ManualResetEvent Reset;

        public ThreadObj(string onlyID)
        {
            Reset = new ManualResetEvent(false);
            Response = new NetResponse(onlyID);
        }

        public NetResponse Response { get; private set; }

        public bool Set() => Reset.Set();

        public bool WaitOne(int Millisecond) => Reset.WaitOne(Millisecond, true);

        /**
         * 回收资源
         */
        public void Dispose()
        {
            if (Reset is null) return;
            Reset.Close();
            Reset.Dispose();
            Reset = null;
            Response = null;
            GC.SuppressFinalize(this);
        }
    }

}
