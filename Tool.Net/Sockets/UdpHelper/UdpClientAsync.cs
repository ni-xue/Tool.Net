using System;
using System.Buffers;
using System.Drawing;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.UdpHelper.Extend;

namespace Tool.Sockets.UdpHelper
{
    /// <summary>
    /// 封装一个底层异步Udp对象（客户端）IpV4
    /// </summary>
    public class UdpClientAsync : INetworkConnect<IUdpCore>
    {
        /// <summary>
        /// 获取当前心跳信息
        /// </summary>
        public KeepAlive Keep { get; private set; }

        private readonly int DataLength = 1024 * 8;

        private Socket client;
        private IUdpCore udp;

        internal Func<System.Net.EndPoint, Task<Socket>> TryP2PConnect;

        private bool isClose = false; //标识客户端连接是否关闭
        private bool isConnect = false; //标识是否调用了连接函数
        private bool isReceive = false; //标识是否调用了接收函数

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        /// <summary>
        /// 标识服务端连接是否关闭
        /// </summary>
        public bool IsClose { get { return isClose; } }

        private Ipv4Port server;//服务端IP
        private UdpEndPoint endPointServer;
        private int millisecond = 20; //默认20毫秒。
        private int receiveTimeout = 60000; //默认60000毫秒。
        private Memory<byte> arrayData;//一个连续的内存块

        /// <summary>
        /// 是否使用线程池调度接收后的数据
        /// 默认 true 开启
        /// </summary>
        public bool IsThreadPool { get; set; } = true;

        /// <summary>
        /// 回复消息延迟时间（警告：当前设置仅在开启了OnlyData模式生效，超时未回复会重发，重发最大次数10，依然没有回复将抛出异常！）小于20将不生效使用默认值
        /// </summary>
        public int ReplyDelay { get; init; } = 500;

        /// <summary>
        /// <para>监听最大等待时长（默认60秒）</para>
        /// <para>不得小于5秒</para>
        /// </summary>
        public int ReceiveTimeout
        {
            get => receiveTimeout;
            init
            {
                if (value < 5000) throw new Exception("设置的等待时长小于5秒。");
                receiveTimeout = value;
            }
        }

        /// <summary>
        /// 服务器创建时的信息
        /// </summary>
        public UserKey Server { get { return server; } }

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
        /// 当前设备的连接信息
        /// </summary>
        public Ipv4Port LocalPoint => StateObject.GetIpPort(Client?.LocalEndPoint);

        /// <summary>
        /// UDP 服务对象
        /// </summary>
        public Socket Client { get { return client; } }

        /// <summary>
        /// UDP 核心控制器
        /// </summary>
        public IUdpCore UdpCore { get { return UdpCore; } }

        /// <summary>
        /// 获取当前是否已连接到远程主机。
        /// </summary>
        public bool Connected => client.Connected;

        /// <summary>
        /// 禁用掉Receive通知事件，方便上层封装
        /// </summary>
        public bool DisabledReceive { get; init; } = false;

        /// <summary>
        /// 表示通讯的包大小
        /// </summary>
        public NetBufferSize BufferSize { get; }

        /**
         * 客户端连接完成、发送完成、连接异常或者服务端关闭触发的事件
        */
        private CompletedEvent<EnClient> Completed; //event

        /**
         * 客户端接收消息触发的事件
         */
        private ReceiveEvent<IUdpCore> Received; //event Span<byte>

        ///// <summary>
        ///// 用于控制异步接收消息
        ///// </summary>
        //private readonly ManualResetEvent doReceive = new(false);

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(CompletedEvent<EnClient> Completed) => this.Completed ??= Completed;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public void SetReceived(ReceiveEvent<IUdpCore> Received)
        {
            if (isReceive) throw new Exception("当前已无法绑定接收委托了，因为ConnectAsync()已经调用了。");
            this.Received ??= Received;
        }

        #region UdpClientAsync

        /// <summary>
        /// 创建一个 <see cref="UdpClientAsync"/> 客户端类
        /// </summary>
        public UdpClientAsync() : this(NetBufferSize.Default) { }

        /// <summary>
        /// 创建一个 <see cref="UdpClientAsync"/> 客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        public UdpClientAsync(NetBufferSize bufferSize) : this(bufferSize, false) { }

