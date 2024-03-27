using System;
using System.Collections.Generic;
using System.Net;
#if NET7_0_OR_GREATER
using System.Net.Quic;
#endif
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using System.Buffers;
using System.IO.Pipelines;
using System.Threading;
using System.Runtime.Versioning;
using System.IO;

namespace Tool.Sockets.QuicHelper
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// 封装一个底层异步Quic对象（服务端）写了但属于预览物无法使用
    /// </summary>
#else
    /// <summary>
    /// .Net7 以上支持
    /// </summary>
#endif
    [SupportedOSPlatform("linux")]
    [SupportedOSPlatform("macOS")]
    [SupportedOSPlatform("OSX")]
    [SupportedOSPlatform("windows")]
    public class QuicServer
        #if NET7_0_OR_GREATER
        : INetworkListener<Socket>, IDisposable
        #endif
    {

#if NET7_0_OR_GREATER

        //System.Net.Quic
        public string Server => throw new NotImplementedException();

        public bool IsClose => throw new NotImplementedException();

        public IReadOnlyDictionary<string, Socket> ListClient => throw new NotImplementedException();

        public bool IsThreadPool => throw new NotImplementedException();

        /// <summary>
        /// 禁用掉Receive通知事件，方便上层封装
        /// </summary>
        public bool DisabledReceive { get; init; } = false;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Send(Socket client, params ArraySegment<byte>[] listData)
        {
            throw new NotImplementedException();
        }

        public void SendAsync(Socket client, params ArraySegment<byte>[] listData)
        {
            throw new NotImplementedException();
        }

        public void SetCompleted(Func<string, EnServer, DateTime, Task> Completed)
        {
            throw new NotImplementedException();
        }

        public void SetReceived(Func<ReceiveBytes<Socket>, Task> Received)
        {
            throw new NotImplementedException();
        }

        public async void StartAsync(string ip, int port)
        {
            //System.Net.Quic.QuicConnection
            // 创建 QuicListener
            //var listener = await QuicListener.ListenAsync(new QuicListenerOptions
            //{
            //    ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
            //    ListenEndPoint = new IPEndPoint(IPAddress.Loopback, 9999),
            //    ConnectionOptionsCallback = (connection, ssl, token) => ValueTask.FromResult(new QuicServerConnectionOptions()
            //    {
            //        DefaultStreamErrorCode = 0,
            //        DefaultCloseErrorCode = 0,
            //        ServerAuthenticationOptions = new SslServerAuthenticationOptions()
            //        {
            //            ApplicationProtocols = new List<SslApplicationProtocol>() { SslApplicationProtocol.Http3 },
            //            ServerCertificate = GenerateManualCertificate()
            //        }
            //    })
            //});

            //var connection = await listener.AcceptConnectionAsync();

            //Console.WriteLine($"Client [{connection.RemoteEndPoint}]: connected");

            //var cts = new CancellationTokenSource();

            //while (!cts.IsCancellationRequested)
            //{
            //    var stream = await connection.AcceptInboundStreamAsync();

            //    //Console.WriteLine($"Stream [{stream.Id}]: created");

            //    //Console.WriteLine();

            //    _ = ProcessLinesAsync(stream);
            //}

            //Console.ReadKey();

            await Task.CompletedTask;
        }

        public bool TrySocket(string key, out Socket client)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// 处理流数据
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        private async Task ProcessLinesAsync(Stream stream)//QuicStream
        {
            var reader = PipeReader.Create(stream);
            var writer = PipeWriter.Create(stream);

            while (true)
            {
                ReadResult result = await reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;

                while (TryReadLine(ref buffer, out ReadOnlySequence<byte> line))
                {
                    // Process the line. 
                    ProcessLine(line);

                    // Ack 
                    //await writer.WriteAsync(System.Text.Encoding.UTF8.GetBytes($"ack: {DateTime.Now.ToString("HH:mm:ss")} \n"));
                }

                // Tell the PipeReader how much of the buffer has been consumed.
                reader.AdvanceTo(buffer.Start, buffer.End);

                // Stop reading if there's no more data coming.
                if (result.IsCompleted)
                {
                    break;
                }
            }

            //Console.WriteLine($"Stream [{stream.Id}]: completed");

            await reader.CompleteAsync();
            await writer.CompleteAsync();
        }
#endif

        /// <summary>
        /// 生成证书《本地证书》
        /// </summary>
        /// <returns></returns>
        public X509Certificate2 GenerateManualCertificate()
        {
            X509Certificate2 cert = null;
            var store = new X509Store("KestrelWebTransportCertificates", StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            if (store.Certificates.Count > 0)
            {
                cert = store.Certificates[^1];

                // rotate key after it expires
                if (DateTime.Parse(cert.GetExpirationDateString(), null) < DateTimeOffset.UtcNow)
                {
                    cert = null;
                }
            }
            if (cert == null)
            {
                // generate a new cert
                var now = DateTimeOffset.UtcNow;
                SubjectAlternativeNameBuilder sanBuilder = new();
                sanBuilder.AddDnsName("localhost");
                using var ec = ECDsa.Create(ECCurve.NamedCurves.nistP256);
                CertificateRequest req = new("CN=localhost", ec, HashAlgorithmName.SHA256);
                // Adds purpose
                req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(new OidCollection{ new("1.3.6.1.5.5.7.3.1") }, false));// serverAuth 
                // Adds usage
                req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature, false));
                // Adds subject alternate names
                req.CertificateExtensions.Add(sanBuilder.Build());
                // Sign
                using var crt = req.CreateSelfSigned(now, now.AddDays(14)); // 14 days is the max duration of a certificate for this
                cert = new(crt.Export(X509ContentType.Pfx));

                // Save
                store.Add(cert);
            }
            store.Close();

            //var hash = SHA256.HashData(cert.RawData);
            //var certStr = Convert.ToBase64String(hash);
            //Console.WriteLine($"\n\n\n\n\nCertificate: {certStr}\n\n\n\n"); // <-- you will need to put this output into the JS API call to allow the connection
            return cert;
        }

        bool TryReadLine(ref ReadOnlySequence<byte> buffer, out ReadOnlySequence<byte> line)
        {
            // Look for a EOL in the buffer.
            SequencePosition? position = buffer.PositionOf((byte)'\n');

            if (position == null)
            {
                line = default;
                return false;
            }

            // Skip the line + the \n.
            line = buffer.Slice(0, position.Value);
            buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
            return true;
        }

        void ProcessLine(in ReadOnlySequence<byte> buffer)
        {
            foreach (var segment in buffer)
            {
                Console.WriteLine("Recevied -> " + System.Text.Encoding.UTF8.GetString(segment.Span));
            }

            Console.WriteLine();
        }
    }
}
