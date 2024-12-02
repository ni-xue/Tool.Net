using System;

namespace Tool.Sockets.NetFrame
{
    /// <summary>
    /// 通信状态
    /// </summary>
    [Flags]
    public enum ProtocolStatus : byte
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unknown = 0,
        /// <summary>
        /// 已连接
        /// </summary>
        Connect = 1,
        /// <summary>
        /// 连接失败
        /// </summary>
        Fail = 2,
        /// <summary>
        /// 已断开
        /// </summary>
        Close = 4,
        /// <summary>
        /// 重连中
        /// </summary>
        Reconnect = 8,
    }
}