        /// <summary>
        /// 创建一个 <see cref="UdpClientAsync"/> 客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        /// <param name="OnlyData">是否启动框架模式</param>
        public UdpClientAsync(NetBufferSize bufferSize, bool OnlyData)
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
            int dataLength = (int)bufferSize;
            if (!OnlyData && bufferSize > NetBufferSize.Size32K)
            {
                if (bufferSize > NetBufferSize.Size64K) throw new ArgumentException($"Udp协议下只能 最大支持到Size64K，{UdpPack.MaxBuffer}B！", nameof(bufferSize));
                dataLength = UdpPack.MaxBuffer; //最大发送区和接收区 IP20 UDP8 （保留最少）12
            }
            this.BufferSize = bufferSize;
            this.DataLength = dataLength;
            this.OnlyData = OnlyData;
            this.arrayData = new byte[dataLength + (OnlyData ? StateObject.HeadSize : 0)];
        }

        #endregion

        #region Reconnection

        /// <summary>
        /// 重连，返回是否重连，如果没有断开是不会重连的
        /// </summary>
        public Task<bool> Reconnection() => throw new NotImplementedException("UDP 无有效连接！");

        #endregion

        /// <summary>
        /// 添加持久化消息（心跳），防止特殊情况下的断开连接
        /// </summary>
        public void AddKeepAlive(byte TimeInterval)
        {
            ThrowIfDisposed();

            lock (StateObject.Lock)
            {
                if (Keep == null)
                {
                    Keep = new KeepAlive(TimeInterval, async () =>
                    {
                        if (udp is not null)
                        {
                            await SendNoWaitAsync(udp.UdpState.GetKeepObj());
                            if (TryP2PConnect is not null) OnComplete(Server, EnClient.HeartBeat);
                        }
                    });
                    return;
                }
            }
            throw new Exception("心跳已经开启啦，请勿重复操作！");
        }

        #region ConnectAsync

        ///// <summary>
        ///// 发送数据目标，指定ip地址为127.0.0.1
        ///// </summary>
        ///// <param name="port">要连接服务端的端口</param>
        //public void Connect(int port)
        //{
        //    Connect(StaticData.LocalIp, port);
        //}

