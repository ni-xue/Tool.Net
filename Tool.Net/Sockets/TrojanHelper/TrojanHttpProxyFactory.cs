using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.TcpHelper;

namespace Tool.Sockets.TrojanHelper
{
    /// <summary>
    /// Trojan 创建 HttpProxy 通用工厂
    /// </summary>
    public partial class TrojanHttpProxyFactory : IDisposable, IAsyncDisposable
    {
        /// <summary>
        /// Http消息头
        /// </summary>
        private static readonly byte[] HeaderPart = new byte[] { 13, 10 };

        /// <summary>
        /// Http消息尾
        /// </summary>
        private static readonly byte[] HeaderEnd = new byte[] { 13, 10, 13, 10 };

        private uint _index;
        private readonly Ipv4Port _ipv4Port;
        private readonly TrojanConnect[] _trojans;
        private readonly ConcurrentDictionary<Ipv4Port, ProxyNet> _trojanTcpClients;
        private readonly CancellationTokenSource _cancellationTokenSource;
        private readonly TcpServerAsync _serverAsync;

        private Func<DnsEndPoint, UserKey, ValueTask<bool>> _proxyOverrideFunc;

        /// <summary>
        /// 用于日志输出
        /// </summary>
        public ILogger Logger { get; init; }

        /// <summary>
        /// 获取当前工厂关闭状态
        /// </summary>
        public CancellationToken Token => _cancellationTokenSource.Token;

        /// <summary>
        /// 设置 TrojanTls 配置信息
        /// </summary>
        public SslClientAuthenticationOptions TrojanSslOptions { get; set; }

        /// <summary>
        /// 构造工厂
        /// </summary>
        /// <param name="ipv4Port">代理地址</param>
        /// <param name="trojans">源地址</param>
        public TrojanHttpProxyFactory(Ipv4Port ipv4Port, params TrojanConnect[] trojans)
        {
            if (trojans == null || trojans.Length == 0)
                throw new ArgumentException("Trojan 节点不能为空", nameof(trojans));

            _ipv4Port = ipv4Port;
            _index = uint.MaxValue;
            _trojans = trojans;
            _cancellationTokenSource = new CancellationTokenSource();
            _trojanTcpClients = new();
            _serverAsync = new(NetBufferSize.Size1024K) { IsThreadPool = false, Millisecond = 0 };
            _serverAsync.SetCompleted(Completed);
            _serverAsync.SetReceived(Received);
        }

        /// <summary>
        /// 构造工厂
        /// </summary>
        /// <param name="ipv4Port">代理地址</param>
        /// <param name="trojans">源地址</param>
        public TrojanHttpProxyFactory(Ipv4Port ipv4Port, IEnumerable<TrojanConnect> trojans) : this(ipv4Port, trojans.ToArray()) { }

        /// <summary>
        /// 绑定连接代理事件（返回 true 时使用当前设备网络访问源站）
        /// </summary>
        /// <param name="proxyOverrideFunc"></param>
        public void SetProxyOverride(Func<DnsEndPoint, UserKey, ValueTask<bool>> proxyOverrideFunc)
        {
            _proxyOverrideFunc = proxyOverrideFunc;
        }

        /// <summary>
        /// 启动代理服务
        /// </summary>
        /// <returns></returns>
        public async ValueTask StartAsync()
        {
            await _serverAsync.StartAsync(_ipv4Port.Ip.ToString(), _ipv4Port.Port);
        }

        /// <summary>
        /// 获取代理源信息
        /// </summary>
        /// <param name="ipv4Port">连接者信息</param>
        /// <returns></returns>
        public IProxyNet GetProxyNet(Ipv4Port ipv4Port) 
        {
            if (_trojanTcpClients.TryGetValue(ipv4Port, out var proxyNet))
            {
                return proxyNet;
            }
            return null;
        }

