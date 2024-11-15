using System;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Buffers;
using System.IO.Pipelines;
using System.Net;
using System.Threading;
using System.Net.Security;
using System.Collections.Generic;


#if NET7_0_OR_GREATER
using System.Net.Quic;
using System.Runtime.Versioning;
#endif

namespace Tool.Sockets.Kernels
{

#if NET7_0_OR_GREATER
    /// <summary>
    /// 对异步接收时的对象状态的封装，将Websocket与接收到的数据封装在一起
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
    public class QuicStateObject : StateObject
    {
#if NET7_0_OR_GREATER
        private ReceiveEvent<QuicSocket> Received;

        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="Client">对象</param>
        /// <param name="DataLength">包的大小</param>
        /// <param name="OnlyData">是否确保数据有效</param>
        /// <param name="received">任务事件</param>
        public QuicStateObject(QuicSocket Client, int DataLength, bool OnlyData, ReceiveEvent<QuicSocket> received)
        {
            //this.ListData = new Memory<byte>(new byte[DataLength]);
            this.DataLength = DataLength;
            this.Client = Client;
            this.SocketKey = GetIpPort(Client);
            this.Received = received;
            this.OnlyData = OnlyData;
        }

        /// <summary>
        /// 根据 <see cref="QuicSocket"/> 获取当前连接是否已经断开
        /// </summary>
        /// <returns></returns>
        public bool IsConnected() => IsConnected(Client);

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        /// <summary>
        /// 为 Quic 网络服务提供客户端连接。
        /// </summary>
        public QuicSocket Client { get; }

        /// <summary>
        /// 包长度
        /// </summary>
        public int DataLength { get; }

        /// <summary>
        /// 当前对象唯一的连接票据
        /// </summary>
        public UserKey SocketKey { get; }

        ///// <summary>
        ///// 接收的数据
        ///// </summary>
        //public Memory<byte> ListData { get; set; }

        internal async ValueTask OnReceiveAsync(bool IsThreadPool, ReadOnlySequence<byte> listData)
        {
            try
            {
                if (Received is null) return;
                ReceiveBytes<QuicSocket> data;

                data = new ReceiveBytes<QuicSocket>(SocketKey, Client, (int)listData.Length, OnlyData);
                data.SetMemory(listData);

                if (IsThreadPool)
                {
                    QueueUserWorkItem(Received, data);
                }
                else
                {
                    await ReceivedAsync(Received, data);//触发接收事件
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Error($"任务Core{(IsThreadPool ? "池" : "")}异常", ex, "Log/Quic");
            }
        }

        internal bool IsKeepAlive(in ReadOnlySequence<byte> ListSpan)
        {
            //if (true)
            //{

            //}
            return OnlyData && Utils.Utility.SequenceCompare(ListSpan.FirstSpan, KeepAlive.TcpKeepObj.Span);
        }

        internal bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            static bool isTopHead(in ReadOnlySequence<byte> buffer, out int tophead)
            {
                long count = buffer.Length;
                if (HeadSize >= count) { tophead = -1; return false; }
                int head = GetDataHeadTcp(buffer.FirstSpan);
                if (head == -1) throw new NotSupportedException("与数据协议不一致，终止连接！");
                tophead = head + HeadSize;
                return count >= tophead;
            }
            if (buffer.IsEmpty) { line = default; return false; }
            if (OnlyData)
            {
                if (isTopHead(in buffer, out int tophead))
                {
                    line = buffer.Slice(0, tophead);
                    buffer = buffer.Slice(tophead);
                }
                else
                {
                    line = default;
                }
            }
            else
            {
                //// Look for a EOL in the buffer.
                //SequencePosition? position = buffer.PositionOf((byte)'\n');
                //if (position == null) { line = default; return false; }
                //// Skip the line + the \n.
                //line = buffer.Slice(0, position.Value);
                //buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                line = buffer;
            }
            return true;
        }

        /// <summary>
        /// 回收对象所以资源
        /// </summary>
        public async ValueTask CloseAsync()
        {
            //ListData = Memory<byte>.Empty;//new ArraySegment<byte>();
            Received = null;
            await Client.CloseAsync();
        }
#endif

