using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.SupportCode;

namespace Tool.Sockets.WebTcp
{
    /// <summary>
    /// Web长连接对象
    /// </summary>
    public sealed class WebServer : IDisposable
    {
        private readonly int DataLength = 1024 * 8;
        private HttpListener listener;
        //用于控制异步接受连接
        private readonly ManualResetEvent doConnect = new ManualResetEvent(false);
        //标识服务端连接是否关闭
        private bool isClose = false;
        private ConcurrentDictionary<string, WebContext> listClient = new ConcurrentDictionary<string, WebContext>();

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        /// <summary>
        /// 标识服务端连接是否关闭
        /// </summary>
        public bool IsClose { get { return isClose; } }

        /// <summary>
        /// 获取指示是否使用安全套接字层 (SSL) 保护 WebSocket 连接的值。
        /// </summary>
        /// <remarks>true 如果使用 SSL; 保护 WebSocket 连接，否则为false。</remarks>
        public bool IsSSL { get { return isssl; } }

        /// <summary>
        /// 已建立连接的集合
        /// key:ip:port
        /// value:TcpClient
        /// </summary>
        public ConcurrentDictionary<string, WebContext> ListClient
        {
            get { return listClient; }
            private set { listClient = value; }
        }

        private bool isssl;//当前启动的是不是受保护的服务
        private string server = string.Empty;//服务端IP
        private int millisecond = 20; //默认20毫秒。

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

        /**
         * 连接、发送、关闭事件
         */
        private Action<string, EnServer, DateTime> Completed; //event

        /**
         * 接收到数据事件
         */
        private Action<string, byte[]> Received; //event

        //**
        //* 信号
        //*/
        //private readonly ManualResetEvent _mre;

        //**
        // * 事件消息Queue
        // */
        //private readonly ConcurrentQueue<GetQueOnEnum> _que;

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(Action<string, EnServer, DateTime> Completed)
        {
            if (this.Completed == null)
                this.Completed = Completed;
        }

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public void SetReceived(Action<string, byte[]> Received)
        {
            if (this.Received == null)
                this.Received = Received;
        }

        /// <summary>
        /// 创建一个WebTcp服务器类
        /// </summary>
        public WebServer() : this(false, 8 * 1024)
        {

        }

        /// <summary>
        /// 创建一个WebTcp服务器类，并确定是否开启框架验证模式保证数据唯一性。
        /// </summary>
        /// <param name="OnlyData">是否启动框架模式</param>
        public WebServer(bool OnlyData) : this(OnlyData, 8 * 1024)
        {

        }

        /// <summary>
        /// 创建一个WebTcp服务器类，设置流大小
        /// </summary>
        /// <param name="DataLength">每次包的最大大小(警告：请务必保证服务端的大小和客户端一致)</param>
        public WebServer(int DataLength) : this(false, DataLength)
        {
        }

        /// <summary>
        /// 创建一个WebTcp服务器类，确认模式和设置流大小
        /// </summary>
        /// <param name="OnlyData">是否启动框架模式</param>
        /// <param name="DataLength">每次包的最大大小(警告：请务必保证服务端的大小和客户端一致)</param>
        public WebServer(bool OnlyData, int DataLength)
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
            this.OnlyData = OnlyData;

            //_que = new ConcurrentQueue<GetQueOnEnum>();
            //_mre = new ManualResetEvent(false);

            //TaskOnComplete();
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="ip">可以使用“*”</param>
        /// <param name="port">端口号</param>
        /// <param name="IsSSL">是否使用安全套接字层 (SSL) 保护 WebSocket 连接的值。</param>
        public void StartAsync(string ip, int port, bool IsSSL)
        {
            if (ip.Equals("0.0.0.0") || ip.Equals("*"))
            {
                string name = Dns.GetHostName();
                IPAddress[] ipadrlist = Dns.GetHostAddresses(name);
                foreach (IPAddress ipadr in ipadrlist)
                {
                    if (ipadr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        ip = ipadr.ToString();
                        break;
                    }
                }
            }

            try
            {
                IPAddress ipAddress = IPAddress.Parse(ip);
                _ = new IPEndPoint(ipAddress, port);
            }
            catch (Exception)
            {
                throw;
            }
            string url = string.Concat(ip, ':', port);
            StartAsync(url, IsSSL);
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="uriPrefix">高级定义法，例如：0.0.0.0:80/tcp，该格式适用</param>
        /// <param name="IsSSL">是否使用安全套接字层 (SSL) 保护 WebSocket 连接的值。</param>
        public void StartAsync(string uriPrefix, bool IsSSL)
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

            isssl = IsSSL;

            Task.Factory.StartNew(() =>
            {
                while (!isClose)
                {
                    doConnect.Reset();
                    listener.BeginGetContext(AcceptCallBack, listener);
                    doConnect.WaitOne();
                }
            }, TaskCreationOptions.LongRunning).ContinueWith((i) => i.Dispose());
        }