        private ValueTask Completed(UserKey userKey, EnServer en, DateTime time)
        {
            switch (en)
            {
                case EnServer.Create:
                case EnServer.Fail:
                case EnServer.Connect:
                case EnServer.Close:
                    LogDebug("{userKey}<->{en}<->{time}", userKey, en, time);
                    break;
                case EnServer.ClientClose:
                    LogDebug("本地连接:{userKey}·断开[{Count}]", userKey, _serverAsync.ListClient.Count);
                    if (_trojanTcpClients.TryRemove(userKey, out var tupleToRemove))
                    {
                        tupleToRemove.Dispose();
                    }
                    break;
            }
            return ValueTask.CompletedTask;
        }

        private async ValueTask Received(ReceiveBytes<Socket> receiveBytes)
        {
            await using (receiveBytes)
            {
                try
                {
                    await GetProxyNet(receiveBytes);
                }
                catch (Exception)
                {
                    receiveBytes.Client.Close(); // 异常发生时主动关闭连接
                }
            }
        }

        private void LogDebug(string message, params object[] args)
        {
            if (Logger is not null && Logger.IsEnabled(LogLevel.Debug))
            {
                Logger.LogDebug(message, args);
            }
        }

        private void LogInformation(string message, params object[] args)
        {
            if (Logger is not null && Logger.IsEnabled(LogLevel.Information))
            {
                Logger.LogInformation(message, args);
            }
        }

        private void LogError(string message, params object[] args)
        {
            if (Logger is not null && Logger.IsEnabled(LogLevel.Error))
            {
                Logger.LogError(message, args);
            }
        }

        private async Task GetProxyNet(ReceiveBytes<Socket> received)
        {
            if (_trojanTcpClients.TryGetValue(received.Key, out var proxyNet))
            {
                LogDebug("out {Key}:{Length}", received.Key, received.Length);
                await proxyNet.SendAsync(received.Memory, Token);
            }
            else
            {
                string topstr = GetHttpTop(received.Span);
                if (topstr.StartsWith("CONNECT"))
                {
                    Match match = ConnectRegex().Match(topstr);
                    if (match.Success)
                    {
                        string hostPort = match.Groups[1].Value;
                        var dnsEndPoint = EnsurePortAtEnd(hostPort);
                        var stream = await CreateProxyNet(dnsEndPoint, received.Key);
                        if (stream is not null)
                        {
                            await _serverAsync.SendAsync(received.Key, "HTTP/1.1 200 Connection Established\r\n\r\n");
                        }
                        else
                        {
                            await SendErrorResponse(received.Key, 502, "无法连接到上游服务器。");
                        }
                    }
                    else
                    {
                        received.Client.Close();
                    }
                }
                else
                {
                    Match match = HttpRegex().Match(topstr);
                    if (match.Success)
                    {
                        string url = match.Groups[1].Value;
                        var hostPort = EnsurePortAtEnd(url);
                        var stream = await CreateProxyNet(hostPort, received.Key);
                        if (stream is not null)
                        {
                            try
                            {
                                await stream.SendAsync(received.Memory, Token);
                            }
                            catch (Exception)
                            {
                                await SendErrorResponse(received.Key, 502, "意外终止连接。");
                            }
                        }
                        else
                        {
                            await SendErrorResponse(received.Key, 502, "无法连接到上游服务器。");
                        }
                    }
                    else
                    {
                        received.Client.Close(); // 关闭 Server 的 Socket
                    }
                }
            }
        }

