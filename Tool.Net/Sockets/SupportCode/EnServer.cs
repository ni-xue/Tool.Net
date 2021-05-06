using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Sockets.SupportCode
{
    /// <summary>
    /// ServerFrame类的行为
    /// </summary>
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
        Connect = 3,
        /// <summary>
        /// 向客户端发送数据时发生
        /// </summary>
        SendMsg = 4,
        /// <summary>
        /// 收到客户端数据时发生
        /// </summary>
        Receive = 5,
        /// <summary>
        /// 当处于连接状态的客户端断开时发生
        /// </summary>
        ClientClose = 6,
        /// <summary>
        /// 服务端关闭时发生
        /// </summary>
        Close = 7,
        /// <summary>
        /// 心跳包事件
        /// </summary>
        HeartBeat = 10
    }
}
