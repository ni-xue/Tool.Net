using System;
using System.Drawing;
using System.IO.Pipelines;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.TrojanHelper
{
    /// <summary>
    /// 测试产物
    /// </summary>
    public class TrojanUdpClient : IDisposable
    {
        private readonly TrojanConnect _trojanConnect;
        private Socket _tcpClient;
        private NetworkStream _network;
        private SslStream _sslStream;
        private PipeReader _reader;
        private bool _disposed = false;

        /// <summary>
        /// Tls认证属性设置
        /// </summary>
        public SslClientAuthenticationOptions SslOptions { get; set; }

        /// <summary>
        /// 测试产物
        /// </summary>
        /// <param name="trojanConnect"></param>
        public TrojanUdpClient(TrojanConnect trojanConnect)
        {
            _trojanConnect = trojanConnect;
        }

        /// <summary>
        /// 连接到 Trojan 服务器
        /// </summary>
        public async Task ConnectAsync(CancellationToken token)
        {
            if (string.IsNullOrWhiteSpace(_trojanConnect.Host)) throw new Exception("Host 不能为空。");
            if (_trojanConnect.Port <= 0 || _trojanConnect.Port > 65535) throw new Exception("Port 0 - 65535。");
            if (string.IsNullOrWhiteSpace(_trojanConnect.Password)) throw new Exception("Password 不能为空。");
            if (_trojanConnect.Address is null)
            {
                var addresses = await Utils.Utility.GetIPAddressListAsync(_trojanConnect.Host);
                if (addresses.Length is 0)
                {
                    throw new Exception($"无法解析到：{_trojanConnect.Host}");
                }
                _trojanConnect.Address = addresses[0];
            }

            var endPoint = new IPEndPoint(_trojanConnect.Address, _trojanConnect.Port);
            _tcpClient = StateObject.CreateSocket(true, NetBufferSize.Default);
            await _tcpClient.ConnectAsync(endPoint, token);

            _network = new NetworkStream(_tcpClient, true);
            _sslStream = new SslStream(_network, false);
            SslOptions ??= new SslClientAuthenticationOptions();
            SslOptions.TargetHost ??= _trojanConnect.Host;
            await _sslStream.AuthenticateAsClientAsync(SslOptions, token);

            // 使用内置方法创建 PipeReader
            _reader = PipeReader.Create(_sslStream, new StreamPipeReaderOptions(
                bufferSize: 4096,
                minimumReadSize: 512,
                leaveOpen: false // SslStream 会随着 PipeReader 一起关闭
            ));
        }

        /// <summary>
        /// 通过 Trojan 代理发起密码验证
        /// </summary>
        public async ValueTask<SslStream> AuthAsync(CancellationToken token)
        {
            if (_sslStream == null)
                throw new InvalidOperationException("未连接到 Trojan 服务器");

            // 构造 Trojan 协议的第一个数据包
            using var requestPacket = TrojanExtension.CreateTrojanUdpAuth(_trojanConnect.Password);
            string hexHash = Convert.ToHexString(requestPacket.Span);
            Console.WriteLine($"认证数据包：{hexHash}");
            await _sslStream.WriteAsync(requestPacket.Memory, token);
            //await _sslStream.FlushAsync(token);

            return _sslStream;
        }

        /// <summary>
        /// 发送 UDP 数据
        /// </summary>
        public async ValueTask SendUdpAsync(byte[] data, string targetHost, int targetPort, CancellationToken token)
        {
            if (_sslStream == null)
                throw new InvalidOperationException("未连接到 Trojan 服务器");

            using var requestPacket = TrojanExtension.CreateTrojanRequestPacketUdp(data, targetHost, targetPort);
            string hexHash = Convert.ToHexString(requestPacket.Span);
            Console.WriteLine($"发送数据报：{hexHash}");
            await _sslStream.WriteAsync(requestPacket.Memory, token);
            await _sslStream.FlushAsync(token);
        }

        /// <summary>
        /// 发送 UDP 数据
        /// </summary>
        public async ValueTask ReceiveUdpAsync(CancellationToken token)
        {
            if (_sslStream == null)
                throw new InvalidOperationException("未连接到 Trojan 服务器");

            using BytesCore bytesCore = new(65535);
            while (true)
            {
                var result = await _reader.ReadAsync(token);
                if (result.IsCompleted)
                {
                    break;
                   
                }
                else
                {
                    if (result.Buffer.Length > 0)
                    {
                        string hexHash = Convert.ToHexString(result.Buffer.FirstSpan);
                        Console.WriteLine($"发送数据报：{hexHash}");
                    }
                }
            }
            int count = await _sslStream.ReadAsync(bytesCore.Memory, token);
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
                GC.SuppressFinalize(this);
            }
        }
    }
}