        private async Task<ProxyNet> CreateProxyNet(DnsEndPoint dnsEndPoint, UserKey ipv4Port)
        {
            Func<DnsEndPoint, UserKey, ValueTask<bool>> proxyOverrideFunc = _proxyOverrideFunc;
            if (proxyOverrideFunc is not null && await proxyOverrideFunc(dnsEndPoint, ipv4Port))
            {
                Socket _tcpClient = StateObject.CreateSocket(true, NetBufferSize.Default);
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    var addresses = await Utils.Utility.GetIPAddressListAsync(dnsEndPoint.Host);
                    if (addresses.Length is 0) throw new Exception($"无法解析到：{dnsEndPoint.Host}");
                    var endPoint = new IPEndPoint(addresses[0], dnsEndPoint.Port);
                    await _tcpClient.ConnectAsync(endPoint, Token);
                    stopwatch.Stop();
                    var proxyNet = new ProxyNet(dnsEndPoint, _tcpClient);
                    if (_tcpClient.Connected && _trojanTcpClients.TryAdd(ipv4Port, proxyNet))
                    {
                        Token.Register(() => _tcpClient?.Dispose());
                        ProxyNetReceive(proxyNet, ipv4Port, dnsEndPoint, stopwatch.GetElapsedMs());
                        return proxyNet;
                    }
                    throw new Exception("通信端口通道无法使用！");
                }
                catch (Exception)
                {
                    stopwatch.Stop();
                    _tcpClient.Dispose();
                    LogError("LocalProxy://{Host}:{Port}<->{ipv4Port}·耗时:{GetElapsedMs}ms", dnsEndPoint.Host, dnsEndPoint.Port, ipv4Port, stopwatch.GetElapsedMs());
                    return null;
                }
            }
            else
            {
                var stopwatch = Stopwatch.StartNew();
                uint current = _index.Increment();
                TrojanConnect selected = _trojans[current % _trojans.Length];

                var trojanClient = new TrojanTcpClient(selected) { SslOptions = TrojanSslOptions };
                try
                {
                    await trojanClient.ConnectAsync(Token).ConfigureAwait(false);
                    await trojanClient.ConnectToTargetAsync(dnsEndPoint.Host, dnsEndPoint.Port, Token).ConfigureAwait(false);
                    stopwatch.Stop();
                    var proxyNet = new ProxyNet(dnsEndPoint, trojanClient);
                    if (trojanClient.Connected && _trojanTcpClients.TryAdd(ipv4Port, proxyNet))
                    {
                        Token.Register(() => trojanClient?.Dispose());
                        ProxyNetReceive(proxyNet, ipv4Port, dnsEndPoint, stopwatch.GetElapsedMs());
                        return proxyNet;
                    }
                    throw new Exception("通信端口通道无法使用！");
                }
                catch
                {
                    stopwatch.Stop();
                    trojanClient.Dispose();
                    LogError("TrojanProxy://{Host}:{Port}<->{ipv4Port}·耗时:{GetElapsedMs}ms", dnsEndPoint.Host, dnsEndPoint.Port, ipv4Port, stopwatch.GetElapsedMs());
                    return null;
                }
            }
        }

        private void ProxyNetReceive(ProxyNet proxyNet, Ipv4Port ipv4Port, DnsEndPoint dnsEndPoint, decimal elapsedMs)
        {
            Task.Run(async () =>
            {
                string Name = proxyNet.IsTrojan ? "TrojanProxy" : "LocalProxy";
                LogInformation("{Name}://{Host}:{Port}<->{ipv4Port}·耗时:{GetElapsedMs}ms", Name, dnsEndPoint.Host, dnsEndPoint.Port, ipv4Port, elapsedMs);

                await using SendBytes<Socket> sendBytes = _serverAsync.CreateSendBytes(ipv4Port, 8192);
                try
                {
                    while (!Token.IsCancellationRequested)
                    {
                        int count = await proxyNet.ReceiveAsync(sendBytes.Memory, Token);
                        if (count is 0) break;
                        await _serverAsync.SendAsync(sendBytes[0..count]);
                    }
                }
                catch (OperationCanceledException)
                {
                    // 预期中的取消
                }
                catch (Exception ex)
                {
                    LogError("{Name}:{Host}:{Port} => {msg}", Name, dnsEndPoint.Host, dnsEndPoint.Port, ex.Message);
                }
                finally
                {
                    proxyNet?.Dispose();
                }
            }, Token);
        }

        private async Task SendErrorResponse(Ipv4Port ipv4Port, int statusCode, string message)
        {
            string statusDescription = GetStatusDescription(statusCode); // 根据状态码获取描述
            string response = $"HTTP/1.1 {statusCode} {statusDescription}\r\nContent-Type: text/html; charset=UTF-8\r\nConnection: close\r\n\r\n<html><head><title>{statusCode} {statusDescription}</title></head><body><h1>{statusCode} {statusDescription}</h1><p>{message}</p><hr><address>ProxyServer/1.0</address></body></html>";
            await using SendBytes<Socket> sendBytes = _serverAsync.CreateSendBytes(ipv4Port, Encoding.UTF8.GetByteCount(response));
            Encoding.UTF8.GetBytes(response, sendBytes.Span);
            try
            {
                await _serverAsync.SendAsync(sendBytes);
                sendBytes.Client.Close();
            }
            catch (Exception)
            {
                //发送取消后，存在先断开我的情况
            }
        }

