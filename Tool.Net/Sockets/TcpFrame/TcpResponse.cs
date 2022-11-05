using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using Tool.Sockets.SupportCode;
using Tool.Utils;

namespace Tool.Sockets.TcpFrame
{
    /// <summary>
    /// 请求服务器返回的数据包信息类
    /// </summary>
    public class TcpResponse
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public TcpResponse(string onlyID)
        {
            OnlyID = onlyID;
        }

        /// <summary>
        /// 消息唯一ID
        /// </summary>
        public string OnlyID { get; }

        /// <summary>
        /// 用于表示当前数据包的执行情况
        /// </summary>
        public TcpFrameState OnTcpFrame { get; internal set; } = TcpFrameState.Default;

        /// <summary>
        /// 是否是异步
        /// </summary>
        public bool IsAsync { get; internal set; }

        /// <summary>
        /// 数据包（文字类型的数据）
        /// </summary>
        public string Text { get; internal set; }

        /// <summary>
        /// 当前消息携带的数据流
        /// </summary>
        public ArraySegment<byte> Bytes { get; internal set; }

        /// <summary>
        /// 当前发生的异常
        /// </summary>
        public Exception Exception { get; internal set; }

        ///// <summary>
        ///// 回收资源
        ///// </summary>
        //public void Dispose()
        //{
        //    Text = null;
        //    Bytes = null;
        //    Exception = null;
        //    GC.SuppressFinalize(this);
        //}

        internal void Complete(ref DataPacket packet)
        {
            OnTcpFrame = packet.IsErr ? TcpFrameState.Exception : TcpFrameState.Success;
            if (!packet.IsErr)
            {
                Text = packet.Text;
                Bytes = packet.Bytes; // packet.Bytes.Count is 0 ? null : packet.Bytes.ToArray();
            }
            else
            {
                Exception = new Exception(packet.Text);
            }
            IsAsync = packet.IsAsync;//是异步的？
        }
    }
}
