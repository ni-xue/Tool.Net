using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.UdpHelper;
using Tool.Utils;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 对异步接收时的对象状态的封装，将socket与接收到的数据封装在一起
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class UdpStateObject : StateObject
    {
        private readonly ReceiveEvent<IUdpCore> received;
        private DateTime TimeoutSignal = DateTime.UtcNow;

        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="udp">udp信息</param>
        /// <param name="DataLength">包的大小</param>
        /// <param name="OnlyData">是否保证有效</param>
        /// <param name="Received">完成时事件</param>
        public UdpStateObject(IUdpCore udp, int DataLength, bool OnlyData, ReceiveEvent<IUdpCore> Received)
        {
            this.Udp = udp;
            this.IpPort = Udp.Ipv4;
            this.DataLength = DataLength;
            this.OnlyData = OnlyData;
            this.received = Received;
            //receiveByteslist = new(func: (a) => received(a)); //ActionBlock
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
            //if (OnlyData)
            //{
            //    keepObj = KeepAlive.UdpKeepObj;
            //    ushort orderCount = Udp.AddWriteOrderCount();
            //    SetDataHeadUdp(keepObj.Span, orderCount);
            //}
            //else
            //{
            keepObj = KeepAlive.KeepAliveObj;
            //}
            return keepObj;
        }

        internal bool IsKeepAlive(in Memory<byte> ListSpan)
        {
            ReadOnlySpan<byte> span =/* OnlyData ? ListSpan[6..].Span : */ ListSpan.Span;
            return Utility.SequenceCompare(in span, KeepAliveObj);
        }

        internal async ValueTask OnReceive(bool IsThreadPool, BytesCore owner)
        {
            try
            {
                if (received is null)
                {
                    owner.Dispose();
                    return;
                }
                var data = new ReceiveBytes<IUdpCore>(IpPort, Udp, owner, OnlyData);
                if (IsThreadPool)
                {
                    QueueUserWorkItem(received, data);
                }
                else
                {
                    await ReceivedAsync(received, data);//触发接收事件
                    //receiveByteslist.Add(data);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"任务Core{(IsThreadPool ? "池" : "")}异常", ex, "Log/Udp");
            }
        }


        /// <summary>
        /// 当前有个通信信息
        /// </summary>
        public IUdpCore Udp { get; }

        /// <summary>
        /// 当前对象唯一的IP：端口
        /// </summary>
        public UserKey IpPort { get; }

        /// <summary>
        /// 可用最大空间
        /// </summary>
        public int DataLength { get; }

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        /// <summary>
        /// 当前连接是否在线
        /// </summary>
        /// <param name="receiveTimeout">参数为超时最大值</param>
        public bool IsOnLine(int receiveTimeout) => (DateTime.UtcNow - TimeoutSignal).TotalMilliseconds < receiveTimeout;

        internal void UpDateSignal() => TimeoutSignal = DateTime.UtcNow;

        internal static async ValueTask<SocketReceiveFromResult> ReceiveFromAsync(Socket client, Memory<byte> bytes, EndPoint point, int receiveTimeout = 0)
        {
            A:
            try
            {
                using CancellationTokenSource tokenSource = receiveTimeout > 0 ? new(receiveTimeout) : null;
                var token = tokenSource?.Token ?? CancellationToken.None;
#if NET5_0
                SocketReceiveFromResult result = await Task.Run(() => client.ReceiveFromAsync(bytes.AsArraySegment(), SocketFlags.None, point), token);
#else
                SocketReceiveFromResult result = await client.ReceiveFromAsync(bytes, SocketFlags.None, point, token);
#endif
                if (result.ReceivedBytes is -1 or 0) throw new Exception("空包，断线！");
                return result;
            }
            catch (SocketException ex) when (ex.SocketErrorCode is SocketError.MessageSize)
            {
                goto A; //直接忽略这类错误
            }
            catch (OperationCanceledException)
            {
                return new SocketReceiveFromResult { ReceivedBytes = -1 };
            }
        }

        /// <summary>
        /// 回收对象所以资源
        /// </summary>
        public void Close()
        {
            //暂无可回收资源
        }
    }
}