        private static DnsEndPoint EnsurePortAtEnd(string input, int defaultPort = 80)
        {
            if (string.IsNullOrEmpty(input)) throw new ArgumentException("输入不能为空");

            Regex portRegex = IsPortRegex();
            var match = portRegex.Match(input);

            if (match.Success)
            {
                string domain = match.Groups[1].Value;

                if (match.Groups[2].Success)
                {
                    int port = int.Parse(match.Groups[2].Value);
                    // 验证端口范围
                    if (port < 1 || port > 65535)
                        throw new ArgumentException($"端口号超出范围: {port}");

                    return new DnsEndPoint(domain, port);
                }
                else
                {
                    return new DnsEndPoint(domain, defaultPort);
                }
            }

            throw new ArgumentException($"无法解析的格式: {input}");
        }

        private static string GetHttpTop(Span<byte> bytes)
        {
            int partIndex = bytes.IndexOf(HeaderPart);
            if (partIndex != -1)
            {
                string topstr = Encoding.UTF8.GetString(bytes[..partIndex]);
                return topstr;
            }
            throw new Exception("请求头异常，无法解析！");
        }

        /// <summary>
        /// 根据状态码获取描述
        /// </summary>
        /// <param name="statusCode">状态码</param>
        /// <returns></returns>
        public static string GetStatusDescription(int statusCode)
        {
            return statusCode switch
            {
                400 => "Bad Request",
                403 => "Forbidden",
                404 => "Not Found",
                502 => "Bad Gateway",
                503 => "Service Unavailable",
                504 => "Gateway Timeout",
                _ => "Internal Server Error",
            };
        }

        /// <summary>
        /// 关闭代理服务
        /// </summary>
        public void Stop()
        {
            _serverAsync.Stop();
        }

