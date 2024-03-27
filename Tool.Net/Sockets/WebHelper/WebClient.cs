using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Server.HttpSys;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.NetFrame.Internal;

namespace Tool.Sockets.WebHelper
{
    public sealed class WebClient : INetworkConnect<WebSocket>
    {
        /*** 锁 */
        private static readonly object Lock = new();

        /// <summary>
        /// 获取当前心跳信息
        /// </summary>
        public KeepAlive Keep { get; private set; }

        private readonly int DataLength = 1024 * 8;
        private ClientWebSocket client;

        //用于控制异步接受连接
        //private readonly ManualResetEvent doConnect = new(false);
        //标识服务端连接是否关闭
        private bool isClose = false;

        ///// <summary>
        ///// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        ///// </summary>
        //public bool OnlyData { get; }

        /// <summary>
        /// 标识服务端连接是否关闭
        /// </summary>
        public bool IsClose { get { return isClose; } }

        /// <summary>
        /// 获取指示是否使用安全套接字层 (SSL) 保护 WebSocket 连接的值。
        /// </summary>
        /// <remarks>true 如果使用 SSL; 保护 WebSocket 连接，否则为false。</remarks>
        public bool IsSSL { get; init; } = false;

        private string server = string.Empty;//服务端IP
        private int millisecond = 20; //默认20毫秒。

        /// <summary>
        /// 是否使用线程池调度接收后的数据
        /// 默认 true 开启
        /// </summary>
        public bool IsThreadPool { get; init; } = true;

        /// <summary>
        /// 是否在与服务器断开后主动重连？ 
        /// </summary>
        public bool IsReconnect { get; private set; }

        /// <summary>
        /// 服务器创建时的信息
        /// </summary>
        public string Server { get { return server; } }

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
        public string LocalPoint => throw new NotImplementedException("WebSocket无法获取");

        /// <summary>
        /// 获取当前是否已连接到远程主机。
        /// </summary>
        public bool Connected => client.State == WebSocketState.Open;

        /// <summary>
        /// 禁用掉Receive通知事件，方便上层封装
        /// </summary>
        public bool DisabledReceive { get; init; } = false;

        /**
         * 提供自定义注册的服务
         */
        private Action<ClientWebSocketOptions> InitWebOptions;

        /**
         * 连接、发送、关闭事件
         */
        private Func<string, EnClient, DateTime, Task> Completed; //event

        /**
         * 接收到数据事件
         */
        private Func<ReceiveBytes<WebSocket>, Task> Received; //event

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
        public void SetInitWebContext(Action<ClientWebSocketOptions> initWebOptions)
        {
            this.InitWebOptions ??= initWebOptions;
        }

        /// <summary>
        /// 连接、发送、关闭事件 <see cref="EnClient"/>
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(Func<string, EnClient, DateTime, Task> Completed)
        {
            this.Completed ??= Completed;
        }

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public void SetReceived(Func<ReceiveBytes<WebSocket>, Task> Received)
        {
            this.Received ??= Received;
        }

        #region WebClient

        /// <summary>
        /// 创建一个 WebClient 客户端类
        /// </summary>
        public WebClient() : this(false)
        {

        }

        /// <summary>
        /// 创建一个 WebClient 客户端类
        /// </summary>
        /// <param name="IsReconnect">设置是否重连</param>
        public WebClient(bool IsReconnect) : this(8 * 1024, IsReconnect)
        {

        }

        /// <summary>
        /// 创建一个 WebClient 客户端类，设置流大小
        /// </summary>
        /// <param name="DataLength">每次包的最大大小(警告：请务必保证服务端的大小和客户端一致)</param>
        /// <param name="IsReconnect">设置是否重连</param>
        public WebClient(int DataLength, bool IsReconnect)
        {
            if (DataLength < 8 * 1024)
            {
                throw new ArgumentException("DataLength 值必须大于8KB！", nameof(DataLength));
            }
            if (DataLength > 1024 * 1024 * 20)
            {
                throw new ArgumentException("DataLength 值必须是在20M(DataLength < 1024 * 1024 * 20)以内！", nameof(DataLength));
            }
            this.DataLength = DataLength;
            this.IsReconnect = IsReconnect;
        }

        #endregion

        #region ConnectAsync

        /// <summary>
        /// 开始异步连接 WebSocket
        /// </summary>
        /// <param name="port">端口号</param>
        public void ConnectAsync(int port)
        {
            ConnectAsync(StaticData.LocalIp, port);
        }

        /// <summary>
        ///  开始异步连接 WebSocket
        /// </summary>
        /// <param name="ip">可以使用“*”</param>
        /// <param name="port">端口号</param>
        public async void ConnectAsync(string ip, int port)
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
        ///  开始异步连接 WebSocket
        /// </summary>
        /// <param name="uriPrefix">高级定义法，例如：0.0.0.0:80/tcp，该格式适用</param>
        public async Task ConnectAsync(string uriPrefix)
        {
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
                ConnectCallBack();
            }
        }

        #endregion

