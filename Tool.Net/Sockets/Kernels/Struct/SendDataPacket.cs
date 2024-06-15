using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Reflection;
using System.Text;

namespace Tool.Sockets.Kernels
{
    /**
    * 数据接收对象
    */
    internal struct SendDataPacket : IDataPacket
    {
        const int HeadSize = StateObject.HeadSize;

        /// <summary>
        /// 获取对应消息Key
        /// </summary>
        public readonly ushort ActionKey => BitConverter.ToUInt16(new byte[] { ClassID, ActionID }); //string.Concat(ClassID, '.', ActionID);

        /// <summary>
        /// 文本数据
        /// </summary>
        public string Text
        {
            readonly get => _text;
            set => _text = value;// ArraySegment<byte>.Empty 
        }

        //ReadOnlySequence
        
        public SendDataPacket(byte ClassID, byte ActionID, in Guid OnlyId)
        {
            this.ClassID = ClassID;
            this.ActionID = ActionID;
            this.OnlyId = OnlyId;

            this._bytes = ArraySegment<byte>.Empty;
            this._text = null;

            this.IsSend = false;
            this.IsErr = false;
            this.IsServer = false;
            this.IsReply = false;
            this.IpPort = Ipv4Port.Empty;
            this.BufferSize = 0;

            _disposed = null;
        }

        /**
         * 当前规定大小
        */
        public int BufferSize { get; set; }

        /**
        * 通道ID
        */
        public byte ClassID { get; }

        /**
         * 事件ID
         */
        public byte ActionID { get; }

        /**
         * 唯一ID流
         */
        public Guid OnlyId { get; }//byte[]

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
        public readonly Span<byte> TextBytes => throw new NotImplementedException("发送数据包不支持");

        /**
         * 携带数据包
         */
        public readonly Memory<byte> Bytes => _bytes;

        private ArraySegment<byte> _bytes { get; set; }

        private string _text { get; set; }

        /**
         * 当为转发时，转发给谁的IpPort
        */
        public Ipv4Port IpPort { get; set; }

        public void SetBuffer(in ArraySegment<byte> Bytes)
        {
            this._bytes = Bytes;
        }

        readonly bool IsIpIdea => !IpPort.IsEmpty;
        readonly bool IsText => !string.IsNullOrEmpty(_text);
        readonly bool IsBytes => !Bytes.IsEmpty;

        readonly int IDataPacket.TotalSize(out int textSize)
        {
            textSize = 0;
            int TotalLength = IDataPacket.BasicSize + (IsIpIdea ? 6 : 0);

            if (IsText && IsBytes) TotalLength += HeadSize;
            if (IsText) TotalLength += textSize = Encoding.UTF8.GetByteCount(_text);
            if (IsBytes) TotalLength += _bytes.Count;

            return TotalLength;
        }

        void IDataPacket.ByteData<T>(in SendBytes<T> sendBytes, int textSize)
        {
            _disposed = sendBytes;

            var span = sendBytes.Span;
            BufferSize = span.Length;
            span[0] = 123;
            OnlyId.TryWriteBytes(span[1..17]);
            span[17] = ClassID;
            span[18] = ActionID;

            byte is_1 = 0;
            IDataPacket.SetBit(ref is_1, 1, IsSend);
            IDataPacket.SetBit(ref is_1, 2, IsErr);
            IDataPacket.SetBit(ref is_1, 3, IsServer);

            IDataPacket.SetBit(ref is_1, 4, IsReply);
            IDataPacket.SetBit(ref is_1, 5, IsIpIdea);
            IDataPacket.SetBit(ref is_1, 6, IsText);
            IDataPacket.SetBit(ref is_1, 7, IsBytes);
            int index = 19;
            span[index++] = is_1;

            if (IsIpIdea)
            {
                StateObject.SetIpPort(span[index..(index += HeadSize)], IpPort);
            }
            if (IsText & IsBytes)
            {
                StateObject.SetDataHeadTcp(span, textSize, index);
                index += HeadSize;
            }
            if (IsText) Encoding.UTF8.GetBytes(_text.AsSpan(), span[index..(index += textSize)]);
            if (IsBytes) Bytes.Span.CopyTo(span[index..]);
        }

        public void SetErr(string ex)
        {
            this._bytes = ArraySegment<byte>.Empty;
            this._text = ex;
            this.IsErr = true;
        }

        public void ResetValue(bool? IsSend = null, bool? IsServer = null, Ipv4Port? IpPort = null)
        {
            if (IsSend.HasValue) this.IsSend = IsSend.Value;
            if (IsServer.HasValue) this.IsServer = IsServer.Value;
            if (IpPort.HasValue) this.IpPort = IpPort.Value;
        }

        IDisposable _disposed { get; set; }

        public readonly void Dispose()
        {
            _disposed?.Dispose();
        }
    }
}