        #region Static

#if NET7_0_OR_GREATER
        /// <summary>
        /// 根据 <see cref="QuicSocket"/> 获取当前连接是否已经断开
        /// </summary>
        /// <param name="Client"><see cref="QuicSocket"/></param>
        /// <returns></returns>
        public static bool IsConnected(QuicSocket Client)
        {
            if (Client == null) return false;
            return Client.Connected;
        }

        /// <summary>
        /// 根据QuicSocket获取IP加端口
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public static Ipv4Port GetIpPort(QuicSocket Client)
        {
            if (Client is not null && Client.RemoteEndPoint is not null)
            {
                return GetIpPort(Client.RemoteEndPoint);
            }
            return EmptyIpv4Port;
        }

        internal static ValueTask<QuicServerConnectionOptions> ServerConnectionOptions(X509Certificate2 certificate2, List<SslApplicationProtocol> ssls)
        {
            return ValueTask.FromResult(new QuicServerConnectionOptions()
            {
                DefaultStreamErrorCode = 505,
                DefaultCloseErrorCode = 500,
                ServerAuthenticationOptions = new SslServerAuthenticationOptions()
                {
                    ApplicationProtocols = ssls,
                    //EnabledSslProtocols = System.Security.Authentication.SslProtocols.Tls13,
                    ServerCertificate = certificate2,
                    //ServerCertificateSelectionCallback = ServerCertificateSelection,
                    RemoteCertificateValidationCallback = RemoteCertificateValidation,
#if NET8_0_OR_GREATER
                    AllowTlsResume = true,
#endif
                    AllowRenegotiation = false,
                    EncryptionPolicy = EncryptionPolicy.RequireEncryption,
                },

                //IdleTimeout = TimeSpan.FromMinutes(1),
                //MaxInboundBidirectionalStreams = 100,
                //MaxInboundUnidirectionalStreams = 10,
            });

            //X509Certificate ServerCertificateSelection(object sender, string hostName)
            //{
            //    return certificate2 ?? GenerateManualCertificate(hostName);
            //}

            static bool RemoteCertificateValidation(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                return true;
            }
        }
#endif

        /// <summary>
        /// 生成证书《本地证书》
        /// </summary>
        /// <param name="name">域名</param>
        /// <returns></returns>
        public static X509Certificate2 GenerateManualCertificate(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) name = "localhost";
            X509Certificate2 cert = null;
            var store = new X509Store($"QuicTransportCertificates", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            if (store.Certificates.Count > 0)
            {
                foreach (var certificate2 in store.Certificates)
                {
                    if (certificate2.Subject.EndsWith(name))
                    {
                        cert = certificate2;// store.Certificates[^1];
                        // rotate key after it expires
                        if (cert.NotAfter < DateTimeOffset.UtcNow)
                        {
                            store.Remove(cert);
                            cert = null;
                        }
                        break;
                    }
                }
            }
            if (cert == null)
            {
                // generate a new cert
                var now = DateTimeOffset.UtcNow;
                SubjectAlternativeNameBuilder sanBuilder = new();
                sanBuilder.AddDnsName(name);
                using var ec = ECDsa.Create(ECCurve.NamedCurves.nistP256);
                CertificateRequest req = new($"CN={name}", ec, HashAlgorithmName.SHA256);
                // Adds purpose
                req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection { new("1.3.6.1.5.5.7.3.1") }, false));// serverAuth 
                // Adds usage
                req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));
                // Adds subject alternate names
                req.CertificateExtensions.Add(sanBuilder.Build());
                // Sign
                using var crt = req.CreateSelfSigned(now, now.AddDays(14)); // 14 days is the max duration of a certificate for this

#if NET9_0_OR_GREATER
                cert = X509CertificateLoader.LoadCertificate(crt.Export(X509ContentType.Pfx));
#else
                cert = new(crt.Export(X509ContentType.Pfx));
#endif

                // Save
                store.Add(cert);
            }
            store.Close();

            //var hash = SHA256.HashData(cert.RawData);
            //var certStr = Convert.ToBase64String(hash);
            //Console.WriteLine($"\n\n\n\n\nCertificate: {certStr}\n\n\n\n"); // <-- you will need to put this output into the JS API call to allow the connection
            return cert;
        }