        /// <summary>
        /// 回收工厂
        /// </summary>
        public void Dispose()
        {
            _serverAsync.Dispose();
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

        private class ProxyNet : IProxyNet
        {
            private readonly TrojanTcpClient _trojanTcpClient;
            private readonly Socket _socket;

            private ulong _totalUpBytes;    // 上传流量（发送）
            private ulong _totalDownBytes;  // 下载流量（接收）

            public bool IsTrojan => _trojanTcpClient is not null;

            // 流量统计属性 - 使用 Interlocked 读取
            public ulong TotalUpBytes => Interlocked.Read(ref _totalUpBytes);
            public ulong TotalDownBytes => Interlocked.Read(ref _totalDownBytes);
            public ulong TotalBytes => TotalUpBytes + TotalDownBytes;

            public DnsEndPoint EndPoint { get; }

            public ProxyNet(DnsEndPoint endPoint, TrojanTcpClient trojanTcpClient)
            {
                EndPoint = endPoint;
                _trojanTcpClient = trojanTcpClient;
            }

            public ProxyNet(DnsEndPoint endPoint, Socket socket)
            {
                EndPoint = endPoint;
                _socket = socket;
            }

            /// <summary>
            /// 发送数据
            /// </summary>
            public async ValueTask SendAsync(ReadOnlyMemory<byte> buffer, CancellationToken token)
            {
                if (IsTrojan)
                {
                    await _trojanTcpClient.SendAsync(buffer, token);
                }
                else
                {
                    await _socket.SendAsync(buffer, SocketFlags.None, token);
                }

                // 使用 Interlocked 原子性增加出口流量
                Interlocked.Add(ref _totalUpBytes, (ulong)buffer.Length);
            }

            /// <summary>
            /// 接收数据
            /// </summary>
            public async ValueTask<int> ReceiveAsync(Memory<byte> buffer, CancellationToken token)
            {
                int bytesReceived;
                if (IsTrojan)
                {
                    bytesReceived = await _trojanTcpClient.ReceiveAsync(buffer, token);
                }
                else
                {
                    bytesReceived = await _socket.ReceiveAsync(buffer, SocketFlags.None, token);
                }

                // 使用 Interlocked 原子性增加入口流量
                if (bytesReceived > 0)
                {
                    Interlocked.Add(ref _totalDownBytes, (ulong)bytesReceived);
                }

                return bytesReceived;
            }

            /// <summary>
            /// 获取流量统计信息
            /// </summary>
            public (ulong UpBytes, ulong DownBytes, ulong TotalBytes) GetTrafficStats()
            {
                return (TotalUpBytes, TotalDownBytes, TotalBytes);
            }

            /// <summary>
            /// 重置流量统计
            /// </summary>
            public void ResetTrafficStats()
            {
                Interlocked.Exchange(ref _totalUpBytes, 0);
                Interlocked.Exchange(ref _totalDownBytes, 0);
            }

            /// <summary>
            /// 获取格式化的流量信息
            /// </summary>
            public string GetFormattedTrafficInfo()
            {
                var (up, down, total) = GetTrafficStats();
                return $"上传: {FormatBytes(up)} | 下载: {FormatBytes(down)} | 总计: {FormatBytes(total)}";
            }

            private static string FormatBytes(ulong bytes)
            {
                string[] sizes = { "B", "KB", "MB", "GB" };
                int order = 0;
                double len = bytes;

                while (len >= 1024 && order < sizes.Length - 1)
                {
                    order++;
                    len /= 1024;
                }

                return $"{len:0.##} {sizes[order]}";
            }

            /// <summary>
            /// 回收对象
            /// </summary>
            public void Dispose()
            {
                _trojanTcpClient?.Dispose();
                _socket?.Dispose();
                GC.SuppressFinalize(this);
            }

            /// <summary>
            /// 回收
            /// </summary>
            ~ProxyNet()
            {
                Dispose();
            }
        }

#if NET7_0_OR_GREATER
        [GeneratedRegex(@"CONNECT\s+([^\s]+)\s+HTTP")]
        private static partial Regex ConnectRegex();

        [GeneratedRegex(@"http[s]?:\/\/(?:www\.)?([^\/]+)")]
        private static partial Regex HttpRegex();

        [GeneratedRegex(@"^([\w.-]+)(?::(\d{1,5}))?$")]
        private static partial Regex IsPortRegex();
#else
        private static Regex ConnectRegex() => new(@"CONNECT\s+([^\s]+)\s+HTTP");

        private static Regex HttpRegex() => new(@"http[s]?:\/\/(?:www\.)?([^\/]+)");

        private static Regex IsPortRegex() => new(@"^([\w.-]+)(?::(\d{1,5}))?$");
#endif
    }

    /// <summary>
    /// 代理连接信息
    /// </summary>
    public interface IProxyNet : IDisposable
    {
        /// <summary>
        /// 是否使用Trojan代理
        /// </summary>
        public bool IsTrojan { get; }

        /// <summary>
        /// 源站信息
        /// </summary>
        public DnsEndPoint EndPoint { get; }

        /// <summary>
        /// 出口流量统计
        /// </summary>
        public ulong TotalUpBytes { get; }

        /// <summary>
        /// 入口流量统计
        /// </summary>
        public ulong TotalDownBytes { get; }

        /// <summary>
        /// 出入流量统计
        /// </summary>
        public ulong TotalBytes { get; }

        /// <summary>
        /// 获取全部统计数据
        /// </summary>
        /// <returns></returns>
        public (ulong UpBytes, ulong DownBytes, ulong TotalBytes) GetTrafficStats();

        /// <summary>
        /// 重置流量统计
        /// </summary>
        public void ResetTrafficStats();

        /// <summary>
        /// 获取格式化的流量信息
        /// </summary>
        public string GetFormattedTrafficInfo();
    }

    /// <summary>
    /// 用于ACL管控
    /// </summary>
    public class FlexibleAclService
    {
        // 使用 ConcurrentDictionary 代替 HashSet 实现线程安全
        private readonly ConcurrentDictionary<string, byte> _localDomains = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<string, byte> _proxyDomains = new(StringComparer.OrdinalIgnoreCase);
        private readonly ConcurrentDictionary<Ipv4Port, byte> _localClients = new();
        private readonly ConcurrentDictionary<Ipv4Port, byte> _proxyClients = new();

