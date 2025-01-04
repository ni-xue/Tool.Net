using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.WebHelper
{
    /// <summary>
    /// WebSocket连接对象
    /// </summary>
    public sealed class WebClientAsync : NetworkConnect<WebSocket>
    {
        /// <summary>
        /// 获取当前心跳信息
        /// </summary>
        public KeepAlive Keep { get; private set; }

        private readonly int DataLength = 1024 * 8;
        private ClientWebSocket client;
        private bool isClose = false; //标识客户端连接是否关闭
        private bool isReceive = false; //标识是否调用了接收函数

        ///// <summary>
        ///// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        ///// </summary>
        //public bool OnlyData { get; }

        /// <summary>
        /// 标识客户端连接是否关闭
        /// </summary>
        public override bool IsClose { get { return isClose; } }

        /// <summary>
        /// 获取指示是否使用安全套接字层 (SSL) 保护 WebSocket 连接的值。
        /// </summary>
        /// <remarks>true 如果使用 SSL; 保护 WebSocket 连接，否则为false。</remarks>
        public bool IsSSL { get; init; } = false;

        private UserKey server; //服务端IP
        private int millisecond = 20; //默认20毫秒。
        private bool isWhileReconnect = false;

        /// <summary>
        /// 是否在与服务器断开后主动重连？ 
        /// </summary>
        public bool IsReconnect { get; private set; }

        /// <summary>
        /// 服务器创建时的信息
        /// </summary>
        public override UserKey Server { get { return server; } }

        /// <summary>
        /// 监听控制毫秒
        /// </summary>
        public override int Millisecond
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
        public override Ipv4Port LocalPoint => throw new NotImplementedException("WebSocket无法获取");

        /// <summary>
        /// 获取当前是否已连接到远程主机。
        /// </summary>
        public override bool Connected => client.State == WebSocketState.Open;

        /**
         * 提供自定义注册的服务
         */
        private Action<ClientWebSocketOptions> InitWebOptions;

        /**
         * 连接、发送、关闭事件
         */
        private CompletedEvent<EnClient> Completed; //event

        /**
         * 接收到数据事件
         */
        private ReceiveEvent<WebSocket> Received; //event

        /**
        * 需要产生重连行为时发生，初衷因存在ip和端口更换，变动故需要产生该行为
        */
        private ReconnectEvent Reconnect; //event

        //**
        //* 信号
        //*/
        //private readonly ManualResetEvent _mre;

        //**
        // * 事件消息Queue
        // */
        //private readonly ConcurrentQueue<GetQueOnEnum> _que;

        /// <summary>
        /// 在连接发送前，回调可设置的参数
        /// </summary>
        /// <param name="initWebOptions"></param>
        public void SetInitWebContext(Action<ClientWebSocketOptions> initWebOptions) => this.InitWebOptions ??= initWebOptions;

        /// <summary>
        /// 连接、发送、关闭事件 <see cref="EnClient"/>
        /// </summary>
        /// <param name="Completed"></param>
        public override void SetCompleted(CompletedEvent<EnClient> Completed) => this.Completed ??= Completed;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public override void SetReceived(ReceiveEvent<WebSocket> Received)
        {
            if (isReceive) throw new Exception("当前已无法绑定接收委托了，因为ConnectAsync()已经调用了。");
            this.Received ??= Received;
        }

        /// <summary>
        /// 需要产生重连行为时发生，初衷因存在ip和端口更换，变动故需要产生该行为
        /// </summary>
        /// <param name="Reconnect">任务委托</param>
        public override void SetReconnect(ReconnectEvent Reconnect) => this.Reconnect ??= Reconnect;

        #region WebClientAsync

        /// <summary>
        /// 创建一个 <see cref="WebClientAsync"/> 客户端类
        /// </summary>
        public WebClientAsync() : this(NetBufferSize.Default) { }

        /// <summary>
        /// 创建一个 <see cref="WebClientAsync"/> 客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        public WebClientAsync(NetBufferSize bufferSize) : this(bufferSize, false) { }

        ///// <summary>
        ///// 创建一个 <see cref="WebClientAsync"/> 客户端类，确认模式和设置流大小
        ///// </summary>
        ///// <param name="bufferSize">包大小枚举</param>
        ///// <param name="OnlyData">是否启动框架模式</param>
        //public WebClientAsync(NetBufferSize bufferSize, bool OnlyData) : this(bufferSize, OnlyData, false) { }

        /// <summary>
        /// 创建一个 <see cref="WebClientAsync"/> 客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举(警告：请务必保证服务端的大小和客户端一致)</param>
        /// <param name="IsReconnect">是否在与服务器断开后主动重连？ </param>

        ///// <param name="OnlyData">是否启动框架模式</param>
        public WebClientAsync(NetBufferSize bufferSize,/* bool OnlyData, */bool IsReconnect)
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
            //this.DataLength = (int)bufferSize;
            //this.OnlyData = OnlyData;
            this.IsReconnect = IsReconnect;
        }

        #endregion

        #region ConnectAsync

        /// <summary>
        /// 开始异步连接 <see cref="WebSocket"/>
        /// </summary>
        /// <param name="port">端口号</param>
        public async Task ConnectAsync(int port)
        {
            await ConnectAsync(StaticData.LocalIp, port);
        }

        /// <summary>
        ///  开始异步连接 <see cref="WebSocket"/>
        /// </summary>
        /// <param name="ip">可以使用“*”</param>
        /// <param name="port">端口号</param>
        public override async Task ConnectAsync(string ip, int port)
        {
            ip = await WebStateObject.IsWebIpEffective(ip);

            string url = string.Concat(ip, ':', port);

            if (!IPEndPoint.TryParse(url, out _))
            {
                throw new FormatException("ip 无法被 IPEndPoint 对象识别！");
            }

            await ConnectAsync(url);
        }

        /// <summary>
        ///  开始异步连接 <see cref="WebSocket"/>
        /// </summary>
        /// <param name="uriPrefix">高级定义法，例如：0.0.0.0:80/tcp，该格式适用</param>
        public async Task ConnectAsync(string uriPrefix)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(uriPrefix))
            {
                throw new NullReferenceException("uriPrefix 值不了为空！");
            }
            server = uriPrefix;
            await ConnectAsync();
        }

        private async Task ConnectAsync()
        {
            client = new ClientWebSocket();
            string url = string.Concat(IsSSL ? "wss://" : "ws://", server, '/');
            try
            {
                if (InitWebOptions is not null)
                {
                    InitWebOptions(client.Options);
                }

                await client.ConnectAsync(new Uri(url), CancellationToken.None);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (!isWhileReconnect) await ConnectCallBack();
            }
        }

        #endregion

        #region SendAsync

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="msg">要发送的内容</param>
        public async ValueTask<bool> SendAsync(string msg)
        {
            var chars = msg.AsMemory();
            if (chars.IsEmpty) throw new ArgumentNullException(nameof(msg));
            var sendBytes = CreateSendBytes(Encoding.UTF8.GetByteCount(chars.Span));

            try
            {
                Encoding.UTF8.GetBytes(chars.Span, sendBytes.Span);
                return await SendAsync(sendBytes.GetMemory());
            }
            finally
            {
                sendBytes.Dispose();
            }
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="listData">要发送的内容</param>
        public async ValueTask<bool> SendAsync(Memory<byte> listData)
        {
            try
            {
                await SendAsync(listData, EnClient.SendMsg);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="sendBytes">数据包对象</param>
        /// <returns></returns>
        public override async ValueTask SendAsync(SendBytes<WebSocket> sendBytes)
        {
            var buffers = sendBytes.GetMemory();
            await SendAsync(buffers, EnClient.SendMsg);
        }

        /// <summary>
        /// 创建数据包对象
        /// </summary>
        /// <param name="length">数据包大小</param>
        /// <returns></returns>
        public override SendBytes<WebSocket> CreateSendBytes(int length = 0)
        {
            if (client == null) throw new Exception("未调用ConnectAsync函数！");
            if (length == 0) length = DataLength;
            return new SendBytes<WebSocket>(client, length, false);
        }

        private async ValueTask SendAsync(Memory<byte> listData, EnClient en)
        {
            ThrowIfDisposed();

            if (!WebStateObject.IsConnected(client)) throw new Exception("与服务端的连接已断开！");

            await WebStateObject.SendAsync(client, listData, DataLength);
            await OnComplete(in server, en);
            if (EnClient.SendMsg == en) Keep?.ResetTime();
        }
        #endregion

        #region Reconnection

        /// <summary>
        /// 重连，返回是否重连，如果没有断开是不会重连的
        /// </summary>
        public override async Task<bool> Reconnection()
        {
            ThrowIfDisposed();

            try
            {
                if (!isClose)
                {
                    if (!WebStateObject.IsConnected(client))
                    {
                        client.Abort();
                        client.Dispose();

                        if (Reconnect is not null)
                        {
                            var userKey = await Reconnect.Invoke(server);
                            if (!userKey.IsEmpty)
                            {
                                server = userKey;
                            }
                        }

                        await ConnectAsync();
                        return WebStateObject.IsConnected(client);
                    }
                    else
                    {
                        return true;
                    }
                }
            }
            catch
            {
                InsideClose();
            }
            return false;
        }

        private async Task WhileReconnect()
        {
            while (IsReconnect)
            {
                if (await Reconnection())
                {
                    isWhileReconnect = false;
                    await ConnectCallBack();
                    break;
                }
                await Task.Delay(100); //等待一下才继续
            }
        }

        private async Task<bool> StartReconnect()
        {
            if (IsReconnect && !isWhileReconnect)
            {
                isWhileReconnect = true;
                await OnComplete(Server, EnClient.Reconnect);
                StateObject.StartTask("Web重连", WhileReconnect);
                return true;
            }
            return false;
        }

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
                        await SendAsync(KeepAlive.KeepAliveObj, EnClient.HeartBeat);
                    });
                    return;
                }
            }
            throw new Exception("心跳已经开启啦，请勿重复操作！");
        }

        /**
        * 异步接收连接的回调函数
        */
        private async Task ConnectCallBack()
        {
            if (WebStateObject.IsConnected(client))
            {
                UserKey key = server;
                //Debug.WriteLine("服务器：{0},连接成功！ {1}", key, DateTime.Now.ToString());

                StateObject.StartReceive("Web", StartReceive, key); //StartReceive(key);
            }
            else
            {
                InsideClose();
                await OnComplete(in server, EnClient.Fail);
                await StartReconnect();
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private async Task StartReceive(UserKey key)
        {
            isReceive = true;
            WebStateObject obj = new(in key, client, this.DataLength);// { doReceive = doReceive };
            await OnComplete(in key, EnClient.Connect);
            while (!isClose)//ListClient.TryGetValue(key, out client) && 不允许意外删除对象问题
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
                    await OnComplete(in key, EnClient.Close);
                    if (!await StartReconnect()) isClose = true;
                    break;
                }
            }
            obj.Close();
        }

        /**
         * 开始异步接收数据
         * obj 要接收的客户端包体
         */
        private async ValueTask ReceiveAsync(WebStateObject obj)
        {
            //obj.doReceive.Reset(); //System.IO.Pipelines.PipeWriter
            try
            {
                //obj.SocketClient.BeginReceive(obj.ListData, obj.WriteIndex, obj.ListData.Length - obj.WriteIndex, SocketFlags.None, ReceiveCallBack, obj);

                if (await obj.ReceiveAsync())
                {
                    await OnComplete(obj.SocketKey, EnClient.Receive);
                    Keep?.ResetTime();
                    await obj.OnReceivedAsync(IsThreadPool, obj.Client, Received);
                    //await Received.InvokeAsync(obj.IpPort, ListData);
                }

                //ValueWebSocketReceiveResult receiveResult = await obj.Client.ReceiveAsync(obj.ListData, CancellationToken.None);

                //if (receiveResult.EndOfMessage)
                //{
                //    if (receiveResult.MessageType == WebSocketMessageType.Close)
                //    {
                //        //Debug.WriteLine($"客户端：{obj.IpPort}，已经断开！");
                //        obj.Client.Abort();
                //        //obj.doReceive.Set();
                //    }
                //    else
                //    {
                //        if (!DisabledReceive) OnComplete(obj.SocketKey, EnClient.Receive);
                //        Keep?.ResetTime();
                //        await obj.OnReceivedAsync(IsThreadPool, obj.Client, receiveResult.Count, Received);
                //        //await Received.InvokeAsync(obj.IpPort, ListData);
                //    }
                //}

                //obj.doReceive.Set();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                obj.Client.Abort();
                //obj.doReceive.Set();
            }
            //obj.doReceive.WaitOne();
        }

        /// <summary>
        /// 事件方法
        /// </summary>
        /// <param name="key">指定发送对象</param>
        /// <param name="enAction">消息类型</param>
        public override ValueTask<IGetQueOnEnum> OnComplete(in UserKey key, EnClient enAction)
        {
            if (IsEvent(enAction))
            {
                return EnumEventQueue.OnComplete(key, enAction, IsQueue(enAction), Completed);
            }
            return IGetQueOnEnum.SuccessAsync;
        }

        /// <summary>
        /// TCP关闭
        /// </summary>
        void InsideClose()
        {
            client?.Abort();
        }

        /// <summary>
        /// TCP关闭
        /// </summary>
        public override void Close()
        {
            IsReconnect = false;
            InsideClose();
            Keep?.Close();
        }

        /// <summary>
        /// 回收UDP相关资源
        /// </summary>
        public override void Dispose()
        {
            _disposed = true;
            Close();
            client?.Dispose();
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
