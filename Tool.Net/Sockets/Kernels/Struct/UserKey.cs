using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 用于通信模块Key数据模型
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public readonly struct UserKey : IEquatable<UserKey>
    {
        /// <summary>
        /// 获取一个空类型
        /// </summary>
        public static UserKey Empty { get; } = new();

        internal readonly Memory<byte> Bytes;

        /// <summary>
        /// 当前Key信息内存数据
        /// </summary>
        public readonly Span<byte> Span => Bytes.Span;

        /// <summary>
        /// 判断当前值是否为空
        /// </summary>
        public readonly bool IsEmpty => Bytes.IsEmpty;

        /// <summary>
        /// 判断当前值是否是<see cref="Ipv4Port"/>值
        /// </summary>
        public readonly bool IsIpv4Port { get; }

        /// <summary>
        /// 将常见Key值转换成可用的<see cref="UserKey"/>
        /// </summary>
        /// <param name="ipv4Port"><see cref="UserKey"/>值</param>
        public UserKey(Ipv4Port ipv4Port)
        {
            IsIpv4Port = true;
            Bytes = ipv4Port.Bytes;
        }

        /// <summary>
        /// 将常见Key值转换成可用的<see cref="UserKey"/>
        /// </summary>
        /// <param name="key"><see cref="string"/>值</param>
        public UserKey(string key)
        {
            if (StateObject.IsIpPort(key, out Ipv4Port ipv4Port))
            {
                IsIpv4Port = true;
                Bytes = ipv4Port.Bytes;
            }
            else
            {
                IsIpv4Port = false;
                Bytes = Encoding.UTF8.GetBytes(key);
            }
        }

        /// <summary>
        /// 获取UserKey信息
        /// </summary>
        /// <returns>结果</returns>
        public override readonly string ToString()
        {
            if (IsEmpty) throw new Exception("无效UserKey！");
            if (IsIpv4Port)
            {
                return $"{Span[0]}.{Span[1]}.{Span[2]}.{Span[3]}:{BitConverter.ToUInt16(Span[4..6])}";
            }
            else
            {
                return Encoding.UTF8.GetString(Span);
            }
        }

        /// <summary>
        /// 判断是否一致
        /// </summary>
        /// <param name="other">比较值</param>
        /// <returns>是或否</returns>
        public bool Equals(UserKey other) => Utils.Utility.SequenceCompare(Span, other.Span);

        /// <summary>
        /// 比较两个值是否一致
        /// </summary>
        /// <param name="obj">比较值</param>
        /// <returns>是或否</returns>
        public override bool Equals(object obj) => obj is UserKey other && Equals(other);

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode() => StateObject.HashCodeByte(Bytes);

        /// <summary>
        /// 指示两个 <see cref="UserKey"/> 结构是否不相等。
        /// </summary>
        /// <param name="a">不等运算符左侧的结构</param>
        /// <param name="b">不等运算符右侧的结构</param>
        /// <returns>如果 true 不等于 a，则为 b；否则为 false。</returns>
        public static bool operator !=(UserKey a, UserKey b) => !a.Equals(b);

        /// <summary>
        /// 指示两个 <see cref="UserKey"/> 结构是否相等。
        /// </summary>
        /// <param name="a">相等运算符左侧的结构</param>
        /// <param name="b">相等运算符右侧的结构</param>
        /// <returns>如果 true 等于 a，则为 b；否则为 false。</returns>
        public static bool operator ==(UserKey a, UserKey b) => a.Equals(b);


        /// <summary>
        /// 指示<see cref="UserKey"/> 和 <see cref="Ipv4Port"/> 结构是否不相等。
        /// </summary>
        /// <param name="a">不等运算符左侧的结构</param>
        /// <param name="b">不等运算符右侧的结构</param>
        /// <returns>如果 true 不等于 a，则为 b；否则为 false。</returns>
        public static bool operator !=(UserKey a, Ipv4Port b) => !a.Equals(b);

        /// <summary>
        /// 指示<see cref="UserKey"/> 和 <see cref="Ipv4Port"/> 结构是否相等。
        /// </summary>
        /// <param name="a">相等运算符左侧的结构</param>
        /// <param name="b">相等运算符右侧的结构</param>
        /// <returns>如果 true 等于 a，则为 b；否则为 false。</returns>
        public static bool operator ==(UserKey a, Ipv4Port b) => a.Equals(b);

        /// <summary>
        ///  定义从 <see cref="string"/> 对象到 <see cref="UserKey"/> 对象的隐式转换。
        /// </summary>
        /// <param name="ipport">要转换的对象。</param>
        /// <returns>转换的 <see cref="UserKey"/> 对象。</returns>
        public static implicit operator UserKey(string ipport) => ipport is null ? Empty : new(ipport);

        /// <summary>
        ///  定义从 <see cref="Ipv4Port"/> 对象到 <see cref="UserKey"/> 对象的隐式转换。
        /// </summary>
        /// <param name="ipport">要转换的对象。</param>
        /// <returns>转换的 <see cref="UserKey"/> 对象。</returns>
        public static implicit operator UserKey(Ipv4Port ipport) => new(ipport);

        /// <summary>
        ///  定义从 <see cref="UserKey"/> 对象到 <see cref="string"/> 对象的隐式转换。
        /// </summary>
        /// <param name="ipport">要转换的对象。</param>
        /// <returns>转换的 <see cref="string"/> 对象。</returns>
        public static implicit operator string(UserKey ipport) => ipport.ToString();

        private string GetDebuggerDisplay() => ToString();
    }
}
