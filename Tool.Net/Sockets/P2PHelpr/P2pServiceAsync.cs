using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.TcpHelper;
using Tool.Utils;

namespace Tool.Sockets.P2PHelpr
{
    /// <summary>
    /// P2P 信标服务端（新版）
    /// <para>TCP：长连接，负责 Key 申请/设置/配对下发/释放清理</para>
    /// <para>UDP：只收“就绪”消息（魔数 + udpGuid + serviceKey），触发 NAT 并记录最新地址，不回复</para>
    /// <para>配对流程：双方 TCP 就绪 → 双方各自发 UDP 就绪 → 服务器按 Key 配对 → 通过 TCP 下发双方公网地址 → 双方打洞</para>
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class P2pServiceAsync : IDisposable
    {
        #region 协议常量

        /// <summary>TCP 消息魔术头，用于校验消息合法性</summary>
        private static Memory<byte> TcpMagic { get; } = new byte[] { 111, 121, 212, 222 };

        /// <summary>UDP 就绪消息魔术头，用于校验消息合法性</summary>
        private static Memory<byte> UdpMagic { get; } = new byte[] { 128, 168, 218, 248 };

        /// <summary>服务器 → 客户端：配对成功，载荷含目标角色 + roundKey + 双方地址</summary>
        private const byte MsgPairReply = 1;

        /// <summary>客户端 → 服务器：申请 Key，Key 由客户端生成并提交</summary>
        private const byte MsgRequestKey = 2;

        /// <summary>客户端 → 服务器：设置 Key，等待方用发起方的 Key 加入配对</summary>
        private const byte MsgSetKey = 3;

        /// <summary>服务器 → 客户端：Key 申请/设置的统一确认，载荷含目标角色 + ok</summary>
        private const byte MsgKeyAck = 4;

        /// <summary>客户端 → 服务器：释放 Key，服务器清理会话</summary>
        private const byte MsgReleaseKey = 5;

        /// <summary>魔术头长度 4B</summary>
        private const int MagicLen = 4;

        /// <summary>Guid 长度 16B</summary>
        private const int GuidLen = 16;

        /// <summary>Ipv4Port 长度 6B（4B IP + 2B 端口）</summary>
        private const int Ipv4PortLen = 6;

        /// <summary>角色字段长度 1B</summary>
        private const int RoleLen = 1;

        /// <summary>ok 字段长度 1B</summary>
        private const int OkLen = 1;

        /// <summary>
        /// TCP 消息头长度：魔术 4B + 类型 1B + Key 16B + 目标角色 1B = 22B
        /// <para>所有 TCP 消息都以此头开始，后接载荷。</para>
        /// </summary>
        private const int TcpHeaderLen = MagicLen + 1 + GuidLen + RoleLen;

        /// <summary>
        /// Key 确认消息总长：头 22B + ok 1B + roundKey 16B = 39B
        /// <para>服务端回复 MsgKeyAck 时使用。</para>
        /// </summary>
        private const int KeyAckLen = TcpHeaderLen + OkLen + GuidLen;

        /// <summary>
        /// 配对回复总长：头 22B + roundKey 16B + New roundKey 16B + 本地地址 6B + 对方地址 6B = 66B
        /// <para>服务端回复 MsgPairReply 时使用。</para>
        /// </summary>
        private const int PairReplyLen = TcpHeaderLen + GuidLen + GuidLen + Ipv4PortLen + Ipv4PortLen;

        /// <summary>
        /// UDP Or TCP 就绪消息总长：魔术 4B + roundKey 16B + serviceKey 16B + RoleLen = 37B
        /// <para>客户端向服务端发送就绪消息时使用，服务端只收不回复。</para>
        /// </summary>
        private const int RoundReadyLen = MagicLen + GuidLen + GuidLen + RoleLen;

        /// <summary>一个 Key 下最多保留的 roundKey 数量</summary>
        private const int MaxRoundKeysPerSession = 20;

        /// <summary>服务端拒绝继续分配 roundKey 的哨兵值（全 F）</summary>
        private static readonly Guid NoMoreRoundKey = new("ffffffff-ffff-ffff-ffff-ffffffffffff");

        #endregion

        #region 会话状态

        /// <summary>单个 roundKey 的双方到位状态</summary>
        private sealed class RoundKeyState
        {
            /// <summary>本轮发起方</summary>
            public P2pRole FirstSendRole;

            /// <summary>本轮配对方</summary>
            public P2pRole LastSendRole;

            /// <summary>发起方本轮的来源公网地址</summary>
            public Ipv4Port InitiatorEP;

            /// <summary>等待方本轮的来源公网地址</summary>
            public Ipv4Port WaiterEP;

            /// <summary>本轮是否已配对下发</summary>
            public bool Paired;

            /// <summary>本轮配对后分配给下一轮的 roundKey（NoMoreRoundKey 表示拒绝继续）</summary>
            public Guid NextRoundKey;
        }

        /// <summary>
        /// 一次配对会话的状态。
        /// <para>由“申请 Key”的发起方创建，等待方通过“设置 Key”加入。</para>
        /// <para>双方各自通过 UDP Or TCP 就绪消息上报当前公网地址，服务器分别记录，配对时互换。</para>
        /// </summary>
        private sealed class Session
        {
            /// <summary>会话唯一标识</summary>
            public Guid Key;

            /// <summary>发起方 TCP UserKey</summary>
            public UserKey InitiatorUser;

            /// <summary>等待方 TCP UserKey</summary>
            public UserKey WaiterUser;

            /// <summary>发起方 TCP 是否在线</summary>
            public bool HasInitiator;

            /// <summary>等待方 TCP 是否在线 / 是否已加入</summary>
            public bool HasWaiter;

            /// <summary>有效 roundKey 集合，容量受 MaxRoundKeysPerSession 限制</summary>
            public Dictionary<Guid, RoundKeyState> RoundKeys = new();

            /// <summary>最后活动时间，用于定时清理</summary>
            public DateTime LastActive;
        }

        #endregion

        #region 字段

        private readonly ILogger _logger;
        private readonly bool islog;

        /// <summary>TCP 信标服务端，承载长连接和信令消息</summary>
        private readonly TcpServerAsync tcpServerAsync;

        /// <summary>UDP 监听 socket，只收就绪消息，不回复</summary>
        private Socket _udpSocket;
        private CancellationTokenSource _udpCts;
        private Task _udpLoop;
        private static readonly TimeSpan SessionTimeout = TimeSpan.FromMinutes(10);

        /// <summary>Key → 会话状态</summary>
        private readonly ConcurrentDictionary<Guid, Session> _sessions = new();

        /// <summary>配对临界区，保证同一 Key 只配对一次</summary>
        private readonly SemaphoreSlim _pairLock = new(1, 1);

        /// <summary>自动清理任务</summary>
        private readonly KeepAlive keepok;

        #endregion

        #region 构造函数与生命周期

        /// <summary>
        /// 构造 P2P 信标服务端。
        /// </summary>
        /// <param name="logger">日志输出器，可为 null 表示不输出日志</param>
        public P2pServiceAsync(ILogger logger)
        {
            islog = logger != null;
            _logger = logger;
            tcpServerAsync = new(NetBufferSize.Size8K, true) { Millisecond = 0 };
            tcpServerAsync.SetCompleted(CompletedTcp);
            tcpServerAsync.SetReceived(ReceivedTcp);

            tcpServerAsync.OnInterceptor(EnServer.SendMsg, false);
            tcpServerAsync.OnInterceptor(EnServer.Receive, false);
            tcpServerAsync.OnInterceptor(EnServer.HeartBeat, false);

            keepok = new(30 * 1000, CleanupExpiredSessions);
        }

        /// <summary>以默认地址 0.0.0.0:11111 启动服务</summary>
        public async Task StartAsync() => await StartAsync("0.0.0.0:11111");

        /// <summary>
        /// 启动服务，TCP 与 UDP 监听同一端口。
        /// </summary>
        /// <param name="ipv4Port">监听地址与端口</param>
        public async Task StartAsync(Ipv4Port ipv4Port)
        {
            string ip = ipv4Port.Ip.ToString();
            int port = ipv4Port.Port;

            await tcpServerAsync.StartAsync(ip, port);
            StartUdpListener(ip, port);
        }

        /// <summary>停止服务并释放资源</summary>
        public async Task StopAsync()
        {
            Dispose();
            await Task.CompletedTask;
        }

        /// <summary>释放所有资源，包括 UDP 监听和 TCP 服务端</summary>
        public void Dispose()
        {
            _udpCts?.Cancel();
            try { _udpSocket?.Close(); } catch { }
            try { _udpLoop?.Wait(1000); } catch { }
            _udpSocket = null;

            keepok.Close();
            tcpServerAsync.Dispose();
            GC.SuppressFinalize(this);
        }

        private async Task CleanupExpiredSessions()
        {
            await _pairLock.WaitAsync();
            try
            {
                var now = DateTime.UtcNow;
                foreach (var kv in _sessions)
                {
                    if (now - kv.Value.LastActive > SessionTimeout) _sessions.TryRemove(kv.Key, out _);
                }
            }
            finally
            {
                _pairLock.Release();
            }
        }

        #endregion

        #region UDP 监听（只收不发）

        /// <summary>
        /// 启动 UDP 监听。只接收就绪消息，不回复、不维护会话。
        /// </summary>
        private void StartUdpListener(string ip, int port)
        {
            _udpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            _udpSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));

