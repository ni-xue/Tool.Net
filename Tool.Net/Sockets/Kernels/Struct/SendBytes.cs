using System.Buffers;
using System;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Tool.Utils;
using System.Reflection;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// Socket 通讯资源 对象（必须回收，丢失风险大）
    /// </summary>
    public readonly struct SendBytes<ISocket> : IBytesCore
    {
        private readonly BytesCore _bytesCore;

        /// <summary>
        /// 资源对象
        /// </summary>
        /// <param name="client">连接对象</param>
        /// <param name="length">包含长度</param>
        /// <param name="onlydata">数据包完整</param>
        public SendBytes(ISocket client, int length, bool onlydata)
        {
            int islength = onlydata ? 6 : 0;
            Client = client;
            OnlyData = onlydata;
            _bytesCore = new BytesCore(islength + length);
        }

        /// <summary>
        /// 资源对象
        /// </summary>
        /// <param name="client">连接对象</param>
        /// <param name="dataOwner">可回收数据对象</param>
        /// <param name="length">包含长度</param>
        /// <param name="onlydata">数据包完整</param>
        public SendBytes(ISocket client, IMemoryOwner<byte> dataOwner, int length, bool onlydata)
        {
            int islength = onlydata ? 6 : 0;
            Client = client;
            OnlyData = onlydata;
            _bytesCore = new BytesCore(dataOwner, islength + length);
        }

        /// <summary>
        /// 资源对象
        /// </summary>
        /// <param name="client">连接对象</param>
        /// <param name="bytesCore">可回收数据对象</param>
        /// <param name="onlydata">数据包完整</param>
        public SendBytes(ISocket client, in BytesCore bytesCore, bool onlydata)
        {
            Client = client;
            OnlyData = onlydata;
            _bytesCore = bytesCore;
        }

        /// <summary>
        /// 表示是否需要验证数据包
        /// </summary>
        public bool OnlyData { get; }

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

        private readonly int GetIsLength() => OnlyData ? 6 : 0;

        /// <summary>
        /// 获取有效的发送数据包
        /// </summary>
        /// <returns></returns>
        public Memory<byte> GetMemory()
        {
            if (OnlyData) StateObject.SetDataHeadTcp(_bytesCore.Span, Length, 0);
            return _bytesCore.Memory;
        }

        /// <summary>
        /// 获取有效的发送数据包(UDP协议版)
        /// </summary>
        /// <returns></returns>
        public Memory<byte> GetMemory(ushort orderCount)
        {
            if (OnlyData) StateObject.SetDataHeadUdp(_bytesCore.Span, orderCount, 0);
            return _bytesCore.Memory;
        }

        /// <summary>
        /// 写入有效的发送数据包
        /// </summary>
        /// <returns></returns>
        public void SetMemory(in Memory<byte> memory, int start = 0)
        {
            memory.CopyTo(start > 0 ? Memory[start..] : Memory);
        }

        /// <summary>
        /// 写入有效的发送数据包
        /// </summary>
        /// <returns></returns>
        public void SetMemory(in Span<byte> span, int start = 0)
        {
            span.CopyTo(start > 0 ? Span[start..] : Span);
        }

        /// <summary>
        /// 写入有效的发送数据包
        /// </summary>
        /// <returns></returns>
        public void SetMemory(in ArraySegment<byte> bytes, int start = 0)
        {
            bytes.AsSpan().CopyTo(start > 0 ? Span[start..] : Span);
        }

        /// <summary>
        /// 用于进行输出缩减包大小
        /// </summary>
        /// <param name="start">只能是0</param>
        /// <param name="length">小于总大小的数</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SendBytes<ISocket> Slice(int start, int length)
        {
            if (start > 0) throw new Exception("start 的值不能大于 0");
            if (length < 0 || length > Length) throw new Exception("length 溢出数组");
            return new SendBytes<ISocket>(Client, GetIMemoryOwner(), length, OnlyData);
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
        public override string ToString() => $"Key:{Client.GetType().Name} Count:{Length}";
    }
}
