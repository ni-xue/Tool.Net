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
using System.Net.Sockets;

namespace Tool.Sockets.QuicHelper
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// 封装一个底层异步Quic对象（客户端）写了但属于预览物无法使用
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
    public class QuicClient
        #if NET7_0_OR_GREATER
        : INetworkConnect<Socket>, IDisposable
        #endif
    {
#if NET7_0_OR_GREATER
        public string Server => throw new NotImplementedException();

        public string LocalPoint => throw new NotImplementedException();

        public bool IsClose => throw new NotImplementedException();

        public bool Connected => throw new NotImplementedException();

        /// <summary>
        /// 禁用掉Receive通知事件，方便上层封装
        /// </summary>
        public bool DisabledReceive { get; init; } = false;

        public void Close()
        {
            throw new NotImplementedException();
        }

        public async void ConnectAsync(string ip, int port)
        {
            // 连接到服务端
            //var connection = await QuicConnection.ConnectAsync(new QuicClientConnectionOptions
            //{
            //    DefaultCloseErrorCode = 0,
            //    DefaultStreamErrorCode = 0,
            //    RemoteEndPoint = new IPEndPoint(IPAddress.Loopback, 9999),
            //    ClientAuthenticationOptions = new SslClientAuthenticationOptions
            //    {
            //        ApplicationProtocols = new List<SslApplicationProtocol> { SslApplicationProtocol.Http3 },
            //        RemoteCertificateValidationCallback = (sender, certificate, chain, errors) =>
            //        {
            //            return true;
            //        }
            //    }
            //});

            //for (int j = 0; j < 5; j++)
            //{
            //    _ = Task.Run(async () => {

            //        // 打开一个出站的双向流
            //        var stream = await connection.OpenOutboundStreamAsync(QuicStreamType.Bidirectional);

            //        var writer = PipeWriter.Create(stream);

            //        Console.WriteLine();

            //        // 写入数据
            //        await Task.Delay(2000);

            //        var message = $"Hello Quic [{stream.Id}] \n";

            //        Console.Write("Send -> " + message);

            //        await writer.WriteAsync(Encoding.UTF8.GetBytes(message));

            //        await writer.CompleteAsync();
            //    });
            //}

            await Task.CompletedTask;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<bool> Reconnection()
        {
            throw new NotImplementedException();
        }

        public void Send(params ArraySegment<byte>[] listData)
        {
            throw new NotImplementedException();
        }

        public void SendAsync(params ArraySegment<byte>[] listData)
        {
            throw new NotImplementedException();
        }

        public void SetCompleted(Func<string, EnClient, DateTime, Task> Completed)
        {
            throw new NotImplementedException();
        }

        public void SetReceived(Func<ReceiveBytes<Socket>, Task> Received)
        {
            throw new NotImplementedException();
        }
#endif


    }
}
