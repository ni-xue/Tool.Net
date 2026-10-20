using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.TcpHelper;
using Tool.Sockets.UdpHelper;

namespace Tool.Sockets.P2PHelpr
{
    /// <summary>
    /// 封装一个可用的P2P打洞的服务商
    /// 帮助实现P2P打洞
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class P2pServiceAsync : IDisposable
    {
        private readonly SemaphoreSlim _objTcp = new(1, 1), _objUdp = new(1, 1);
        private readonly ConcurrentDictionary<Ipv4Port, Ipv4Port> WaitTcpHash = new(), WaitUdpHash = new();

        private readonly bool islog;
        private readonly ILogger _logger;
        private readonly TcpServerAsync tcpServerAsync;
        private readonly UdpServerAsync udpClientAsync;

        async ValueTask Completed(UserKey a1, EnServer b1, DateTime c1, string name)
        {
            switch (b1)
            {
                case EnServer.ClientClose:
                    if (name is "UDP")
                    {
                        WaitUdpHash.TryRemove(a1, out _);
                    }
                    else if (name is "TCP")
                    {
                        WaitTcpHash.TryRemove(a1, out _);
                    }
                    break;
            }
            if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[{name}]IP:{a1} \t{b1} \t{c1:yyyy/MM/dd HH:mm:ss:fffffff}", name, a1, b1, c1);
        }

        async ValueTask ReceivedTcp(ReceiveBytes<Socket> receive)
        {
            await using (receive)
            {
                if (P2pServerAsync.IsP2pAuth(receive.Span, out var ipnum))
                {
                    string ok = $"[TCP]P2P:{ipnum}->{receive.Key} \t{DateTime.Now:yyyy/MM/dd HH:mm:ss:fffffff}";
                    using var sendBytes = tcpServerAsync.CreateSendBytes(receive.Client, 16);
                    sendBytes.SetMemory(P2pServerAsync.TcpTop);
                    sendBytes.SetMemory(ipnum.Span, 4);
                    sendBytes.SetMemory(receive.Key.Span, 10);
                    await tcpServerAsync.SendAsync(sendBytes);
                    if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("{ok}", ok);
                }
                else if (P2pServerAsync.IsP2pWait(receive.Span, out ipnum))
                {
                    await _objTcp.WaitAsync(); // 异步等待
                    try
                    {
                        Ipv4Port self = receive.Key;
                        if (WaitTcpHash.TryRemove(self, out var ipv4) && ipv4 == ipnum) //存在时移除，不保留。
                        {
                            string ok = $"[TCP]P2P:{ipnum}<->{receive.Key} \t{DateTime.Now:yyyy/MM/dd HH:mm:ss:fffffff}";
                            using var sendBytes0 = tcpServerAsync.CreateSendBytes(receive.Client, 10);
                            sendBytes0.SetMemory(self.Span);
                            sendBytes0.SetMemory(P2pServerAsync.TcpTop, 6);

                            using var sendBytes1 = tcpServerAsync.CreateSendBytes(ipv4, 10);
                            sendBytes1.SetMemory(ipv4.Span);
                            sendBytes1.SetMemory(P2pServerAsync.TcpTop, 6);

                            await Task.WhenAll(tcpServerAsync.SendAsync(sendBytes0).AsTask(), tcpServerAsync.SendAsync(sendBytes1).AsTask());
                            if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("{ok}", ok);
                        }
                        else if (tcpServerAsync.ListClient.ContainsKey(ipnum) && !WaitTcpHash.TryAdd(ipnum, self))
                        {
                            WaitTcpHash[ipnum] = self;
                        }
                    }
                    finally
                    {
                        _objTcp.Release();
                    }
                }
            }
        }

        async ValueTask ReceivedUdp(ReceiveBytes<IUdpCore> receive)
        {
            await using (receive)
            {
                if (P2pServerAsync.IsP2pAuth(receive.Span, out var ipnum))
                {
                    string ok = $"[UDP]P2P:{ipnum}->{receive.Key} \t{DateTime.Now:yyyy/MM/dd HH:mm:ss:fffffff}";
                    using var sendBytes = udpClientAsync.CreateSendBytes(receive.Client, 16);
                    sendBytes.SetMemory(P2pServerAsync.UdpTop);
                    sendBytes.SetMemory(ipnum.Span, 4);
                    sendBytes.SetMemory(receive.Key.Span, 10);
                    await udpClientAsync.SendAsync(sendBytes);
                    if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("{ok}", ok);
                }
                else if (P2pServerAsync.IsP2pWait(receive.Span, out ipnum))
                {
                    await _objUdp.WaitAsync(); // 异步等待
                    try
                    {
                        Ipv4Port self = receive.Key;
                        if (WaitUdpHash.TryRemove(self, out var ipv4) && ipv4 == ipnum) //存在时移除，不保留。
                        {
                            string ok = $"[UDP]P2P:{ipnum}<->{receive.Key} \t{DateTime.Now:yyyy/MM/dd HH:mm:ss:fffffff}";
                            using var sendBytes0 = udpClientAsync.CreateSendBytes(receive.Client, 10);
                            sendBytes0.SetMemory(self.Span);
                            sendBytes0.SetMemory(P2pServerAsync.UdpTop, 6);

                            using var sendBytes1 = udpClientAsync.CreateSendBytes(ipv4, 10);
                            sendBytes1.SetMemory(ipv4.Span);
                            sendBytes1.SetMemory(P2pServerAsync.UdpTop, 6);

                            await Task.WhenAll(udpClientAsync.SendAsync(sendBytes0).AsTask(), udpClientAsync.SendAsync(sendBytes1).AsTask());
                            if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("{ok}", ok);
                        }
                        else if (udpClientAsync.ListClient.ContainsKey(ipnum) && !WaitUdpHash.TryAdd(ipnum, self))
                        {
                            WaitUdpHash[ipnum] = self;
                        }
                    }
                    finally
                    {
                        _objUdp.Release();
                    }
                }
            }
        }

        /// <summary>
        /// 构造函数，提供日志输出
        /// </summary>
        /// <param name="logger">日志信息</param>
        public P2pServiceAsync(ILogger logger)
        {
            islog = logger != null;
            _logger = logger;

            EnumEventQueue.OnInterceptor(EnServer.SendMsg, false);
            EnumEventQueue.OnInterceptor(EnServer.Receive, false);
            EnumEventQueue.OnInterceptor(EnServer.HeartBeat, false);

            tcpServerAsync = new(NetBufferSize.Size8K, true) { Millisecond = 0 };
            udpClientAsync = new(NetBufferSize.Size8K, true) { ReplyDelay = 20, Millisecond = 0 };

            tcpServerAsync.SetCompleted((a1, b1, c1) => Completed(a1, b1, c1, "TCP"));
            udpClientAsync.SetCompleted((a1, b1, c1) => Completed(a1, b1, c1, "UDP"));

            tcpServerAsync.SetReceived(ReceivedTcp);
            udpClientAsync.SetReceived(ReceivedUdp);
        }

        /// <summary>
        /// 启动服务商，TCP 和 UDP 必须是同端口 默认监听 0.0.0.0:11111
        /// </summary>
        /// <returns>异步完成</returns>
        public async Task StartAsync() => await StartAsync("0.0.0.0:11111");

        /// <summary>
        /// 启动服务商，TCP 和 UDP 必须是同端口
        /// </summary>
        /// <param name="ipv4Port">可以指定，服务监听端口</param>
        /// <returns>异步完成</returns>
        public async Task StartAsync(Ipv4Port ipv4Port)
        {
            string ip = ipv4Port.Ip.ToString();
            int port = ipv4Port.Port;

            await tcpServerAsync.StartAsync(ip, port);
            await udpClientAsync.StartAsync(ip, port);
        }

        /// <summary>
        /// 关闭服务商
        /// </summary>
        /// <returns>异步完成</returns>
        public async Task StopAsync()
        {
            Dispose();
            await Task.CompletedTask;
        }

        /// <summary>
        /// 回收服务商
        /// </summary>
        public void Dispose()
        {
            tcpServerAsync.Dispose();
            udpClientAsync.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}
