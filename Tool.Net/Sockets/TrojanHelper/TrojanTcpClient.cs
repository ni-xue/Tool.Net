using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;

namespace Tool.Sockets.TrojanHelper
{
    /// <summary>
    /// Trojan 的Tcp通道
    /// </summary>
    public class TrojanTcpClient : IDisposable
    {
        private readonly TrojanConnect _trojanConnect;
        private Socket _tcpClient;
        private NetworkStream _network;
        private SslStream _sslStream;
        private bool _disposed = false;

        /// <summary>
        /// Tls认证属性设置
        /// </summary>
        public SslClientAuthenticationOptions SslOptions { get; set; }

        /// <summary>
        /// 测试产物
        /// </summary>
        /// <param name="trojanConnect"></param>
        public TrojanTcpClient(TrojanConnect trojanConnect)
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
        }

        /// <summary>
        /// 通过 Trojan 代理连接到目标服务器
        /// </summary>
        public async Task<SslStream> ConnectToTargetAsync(string targetHost, int targetPort, CancellationToken token)
        {
            if (_sslStream == null)
                throw new InvalidOperationException("未连接到 Trojan 服务器");

            // 构造 Trojan 协议的第一个数据包
            using var requestPacket = TrojanExtension.CreateTrojanRequestPacket(true, targetHost, targetPort, _trojanConnect.Password);
            await _sslStream.WriteAsync(requestPacket.Memory, token);
            await _sslStream.FlushAsync(token);

            return _sslStream;
        }

        /// <summary>
        /// 发送数据
        /// </summary>
        public async Task SendAsync(byte[] data, CancellationToken token)
        {
            if (_sslStream == null)
                throw new InvalidOperationException("未连接到目标");

            await _sslStream.WriteAsync(data, token);
        }

        /// <summary>
        /// 接收数据
        /// </summary>
        public async Task<int> ReceiveAsync(byte[] buffer, CancellationToken token)
        {
            if (_sslStream == null)
                throw new InvalidOperationException("未连接到目标");

            return await _sslStream.ReadAsync(buffer, token);
        }

        /// <summary>
        /// 回收资源
        /// </summary>
        public void Dispose()
        {
            if (!_disposed)
            {
                _sslStream?.Dispose();
                _network?.Dispose();
                _tcpClient?.Dispose();
                _disposed = true;
                GC.SuppressFinalize(this);
            }
        }
    }
}
