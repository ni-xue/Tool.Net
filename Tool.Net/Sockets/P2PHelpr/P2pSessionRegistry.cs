using System;
using System.Collections.Concurrent;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.P2PHelpr
{
    /// <summary>
    /// 会话注册表：Key → 客户端会话，供共享连接收到的消息按 Key 路由。
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    internal sealed class P2pSessionRegistry
    {
        private static readonly P2pSessionRegistry _instance = new();
        public static P2pSessionRegistry Instance => _instance;

        private readonly ConcurrentDictionary<P2pSessionKey, P2pServerAsync> _sessions = new();

        public void Register(Guid key, P2pRole role, P2pServerAsync session) => _sessions[new P2pSessionKey(key, role)] = session;

        public void Unregister(Guid key, P2pRole role) => _sessions.TryRemove(new P2pSessionKey(key, role), out _);

        public bool TryGet(Guid key, P2pRole role, out P2pServerAsync session) => _sessions.TryGetValue(new P2pSessionKey(key, role), out session);

        /// <summary>
        /// 处理共享 TCP 连接上收到的一条完整消息（不粘包模式下一条即一包）。
        /// </summary>
        public void OnTcpMessage(ReadOnlySpan<byte> span)
        {
            if (span.Length < P2pClientProtocol.TcpHeaderLen) return;
            if (!P2pClientProtocol.IsTcpMagic(span)) return;

            byte type = span[P2pClientProtocol.MagicLen];
            var key = new Guid(span.Slice(P2pClientProtocol.MagicLen + 1, P2pClientProtocol.GuidLen));

            // 头内固定位置：魔术 4 + 类型 1 + Key 16 之后是角色 1B
            int roleOff = P2pClientProtocol.MagicLen + 1 + P2pClientProtocol.GuidLen;
            var targetRole = (P2pRole)span[roleOff];

            // 外层先按身份查会话，查不到直接丢
            if (!TryGet(key, targetRole, out var session)) return;

            switch (type)
            {
                case P2pClientProtocol.MsgKeyAck:
                    if (span.Length != P2pClientProtocol.KeyAckLen)
                    {
                        session.OnKeyAck(false, Guid.Empty);   // 格式错误按失败处理，避免等待方永久挂起
                        break;
                    }
                    bool ok = span[P2pClientProtocol.TcpHeaderLen] == 1;
                    Guid firstRoundKey = Guid.Empty;
                    if (ok)
                    {
                        int ackOff = P2pClientProtocol.TcpHeaderLen + P2pClientProtocol.OkLen;
                        firstRoundKey = new Guid(span.Slice(ackOff, P2pClientProtocol.GuidLen));
                    }
                    session.OnKeyAck(ok, firstRoundKey);
                    break;
                case P2pClientProtocol.MsgPairReply:
                    if (span.Length != P2pClientProtocol.PairReplyLen)
                    {
                        session.OnPairReplyFailed();   // 或 OnPairReply 传一个失败标记
                        break;
                    }
                    int off = P2pClientProtocol.TcpHeaderLen;
                    var udpGuid = new Guid(span.Slice(off, P2pClientProtocol.GuidLen)); off += P2pClientProtocol.GuidLen;
                    var nextRoundKey = new Guid(span.Slice(off, P2pClientProtocol.GuidLen)); off += P2pClientProtocol.GuidLen;
                    var localEP = new Ipv4Port(span.Slice(off, P2pClientProtocol.Ipv4PortLen).ToArray()); off += P2pClientProtocol.Ipv4PortLen;
                    var remoteEP = new Ipv4Port(span.Slice(off, P2pClientProtocol.Ipv4PortLen).ToArray());
                    session.OnPairReply(udpGuid, nextRoundKey, localEP, remoteEP);
                    break;
            }
        }
    }
}