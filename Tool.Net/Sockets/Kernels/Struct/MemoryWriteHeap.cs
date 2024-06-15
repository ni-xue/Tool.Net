using System.Buffers;
using System;
using System.Net.Sockets;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 用于处理超过缓冲区大小的包体
    /// </summary>
    public struct MemoryWriteHeap : IBytesCore
    {
        private BytesCore _bytesCore;

        ///// <summary>
        ///// 无效构造
        ///// </summary>
        //public MemoryWriteHeap()
        //{
        //    WriteIndex = -1;
        //    SpareSize = -1;
        //    _bytesCore = default;
        //}

        /// <summary>
        /// 创建一个内存资源对象
        /// </summary>
        /// <param name="length">内存大小</param>
        /// <param name="memory">缓冲内存移动</param>
        public MemoryWriteHeap(int length, Memory<byte> memory)
        {
            _bytesCore = new BytesCore(length);
            memory.CopyTo(_bytesCore.Memory);
            WriteIndex = memory.Length;
            SpareSize = length - memory.Length;
        }

        /// <summary>
        /// 写入索引
        /// </summary>
        public int WriteIndex { get; private set; }

        /// <summary>
        /// 剩余大小
        /// </summary>
        public int SpareSize { get; private set; }

        /// <summary>
        /// 表示是否完全读取完成
        /// </summary>
        public readonly bool IsSuccess => Length > 0 && WriteIndex == Length && SpareSize == 0;

        /// <summary>
        /// 流长度
        /// </summary>
        public readonly int Length => _bytesCore.Length;

        /// <summary>
        /// 返回数据
        /// </summary>
        public readonly Span<byte> Span => Memory.Span;

        /// <summary>
        /// 返回数据
        /// </summary>
        public readonly Memory<byte> Memory => _bytesCore.Memory;

        /// <summary>
        /// 返回数据
        /// </summary>
        public readonly ArraySegment<byte> Array => _bytesCore.Array;

        /// <summary>
        /// 一个连续的空内存
        /// </summary>
        public readonly Memory<byte> EmptyData => Memory.Slice(WriteIndex, SpareSize);

        /// <summary>
        /// 移交内存器
        /// </summary>
        /// <returns></returns>
        public readonly BytesCore TransferByte() => _bytesCore.TransferByte();

        /// <summary>
        /// 获取内存器核心
        /// </summary>
        /// <returns></returns>
        public readonly IMemoryOwner<byte> GetIMemoryOwner() => _bytesCore.GetIMemoryOwner();

        /// <summary>
        /// 写入有效的接收数据包长度
        /// </summary>
        /// <returns></returns>
        public void SetCount(int count)
        {
            if (count > SpareSize)
            {
                count = SpareSize;
            }
            SpareSize -= count;
            WriteIndex += count;
        }

        ///// <summary>
        ///// 写入有效的接收数据包
        ///// </summary>
        ///// <returns></returns>
        //public int SetMemory(Memory<byte> memory, int start, int end)
        //{
        //    int ru = 0;
        //    if (end > SpareSize)
        //    {
        //        ru = end - SpareSize;
        //        end = SpareSize;
        //    }
        //    SpareSize -= end;
        //    memory[start..end].CopyTo(Memory[WriteIndex..]);
        //    WriteIndex += end;

        //    return ru;
        //}

        /// <summary>
        /// 获取完整包返回
        /// </summary>
        /// <typeparam name="T">连接对象</typeparam>
        /// <param name="IpPort">Key</param>
        /// <param name="Client">连接对象</param>
        /// <returns></returns>
        public ReceiveBytes<T> GetReceiveBytes<T>(in UserKey IpPort, T Client) 
        {
            Empty();
            return new ReceiveBytes<T>(in IpPort, Client, TransferByte(), true);
        }

        /// <summary>
        /// 清空标记
        /// </summary>
        public void Empty() 
        {
            WriteIndex = 0;
            SpareSize = 0;
        }

        /// <summary>
        /// 获取是否被清空
        /// </summary>
        public readonly bool IsEmpty => WriteIndex == 0 && SpareSize == 0; //_bytesCore.IsDispose;

        /// <summary>
        /// 使用完后及时回收
        /// </summary>
        public void Dispose()
        {
            Empty();
            _bytesCore.Dispose();
        }

        /// <summary>
        /// 获取是否被回收
        /// </summary>
        public readonly bool IsDispose => _bytesCore.IsDispose;
    }
}
