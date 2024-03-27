using System;
using System.Buffers;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tool.Sockets.Kernels;
using Tool.Utils;

namespace Tool.Sockets.WebHelper
{
    /// <summary>
    /// WebServer长连接对象
    /// </summary>
    public sealed class WebServer : INetworkListener<WebSocketContext>
    {
        private readonly int DataLength = 1024 * 8;
        private HttpListener listener;
        //用于控制异步接受连接
        //private readonly ManualResetEvent doConnect = new(false);
        //标识服务端连接是否关闭
        private bool isClose = false;
        private readonly ConcurrentDictionary<string, WebSocketContext> listClient = new();

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
        /// key:ip:port
        /// value:TcpClient
        /// </summary>
        public IReadOnlyDictionary<string, WebSocketContext> ListClient
        {
            get { return listClient; }
            //private set { listClient = value; }
        }

        private string server = string.Empty;//服务端IP
        private int millisecond = 20; //默认20毫秒。

        /// <summary>
        /// 服务器创建时的信息
        /// </summary>
        public string Server { get { return server; } }

        /// <summary>
        /// 是否使用线程池调度接收后的数据
        /// 默认 true 开启
        /// </summary>
        public bool IsThreadPool { get; init; } = true;

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
        private Func<HttpListenerContext, HttpListenerWebSocketContext> InitWebContext;

        /**
         * 连接、发送、关闭事件
         */
        private Func<string, EnServer, DateTime, Task> Completed; //event

        /**
         * 接收到数据事件
         */
        private Func<ReceiveBytes<WebSocketContext>, Task> Received; //event

        //**
        //* 信号
        //*/
        //private readonly ManualResetEvent _mre;

        //**
        // * 事件消息Queue
        // */
        //private readonly ConcurrentQueue<GetQueOnEnum> _que;

        /// <summary>
        /// 当新连接，创建时，可以自定义之协议，不实现走默认流程（当实现后，返回null时取消后续业务，请自行关闭连接）
        /// </summary>
        /// <param name="InitWebContext"></param>
        public void SetInitWebContext(Func<HttpListenerContext, HttpListenerWebSocketContext> InitWebContext)
        {
            this.InitWebContext ??= InitWebContext;
        }

        /// <summary>
        /// 连接、发送、关闭事件 <see cref="EnServer"/>
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(Func<string, EnServer, DateTime, Task> Completed)
        {
            this.Completed ??= Completed;
        }

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public void SetReceived(Func<ReceiveBytes<WebSocketContext>, Task> Received)
        {
            this.Received ??= Received;
        }

        /// <summary>
        /// 创建一个 WebServer 服务器类
        /// </summary>
        public WebServer() : this(8 * 1024)
        {

        }

        /// <summary>
        /// 创建一个 WebServer 服务器类，确认模式和设置流大小
        /// </summary>
        /// <param name="DataLength">每次包的最大大小(警告：请务必保证服务端的大小和客户端一致)</param>
        private WebServer(int DataLength)
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
        }

        #region StartAsync

