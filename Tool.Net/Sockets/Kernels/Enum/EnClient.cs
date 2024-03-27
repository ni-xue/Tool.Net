using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// ClientFrame类的行为
    /// </summary>
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
        SendMsg = 3,
        /// <summary>
        /// 收到服务器数据时发生
        /// </summary>
        Receive = 4,
        /// <summary>
        /// 与服务器断开连接时发生
        /// </summary>
        Close = 5,
        /// <summary>
        /// 心跳包事件（推送后触发）
        /// </summary>
        HeartBeat = 10
    }
}
