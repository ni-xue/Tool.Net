using System;

namespace Tool.Sockets.P2PHelpr
{
    /// <summary>会话注册表的复合键：同一个 Key 下区分发起方和等待方</summary>
    /// <remarks>代码由逆血提供支持</remarks>
    internal readonly struct P2pSessionKey : IEquatable<P2pSessionKey>
    {
        /// <summary>配对会话的 Key</summary>
        public Guid Key { get; }

        /// <summary>本端在配对中的角色</summary>
        public P2pRole Role { get; }

        public P2pSessionKey(Guid key, P2pRole role)
        {
            Key = key;
            Role = role;
        }

        public bool Equals(P2pSessionKey other) => Key == other.Key && Role == other.Role;

        public override bool Equals(object obj) => obj is P2pSessionKey other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Key, Role);

        public static bool operator ==(P2pSessionKey left, P2pSessionKey right) => left.Equals(right);
        public static bool operator !=(P2pSessionKey left, P2pSessionKey right) => !left.Equals(right);
    }
}
