using System;
using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 连接通信模型
    /// </summary>
    public interface INetworkConnect : IDisposable
    {
        /// <summary>
        /// 服务器信息
        /// </summary>
        UserKey Server { get; }

        /// <summary>
        /// 本机通信IP
        /// </summary>
        Ipv4Port LocalPoint { get; }

        /// <summary>
        /// 是否关闭
        /// </summary>
        bool IsClose { get; }

        /// <summary>
        /// 是否连接中
        /// </summary>
        bool Connected { get; }

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
        IGetQueOnEnum OnComplete(in UserKey key, EnClient enAction);

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
