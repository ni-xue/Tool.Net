using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.WebHelper
{
    /// <summary>
    /// WebServer长连接对象
    /// </summary>
    public sealed class WebServerAsync : INetworkListener<WebSocketContext>
    {
        private readonly int DataLength = 1024 * 8;
        private HttpListener listener;
        private bool isClose = false; //标识服务端连接是否关闭
        private bool isReceive = false; //标识是否调用了接收函数
        private readonly ConcurrentDictionary<UserKey, WebSocketContext> listClient = new();

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

        /// <summary>
        /// 已建立连接的集合
        /// key:UserKey
        /// value:WebSocketContext
        /// </summary>
        public IReadOnlyDictionary<UserKey, WebSocketContext> ListClient => listClient;

        private UserKey server; //服务端IP
        private int millisecond = 20; //默认20毫秒。

        /// <summary>
        /// 服务器创建时的信息
        /// </summary>
        public UserKey Server { get { return server; } }

        /// <summary>
        /// 是否使用线程池调度接收后的数据
        /// 默认 true 开启
        /// </summary>
        public bool IsThreadPool { get; set; } = true;

        /// <summary>
        /// 表示通讯的包大小
        /// </summary>
        public NetBufferSize BufferSize { get; }

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
        /// 禁用掉Receive通知事件，方便上层封装
        /// </summary>
        public bool DisabledReceive { get; init; } = false;

        /**
         * 提供自定义注册的服务
         */
        private Func<HttpListenerContext, Task<HttpListenerWebSocketContext>> InitWebContext;

        /**
         * 连接、发送、关闭事件
         */
        private CompletedEvent<EnServer> Completed; //event

        /**
         * 接收到数据事件
         */
        private ReceiveEvent<WebSocketContext> Received; //event

        /// <summary>
        /// 当新连接，创建时，可以自定义之协议，不实现走默认流程（当实现后，返回null时取消后续业务，请自行关闭连接）
        /// </summary>
        /// <param name="InitWebContext"></param>
        public void SetInitWebContext(Func<HttpListenerContext, Task<HttpListenerWebSocketContext>> InitWebContext) => this.InitWebContext ??= InitWebContext;

        /// <summary>
        /// 连接、发送、关闭事件 <see cref="EnServer"/>
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(CompletedEvent<EnServer> Completed) => this.Completed ??= Completed;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public void SetReceived(ReceiveEvent<WebSocketContext> Received)
        {
            if (isReceive) throw new Exception("当前已无法绑定接收委托了，因为StartAsync()已经调用了。");
            this.Received ??= Received;
        }

        /// <summary>
        /// 根据Key获取对应的连接对象
        /// </summary>
        /// <param name="key">IP:Port</param>
        /// <param name="client">连接对象</param>
        /// <returns>返回成功状态</returns>
        public bool TrySocket(in UserKey key, out WebSocketContext client) => ListClient.TryGetValue(key, out client);

        #region WebServerAsync

        /// <summary>
        /// 创建一个 <see cref="WebServerAsync"/> 服务器类
        /// </summary>
        public WebServerAsync() : this(NetBufferSize.Default) { }

        ///// <summary>
        ///// 创建一个 <see cref="WebServerAsync"/> 服务器类，确认模式和设置流大小
        ///// </summary>
        ///// <param name="size">包大小枚举(警告：请务必保证服务端的大小和客户端一致)</param>
        //public WebServerAsync(NetBufferSize size) : this(size, false) { }

        /// <summary>
        /// 创建一个 <see cref="WebServerAsync"/> 服务器类，确认模式和设置流大小
        /// </summary>
        /// <param name="size">包大小枚举(警告：请务必保证服务端的大小和客户端一致)</param>
        /// 
        ///// <param name="OnlyData">是否启动框架模式</param>
        public WebServerAsync(NetBufferSize size/*, bool OnlyData*/)
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
            this.BufferSize = size;
            this.DataLength = (int)size;
            //this.OnlyData = OnlyData;
        }

        #endregion

        #region StartAsync

        /// <summary>
        /// 开始异步监听本机127.0.0.1的端口号
        /// </summary>
        /// <param name="port">端口号</param>
        public async Task StartAsync(int port)
        {
            await StartAsync(StaticData.LocalIp, port);
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="ip">可以使用“*”</param>
        /// <param name="port">端口号</param>
        public async Task StartAsync(string ip, int port)
        {
            ip = await WebStateObject.IsWebIpEffective(ip);

            string url = string.Concat(ip, ':', port);

            if (!IPEndPoint.TryParse(url, out _))
            {
                throw new FormatException("ip:port 无法被 IPEndPoint 对象识别！");
            }

            await StartAsync(url);
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="uriPrefix">高级定义法，例如：0.0.0.0:80/tcp，该格式适用</param>
        public async Task StartAsync(string uriPrefix)
        {
            ThrowIfDisposed();

            if (string.IsNullOrWhiteSpace(uriPrefix))
            {
                throw new NullReferenceException("uriPrefix 值不了为空！");
            }
            listener = new HttpListener();

            string url = string.Concat(IsSSL ? "https://" : "http://", uriPrefix, '/');
            server = uriPrefix;
            try
            {
                listener.Prefixes.Add(url);
                listener.Start();

               await OnComplete(in server, EnServer.Create);
            }
            catch (Exception e)
            {
                await OnComplete(in server, EnServer.Fail);
                throw new Exception("服务器无法创建！", e);
            }

            StartAsync();
        }

        private async void StartAsync()
        {
            try
            {
                while (!isClose)
                {
                    var context = await listener.GetContextAsync();
                    await AcceptCallBack(context);
                }
            }
            catch (Exception)
            {
                Stop();//出错可能是监听断开等
                if (isClose)
                {
                    //doConnect.Set();
                    foreach (var _client in listClient.Values)
                    {
                        await WebSocketAbort(_client);
                    }
                    listClient.Clear();
                    await OnComplete(in server, EnServer.Close);
                    listener.Close();
                    return;
                }
            }
        }

        #endregion

        #region SendAsync

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端密钥</param>
        /// <param name="msg">要发送的内容</param>
        public async ValueTask<bool> SendAsync(UserKey key, string msg)
        {
            if (TrySocket(in key, out WebSocketContext client))
            {
                return await SendAsync(client, msg);
            }
            else
            {
                throw new Exception("在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。");//在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。
            }
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="client">客户端密钥</param>
        /// <param name="msg">要发送的内容</param>
        public async ValueTask<bool> SendAsync(WebSocketContext client, string msg)
        {
            var chars = msg.AsMemory();
            if (chars.IsEmpty) throw new ArgumentNullException(nameof(msg));
            var sendBytes = CreateSendBytes(client, Encoding.UTF8.GetByteCount(chars.Span));

            try
            {
                Encoding.UTF8.GetBytes(chars.Span, sendBytes.Span);
                return await SendAsync(client, sendBytes.GetMemory());
            }
            finally
            {
                sendBytes.Dispose();
            }
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端密钥</param>
        /// <param name="Data">要发送的内容</param>
        public async ValueTask<bool> SendAsync(UserKey key, Memory<byte> Data)
        {
            if (TrySocket(in key, out WebSocketContext client))
            {
                return await SendAsync(client, Data);
            }
            else
            {
                throw new Exception("在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。");
            }
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="client">客户端密钥</param>
        /// <param name="listData">要发送的内容</param>
        public async ValueTask<bool> SendAsync(WebSocketContext client, Memory<byte> listData)
        {
            var sendBytes = CreateSendBytes(client, listData.Length);

            try
            {
                sendBytes.SetMemory(listData);
                await SendAsync(sendBytes);
                return true;
            }
            catch (Exception)
            {
                return false;
                //OnComplete(key.IpPort, EnSocketAction.SendMsg);
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
        /// <exception cref="ArgumentException">Client空 或 已断开连接</exception>
        public async ValueTask SendAsync(SendBytes<WebSocketContext> sendBytes)
        {
            ThrowIfDisposed();

            if (sendBytes.Client == null)
            {
                throw new ArgumentException("client 对象是空的！", nameof(sendBytes.Client));
            }
            if (!WebStateObject.IsConnected(sendBytes.Client.WebSocket))
            {
                throw new Exception("与客户端的连接已断开！");
            }

            var buffers = sendBytes.GetMemory();

            await WebStateObject.SendAsync(sendBytes.Client.WebSocket, buffers, DataLength);
            await OnComplete(sendBytes.Client.SecWebSocketKey, EnServer.SendMsg);
        }

        /// <summary>
        /// 创建数据发送空间
        /// </summary>
        /// <param name="key">接收者信息</param>
        /// <param name="length">数据大小</param>
        /// <returns></returns>
        /// <exception cref="Exception">连接已中断</exception>
        public SendBytes<WebSocketContext> CreateSendBytes(in UserKey key, int length)
        {
            if (TrySocket(in key, out WebSocketContext client))
            {
                return CreateSendBytes(client, length);
            }
            else
            {
                throw new Exception("在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。");//在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。
            }
        }

        /// <summary>
        /// 创建数据发送空间
        /// </summary>
        /// <param name="client">收数据的对象</param>
        /// <param name="length">数据大小</param>
        /// <returns></returns>
        public SendBytes<WebSocketContext> CreateSendBytes(WebSocketContext client, int length = 0)
        {
            if (client is null) throw new ArgumentException("WebSocketContext不能为空！", nameof(client));
            if (length == 0) length = DataLength;
            return new SendBytes<WebSocketContext>(client, length, false);
        }

        #endregion

        /**
        * 异步接收连接的回调函数
        */
        private async Task AcceptCallBack(HttpListenerContext client)
        {
            if (!client.Request.IsWebSocketRequest)
            {
                byte[] bytes = Encoding.UTF8.GetBytes("{\"code\": 403,\"msg\": \"已被拦截，非[ws,wss]请求！\"}");
                client.Response.ContentEncoding = Encoding.UTF8;
                client.Response.ContentType = "application/json; charset=utf-8";
                client.Response.StatusCode = 403;
                client.Response.Close(bytes, false);

                //Debug.WriteLine("已拦截一个未满足WebSocket的请求！");
            }
            else
            {
                WebSocketContext webContext = InitWebContext is null ? await client.AcceptWebSocketAsync(null) : await InitWebContext(client);
                if (webContext == null) return;
                UserKey key = webContext.SecWebSocketKey;
                //Debug.WriteLine("来自：{0},连接完成！ {1}", key, DateTime.Now.ToString());
                if (listClient.TryAdd(key, webContext))
                {
                    StateObject.StartReceive("Web", StartReceive, webContext); //StartReceive(webContext);
                }
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private async Task StartReceive(WebSocketContext client)
        {
            isReceive = true;
            WebStateObject obj = new(client, this.DataLength);// { doReceive = doReceive };
            await OnComplete(obj.SocketKey, EnServer.Connect);
            while (!isClose)//ListClient.TryGetValue(key, out client) && 不允许意外删除对象问题
            {
                await Task.Delay(Millisecond);
                if (obj.IsConnected())
                {
                    await ReceiveAsync(obj);
                }
                else
                {
                    if (listClient.TryRemove(obj.SocketKey, out client))
                    {
                        await WebSocketAbort(client);
                    }
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
            //obj.doReceive.Reset();
            try
            {
                //obj.SocketClient.BeginReceive(obj.ListData, obj.WriteIndex, obj.ListData.Length - obj.WriteIndex, SocketFlags.None, ReceiveCallBack, obj);

                if (await obj.ReceiveAsync())
                {
                    if (obj.IsKeepAlive())
                    {
                        await OnComplete(obj.SocketKey, EnServer.HeartBeat);
                    }
                    else
                    {
                        if (!DisabledReceive) await OnComplete(obj.SocketKey, EnServer.Receive);
                        await obj.OnReceivedAsync(IsThreadPool, obj.WebSocketContext, Received);
                        //await Received.InvokeAsync(obj.IpPort, ListData);
                    }
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
                //        if (obj.IsKeepAlive(receiveResult.Count))
                //        {
                //            OnComplete(obj.SocketKey, EnServer.HeartBeat);
                //        }
                //        else
                //        {
                //            if (!DisabledReceive) OnComplete(obj.SocketKey, EnServer.Receive);
                //            await obj.OnReceivedAsync(IsThreadPool, obj.WebSocketContext, receiveResult.Count, Received);
                //            //await Received.InvokeAsync(obj.IpPort, ListData);
                //        }
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
        public ValueTask<IGetQueOnEnum> OnComplete(in UserKey key, EnServer enAction) => EnumEventQueue.OnComplete(in key, enAction, Completed);

        /// <summary>
        /// 中断连接并触发事件
        /// </summary>
        /// <param name="client"></param>
        private async ValueTask WebSocketAbort(WebSocketContext client)
        {
            string IpPort = client.SecWebSocketKey;
            client.WebSocket.Abort();
            await OnComplete(IpPort, EnServer.ClientClose);
        }

        /// <summary>
        /// HttpListener关闭
        /// </summary>
        public void Stop()
        {
            isClose = true;
            if (listener != null && listener.IsListening)
            {
                listener.Stop();//当他不在监听，就关闭监听。
            }
        }

        /// <summary>
        /// 回收资源，并关闭所有连接
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            Stop();
            ((IDisposable)listener).Dispose();
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
