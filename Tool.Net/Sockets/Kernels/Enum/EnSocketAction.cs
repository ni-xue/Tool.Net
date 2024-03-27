using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 接收socket的行为
    /// </summary>
    public enum EnSocketAction
    {
        /// <summary>
        /// 连接创建成功
        /// </summary>
        Create = 1,
        /// <summary>
        /// 连接创建失败
        /// </summary>
        Fail = 2,
        /// <summary>
        /// socket发生连接
        /// </summary>
        Connect = 4,
        /// <summary>
        /// socket发送数据
        /// </summary>
        SendMsg = 6,
        /// <summary>
        /// socket关闭
        /// </summary>
        Close = 8,
        /// <summary>
        /// socket监视器关闭
        /// </summary>
        MonitorClose = 10
    }
}
