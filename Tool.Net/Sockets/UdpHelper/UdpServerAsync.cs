using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Intrinsics.Arm;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Tool.Sockets.Kernels;
using Tool.Sockets.UdpHelper.Extend;

namespace Tool.Sockets.UdpHelper
{
    /// <summary>
    /// 封装一个底层异步Udp对象（服务端）IpV4
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    //[Obsolete("UDP方案，存在问题，升级改动中", true)]
    public class UdpServerAsync : NetworkListener<IUdpCore>
    {
        private readonly int DataLength;// = 1024 * 8;
        private Socket listener;
        private bool isClose = false; //标识服务端连接是否关闭
        private bool isReceive = false; //标识是否调用了接收函数
        private readonly ConcurrentDictionary<UserKey, IUdpCore> listClient = new();

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        /// <summary>
        /// 标识服务端连接是否关闭
        /// </summary>
        public override bool IsClose { get { return isClose; } }

        /// <summary>
        /// 回复消息延迟时间（警告：当前设置仅在开启了OnlyData模式生效，超时未回复会重发，重发最大次数10，依然没有回复将抛出异常！）小于20将不生效使用默认值
        /// </summary>
        public int ReplyDelay { get; init; } = 100;

        /// <summary>
        /// 已建立连接的集合
        /// key:UserKey
        /// value:UdpEndPoint
        /// </summary>
        public override IReadOnlyDictionary<UserKey, IUdpCore> ListClient => listClient;

        private Ipv4Port server; //服务端IP
        private UdpEndPoint endPointServer;
        private int millisecond = 20; //默认20毫秒。
        private int receiveTimeout = 60000; //默认60000毫秒。
        private Memory<byte> arrayData;//一个连续的内存块

        /// <summary>
        /// 服务器创建时的信息
        /// </summary>
        public override UserKey Server { get { return server; } }

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

        /**
         * 连接、发送、关闭事件
         */
        private CompletedEvent<EnServer> Completed; //event

        /**
         * 接收到数据事件
         */
        private ReceiveEvent<IUdpCore> Received; //event

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public override void SetCompleted(CompletedEvent<EnServer> Completed) => this.Completed ??= Completed;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public override void SetReceived(ReceiveEvent<IUdpCore> Received)
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
        public override bool TrySocket(in UserKey key, out IUdpCore client) => ListClient.TryGetValue(key, out client);

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
            if (!OnlyData && size > NetBufferSize.Size32K)
            {
                if (size > NetBufferSize.Size64K) throw new ArgumentException($"Udp协议下只能 最大支持到Size64K，{UdpPack.MaxBuffer}B！", nameof(size));
                dataLength = UdpPack.MaxBuffer; //最大发送区和接收区 IP20 UDP8 （保留最少）12
            }
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
        public override async Task StartAsync(string ip, int port)
        {
            ThrowIfDisposed();

            if (!UdpEndPoint.TryParse(ip, port, out endPointServer))
            {
                throw new FormatException("ip:port 无法被 IPEndPoint 对象识别！");
            }
            server = $"{ip}:{port}";
            await StartAsync();

            //await Task.CompletedTask;
        }

