using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 通信公共模板抽象类（客户端版）
    /// </summary>
    /// <typeparam name="ISocket"></typeparam>
    /// <remarks>代码由逆血提供支持</remarks>
    public abstract class NetworkConnect<ISocket> : EnClientEventDrive,  INetworkConnect<ISocket>
    {
        /// <summary>
        /// 默认构造（公共模板信息）
        /// </summary>
        protected NetworkConnect() { }

        /// <summary>
        /// 当前设备的连接信息
        /// </summary>
        public abstract Ipv4Port LocalPoint { get; }

        /// <summary>
        /// 获取一个值，该值指示 Client 的基础 Socket 是否已连接到远程主机。
        /// </summary>
        public abstract bool Connected { get; }

        /// <summary>
        /// 服务器的连接信息
        /// </summary>
        public abstract UserKey Server { get; }

        /// <summary>
        /// 监听控制毫秒
        /// </summary>
        public abstract int Millisecond { get; set; }

        /// <summary>
        /// 标识客户端是否关闭，改状态为调用关闭方法后的状态。
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
        /// TCP关闭
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="ip">要连接的服务器的ip地址</param>
        /// <param name="port">要连接的服务器的端口</param>
        public abstract Task ConnectAsync(string ip, int port);

        /// <summary>
        /// 创建数据发送空间
        /// </summary>
        /// <param name="length">数据大小</param>
        /// <returns></returns>
        public abstract SendBytes<ISocket> CreateSendBytes(int length);

        /// <summary>
        /// 关闭连接，回收相关资源
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// 可供开发重写的事件方法
        /// </summary>
        /// <param name="key">IP：端口</param>
        /// <param name="enAction">消息类型</param>
        public abstract ValueTask<IGetQueOnEnum> OnComplete(in UserKey key, EnClient enAction);

        /// <summary>
        /// 重连，返回是否重连，如果没有断开是不会重连的
        /// </summary>
        public abstract Task<bool> Reconnection();

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
        public abstract void SetCompleted(CompletedEvent<EnClient> Completed);

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public abstract void SetReceived(ReceiveEvent<ISocket> Received);
    }
}
