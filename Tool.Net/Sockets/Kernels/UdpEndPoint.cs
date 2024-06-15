using Microsoft.AspNetCore.Hosting.Server;
using System;
using System.Buffers.Binary;
using System.Drawing;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// Udp通信专属模块
    /// </summary>
    public class UdpEndPoint : EndPoint
    {
        private UdpStateObject udpState { get; set; }

        /// <summary>
        /// IP地址信息
        /// </summary>
        public IPAddress Address { get; }

        /// <summary>
        /// 通信端口
        /// </summary>
        public ushort Port { get; }

        /// <summary>
        /// 通信相关核心
        /// </summary>
        public UdpStateObject UdpState
        {
            get
            {
                if (udpState is null)
                {
                    if (!SpinWait.SpinUntil(isOrder, UdpStateObject.SpinWaitTimeout)) 
                    {
                        throw new Exception("无法使用UdpStateObject核心模型，请重试！");
                    }
                }
                return udpState;
                bool isOrder() => udpState is not null;
            }
        }

        /// <summary>
        /// 创建一个Udp通信IP地址
        /// </summary>
        /// <param name="Address"></param>
        /// <param name="port"></param>
        public UdpEndPoint(IPAddress Address, ushort port) : base()
        {
            this.Address = Address;
            this.Port = port;
        }

        /// <summary>
        /// 创建通信核心
        /// </summary>
        /// <param name="DataLength">包大小</param>
        /// <param name="OnlyData">是否保证包</param>
        /// <param name="Received">完成时的委托</param>
        public void SetUdpState(int DataLength, bool OnlyData, ReceiveEvent<UdpEndPoint> Received)
        {
            udpState = new(this, DataLength, OnlyData, Received);
        }

        internal ushort AddWriteOrderCount() => UdpState.AddWriteOrderCount();

        internal bool Wait(ushort orderCount, int replyDelay, ref int count) => UdpState.Wait(orderCount, replyDelay, ref count);

        internal void UpDateSignal() => UdpState.UpDateSignal();

        internal bool IsOnLine(int receiveTimeout) => UdpState.IsOnLine(receiveTimeout);

        internal static async Task ShareSendAsync(SendBytes<UdpEndPoint> sendBytes, bool onlyData, int replyDelay, Func<UdpEndPoint, Memory<byte>, ValueTask> func) 
        {
            if (sendBytes.OnlyData != onlyData)
            {
                throw new ArgumentException("与当前套接字（OnlyData）协议不一致！", nameof(sendBytes));
            }
            await Task.Run(async () =>
            {
                var point = sendBytes.Client;
                var udpState = point.UdpState;
                ushort orderCount = udpState.AddWriteOrderCount();

                var buffers = sendBytes.GetMemory(orderCount);
                int count = 1;
                if (replyDelay <= 0) replyDelay = 100;
            A:
                await func(point, buffers);
                if (!udpState.Wait(orderCount, replyDelay, ref count))
                {
                    if (count == 10) { throw new Exception("发送失败：重试10次，无回应！"); }
                    goto A;
                }
            });
        }

        /// <summary>
        /// 获取<see cref="SocketAddress"/>
        /// </summary>
        /// <returns></returns>
        public override SocketAddress Serialize()
        {
            SocketAddress address = new(AddressFamily);
#if NET8_0_OR_GREATER
            var span = address.Buffer.Span;
            BinaryPrimitives.WriteUInt16BigEndian(span[2..4], Port);
            Address.TryWriteBytes(span[4..8], out int byteWritten);
#else
            Span<byte> span = stackalloc byte[8];
            BinaryPrimitives.WriteUInt16BigEndian(span[2..4], Port);
            Address.TryWriteBytes(span[4..8], out int byteWritten);
            for (int i = 2; i < span.Length; i++)
            {
                address[i] = span[i];
            }
#endif
            return address;
        }

        /// <summary>
        /// IP地址类型
        /// </summary>
        public override AddressFamily AddressFamily => Address.AddressFamily;

        /// <summary>
        /// 根据数据获取<see cref="EndPoint"/>
        /// </summary>
        /// <param name="socketAddress"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public override EndPoint Create(SocketAddress socketAddress)
        {
            if (socketAddress.Family != AddressFamily.InterNetwork) throw new Exception("Family 非 InterNetwork");
#if NET8_0_OR_GREATER
            var span = socketAddress.Buffer.Span;
            ushort port = BinaryPrimitives.ReadUInt16BigEndian(span[2..4]);
            IPAddress address = new(span[4..8]);
            return new UdpEndPoint(address, port);
#else
            Span<byte> span = stackalloc byte[8];
            for (int i = 2; i < span.Length; i++)
            {
                span[i] = socketAddress[i];
            }
            ushort port = BinaryPrimitives.ReadUInt16BigEndian(span[2..4]);
            IPAddress address = new(span[4..8]);
            return new UdpEndPoint(address, port);
#endif
        }

        /// <summary>
        /// 默认信息
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Address}:{Port}";
        }

        /// <summary>
        /// 尝试判断是否可用的IP端口信息
        /// </summary>
        /// <param name="ip">IP信息</param>
        /// <param name="port">端口信息</param>
        /// <param name="point">返回的可用对象</param>
        /// <returns>是否成功</returns>
        /// <exception cref="Exception">失败的信息</exception>
        public static bool TryParse(string ip, int port, out UdpEndPoint point)
        {
            if (IPAddress.TryParse(ip, out var address))
            {
                if (address.AddressFamily != AddressFamily.InterNetwork) throw new Exception("AddressFamily 非 InterNetwork");
                point = new UdpEndPoint(address, (ushort)port);
                return true;
            }
            point = null;
            return false;
        }
    }
}
