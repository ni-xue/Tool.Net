using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Utils;

namespace Tool.Sockets.TcpHelper
{
    /// <summary>
    /// 封装一个底层异步TCP对象（服务端）IpV4
    /// </summary>
    public class TcpServerAsync : INetworkListener<Socket>
    {
        private readonly int DataLength = 1024 * 8;
        private Socket listener;
        private bool isClose = false; //标识服务端连接是否关闭
        private bool isReceive = false; //标识是否调用了接收函数
        internal readonly ConcurrentDictionary<UserKey, Socket> listClient = new();

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
        /// 表示通讯的包大小
        /// </summary>
        public NetBufferSize BufferSize { get; }// = NetBufferSize.Size8K;

        /// <summary>
        /// 已建立连接的集合
        /// key:UserKey
        /// value:Socket
        /// </summary>
        public IReadOnlyDictionary<UserKey, Socket> ListClient => listClient;

        private Ipv4Port server; //服务端IP
        private IPEndPoint endPointServer;
        private int millisecond = 20; //默认20毫秒。

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

        /**
         * 连接、发送、关闭事件
         */
        private CompletedEvent<EnServer> Completed; //event

        /**
         * 接收到数据事件
         */
        private ReceiveEvent<Socket> Received; //event

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(CompletedEvent<EnServer> Completed) => this.Completed ??= Completed;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public void SetReceived(ReceiveEvent<Socket> Received)
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
        public bool TrySocket(in UserKey key, out Socket client) => ListClient.TryGetValue(key, out client);

        #region TcpServerAsync

        /// <summary>
        /// 创建一个 <see cref="TcpClientAsync"/> 服务器类
        /// </summary>
        public TcpServerAsync() : this(NetBufferSize.Default) { }

        /// <summary>
        /// 创建一个 <see cref="TcpClientAsync"/> 服务器类，并确定是否开启框架验证模式保证数据唯一性。
        /// </summary>
        /// <param name="size">数据缓冲区大小</param>
        public TcpServerAsync(NetBufferSize size) : this(size, false) { }

        /// <summary>
        /// 创建一个 <see cref="TcpClientAsync"/> 服务器类，确认模式和设置流大小
        /// </summary>
        /// <param name="size">数据缓冲区大小</param>
        /// <param name="OnlyData">是否启动框架模式</param>
        public TcpServerAsync(NetBufferSize size, bool OnlyData)
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
            this.OnlyData = OnlyData;
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
        public Task StartAsync(string ip, int port)
        {
            ThrowIfDisposed();

            string _server = $"{ip}:{port}";
            if (!IPEndPoint.TryParse(_server, out endPointServer))
            {
                throw new FormatException("ip:port 无法被 IPEndPoint 对象识别！");
            }
            server = _server;

            listener = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp)
            {
                ReceiveBufferSize = (int)this.BufferSize,
                SendBufferSize = (int)this.BufferSize
            };

            listener.Bind(endPointServer);
            try
            {
                listener.Listen();
                OnComplete(Server, EnServer.Create);
            }
            catch (Exception e)
            {
                OnComplete(Server, EnServer.Fail);
                throw new Exception("服务器监听时发生异常！", e);
            }

            StartAsync();
            return Task.CompletedTask;
        }

