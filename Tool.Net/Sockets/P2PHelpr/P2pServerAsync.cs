using Microsoft.Extensions.Logging;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.TcpHelper;
using Tool.Sockets.UdpHelper;
using Tool.Utils;

namespace Tool.Sockets.P2PHelpr
{
    /// <summary>客户端在配对中的角色</summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public enum P2pRole : byte
    {
        /// <summary>发起方：申请 Key</summary>
        InKey = 1,
        /// <summary>等待方：设置 Key</summary>
        OutKey = 2
    }

    /// <summary>服务器下发的配对信息</summary>
    internal readonly struct PairInfo
    {
        /// <summary>本次回复对应的 roundKey</summary>
        public Guid RoundKey { get; }
        /// <summary>下一轮 roundKey（NoMoreRoundKey 表示服务端拒绝继续）</summary>
        public Guid NextRoundKey { get; }
        /// <summary>本端公网地址</summary>
        public Ipv4Port LocalEP { get; }
        /// <summary>对端公网地址</summary>
        public Ipv4Port RemoteEP { get; }

        public PairInfo(Guid roundKey, Guid nextRoundKey, Ipv4Port local, Ipv4Port remote)
        {
            RoundKey = roundKey;
            NextRoundKey = nextRoundKey;
            LocalEP = local;
            RemoteEP = remote;
        }
    }

