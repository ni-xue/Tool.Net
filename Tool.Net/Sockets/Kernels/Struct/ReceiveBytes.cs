using System.Buffers;
using System.Net.Sockets;
using System;
using System.Net.WebSockets;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// Socket 通讯资源 对象（必须回收，丢失风险大）
    /// </summary>
    public readonly struct ReceiveBytes<ISocket> : IBytesCore
    {
        private readonly BytesCore _bytesCore;

        /// <summary>
        /// 资源对象
        /// </summary>
        /// <param name="key">IP端口</param>
        /// <param name="client">连接对象</param>
        /// <param name="length">包含长度</param>
        /// <param name="onlydata">数据包完整</param>
        public ReceiveBytes(in UserKey key, ISocket client, int length, bool onlydata)
        {
            Key = key;
            OnlyData = onlydata;
            Client = client;
            _bytesCore = new BytesCore(length);
        }

        /// <summary>
        /// 资源对象
        /// </summary>
        /// <param name="key">IP端口</param>
        /// <param name="client">连接对象</param>
        /// <param name="dataOwner">可回收数据对象</param>
        /// <param name="length">包含长度</param>
        /// <param name="onlydata">数据包完整</param>
        public ReceiveBytes(in UserKey key, ISocket client, IMemoryOwner<byte> dataOwner, int length, bool onlydata)
        {
            Key = key;
            OnlyData = onlydata;
            Client = client;
            _bytesCore = new BytesCore(dataOwner, length);
        }

        /// <summary>
        /// 资源对象
        /// </summary>
        /// <param name="key">IP端口</param>
        /// <param name="client">连接对象</param>
        /// <param name="bytesCore">可回收数据对象</param>
        /// <param name="onlydata">数据包完整</param>
        public ReceiveBytes(in UserKey key, ISocket client, in BytesCore bytesCore, bool onlydata)
        {
            Key = key;
            OnlyData = onlydata;
            Client = client;
            _bytesCore = bytesCore;
        }

        /// <summary>
        /// 表示是否需要验证数据包
        /// </summary>
        public bool OnlyData { get; }

        /// <summary>
        /// 身份标识或IP端口
        /// </summary>
        public UserKey Key { get; }

        /// <summary>
        /// 连接对象
        /// </summary>
        public ISocket Client { get; }

        /// <summary>
        /// 流长度
        /// </summary>
        public int Length => _bytesCore.Length - GetIsLength();

        /// <summary>
        /// 返回数据
        /// </summary>
        public Span<byte> Span => _bytesCore.Span[GetIsLength()..];

        /// <summary>
        /// 返回数据
        /// </summary>
        public Memory<byte> Memory => _bytesCore.Memory[GetIsLength()..];

        /// <summary>
        /// 获取连续内存
        /// </summary>
        public unsafe ArraySegment<byte> Array => _bytesCore.Array[GetIsLength()..];

        /// <summary>
        /// 获取在Udp模式下保证数据准确性，获取当前数据包序列位
        /// </summary>
        public ushort UdpOrderCount
        {
            get
            {
                if (Client is UdpEndPoint && OnlyData)
                {
                    StateObject.GetDataHeadUdp(_bytesCore.Span, out ushort orderCount);
                    return orderCount;
                }
                throw new InvalidOperationException("当前模式下不支持！");
            }
        }

        private readonly int GetIsLength() => OnlyData ? 6 : 0;

        /// <summary>
        /// 写入有效的接收数据包
        /// </summary>
        /// <returns></returns>
        public void SetMemory(in Memory<byte> memory) => _bytesCore.SetMemory(in memory);

        /// <summary>
        /// 写入有效的发送数据包
        /// </summary>
        /// <returns></returns>
        public void SetMemory(in Span<byte> span) => _bytesCore.SetMemory(in span);

        /// <summary>
        /// 写入有效的发送数据包
        /// </summary>
        /// <returns></returns>
        public void SetMemory(in ReadOnlySequence<byte> memories)
        {
            int position = 0;
            var memory = _bytesCore.Memory;
            foreach (var current in memories)
            {
                current.CopyTo(memory[position..]);
                position += current.Length;
            }
        }

        /// <summary>
        /// 移交内存器
        /// </summary>
        /// <returns></returns>
        public BytesCore TransferByte() => _bytesCore.TransferByte();

        /// <summary>
        /// 获取内存器核心
        /// </summary>
        /// <returns></returns>
        public IMemoryOwner<byte> GetIMemoryOwner() => _bytesCore.GetIMemoryOwner();

        /// <summary>
        /// 使用完后及时回收
        /// </summary>
        public void Dispose() => _bytesCore.Dispose();

        /// <summary>
        /// 获取是否被回收
        /// </summary>
        public readonly bool IsDispose => _bytesCore.IsDispose;

        /// <summary>
        /// 文本信息
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"Key:{Key} Count:{Length}";
    }

}
