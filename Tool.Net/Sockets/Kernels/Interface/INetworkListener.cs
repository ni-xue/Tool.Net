using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 服务端模型
    /// </summary>
    public interface INetworkListener : INetworkCore
    {
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
        ValueTask<IGetQueOnEnum> OnComplete(in UserKey key, EnServer enAction);

        /// <summary>
        /// 设置开启或关闭不想收到的消息事件
        /// </summary>
        /// <param name="enServer"><see cref="EnServer"/></param>
        /// <param name="state">等于true时生效，将关闭一切的相关事件</param>
        /// <returns>返回true时表示设置成功！</returns>
        bool OnInterceptor(EnServer enServer, bool state);

        /// <summary>
        /// 设置将<see cref="EnServer"/>事件，载入或不载入，队列池
        /// </summary>
        /// <param name="enServer"><see cref="EnServer"/></param>
        /// <param name="state">等于true时，事件由队列线程完成，false时交由任务线程自行完成</param>
        /// <returns>返回true时表示设置成功！</returns>
        bool OnIsQueue(EnServer enServer, bool state);

        /// <summary>
        /// 获取该事件是否会触发
        /// </summary>
        /// <param name="enServer"><see cref="EnServer"/></param>
        /// <returns><see cref="bool"/></returns>
        bool IsEvent(EnServer enServer);

        /// <summary>
        /// 获取该事件是否在队列任务中运行
        /// </summary>
        /// <param name="enServer"><see cref="EnServer"/></param>
        /// <returns><see cref="bool"/></returns>
        bool IsQueue(EnServer enServer);

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