            _udpCts = new CancellationTokenSource();
            _udpLoop = Task.Run(() => UdpReceiveLoopAsync(_udpCts.Token));
        }

        /// <summary>
        /// UDP 接收循环：收到包 → 校验魔术 → 解析 roundKey + serviceKey → 记录最新地址。
        /// 解析失败或长度不足的包直接丢弃。
        /// </summary>
        private async Task UdpReceiveLoopAsync(CancellationToken token)
        {
            var buffer = new byte[RoundReadyLen];
            EndPoint remote = new IPEndPoint(IPAddress.Any, 0);

            while (!token.IsCancellationRequested)
            {
                try
                {
#if NET5_0
                    SocketReceiveFromResult result = await Task.Run(() => _udpSocket.ReceiveFromAsync(buffer, SocketFlags.None, remote), token);
#else
                    SocketReceiveFromResult result = await _udpSocket.ReceiveFromAsync(buffer, SocketFlags.None, remote, token);
#endif
                    if (result.ReceivedBytes != RoundReadyLen) continue;
                    await TryParseOnRoundReady("UDP", (IPEndPoint)result.RemoteEndPoint, buffer); //UDP 洞口协议

                    //if (!Utility.SequenceCompare(buffer.AsSpan(0, MagicLen), UdpMagic.Span)) continue;

                    //var roundKey = new Guid(buffer.AsSpan(MagicLen, GuidLen));
                    //var serviceKey = new Guid(buffer.AsSpan(MagicLen + GuidLen, GuidLen));
                    //int roleOff = MagicLen + GuidLen + GuidLen;
                    //var role = (P2pRole)buffer[roleOff];

                    //if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[UDP]收到就绪, from={from}, roundKey={roundKey}, key={key} role={role}", result.RemoteEndPoint, roundKey, serviceKey, role);

                    //await OnRoundReady((IPEndPoint)result.RemoteEndPoint, roundKey, serviceKey, role);
                }
                catch (OperationCanceledException) { break; }
                catch (Exception ex)
                {
                    if (token.IsCancellationRequested || _udpSocket == null) break;
                    if (islog && _logger.IsEnabled(LogLevel.Warning)) _logger.LogWarning(ex, "[UDP]接收异常");
                }
            }
        }

        /// <summary>
        /// 解析一条 UDP Or TCP 就绪消息
        /// </summary>
        private Task TryParseOnRoundReady(string name, Ipv4Port from, Span<byte> span)
        {
            if (Utility.SequenceCompare(span[..MagicLen], UdpMagic.Span))
            {
                var roundKey = new Guid(span.Slice(MagicLen, GuidLen));
                var serviceKey = new Guid(span.Slice(MagicLen + GuidLen, GuidLen));
                int roleOff = MagicLen + GuidLen + GuidLen;
                var role = (P2pRole)span[roleOff];

                if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[{name}]收到就绪, from={from}, roundKey={roundKey}, key={key} role={role}", name, from, roundKey, serviceKey, role);

                return OnRoundReady(from, roundKey, serviceKey, role);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// 处理一条 UDP Or TCP 就绪消息：按发起方/等待方分别记录其最新公网地址和 roundKey，然后尝试配对。
        /// <para>来源地址由服务器从 UDP Or TCP 包的实际源端获取，即 NAT 映射后的公网地址。</para>
        /// </summary>
        private async Task OnRoundReady(Ipv4Port from, Guid roundKey, Guid serviceKey, P2pRole role)
        {
            if (!_sessions.TryGetValue(serviceKey, out var session)) return;

            RoundKeyState state;
            await _pairLock.WaitAsync();
            try
            {
                if (!session.RoundKeys.TryGetValue(roundKey, out state)) return;   // 无效/过期 roundKey，丢弃

                if (state.FirstSendRole is 0) state.FirstSendRole = role; else state.LastSendRole = role;
                if (role is P2pRole.InKey) state.InitiatorEP = from; else state.WaiterEP = from;
                session.LastActive = DateTime.UtcNow;
            }
            finally
            {
                _pairLock.Release();
            }

            await TryPairAsync(serviceKey, roundKey, session, state);
        }

        #endregion

        #region TCP 连接完成/关闭

        /// <summary>
        /// TCP 连接关闭时，按 UserKey 反查会话，清理对应的在线标记；双方都断开时移除会话。
        /// </summary>
        private async ValueTask CompletedTcp(UserKey user, EnServer evt, DateTime time)
        {
            switch (evt)
            {
                case EnServer.Connect:
                    { if (tcpServerAsync.TrySocket(user, out var client)) client.NoDelay = true; }
                    break;
                case EnServer.ClientClose:
                    {
                        await _pairLock.WaitAsync();
                        try
                        {
                            foreach (var kv in _sessions)
                            {
                                var s = kv.Value;
                                if (s.InitiatorUser == user) s.HasInitiator = false;
                                if (s.WaiterUser == user) s.HasWaiter = false;
                                if (!s.HasInitiator && !s.HasWaiter) _sessions.TryRemove(kv.Key, out _);
                            }
                        }
                        finally
                        {
                            _pairLock.Release();
                        }
                    }
                    break;
            }

            if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[TCP][{evt}]IP:{user} \t{time:yyyy/MM/dd HH:mm:ss:fffffff}", evt, user, time);
        }

        #endregion

        #region TCP 消息处理

        /// <summary>
        /// TCP 消息入口：校验魔术 → 解析类型和 Key → 分发到对应处理逻辑。
        /// </summary>
        private async ValueTask ReceivedTcp(ReceiveBytes<Socket> receive)
        {
            await using (receive)
            {
                if (receive.Span.Length < TcpHeaderLen) return;
                if (!Utility.SequenceCompare(receive.Span[..MagicLen], TcpMagic.Span))
                {
                    await TryParseOnRoundReady("TCP", receive.Key, receive.Span); //TCP 洞口协议
                    return;
                }

                byte type = receive.Span[MagicLen];
                var key = new Guid(receive.Span.Slice(MagicLen + 1, GuidLen));
                int roleOff = MagicLen + 1 + GuidLen;
                var role = (P2pRole)receive.Span[roleOff];

                switch (type)
                {
                    case MsgRequestKey:
                        await HandleRequestKey(receive, key, role);
                        break;
                    case MsgSetKey:
                        await HandleSetKey(receive, key, role);
                        break;
                    case MsgReleaseKey:
                        await HandleReleaseKey(receive, key, role);
                        break;
                }
            }
        }

        /// <summary>
        /// 处理“申请 使用 Key”：服务器确认是新 Key，创建会话，把新 Key 返回给发起方。
        /// </summary>
        private async Task HandleRequestKey(ReceiveBytes<Socket> receive, Guid key, P2pRole role)
        {
            // 角色校验：申请只能由发起方发
            if (role != P2pRole.InKey)
            {
                await SendKeyAck(receive, key, role, false, Guid.Empty);
                if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogWarning("[TCP]Key申请拒绝(角色错误): {key}, role={role}", key, role);
                return;
            }

            var session = new Session
            {
                Key = key,
                InitiatorUser = receive.Key,
                HasInitiator = true,
                LastActive = DateTime.UtcNow
            };

            var firstRoundKey = AllocateRoundKeyOrNoMore(session);
            if (firstRoundKey == NoMoreRoundKey)
            {
                await SendKeyAck(receive, key, role, false, Guid.Empty);
                if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[TCP]未分配udpkey: {key} -> {user}", key, receive.Key);
                return;
            }

            if (!_sessions.TryAdd(key, session))
            {
                await SendKeyAck(receive, key, role, false, Guid.Empty);
                if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[TCP]Key申请拒绝(已存在): {key} -> {user}", key, receive.Key);
                return;
            }

            await SendKeyAck(receive, key, role, true, firstRoundKey);
            if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[TCP]Key申请通过: {key} -> {user}", key, receive.Key);
        }

        /// <summary>
        /// 处理“设置 Key”：等待方加入已有会话，标记为在线，回复确认。
        /// <para>Key 不存在时回复失败（0），客户端应重新申请或换 Key。</para>
        /// </summary>
        private async Task HandleSetKey(ReceiveBytes<Socket> receive, Guid key, P2pRole role)
        {
            // 角色校验：设置只能由等待方发
            if (role != P2pRole.OutKey)
            {
                await SendKeyAck(receive, key, role, false, Guid.Empty);
                if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogWarning("[TCP]Key设置拒绝(角色错误): {key}, role={role}", key, role);
                return;
            }
            if (!_sessions.TryGetValue(key, out var session))
            {
                await SendKeyAck(receive, key, role, false, Guid.Empty);   // Key 不存在，拒绝
                if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[TCP]Key设置拒绝(不存在): {key} <- {user}", key, receive.Key);
                return;
            }
            if (session.HasWaiter)
            {
                await SendKeyAck(receive, key, role, false, Guid.Empty);   // 已有等待方，拒绝
                if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[TCP]Key设置拒绝(已占用): {key} <- {user}", key, receive.Key);
                return;
            }
            if (session.RoundKeys.Count == 0)
            {
                await SendKeyAck(receive, key, role, false, Guid.Empty);
                if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[TCP]未分配udpkey: {key} <- {user}", key, receive.Key);
                return;
            }

            var firstRoundKey = NoMoreRoundKey;
            await _pairLock.WaitAsync();
            try
            {
                firstRoundKey = session.RoundKeys.Keys.First();
                session.WaiterUser = receive.Key;
                session.HasWaiter = true;
                session.LastActive = DateTime.UtcNow;
            }
            finally
            {
                _pairLock.Release();
            }

            await SendKeyAck(receive, key, role, true, firstRoundKey);
            if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[TCP]Key设置通过: {key} <- {user}", key, receive.Key);
        }

        /// <summary>向客户端回复“设置 Key”的确认结果</summary>
        private async Task SendKeyAck(ReceiveBytes<Socket> receive, Guid key, P2pRole targetRole, bool ok, Guid firstRoundKey)
        {
            using var send = tcpServerAsync.CreateSendBytes(receive.Client, KeyAckLen);
            int off = 0;

            send.SetMemory(TcpMagic, off); off += MagicLen;
            send.Span[off] = MsgKeyAck; off += 1;
            key.TryWriteBytes(send.Span[off..(off + GuidLen)]); off += GuidLen;
            send.Span[off] = (byte)targetRole; off += 1;
            send.Span[off] = ok ? (byte)1 : (byte)0; off += 1;
            firstRoundKey.TryWriteBytes(send.Span[off..(off + GuidLen)]);

            await tcpServerAsync.SendAsync(send);
        }

        /// <summary>
        /// 处理“释放 Key”：移除会话，清理双方的 UserKey 映射。
        /// </summary>
        private async Task HandleReleaseKey(ReceiveBytes<Socket> receive, Guid key, P2pRole role)
        {
            if (!_sessions.TryGetValue(key, out var session)) return;

            await _pairLock.WaitAsync();
            try
            {
                if (role == P2pRole.InKey) session.HasInitiator = false; else session.HasWaiter = false;
            }
            finally
            {
                _pairLock.Release();
            }

            // 双方都释放了才移除会话
            if (!session.HasInitiator && !session.HasWaiter)
                _sessions.TryRemove(key, out _);
        }

        #endregion

        #region 配对

        /// <summary>
        /// 尝试配对：双方 TCP 就绪且双方 UDP 就绪时，互换公网地址，通过 TCP 下发。
        /// <para>配对锁保证同一 Key 只配对一次，Paired 标记防止重复下发。</para>
        /// </summary>
        private async Task TryPairAsync(Guid key, Guid roundKey, Session session, RoundKeyState state)
        {
            await _pairLock.WaitAsync();
            try
            {
                if (state.Paired) return;
                if (state.InitiatorEP == Ipv4Port.Empty || state.WaiterEP == Ipv4Port.Empty) return;

                state.Paired = true;
                state.NextRoundKey = AllocateRoundKeyOrNoMore(session);
            }
            finally
            {
                _pairLock.Release();
            }

            Stopwatch sw = Stopwatch.StartNew();

            // 本轮先发：自己的公网地址 + 等待方的公网地址
            await SendByRoleAsync(state.FirstSendRole);

            // 本轮后发：自己的公网地址 + 发起方的公网地址
            await SendByRoleAsync(state.LastSendRole);

            //await Task.WhenAll(taskA, taskB); //一起完成

            if (islog && _logger.IsEnabled(LogLevel.Information)) _logger.LogInformation("[TCP]配对成功: {key},roundKey={roundKey} -> next={next} {ms:F3}ms [{time:yy/MM/dd HH:mm:ss:fffff}]", key, roundKey, state.NextRoundKey, sw.Elapsed.TotalMilliseconds, DateTime.Now);

            Task SendByRoleAsync(P2pRole targetRole)
            {
                Ipv4Port
                    user = targetRole is P2pRole.InKey ? session.InitiatorUser : session.WaiterUser,
                    localEP = targetRole is P2pRole.InKey ? state.InitiatorEP : state.WaiterEP,
                    remoteEP = targetRole is P2pRole.InKey ? state.WaiterEP : state.InitiatorEP;
                return SendPairReply(user, key, targetRole, roundKey, state.NextRoundKey, localEP, remoteEP);
            }
        }

        /// <summary>
        /// 向一端下发配对结果：带 Key、该端最近一次 roundKey、该端公网地址、对方公网地址。
        /// <para>客户端用 roundKey 校验“这条回复对应我哪一轮 UDP 触发”，避免多轮重试串台。</para>
        /// </summary>
        private async Task SendPairReply(Ipv4Port user, Guid key, P2pRole targetRole, Guid roundKey, Guid nextRoundKey, Ipv4Port localEP, Ipv4Port remoteEP)
        {
            try
            {
                if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[TCP]发送配对回复 -> {user}, role={role}, key={key}", user, targetRole, key);
                using var send = tcpServerAsync.CreateSendBytes(user, PairReplyLen);
                int off = 0;
                send.SetMemory(TcpMagic, off); off += MagicLen;
                send.Span[off] = MsgPairReply; off += 1;
                key.TryWriteBytes(send.Span[off..(off + GuidLen)]); off += GuidLen;
                send.Span[off] = (byte)targetRole; off += 1;
                roundKey.TryWriteBytes(send.Span[off..(off + GuidLen)]); off += GuidLen;
                nextRoundKey.TryWriteBytes(send.Span[off..(off + GuidLen)]); off += GuidLen;
                localEP.Span.CopyTo(send.Span[off..(off + Ipv4PortLen)]); off += Ipv4PortLen;
                remoteEP.Span.CopyTo(send.Span[off..(off + Ipv4PortLen)]);
                await tcpServerAsync.SendAsync(send);
                if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[TCP]配对回复已发送 -> {user}, role={role}", user, targetRole);
            }
            catch (Exception ex)
            {
                if (islog) _logger.LogWarning(ex, "[TCP]配对回复发送失败({Role}): {key}", key, targetRole);
            }
        }

        private static Guid AllocateRoundKeyOrNoMore(Session session)
        {
            if (session.RoundKeys.Count >= MaxRoundKeysPerSession) return NoMoreRoundKey;

            var newKey = Guid.NewGuid();
            session.RoundKeys[newKey] = new RoundKeyState();
            return newKey;
        }

        #endregion
    }
}