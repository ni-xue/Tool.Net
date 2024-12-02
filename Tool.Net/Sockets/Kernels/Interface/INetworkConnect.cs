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
        /// 设置开启或关闭不想收到的消息事件
        /// </summary>
        /// <param name="enClient"><see cref="EnClient"/></param>
        /// <param name="state">等于true时生效，将关闭一切的相关事件</param>
        /// <returns>返回true时表示设置成功！</returns>
        bool OnInterceptor(EnClient enClient, bool state);

        /// <summary>
        /// 设置将<see cref="EnClient"/>事件，载入或不载入
        /// </summary>
        /// <param name="enClient"><see cref="EnClient"/></param>
        /// <param name="state">等于true时，事件由队列线程完成，false时交由任务线程自行完成</param>
        /// <returns>返回true时表示设置成功！</returns>
        bool OnIsQueue(EnClient enClient, bool state);

        /// <summary>
        /// 获取该事件是否会触发
        /// </summary>
        /// <param name="enClient"><see cref="EnClient"/></param>
        /// <returns><see cref="bool"/></returns>
        bool IsEvent(EnClient enClient);

        /// <summary>
        /// 获取该事件是否在队列任务中运行
        /// </summary>
        /// <param name="enClient"><see cref="EnClient"/></param>
        /// <returns><see cref="bool"/></returns>
        bool IsQueue(EnClient enClient);

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
