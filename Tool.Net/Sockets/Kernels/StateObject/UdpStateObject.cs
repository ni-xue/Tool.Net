using System;
//using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Utils;
using Tool.Utils.ThreadQueue;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 对异步接收时的对象状态的封装，将socket与接收到的数据封装在一起
    /// </summary>
    public class UdpStateObject : StateObject
    {
        /// <summary>
        /// 分配id最大等待时间
        /// </summary>
        public static int SpinWaitTimeout { get; set; } = 60 * 1000;

        /// <summary>
        /// 控制滑动窗口大小（限制流量）
        /// </summary>
        public static int LimitingSize { get; set; } = 100;

        private ushort readOrderCount = ushort.MinValue;
        private ushort writeOrderCount = ushort.MinValue;
        private readonly ReceiveEvent<UdpEndPoint> received;
        private readonly ConcurrentDictionary<ushort, ManualResetEventSlim> doSends;
        private readonly HashSet<ushort> readHash;
        private readonly TaskOueue<ReceiveBytes<UdpEndPoint>> receiveByteslist;
        private DateTime TimeoutSignal = DateTime.UtcNow;

        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="point">IP信息</param>
        /// <param name="DataLength">包的大小</param>
        /// <param name="OnlyData">是否保证有效</param>
        /// <param name="Received">完成时事件</param>
        public UdpStateObject(UdpEndPoint point, int DataLength, bool OnlyData, ReceiveEvent<UdpEndPoint> Received)
        {
            this.Point = point;
            this.IpPort = GetIpPort(point);
            this.DataLength = DataLength;
            this.OnlyData = OnlyData;
            this.received = Received;
            doSends = new();
            readHash = new();
            if (OnlyData) receiveByteslist = new(func: (a) => received(a));
        }

        /// <summary>
        /// 根据Socket获取当前连接是否已经断开
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public static bool IsConnected(Socket Client)
        {
            if (Client is null) return false;
            if (SocketIsDispose(Client)) return false;
            //if (!Client.Connected) return false;
            return true;
        }

        internal Memory<byte> GetKeepObj()
        {
            Memory<byte> keepObj;
            if (OnlyData)
            {
                keepObj = KeepAlive.UdpKeepObj;
                ushort orderCount = AddWriteOrderCount();
                SetDataHeadUdp(keepObj.Span, orderCount, 0);
            }
            else
            {
                keepObj = KeepAlive.KeepAliveObj;
            }
            return keepObj;
        }

        internal bool IsKeepAlive(ReadOnlySpan<byte> ListSpan)
        {
            if (OnlyData) ListSpan = ListSpan[6..];
            return Utility.SequenceCompare(in ListSpan, KeepAliveObj);
        }

        internal void OnReceive(bool IsThreadPool, Memory<byte> listData)
        {
            try
            {
                if (received is null) return;
                var data = new ReceiveBytes<UdpEndPoint>(IpPort, Point, listData.Length, OnlyData);
                data.SetMemory(listData);
                if (IsThreadPool)
                {
                    QueueUserWorkItem(received, data);
                }
                else
                {
                    receiveByteslist.Add(data);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"多包线程{(IsThreadPool ? "池" : "")}异常", ex, "Log/Tcp");
            }
        }

        /// <summary>
        /// 当前有个通信信息
        /// </summary>
        public UdpEndPoint Point { get; }

        /// <summary>
        /// 当前对象唯一的IP：端口
        /// </summary>
        public UserKey IpPort { get; }

        /// <summary>
        /// 读序列数（仅在OnlyData状态下支持）
        /// </summary>
        public ushort ReadOrderCount => readOrderCount;

        /// <summary>
        /// 写序列数（仅在OnlyData状态下支持）
        /// </summary>
        public ushort WriteOrderCount => writeOrderCount;

        /// <summary>
        /// 可用最大空间
        /// </summary>
        public int DataLength { get; }

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        internal ushort AddReadOrderCount(ushort orderCount, out bool isReceive)
        {
            ushort count = (ushort)(readOrderCount + 1);
            if (count == orderCount)
            {
                isReceive = true;
                return readOrderCount.Increment();
            }
            else if (orderCount > count)
            {
                do
                {
                    readHash.Add(count);
                    count = readOrderCount.Increment();
                } while (orderCount > count);
                isReceive = true;
                return count;
            }
            else
            {
                readHash.Remove(orderCount);
                isReceive = false;
                return orderCount;
            }
        }

        internal ushort AddWriteOrderCount()
        {
            if (!OnlyData) return 0;
            if (SpinWait.SpinUntil(isOrder, SpinWaitTimeout))//Timeout.Infinite
            {
                return writeOrderCount.Increment();//SpinWaitTimeout
            }
            throw new Exception("无法获取，新的序列号，滑动窗口长时间没有结果！");
            bool isOrder() => doSends.Count <= LimitingSize;
        }

        /// <summary>
        /// 当前连接是否在线
        /// </summary>
        /// <param name="receiveTimeout">参数为超时最大值</param>
        public bool IsOnLine(int receiveTimeout) => (DateTime.UtcNow - TimeoutSignal).TotalMilliseconds < receiveTimeout;

        internal void UpDateSignal() => TimeoutSignal = DateTime.UtcNow;

        internal void Set(ushort orderCount)
        {
            if (doSends.TryRemove(orderCount, out var doSend))
            {
                doSend.Set();
            }
        }

        internal bool Wait(ushort orderCount, int replyDelay, ref int count)
        {
            if (!OnlyData) return true;
            var doSend = doSends.AddOrUpdate(orderCount, AddSlim, UpdateSlim);
            bool iswait = false;
            try
            {
                return iswait = doSend.Wait(replyDelay);
            }
            finally
            {
                if (iswait)
                {
                    doSend.Dispose();
                }
                else
                {
                    if (count == 10)
                    {
                        doSend.Dispose();
                    }
                    else
                    {
                        count++;
                    }
                }
            }

            ManualResetEventSlim AddSlim(ushort orderCount) => new(false);

            ManualResetEventSlim UpdateSlim(ushort orderCount, ManualResetEventSlim removeSlim)
            {
                removeSlim.Set();
                return AddSlim(orderCount);
            }
        }

        /// <summary>
        /// 任务处理机制 合并
        /// </summary>
        internal bool OnReceiveTask(in ReadOnlySpan<byte> bytes, int count, out bool isreply, out bool isReceive)
        {
            if (OnlyData)
            {
                if (count >= HeadSize && GetDataHeadUdp(in bytes, out ushort orderCount))
                {
                    if (count == HeadSize)
                    {
                        Set(orderCount);
                        return isreply = isReceive = false;
                    }
                    else
                    {
                        uint a = AddReadOrderCount(orderCount, out isReceive);
                        Debug.WriteLineIf(orderCount - a != 0, $"当前位置：{orderCount}-{a}");
                        return isreply = true;
                    }
                }
                else
                {
                    throw new InvalidOperationException("有效版Udp数据异常！");
                }
            }
            else
            {
                isreply = false;
            }
            return isReceive = true;
        }

        internal static async ValueTask<int> SendNoWaitAsync(Socket client, EndPoint point, Memory<byte> buffers)
        {
            if (point is null)
            {
                throw new ArgumentException("client 对象是空的！", nameof(point));
            }
#if NET5_0
            return await client.SendToAsync(buffers.AsArraySegment(), SocketFlags.None, point);
#else
            return await client.SendToAsync(buffers, SocketFlags.None, point);
#endif
        }

        internal static async ValueTask<SocketReceiveFromResult> ReceiveFromAsync(Socket client, ArraySegment<byte> bytes, EndPoint point, int receiveTimeout = 0)
        {
            using CancellationTokenSource tokenSource = receiveTimeout > 0 ? new(receiveTimeout) : null;
            var token = tokenSource?.Token ?? CancellationToken.None;
#if NET5_0
            SocketReceiveFromResult result = await Task.Run(() => client.ReceiveFromAsync(bytes, SocketFlags.None, point), token);
#else
            SocketReceiveFromResult result = await client.ReceiveFromAsync(bytes, SocketFlags.None, point, token);
#endif
            if (result.ReceivedBytes is -1 or 0) throw new Exception("空包，断线！");
            return result;
        }

        /// <summary>
        /// 回收对象所以资源
        /// </summary>
        public void Close()
        {
            //SpareSize = 0;
            //WriteIndex = 0;
            var values = doSends.Values;
            foreach (var val in values)
            {
                val.Dispose();
            }
            doSends.Clear();
            readHash.Clear();
        }
    }
}
