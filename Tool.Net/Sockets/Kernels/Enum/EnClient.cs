using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// Client客户端枚举
    /// </summary>
    [Flags]
    public enum EnClient : byte
    {
        /// <summary>
        /// 连接服务器成功时发生
        /// </summary>
        Connect = 1,
        /// <summary>
        /// 连接服务器失败时发生
        /// </summary>
        Fail = 2,
        /// <summary>
        /// 向服务器发送数据时发生
        /// </summary>
        SendMsg = 4,
        /// <summary>
        /// 收到服务器数据时发生
        /// </summary>
        Receive = 8,
        /// <summary>
        /// 与服务器断开连接时发生
        /// </summary>
        Close = 16,
        /// <summary>
        /// 心跳包事件（推送后触发）
        /// </summary>
        HeartBeat = 32,
        /// <summary>
        /// （断线后/连接失败）需要重连时触发
        /// </summary>
        Reconnect = 64,
    }
}
