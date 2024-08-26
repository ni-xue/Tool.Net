using System;
using System.Collections.Generic;
#if NET7_0_OR_GREATER
using System.Net.Quic;
#endif
using System.Net.Security;
using System.Net;
using Tool.Sockets.Kernels;
using System.Threading.Tasks;
using System.IO.Pipelines;
using System.Text;
using System.Runtime.Versioning;
using System.Diagnostics;
using System.Buffers;
using System.Net.Sockets;
using System.Security.Authentication;

namespace Tool.Sockets.QuicHelper
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// 封装一个底层异步Quic对象（客户端）写了但属于预览物无法使用
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
    public class QuicClientAsync
#if NET7_0_OR_GREATER
        : INetworkConnect<QuicSocket>
#endif
    {
#if NET7_0_OR_GREATER
        /// <summary>
        /// 获取当前心跳信息
        /// </summary>
        public KeepAlive Keep { get; private set; }

        private readonly int DataLength = 1024 * 8;
        private QuicSocket quicclient;
        private bool isClose = false; //标识客户端连接是否关闭
        private bool isReceive = false; //标识是否调用了接收函数

        private Ipv4Port server; //服务端IP
        private EndPoint endPointServer;
        private int millisecond = 20; //默认20毫秒。
        private bool isWhileReconnect = false;

        /**
        * 连接、发送、关闭事件
        */
        private CompletedEvent<EnClient> Completed; //event

        /**
         * 接收到数据事件
         */
        private ReceiveEvent<QuicSocket> Received; //event

        /// <summary>
        /// 标识客户端连接是否关闭
        /// </summary>
        public bool IsClose { get { return isClose; } }

        /// <summary>
        /// 服务器的连接信息
        /// </summary>
        public UserKey Server { get { return server; } }

        /// <summary>
        /// 当前设备的连接信息
        /// </summary>
        public Ipv4Port LocalPoint => StateObject.GetIpPort(quicclient?.LocalEndPoint);

        /// <summary>
        /// 连接状态
        /// </summary>
        public bool Connected => quicclient.Connected;

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
        /// 设置支持的HTTP协议
        /// </summary>
        public List<SslApplicationProtocol> ApplicationProtocols { get; } = new() { SslApplicationProtocol.Http3 };

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(CompletedEvent<EnClient> Completed) => this.Completed ??= Completed;

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public void SetReceived(ReceiveEvent<QuicSocket> Received)
        {
            if (isReceive) throw new Exception("当前已无法绑定接收委托了，因为ConnectAsync()已经调用了。");
            this.Received ??= Received;
        }

        /// <summary>
        /// 是否使用线程池调度接收后的数据
        /// 默认 true 开启
        /// </summary>
        public bool IsThreadPool { get; set; } = true;

        /// <summary>
        /// 是否在与服务器断开后主动重连？ 
        /// </summary>
        public bool IsReconnect { get; private set; }

        /// <summary>
        /// 禁用掉Receive通知事件，方便上层封装
        /// </summary>
        public bool DisabledReceive { get; init; } = false;

        /// <summary>
        /// 表示通讯的包大小
        /// </summary>
        public NetBufferSize BufferSize { get; }

        #region QuicClientAsync

        /// <summary>
        /// 创建一个 <see cref="QuicClientAsync"/> 客户端类
        /// </summary>
        public QuicClientAsync() : this(NetBufferSize.Default) { }

        /// <summary>
        /// 创建一个 <see cref="QuicClientAsync"/> 客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        public QuicClientAsync(NetBufferSize bufferSize) : this(bufferSize, false) { }

        /// <summary>
        /// 创建一个 <see cref="QuicClientAsync"/> 客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        /// <param name="OnlyData">是否启动框架模式</param>
        public QuicClientAsync(NetBufferSize bufferSize, bool OnlyData) : this(bufferSize, OnlyData, false) { }

        /// <summary>
        /// 创建一个 <see cref="QuicClientAsync"/> 客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        /// <param name="OnlyData">是否启动框架模式</param>
        /// <param name="IsReconnect">是否在与服务器断开后主动重连？ </param>
        public QuicClientAsync(NetBufferSize bufferSize, bool OnlyData, bool IsReconnect)
        {
            if (!QuicConnection.IsSupported) throw new Exception("不支持QUIC，请检查是否存在libmsquic以及是否支持TLS 1.3。");
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
            this.DataLength = (int)bufferSize;
            this.OnlyData = OnlyData;
            this.IsReconnect = IsReconnect;
        }

        #endregion

        #region ConnectAsync

        /// <summary>
        /// 异步连接，连接ip地址为127.0.0.1
        /// </summary>
        /// <param name="port">要连接服务端的端口</param>
        public async Task ConnectAsync(int port)
        {
            await ConnectAsync(StaticData.LocalIp, port);
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="ip">要连接的服务器的ip地址</param>
        /// <param name="port">要连接的服务器的端口</param>
        public async Task ConnectAsync(string ip, int port)
        {
            ThrowIfDisposed();

            string _server = $"{ip}:{port}";
            if (!IPEndPoint.TryParse(_server, out IPEndPoint endPointServer))
            {
                throw new FormatException("ip:port 无法被 IPEndPoint 对象识别！");
            }
            this.endPointServer = endPointServer;
            server = _server;
            await ConnectAsync();
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="dnsEndPoint">域名:端口</param>
        public async Task ConnectAsync(DnsEndPoint dnsEndPoint)
        {
            ThrowIfDisposed();

            ArgumentNullException.ThrowIfNull(dnsEndPoint);
            var ipadrs = await Utils.Utility.GetIPAddressAsync(dnsEndPoint.Host, AddressFamily.InterNetwork) ?? throw new Exception("域名无法解析到服务器！");
            this.endPointServer = dnsEndPoint;
            server = $"{ipadrs}:{dnsEndPoint.Port}";
            await ConnectAsync();
        }

        private async Task ConnectAsync()
        {
            bool isAuthex = false;
            try
            {
                // 连接到服务端
                QuicConnection quicconn = await QuicConnection.ConnectAsync(new QuicClientConnectionOptions
                {
                    DefaultStreamErrorCode = 505,
                    DefaultCloseErrorCode = 500,
                    RemoteEndPoint = endPointServer,
                    ClientAuthenticationOptions = new SslClientAuthenticationOptions
                    {
                        ApplicationProtocols = ApplicationProtocols,
                        //EnabledSslProtocols = SslProtocols.Tls13,
                        RemoteCertificateValidationCallback = (sender, certificate, chain, errors) =>
                        {
                            return true;
                        },
                        //LocalCertificateSelectionCallback = (sender, targetHost, certificates, certificate, acceptableIssuers) =>
                        //{
                        //    return certificate ?? QuicStateObject.GenerateManualCertificate(targetHost);
                        //}
                    },
                    //MaxInboundBidirectionalStreams = 100,
                    //MaxInboundUnidirectionalStreams = 100,
                });
                quicclient = await QuicSocket.QuicSocketAsync(quicconn, false);
            }
            catch (AuthenticationException ex)
            {
                isAuthex = true;
                throw new Exception("证书异常！", ex);
            }
            catch (Exception)
            {
            }
            finally
            {
                if (!isAuthex) ConnectCallBack();
            }
        }

        #endregion

        #region SendAsync

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="msg">要发送的内容</param>
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
        /// 开始异步发送数据
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

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="sendBytes">数据包</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">OnlyData验证失败</exception>
        public async ValueTask SendAsync(SendBytes<QuicSocket> sendBytes)
        {
            if (sendBytes.OnlyData != OnlyData) throw new ArgumentException("与当前套接字协议不一致！", nameof(sendBytes.OnlyData));

            var buffers = sendBytes.GetMemory();
            await SendAsync(buffers, EnClient.SendMsg);
        }

        private async ValueTask SendAsync(Memory<byte> listData, EnClient en)
        {
            ThrowIfDisposed();

            if (!QuicStateObject.IsConnected(quicclient)) throw new Exception("与服务端的连接已断开！");

            try
            {
                await quicclient.SendAsync(listData);

                OnComplete(Server, en);
                if (EnClient.SendMsg == en) Keep?.ResetTime();
            }
            catch
            {
                await InsideClose();
                throw;
            }
        }

        #endregion

        /**
        * 异步接收连接的回调函数
        */
        private void ConnectCallBack()
        {
            if (QuicStateObject.IsConnected(quicclient))
            {
                Debug.WriteLine($"Server [{quicclient.RemoteEndPoint}]: connected");
                //Debug.WriteLine("服务器：{0},连接成功！ {1}", key, DateTime.Now.ToString());

                StateObject.StartReceive("Quic", StartReceive, quicclient); //StartReceive(quicclient);
            }
            else
            {
                InsideClose().Preserve();
                OnComplete(Server, EnClient.Fail);

                StartReconnect();
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private async Task StartReceive(QuicSocket client)
        {
            isReceive = true;
            QuicStateObject obj = new(client, DataLength, OnlyData, Received);
            OnComplete(obj.SocketKey, EnClient.Connect).Wait();
            while (!isClose)//ListClient.TryGetValue(key, out client) && 不允许意外删除对象问题
            {
                await Task.Delay(Millisecond); //Thread.Sleep(Millisecond);
                if (obj.IsConnected())
                {
                    await ReceiveAsync(obj);
                }
                else
                {
                    //如果发生异常，说明客户端失去连接，触发关闭事件
                    await InsideClose();
                    OnComplete(obj.SocketKey, EnClient.Close);
                    StartReconnect();
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
                ReadResult result = await quicclient.ReceiveAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;

                while (obj.TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                {
                    // Process the line. 
                    //ProcessLine(in line);

                    await OnReceived(line, obj);
                }

                quicclient.AdvanceTo(in buffer);

                //if (await obj.ReceiveAsync())
                //{
                //    if (!DisabledReceive) OnComplete(obj.SocketKey, EnClient.Receive);
                //    Keep?.ResetTime();
                //    await obj.OnReceivedAsync(IsThreadPool, obj.Client, Received);
                //    //await Received.InvokeAsync(obj.IpPort, ListData);
                //}

                if (result.IsCompleted) isClose = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                await obj.CloseAsync();
            }

            async ValueTask OnReceived(ReadOnlySequence<byte> listData, QuicStateObject obj)
            {
                if (!DisabledReceive) OnComplete(obj.SocketKey, EnClient.Receive);
                Keep?.ResetTime();
                await obj.OnReceiveAsync(IsThreadPool, listData);
            }
        }

        #region Reconnection

        /// <summary>
        /// 重连，返回是否重连，如果没有断开是不会重连的
        /// </summary>
        public async Task<bool> Reconnection()
        {
            ThrowIfDisposed();

            try
            {
                if (!isClose)
                {
                    if (!QuicStateObject.IsConnected(quicclient))
                    {
                        //client.Abort();
                        //client.Dispose();
                        await ConnectAsync();
                    }
                }
                return true;
            }
            catch
            {
                await InsideClose();
                return false;
            }
        }

        private async Task WhileReconnect()
        {
            try
            {
                while (IsReconnect)
                {
                    if (await Reconnection()) break;
                    await Task.Delay(100); //等待一下才继续
                }
            }
            catch (Exception)
            {
            }
        }

        private void StartReconnect()
        {
            if (!isWhileReconnect)
            {
                isWhileReconnect = true;
                StateObject.StartTask("Quic重连", WhileReconnect);
            }
        }

        #endregion

        /// <summary>
        /// 添加持久化消息（心跳），防止特殊情况下的断开连接
        /// </summary>
        public void AddKeepAlive(byte TimeInterval)
        {
            ThrowIfDisposed();

            if (!OnlyData) throw new InvalidOperationException("OnlyData 未设置成 true 无法提供相关服务。");

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

        /// <summary>
        /// 可供开发重写的事件方法
        /// </summary>
        /// <param name="key">IP：端口</param>
        /// <param name="enAction">消息类型</param>
        public virtual IGetQueOnEnum OnComplete(in UserKey key, EnClient enAction) => EnumEventQueue.OnComplete(in key, enAction, Completed);

        /// <summary>
        /// 创建数据发送空间
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public SendBytes<QuicSocket> CreateSendBytes(int length = 0)
        {
            if (quicclient == null) throw new Exception("未调用ConnectAsync函数！");
            if (length == 0) length = DataLength;
            return new SendBytes<QuicSocket>(quicclient, length, OnlyData);
        }

        /// <summary>
        /// TCP关闭
        /// </summary>
        private async ValueTask InsideClose()
        {
            if (quicclient is null) return;
            await quicclient.CloseAsync();
        }

        /// <summary>
        /// 异步关闭Quic
        /// </summary>
        public async ValueTask CloseAsync()
        {
            IsReconnect = false;
            isClose = true;
            await InsideClose();
            Keep?.Close();
        }

        /// <summary>
        /// 关闭Quic
        /// </summary>
        public void Close() => CloseAsync().Preserve();

        /// <summary>
        /// 关闭连接，回收相关资源
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            Close();
            //client.Dispose();
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
