using System;
using System.Collections.Generic;
using System.Text;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 设置传输大小
    /// </summary>
    public enum NetBufferSize : int
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
        /// 32KB=32768字节（带宽大于50Mbps）建议使用
        /// </summary>
        Size32K = 32768,
        /// <summary>
        /// 64KB=65536字节（带宽大于100Mbps）建议使用
        /// </summary>
        Size64K = 65536,
        /// <summary>
        /// 128KB=131072 字节（带宽大于200Mbps）建议使用
        /// </summary>
        Size128K = 131072,
        /// <summary>
        /// 256KB=262144 字节（带宽大于400Mbps）建议使用
        /// </summary>
        Size256K = 262144,
        /// <summary>
        /// 512KB=524288 字节（带宽大于800Mbps）建议使用
        /// </summary>
        Size512K = 524288,
        /// <summary>
        /// 1024KB=1048576 字节（带宽大于1000Mbps）建议使用
        /// </summary>
        Size1024K = 1048576,
    }
}
