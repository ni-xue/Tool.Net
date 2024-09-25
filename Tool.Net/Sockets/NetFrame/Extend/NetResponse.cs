using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.NetFrame
{
    /// <summary>
    /// 请求服务器返回的数据包信息类（请务必回收资源！！！）
    /// </summary>
    public readonly struct NetResponse: IDisposable
    {
        readonly IDisposable Packet;

        /// <summary>
        /// 未完成时
        /// </summary>
        public NetResponse(in Guid onlyId, bool isreply, NetFrameState state, Exception error)
        {
            OnlyId = onlyId;
            State = state;
            Error = error;
            IsReply = isreply;

            Text = null;
            Bytes = null;
            Packet = null;
        }

        /// <summary>
        /// 完成后
        /// </summary>
        public NetResponse(in IDataPacket packet)
        {
            Packet = packet;
            OnlyId = packet.OnlyId; 
            IsReply = packet.IsReply;
            if (packet.IsErr)
            {
                State = NetFrameState.Exception;
                Error = new Exception(packet.Text ?? "相关错误信息未能传播，请查看相关通信服务器日志。");
                Text = null;
                Bytes = null;
            }
            else
            {
                State = NetFrameState.Success;
                Error = null;
                Text = packet.Text;
                Bytes = packet.Bytes; // packet.Bytes.Count is 0 ? null : packet.Bytes.ToArray();
            }
        }

        /// <summary>
        /// 消息唯一ID
        /// </summary>
        public Guid OnlyId { get; }

        ///// <summary>
        ///// 消息唯一ID
        ///// </summary>
        //public string OnlyID => OnlyId.ToString();

        /// <summary>
        /// 用于表示当前数据包的执行情况
        /// </summary>
        public NetFrameState State { get; }

        /// <summary>
        /// 是否需要有回复消息
        /// </summary>
        public bool IsReply { get; }

        /// <summary>
        /// 数据包（文字类型的数据）
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 当前消息携带的数据流
        /// </summary>
        public Memory<byte> Bytes { get; }

        /// <summary>
        /// 当前消息携带的数据流
        /// </summary>
        public Span<byte> Span => Bytes.Span;

        /// <summary>
        /// 当前消息携带的数据流
        /// </summary>
        public ArraySegment<byte> Array => Bytes.AsArraySegment();

        /// <summary>
        /// 当前发生的异常
        /// </summary>
        public Exception Error { get; }

        /// <summary>
        /// 回收资源
        /// </summary>
        public void Dispose()
        {
            Packet?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
