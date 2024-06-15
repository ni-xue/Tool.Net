using System;
using System.Buffers;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 内存管理对象
    /// </summary>
    public interface IBytesCore : IDisposable
    {
        /// <summary>
        /// 仅标记作用
        /// </summary>
        public bool OnlyData => throw new NotImplementedException();

        /// <summary>
        /// 流长度
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public Span<byte> Span { get; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public Memory<byte> Memory { get; }

        /// <summary>
        /// 获取连续内存
        /// </summary>
        public ArraySegment<byte> Array { get; }

        /// <summary>
        /// 移交内存器
        /// </summary>
        /// <returns></returns>
        public BytesCore TransferByte();

        /// <summary>
        /// 获取内存器核心
        /// </summary>
        /// <returns></returns>
        public IMemoryOwner<byte> GetIMemoryOwner();

        /// <summary>
        /// 获取是否被回收
        /// </summary>
        public bool IsDispose { get; }
    }
}
