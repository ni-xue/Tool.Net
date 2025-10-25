using System;
using System.Buffers;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Sockets.TrojanHelper
{
#if NET7_0_OR_GREATER
    /// <summary>
    /// 测试产物
    /// </summary>
    public class TrojanUdpClient : IDisposable
    {
        private readonly string _server;
        private readonly int _port;
        private readonly string _password;
        private TcpClient _tcpClient;
        private SslStream _sslStream;
        private PipeReader _reader;
        private bool _disposed = false;
        private readonly byte[] _passwordHash;

        /// <summary>
        /// 测试产物
        /// </summary>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="password"></param>
        public TrojanUdpClient(string server, int port, string password)
        {
            _server = server;
            _port = port;
            _password = password;
            _passwordHash = CalculateSHA224(password);
        }

        /// <summary>
        /// 连接到 Trojan 服务器
        /// </summary>
        public async Task ConnectAsync()
        {
            _tcpClient = new TcpClient();
            await _tcpClient.ConnectAsync(_server, _port);

            _sslStream = new SslStream(_tcpClient.GetStream(), false);
            await _sslStream.AuthenticateAsClientAsync(_server);

            // 使用内置方法创建 PipeReader
            _reader = PipeReader.Create(_sslStream, new StreamPipeReaderOptions(
                bufferSize: 4096,
                minimumReadSize: 512,
                leaveOpen: false // SslStream 会随着 PipeReader 一起关闭
            ));
        }

        /// <summary>
        /// 通过 Trojan 代理连接到目标服务器
        /// </summary>
        public async Task<SslStream> ConnectToTargetAsync(string targetHost, int targetPort, CancellationToken token)
        {
            if (_sslStream == null)
                throw new InvalidOperationException("未连接到 Trojan 服务器");

            // 构造 Trojan 协议的第一个数据包
            using var requestPacket = TrojanExtension.CreateTrojanRequestPacket(false, targetHost, targetPort, _trojanConnect.Password);
            await _sslStream.WriteAsync(requestPacket.Memory, token);
            await _sslStream.FlushAsync(token);

            return _sslStream;
        }

        /// <summary>
        /// 发送 UDP 数据
        /// </summary>
        public async Task SendUdpAsync(byte[] data, string targetHost, int targetPort)
        {
            if (_sslStream == null)
                throw new InvalidOperationException("未连接到 Trojan 服务器");

            byte[] packet = CreateUdpPacket(data, targetHost, targetPort);
            await _sslStream.WriteAsync(packet, 0, packet.Length);
            await _sslStream.FlushAsync();
        }

        /// <summary>
        /// 接收 UDP 响应（使用 PipeReader 处理粘包）
        /// </summary>
        public async Task<(byte[] data, string remoteHost, int remotePort)> ReceiveUdpAsync()
        {
            if (_reader == null)
                throw new InvalidOperationException("未连接到 Trojan 服务器");

            while (true)
            {
                ReadResult result = await _reader.ReadAsync();
                ReadOnlySequence<byte> buffer = result.Buffer;

                // 尝试从缓冲区解析完整的 UDP 包
                var parseResult = TryParseUdpPacket(ref buffer);

                if (parseResult.success)
                {
                    _reader.AdvanceTo(buffer.Start);
                    return (parseResult.data, parseResult.remoteHost, parseResult.remotePort);
                }

                // 如果没有完整的数据包，告诉 PipeReader 我们检查了所有数据
                _reader.AdvanceTo(buffer.Start, buffer.End);

                if (result.IsCompleted)
                    throw new IOException("连接已关闭");
            }
        }

        /// <summary>
        /// 尝试解析 UDP 数据包（处理粘包）
        /// </summary>
        private (bool success, byte[] data, string remoteHost, int remotePort) TryParseUdpPacket(ref ReadOnlySequence<byte> buffer)
        {
            if (buffer.Length < 59) // 最小头部长度
                return (false, null, null, 0);

            var reader = new SequenceReader<byte>(buffer);

            // 读取并验证密码哈希 (56字节)
            if (!reader.TryReadExact(_passwordHash.Length, out ReadOnlySequence<byte> hashSegment) ||
                !hashSegment.ToArray().SequenceEqual(_passwordHash))
            {
                return (false, null, null, 0);
            }

            // 验证 CRLF
            if (!reader.TryRead(out byte cr1) || cr1 != 0x0D ||
                !reader.TryRead(out byte cr2) || cr2 != 0x0A)
            {
                return (false, null, null, 0);
            }

            // 验证命令类型
            if (!reader.TryRead(out byte command) || command != 0x03)
            {
                return (false, null, null, 0);
            }

            // 解析地址信息
            var (addressSuccess, remoteHost, remotePort, addressLength) = TryParseAddressInfo(ref reader);
            if (!addressSuccess)
                return (false, null, null, 0);

            // 验证结束 CRLF
            if (!reader.TryRead(out byte endCr1) || endCr1 != 0x0D ||
                !reader.TryRead(out byte endCr2) || endCr2 != 0x0A)
            {
                return (false, null, null, 0);
            }

            // 剩余的数据就是 UDP 负载
            ReadOnlySequence<byte> dataSequence = buffer.Slice(reader.Position);
            byte[] data = dataSequence.ToArray();

            // 更新缓冲区位置（跳过整个数据包）
            buffer = buffer.Slice(buffer.GetPosition(reader.Consumed + dataSequence.Length));

            return (true, data, remoteHost, remotePort);
        }

        /// <summary>
        /// 解析地址信息
        /// </summary>
        private (bool success, string host, int port, int length) TryParseAddressInfo(ref SequenceReader<byte> reader)
        {
            long startPosition = reader.Consumed;

            if (!reader.TryRead(out byte addressType))
                return (false, null, 0, 0);

            string host;
            int addressLength;

            switch (addressType)
            {
                case 0x01: // IPv4
                    if (!reader.TryReadExact(4, out ReadOnlySequence<byte> ipv4Bytes))
                        return (false, null, 0, 0);
                    host = new IPAddress(ipv4Bytes.ToArray()).ToString();
                    addressLength = 1 + 4;
                    break;

                case 0x04: // IPv6
                    if (!reader.TryReadExact(16, out ReadOnlySequence<byte> ipv6Bytes))
                        return (false, null, 0, 0);
                    host = new IPAddress(ipv6Bytes.ToArray()).ToString();
                    addressLength = 1 + 16;
                    break;

                case 0x03: // 域名
                    if (!reader.TryRead(out byte domainLength))
                        return (false, null, 0, 0);
                    if (!reader.TryReadExact(domainLength, out ReadOnlySequence<byte> domainBytes))
                        return (false, null, 0, 0);
                    host = Encoding.ASCII.GetString(domainBytes.ToArray());
                    addressLength = 1 + 1 + domainLength;
                    break;

                default:
                    return (false, null, 0, 0);
            }

            // 读取端口（大端序）
            if (!reader.TryReadBigEndian(out short port))
                return (false, null, 0, 0);

            int totalLength = (int)(reader.Consumed - startPosition);
            return (true, host, port, totalLength);
        }

        // 原有的辅助方法保持不变
        private byte[] CreateUdpPacket(byte[] data, string targetHost, int targetPort)
        {
            using var ms = new MemoryStream();
            ms.Write(_passwordHash, 0, _passwordHash.Length);
            ms.WriteByte(0x0D); ms.WriteByte(0x0A);
            ms.WriteByte(0x03);
            WriteAddressInfo(ms, targetHost, targetPort);
            ms.WriteByte(0x0D); ms.WriteByte(0x0A);
            ms.Write(data, 0, data.Length);
            return ms.ToArray();
        }

        private void WriteAddressInfo(MemoryStream ms, string targetHost, int targetPort)
        {
            if (IPAddress.TryParse(targetHost, out var ipAddress))
            {
                if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
                {
                    ms.WriteByte(0x01);
                    ms.Write(ipAddress.GetAddressBytes(), 0, 4);
                }
                else
                {
                    ms.WriteByte(0x04);
                    ms.Write(ipAddress.GetAddressBytes(), 0, 16);
                }
            }
            else
            {
                ms.WriteByte(0x03);
                byte[] domainBytes = Encoding.ASCII.GetBytes(targetHost);
                ms.WriteByte((byte)domainBytes.Length);
                ms.Write(domainBytes, 0, domainBytes.Length);
            }

            byte[] portBytes = BitConverter.GetBytes((ushort)targetPort);
            if (BitConverter.IsLittleEndian) Array.Reverse(portBytes);
            ms.Write(portBytes, 0, 2);
        }

        private byte[] CalculateSHA224(string input)
        {
            using var sha256 = System.Security.Cryptography.SHA256.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(input);
            byte[] hash = sha256.ComputeHash(bytes);
            byte[] sha224Hash = new byte[28];
            Array.Copy(hash, sha224Hash, 28);
            return sha224Hash;
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _reader?.Complete();
                _sslStream?.Dispose();
                _tcpClient?.Dispose();
                _disposed = true;
            }
        }
    }
#endif
}