        /// <summary>
        /// 加载默认的国内域名列表（直连）
        /// </summary>
        public void LoadDefaultLocalDomains()
        {
            var defaultDomains = new[]
            {
                "www.baidu.com", "baidu.com", "*.baidu.com",
                "qq.com", "*.qq.com", "taobao.com", "*.taobao.com",
                "jd.com", "*.jd.com", "weibo.com", "*.weibo.com",
                "zhihu.com", "*.zhihu.com", "bilibili.com", "*.bilibili.com",
                "localhost", "*.local", "127.0.0.1", "::1",
                "*.cn", "cn", "china.com", "*.china.com"
            };

            ImportLocalDomains(defaultDomains);
        }

        #region 域名管理

        /// <summary>
        /// 批量导入直连域名
        /// </summary>
        public void ImportLocalDomains(IEnumerable<string> domains)
        {
            if (domains == null) return;

            foreach (var domain in domains)
            {
                if (!string.IsNullOrWhiteSpace(domain))
                    _localDomains.TryAdd(domain.Trim(), 0);
            }
        }

        /// <summary>
        /// 批量导入代理域名（强制走代理）
        /// </summary>
        public void ImportProxyDomains(IEnumerable<string> domains)
        {
            if (domains == null) return;

            foreach (var domain in domains)
            {
                if (!string.IsNullOrWhiteSpace(domain))
                    _proxyDomains.TryAdd(domain.Trim(), 0);
            }
        }

        /// <summary>
        /// 从文件导入域名列表（每行一个域名）
        /// </summary>
        public async Task ImportLocalDomainsFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            var domains = await File.ReadAllLinesAsync(filePath);
            ImportLocalDomains(domains.Where(d => !string.IsNullOrWhiteSpace(d) && !d.StartsWith("#")));
        }

