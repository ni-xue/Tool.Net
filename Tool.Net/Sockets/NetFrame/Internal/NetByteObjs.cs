using System;

namespace Tool.Sockets.NetFrame.Internal
{
    /**
     * 内部多包处理
     */
    internal class NetByteObjs //: IDisposable
    {
        public NetByteObjs(int count)
        {
            //_lock = new();
            Count = count;
            OjbCount = new ArraySegment<byte>[Count];
            Length = 0;
        }

        //public readonly object _lock; //一个锁，保证其在线程中的安全

        public int Count { get; set; }

        public int Length { get; set; }

        public ArraySegment<byte>[] OjbCount { get; private set; }

        //public void Dispose()
        //{
        //    if (this.OjbCount is not null)
        //    {
        //        // 请将清理代码放入下面
        //        this.Length = 0;
        //        this.Count = 0;
        //        this.OjbCount = null;
        //        GC.SuppressFinalize(this);
        //    }
        //}
    }
}
