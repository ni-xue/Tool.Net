using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 设置传输大小
    /// </summary>
    public enum TcpBufferSize : int
    {
        /// <summary>
        /// 默认有系统分配大小
        /// </summary>
        Default = 0,
        /// <summary>
        /// 8KB=8192字节（带宽小于1Mbps）建议使用
        /// </summary>
        Size8K = 8192,
        /// <summary>
        /// 16KB=16384字节（带宽在1Mbps----100Mbps）建议使用
        /// </summary>
        Size16K = 16384,
        /// <summary>
        /// 64KB=65536字节（带宽大于100Mbps）建议使用
        /// </summary>
        Size64K = 65536
    }
}