        /// <summary>
        /// 从文件导入代理域名列表
        /// </summary>
        public async Task ImportProxyDomainsFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            var domains = await File.ReadAllLinesAsync(filePath);
            ImportProxyDomains(domains.Where(d => !string.IsNullOrWhiteSpace(d) && !d.StartsWith("#")));
        }

        /// <summary>
        /// 添加单个直连域名
        /// </summary>
        public bool AddLocalDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain)) return false;
            return _localDomains.TryAdd(domain.Trim(), 0);
        }

        /// <summary>
        /// 添加单个代理域名
        /// </summary>
        public bool AddProxyDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain)) return false;
            return _proxyDomains.TryAdd(domain.Trim(), 0);
        }

        /// <summary>
        /// 移除域名
        /// </summary>
        public void RemoveDomain(string domain)
        {
            if (string.IsNullOrWhiteSpace(domain)) return;

            _localDomains.TryRemove(domain, out _);
            _proxyDomains.TryRemove(domain, out _);
        }

        /// <summary>
        /// 清空所有域名规则
        /// </summary>
        public void ClearAllDomains()
        {
            _localDomains.Clear();
            _proxyDomains.Clear();
        }

        /// <summary>
        /// 获取所有直连域名
        /// </summary>
        public IReadOnlyCollection<string> GetLocalDomains()
        {
            return _localDomains.Keys.ToList().AsReadOnly();
        }

        /// <summary>
        /// 获取所有代理域名
        /// </summary>
        public IReadOnlyCollection<string> GetProxyDomains()
        {
            return _proxyDomains.Keys.ToList().AsReadOnly();
        }

        #endregion

        #region 客户端管理

        /// <summary>
        /// 批量导入直连客户端
        /// </summary>
        public void ImportLocalClients(IEnumerable<Ipv4Port> clients)
        {
            if (clients == null) return;

            foreach (var client in clients)
            {
                if (!client.IsEmpty)
                {
                    _localClients.TryAdd(client, 0);
                }
            }
        }

        /// <summary>
        /// 批量导入代理客户端（强制走代理）
        /// </summary>
        public void ImportProxyClients(IEnumerable<Ipv4Port> clients)
        {
            if (clients == null) return;

            foreach (var client in clients)
            {
                if (!client.IsEmpty)
                {
                    _proxyClients.TryAdd(client, 0);
                }
            }
        }

        /// <summary>
        /// 从文件导入客户端列表（每行一个 IP:Port）
        /// </summary>
        public async Task ImportLocalClientsFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return;

            var lines = await File.ReadAllLinesAsync(filePath);
            var clients = lines
                .Where(line => !string.IsNullOrWhiteSpace(line) && !line.StartsWith("#"))
                .Select(line => ParseIpv4Port(line.Trim()))
                .Where(client => client != null);

            ImportLocalClients(clients!);
        }

        /// <summary>
        /// 添加单个直连客户端
        /// </summary>
        public bool AddLocalClient(Ipv4Port client)
        {
            return _localClients.TryAdd(client, 0);
        }

        /// <summary>
        /// 添加单个代理客户端
        /// </summary>
        public bool AddProxyClient(Ipv4Port client)
        {
            return _proxyClients.TryAdd(client, 0);
        }

        /// <summary>
        /// 移除客户端
        /// </summary>
        public void RemoveClient(Ipv4Port client)
        {
            _localClients.TryRemove(client, out _);
            _proxyClients.TryRemove(client, out _);
        }

        /// <summary>
        /// 清空所有客户端规则
        /// </summary>
        public void ClearAllClients()
        {
            _localClients.Clear();
            _proxyClients.Clear();
        }

        /// <summary>
        /// 获取所有直连客户端
        /// </summary>
        public IReadOnlyCollection<Ipv4Port> GetLocalClients()
        {
            return _localClients.Keys.ToList().AsReadOnly();
        }

        /// <summary>
        /// 获取所有代理客户端
        /// </summary>
        public IReadOnlyCollection<Ipv4Port> GetProxyClients()
        {
            return _proxyClients.Keys.ToList().AsReadOnly();
        }

        private static Ipv4Port ParseIpv4Port(string line)
        {
            try
            {
                return line;
            }
            catch
            {
                return Ipv4Port.Empty;
            }
        }

        #endregion

        /// <summary>
        /// 清空所有规则
        /// </summary>
        public void ClearAll()
        {
            ClearAllDomains();
            ClearAllClients();
        }

        /// <summary>
        /// 判断是否使用本地连接
        /// </summary>
        public ValueTask<bool> ShouldUseLocal(DnsEndPoint dnsEndPoint, Ipv4Port clientInfo)
        {
            string host = dnsEndPoint.Host;

            if (string.IsNullOrEmpty(host))
                return ValueTask.FromResult(false);

            // 1. 首先检查客户端是否在代理列表中（强制走代理）
            if (_proxyClients.ContainsKey(clientInfo))
                return ValueTask.FromResult(false);

            // 2. 检查域名是否在代理列表中（强制走代理）
            if (IsDomainMatch(host, _proxyDomains))
                return ValueTask.FromResult(false);

            // 3. 检查客户端是否在直连列表中
            if (_localClients.ContainsKey(clientInfo))
                return ValueTask.FromResult(true);

            // 4. 检查域名是否在直连列表中
            if (IsDomainMatch(host, _localDomains))
                return ValueTask.FromResult(true);

            // 5. 内网地址和localhost自动直连
            if (host == "localhost" || host == "127.0.0.1" || host == "::1")
                return ValueTask.FromResult(true);

            // 6. 默认走代理
            return ValueTask.FromResult(false);
        }

        private static bool IsDomainMatch(string host, ConcurrentDictionary<string, byte> domainSet)
        {
            // 精确匹配
            if (domainSet.ContainsKey(host))
                return true;

            // 通配符匹配
            foreach (var domain in domainSet.Keys.Where(d => d.StartsWith("*.")))
            {
                var suffix = domain[1..]; // 去掉星号，保留 ".domain.com"
                if (host.EndsWith(suffix, StringComparison.OrdinalIgnoreCase) || host.Equals(domain[2..], StringComparison.OrdinalIgnoreCase)) // 也匹配 "domain.com"
                    return true;
            }

            return false;
        }
    }
}