    /// <summary>
    /// P2P 客户端会话。
    /// <para>每次 <see cref="GetFreeInKeyAsync(ILogger, int)"/> / <see cref="GetFreeOutKeyAsync(Guid, ILogger, int)"/> 创建一个新实例，底层 TCP 长连接由连接池共享。</para>
    /// <para>InKey 申请 Key，OutKey 设置 Key；双方各自发 UDP/TCP 就绪消息，服务器配对后通过 TCP 下发双方公网地址，然后打洞。</para>
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class P2pServerAsync : IDisposable
    {
        /// <summary>本会话角色</summary>
        public P2pRole Role { get; private set; }

        /// <summary>服务端分配的 Key，InKey 时由服务器返回，OutKey 时由调用方传入</summary>
        public Guid Key { get; private set; }

        /// <summary>服务器下发的本端公网地址（配对成功后有效）</summary>
        public Ipv4Port LocalEP { get; private set; }

        /// <summary>服务器下发的对端公网地址（配对成功后有效）</summary>
        public Ipv4Port RemoteEP { get; private set; }

        private readonly TcpClientAsync _network;
        private readonly IPEndPoint _endPoint;
        private readonly ILogger _logger;
        private readonly bool islog;

        /// <summary>等待服务器回复（Key 申请/设置/配对）的 TCS</summary>
        private TaskCompletionSource<PairInfo> _pairTcs;
        private TaskCompletionSource<bool> _ackTcs;

        /// <summary>当前可用的 roundKey（一次性，MsgKeyAck / MsgPairReply 更新）</summary>
        private Guid _roundKey;

        /// <summary>服务端拒绝继续分配 roundKey 的哨兵值</summary>
        private static readonly Guid NoMoreRoundKey = new("ffffffff-ffff-ffff-ffff-ffffffffffff");

        private int _p2pStarted;
        private bool _disposed;

        private P2pServerAsync(TcpClientAsync network, IPEndPoint endPoint, P2pRole role, ILogger logger)
        {
            _network = network;
            _endPoint = endPoint;
            Role = role;
            _logger = logger;
            islog = logger != null;
        }

        #region 静态入口

        private const string DefaultHost = "p2p.nixue.top";
        private const int DefaultPort = 11111;

        private static async ValueTask<IPEndPoint> GetDefaultEndPointAsync()
        {
            var ip = await Utility.GetIPAddressAsync(DefaultHost, AddressFamily.InterNetwork) ?? throw new Exception("无法获取云端服务器信息，请确认网络是否正常！");
            return new IPEndPoint(ip, DefaultPort);
        }

        /// <summary>获取发起方会话（使用内置免费服务商 p2p.nixue.top）</summary>
        public static async ValueTask<P2pServerAsync> GetFreeInKeyAsync(ILogger logger = null, int timeoutMs = 5000)
        {
            var ep = await GetDefaultEndPointAsync();
            return await GetFreeInKeyAsync(ep, logger, timeoutMs);
        }

        /// <summary>获取等待方会话（使用内置免费服务商 p2p.nixue.top）</summary>
        public static async ValueTask<P2pServerAsync> GetFreeOutKeyAsync(Guid key, ILogger logger = null, int timeoutMs = 5000)
        {
            var ep = await GetDefaultEndPointAsync();
            return await GetFreeOutKeyAsync(ep, key, logger, timeoutMs);
        }

        /// <summary>
        /// 获取发起方会话：向服务器申请 Key。
        /// </summary>
        /// <param name="endPoint">信标服务器 TCP 和 UDP 地址</param>
        /// <param name="logger">可选日志</param>
        /// <param name="timeoutMs">等待耗时 默认5秒</param>
        public static async ValueTask<P2pServerAsync> GetFreeInKeyAsync(IPEndPoint endPoint, ILogger logger = null, int timeoutMs = 5000)
        {
            var network = await P2pConnectionPool.Instance.GetOrCreateAsync(endPoint);
            var session = new P2pServerAsync(network, endPoint, P2pRole.InKey, logger);
            await session.RequestKeyAsync(timeoutMs);
            return session;
        }

        /// <summary>
        /// 获取等待方会话：用已有 Key 加入配对。
        /// </summary>
        /// <param name="endPoint">信标服务器 TCP 和 UDP 地址</param>
        /// <param name="key">发起方申请到的 Key</param>
        /// <param name="logger">可选日志</param>
        /// <param name="timeoutMs">等待时长</param>
        public static async ValueTask<P2pServerAsync> GetFreeOutKeyAsync(IPEndPoint endPoint, Guid key, ILogger logger = null, int timeoutMs = 5000)
        {
            var network = await P2pConnectionPool.Instance.GetOrCreateAsync(endPoint);
            var session = new P2pServerAsync(network, endPoint, P2pRole.OutKey, logger);
            await session.SetKeyAsync(key, timeoutMs);
            return session;
        }

        #endregion

        #region Key 申请/设置

        private async Task RequestKeyAsync(int timeoutMs)
        {
            using var cts = new CancellationTokenSource(timeoutMs);
            _ackTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var reg = cts.Token.Register(() => _ackTcs.TrySetCanceled());

            var key = Guid.NewGuid();
            P2pSessionRegistry.Instance.Register(key, Role, this);

            if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[P2P-{role}]申请 Key 发送, key={key}", Role, key);

            using var send = _network.CreateSendBytes(P2pClientProtocol.TcpHeaderLen);
            P2pClientProtocol.WriteTcpHeader(send.Span, P2pClientProtocol.MsgRequestKey, key, Role);
            await _network.SendAsync(send);

            var ok = await _ackTcs.Task;
            if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[P2P-{role}]申请 Key 结果, key={key}, ok={ok}", Role, key, ok);

            if (!ok)
            {
                P2pSessionRegistry.Instance.Unregister(key, Role);
                throw new InvalidOperationException($"申请 Key 被拒绝：{key}");
            }
            Key = key;
        }

        private async Task SetKeyAsync(Guid key, int timeoutMs)
        {
            using var cts = new CancellationTokenSource(timeoutMs);
            _ackTcs = new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);
            using var reg = cts.Token.Register(() => _ackTcs.TrySetCanceled());

            if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[P2P-{role}]设置 Key 发送, key={key}", Role, key);
            P2pSessionRegistry.Instance.Register(key, Role, this);

            using var send = _network.CreateSendBytes(P2pClientProtocol.TcpHeaderLen);
            P2pClientProtocol.WriteTcpHeader(send.Span, P2pClientProtocol.MsgSetKey, key, Role);
            await _network.SendAsync(send);

            var ok = await _ackTcs.Task;
            if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[P2P-{role}]设置 Key 结果, key={key}, ok={ok}", Role, key, ok);

            if (!ok)
            {
                P2pSessionRegistry.Instance.Unregister(key, Role);
                throw new InvalidOperationException($"设置 Key 被拒绝：{key}");
            }
            Key = key;
        }