        /// <summary>
        /// 开始异步监听本机127.0.0.1的端口号
        /// </summary>
        /// <param name="port">端口号</param>
        /// <param name="IsSSL">是否使用安全套接字层 (SSL) 保护 WebSocket 连接的值。</param>
        public void StartAsync(int port, bool IsSSL)
        {
            StartAsync("127.0.0.1", port, IsSSL);
        }

        //private void TaskOnComplete()
        //{
        //    Task.Factory.StartNew(() =>
        //    {
        //        while (!IsClose)
        //        {
        //            // 等待信号通知
        //            _mre.WaitOne();

        //            GetQueOnEnum getQueOn;
        //            // 判断是否有内容需要执行的事件 从列队中获取内容，并删除列队中的内容
        //            while (_que.Count > 0 && _que.TryDequeue(out getQueOn))
        //            {
        //                try
        //                {
        //                    Completed?.Invoke(getQueOn.Key, getQueOn.ServerEnum);
        //                }
        //                catch
        //                {
        //                }
        //            }

        //            // 重新设置信号
        //            _mre.Reset();
        //            Thread.Sleep(1);
        //        }
        //    }, TaskCreationOptions.LongRunning).ContinueWith((i) =>  /*_mre.Close();*/ i.Dispose());
        //}

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端的ip地址和端口号</param>
        /// <param name="msg">要发送的内容</param>
        public void SendAsync(string key, string msg)
        {
            if (ListClient.TryGetValue(key, out WebContext client))
            {
                SendAsync(client, msg);
            }
            else
            {
                throw new Exception("在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。");//在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。
            }
        }

