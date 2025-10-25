using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Sockets.TrojanHelper
{
    /// <summary>
    /// Trojan 创建 Http 通用工厂
    /// </summary>
    public class TrojanHttpHandlerFactory : IDisposable, IAsyncDisposable
    {
        private int _index;
        private readonly TrojanConnect[] _trojans;
        //private readonly HashSet<TrojanTcpClient> trojanTcpClients;
        private readonly CancellationTokenSource _cancellationTokenSource;

        /// <summary>
        /// 获取当前工厂关闭状态
        /// </summary>
        public CancellationToken Token => _cancellationTokenSource.Token;

        /// <summary>
        /// 获取工厂提供的 <see cref="HttpMessageHandler"/> 对象
        /// </summary>
        public HttpMessageHandler HttpMessageHandler { get; }

        /// <summary>
        /// 设置 TrojanTls 配置信息
        /// </summary>
        public SslClientAuthenticationOptions TrojanSslOptions { get; set; }

        /// <summary>
        /// 设置 Https 配置信息
        /// </summary>
        public SslClientAuthenticationOptions HttpSslOptions { get; set; }

        /// <summary>
        /// 构造工厂
        /// </summary>
        /// <param name="trojans"></param>
        public TrojanHttpHandlerFactory(params TrojanConnect[] trojans)
        {
            if (trojans == null || trojans.Length == 0)
                throw new ArgumentException("Trojan 节点不能为空", nameof(trojans));

            _index = 0;
            _trojans = trojans;
            _cancellationTokenSource = new CancellationTokenSource();
            HttpMessageHandler = new SocketsHttpHandler() { ConnectCallback = HttpConnect, AutomaticDecompression = DecompressionMethods.All };
        }

        private async ValueTask<Stream> HttpConnect(SocketsHttpConnectionContext context, CancellationToken token)
        {
            int current = _index.Increment();
            TrojanConnect selected = _trojans[current % _trojans.Length];

            var trojanClient = new TrojanTcpClient(selected) { SslOptions = TrojanSslOptions };
            await trojanClient.ConnectAsync(token).ConfigureAwait(false);
            var dnsEndPoint = context.DnsEndPoint;
            var stream = await trojanClient.ConnectToTargetAsync(dnsEndPoint.Host, dnsEndPoint.Port, token).ConfigureAwait(false);
            bool isssl = context.InitialRequestMessage.RequestUri?.Scheme.Equals("https", StringComparison.OrdinalIgnoreCase) ?? false;
            if (isssl)
            {
                var sslStream = new SslStream(stream, leaveInnerStreamOpen: false);
                var options = HttpSslOptions != null ? CloneSslOptions(HttpSslOptions) : new SslClientAuthenticationOptions();
                options.TargetHost ??= dnsEndPoint.Host;
                await sslStream.AuthenticateAsClientAsync(options, token);
                stream = sslStream;
            }
            Token.Register(() => trojanClient.Dispose());
            return stream;
        }

        private static SslClientAuthenticationOptions CloneSslOptions(SslClientAuthenticationOptions original)
        {
            return new SslClientAuthenticationOptions
            {
                TargetHost = original.TargetHost,
                EnabledSslProtocols = original.EnabledSslProtocols,
                CertificateRevocationCheckMode = original.CertificateRevocationCheckMode,
                RemoteCertificateValidationCallback = original.RemoteCertificateValidationCallback,
                ClientCertificates = original.ClientCertificates,
                EncryptionPolicy = original.EncryptionPolicy,
                AllowRenegotiation = original.AllowRenegotiation,
                ApplicationProtocols = original.ApplicationProtocols == null ? null : new List<SslApplicationProtocol>(original.ApplicationProtocols)
            };
        }

        /// <summary>
        /// 回收工厂
        /// </summary>
        public void Dispose()
        {
            HttpMessageHandler.Dispose();
            _cancellationTokenSource.Cancel();
            _cancellationTokenSource.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 异步回收工厂
        /// </summary>
        public ValueTask DisposeAsync()
        {
            Dispose();
            GC.SuppressFinalize(this);
            return ValueTask.CompletedTask;
        }
    }
}
