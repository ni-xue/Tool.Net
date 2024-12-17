using System.Threading;
using System;
using System.Collections.Concurrent;
using Tool.Sockets.Kernels;
using Tool.Utils;
using Tool.Utils.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

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

        //internal static void SetTimeout(this ConcurrentDictionary<Guid, ThreadObj> pairs, in Guid onlyId)
        //{
        //    if (pairs.TryRemove(onlyId, out var threadObj))
        //    {
        //        threadObj.State = NetFrameState.Timeout;
        //    }
        //}

        //internal static void SetException(this ConcurrentDictionary<Guid, ThreadObj> pairs, in Guid onlyId, in Exception ex)
        //{
        //    if (pairs.TryRemove(onlyId, out var threadObj)) threadObj.SetSendFail(ex);
        //}

        internal static void SetSendFail(this ThreadObj threadObj, in Exception ex)
        {
            threadObj.Error = ex;
            threadObj.Set(NetFrameState.SendFail);
        }
    }

    /**
     * 内部包
     */
    internal class ThreadObj : IDisposable
    {
        internal readonly bool IsReply;
        internal readonly Guid OnlyId;

        private bool disposedValue;
        //private ManualResetEventSlim ResetEven;
        internal NetFrameState State;
        internal Exception Error;
        internal IDataPacket Packet;

        private TaskCompletionSource<NetResponse> source;

        public ThreadObj(Guid clmidmt, bool isreply)
        {
            OnlyId = clmidmt;
            IsReply = isreply;
            //ResetEven = new ManualResetEventSlim(false, 50);//ManualResetEvent(false);
            source = new(TaskCreationOptions.RunContinuationsAsynchronously);
        }

        private NetResponse GetResponse()
        {
            if (State > NetFrameState.Default)
            {
                return new NetResponse(in OnlyId, IsReply, State, Error);
            }
            else if (Packet is not null)
            {
                return new NetResponse(in Packet);
            }
            else
            {
                return new NetResponse(in OnlyId, IsReply, NetFrameState.Exception, new Exception("意料之外的异常，不因发生！"));
            }
        }

        private bool Set()
        {
            if (!disposedValue)
            {
                source.TrySetResult(GetResponse());
                //ResetEven.Set();
                return true;
            }
            return false;
        }

        public bool Set(in IDataPacket packet)
        {
            Packet = packet;
            return Set();
        }

        public bool Set(in NetFrameState state)
        {
            State = state;
            return Set();
        }

        public async Task<NetResponse> WaitResponse(int Millisecond, ThreadUuIdObj threadUuIdObj)
        {
            if (IsReply)
            {
                using CancellationTokenSource cts = new(Millisecond);
                cts.Token.UnsafeRegister((state) =>
                {
                    threadUuIdObj.SetTimeout(in OnlyId, this);
                }, null);
                //return ResetEven.Wait(Millisecond);
                return await source.Task;
            }
            else
            {
                State = NetFrameState.Success;
                Set();
                return await source.Task;
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
            if (!disposedValue)
            {
                disposedValue = true;
                //ResetEven.Dispose();
                Error = null;
                Packet = null;
                source = null;
                //ResetEven = null;
            }
            GC.SuppressFinalize(this);
        }
    }

    internal class ThreadUuIdObj : IDisposable
    {
        private bool disposedValue;

        internal ProtocolStatus protocol = ProtocolStatus.Unknown;

        /**
         * 当前要同步等待的线程组信息
         */
        private readonly ConcurrentDictionary<Guid, ThreadObj> pairs = new();

        internal bool TryThreadObj(in Guid clmidmt, bool isreply, out ThreadObj threadObj, out NetResponse response)
        {
            if (FrameCommon.TryProtocolStatus(protocol, in clmidmt, isreply, out response))
            {
                threadObj = isreply ? pairs.AddOrUpdate(clmidmt, AddThreadObj, UpdateThreadObj) : AddThreadObj(clmidmt);
                return true;
            }
            threadObj = default;
            return false;

            ThreadObj AddThreadObj(Guid clmidmt) => new(clmidmt, isreply);
            ThreadObj UpdateThreadObj(Guid clmidmt, ThreadObj removeThreadObj)
            {
                removeThreadObj.Set(NetFrameState.OnlyID);
                return AddThreadObj(clmidmt);
            }
        }

        internal void SetTimeout(in Guid onlyId, ThreadObj threadObj)
        {
            //if (pairs.TryRemove(onlyId, out var threadObj)) threadObj.State = NetFrameState.Timeout;
            if (Destroy(in onlyId, in threadObj)) threadObj.Set(NetFrameState.Timeout);
        }

        internal void SetException(in Guid onlyId, ThreadObj threadObj, Exception ex)
        {
            //if (pairs.TryRemove(onlyId, out var threadObj)) threadObj.SetSendFail(ex);
            if (Destroy(in onlyId, in threadObj)) threadObj.SetSendFail(ex);
        }

        private bool Destroy(in Guid onlyId, in ThreadObj threadObj) 
        {
            return pairs.TryRemove(new(onlyId, threadObj));
        }

        internal bool Complete(in Guid clmidmt, out ThreadObj Threads)
        {
            return pairs.TryRemove(clmidmt, out Threads);
        }

        internal void AllError() //广播所有错误
        {
            while (!pairs.IsEmpty)
            {
                foreach (var pair in pairs)
                {
                    if (pairs.TryRemove(pair))
                    {
                        var threadobj = pair.Value;
                        threadobj.Error = new Exception("等待回复时与服务器已断开连接！");
                        threadobj.Set(NetFrameState.Exception);
                    }
                }
            }
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                disposedValue = true;
                AllError();
            }
            GC.SuppressFinalize(this);
        }
    }

    internal class ThreadKeyObj : IDisposable
    {
        private bool disposedValue;

        /**
         * 当前要同步等待的线程组信息
         */
        private readonly ConcurrentDictionary<Ipv4Port, ThreadUuIdObj> pairs = new();

        internal ThreadUuIdObj TryAdd(in Ipv4Port ipv4Port)
        {
            var threadUuIdObj = pairs.GetOrAdd(ipv4Port, AddThreadObj);
            threadUuIdObj.protocol = ProtocolStatus.Connect;
            return threadUuIdObj;
            static ThreadUuIdObj AddThreadObj(Ipv4Port ipv4Port) => new();
        }

        internal bool TryThreadObj(in Ipv4Port ipv4Port, in Guid clmidmt, bool isreply, out ThreadUuIdObj threadUuIdObj, out ThreadObj threadObj, out NetResponse response)
        {
            if (pairs.TryGetValue(ipv4Port, out threadUuIdObj) && threadUuIdObj.TryThreadObj(in clmidmt, isreply, out threadObj, out response))
            {
                return true;
            }
            else
            {
                //必然是已经断开连接的用户
                threadUuIdObj = default;
                threadObj = default;
                return FrameCommon.TryProtocolStatus(ProtocolStatus.Close, in clmidmt, isreply, out response);
            }
        }

        internal bool Complete(in Ipv4Port ipv4Port, in Guid clmidmt, out ThreadObj Threads)
        {
            if (pairs.TryGetValue(ipv4Port, out var threadUuIdObj) && threadUuIdObj.Complete(clmidmt, out Threads))
            {
                return true;
            }
            else
            {
                Threads = default;
                return false;
            }
        }

        internal void Release(in Ipv4Port ipv4Port)
        {
            if (pairs.TryRemove(ipv4Port, out ThreadUuIdObj threadUuIdObj))
            {
                threadUuIdObj.protocol = ProtocolStatus.Close;
                threadUuIdObj.Dispose();
            }
        }

        private void AllError() //广播所有错误
        {
            while (!pairs.IsEmpty)
            {
                foreach (var pair in pairs)
                {
                    if (pairs.TryRemove(pair))
                    {
                        var threadUuIdObj = pair.Value;
                        threadUuIdObj.Dispose();
                    }
                }
            }
        }

        public void Dispose()
        {
            if (!disposedValue)
            {
                disposedValue = true;
                AllError();
            }
            GC.SuppressFinalize(this);
        }
    }
}
