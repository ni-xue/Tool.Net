using System;
using System.Collections.Generic;
using System.Net;
#if NET7_0_OR_GREATER
using System.Net.Quic;
#endif
using System.Net.Security;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using System.Buffers;
using System.IO.Pipelines;
using System.Runtime.Versioning;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;
using System.Threading;
using Tool.Utils;

namespace Tool.Sockets.QuicHelper
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// 封装一个底层异步Quic对象（服务端）写了但属于预览物无法使用
    /// </summary>
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macOS")]
    [SupportedOSPlatform("OSX")]
    [SupportedOSPlatform("windows")]
    [RequiresPreviewFeatures]
#else
    /// <summary>
    /// .Net7 以上支持
    /// </summary>
#endif
    public class QuicServerAsync
#if NET7_0_OR_GREATER
        : INetworkListener<QuicSocket>
#endif
    {

#if NET7_0_OR_GREATER

        private readonly int DataLength = 1024 * 8;
        private QuicListener quicListener;
        private bool isClose = false; //标识服务端连接是否关闭
        private bool isReceive = false; //标识是否调用了接收函数
        private readonly ConcurrentDictionary<UserKey, QuicSocket> listClient = new();

        private Ipv4Port server; //服务端IP
        private IPEndPoint endPointServer;
        private int millisecond = 20; //默认20毫秒。

        /**
        * 连接、发送、关闭事件
        */
        private CompletedEvent<EnServer> Completed; //event

        /**
         * 接收到数据事件
         */
        private ReceiveEvent<QuicSocket> Received; //event

        /// <summary>
        /// 标识客户端连接是否关闭
        /// </summary>
        public bool IsClose { get { return isClose; } }

        /// <summary>
        /// 服务器的监听信息
        /// </summary>
        public UserKey Server { get { return server; } }

        /// <summary>
        /// 已建立连接的集合
        /// key:UserKey
        /// value:QuicSocket
        /// </summary>
        public IReadOnlyDictionary<UserKey, QuicSocket> ListClient => listClient;

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

        /**
        * 提供自定义设置证书的服务
        */
        private Func<QuicConnection, SslClientHelloInfo, ValueTask<System.Security.Cryptography.X509Certificates.X509Certificate2>> InitCertificate;

        /// <summary>
        /// 设置支持的HTTP协议
        /// </summary>
        public List<SslApplicationProtocol> ApplicationProtocols { get; } = new() { SslApplicationProtocol.Http3 };

        /// <summary>
        /// 表示通讯的包大小
        /// </summary>
        public NetBufferSize BufferSize { get; }

        /// <summary>
        /// 当新连接，创建时，返回对应用于验证的证书
        /// </summary>
        /// <param name="InitCertificate"></param>
        public void SetInitCertificate(Func<QuicConnection, SslClientHelloInfo, ValueTask<System.Security.Cryptography.X509Certificates.X509Certificate2>> InitCertificate) => this.InitCertificate ??= InitCertificate;

        /// <summary>
        /// 连接、发送、关闭事件 <see cref="EnServer"/>
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(CompletedEvent<EnServer> Completed) => this.Completed ??= Completed;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public void SetReceived(ReceiveEvent<QuicSocket> Received)
        {
            if (isReceive) throw new Exception("当前已无法绑定接收委托了，因为StartAsync()已经调用了");
            this.Received ??= Received;
        }

        /// <summary>
        /// 根据IP:Port获取对应的连接对象
        /// </summary>
        /// <param name="key">IP:Port</param>
        /// <param name="client">连接对象</param>
        /// <returns>返回成功状态</returns>
        public bool TrySocket(in UserKey key, out QuicSocket client) => ListClient.TryGetValue(key, out client);

        #region QuicServerAsync

        /// <summary>
        /// 创建一个 <see cref="QuicServerAsync"/> 服务器类
        /// </summary>
        public QuicServerAsync() : this(NetBufferSize.Default) { }

        /// <summary>
        /// 创建一个 <see cref="QuicServerAsync"/> 服务器类，并确定是否开启框架验证模式保证数据唯一性。
        /// </summary>
        /// <param name="size">数据缓冲区大小</param>
        public QuicServerAsync(NetBufferSize size) : this(size, false) { }

        /// <summary>
        /// 创建一个 <see cref="QuicServerAsync"/> 服务器类，确认模式和设置流大小
        /// </summary>
        /// <param name="size">数据缓冲区大小</param>
        /// <param name="OnlyData">是否启动框架模式</param>
        public QuicServerAsync(NetBufferSize size, bool OnlyData)
        {
            if (!QuicListener.IsSupported) throw new Exception("不支持QUIC，请检查是否存在libmsquic以及是否支持TLS 1.3。");
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
        public async Task StartAsync(string ip, int port)
        {
            ThrowIfDisposed();

            string _server = $"{ip}:{port}";
            if (!IPEndPoint.TryParse(_server, out endPointServer))
            {
                throw new FormatException("ip:port 无法被 IPEndPoint 对象识别！");
            }
            server = _server;
            //创建 QuicListener
            try
            {
                quicListener = await QuicListener.ListenAsync(new QuicListenerOptions
                {
                    ApplicationProtocols = ApplicationProtocols,
                    ListenEndPoint = endPointServer,
                    ConnectionOptionsCallback = ServerConnectionOptions
                });
                StartAsync();
                OnComplete(Server, EnServer.Create);
            }
            catch (Exception e)
            {
                OnComplete(Server, EnServer.Fail);
                throw new Exception("服务器监听时发生异常！", e);
            }
        }

        private async void StartAsync()
        {
            try
            {
                while (!isClose)
                {
                    var connection = await quicListener.AcceptConnectionAsync();
                    AcceptCallBack(await QuicSocket.QuicSocketAsync(connection, true));
                }
            }
            catch (Exception ex)
            {
                Log.Error("Quic监听错误：", ex, "Log/QuicServer");
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
            if (TrySocket(key, out QuicSocket client))
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
            if (TrySocket(key, out QuicSocket client))
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
        public async ValueTask SendAsync(QuicSocket client, string msg)
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
        /// <param name="client">Socket对象</param>
        /// <param name="listData">要发送的内容</param>
        public async ValueTask SendAsync(QuicSocket client, ArraySegment<byte> listData)// byte[] listData
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
        public async ValueTask SendAsync(SendBytes<QuicSocket> sendBytes)
        {
            ThrowIfDisposed();

            var client = sendBytes.Client;
            if (sendBytes.OnlyData != OnlyData) throw new ArgumentException("与当前套接字协议不一致！", nameof(sendBytes.OnlyData));
            if (!QuicStateObject.IsConnected(client)) throw new Exception("与客户端的连接已断开！");
            var buffers = sendBytes.GetMemory();
            try
            {
                await client.SendAsync(buffers);

                UserKey key = QuicStateObject.GetIpPort(client);
                OnComplete(in key, EnServer.SendMsg);
            }
            catch (Exception)
            {
                await client.CloseAsync();
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
        public SendBytes<QuicSocket> CreateSendBytes(Ipv4Port key, int length)
        {
            if (TrySocket(key, out QuicSocket client))
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
        /// <param name="quicclient">收数据的对象</param>
        /// <param name="length">数据大小</param>
        /// <returns></returns>
        public SendBytes<QuicSocket> CreateSendBytes(QuicSocket quicclient, int length = 0)
        {
            if (quicclient == null) throw new ArgumentException("QuicSocket不能为空！", nameof(quicclient));
            if (length == 0) length = DataLength;
            return new SendBytes<QuicSocket>(quicclient, length, OnlyData);
        }

        #endregion

        /**
        * 异步接收连接的回调函数
        */
        private void AcceptCallBack(QuicSocket quicSocket)
        {
            Debug.WriteLine($"Client [{quicSocket.RemoteEndPoint}]: connected");

            UserKey key = StateObject.GetIpPort(quicSocket.RemoteEndPoint);
            //Debug.WriteLine("来自：{0},连接完成！ {1}", key, DateTime.Now.ToString());
            if (listClient.TryAdd(key, quicSocket))
            {
                StateObject.StartReceive("Quic", StartReceive, quicSocket); //StartReceive(key, quicSocket);
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private async Task StartReceive(QuicSocket quicSocket)
        {
            isReceive = true;
            QuicStateObject obj = new(quicSocket, DataLength, OnlyData, Received);
            OnComplete(obj.SocketKey, EnServer.Connect).Wait();
            while (!isClose)//ListClient.TryGetValue(key, out client) && 
            {
                await Task.Delay(Millisecond); //Thread.Sleep(Millisecond);
                if (obj.IsConnected())
                {
                    await ReceiveAsync(obj);
                }
                else
                {
                    if (listClient.TryRemove(obj.SocketKey, out quicSocket))
                    {
                        await QuicAbortAsync(obj.SocketKey, quicSocket);
                    }
                    break;
                } //Thread.Sleep(Millisecond);
            }
            await obj.CloseAsync();
        }

        /**
         * 开始异步接收数据
         * obj 要接收的客户端包体
         */
        private async ValueTask ReceiveAsync(QuicStateObject obj)
        {
            try
            {
                ReadResult result = await obj.Client.ReceiveAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;

                while (obj.TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                {
                    // Process the line. 
                    //ProcessLine(in line);

                    await OnReceived(line, obj);
                }

                obj.Client.AdvanceTo(in buffer);

                //if (await obj.ReceiveAsync())
                //{
                //    if (!DisabledReceive) OnComplete(obj.SocketKey, EnClient.Receive);
                //    Keep?.ResetTime();
                //    await obj.OnReceivedAsync(IsThreadPool, obj.Client, Received);
                //    //await Received.InvokeAsync(obj.IpPort, ListData);
                //}

                if (result.IsCompleted) await obj.CloseAsync();//isClose = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await obj.CloseAsync();
            }

            async ValueTask OnReceived(ReadOnlySequence<byte> listData, QuicStateObject obj)
            {
                if (obj.IsKeepAlive(in listData))
                {
                    OnComplete(obj.SocketKey, EnServer.HeartBeat);
                }
                else
                {
                    if (!DisabledReceive) OnComplete(obj.SocketKey, EnServer.Receive);
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

        private async ValueTask QuicAbortAsync(UserKey key, QuicSocket _client)
        {
            await _client.CloseAsync();
            OnComplete(key, EnServer.ClientClose);
        }

        private async ValueTask<QuicServerConnectionOptions> ServerConnectionOptions(QuicConnection connection, SslClientHelloInfo ssl, CancellationToken token = default)
        {
            var certificate = InitCertificate is null ? QuicStateObject.GenerateManualCertificate(ssl.ServerName) : await InitCertificate(connection, ssl);
            return await QuicStateObject.ServerConnectionOptions(certificate, ApplicationProtocols);
        }

        /// <summary>
        /// 不等待异步关闭Ouic
        /// </summary>
        public async void Stop()
        {
            try
            {
                if (quicListener is null || isClose) return;
                isClose = true;
                await quicListener.DisposeAsync();//当他不在监听，就关闭监听。
                if (isClose)
                {
                    foreach (var _client in listClient)
                    {
                        await QuicAbortAsync(_client.Key, _client.Value);
                    }
                    listClient.Clear();
                    OnComplete(Server, EnServer.Close);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Quic关闭错误：", ex, "Log/QuicServer");
            }
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
#endif
    }
}
