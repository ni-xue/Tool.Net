using System.Buffers;
using System.Net.Sockets;
using System;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    ///  资源 对象（必须回收，丢失风险大）
    /// </summary>
    public readonly struct BytesCore : IDisposable
    {
        private readonly IMemoryOwner<byte> _dataOwner;

        /// <summary>
        /// 流长度
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// 创建一个内存资源对象
        /// </summary>
        /// <param name="length">内存大小</param>
        public BytesCore(int length) : this()
        {
            _dataOwner = MemoryPool<byte>.Shared.Rent(length);
            Length = length;
        }

        /// <summary>
        /// 资源对象
        /// </summary>
        /// <param name="dataOwner">可回收数据对象</param>
        /// <param name="length">包含长度</param>
        public BytesCore(IMemoryOwner<byte> dataOwner, int length) : this()
        {
            _dataOwner = dataOwner;
            Length = length;
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        public Memory<byte> Memory => _dataOwner.Memory[..Length];//{ get; }

        /// <summary>
        /// 使用完后及时回收
        /// </summary>
        public void Dispose() => _dataOwner.Dispose();
    }
}