        /**
         * 开始异步发送数据
         * key 客户端的ip地址和端口号
         * Data 要发送的内容
         */
        internal void SendAsync(string key, byte[] Data)
        {
            if (ListClient.TryGetValue(key, out WebContext client))
            {
                SendAsync(client, Data, true);
            }
            else
            {
                throw new Exception("在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。");
            }
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="client">客户端的ip地址和端口号</param>
        /// <param name="msg">要发送的内容</param>
        public void SendAsync(WebContext client, string msg)
        {
            byte[] listData = Encoding.UTF8.GetBytes(msg);
            SendAsync(client, listData, false);
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="client">客户端的ip地址和端口号</param>
        /// <param name="listData">要发送的内容</param>
        /// <param name="isbyte">发送数据是文本还是字节流</param>
        public void SendAsync(WebContext client, byte[] listData, bool isbyte)
        {
            ArraySegment<byte> _data = new ArraySegment<byte>(listData);

            if (this.OnlyData)
            {
                byte[] Data = WebStateObject.GetDataSend(listData, DataLength);

                if (client == null)
                {
                    throw new ArgumentException("client 对象是空的！", nameof(client));
                }
                if (!WebStateObject.IsConnected(client))
                {
                    throw new Exception("与客户端的连接已断开！");
                }
                //client.Client.BeginSend(Data, 0, Data.Length, SocketFlags.None, SendCallBack, client);
            }
            else
            {
                if (client == null)
                {
                    throw new ArgumentException("client 对象是空的！", nameof(client));
                }
                if (!WebStateObject.IsConnected(client))
                {
                    throw new Exception("与客户端的连接已断开！");
                }

                Task task = client.Socket.SendAsync(_data, isbyte ? WebSocketMessageType.Binary : WebSocketMessageType.Text, true, CancellationToken.None);
                task.ContinueWith((_task, _client) =>
                {
                    WebContext key = _client as WebContext;
                    if (_task.IsCompleted)
                    {
                        OnComplete(key.IpPort, EnServer.SendMsg);
                    }
                    else
                    {
                        //OnComplete(key.IpPort, EnSocketAction.SendMsg);
                    }
                }, client);
            }
        }

        /**
        * 异步接收连接的回调函数
        */
        private void AcceptCallBack(IAsyncResult ar)
        {
            HttpListener l = ar.AsyncState as HttpListener;
            if (isClose)
            {
                doConnect.Set();
                foreach (var _client in listClient)
                {
                    string IpPort = _client.Value.IpPort;
                    _client.Value.Close();
                    OnComplete(IpPort, EnServer.ClientClose);
                }
                listClient.Clear();
                OnComplete(server, EnServer.Close);
                l.Close();
                return;
            }

            HttpListenerContext client = l.EndGetContext(ar);
            doConnect.Set();

            if (!client.Request.IsWebSocketRequest)
            {
                byte[] bytes = Encoding.UTF8.GetBytes("{\"code\": 500,\"msg\": \"已被拦截，非HTTP请求！\"}");
                client.Response.ContentEncoding = Encoding.UTF8;
                client.Response.ContentType = "application/json; charset=utf-8";
                client.Response.Close(bytes, false);

                Debug.WriteLine("已拦截一个未满足WebSocket的请求！");
            }
            else
            {
                WebContext webContext = new WebContext(client);
                string key = webContext.IpPort;
                Debug.WriteLine("来自：{0},连接完成！ {1}", key, DateTime.Now.ToString());
                if (ListClient.TryAdd(key, webContext))
                {
                    StartReceive(key, webContext);
                    OnComplete(key, EnServer.Connect);
                }
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private void StartReceive(string key, WebContext client)
        {
            Task.Factory.StartNew(() =>
            {
                //用于控制异步接收数据
                ManualResetEvent doReceive = new ManualResetEvent(false);
                WebStateObject obj = new WebStateObject(client, this.DataLength) { doReceive = doReceive };
                while (ListClient.TryGetValue(key, out client) && !isClose)
                {
                    if (WebStateObject.IsConnected(client))
                    {
                        Thread.Sleep(Millisecond);
                        ReceiveAsync(obj);
                        Thread.Sleep(Millisecond);
                    }
                    else
                    {
                        if (ListClient.TryRemove(key, out client))
                        {
                            client.Close();
                            OnComplete(key, EnServer.ClientClose);
                        }
                        break;
                    }
                }
                obj.Close();
            }, TaskCreationOptions.LongRunning).ContinueWith((i) => i.Dispose());
        }

        /**
         * 开始异步接收数据
         * obj 要接收的客户端包体
         */
        private void ReceiveAsync(WebStateObject obj)
        {
            obj.doReceive.Reset();
            if (WebStateObject.IsConnected(obj.Client))
            {
                try
                {
                    //obj.SocketClient.BeginReceive(obj.ListData, obj.WriteIndex, obj.ListData.Length - obj.WriteIndex, SocketFlags.None, ReceiveCallBack, obj);

                    Task<WebSocketReceiveResult> webSocketReceiveResult = obj.SocketClient.ReceiveAsync(obj.ListData, CancellationToken.None);
                    webSocketReceiveResult.ContinueWith<WebSocketReceiveResult>((i, _obj) =>
                    {
                        if (!i.IsCompleted)
                            throw new Exception("异步接收为完成！");

                        WebStateObject _object = _obj as WebStateObject;

                        if (i.IsFaulted)
                        {
                            Debug.WriteLine(i.Exception.InnerException.Message);
                            _object.SocketClient.Abort();
                            _object.doReceive.Set();
                            return null;
                        }

                        WebSocketReceiveResult receiveResult = i.Result;

                        if (receiveResult.EndOfMessage)
                        {
                            if (receiveResult.MessageType == WebSocketMessageType.Close)
                            {
                                Debug.WriteLine("客户端：{0}，已经断开！", (object)_object.IpPort);
                                _object.SocketClient.Abort();
                                _object.doReceive.Set();
                                return receiveResult;
                            }

                            byte[] ListData = new byte[receiveResult.Count];
                            Array.Copy(_object.ListData.Array, 0, ListData, 0, receiveResult.Count);
                            OnComplete(_object.IpPort, EnServer.Receive);
                            ThreadPool.QueueUserWorkItem(x =>
                            {
                                Received?.Invoke(_object.IpPort, x as byte[]);//触发接收事件
                            }, ListData);
                        }
                        _object.doReceive.Set();
                        return receiveResult;
                    }, obj);
                }
                catch (Exception)
                {

                }
                obj.doReceive.WaitOne();
            }
        }

        /// <summary>
        /// 事件方法
        /// </summary>
        /// <param name="key">指定发送对象</param>
        /// <param name="enAction">消息类型</param>
        internal void OnComplete(string key, EnServer enAction)
        {
            TcpEventQueue.OnComplete(key, enAction, Completed);
            //if (!_mre.SafeWaitHandle.IsClosed)
            //{
            //    _que.Enqueue(new GetQueOnEnum(key, enAction));//Completed?.Invoke(key, enAction)
            //    _mre.Set();//启动
            //}
            //Completed?.Invoke(key, enAction);
        }

        /// <summary>
        /// TCP关闭
        /// </summary>
        public void Close()
        {
            isClose = true;
            if (listener != null && listener.IsListening)
            {
                listener.Stop();//当他不在监听，就关闭监听。
            }
        }

        void IDisposable.Dispose()
        {
            ((IDisposable)listener).Dispose();
            doConnect.Close();
            //_mre.Close();
        }
    }
}
