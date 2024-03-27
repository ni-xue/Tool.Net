using System.Buffers;
using System;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// Socket 通讯资源 对象（必须回收，丢失风险大）
    /// </summary>
    public readonly struct SendBytes : IDisposable
    {
        private readonly BytesCore _bytesCore;

        /// <summary>
        /// 资源对象
        /// </summary>
        /// <param name="dataOwner">可回收数据对象</param>
        /// <param name="length">包含长度</param>
        public SendBytes(IMemoryOwner<byte> dataOwner, int length) : this()
        {
            _bytesCore = new BytesCore(dataOwner, length);
        }

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