        #endregion

        #region 服务器消息分发（由注册表回调）

        /// <summary>收到服务器对“设置 Key”的确认</summary>
        internal void OnKeyAck(bool ok, Guid firstRoundKey)
        {
            if (ok) _roundKey = firstRoundKey;   // 第 1 轮
            _ackTcs?.TrySetResult(ok);
        }

        /// <summary>收到服务器配对结果</summary>
        internal void OnPairReply(Guid roundKey, Guid nextRoundKey, Ipv4Port localEP, Ipv4Port remoteEP)
        {
            if (roundKey != _roundKey) return;
            _roundKey = nextRoundKey;
            LocalEP = localEP;
            RemoteEP = remoteEP;
            _pairTcs?.TrySetResult(new PairInfo(roundKey, nextRoundKey, localEP, remoteEP));
        }

        /// <summary>配对回复格式错误，通知等待方失败</summary>
        internal void OnPairReplyFailed()
        {
            _pairTcs?.TrySetException(new InvalidOperationException("配对回复格式错误"));
        }

        #endregion

        #region 主流程：UDP 触发 → 等配对 → 打洞 → 重试

        /// <summary>
        /// 执行 P2P 打洞 - Udp
        /// <para>循环：创建 UDP 随机端口 → 发就绪消息 → 等服务器 TCP 下发对方地址 → 用同一端口打洞；失败则换新端口重试，直到总超时。</para>
        /// </summary>
        /// <param name="client">UDP - P2P</param>
        /// <param name="timedDelay">总超时毫秒数</param>
        public Task P2PConnectAsync(UdpClientAsync client, int timedDelay = 5000) => P2PConnectAsync(client, timedDelay, null);

        /// <summary>
        /// 执行 P2P 打洞 - Tcp
        /// <para>循环：创建 UDP 随机端口 → 发就绪消息 → 等服务器 TCP 下发对方地址 → 用同一端口打洞；失败则换新端口重试，直到总超时。</para>
        /// </summary>
        /// <param name="client">TCP - P2P</param>
        /// <param name="timedDelay">总超时毫秒数</param>
        public Task P2PConnectAsync(TcpClientAsync client, int timedDelay = 5000) => P2PConnectAsync(client, timedDelay, null);

        /// <summary>
        /// 执行 P2P 打洞 - Udp
        /// <para>循环：创建 UDP 随机端口 → 发就绪消息 → 等服务器 TCP 下发对方地址 → 用同一端口打洞；失败则换新端口重试，直到总超时。</para>
        /// </summary>
        /// <param name="client">UDP - P2P</param>
        /// <param name="timedDelay">总超时毫秒数</param>
        /// <param name="holePuncher">连接信息窗口</param>
        public Task P2PConnectAsync(UdpClientAsync client, int timedDelay, Func<P2pRetryInfo, ValueTask> holePuncher = null)
        {
            if (Interlocked.Exchange(ref _p2pStarted, 1) == 1) throw new InvalidOperationException("P2PConnectAsync 只能调用一次，请为新配对申请新的 Key");
            if (timedDelay < 1000) throw new ArgumentOutOfRangeException(nameof(timedDelay), "不能小于 1000ms");
            return client.P2PConnectAsync(async p2PHole =>
            {
                var p2PRetry = await GetP2PRetryAsync(p2PHole.StunSocket, p2PHole.HoleCount, timedDelay - p2PHole.InitElapsedMs, p2PHole.Token);
                if (holePuncher is not null) await holePuncher.Invoke(p2PRetry);
                return p2PRetry;
            }, _logger);
        }

