using System.Collections.Generic;
using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 通信公共模板抽象类（服务端版）
    /// </summary>
    /// <typeparam name="ISocket"></typeparam>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class NetworkListener<ISocket> : EnServerEventDrive, INetworkListener<ISocket>
    {
        /// <summary>
        /// 默认构造（公共模板信息）
        /// </summary>
        protected NetworkListener() { }

        /// <summary>
        /// 已建立连接的集合
        /// key:UserKey
        /// value:Socket
        /// </summary>
        public abstract IReadOnlyDictionary<UserKey, ISocket> ListClient { get; }

        /// <summary>
        /// 服务器创建时的信息
        /// </summary>
        public abstract UserKey Server { get; }

        /// <summary>
        /// 监听控制毫秒
        /// </summary>
        public abstract int Millisecond { get; set; }

        /// <summary>
        /// 标识服务端连接是否关闭
        /// </summary>
        public abstract bool IsClose { get; }

        /// <summary>
        /// 是否使用线程池调度接收后的数据
        /// 默认 true 开启
        /// </summary>
        public virtual bool IsThreadPool { get; set; } = true;

        /// <summary>
        /// 表示通讯的包大小
        /// </summary>
        public virtual NetBufferSize BufferSize { get; protected init; } = NetBufferSize.Size8K;

        /// <summary>
        /// 创建数据发送空间
        /// </summary>
        /// <param name="client">收数据的对象</param>
        /// <param name="length">数据大小</param>
        /// <returns></returns>
        public abstract SendBytes<ISocket> CreateSendBytes(ISocket client, int length);

        /// <summary>
        /// 关闭连接，回收相关资源
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 可供开发重写的事件方法
        /// </summary>
        /// <param name="key">指定发送对象</param>
        /// <param name="enAction">消息类型</param>
        public abstract ValueTask<IGetQueOnEnum> OnComplete(in UserKey key, EnServer enAction);

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="sendBytes">数据包</param>
        /// <returns></returns>
        public abstract ValueTask SendAsync(SendBytes<ISocket> sendBytes);

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public abstract void SetCompleted(CompletedEvent<EnServer> Completed);

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public abstract void SetReceived(ReceiveEvent<ISocket> Received);

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public abstract Task StartAsync(string ip, int port);

        /// <summary>
        /// TCP关闭
        /// </summary>
        public abstract void Stop();

        /// <summary>
        /// 根据IP:Port获取对应的连接对象
        /// </summary>
        /// <param name="key">IP:Port</param>
        /// <param name="client">连接对象</param>
        /// <returns>返回成功状态</returns>
        public abstract bool TrySocket(in UserKey key, out ISocket client);
    }
}
