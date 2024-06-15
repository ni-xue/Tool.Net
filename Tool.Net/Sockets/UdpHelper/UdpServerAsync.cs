using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.UdpHelper
{
    /// <summary>
    /// 封装一个底层异步Udp对象（服务端）IpV4
    /// </summary>
    //[Obsolete("UDP方案，存在问题，升级改动中", true)]
    public class UdpServerAsync : INetworkListener<UdpEndPoint>
    {
        private readonly int DataLength;// = 1024 * 8;
        private Socket listener;
        private bool isClose = false; //标识服务端连接是否关闭
        private bool isReceive = false; //标识是否调用了接收函数
        private readonly ConcurrentDictionary<UserKey, UdpEndPoint> listClient = new();

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        /// <summary>
        /// 标识服务端连接是否关闭
        /// </summary>
        public bool IsClose { get { return isClose; } }

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
        /// 回复消息延迟时间（警告：当前设置仅在开启了OnlyData模式生效，超时未回复会重发，重发最大次数10，依然没有回复将抛出异常！）小于1将不生效使用默认值
        /// </summary>
        public int ReplyDelay { get; init; } = 100;

        /// <summary>
        /// 表示通讯的包大小
        /// </summary>
        public NetBufferSize BufferSize { get; } = NetBufferSize.Size8K;

        /// <summary>
        /// 已建立连接的集合
        /// key:UserKey
        /// value:UdpEndPoint
        /// </summary>
        public IReadOnlyDictionary<UserKey, UdpEndPoint> ListClient => listClient;

        private Ipv4Port server; //服务端IP
        private UdpEndPoint endPointServer;
        private int millisecond = 20; //默认20毫秒。
        private int receiveTimeout = 60000; //默认60000毫秒。
        private ArraySegment<byte> arrayData;//一个连续的内存块

        /// <summary>
        /// 服务器创建时的信息
        /// </summary>
        public UserKey Server { get { return server; } }

        /// <summary>
        /// <para>监听每个连接用户的最大等待时长（默认60秒一直等待）</para>
        /// <para>不得小于5秒</para>
        /// </summary>
        public int ReceiveTimeout
        {
            get => receiveTimeout; init
            {
                if (value < 5000) throw new Exception("设置的等待时长小于5秒。");
                receiveTimeout = value;
            }
        }

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

        /**
         * 连接、发送、关闭事件
         */
        private CompletedEvent<EnServer> Completed; //event

        /**
         * 接收到数据事件
         */
        private ReceiveEvent<UdpEndPoint> Received; //event

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(CompletedEvent<EnServer> Completed) => this.Completed ??= Completed;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public void SetReceived(ReceiveEvent<UdpEndPoint> Received)
        {
            if (isReceive) throw new Exception("当前已无法绑定接收委托了，因为StartAsync()已经调用了。");
            this.Received ??= Received;
        }

        /// <summary>
        /// 根据IP:Port获取对应的连接对象
        /// </summary>
        /// <param name="key">IP:Port</param>
        /// <param name="client">连接对象</param>
        /// <returns>返回成功状态</returns>
        public bool TrySocket(in UserKey key, out UdpEndPoint client) => ListClient.TryGetValue(key, out client);

        #region UdpServerAsync

        /// <summary>
        /// 创建一个 <see cref="UdpServerAsync"/> 服务器类
        /// </summary>
        public UdpServerAsync() : this(NetBufferSize.Default) { }

        /// <summary>
        /// 创建一个 <see cref="UdpServerAsync"/> 服务器类，并确定是否开启框架验证模式保证数据唯一性。
        /// </summary>
        /// <param name="size">数据缓冲区大小</param>
        public UdpServerAsync(NetBufferSize size) : this(size, false) { }

        /// <summary>
        /// 创建一个 <see cref="UdpServerAsync"/> 服务器类，确认模式和设置流大小
        /// </summary>
        /// <param name="size">数据缓冲区大小</param>
        /// <param name="OnlyData">是否启动框架模式</param>
        public UdpServerAsync(NetBufferSize size, bool OnlyData)
        {
            if (NetBufferSize.Default == size) size = NetBufferSize.Size8K;
            //if (DataLength < 8 * 1024)
            //{
            //    throw new ArgumentException("DataLength 值必须大于8KB！", nameof(DataLength));
            //}
            //if (DataLength > 1024 * 1024 * 20)
            //{
            //    throw new ArgumentException("DataLength 值必须是在20M(DataLength < 1024 * 1024 * 20)以内！", nameof(DataLength));
            //}
            int dataLength = (int)size;
            if (size > NetBufferSize.Size32K) dataLength = (int)NetBufferSize.Size32K;
            this.BufferSize = size;
            this.DataLength = dataLength;
            this.OnlyData = OnlyData;
            this.arrayData = new byte[dataLength + (OnlyData ? StateObject.HeadSize : 0)];
        }

        #endregion

        #region StartAsync

        /// <summary>
        /// 开始异步监听本机127.0.0.1的端口号
        /// </summary>
        /// <param name="port"></param>
        public async Task StartAsync(int port)
        {
            await StartAsync(StaticData.LocalIp, port);
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public async Task StartAsync(string ip, int port)
        {
            ThrowIfDisposed();

            if (!UdpEndPoint.TryParse(ip, port, out endPointServer))
            {
                throw new FormatException("ip:port 无法被 IPEndPoint 对象识别！");
            }
            server = $"{ip}:{port}";
            StartAsync();

            await Task.CompletedTask;
        }

        private void StartAsync()
        {
            listener = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            listener.ReceiveBufferSize = (int)this.BufferSize;
            listener.SendBufferSize = (int)this.BufferSize;

            try
            {
                listener.Bind(endPointServer);
                OnComplete(Server, EnServer.Create);
            }
            catch (Exception e)
            {
                OnComplete(Server, EnServer.Fail);
                throw new Exception("监听服务器时发生异常！", e);
            }

            StateObject.StartReceive("Udp", StartReceive, Server); //StartReceive();

            //try
            //{
            //    while (!isClose)
            //    {
            //        //Thread.CurrentThread.Name ??= $"Tcp服务端-监听({server})";
            //        var context = await listener.AcceptAsync();
            //        AcceptCallBack(context);
            //    }
            //}
            //catch (Exception)
            //{
            //    Stop();//出错可能是监听断开等
            //    if (isClose)
            //    {
            //        //doConnect.Set();
            //        foreach (var _client in listClient)
            //        {
            //            SocketAbort(_client.Key, _client.Value);
            //        }
            //        listClient.Clear();
            //        OnComplete(server, EnServer.Close);
            //        listener.Close();
            //        return;
            //    }
            //}
        }

        #endregion

        #region SendAsync

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端的ip地址和端口号</param>
        /// <param name="msg">要发送的内容</param>
        public async ValueTask SendAsync(Ipv4Port key, string msg)
        {
            if (ListClient.TryGetValue(key, out UdpEndPoint client))
            {
                await SendAsync(client, msg);
            }
            else
            {
                throw new Exception("在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。");
            }
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端的ip地址和端口号</param>
        /// <param name="listData">要发送的内容，允许多个包</param>
        public async ValueTask SendAsync(Ipv4Port key, ArraySegment<byte> listData)
        {
            if (ListClient.TryGetValue(key, out UdpEndPoint client))
            {
                await SendAsync(client, listData);
            }
            else
            {
                throw new Exception("在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。");
            }
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="client">UdpEndPoint对象</param>
        /// <param name="msg">要发送的内容</param>
        public async ValueTask SendAsync(UdpEndPoint client, string msg)
        {
            byte[] listData = Encoding.UTF8.GetBytes(msg);
            await SendAsync(client, listData);
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="client">UdpEndPoint对象</param>
        /// <param name="listData">要发送的内容，允许多个包</param>
        public async ValueTask SendAsync(UdpEndPoint client, ArraySegment<byte> listData)// byte[] listData
        {
            var sendBytes = CreateSendBytes(client, listData.Count);

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
        /// 开始异步发送数据
        /// </summary>
        /// <param name="sendBytes">数据包对象</param>
        /// <returns></returns>
        public async ValueTask SendAsync(SendBytes<UdpEndPoint> sendBytes)
        {
            await UdpEndPoint.ShareSendAsync(sendBytes, OnlyData, ReplyDelay, SendNoWaitAsync);
            OnComplete(sendBytes.Client, EnServer.SendMsg);
        }

        private async ValueTask SendNoWaitAsync(UdpEndPoint point, Memory<byte> buffers)
        {
            ThrowIfDisposed();
            if (!UdpStateObject.IsConnected(listener)) throw new Exception("服务端已关闭！");
            await UdpStateObject.SendNoWaitAsync(listener, point, buffers);
            point.UpDateSignal();
        }

        /// <summary>
        /// 创建数据包对象
        /// </summary>
        /// <param name="key">通信IP:Port</param>
        /// <param name="length">数据包大小</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SendBytes<UdpEndPoint> CreateSendBytes(Ipv4Port key, int length)
        {
            if (TrySocket(key, out UdpEndPoint client))
            {
                return CreateSendBytes(client, length);
            }
            else
            {
                throw new Exception("在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。");//在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。
            }
        }

        /// <summary>
        /// 创建数据包对象
        /// </summary>
        /// <param name="client">通信对象</param>
        /// <param name="length">数据包大小</param>
        /// <returns></returns>
        public SendBytes<UdpEndPoint> CreateSendBytes(UdpEndPoint client, int length = 0)
        {
            if (client is null) throw new ArgumentException("UdpEndPoint不能为空！", nameof(client));
            if (length > (int)NetBufferSize.Size32K) throw new ArgumentException("Udp协议下只能 最大支持到Size32K！", nameof(length));
            if (length == 0) length = DataLength;
            return new SendBytes<UdpEndPoint>(client, length, OnlyData);
        }

        #endregion

        /**
         * 异步接收连接的回调函数
         */
        private void AcceptCallBack(EndPoint client, out UdpEndPoint udpEndPoint, out UdpStateObject obj)
        {
            UserKey key = StateObject.GetIpPort(client);
            udpEndPoint = listClient.AddOrUpdate(key, add, update);
            obj = udpEndPoint.UdpState;
            obj.UpDateSignal();

            UdpEndPoint add(UserKey key)
            {
                var point = client as UdpEndPoint;
                OnComplete(in key, EnServer.Connect).Wait();
                point.SetUdpState(this.DataLength, this.OnlyData, Received);
                return point;
            }

            UdpEndPoint update(UserKey key, UdpEndPoint point)
            {
                return point;
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private async Task StartReceive(UserKey server)//string key, IPEndPoint client
        {
            isReceive = true;
            KeepAlive Keep = new(1, () =>
            {
                foreach (var keyValue in listClient.ToArray())
                {
                    if (!keyValue.Value.IsOnLine(receiveTimeout))
                    {
                        if (listClient.TryRemove(keyValue))
                        {
                            SocketAbort(keyValue.Key, keyValue.Value.UdpState);
                        }
                    }
                }
                return Task.CompletedTask;
            });

            while (!isClose)
            {
                await Task.Delay(Millisecond);
                if (UdpStateObject.IsConnected(listener))
                {
                    await ReceiveAsync();
                }
                else
                {
                    break;
                }
            }

            Keep.Close();
            OnComplete(in server, EnServer.Close);
            ListenerClose();
        }

        /**
         * 开始异步接收数据
         * obj 要接收的客户端包体
         */
        private async Task ReceiveAsync()
        {
            //obj.doReceive.Reset();
            try
            {
                SocketReceiveFromResult result = await UdpStateObject.ReceiveFromAsync(listener, arrayData, endPointServer);
                AcceptCallBack(result.RemoteEndPoint, out var udpEndPoint, out var obj);

                var head = arrayData[..StateObject.HeadSize];
                if (obj.OnReceiveTask(head, result.ReceivedBytes, out bool isreply, out bool isReceive))//尝试使用，原线程处理包解析，验证效率
                {
                    if (isreply) await SendNoWaitAsync(obj.Point, head);
                    if (isReceive) OnReceived(arrayData[..result.ReceivedBytes], obj);
                }
            }
            catch (Exception) { }

            void OnReceived(ArraySegment<byte> bytes, UdpStateObject obj)
            {
                if (obj.IsKeepAlive(bytes))
                {
                    OnComplete(obj.IpPort, EnServer.HeartBeat);
                }
                else
                {
                    if (!DisabledReceive) OnComplete(obj.IpPort, EnServer.Receive);
                    obj.OnReceive(IsThreadPool, bytes);
                }
            }
        }

        /// <summary>
        /// 可供开发重写的实现类
        /// </summary>
        /// <param name="key">指定发送对象</param>
        /// <param name="enAction">消息类型</param>
        public virtual IGetQueOnEnum OnComplete(in UserKey key, EnServer enAction) => EnumEventQueue.OnComplete(in key, enAction, Completed);

        private void OnComplete(UdpEndPoint key, EnServer enAction)
        {
            OnComplete(StateObject.GetIpPort(key), enAction);
        }

        //private void TimeoutClose() 
        //{
        //    foreach (var item in listClient)
        //    {
        //        SocketAbort(item.Key, item.Value.UdpState);
        //    }
        //}

        private void ListenerClose()
        {
            listener.Close();
            foreach (var item in listClient)
            {
                SocketAbort(item.Key, item.Value.UdpState);
            }
            listClient.Clear();
        }

        private void SocketAbort(in UserKey key, UdpStateObject _client)
        {
            _client.Close();
            OnComplete(key, EnServer.ClientClose);
        }

        /// <summary>
        /// UDP关闭
        /// </summary>
        public void Stop()
        {
            isClose = true;
            listener?.Dispose();//当他不在监听，就关闭监听。
        }

        /// <summary>
        /// 关闭连接，回收相关资源
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            Stop();
            listClient.Clear();
            arrayData = null;

            //listClient = null;
            //listener.Server.Dispose();
            //((IDisposable)listener.Server).Dispose();
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