        #region SendAsync

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="msg">要发送的内容</param>
        public async Task<bool> SendAsync(string msg)
        {
            byte[] listData = Encoding.UTF8.GetBytes(msg);
            return await SendAsync(listData, false);
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="listData">要发送的内容</param>
        /// <param name="isbyte">发送数据是文本还是字节流</param>
        public async Task<bool> SendAsync(byte[] listData, bool isbyte)
        {
            Memory<byte> _data = new(listData);

            if (client == null)
            {
                throw new ArgumentException("client 对象是空的！", nameof(client));
            }
            if (!WebStateObject.IsConnected(client))
            {
                throw new Exception("与服务端的连接已断开！");
            }

            try
            {
                await client.SendAsync(_data, isbyte ? WebSocketMessageType.Binary : WebSocketMessageType.Text, true, CancellationToken.None);
                OnComplete(Server, EnClient.SendMsg);

                return true;
            }
            catch (Exception)
            {
                return false;
                //OnComplete(key.IpPort, EnSocketAction.SendMsg);
            }
        }

        #endregion

        #region Reconnection

        /// <summary>
        /// 重连，返回是否重连，如果没有断开是不会重连的
        /// </summary>
        public async Task<bool> Reconnection()
        {
            try
            {
                if (!isClose)
                {
                    if (!WebStateObject.IsConnected(client))
                    {
                        client.Abort();
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

        private async void WhileReconnect()
        {
            //Task.Factory.StartNew(() =>
            //{
            Thread.CurrentThread.Name ??= "WebSocket客户端-重连";
            while (IsReconnect)
            {
                if (await Reconnection()) break;
                await Task.Delay(100); //等待一下才继续
            }
            //}, TaskCreationOptions.LongRunning);
        }

        #endregion

        /// <summary>
        /// 添加持久化消息（心跳），防止特殊情况下的断开连接
        /// </summary>
        public void AddKeepAlive(byte TimeInterval)
        {
            lock (Lock)
            {
                if (Keep == null)
                {
                    Keep = new KeepAlive(TimeInterval, async () =>
                    {
                        await SendAsync(KeepAlive.KeepAliveObj, true);
                        OnComplete(server, EnClient.HeartBeat);
                    });
                    return;
                }
            }
            throw new Exception("心跳已经开启啦，请勿重复操作！");
        }

        /**
        * 异步接收连接的回调函数
        */
        private void ConnectCallBack()
        {
            if (WebStateObject.IsConnected(client))
            {
                string key = server;
                Debug.WriteLine("服务器：{0},连接成功！ {1}", key, DateTime.Now.ToString());
                StartReceive(key, client);
                OnComplete(key, EnClient.Connect);
            }
            else
            {
                InsideClose();
                OnComplete(server, EnClient.Fail);

                if (this.IsReconnect)
                {
                    WhileReconnect();
                }
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private async void StartReceive(string key, WebSocket client)
        {
            WebStateObject obj = new(key, client, this.DataLength);// { doReceive = doReceive };
            while (!isClose)//ListClient.TryGetValue(key, out client) && 不允许意外删除对象问题
            {
                if (WebStateObject.IsConnected(obj.Client))
                {
                    await Task.Delay(Millisecond); //Thread.Sleep(Millisecond);
                    await ReceiveAsync(obj);
                    //await Task.Delay(Millisecond); //Thread.Sleep(Millisecond);
                }
                else
                {
                    //如果发生异常，说明客户端失去连接，触发关闭事件
                    InsideClose();
                    OnComplete(key, EnClient.Close);
                    if (this.IsReconnect) WhileReconnect();
                    break;
                }
            }
            obj.Close();
        }

        /**
         * 开始异步接收数据
         * obj 要接收的客户端包体
         */
        private async Task ReceiveAsync(WebStateObject obj)
        {
            //obj.doReceive.Reset();
            if (WebStateObject.IsConnected(obj.Client))
            {
                try
                {
                    //obj.SocketClient.BeginReceive(obj.ListData, obj.WriteIndex, obj.ListData.Length - obj.WriteIndex, SocketFlags.None, ReceiveCallBack, obj);

                    ValueWebSocketReceiveResult receiveResult = await obj.Client.ReceiveAsync(obj.ListData, CancellationToken.None);

                    if (receiveResult.EndOfMessage)
                    {
                        if (receiveResult.MessageType == WebSocketMessageType.Close)
                        {
                            //Debug.WriteLine($"客户端：{obj.IpPort}，已经断开！");
                            obj.Client.Abort();
                            //obj.doReceive.Set();
                        }
                        else
                        {
                            //byte[] ListData = new byte[receiveResult.Count];
                            //Array.Copy(obj.ListData.Array, 0, ListData, 0, receiveResult.Count);

                            if (!DisabledReceive) OnComplete(obj.SocketKey, EnClient.Receive);

                            if (Received is not null)
                            {
                                obj.OnReceived(IsThreadPool, obj.Client, receiveResult.Count, Received);

                                //await Received.InvokeAsync(obj.IpPort, ListData);
                            }
                        }
                    }

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
        }

        /// <summary>
        /// 事件方法
        /// </summary>
        /// <param name="key">指定发送对象</param>
        /// <param name="enAction">消息类型</param>
        internal void OnComplete(string key, EnClient enAction)
        {
            if (Completed is null) return;
            EnumEventQueue.OnComplete(key, enAction, completed);
            void completed(string age0, Enum age1, DateTime age2)
            {
                Completed(age0, (EnClient)age1, age2).Wait();
            }
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
        public void Close()
        {
            IsReconnect = false;
            isClose = true;
            InsideClose();
            Keep.Close();
        }

        public void Dispose()
        {
            Close();
            client.Dispose();
            //doConnect.Close();
            //_mre.Close();
        }

        public void SendAsync(params ArraySegment<byte>[] listData)
        {
            throw new NotImplementedException();
        }

        public void Send(params ArraySegment<byte>[] listData)
        {
            throw new NotImplementedException();
        }
    }
}
