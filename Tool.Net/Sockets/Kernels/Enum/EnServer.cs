using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// Server服务器枚举
    /// </summary>
    [Flags]
    public enum EnServer : byte
    {
        /// <summary>
        /// 服务器创建成功时发生
        /// </summary>
        Create = 1,
        /// <summary>
        /// 服务器创建失败时发生
        /// </summary>
        Fail = 2,
        /// <summary>
        /// 客户端连接服务器成功时发生
        /// </summary>
        Connect = 4,
        /// <summary>
        /// 向客户端发送数据时发生
        /// </summary>
        SendMsg = 8,
        /// <summary>
        /// 收到客户端数据时发生
        /// </summary>
        Receive = 16,
        /// <summary>
        /// 当处于连接状态的客户端断开时发生
        /// </summary>
        ClientClose = 32,
        /// <summary>
        /// 服务端关闭时发生
        /// </summary>
        Close = 64,
        /// <summary>
        /// 心跳包事件（接收后触发）
        /// </summary>
        HeartBeat = 128
    }
}