#endregion
    }

#if NET7_0_OR_GREATER
    /// <summary>
    /// QuicSocket通信模块
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
    public sealed class QuicSocket
    {
#if NET7_0_OR_GREATER
        private readonly SemaphoreSlim slimlock = new(1, 1); //发送数据限流
        private static async ValueTask<QuicStream> OutOrInStreamAsync(QuicConnection connection, bool isServer) => await (isServer ? connection.AcceptInboundStreamAsync() : connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional));

        private QuicSocket(QuicConnection connection, QuicStream stream, bool isServer)
        {
            Connection = connection;
            Stream = stream;
            IsServer = isServer;
            Reader = PipeReader.Create(Stream);
            Writer = PipeWriter.Create(Stream);
        }

        /// <summary>
        /// 使用连接信息创建或获取流
        /// </summary>
        /// <param name="connection">连接信息</param>
        /// <param name="isServer">发起方</param>
        /// <returns></returns>
        public static async ValueTask<QuicSocket> QuicSocketAsync(QuicConnection connection, bool isServer)
        {
            if (connection is null) throw new ArgumentNullException(nameof(connection), "连接对象不能为空！");
            var stream = await OutOrInStreamAsync(connection, isServer);
            return new(connection, stream, isServer);
        }

        /// <summary>
        /// 获取服务端或客户端的new流对象
        /// </summary>
        /// <returns></returns>
        public async ValueTask<QuicStream> OutOrInStreamAsync() => await OutOrInStreamAsync(Connection, IsServer);

        /// <summary>
        /// 连接信息
        /// </summary>
        public bool IsServer { get; }

        /// <summary>
        /// 连接信息
        /// </summary>
        public QuicConnection Connection { get; }

        /// <summary>
        /// Quic默认流信息
        /// </summary>
        public QuicStream Stream { get; }

        private PipeWriter Writer { get; }

        private PipeReader Reader { get; }

        /// <summary>
        /// 当前Quic是否连接
        /// </summary>
        public bool Connected => IsStatus(Stream.ReadsClosed.Status) && IsStatus(Stream.WritesClosed.Status);

        /// <summary>
        /// 用于此连接的远程终结点。
        /// </summary>
        public IPEndPoint RemoteEndPoint => Connection.RemoteEndPoint;

        /// <summary>
        /// 用于此连接的本地终结点。
        /// </summary>
        public IPEndPoint LocalEndPoint => Connection.LocalEndPoint;

        /// <summary>
        /// 向当前默认流发送数据
        /// </summary>
        /// <param name="listData">数据</param>
        /// <returns></returns>
        public async ValueTask SendAsync(Memory<byte> listData)
        {
            try
            {
                await slimlock.WaitAsync();
                FlushResult result = await Writer.WriteAsync(listData);
                if (result.IsCompleted) await CloseAsync();
            }
            finally
            {
                slimlock.Release();
            }
        }

        /// <summary>
        /// 获取从当前流中获取新的数据
        /// </summary>
        /// <returns>结果</returns>
        public async ValueTask<ReadResult> ReceiveAsync()
        {
            return await Reader.ReadAsync();
        }

        /// <summary>
        /// 标记已使用数据的位置
        /// </summary>
        /// <param name="buffer">位置</param>
        public void AdvanceTo(in ReadOnlySequence<byte> buffer)
        {
            Reader.AdvanceTo(buffer.Start, buffer.End);
        }

        /// <summary>
        /// 关闭Quic连接
        /// </summary>
        /// <returns></returns>
        public async ValueTask CloseAsync()
        {
            await Reader.CompleteAsync();
            await Writer.CompleteAsync();
            Stream.Abort(QuicAbortDirection.Both, 500);
            Stream.Close();
            await Stream.DisposeAsync();
            await Connection.DisposeAsync();
            slimlock.Dispose();
        }
#endif

        private static bool IsStatus(TaskStatus status)
        {
            return status switch
            {
                TaskStatus.Created or TaskStatus.WaitingForActivation or TaskStatus.WaitingToRun or TaskStatus.Running => true,
                _ => false,
            };
        }
    }
}
