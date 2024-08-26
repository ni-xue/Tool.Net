using System;
using System.Buffers.Binary;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.UdpHelper
{
    /// <summary>
    /// Udp通信专属模块
    /// </summary>
    public class UdpEndPoint : EndPoint
    {
        private SocketAddress _cacheAddress; //缓存可重用的对象提高性能

        /// <summary>
        /// IP地址信息
        /// </summary>
        public IPAddress Address => Ipv4.Ip;

        /// <summary>
        /// 通信端口
        /// </summary>
        public ushort Port => Ipv4.Port;

        /// <summary>
        /// 获取IpV4地址信息
        /// </summary>
        public Ipv4Port Ipv4 { get; }

        /// <summary>
        /// 创建一个Udp通信IP地址
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public UdpEndPoint(IPAddress address, ushort port) : base()
        {
            Memory<byte> memory = new byte[6];
            var span = memory.Span;
            address.TryWriteBytes(span, out int i);
            BinaryPrimitives.WriteUInt16LittleEndian(span[i..], port);
            Ipv4 = new Ipv4Port(in memory);
        }

        private UdpEndPoint(in Memory<byte> memory)
        {
            Ipv4 = new Ipv4Port(in memory);
        }

        /// <summary>
        /// 获取<see cref="SocketAddress"/>
        /// </summary>
        /// <returns></returns>
        public override SocketAddress Serialize()
        {
            if (_cacheAddress is { }) return _cacheAddress;

            SocketAddress address = new(AddressFamily);
            Span<byte> bytes = Ipv4.Span;
#if NET8_0_OR_GREATER
            Span<byte> span = address.Buffer.Span;
            bytes[4..].CopyTo(span[2..4]);
            bytes[..4].CopyTo(span[4..8]);
            span[2..4].Reverse();//大端
#else
            for (int i = 0; i < 4; i++)
            {
                if (i is 0)
                {
                    address[2] = bytes[5];
                    address[3] = bytes[4];
                }
                address[i + 4] = bytes[i];
            }
#endif
            Interlocked.CompareExchange(ref _cacheAddress, address, null); //当没有指针时，赋值。
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
            if (_cacheAddress is { })
            {
#if NET8_0_OR_GREATER
                if (Utils.Utility.SequenceCompare(_cacheAddress.Buffer.Span[..8], socketAddress.Buffer.Span[..8])) return this;
#else
                bool isok = true;
                for (int i = 0; i < 8; i++)
                {
                    if (_cacheAddress[i] != socketAddress[i])
                    {
                        isok = false;
                        break;
                    }
                }
                if (isok) return this;
#endif
            }

            if (socketAddress.Family != AddressFamily.InterNetwork) throw new Exception("Family 非 InterNetwork");
#if NET8_0_OR_GREATER
            Span<byte> span = socketAddress.Buffer.Span;
            Memory<byte> memory = new byte[6];
            Span<byte> bytes = memory.Span;
            span[4..8].CopyTo(bytes);
            span[2..4].CopyTo(bytes[4..]);
            bytes[4..].Reverse();
            return new UdpEndPoint(memory);
#else
            Memory<byte> memory = new byte[6];
            var span = memory.Span;
            for (int i = 0; i < 4; i++)
            {
                span[i] = socketAddress[i + 4];
                if (i is 3)
                {
                    span[4] = socketAddress[3];
                    span[5] = socketAddress[2];
                }
            }
            return new UdpEndPoint(memory);
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
        /// 判断是否一致
        /// </summary>
        /// <param name="obj">比较值</param>
        /// <returns>是或否</returns>
        public override bool Equals([NotNullWhen(true)] object obj)
        {
            return obj is UdpEndPoint other && Equals(other);
        }

        /// <summary>
        /// 判断是否一致
        /// </summary>
        /// <param name="other">比较值</param>
        /// <returns>是或否</returns>
        public bool Equals([NotNullWhen(true)] UdpEndPoint other)
        {
            return Ipv4 == other.Ipv4;
            //return Address.Equals(other.Address) && Port == other.Port;
        }

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return HashCode.Combine(Address, Port);
        }

        /// <summary>
        /// 指示两个 <see cref="UdpEndPoint"/> 结构是否不相等。
        /// </summary>
        /// <param name="a">不等运算符左侧的结构</param>
        /// <param name="b">不等运算符右侧的结构</param>
        /// <returns>如果 true 不等于 a，则为 b；否则为 false。</returns>
        public static bool operator !=(UdpEndPoint a, UdpEndPoint b) => !a.Equals(b);

        /// <summary>
        /// 指示两个 <see cref="UdpEndPoint"/> 结构是否相等。
        /// </summary>
        /// <param name="a">相等运算符左侧的结构</param>
        /// <param name="b">相等运算符右侧的结构</param>
        /// <returns>如果 true 等于 a，则为 b；否则为 false。</returns>
        public static bool operator ==(UdpEndPoint a, UdpEndPoint b) => a.Equals(b);

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
                if (port is < ushort.MinValue or > ushort.MaxValue) throw new Exception($"port 端口 {ushort.MinValue}~{ushort.MaxValue}");
                point = new UdpEndPoint(address, (ushort)port);
                return true;
            }
            point = null;
            return false;
        }
    }
}
