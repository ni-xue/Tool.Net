using System;
using System.Globalization;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.TrojanHelper
{
    /// <summary>
    /// Trojan 帮助类
    /// </summary>
    public static class TrojanExtension
    {
        /// <summary>
        /// 创建 Trojan 协议请求包
        /// </summary>
        internal static BytesCore CreateTrojanRequestPacketTcp(string targetHost, int targetPort, string _password)
        {
            using var hostBytes = GetHostBytes(targetHost, out byte atyp);
            BytesCore bytesCore = new(56 + 2 + 1 + 1 + hostBytes.Length + 2 + 2);

            int index = 0;
            // 密码的 SHA224 哈希
            CalculateSHA224(bytesCore, _password);
            index += 56;

            // CRLF (Trojan 协议要求)
            bytesCore.Span[index++] = 0x0D;
            bytesCore.Span[index++] = 0x0A;

            // 命令类型 (0x01 表示 CONNECT)
            bytesCore.Span[index++] = 0x01;

            // 目标地址类型和地址
            bytesCore.Span[index++] = atyp;
            hostBytes.Span.CopyTo(bytesCore.Span[index..]);
            index += hostBytes.Length;

            // 目标端口
            bytesCore.Span[index++] = (byte)(targetPort >> 8);
            bytesCore.Span[index++] = (byte)targetPort;

            // CRLF 结束
            bytesCore.Span[index++] = 0x0D;
            bytesCore.Span[index++] = 0x0A;

            return bytesCore;
        }

        /// <summary>
        /// 创建 Trojan 协议请求包
        /// </summary>
        internal static BytesCore CreateTrojanUdpAuth(string _password)
        {
            BytesCore bytesCore = new(56 + 2 + 1);

            int index = 0;
            // 密码的 SHA224 哈希
            CalculateSHA224(bytesCore, _password);
            index += 56;

            // CRLF (Trojan 协议要求)
            bytesCore.Span[index++] = 0x0D;
            bytesCore.Span[index++] = 0x0A;

            // 命令类型 (0x03 表示 UDP)
            bytesCore.Span[index++] = 0x03;

            return bytesCore;
        }

        /// <summary>
        /// 创建 Trojan 协议请求包
        /// </summary>
        internal static BytesCore CreateTrojanRequestPacketUdp(byte[] data, string targetHost, int targetPort)
        {
            if (targetPort < ushort.MinValue || targetPort > ushort.MaxValue) throw new ArgumentException("端口号无效。");

            int dataLength = data.Length;
            if (dataLength > ushort.MaxValue)
                throw new ArgumentException($"数据报长度过长，不能超过 {ushort.MaxValue} 字节。");

            using var hostBytes = GetHostBytes(targetHost, out byte atyp);

            // 1(ATYP) + addr + 2(PORT) + 2(LENGTH) + 2(CRLF) + payload
            BytesCore bytesCore = new(1 + hostBytes.Length + 2 + 2 + 2 + dataLength);

            int index = 0;

            bytesCore.Span[index++] = atyp;
            hostBytes.Span.CopyTo(bytesCore.Span[index..]);
            index += hostBytes.Length;

            // 目标端口
            bytesCore.Span[index++] = (byte)(targetPort >> 8);
            bytesCore.Span[index++] = (byte)targetPort;

            //数据包大小
            bytesCore.Span[index++] = (byte)(dataLength >> 8);
            bytesCore.Span[index++] = (byte)dataLength;

            // CRLF 结束
            bytesCore.Span[index++] = 0x0D;
            bytesCore.Span[index++] = 0x0A;

            // 写入携带数据报
            data.CopyTo(bytesCore.Span[index..]);
            index += dataLength;

            return bytesCore;
        }

        private static BytesCore GetHostBytes(string targetHost, out byte atyp)
        {
            if (IPAddress.TryParse(targetHost, out var ipAddress))
            {
                if (ipAddress.AddressFamily is AddressFamily.InterNetwork)
                {
                    atyp = 0x01;
                    BytesCore bytesCore = new(4);
                    ipAddress.TryWriteBytes(bytesCore.Span, out _);
                    return bytesCore;
                }
                else
                {
                    atyp = 0x04;
                    BytesCore bytesCore = new(16);
                    ipAddress.TryWriteBytes(bytesCore.Span, out _);
                    return bytesCore;
                }
            }
            else
            {
                string normalizedDomain = targetHost.ToLowerInvariant();
                if (!IsAscii(normalizedDomain))
                {
                    normalizedDomain = new IdnMapping().GetAscii(normalizedDomain);
                }
                atyp = 0x03;
                var domainLength = Encoding.ASCII.GetByteCount(normalizedDomain);
                if (domainLength > 255)
                {
                    throw new ArgumentException($"Domain name too long: {normalizedDomain} ({domainLength} bytes)");
                }
                BytesCore bytesCore = new(domainLength + 1);
                bytesCore.Span[0] = (byte)domainLength;
                Encoding.ASCII.GetBytes(normalizedDomain, bytesCore.Span[1..]);
                return bytesCore;
            }
        }

        private static bool IsAscii(string text)
        {
            foreach (char c in text)
            {
                if (c > 127)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 计算 SHA224 哈希（输出为 56 字节 ASCII 十六进制字符串）
        /// </summary>
        private static void CalculateSHA224(BytesCore bytesCore, string input)
        {
            int count = Encoding.UTF8.GetByteCount(input);
            using BytesCore passwordBytesCore = new(count);
            int bytesSize = Encoding.UTF8.GetBytes(input, passwordBytesCore.Span);
            int bytesWritten = Utils.Encryption.SHA224.HashData(passwordBytesCore.Span[..bytesSize], bytesCore.Span);
            string hexHash = Convert.ToHexString(bytesCore.Span[..bytesWritten]).ToLowerInvariant();
            Encoding.ASCII.GetBytes(hexHash, bytesCore.Span);
        }

        //// 零分配 Hex 编码
        //private static void EncodeToHex(ReadOnlySpan<byte> source, Span<byte> destination, bool lowerCase = true)
        //{
        //    if (destination.Length < source.Length * 2)
        //        throw new ArgumentException("Destination too small");
        //    ReadOnlySpan<byte> hexAlphabet = lowerCase ? "0123456789abcdef"u8 : "0123456789ABCDEF"u8;
        //    for (int i = 0; i < source.Length; i++)
        //    {
        //        byte b = source[i];
        //        destination[i * 2] = hexAlphabet[b >> 4];
        //        destination[i * 2 + 1] = hexAlphabet[b & 0x0F];
        //    }
        //}


    }
}