        /// <summary>
        /// 执行 P2P 打洞 - Tcp
        /// <para>循环：创建 UDP 随机端口 → 发就绪消息 → 等服务器 TCP 下发对方地址 → 用同一端口打洞；失败则换新端口重试，直到总超时。</para>
        /// </summary>
        /// <param name="client">TCP - P2P</param>
        /// <param name="timedDelay">总超时毫秒数</param>
        /// <param name="holePuncher">连接信息窗口</param>
        public Task P2PConnectAsync(TcpClientAsync client, int timedDelay, Func<P2pRetryInfo, ValueTask> holePuncher = null)
        {
            if (Interlocked.Exchange(ref _p2pStarted, 1) == 1) throw new InvalidOperationException("P2PConnectAsync 只能调用一次，请为新配对申请新的 Key");
            if (timedDelay < 1000) throw new ArgumentOutOfRangeException(nameof(timedDelay), "不能小于 1000ms");
            return client.P2PConnectAsync(async p2PHole =>
            {
                var p2PRetry = await GetP2PRetryAsync(p2PHole.StunSocket, p2PHole.HoleCount, timedDelay - p2PHole.InitElapsedMs, p2PHole.Token);
                if (holePuncher is not null) await holePuncher.Invoke(p2PRetry);
                return p2PRetry;
            }, _logger);
        }

        //public async Task P2PConnectTestAsync(int timedDelay, Func<P2PRetryInfo, ValueTask> holePuncher = null) //测试代码
        //{
        //    if (Interlocked.Exchange(ref _p2pStarted, 1) == 1) throw new InvalidOperationException("P2PConnectAsync 只能调用一次，请为新配对申请新的 Key");
        //    if (timedDelay < 1000) throw new ArgumentOutOfRangeException(nameof(timedDelay), "不能小于 1000ms");
        //    int round = 0;

        //    Stopwatch sw = Stopwatch.StartNew();
        //    using var cts = new CancellationTokenSource(timedDelay);
        //    CancellationToken token = cts.Token;
        //    var rand = new Random();
        //    while (true)
        //    {
        //        var p2PRetry = await GetP2PRetryAsync(++round, timedDelay - (int)sw.ElapsedMilliseconds, token);
        //        if (holePuncher is not null) await holePuncher.Invoke(p2PRetry);
        //        await Task.Delay(rand.Next(100, 800));
        //    }
        //}

