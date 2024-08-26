using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.TcpHelper
{
    /// <summary>
    /// 封装一个底层异步TCP对象（客户端）IpV4
    /// </summary>
    public class TcpClientAsync : INetworkConnect<Socket>
    {
        /// <summary>
        /// 获取当前心跳信息
        /// </summary>
        public KeepAlive Keep { get; private set; }

        private readonly int DataLength = 1024 * 8;

        private Socket client;

        internal Func<EndPoint, Task<Socket>> TryP2PConnect;

        /**
         * 客户端连接完成、发送完成、连接异常或者服务端关闭触发的事件
         */
        private CompletedEvent<EnClient> Completed; //event

        /**
         * 客户端接收消息触发的事件
         */
        private ReceiveEvent<Socket> Received; //event Span<byte>


        private bool isClose = false; //标识客户端是否关闭
        private bool isConnect = false; //标识是否调用了连接函数
        private bool isReceive = false; //标识是否调用了接收函数

        //服务端IP
        private Ipv4Port server;
        private IPEndPoint endPointServer;
        private int millisecond = 20; //默认20毫秒。
        private bool isWhileReconnect = false;

        /// <summary>
        /// 表示通讯的包大小
        /// </summary>
        public NetBufferSize BufferSize { get; }// = NetBufferSize.Size8K;

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        /// <summary>
        /// 是否使用线程池调度接收后的数据
        /// 默认 true 开启
        /// </summary>
        public bool IsThreadPool { get; set; } = true;

        /// <summary>
        /// 禁用掉Receive通知事件，方便上层封装
        /// </summary>
        public bool DisabledReceive { get; init; } = false;

        /// <summary>
        /// 是否在与服务器断开后主动重连？ 
        /// </summary>
        public bool IsReconnect { get; private set; }

        /// <summary>
        /// 服务器的连接信息
        /// </summary>
        public UserKey Server { get { return server; } }

        /// <summary>
        /// 当前设备的连接信息
        /// </summary>
        public Ipv4Port LocalPoint => StateObject.GetIpPort(Client?.LocalEndPoint);

        /// <summary>
        /// TCP 服务对象
        /// </summary>
        public Socket Client { get { return client; } }

        /// <summary>
        /// 标识客户端是否关闭，改状态为调用关闭方法后的状态。
        /// </summary>
        public bool IsClose { get { return isClose; } }

        /// <summary>
        /// 获取一个值，该值指示 Client 的基础 Socket 是否已连接到远程主机。
        /// </summary>
        public bool Connected => client.Connected;

        /// <summary>
        /// 监听控制毫秒
        /// </summary>
        public int Millisecond
        {
            get
            {
                return millisecond;
            }
            set
            {
                if (value > 60 * 1000) { millisecond = 60 * 1000; }
                else if (value < 0) { millisecond = 0; }
                else { millisecond = value; }
            }
        }

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(CompletedEvent<EnClient> Completed) => this.Completed ??= Completed;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public void SetReceived(ReceiveEvent<Socket> Received)
        {
            if (isReceive) throw new Exception("当前已无法绑定接收委托了，因为ConnectAsync()已经调用了。");
            this.Received ??= Received;
        }

        #region TcpClientAsync

        /// <summary>
        /// 创建一个 <see cref="TcpClientAsync"/> 客户端类
        /// </summary>
        public TcpClientAsync() : this(NetBufferSize.Default) { }

        /// <summary>
        /// 创建一个 <see cref="TcpClientAsync"/> 客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        public TcpClientAsync(NetBufferSize bufferSize) : this(bufferSize, false) { }

        /// <summary>
        /// 创建一个 <see cref="TcpClientAsync"/> 客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        /// <param name="OnlyData">是否启动框架模式</param>
        public TcpClientAsync(NetBufferSize bufferSize, bool OnlyData) : this(bufferSize, OnlyData, false) { }

        /// <summary>
        /// 创建一个 <see cref="TcpClientAsync"/> 客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        /// <param name="OnlyData">是否启动框架模式</param>
        /// <param name="IsReconnect">是否在与服务器断开后主动重连？ </param>
        public TcpClientAsync(NetBufferSize bufferSize, bool OnlyData, bool IsReconnect)
        {
            if (NetBufferSize.Default == bufferSize) bufferSize = NetBufferSize.Size8K;
            //if (DataLength < 8 * 1024)
            //{
            //    throw new ArgumentException("DataLength 值必须大于8KB！", nameof(DataLength));
            //}
            //if (DataLength > 1024 * 1024 * 20)
            //{
            //    throw new ArgumentException("DataLength 值必须是在20M(DataLength < 1024 * 1024 * 20)以内！", nameof(DataLength));
            //}
            this.BufferSize = bufferSize;
            this.DataLength = (int)bufferSize;
            this.OnlyData = OnlyData;
            this.IsReconnect = IsReconnect;
        }

        #endregion

        #region Reconnection

        /// <summary>
        /// 重连，返回是否重连，如果没有断开是不会重连的
        /// </summary>
        public async Task<bool> Reconnection()
        {
            ThrowIfDisposed();

            try
            {
                if (!isClose)
                {
                    if (!TcpStateObject.IsConnected(client))
                    {
                        client.Close();
                        client.Dispose();
                        await ConnectAsync();
                    }
                }
                return true;
            }
            catch
            {
                InsideClose();
                return false;
            }
        }

        private async Task WhileReconnect()
        {
            while (IsReconnect)
            {
                if (await Reconnection()) break;
                await Task.Delay(100); //等待一下才继续
            }
        }

        private void StartReconnect()
        {
            if (!isWhileReconnect)
            {
                isWhileReconnect = true;
                StateObject.StartTask("Tcp重连", WhileReconnect);
            }
        }

        #endregion

        /// <summary>
        /// 添加持久化消息（心跳），防止特殊情况下的断开连接
        /// </summary>
        public void AddKeepAlive(byte TimeInterval)
        {
            ThrowIfDisposed();

            if (!OnlyData) throw new InvalidOperationException("OnlyData 未设置成 true 无法提供相关服务。");

            lock (StateObject.Lock)
            {
                if (Keep == null)
                {
                    Keep = new KeepAlive(TimeInterval, async () =>
                    {
                        await SendNoWaitAsync(KeepAlive.TcpKeepObj);
                        if (TryP2PConnect is not null) OnComplete(Server, EnClient.HeartBeat);
                    });
                    return;
                }
            }
            throw new Exception("心跳已经开启啦，请勿重复操作！");
        }

        #region ConnectAsync

        ///// <summary>
        ///// 异步连接，连接ip地址为127.0.0.1
        ///// </summary>
        ///// <param name="port">要连接服务端的端口</param>
        //public void Connect(int port)
        //{
        //    Connect(StaticData.LocalIp, port);
        //}

        ///// <summary>
        ///// 异步连接
        ///// </summary>
        ///// <param name="ip">要连接的服务器的ip地址</param>
        ///// <param name="port">要连接的服务器的端口</param>
        //public void Connect(string ip, int port)
        //{
        //    IPAddress ipAddress;
        //    try
        //    {
        //        ipAddress = IPAddress.Parse(ip);
        //    }
        //    catch (Exception)
        //    {
        //        throw new Exception("ip地址格式不正确，请使用正确的ip地址！");
        //    }
        //    server = $"{ip}:{port}";

        //    try
        //    {
        //        client.Connect(ipAddress, port);
        //    }
        //    catch //(Exception ex)
        //    {
        //        throw;
        //    }
        //    finally
        //    {
        //        ActionConnect(client);
        //    }
        //}

        /// <summary>
        /// 异步连接，连接ip地址为127.0.0.1
        /// </summary>
        /// <param name="port">要连接服务端的端口</param>
        public async Task ConnectAsync(int port)
        {
            await ConnectAsync(StaticData.LocalIp, port);
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="ip">要连接的服务器的ip地址</param>
        /// <param name="port">要连接的服务器的端口</param>
        public async Task ConnectAsync(string ip, int port)
        {
            if (!StateObject.IsIpPort($"{ip}:{port}", out Ipv4Port ipv4Port))
            {
                throw new FormatException("ip:port 无法被 IPEndPoint 对象识别！");
            }
            await ConnectAsync(ipv4Port);
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="ipv4Port">要连接的服务器的ip地址端口</param>
        public async Task ConnectAsync(Ipv4Port ipv4Port)
        {
            ThrowIfDisposed();

            if (isConnect) throw new Exception("当前对象以调用ConnectAsync该函数！");
            endPointServer = new(ipv4Port.Ip, ipv4Port.Port);
            server = ipv4Port;
            isConnect = true;
            await ConnectAsync();
        }

        private async Task ConnectAsync()
        {
            bool isAuth = false;
            try
            {
                if (TryP2PConnect is not null)
                {
                    client = await TryP2PConnect.Invoke(endPointServer);
                }
                else
                {
                    client = StateObject.CreateSocket(true, BufferSize);
                    await client.ConnectAsync(endPointServer, CancellationToken.None);
                }
                //需要增加对有效连接的验证消息
                if (OnlyData)
                {
                    await Handshake.TcpAuthenticAtion(client);
                }
                isAuth = true;
            }
            catch (Exception ex)
            {
                if (TryP2PConnect is not null) throw new Exception("P2P打洞失败！", ex);
                if (!IsReconnect) throw;
                if (isAuth is false) throw;
            }
            finally
            {
                if (client is not null) ConnectCallBack();
            }
        }

        #endregion

        #region SendAsync

        ///// <summary>
        ///// 发送消息
        ///// </summary>
        ///// <param name="msg"></param>
        //public void Send(string msg)
        //{
        //    byte[] listData = Encoding.UTF8.GetBytes(msg);
        //    Send(listData);
        //}

        ///// <summary>
        ///// 发送消息
        ///// </summary>
        ///// <param name="listData">数据包</param>
        //public void Send(params ArraySegment<byte>[] listData)
        //{
        //    if (!TcpStateObject.IsConnected(client))
        //    {
        //        throw new Exception("与服务端的连接已中断！");
        //    }

        //    IList<ArraySegment<byte>> buffers = TcpStateObject.GetBuffers(this.OnlyData, DataLength, listData);

        //    try
        //    {
        //        int count = client.Send(buffers, SocketFlags.None);
        //        //string key = StateObject.GetIpPort(client);
        //        OnComplete(server, EnClient.SendMsg);
        //    }
        //    catch (Exception)
        //    {
        //        //如果发生异常，说明客户端失去连接，触发关闭事件
        //        InsideClose();
        //        //string key = StateObject.GetIpPort(client);
        //        OnComplete(server, EnClient.Close);
        //    }

        //    Keep?.ResetTime();
        //}

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="msg">文本数据</param>
        public async ValueTask SendAsync(string msg)
        {
            var chars = msg.AsMemory();
            if (chars.IsEmpty) throw new ArgumentNullException(nameof(msg));
            var sendBytes = CreateSendBytes(Encoding.UTF8.GetByteCount(chars.Span));

            try
            {
                Encoding.UTF8.GetBytes(chars.Span, sendBytes.Span);
                await SendAsync(sendBytes);
            }
            finally
            {
                sendBytes.Dispose();
            }
        }

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="listData">数据包</param>
        public async ValueTask SendAsync(ArraySegment<byte> listData)
        {
            var sendBytes = CreateSendBytes(listData.Count);

            try
            {
                sendBytes.SetMemory(listData);
                await SendAsync(sendBytes);
            }
            finally
            {
                sendBytes.Dispose();
            }
        }

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="sendBytes">数据包</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">OnlyData验证失败</exception>
        public async ValueTask SendAsync(SendBytes<Socket> sendBytes)
        {
            if (sendBytes.OnlyData != OnlyData) throw new ArgumentException("与当前套接字协议不一致！", nameof(sendBytes.OnlyData));

            var buffers = sendBytes.GetMemory();
            await SendNoWaitAsync(buffers);
            OnComplete(Server, EnClient.SendMsg);
            Keep?.ResetTime();
        }

        private async ValueTask SendNoWaitAsync(Memory<byte> buffers)
        {
            ThrowIfDisposed();
            if (!TcpStateObject.IsConnected(client)) throw new Exception("与服务端的连接已中断！");
            try
            {
                int count = await client.SendAsync(buffers, SocketFlags.None); 
            }
            catch (Exception)
            {
                //如果发生异常，说明客户端失去连接，触发关闭事件
                InsideClose();
                //OnComplete(server, EnClient.Close);
                throw;
            }
        }

        /// <summary>
        /// 创建数据发送空间
        /// </summary>
        /// <param name="length">数据大小</param>
        /// <returns></returns>
        /// <exception cref="Exception">连接已中断</exception>
        public SendBytes<Socket> CreateSendBytes(int length = 0)
        {
            if (!TcpStateObject.IsConnected(Client)) throw new Exception("与服务端的连接已中断！");
            if (length == 0) length = DataLength;
            return new SendBytes<Socket>(Client, length, OnlyData);
        }

        #endregion

        /**
         * 异步连接的回调函数
         */
        private void ConnectCallBack()
        {
            if (TcpStateObject.IsConnected(client))
            {
                StateObject.StartReceive("Tcp", StartReceive, client); //StartReceive(client);
            }
            else
            {
                InsideClose();
                OnComplete(Server, EnClient.Fail);
                StartReconnect();
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private async Task StartReceive(Socket client)
        {
            isReceive = true;
            //接收数据包
            TcpStateObject obj = new(client, this.DataLength, this.OnlyData, Received);
            OnComplete(obj.IpPort, EnClient.Connect).Wait();
            while (!isClose)
            {
                await Task.Delay(Millisecond);
                if (obj.IsConnected())
                {
                    await ReceiveAsync(obj);
                }
                else
                {
                    //如果发生异常，说明客户端失去连接，触发关闭事件
                    InsideClose();
                    OnComplete(obj.IpPort, EnClient.Close);
                    StartReconnect();
                    break;
                }
            }
            obj.Close();
        }

        /**
         * 异步接收消息
         */
        private async Task ReceiveAsync(TcpStateObject obj)
        {
            //obj.doReceive.Reset();
            try
            {
                if (!await obj.ReceiveAsync()) { return; }
                //obj.OnReceiveTask(OnReceived);//尝试使用，原线程处理包解析，验证效率
                Memory<byte> memory = Memory<byte>.Empty;
                bool isend = false;
                while (obj.OnReceiveTask(ref memory, ref isend)) await OnReceived(memory, obj);

                //obj.Client.BeginReceive(obj.MemoryData, obj.WriteIndex, obj.SpareSize, SocketFlags.None, ReceiveCallBack, obj);
                //obj.doReceive.WaitOne();
            }
            catch (Exception)
            {
                obj.ClientClose();
            }

            async ValueTask OnReceived(Memory<byte> listData, TcpStateObject obj)
            {
                Keep?.ResetTime();
                if (TryP2PConnect is not null && obj.IsKeepAlive(in listData)) 
                {
                    OnComplete(obj.IpPort, EnClient.HeartBeat);
                }
                else
                {
                    if (!DisabledReceive) OnComplete(obj.IpPort, EnClient.Receive);
                    await obj.OnReceiveAsync(IsThreadPool, listData);
                }
            }
        }

        /// <summary>
        /// 可供开发重写的事件方法
        /// </summary>
        /// <param name="IpPort">IP：端口</param>
        /// <param name="enAction">消息类型</param>
        public virtual IGetQueOnEnum OnComplete(in UserKey IpPort, EnClient enAction) => EnumEventQueue.OnComplete(IpPort, enAction, Completed);

        /// <summary>
        /// TCP关闭
        /// </summary>
        void InsideClose()
        {
            client?.Close();
        }

        /// <summary>
        /// TCP关闭
        /// </summary>
        public void Close()
        {
            IsReconnect = false;
            isClose = true;
            InsideClose();
            Keep?.Close();
        }

        /// <summary>
        /// 关闭连接，回收相关资源
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            Close();
            client?.Dispose();
            GC.SuppressFinalize(this);
        }

        bool _disposed = false;

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                ThrowObjectDisposedException();
            }

            void ThrowObjectDisposedException() => throw new ObjectDisposedException(GetType().FullName);
        }
    }
}
