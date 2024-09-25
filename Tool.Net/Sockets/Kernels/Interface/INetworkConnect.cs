using System;
using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 连接通信模型
    /// </summary>
    public interface INetworkConnect : INetworkCore
    {
        /// <summary>
        /// 本机通信IP
        /// </summary>
        Ipv4Port LocalPoint { get; }

        /// <summary>
        /// 是否连接中
        /// </summary>
        bool Connected { get; }

        /// <summary>
        /// 相关事件委托
        /// </summary>
        /// <param name="Completed"></param>
        void SetCompleted(CompletedEvent<EnClient> Completed);

        /// <summary>
        /// 连接服务器
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        Task ConnectAsync(string ip, int port);

        /// <summary>
        /// 重连
        /// </summary>
        /// <returns></returns>
        Task<bool> Reconnection();

        /// <summary>
        /// 可重写的事件
        /// </summary>
        /// <param name="key"></param>
        /// <param name="enAction"></param>
        ValueTask<IGetQueOnEnum> OnComplete(in UserKey key, EnClient enAction);

        /// <summary>
        /// 关闭当前连接
        /// </summary>
        void Close();
    }

    /// <summary>
    /// 连接通信模型二
    /// </summary>
    /// <typeparam name="ISocket"></typeparam>
    public interface INetworkConnect<ISocket> : INetworkConnect
    {
        /// <summary>
        /// 接收数据委托
        /// </summary>
        /// <param name="Received"></param>
        void SetReceived(ReceiveEvent<ISocket> Received);

        /// <summary>
        /// 发送数据
        /// </summary>
        /// <param name="sendBytes"></param>
        /// <returns></returns>
        ValueTask SendAsync(SendBytes<ISocket> sendBytes);

        //void Send(params ArraySegment<byte>[] listData);

        /// <summary>
        /// 创建发送数据需要用的容器
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        SendBytes<ISocket> CreateSendBytes(int length);
    }
}
