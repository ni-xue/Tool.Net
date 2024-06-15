using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 服务端模型
    /// </summary>
    public interface INetworkListener : IDisposable
    {
        /// <summary>
        /// 服务器IP
        /// </summary>
        UserKey Server { get; }

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

        /// <summary>
        /// 相关事件委托
        /// </summary>
        /// <param name="Completed"></param>
        void SetCompleted(CompletedEvent<EnServer> Completed);

        /// <summary>
        /// 创建服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        Task StartAsync(string ip, int port);

        /// <summary>
        /// 可重写的事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="enAction"></param>
        IGetQueOnEnum OnComplete(in UserKey key, EnServer enAction);

        /// <summary>
        /// 关闭服务器
        /// </summary>
        void Stop();
    }

    /// <summary>
    /// 服务器模型二
    /// </summary>
    /// <typeparam name="ISocket"></typeparam>
    public interface INetworkListener<ISocket> : INetworkListener //ISocket -> Socket, WebSocket, Quic
    {
        /// <summary>
        /// 当前连接的用户池
        /// </summary>
        IReadOnlyDictionary<UserKey, ISocket> ListClient { get; }

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sendBytes"></param>
        /// <returns></returns>
        ValueTask SendAsync(SendBytes<ISocket> sendBytes);

        //void Send(ISocket client, params ArraySegment<byte>[] listData);

        /// <summary>
        /// 接收数据委托
        /// </summary>
        /// <param name="Received"></param>
        void SetReceived(ReceiveEvent<ISocket> Received);

        /// <summary>
        /// 根据key获取连接池中的用户
        /// </summary>
        /// <param name="key"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        bool TrySocket(in UserKey key, out ISocket client);

        /// <summary>
        /// 创建发送数据需要用的容器
        /// </summary>
        /// <param name="client"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        SendBytes<ISocket> CreateSendBytes(ISocket client, int length);
    }
}
