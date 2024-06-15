using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using Tool.Sockets.Kernels.Struct;

namespace Tool.Sockets.Kernels
{
    /**
    * 数据接收对象
    */
    internal readonly struct ReceiveDataPacket : IDataPacket
    {
        const int HeadSize = StateObject.HeadSize;
        private static readonly Range RangeEmpty = new(0, 0);

        internal readonly Range GuidRange, TextRange, ByteRange, IpPortRange;
        internal readonly IBytesCore BytesCore;

        internal readonly Memory<byte> MemoryByte => BytesCore.Memory;

        internal readonly Span<byte> SpanByte => MemoryByte.Span;

        public ReceiveDataPacket(in IBytesCore bytesCore)
        {
            BufferSize = bytesCore.Length;

            if (BufferSize < IDataPacket.BasicSize) throw new Exception("数据协议异常");

            this.BytesCore = bytesCore;

            var memory = BytesCore.Memory;
            var bytes = memory.Span;

            if (bytes[0] != 123) throw new Exception("数据协议异常");
            //int i = 1;
            //OnlyId = new Guid(bytes.Slice(i, 16));
            GuidRange = new Range(1, 17);
            //ClassID = bytes[17];
            //ActionID = bytes[18];
            //Many = new Range(bytes[19], ^bytes[20]);
            byte is_1 = bytes[19];

            this.IsSend = IDataPacket.GetBitIs(is_1, 1);
            this.IsErr = IDataPacket.GetBitIs(is_1, 2);
            this.IsServer = IDataPacket.GetBitIs(is_1, 3);

            this.IsReply = IDataPacket.GetBitIs(is_1, 4);
            bool IsIpPort = IDataPacket.GetBitIs(is_1, 5);
            bool IsText = IDataPacket.GetBitIs(is_1, 6);
            bool IsBytes = IDataPacket.GetBitIs(is_1, 7);

            int i = 20;
            IpPortRange = IsIpPort ? new Range(i, i += HeadSize) : RangeEmpty;

            if (IsText && IsBytes)
            {
                int length = StateObject.GetDataHeadTcp(bytes[i..(i += HeadSize)]);

                TextRange = new Range(i, i += length);
                ByteRange = new Range(i, BufferSize);
            }
            else
            {
                TextRange = IsText ? new(i, BufferSize) : RangeEmpty;
                ByteRange = IsBytes ? new(i, BufferSize) : RangeEmpty;
            }
        }

        /**
         * 当前规定大小
        */
        public int BufferSize { get; }

        /// <summary>
        /// 文本数据
        /// </summary>
        public string Text
        {
            get => TextBytes.IsEmpty ? null : Encoding.UTF8.GetString(TextBytes);
        }

        /**
         * 获取对应消息Key
        */
        public ushort ActionKey => BitConverter.ToUInt16(SpanByte[17..19]);

        /**
        * 唯一ID流
        */
        public Guid OnlyId => new(SpanByte[GuidRange]); //{ get; }//byte[]

        /**
        * 通道ID
        */
        public byte ClassID => SpanByte[17]; //{ get; }

        /**
         * 事件ID
         */
        public byte ActionID => SpanByte[18]; //{ get; }

        /**
         * 当前包是发包还是回复
         */
        public bool IsSend { get; }

        /**
         * 当前包是否发生异常
         */
        public bool IsErr { get; }

        /**
         * 消息是发送给那一端
         */
        public bool IsServer { get; }

        /**
         * 是否需要有回复消息
         */
        public bool IsReply { get; }

        /**
         * 文本流数据包
         */
        public Span<byte> TextBytes => SpanByte[TextRange]; //{ get; }

        /**
         * 携带数据包
         */
        public Memory<byte> Bytes => MemoryByte[ByteRange]; //{ get; }

        /**
         * 当为转发时，转发给谁的IpPort
        */
        public Ipv4Port IpPort => new(MemoryByte[IpPortRange]);
        //{
        //    get
        //    {
        //        if (IpPortRange.Equals(RangeEmpty)) return null;
        //        var ipbytes = SpanByte[IpPortRange];
        //        return $"{ipbytes[0]}.{ipbytes[1]}.{ipbytes[2]}.{ipbytes[3]}:{BitConverter.ToUInt16(ipbytes[4..6])}";
        //    }
        //}

        public IDataPacket CopyTo(bool isbytes, bool istxet)
        {
            SendDataPacket dataPacket = new(ClassID, ActionID, OnlyId)
            {
                IsSend = IsSend,
                IsErr = IsErr,
                IsServer = IsServer,
                IsReply = IsReply,
                IpPort = IpPort,
                BufferSize = BufferSize,
            };

            if (isbytes) dataPacket.SetBuffer(Bytes.ToArray());
            if (istxet) dataPacket.Text = Text;

            return dataPacket;
        }

        public IDataPacket Clone()
        {
            return new IpIdeaDataPacket(in this);
        }

        public void Dispose()
        {
            BytesCore.Dispose();
        }
    }
}