        ///// <summary>
        ///// 发送数据目标
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
        //    client.Connect(ipAddress, port);
        //    OnComplete(server, EnClient.Connect);
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
        /// 发送数据目标
        /// </summary>
        /// <param name="ip">要连接的服务器的ip地址</param>
        /// <param name="port">要连接的服务器的端口</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
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
                    client = StateObject.CreateSocket(false, BufferSize);
                    await client.ConnectAsync(endPointServer, CancellationToken.None);
                }
                //需要增加对有效连接的验证消息
                if (OnlyData)
                {
                    await Handshake.UdpAuthenticAtion(client, endPointServer);
                }
                isAuth = true;
            }
            catch (Exception ex)
            {
                if (TryP2PConnect is not null) throw new Exception("P2P打洞失败！", ex);
                if (isAuth is false) throw;
            }
            finally
            {
                if (client is not null) ConnectCallBack();
            }
        }

        #endregion

        #region SendAsync

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

        //private async Task SendAsync(ArraySegment<byte>[] listData, EnClient en)
        //{
        //    ThrowIfDisposed();

        //    if (!TcpStateObject.IsConnected(client))
        //    {
        //        throw new Exception("与服务端的连接已中断！");
        //    }

        //    IList<ArraySegment<byte>> buffers = TcpStateObject.GetBuffers(this.OnlyData, DataLength, listData);

        //    try
        //    {
        //        int count = await client.SendAsync(buffers, SocketFlags.None);//To endPointServer
        //        OnComplete(Server, en);
        //        if (EnClient.SendMsg == en) Keep?.ResetTime();
        //    }
        //    catch (Exception)
        //    {
        //        //如果发生异常，说明客户端失去连接，触发关闭事件
        //        InsideClose();
        //        //OnComplete(server, EnClient.Close);
        //    }
        //}

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="sendBytes">数据包对象</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public async ValueTask SendAsync(SendBytes<IUdpCore> sendBytes)
        {
            bool ispart = false;
            int i = 0;
            do
            {
                var memory = udp.GetSendMemory(sendBytes, ref ispart, ref i);
                await SendNoWaitAsync(memory);
            } while (ispart);
            OnComplete(Server, EnClient.SendMsg);
            Keep?.ResetTime();
        }

        private async ValueTask SendNoWaitAsync(Memory<byte> buffers)
        {
            ThrowIfDisposed();
            if (udp is null) throw new Exception("未调用ConnectAsync函数或未连接！");
            await udp.SendAsync(buffers);
        }

        /// <summary>
        /// 创建数据包对象
        /// </summary>
        /// <param name="length">数据包大小</param>
        /// <returns></returns>
        public SendBytes<IUdpCore> CreateSendBytes(int length = 0)
        {
            if (udp is null) throw new Exception("未调用ConnectAsync函数或未连接！");
            if (!OnlyData && length > DataLength) throw new ArgumentException($"Udp协议下文报只能 最大支持到{DataLength}B！（这与你设置的 NetBufferSize 枚举有关！）", nameof(length));
            if (length == 0) length = DataLength;
            return new SendBytes<IUdpCore>(udp, length, OnlyData);
        }

        #endregion

        /**
         * 异步连接的回调函数
         */
        private void ConnectCallBack()
        {
            if (UdpStateObject.IsConnected(client))
            {
                udp = IUdpCore.GetUdpCore(this, endPointServer, client, this.DataLength, this.OnlyData, ReplyDelay, false, TryP2PConnect is not null, IsReceived, Received);
                StateObject.StartReceive("Udp", StartReceive, udp); //StartReceive();
            }
            else
            {
                InsideClose();
                OnComplete(Server, EnClient.Fail);
            }

            void IsReceived(UserKey key, byte type)
            {
                Keep?.ResetTime();
                switch (type)
                {
                    case 0:
                        OnComplete(key, EnClient.HeartBeat);
                        break;
                    default:
                        if (!DisabledReceive) OnComplete(key, EnClient.Receive);
                        break;
                }
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private async Task StartReceive(IUdpCore udp)
        {
            isReceive = true;
            //接收数据包
            //endPointServer.SetUdpState(this.DataLength, this.OnlyData, Received);
            OnComplete(Server, EnClient.Connect).Wait();
            while (!isClose)
            {
                if (UdpStateObject.IsConnected(client))
                {
                    await ReceiveAsync(udp.UdpState);
                }
                else
                {
                    //如果发生异常，说明客户端失去连接，触发关闭事件
                    InsideClose();
                    //OnComplete(obj.IpPort, EnClient.Close);
                    //if (this.IsReconnect) WhileReconnect();
                    break;
                }
            }
            OnComplete(Server, EnClient.Close);
            await udp.DisposeAsync();
        }

        /// <summary>
        /// 异步接收消息
        /// </summary>
        private async ValueTask ReceiveAsync(UdpStateObject obj)
        {
            //doReceive.Reset();
            try
            {
                SocketReceiveFromResult result = await UdpStateObject.ReceiveFromAsync(client, arrayData, endPointServer, receiveTimeout);
                if (result.RemoteEndPoint is UdpEndPoint point && point == endPointServer) //客户端只处理匹配的数据包
                {
                    obj.UpDateSignal();

                    await udp.ReceiveAsync(arrayData[..result.ReceivedBytes]);

                    //var head = arrayData[..StateObject.HeadSize];
                    //if (obj.OnReceiveTask(head, result.ReceivedBytes, out bool isreply, out bool isReceive))//尝试使用，原线程处理包解析，验证效率
                    //{
                    //    if (isreply) await SendNoWaitAsync(head);
                    //    if (isReceive) OnReceived(arrayData[..result.ReceivedBytes], obj);
                    //}
                }
            }
            catch (OperationCanceledException)
            {
                client.Close(); //Udp等待超时，关闭连接。
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 可供开发重写的实现类
        /// </summary>
        /// <param name="IpPort">IP：端口</param>
        /// <param name="enAction">消息类型</param>
        public virtual IGetQueOnEnum OnComplete(in UserKey IpPort, EnClient enAction) => EnumEventQueue.OnComplete(in IpPort, enAction, Completed);

        /// <summary>
        /// UDP关闭
        /// </summary>
        void InsideClose()
        {
            udp?.Close();
        }

        /// <summary>
        /// UDP关闭
        /// </summary>
        public void Close()
        {
            isClose = true;
            InsideClose();
            Keep?.Close();
        }

        /// <summary>
        /// 回收UDP相关资源
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            Close();
            client?.Dispose();
            arrayData = null;
            udp?.Dispose();

            //doConnect.Close();
            //_mre.Close();
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
