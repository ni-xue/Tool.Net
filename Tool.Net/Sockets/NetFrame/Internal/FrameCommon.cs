using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Text;
using Tool.Sockets.Kernels;
using Tool.Utils;

namespace Tool.Sockets.NetFrame.Internal
{
    internal class FrameCommon
    {
        ///**
        // * 返回可查找的方法键值
        // */
        //internal static string GetOnlyID(int ClassID, int ActionID)
        //{
        //    //string ClMID = string.Empty;

        //    //int ManagedThreadId = ObjectExtension.Thread.ManagedThreadId;
        //    string guid = StringExtension.GetGuid();
        //    string clmidmt = string.Concat(guid, ClassID.ToString(), ".", ActionID.ToString()); //$"{ClMID}/{ManagedThreadId}";.Substring(10,21)
        //    return clmidmt;
        //}

        internal static void SetApiPacket(ApiPacket api, bool isServer, Ipv4Port ipPort = default) //bool isSend, bool isErr, 
        {
            if (api == null) throw new ArgumentException(" ApiPacket 对象不能为空！", nameof(api));
            api.isServer = isServer;
            api.ipPort = ipPort;
        }

        internal static IDataPacket GetDataPacket(ApiPacket api, in Guid clmidmt)
        {
            SendDataPacket dataPacket = new(api.ClassID, api.ActionID, clmidmt)
            {
                IsSend = true,
                IsErr = false,
                IsServer = api.isServer,
                IsReply = api.IsReply,
                IpPort = api.ipPort,
                Text = api.FormatData(),
            };
            dataPacket.SetBuffer(api.Bytes);
            return dataPacket;
        }

        /// <summary>
        /// 验证并确保包100%完整
        /// </summary>
        /// <param name="isSorC">验证差异</param>
        /// <param name="packet">原始包</param>
        /// <returns></returns>
        internal static bool IsComplete(bool isSorC, ref DataPacket packet)
        {
            if (packet.IsServer == isSorC)// 验证差异 服务端客户端 差异
            {
                packet.Dispose();
                return false;
            }

            if (packet.NotIsMany || packet.IsIpIdea && !packet.IsServer)
            {
                return true;
            }
            else
            {
                Guid OnlyID = packet.OnlyId;
                int objcount = packet.Many.End.Value;

                //if (packet.IsSend)
                //{
                //    int count = packet.Many.End.Value;
                //    _threadObj = StaticData.TcpByteObjs.GetOrAdd(OnlyID, a => new TcpByteObjs(count));
                //}
                //else if (!StaticData.TcpByteObjs.TryGetValue(OnlyID, out _threadObj))
                //{
                //    packet.Dispose();
                //    return false;
                //}

                NetByteObjs _byteObjs = StaticData.TcpByteObjs.GetOrAdd(OnlyID, a => new NetByteObjs(objcount));

                //lock (_byteObjs._lock)
                //{
                _byteObjs.OjbCount[packet.Many.Start.Value] = packet.Bytes;
                _byteObjs.Length += packet.Bytes.Count;
                _byteObjs.Count--;

                if (_byteObjs.Count == 0)
                {
                    // Memory<>
                    ArraySegment<byte> bytes = new byte[_byteObjs.Length];
                    int count = 0; //计数用于处理多包叠加
                    for (int i = 0; i < _byteObjs.OjbCount.Length; i++)
                    {
                        count += i > 0 ? _byteObjs.OjbCount[i - 1].Count : 0;
                        _byteObjs.OjbCount[i].CopyTo(bytes[count..]);
                    }

                    int length = StateObject.GetDataHeadTcp(bytes);//(bytes[0..TcpStateObject.HeadSize]);

                    if (length > 0)//这里为处理问题
                    {
                        //packet.Text = Encoding.UTF8.GetString(bytes, 6, length);
                        packet.TextBytes = bytes.Slice(StateObject.HeadSize, length).ToArray();
                        int bytelength = StateObject.HeadSize + length;
                        if (bytes.Count > bytelength) packet.Bytes = bytes[bytelength..].ToArray();//完全拷贝
                    }
                    else
                    {
                        packet.Bytes = bytes;
                    }
                    packet.EmptyMany();
                    //if (packet.IsSend)
                    //{
                    //    StaticData.TcpByteObjs.TryRemove(OnlyID, out _);
                    //}
                    StaticData.TcpByteObjs.TryRemove(OnlyID, out _);
                    //_byteObjs.Dispose();
                    return true;
                }
                packet.Dispose();
                //}

                return false;
            }
        }

        /**
         * 根据一次性回复数据包解析成一个或多个包
         */
        internal static bool Receiveds(in ReceiveBytes<Socket> tcpBytes, out IDataPacket dataPacket)
        {
            try
            {
                dataPacket = new ReceiveDataPacket(tcpBytes);
                return true;

                //var spans = tcpBytes.Memory.Span;
                //if (spans[0] == KeepAlive.KeepAliveObj[0])
                //{
                //    dataPacket = DataPacket.DataByte(spans);
                //    return true;
                //}
                //dataPacket = default;
                //return false;
            }
            catch (Exception ex)
            {
                Log.Error("消息解包异常", ex, "Log/NetFrame");
                dataPacket = default;
                return false;
            }
        }

        internal static ThreadObj TryThreadObj(ConcurrentDictionary<Guid, ThreadObj> pairs, in Guid clmidmt, bool isreply)
        {
            return isreply ? pairs.AddOrUpdate(clmidmt, AddThreadObj, UpdateThreadObj) : AddThreadObj(clmidmt);

            //if (pairs.TryRemove(clmidmt, out ThreadObj _threadObj))
            //{
            //    _threadObj.Response.OnNetFrame = NetFrameState.OnlyID;
            //    _threadObj.Set();
            //}

            //pairs.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt));

            //return _threadObj;

            ThreadObj AddThreadObj(Guid clmidmt)
            {
                var threadObj = new ThreadObj(isreply);//ThreadObjExtension.ObjPool.Get();
                //threadObj.Reset();
                return threadObj;
            }

            ThreadObj UpdateThreadObj(Guid clmidmt, ThreadObj removeThreadObj)
            {
                removeThreadObj.Set(NetFrameState.OnlyID);
                return AddThreadObj(clmidmt);
            }
        }
    }
}
