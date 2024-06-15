using System;
using System.Buffers.Binary;
using System.Buffers;
using System.Text;
using System.Reflection;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 通讯协议模型
    /// </summary>
    public interface IDataPacket : IDisposable
    {
        #region 内置实现区

        internal static int BasicSize => 20;

        /// <summary>
        /// 是否转发数据，默认不转发
        /// </summary>
        public bool IsIpIdea => !IpPort.IsEmpty; //{ get; set; }

        #endregion

        #region 待实现区

        /**
        * 当前规定大小
        */
        public int BufferSize { get; }

        /// <summary>
        /// 获取对应消息Key
        /// </summary>
        public ushort ActionKey { get; }

        /// <summary>
        /// 唯一ID
        /// </summary>
        public Guid OnlyId { get; }//byte[]

        /// <summary>
        /// 通道ID
        /// </summary>
        public byte ClassID { get; }

        /// <summary>
        /// 事件ID
        /// </summary>
        public byte ActionID { get; }

        /// <summary>
        /// 当前包是发包还是回复
        /// </summary>
        public bool IsSend { get; }

        /// <summary>
        /// 当前包是否发生异常
        /// </summary>
        public bool IsErr { get; }

        /// <summary>
        /// 消息是发送给那一端
        /// </summary>
        public bool IsServer { get; }

        /// <summary>
        /// 是否需要有回复消息
        /// </summary>
        public bool IsReply { get; }

        /// <summary>
        /// 文本数据
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 文本流数据包
        /// </summary>
        public Span<byte> TextBytes { get; }

        /// <summary>
        /// 携带字节包
        /// </summary>
        public Memory<byte> Bytes { get; }

        /// <summary>
        /// 当为转发时，转发给谁的IpPort
        /// </summary>
        public Ipv4Port IpPort { get; }

        /// <summary>
        /// 获取包总大小
        /// </summary>
        /// <returns></returns>
        public int TotalSize(out int textSize) => throw new NotImplementedException();

        /// <summary>
        /// 获取完整字节流
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sendBytes"></param>
        /// <param name="textSize"></param>
        public void ByteData<T>(in SendBytes<T> sendBytes, int textSize) => throw new NotImplementedException();

        /// <summary>
        /// 拷贝当前数据
        /// </summary>
        /// <returns></returns>
        public IDataPacket CopyTo(bool isbytes, bool istxet) => throw new NotImplementedException();

        /// <summary>
        /// 克隆完整副本
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public IDataPacket Clone() => throw new NotImplementedException();

        /// <summary>
        /// 设置错误信息
        /// </summary>
        /// <param name="ex"></param>
        public void SetErr(string ex) => throw new NotImplementedException();

        /// <summary>
        /// 设置发送状态
        /// </summary>
        public void ResetValue(bool? IsSend = null, bool? IsServer = null, Ipv4Port? IpPort = null) => throw new NotImplementedException();

        /// <summary>
        /// 获取转发模式下专用数据对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="client"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public SendBytes<T> GetAgentBytes<T>(T client) => throw new NotImplementedException();

        #endregion

        #region 内置可用函数

        internal static void IsByteState(byte state, out bool age0, out bool age1, out bool age2)
        {
            //方案一
            //if (state > 99)
            //{
            //    state -= 100;
            //    age0 = true;
            //}
            //else
            //{
            //    age0 = false;
            //}
            //if (state > 9)
            //{
            //    state -= 10;
            //    age1 = true;
            //}
            //else
            //{
            //    age1 = false;
            //}
            //if (state > 0)
            //{
            //    age2 = true;
            //}
            //else
            //{
            //    age2 = false;
            //}

            //方案二
            switch (state)
            {
                case 000:
                    age0 = false;
                    age1 = false;
                    age2 = false;
                    break;
                case 001:
                    age0 = false;
                    age1 = false;
                    age2 = true;
                    break;
                case 010:
                    age0 = false;
                    age1 = true;
                    age2 = false;
                    break;
                case 011:
                    age0 = false;
                    age1 = true;
                    age2 = true;
                    break;

                case 100:
                    age0 = true;
                    age1 = false;
                    age2 = false;
                    break;
                case 101:
                    age0 = true;
                    age1 = false;
                    age2 = true;
                    break;
                case 110:
                    age0 = true;
                    age1 = true;
                    age2 = false;
                    break;
                case 111:
                    age0 = true;
                    age1 = true;
                    age2 = true;
                    break;
                default:
                    throw new NotImplementedException("发生意料之外的错误！");
            }
        }

        internal static byte ToByteState(bool age0, bool age1, bool age2)
        {
            //方案一
            //byte a = 0, b = 1, c = 10, d = 100;
            //return (byte)((age0 ? d : a) + (age1 ? c : a) + (age2 ? b : a));

            //方案二
            if (age0 & age1 & age2) return 111;
            else if (age0 & age1) return 110;
            else if (age0 & age2) return 101;
            else if (age0) return 100;

            else if (age1 & age2) return 011;
            else if (age1) return 010;
            else if (age2) return 001;
            else return 000;
        }

        internal static byte SetBit(byte data, int index, bool flag)
        {
            SetBit(ref data, index, flag);
            return data;
        }

        internal static void SetBit(ref byte data, int index, bool flag)
        {
            int v = TryIndex(index);
            data = flag ? (byte)(data | v) : (byte)(data & ~v);
        }

        internal static int GetBit(byte data, int index)
        {
            return GetBitIs(data, index) ? 1 : 0;
        }

        internal static bool GetBitIs(byte data, int index)
        {
            int v = TryIndex(index);
            return (data & v) == v;
        }

        private static int TryIndex(int index) 
        {
            if (index > 8 || index < 1) throw new ArgumentOutOfRangeException($"设置{nameof(index)}位置溢出");
            return index < 2 ? index : (2 << (index - 2));
        }

        //static bool TryReadInt32(ref ReadOnlySequence<byte> buffer, out int? value)
        //{
        //    if (buffer.Length < 4) { value = null; return false; }
        //    var slice = buffer.Slice(buffer.Start, 4);
        //    if (slice.IsSingleSegment)
        //    {
        //        value = BinaryPrimitives.ReadInt32BigEndian(slice.FirstSpan);
        //    }
        //    else
        //    {
        //        Span<byte> bytes = stackalloc byte[4];
        //        slice.CopyTo(bytes);
        //        value = BinaryPrimitives.ReadInt32BigEndian(bytes);
        //    }
        //    buffer = buffer.Slice(slice.End); return true;
        //}

        //static bool TryReadInt32(ref ReadOnlySequence<byte> buffer, out int? value)
        //{
        //    var reader = new SequenceReader<byte>(buffer);
        //    if (reader.TryReadBigEndian(out int v))
        //    {
        //        value = v; buffer = buffer.Slice(4); return true;
        //    }
        //    value = null; return false;
        //}

        #endregion
    }
}
