using System.Threading;
using System;
using System.Collections.Concurrent;
using Tool.Sockets.Kernels;
using Tool.Utils;

namespace Tool.Sockets.NetFrame.Internal
{

    internal static class ThreadObjExtension
    {
        //static ObjectPool<ThreadObj> objectPool;

        //internal static ObjectPool<ThreadObj> ObjPool 
        //{ 
        //    get {

        //        if (objectPool is null)
        //        {
        //            lock (StateObject._lonk)
        //            {
        //                objectPool ??= new ObjectPool<ThreadObj>();
        //            }
        //        }
        //        return objectPool; 
        //    }
        //}

        internal static void SetTimeout(this ConcurrentDictionary<Guid, ThreadObj> pairs, in Guid onlyId)
        {
            if (pairs.TryRemove(onlyId, out var threadObj))
            {
                threadObj.State = NetFrameState.Timeout;
            }
        }

        internal static void SetException(this ConcurrentDictionary<Guid, ThreadObj> pairs, in Guid onlyId, in Exception ex)
        {
            if (pairs.TryRemove(onlyId, out var threadObj)) threadObj.SetSendFail(ex);
        }

        internal static void SetSendFail(this ThreadObj threadObj, in Exception ex)
        {
            threadObj.Error = ex;
            threadObj.State = NetFrameState.SendFail;
        }
    }

    /**
     * 内部包
     */
    internal class ThreadObj : IDisposable
    {
        private readonly bool IsReply;
        private ManualResetEventSlim ResetEven;
        internal NetFrameState State;
        internal Exception Error;
        internal IDataPacket Packet;

        public ThreadObj(bool isreply)
        {
            IsReply = isreply;
            ResetEven = new ManualResetEventSlim(false, 50);//ManualResetEvent(false);
        }

        public NetResponse GetResponse(in Guid onlyId)
        {
            if (State > NetFrameState.Default)
            {
                return new NetResponse(in onlyId, IsReply, State, Error);
            }
            else if (Packet is not null)
            {
                return new NetResponse(in Packet);
            }
            else
            {
                return new NetResponse(in onlyId, IsReply, NetFrameState.Exception, new Exception("意料之外的异常，不因发生！"));
            }
        }

        ///// <summary>
        ///// 初始化
        ///// </summary>
        //public void Reset()
        //{
        //    State = NetFrameState.Default;
        //    Error = null;
        //    Packet = null;
        //}

        public bool Set()
        {
            ResetEven.Set();
            return true;
        }

        public bool Set(in IDataPacket packet)
        {
            Packet = packet;
            return Set();
        }

        public bool Set(NetFrameState state)
        {
            State = state;
            return Set();
        }

        public bool WaitOne(int Millisecond)
        {
            if (IsReply)
            {
                return ResetEven.Wait(Millisecond);
            }
            else
            {
                State = NetFrameState.Success;
                return true;
            }

        }

        ///// <summary>
        ///// 回收复用
        ///// </summary>
        //public void Return() 
        //{
        //    ResetEven.Reset();
        //    ThreadObjExtension.ObjPool.Return(this);
        //}

        /**
         * 回收资源
         */
        public void Dispose()
        {
            ResetEven.Dispose();
            Error = null;
            Packet = null;
            ResetEven = null;
            GC.SuppressFinalize(this);
        }
    }
}