        private async void StartAsync()
        {
            try
            {
                while (!isClose)
                {
                    //Thread.CurrentThread.Name ??= $"Tcp服务端-监听({server})";
                    var context = await listener.AcceptAsync();
                    AcceptCallBack(context);
                }
            }
            catch (Exception)
            {
                Stop();//出错可能是监听断开等
                if (isClose)
                {
                    //doConnect.Set();
                    foreach (var _client in listClient)
                    {
                        SocketAbort(_client.Key, _client.Value);
                    }
                    listClient.Clear();
                    OnComplete(Server, EnServer.Close);
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
        /// <param name="key">客户端的ip地址和端口号</param>
        /// <param name="msg">要发送的内容</param>
        public async ValueTask SendAsync(Ipv4Port key, string msg)
        {
            if (TrySocket(key, out Socket client))
            {
                await SendAsync(client, msg);
            }
            else
            {
                throw new Exception("在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。");//在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。
            }
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端的ip地址和端口号</param>
        /// <param name="listData">要发送的内容</param>
        public async ValueTask SendAsync(Ipv4Port key, ArraySegment<byte> listData)
        {
            if (TrySocket(key, out Socket client))
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
        /// <param name="client">Socket对象</param>
        /// <param name="msg">要发送的内容</param>
        public async ValueTask SendAsync(Socket client, string msg)
        {
            byte[] listData = Encoding.UTF8.GetBytes(msg);
            await SendAsync(client, listData);
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="client">Socket对象</param>
        /// <param name="listData">要发送的内容</param>
        public async ValueTask SendAsync(Socket client, ArraySegment<byte> listData)// byte[] listData
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
        /// 异步发送消息
        /// </summary>
        /// <param name="sendBytes">数据包</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">OnlyData验证失败</exception>
        /// <exception cref="Exception">连接已断开</exception>
        public async ValueTask SendAsync(SendBytes<Socket> sendBytes)
        {
            ThrowIfDisposed();

            var client = sendBytes.Client;
            if (sendBytes.OnlyData != OnlyData) throw new ArgumentException("与当前套接字协议不一致！", nameof(sendBytes.OnlyData));
            if (!TcpStateObject.IsConnected(client)) throw new Exception("与客户端的连接已断开！");
            var buffers = sendBytes.GetMemory();
            try
            {
                int count = await client.SendAsync(buffers, SocketFlags.None);
                UserKey key = StateObject.GetIpPort(client);
                OnComplete(in key, EnServer.SendMsg);
            }
            catch (Exception)
            {
                //如果发生异常，说明客户端失去连接，触发关闭事件
                client.Close();
                //OnComplete(server, EnServer.ClientClose);
                throw;
            }
        }

        /// <summary>
        /// 创建数据发送空间
        /// </summary>
        /// <param name="key">接收者信息</param>
        /// <param name="length">数据大小</param>
        /// <returns></returns>
        /// <exception cref="Exception">连接已中断</exception>
        public SendBytes<Socket> CreateSendBytes(Ipv4Port key, int length)
        {
            if (TrySocket(key, out Socket client))
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
        public SendBytes<Socket> CreateSendBytes(Socket client, int length = 0)
        {
            if (client is null) throw new ArgumentException("Socket不能为空！", nameof(client));
            if (length == 0) length = DataLength;
            return new SendBytes<Socket>(client, length, OnlyData);
        }

        #endregion

        /**
         * 异步接收连接的回调函数
         */
        private void AcceptCallBack(Socket client)
        {
            UserKey key = StateObject.GetIpPort(client);
            if (listClient.TryAdd(key, client))
            {
                StateObject.StartReceive("Tcp", StartReceive, client); //StartReceive(client);
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private async Task StartReceive(Socket client)
        {
            isReceive = true;
            TcpStateObject obj = new(client, this.DataLength, this.OnlyData, Received);
            OnComplete(obj.IpPort, EnServer.Connect).Wait();
            while (!isClose)//ListClient.TryGetValue(key, out client) && 
            {
                await Task.Delay(Millisecond);
                if (obj.IsConnected())
                {
                    await ReceiveAsync(obj);
                }
                else
                {
                    if (listClient.TryRemove(obj.IpPort, out client))
                    {
                        SocketAbort(obj.IpPort, client);
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
        private async ValueTask ReceiveAsync(TcpStateObject obj)
        {
            //obj.doReceive.Reset();
            try
            {
                if (!await obj.ReceiveAsync()) { return; }
                //obj.OnReceiveTask(OnReceived);//尝试使用，原线程处理包解析，验证效率
                Memory<byte> memory = Memory<byte>.Empty;
                bool isend = false;
                while (obj.OnReceiveTask(ref memory, ref isend)) await OnReceived(memory, obj);

                //obj.Client.BeginReceive(obj.ListData, obj.WriteIndex, obj.SpareSize, SocketFlags.None, ReceiveCallBack, obj);
                //obj.doReceive.WaitOne();
            }
            catch (Exception)
            {
                obj.ClientClose();
            }

            async ValueTask OnReceived(Memory<byte> listData, TcpStateObject obj)
            {
                if (obj.IsKeepAlive(in listData))
                {
                    OnComplete(obj.IpPort, EnServer.HeartBeat);
                }
                else
                {
                    if (!DisabledReceive) OnComplete(obj.IpPort, EnServer.Receive);
                    await obj.OnReceiveAsync(IsThreadPool, listData);
                }
            }
        }

        /// <summary>
        /// 可供开发重写的事件方法
        /// </summary>
        /// <param name="key">指定发送对象</param>
        /// <param name="enAction">消息类型</param>
        public virtual IGetQueOnEnum OnComplete(in UserKey key, EnServer enAction) => EnumEventQueue.OnComplete(in key, enAction, Completed);

        private void SocketAbort(in UserKey key, Socket _client)
        {
            _client.Close();
            OnComplete(in key, EnServer.ClientClose);
        }

        /// <summary>
        /// TCP关闭
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
