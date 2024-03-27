using System.Buffers;
using System.Net.Sockets;
using System;
using System.Net.WebSockets;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// Socket 通讯资源 对象（必须回收，丢失风险大）
    /// </summary>
    public readonly struct ReceiveBytes<ISocket> : IDisposable
    {
        private readonly BytesCore _bytesCore;

        /// <summary>
        /// 资源对象
        /// </summary>
        /// <param name="key">IP端口</param>
        /// <param name="client">连接对象</param>
        /// <param name="length">包含长度</param>
        internal ReceiveBytes(string key, ISocket client, int length) : this()
        {
            Key = key;
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
        public ReceiveBytes(string key, ISocket client, IMemoryOwner<byte> dataOwner, int length) : this()
        {
            Key = key;
            Client = client;
            _bytesCore = new BytesCore(dataOwner, length);
        }

        /// <summary>
        /// 身份标识或IP端口
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 连接对象
        /// </summary>
        public ISocket Client { get; }

        /// <summary>
        /// 流长度
        /// </summary>
        public int Length => _bytesCore.Length;

        /// <summary>
        /// 返回数据
        /// </summary>
        public Memory<byte> Memory => _bytesCore.Memory;

        /// <summary>
        /// 使用完后及时回收
        /// </summary>
        public void Dispose() => _bytesCore.Dispose();
    }

}
