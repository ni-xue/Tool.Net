using System;

namespace Tool.Sockets.P2PHelpr
{
    /// <summary>
    /// 客户端侧 P2P 协议常量与消息构造/解析。
    /// 与服务端 <see cref="P2pServiceAsync"/> 的格式一一对应。
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    internal static class P2pClientProtocol
    {
        /// <summary>TCP 消息魔术头</summary>
        public static ReadOnlyMemory<byte> TcpMagic { get; } = new byte[] { 111, 121, 212, 222 };

        /// <summary>UDP 就绪消息魔术头</summary>
        public static ReadOnlyMemory<byte> UdpMagic { get; } = new byte[] { 128, 168, 218, 248 };

        /// <summary>服务器 → 客户端：配对成功，载荷含 roundKey + 双方地址</summary>
        public const byte MsgPairReply = 1;

        /// <summary>客户端 → 服务器：申请 Key，Key 由客户端生成并提交</summary>
        public const byte MsgRequestKey = 2;

        /// <summary>客户端 → 服务器：设置 Key，等待方用发起方的 Key 加入配对</summary>
        public const byte MsgSetKey = 3;

        /// <summary>服务器 → 客户端：Key 申请/设置的统一确认，载荷含 ok 字节（1 成功 / 0 失败）</summary>
        public const byte MsgKeyAck = 4;

        public const int MagicLen = 4;
        public const int GuidLen = 16;
        public const int Ipv4PortLen = 6;
        public const int RoleLen = 1;
        public const int OkLen = 1;

        /// <summary>TCP 消息头：魔术4 + 类型1 + Key16 + 角色1 = 22B</summary>
        public const int TcpHeaderLen = MagicLen + 1 + GuidLen + RoleLen;

        /// <summary>Key 确认：头22 + ok1 + 第1轮roundKey16 = 39B</summary>
        public const int KeyAckLen = TcpHeaderLen + OkLen + GuidLen;

        /// <summary>配对回复：头22 + 本轮roundKey16 + 下一轮roundKey16 + 本地地址6 + 对方地址6 = 66B</summary>
        public const int PairReplyLen = TcpHeaderLen + GuidLen + GuidLen + Ipv4PortLen + Ipv4PortLen;

        /// <summary>UDP Or TCP 就绪：魔术4 + roundKey16 + serviceKey16 + 角色1 = 37B</summary>
        public const int RoundReadyLen = MagicLen + GuidLen + GuidLen + RoleLen;

        /// <summary>构造 TCP 消息头：魔术 + 类型 + Key + 目标角色，写入目标 Span。</summary>
        /// <param name="target">目标缓冲区，长度必须 >= <see cref="TcpHeaderLen"/></param>
        /// <param name="type">消息类型</param>
        /// <param name="key">会话 Key</param>
        /// <param name="targetRole">目标角色（服务端回复时标明这条给谁；客户端发送时填本端角色）</param>
        /// <exception cref="ArgumentException">目标缓冲区长度不足</exception>
        public static void WriteTcpHeader(Span<byte> target, byte type, Guid key, P2pRole targetRole)
        {
            if (target.Length < TcpHeaderLen)
                throw new ArgumentException($"目标缓冲区至少需要 {TcpHeaderLen} 字节", nameof(target));

            int off = 0;
            TcpMagic.Span.CopyTo(target[off..]); off += MagicLen;
            target[off] = type; off += 1;
            key.TryWriteBytes(target[off..(off + GuidLen)]); off += GuidLen;
            target[off] = (byte)targetRole;
        }

        /// <summary>构造 UDP Or TCP 就绪消息：魔术 + roundKey + serviceKey，写入目标 Span。</summary>
        /// <param name="target">目标缓冲区，长度必须 >= <see cref="RoundReadyLen"/></param>
        /// <param name="roundKey">本轮 UDP Or TCP 触发的标识</param>
        /// <param name="serviceKey">配对会话的 Key</param>
        /// <param name="role">身份信息</param>
        /// <exception cref="ArgumentException">目标缓冲区长度不足</exception>
        public static void WriteRoundReady(Span<byte> target, Guid roundKey, Guid serviceKey, P2pRole role)
        {
            if (target.Length < RoundReadyLen)
                throw new ArgumentException($"目标缓冲区至少需要 {RoundReadyLen} 字节", nameof(target));

            int off = 0;
            UdpMagic.Span.CopyTo(target[off..]); off += MagicLen;
            roundKey.TryWriteBytes(target[off..(off + GuidLen)]); off += GuidLen;
            serviceKey.TryWriteBytes(target[off..(off + GuidLen)]); off += GuidLen;
            target[off] = (byte)role;
        }

        /// <summary>校验 TCP 消息魔术头</summary>
        public static bool IsTcpMagic(ReadOnlySpan<byte> span)
            => span.Length >= MagicLen && span[..MagicLen].SequenceEqual(TcpMagic.Span);

        /// <summary>校验 UDP 消息魔术头</summary>
        public static bool IsUdpMagic(ReadOnlySpan<byte> span)
            => span.Length >= MagicLen && span[..MagicLen].SequenceEqual(UdpMagic.Span);
    }
}