        private async Task<P2pRetryInfo> GetP2PRetryAsync(Socket stunSocket, int round, int timedDelay, CancellationToken token)
        {
            if (_roundKey == NoMoreRoundKey) 
            {
                if (islog && _logger.IsEnabled(LogLevel.Warning)) _logger.LogWarning("[P2P-{role}]服务端拒绝继续分配 roundKey，打洞终止, key={key}, 超时={ms}ms", Role, Key, timedDelay);
                throw new InvalidOperationException("服务端拒绝继续分配 roundKey，打洞终止");
            }

            using var cts = token == CancellationToken.None ? new CancellationTokenSource(timedDelay) : CancellationTokenSource.CreateLinkedTokenSource(token);
            token = cts.Token;

            if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[P2P-{role}]开始打洞, key={key}, 超时={ms}ms", Role, Key, timedDelay);

            // 先准备等待，再发就绪
            var tcs = new TaskCompletionSource<PairInfo>(TaskCreationOptions.RunContinuationsAsynchronously);
            var roundKey = _roundKey;

            Ipv4Port localEP = await GetLocalEndPoint();

            PairInfo pair;
            try
            {
                if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[P2P-{role}]等待配对回复, roundKey={roundKey}", Role, roundKey);
                pair = await WaitPairInfoAsync();

                if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[P2P-{role}]配对回复到达, roundKey={roundKey}, local={local}, remote={remote}, Time={time}", Role, roundKey, pair.LocalEP, pair.RemoteEP, DateTime.UtcNow.Ticks);
                
                try
                {
                    //if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[P2P]开始打洞, local={local}, remote={remote}", localEP, pair.RemoteEP);
                    //await holePuncher(localEP, pair.RemoteEP, token, _logger);
                    //if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[P2P]打洞成功, local={local}, remote={remote}", localEP, pair.RemoteEP);
                    var effectiveDelay = Math.Max(timedDelay, 1000);
                    return new P2pRetryInfo(effectiveDelay, localEP, pair.RemoteEP);
                }
                catch (Exception ex)
                {
                    if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogWarning(ex, "[P2P-{role}] 第 {round} 轮打洞失败, 换新端口重试", Role, round);
                }
            }
            catch (OperationCanceledException)
            {
                if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[P2P-{role}]等待配对超时/取消, roundKey={roundKey}", Role, roundKey);
            }
            catch (InvalidOperationException ex)
            {
                if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogWarning(ex, "[P2P-{role}]配对回复格式错误, roundKey={roundKey}", Role, roundKey);
            }

            async Task<Ipv4Port> GetLocalEndPoint()
            {
                _pairTcs = tcs;
                stunSocket.Bind(new IPEndPoint(IPAddress.Any, 0)); // 绑定本地端口（如果需要固定端口），或让内核分配
                bool isTcp = stunSocket.ProtocolType is ProtocolType.Tcp;

                if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[P2P-{role}]第 {round} 轮, roundKey={roundKey}, local={local}", Role, round, roundKey, stunSocket.LocalEndPoint);

                if (isTcp)
                {
                    using BytesCore bytesCore = new(P2pClientProtocol.RoundReadyLen + 6);
                    P2pClientProtocol.WriteRoundReady(bytesCore.Span[6..], roundKey, Key, Role);
                    StateObject.SetDataHeadTcp(bytesCore.Span, P2pClientProtocol.RoundReadyLen, 0);

                    await stunSocket.ConnectAsync(_endPoint).ConfigureAwait(false); // TCP 先握手
                    await Handshake.TcpAuthenticAtion(stunSocket, false).ConfigureAwait(false);
                    await stunSocket.SendAsync(bytesCore.Memory, SocketFlags.None, token).ConfigureAwait(false);
                    var localEndPoint = (IPEndPoint)stunSocket.LocalEndPoint;
                    Ipv4Port localEP = localEndPoint;

                    if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[P2P-{role}]TCP 就绪已发 1 条 -> {ep}, roundKey={roundKey}, local={local}", Role, _endPoint, roundKey, localEP);

                    return localEP;
                }
                else
                {
                    using BytesCore bytesCore = new(P2pClientProtocol.RoundReadyLen);
                    P2pClientProtocol.WriteRoundReady(bytesCore.Span, roundKey, Key, Role);

                    for (int i = 0; i < 3; i++)
                    {
#if NET5_0
                        await stunSocket.SendToAsync(bytesCore.Array, SocketFlags.None, _endPoint).ConfigureAwait(false);
#else
                        await stunSocket.SendToAsync(bytesCore.Memory, SocketFlags.None, _endPoint, token).ConfigureAwait(false);
#endif
                        await Task.Delay(1, token);
                    }

                    // 发完立刻取地址、关闭 UDP，让内核清掉本地绑定
                    await stunSocket.ConnectAsync(_endPoint).ConfigureAwait(false); // UDP 的 Connect 不握手，只选路
                    var localEndPoint = (IPEndPoint)stunSocket.LocalEndPoint;
                    Ipv4Port localEP = localEndPoint;
                    stunSocket.Close();

                    if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[P2P-{role}]UDP 就绪已发 3 条 -> {ep}, roundKey={roundKey}, local={local}", Role, _endPoint, roundKey, localEP);

                    return localEP;
                }
            }

            async Task<PairInfo> WaitPairInfoAsync()
            {
                using var reg = token.Register(() => tcs.TrySetCanceled());
                return await tcs.Task;
            }

            if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogError("[P2P-{role}]打洞申请超时, key={key}, 共 {round} 轮", Role, Key, round);
            throw new TimeoutException("P2P 打洞申请超时");
        }

        #endregion

        #region 释放

        /// <summary>清理本会话状态，不释放共享连接</summary>
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _ackTcs?.TrySetCanceled();
            _pairTcs?.TrySetCanceled();
            P2pSessionRegistry.Instance.Unregister(Key, Role);
        }

        #endregion
    }
}