        private async Task StartAsync()
        {
            listener = StateObject.CreateSocket(false, BufferSize);

            try
            {
                listener.Bind(endPointServer);
                await OnComplete(Server, EnServer.Create);
            }
            catch (Exception e)
            {
                await OnComplete(Server, EnServer.Fail);
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
            if (ListClient.TryGetValue(key, out IUdpCore client))
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
            if (ListClient.TryGetValue(key, out IUdpCore client))
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
        public async ValueTask SendAsync(IUdpCore client, string msg)
        {
            var chars = msg.AsMemory();
            if (chars.IsEmpty) throw new ArgumentNullException(nameof(msg));
            var sendBytes = CreateSendBytes(client, Encoding.UTF8.GetByteCount(chars.Span));

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
        /// 开始异步发送数据
        /// </summary>
        /// <param name="client">UdpEndPoint对象</param>
        /// <param name="listData">要发送的内容，允许多个包</param>
        public async ValueTask SendAsync(IUdpCore client, ArraySegment<byte> listData)// byte[] listData
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
        public override async ValueTask SendAsync(SendBytes<IUdpCore> sendBytes)
        {
            var udp = sendBytes.Client;
            bool ispart = false;
            int i = 0;
            do
            {
                var memory = udp.GetSendMemory(sendBytes, ref ispart, ref i);
                await SendNoWaitAsync(udp, memory);
            } while (ispart);
            await OnComplete(udp.Ipv4, EnServer.SendMsg);
        }

        private async ValueTask SendNoWaitAsync(IUdpCore udp, Memory<byte> buffers)
        {
            ThrowIfDisposed();
            await udp.SendAsync(buffers);
        }

        /// <summary>
        /// 创建数据包对象
        /// </summary>
        /// <param name="key">通信IP:Port</param>
        /// <param name="length">数据包大小</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public SendBytes<IUdpCore> CreateSendBytes(Ipv4Port key, int length)
        {
            if (TrySocket(key, out IUdpCore client))
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
        public override SendBytes<IUdpCore> CreateSendBytes(IUdpCore client, int length = 0)
        {
            if (client is null) throw new ArgumentException("UdpEndPoint不能为空！", nameof(client));
            if (!OnlyData && length > DataLength) throw new ArgumentException($"Udp协议下文报只能 最大支持到{DataLength}B！（这与你设置的 NetBufferSize 枚举有关！）", nameof(length));
            if (length == 0) length = DataLength;
            return new SendBytes<IUdpCore>(client, length, OnlyData);
        }

        #endregion

        /**
         * 异步接收连接的回调函数 （返回是否是新连接）
         */
        private bool AcceptCallBack(EndPoint client, out IUdpCore udpCore, out UdpStateObject obj)
        {
            bool isadd = false;
            if (client is UdpEndPoint point)
            {
                UserKey key = point.Ipv4; //StateObject.GetIpPort(client);
                //udpCore = listClient.AddOrUpdate(key, add, update);
                udpCore = listClient.GetOrAdd(key, add, listener);
                obj = udpCore.UdpState;
                obj.UpDateSignal();

                IUdpCore add(UserKey key, Socket socket)
                {
                    isadd = true;
                    //point.SetUdpState(this.DataLength, this.OnlyData, Received);
                    return IUdpCore.GetUdpCore(this, point, socket, DataLength, OnlyData, ReplyDelay, true, false, IsReceived, Received);
                }

                return isadd;
            }
            else
            {
                throw new Exception($"意料外的异常错误，EndPoint 类型不匹配。");
            }

            async ValueTask IsReceived(UserKey key, byte type)
            {
                switch (type)
                {
                    case 0:
                        await OnComplete(key, EnServer.HeartBeat);
                        break;
                    case 1:
                        await OnComplete(key, EnServer.Receive);
                        break;
                    case 2:
                        await OnComplete(in key, EnServer.Connect);
                        break;
                }
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private async Task StartReceive(UserKey server)//string key, IPEndPoint client
        {
            isReceive = true;
            KeepAlive Keep = new(1000, async () =>
            {
                //var data = from a in listClient where a.Value.IsOnLine(receiveTimeout) select listClient;
                foreach (var udp in listClient.Values.ToArray())
                {
                    if (!udp.IsOnLine(receiveTimeout))
                    {
                        await udp.DisposeAsync();
                    }
                }
            });

            while (!isClose)
            {
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
            await OnComplete(in server, EnServer.Close);
            await ListenerClose();
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
                bool newuser = AcceptCallBack(result.RemoteEndPoint, out var udp, out var obj);

                if (newuser && OnlyData) //需要验证协议，不一致时关闭
                {
                    if (await Handshake.UdpAutograph(udp.Socket, udp.EndPoint, arrayData) is false)
                    {
                        await ClientCloes(new KeyValuePair<UserKey, IUdpCore>(udp.Ipv4, udp)); //失败后，销毁记录
                    }
                    return; //跳出业务
                }

                await udp.ReceiveAsync(arrayData[..result.ReceivedBytes]);

                //var head = arrayData[..StateObject.HeadSize];
                //if (obj.OnReceiveTask(head, result.ReceivedBytes, out bool isreply, out bool isReceive))//尝试使用，原线程处理包解析，验证效率
                //{
                //    if (isreply) await SendNoWaitAsync(obj.Udp, head);
                //    if (isReceive) await OnReceived(arrayData[..result.ReceivedBytes], obj);
                //}
            }
            catch (Exception) { }
        }

        /// <summary>
        /// 可供开发重写的实现类
        /// </summary>
        /// <param name="key">指定发送对象</param>
        /// <param name="enAction">消息类型</param>
        public override ValueTask<IGetQueOnEnum> OnComplete(in UserKey key, EnServer enAction)
        {
            if (IsEvent(enAction))
            {
                return EnumEventQueue.OnComplete(key, enAction, IsQueue(enAction), Completed);
            }
            return IGetQueOnEnum.SuccessAsync;
        }

        //private void TimeoutClose() 
        //{
        //    foreach (var item in listClient)
        //    {
        //        SocketAbort(item.Key, item.Value.UdpState);
        //    }
        //}

        private async ValueTask ListenerClose()
        {
            listener.Close();
            foreach (var item in listClient)
            {
                await SocketAbort(item.Key, item.Value);
            }
            listClient.Clear();
        }

        private async ValueTask SocketAbort(UserKey key, IUdpCore _client)
        {
            await _client.DisposeAsync();
            await OnComplete(key, EnServer.ClientClose);
        }

        internal async ValueTask ClientCloes(KeyValuePair<UserKey, IUdpCore> pair)
        {
            if (listClient.TryRemove(pair))
            {
                await SocketAbort(pair.Key, pair.Value);
            }
        }

        /// <summary>
        /// UDP关闭
        /// </summary>
        public override void Stop()
        {
            isClose = true;
            listener?.Dispose();//当他不在监听，就关闭监听。
        }

        /// <summary>
        /// 关闭连接，回收相关资源
        /// </summary>
        public override void Dispose()
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
