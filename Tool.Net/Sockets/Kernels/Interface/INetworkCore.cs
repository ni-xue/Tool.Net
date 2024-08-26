using System;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 连接协议顶层接口
    /// </summary>
    public interface INetworkCore : IDisposable
    {
        /// <summary>
        /// 服务器信息
        /// </summary>
        UserKey Server { get; }

        /// <summary>
        /// 监听控制毫秒
        /// </summary>
        int Millisecond { get; }

        /// <summary>
        /// 是否关闭
        /// </summary>
        bool IsClose { get; }

        /// <summary>
        /// 是否启用线程池处理接收数据
        /// </summary>
        bool IsThreadPool { get; }

        /// <summary>
        /// 是否取消内部接收数据事件推送
        /// </summary>
        bool DisabledReceive { get; }

        /// <summary>
        /// 表示通讯的包大小
        /// </summary>
        NetBufferSize BufferSize { get; }
    }
}