        /// <summary>
        /// 开始异步监听本机127.0.0.1的端口号
        /// </summary>
        /// <param name="port">端口号</param>
        public void StartAsync(int port)
        {
            StartAsync(StaticData.LocalIp, port);
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="ip">可以使用“*”</param>
        /// <param name="port">端口号</param>
        public async void StartAsync(string ip, int port)
        {
            ip = await WebStateObject.IsWebIpEffective(ip);

            string url = string.Concat(ip, ':', port);

            if (!IPEndPoint.TryParse(url, out _))
            {
                throw new FormatException("ip 无法被 IPEndPoint 对象识别！");
            }

            await StartAsync(url);
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="uriPrefix">高级定义法，例如：0.0.0.0:80/tcp，该格式适用</param>
        public async Task StartAsync(string uriPrefix)
        {
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

                OnComplete(server, EnServer.Create);
            }
            catch (Exception e)
            {
                OnComplete(server, EnServer.Fail);
                throw new Exception("服务器无法创建！", e);
            }

            try
            {
                while (!isClose)
                {
                    var context = await listener.GetContextAsync();
                    AcceptCallBack(context);
                }
            }
            catch (Exception)
            {
                Close();//出错可能是监听断开等
                if (isClose)
                {
                    //doConnect.Set();
                    foreach (var _client in listClient.Values)
                    {
                        WebSocketAbort(_client);
                    }
                    listClient.Clear();
                    OnComplete(server, EnServer.Close);
                    listener.Close();
                    return;
                }
                //throw;
            }
            //finally
            //{

            //}

            //await Task.Factory.StartNew(() =>
            //{
            //    while (!isClose)
            //    {
            //        doConnect.Reset();
            //        listener.BeginGetContext(AcceptCallBack, listener);
            //        doConnect.WaitOne();
            //    }
            //}, TaskCreationOptions.LongRunning).ContinueWith((i) => i.Dispose());
        }

        #endregion

        #region SendAsync

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端密钥</param>
        /// <param name="msg">要发送的内容</param>
        public async Task<bool> SendAsync(string key, string msg)
        {
            if (TrySocket(key, out WebSocketContext client))
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
        public async Task<bool> SendAsync(WebSocketContext client, string msg)
        {
            byte[] listData = Encoding.UTF8.GetBytes(msg);
            return await SendAsync(client, listData, false);
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端密钥</param>
        /// <param name="Data">要发送的内容</param>
        public async Task<bool> SendAsync(string key, byte[] Data)
        {
            if (TrySocket(key, out WebSocketContext client))
            {
                return await SendAsync(client, Data, true);
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
        /// <param name="isbyte">发送数据是文本还是字节流</param>
        public async Task<bool> SendAsync(WebSocketContext client, byte[] listData, bool isbyte)
        {
            Memory<byte> _data = new(listData);

            if (client == null)
            {
                throw new ArgumentException("client 对象是空的！", nameof(client));
            }
            if (!WebStateObject.IsConnected(client.WebSocket))
            {
                throw new Exception("与客户端的连接已断开！");
            }

            try
            {
                await client.WebSocket.SendAsync(_data, isbyte ? WebSocketMessageType.Binary : WebSocketMessageType.Text, true, CancellationToken.None);
                OnComplete(client.SecWebSocketKey, EnServer.SendMsg);

                return true;
            }
            catch (Exception)
            {
                return false;
                //OnComplete(key.IpPort, EnSocketAction.SendMsg);
            }
        }

        #endregion

        /**
        * 异步接收连接的回调函数
        */
        private async void AcceptCallBack(HttpListenerContext client)
        {
            if (!client.Request.IsWebSocketRequest)
            {
                byte[] bytes = Encoding.UTF8.GetBytes("{\"code\": 403,\"msg\": \"已被拦截，非[ws,wss]请求！\"}");
                client.Response.ContentEncoding = Encoding.UTF8;
                client.Response.ContentType = "application/json; charset=utf-8";
                client.Response.StatusCode = 403;
                client.Response.Close(bytes, false);

                Debug.WriteLine("已拦截一个未满足WebSocket的请求！");
            }
            else
            {
                WebSocketContext webContext = InitWebContext is null ? await client.AcceptWebSocketAsync(null) : InitWebContext(client);
                if (webContext == null) return;
                string key = webContext.SecWebSocketKey;
                Debug.WriteLine("来自：{0},连接完成！ {1}", key, DateTime.Now.ToString());
                if (listClient.TryAdd(key, webContext))
                {
                    StartReceive(key, webContext);
                    OnComplete(key, EnServer.Connect);
                }
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private async void StartReceive(string key, WebSocketContext client)
        {
            //await Task.Factory.StartNew(async () =>
            //{
            //用于控制异步接收数据
            //ManualResetEvent doReceive = new(false);
            WebStateObject obj = new(client, this.DataLength);// { doReceive = doReceive };
            while (!isClose)//ListClient.TryGetValue(key, out client) && 不允许意外删除对象问题
            {
                if (WebStateObject.IsConnected(client.WebSocket))
                {
                    await Task.Delay(Millisecond); //Thread.Sleep(Millisecond);
                    await ReceiveAsync(obj);
                    //await Task.Delay(Millisecond); //Thread.Sleep(Millisecond);
                }
                else
                {
                    if (listClient.TryRemove(key, out client))
                    {
                        WebSocketAbort(client);
                    }
                    break;
                }
            }
            obj.Close();
            //}, TaskCreationOptions.LongRunning).ContinueWith((i) => i.Dispose());
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
                            if (obj.IsKeepAlive(receiveResult.Count))
                            {
                                OnComplete(obj.SocketKey, EnServer.HeartBeat);
                            }
                            else
                            {
                                if (!DisabledReceive) OnComplete(obj.SocketKey, EnServer.Receive);

                                if (Received is not null)
                                {
                                    obj.OnReceived(IsThreadPool, obj.WebSocketContext, receiveResult.Count, Received);

                                    //await Received.InvokeAsync(obj.IpPort, ListData);
                                }
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
        internal void OnComplete(string key, EnServer enAction)
        {
            if (Completed is null) return;
            EnumEventQueue.OnComplete(key, enAction, completed);
            void completed(string age0, Enum age1, DateTime age2)
            {
                Completed(age0, (EnServer)age1, age2).Wait();  //Unsafe.NullRef<T>();var a = ref Unsafe.AsRef<EnServer>(ref age1);
            }
        }

        /// <summary>
        /// 中断连接并触发事件
        /// </summary>
        /// <param name="client"></param>
        private void WebSocketAbort(WebSocketContext client) 
        {
            string IpPort = client.SecWebSocketKey;
            client.WebSocket.Abort();
            OnComplete(IpPort, EnServer.ClientClose);
        }

        /// <summary>
        /// HttpListener关闭
        /// </summary>
        public void Close()
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
            Close();
            ((IDisposable)listener).Dispose();
            //doConnect.Close();
            //_mre.Close();
        }

        public void SendAsync(WebSocketContext client, params ArraySegment<byte>[] listData)
        {
            throw new NotImplementedException();
        }

        public void Send(WebSocketContext client, params ArraySegment<byte>[] listData)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 根据Key获取对应的连接对象
        /// </summary>
        /// <param name="key">IP:Port</param>
        /// <param name="client">连接对象</param>
        /// <returns>返回成功状态</returns>
        public bool TrySocket(string key, out WebSocketContext client)
        {
            return ListClient.TryGetValue(key, out client);
        }
    }
}
