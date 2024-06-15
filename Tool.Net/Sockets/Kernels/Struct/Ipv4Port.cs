using Microsoft.AspNetCore.DataProtection.KeyManagement;
using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 用于IP:Port信息
    /// </summary>
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public readonly struct Ipv4Port : IEquatable<Ipv4Port>
    {
        /// <summary>
        /// 获取一个空类型
        /// </summary>
        public static Ipv4Port Empty { get; } = new();

        internal readonly Memory<byte> Bytes;

        /// <summary>
        /// 当前IP:Port信息内存数据
        /// </summary>
        public readonly Span<byte> Span => Bytes.Span;

        /// <summary>
        /// 判断当前值是否为空
        /// </summary>
        public readonly bool IsEmpty => Bytes.IsEmpty;

        /// <summary>
        /// 获取 <see cref="IPAddress"/> 对象
        /// </summary>
        public readonly IPAddress Ip => new(Span[..4]);

        /// <summary>
        /// 获取端口号
        /// </summary>
        public readonly ushort Port => BitConverter.ToUInt16(Span[4..]);

        /// <summary>
        /// 将已有内存数据转换成IP:Port信息
        /// </summary>
        /// <param name="bytes">内存数据</param>
        internal Ipv4Port(in Memory<byte> bytes) => Bytes = bytes;

        /// <summary>
        /// 将IP:Port信息拷贝到内存
        /// </summary>
        /// <param name="destination">内存数据</param>
        public readonly void CopyTo(Span<byte> destination) => Span.CopyTo(destination);

        /// <summary>
        /// 获取IP:Port信息
        /// </summary>
        /// <returns>结果</returns>
        public override readonly string ToString()
        {
            if (IsEmpty) throw new Exception("无效Ipv4Port！");
            return $"{Span[0]}.{Span[1]}.{Span[2]}.{Span[3]}:{BitConverter.ToUInt16(Span[4..6])}";
        }

        /// <summary>
        /// 判断是否一致
        /// </summary>
        /// <param name="other">比较值</param>
        /// <returns>是或否</returns>
        public bool Equals(Ipv4Port other) => Utils.Utility.SequenceCompare(Span, other.Span);

        /// <summary>
        /// 比较两个值是否一致
        /// </summary>
        /// <param name="obj">比较值</param>
        /// <returns>是或否</returns>
        public override bool Equals(object obj) => obj is Ipv4Port other && Equals(other);

        /// <summary>
        /// 获取哈希值
        /// </summary>
        /// <returns>哈希值</returns>
        public override int GetHashCode() => StateObject.HashCodeByte(in Bytes);

        /// <summary>
        /// 指示两个 <see cref="Ipv4Port"/> 结构是否不相等。
        /// </summary>
        /// <param name="a">不等运算符左侧的结构</param>
        /// <param name="b">不等运算符右侧的结构</param>
        /// <returns>如果 true 不等于 a，则为 b；否则为 false。</returns>
        public static bool operator !=(Ipv4Port a, Ipv4Port b) => !a.Equals(b);

        /// <summary>
        /// 指示两个 <see cref="Ipv4Port"/> 结构是否相等。
        /// </summary>
        /// <param name="a">相等运算符左侧的结构</param>
        /// <param name="b">相等运算符右侧的结构</param>
        /// <returns>如果 true 等于 a，则为 b；否则为 false。</returns>
        public static bool operator ==(Ipv4Port a, Ipv4Port b) => a.Equals(b);

        /// <summary>
        ///  定义从 <see cref="string"/> 对象到 <see cref="Ipv4Port"/> 对象的隐式转换。
        /// </summary>
        /// <param name="ipport">要转换的对象。</param>
        /// <returns>转换的 <see cref="Ipv4Port"/> 对象。</returns>
        public static implicit operator Ipv4Port(string ipport)
        {
            if (ipport is null) return Empty;
            return StateObject.IsIpPort(ipport, out Ipv4Port ipnum) ? ipnum : throw new ArgumentException("参数无法被转换，数据无效！", nameof(ipport));
        }

        /// <summary>
        ///  定义从 <see cref="UserKey"/> 对象到 <see cref="Ipv4Port"/> 对象的隐式转换。
        /// </summary>
        /// <param name="ipport">要转换的对象。</param>
        /// <returns>转换的 <see cref="Ipv4Port"/> 对象。</returns>
        public static implicit operator Ipv4Port(UserKey ipport)
        {
            if (!ipport.IsIpv4Port) throw new ArgumentException("参数无法被转换，数据无效！", nameof(ipport));
            return new(ipport.Bytes);
        }

        /// <summary>
        ///  定义从 <see cref="Ipv4Port"/> 对象到 <see cref="string"/> 对象的隐式转换。
        /// </summary>
        /// <param name="ipport">要转换的对象。</param>
        /// <returns>转换的 <see cref="string"/> 对象。</returns>
        public static implicit operator string(Ipv4Port ipport) => ipport.ToString();

        private string GetDebuggerDisplay() => ToString();
    }
}
