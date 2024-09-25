using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.UdpHelper.Extend;
using Tool.Utils;

namespace Tool.Sockets.UdpHelper
{
    /// <summary>
    /// Udp通信核心，管理相关连接信息
    /// </summary>
    public interface IUdpCore : IAsyncDisposable
    {
        /// <summary>
        /// 分配id最大等待时间
        /// </summary>
        public static int SpinWaitTimeout { get; set; } = 60 * 1000;

        /// <summary>
        /// 控制滑动窗口大小（限制流量）备注：计算得出1000个窗口比较满足大多数环境
        /// </summary>
        public static int LimitingSize { get; set; } = 100;

        /// <summary>
        /// 获取IpV4地址信息
        /// </summary>
        public Ipv4Port Ipv4 => EndPoint.Ipv4;

        /// <summary>
        /// 连接的设备地址信息
        /// </summary>
        public UdpEndPoint EndPoint { get; }

        /// <summary>
        /// 连接的对象（请勿脱离框架使用，避免出现各种未知异常）
        /// </summary>
        public Socket Socket { get; }

        /// <summary>
        /// 可用最大空间
        /// </summary>
        public int DataLength { get; }

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        /// <summary>
        /// 管理核心
        /// </summary>
        internal UdpStateObject UdpState { get; }

        internal uint AddWriteOrderCount() => uint.MinValue;

        internal bool IsOnLine(int receiveTimeout);

        internal Memory<byte> GetSendMemory(in SendBytes<IUdpCore> sendBytes, ref bool ispart, ref int i);

        internal Task ReceiveAsync(Memory<byte> memory);

        #region 公开接口

        /// <summary>
        /// 尝试异步关闭连接
        /// </summary>
        /// <returns></returns>
        public Task CloseAsync();

        /// <summary>
        /// 直接发送数据（警告非对内核完全了解的开发者，请谨慎，会出现各种未知异常！）
        /// </summary>
        /// <param name="memory">发送的数据</param>
        /// <returns></returns>
        public Task SendAsync(Memory<byte> memory);

        /// <summary>
        /// 创建可用的公共UDP核心
        /// </summary>
        /// <param name="networkCore"></param>
        /// <param name="endPoint"></param>
        /// <param name="socket"></param>
        /// <param name="dataLength"></param>
        /// <param name="onlyData"></param>
        /// <param name="replyDelay"></param>
        /// <param name="isserver"></param>
        /// <param name="isp2p"></param>
        /// <param name="complete"></param>
        /// <param name="received"></param>
        /// <returns></returns>
        public static IUdpCore GetUdpCore(INetworkCore networkCore, UdpEndPoint endPoint, Socket socket, int dataLength, bool onlyData, int replyDelay, bool isserver, bool isp2p, Func<UserKey, byte, ValueTask> complete, ReceiveEvent<IUdpCore> received) 
        {
            if (onlyData)
            {
                return new UdpStream(networkCore, endPoint, socket, dataLength, onlyData, replyDelay, isserver, isp2p, complete, received);
            }
            else 
            {
                return new UdpPack(networkCore, endPoint, socket, dataLength, onlyData, isserver, isp2p, complete, received);
            }
        }

        #endregion
    }
}
