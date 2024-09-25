using System.Text;
using System;

namespace Tool.Sockets.Kernels.Struct
{
    /**
    * 数据转发对象
    */
    internal struct IpIdeaDataPacket : IDataPacket
    {
        const int HeadSize = StateObject.HeadSize;

        private readonly ReceiveDataPacket packet;
        private readonly IBytesCore BytesCore => packet.BytesCore;

        private readonly Memory<byte> MemoryByte => BytesCore.GetIMemoryOwner().Memory[(BytesCore.OnlyData ? 6 : 0)..]; //因存在会报错的特殊情况，导致需要获取而外的内存故从核心区外拿数据

        private readonly Span<byte> SpanByte => MemoryByte.Span;

        public IpIdeaDataPacket(in ReceiveDataPacket packet)
        {
            this.packet = packet;

            this.IsSend = packet.IsSend;
            this.IsErr = packet.IsErr;
            this.IsServer = packet.IsServer;

            this.IsReply = packet.IsReply;
            this.IpPort = packet.IpPort;
            this.BufferSize = packet.BufferSize;
        }

        /**
         * 当前规定大小
        */
        public int BufferSize { get; set; }

        /// <summary>
        /// 文本数据
        /// </summary>
        public readonly string Text => packet.Text;

        /**
         * 获取对应消息Key
        */
        public readonly ushort ActionKey => packet.ActionKey;

        /**
        * 唯一ID流
        */
        public readonly Guid OnlyId => packet.OnlyId;

        /**
        * 通道ID
        */
        public readonly byte ClassID => packet.ClassID;

        /**
         * 事件ID
         */
        public readonly byte ActionID => packet.ActionID;

        /**
         * 当前包是发包还是回复
         */
        public bool IsSend { get; set; }

        /**
         * 当前包是否发生异常
         */
        public bool IsErr { get; set; }

        /**
         * 消息是发送给那一端
         */
        public bool IsServer { get; set; }

        /**
         * 是否需要有回复消息
         */
        public bool IsReply { get; set; }

        /**
         * 文本流数据包
         */
        public readonly Span<byte> TextBytes => packet.TextBytes; //{ get; }

        /**
         * 携带数据包
         */
        public readonly Memory<byte> Bytes => packet.Bytes; //{ get; }

        /**
         * 当为转发时，转发给谁的IpPort
        */
        public Ipv4Port IpPort { get; set; }

        private static int GettopLength() => IDataPacket.BasicSize + 6;

        public void SetErr(string ex)
        {
            int topLength = GettopLength(), bodyLength = MemoryByte.Length - topLength;
            bool islog = true;
            if (bodyLength > 0)
            {
                var textspan = ex.AsSpan();
                int textSize = Encoding.UTF8.GetByteCount(textspan);
                if (textSize <= bodyLength)
                {
                    Encoding.UTF8.GetBytes(textspan, SpanByte[topLength..]);
                    BufferSize = topLength + textSize;
                    islog = false;
                }
                else
                {
                    BufferSize = topLength;
                }
            }
            if (islog)
            {
                Utils.Log.Error($"服务器转发消息异常：{ex}", "Log/NetFrame/Agent");
            }
            this.IsErr = true;
        }

        public readonly SendBytes<T> GetAgentBytes<T>(T client)
        {
            if (IsSend != packet.IsSend || IsServer != packet.IsServer || IsErr != packet.IsErr)
            {
                ref byte is_1 = ref SpanByte[19];
                IDataPacket.SetBit(ref is_1, 1, IsSend);
                IDataPacket.SetBit(ref is_1, 3, IsServer);
                if (IsErr)
                {
                    IDataPacket.SetBit(ref is_1, 2, IsErr);
                    IDataPacket.SetBit(ref is_1, 6, BufferSize != GettopLength());
                    IDataPacket.SetBit(ref is_1, 7, false);
                }
                //SpanByte[19] = is_1;
            }
            int index = 20;
            if (IpPort != packet.IpPort)
            {
                StateObject.SetIpPort(SpanByte[index..(index += HeadSize)], IpPort);
            }
            return new SendBytes<T>(client, BytesCore.GetIMemoryOwner(), BufferSize, BytesCore.OnlyData);
        }

        public void ResetValue(bool? IsSend = null, bool? IsServer = null, Ipv4Port? IpPort = null)
        {
            if (IsSend.HasValue) this.IsSend = IsSend.Value;
            if (IsServer.HasValue) this.IsServer = IsServer.Value;
            if (IpPort.HasValue) this.IpPort = IpPort.Value;
        }

        public readonly void Dispose()
        {
            BytesCore.Dispose();
        }
    }
}
