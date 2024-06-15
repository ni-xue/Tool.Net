using System.Buffers;
using System.Net.Sockets;
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    ///  资源 对象（必须回收，丢失风险大）
    /// </summary>
    public struct BytesCore : IBytesCore
    {
        private readonly IMemoryOwner<byte> _dataOwner;

        private bool _dispose { get; set; }

        /// <summary>
        /// 流长度
        /// </summary>
        public int Length { get; }

        /// <summary>
        /// 创建一个内存资源对象
        /// </summary>
        /// <param name="length">内存大小</param>
        public BytesCore(int length)
        {
            _dispose = false;
            _dataOwner = MemoryPool<byte>.Shared.Rent(length);
            Length = length;
        }

        /// <summary>
        /// 资源对象
        /// </summary>
        /// <param name="dataOwner">可回收数据对象</param>
        /// <param name="length">包含长度</param>
        public BytesCore(IMemoryOwner<byte> dataOwner, int length)
        {
            _dispose = false;
            _dataOwner = dataOwner;
            Length = length;
        }

        /// <summary>
        /// 返回数据
        /// </summary>
        public readonly Span<byte> Span => Memory.Span;

        /// <summary>
        /// 返回数据
        /// </summary>
        public readonly Memory<byte> Memory => GetMemory();

        /// <summary>
        /// 获取连续内存
        /// </summary>
        public readonly ArraySegment<byte> Array => Memory.AsArraySegment();

        private readonly Memory<byte> GetMemory() 
        {
            if (_dataOwner is null) throw new NullReferenceException("当前值类型不可用！");
            return _dataOwner.Memory[..Length];
        }

        /// <summary>
        /// 移交内存器
        /// </summary>
        /// <returns></returns>
        public readonly BytesCore TransferByte() => this;

        /// <summary>
        /// 获取内存器核心
        /// </summary>
        /// <returns></returns>
        public readonly IMemoryOwner<byte> GetIMemoryOwner() => _dataOwner;

        /// <summary>
        /// 写入有效的接收数据包
        /// </summary>
        /// <returns></returns>
        public readonly void SetMemory(in Memory<byte> memory)
        {
            memory.CopyTo(Memory);
        }

        /// <summary>
        /// 写入有效的接收数据包
        /// </summary>
        /// <returns></returns>
        public readonly void SetMemory(in Span<byte> span)
        {
            span.CopyTo(Span);
        }

        //public void ResetMemory(int length, bool isCopy) 
        //{
        //    _dataOwner = MemoryPool<byte>.Shared.Rent(length);
        //    Length = length;
        //}

        /// <summary>
        /// 使用完后及时回收
        /// </summary>
        public void Dispose()
        {
            _dispose = true;
            _dataOwner?.Dispose();
        }

        /// <summary>
        /// 获取是否被回收
        /// </summary>
        public readonly bool IsDispose => _dataOwner is null || _dispose;

        /// <summary>
        /// 是否为空对象
        /// </summary>
        public readonly bool IsEmpty => IsDispose;

        /// <summary>
        /// 获取空对象
        /// </summary>
        public static readonly BytesCore Empty = new();
    }
}
