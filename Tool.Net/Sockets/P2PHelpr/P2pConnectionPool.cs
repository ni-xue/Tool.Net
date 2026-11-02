using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.TcpHelper;

namespace Tool.Sockets.P2PHelpr
{
    /// <summary>
    /// TCP 长连接池：按信标服务器地址复用长连接，空闲 5 分钟回收。
    /// <para>连接上的消息统一交给 <see cref="P2pSessionRegistry"/> 按 Key 路由。</para>
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    internal class P2pConnectionPool
    {
        private sealed class ConnectionEntry
        {
            public TcpClientAsync Network;
            public DateTime LastActive;
        }

        private static readonly P2pConnectionPool _instance = new();
        public static P2pConnectionPool Instance => _instance;

        private readonly Dictionary<Ipv4Port, ConnectionEntry> _connections = new();
        private readonly SemaphoreSlim _lock = new(1, 1);
        private static readonly TimeSpan IdleTimeout = TimeSpan.FromMinutes(5);

        private P2pConnectionPool()
        {
            KeepAlive keepok = new(30, RecycleIdleConnectionsAsync);
        }

        /// <summary>按 endPoint 获取或创建共享 TCP 长连接</summary>
        internal async Task<TcpClientAsync> GetOrCreateAsync(Ipv4Port endPoint)
        {
            await _lock.WaitAsync();
            try
            {
                if (_connections.TryGetValue(endPoint, out var entry))
                {
                    entry.LastActive = DateTime.UtcNow;
                    return entry.Network;
                }

                var network = new TcpClientAsync(NetBufferSize.Size8K, true, true) { Millisecond = 0 };
                network.SetCompleted(OnConnectionCompleted);
                network.SetReceived(OnServerMessage);
                network.OnInterceptor(EnClient.SendMsg, false);
                network.OnInterceptor(EnClient.Receive, false);
                network.OnInterceptor(EnClient.HeartBeat, false);
                network.AddKeepAlive(30 * 1000);

                try
                {
                    await network.ConnectAsync(endPoint);
                }
                catch
                {
                    network.Dispose();
                    throw;
                }

                _connections[endPoint] = new ConnectionEntry { Network = network, LastActive = DateTime.UtcNow };
                return network;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async ValueTask OnConnectionCompleted(UserKey user, EnClient evt, DateTime time)
        {
            await _lock.WaitAsync();
            try
            {
                switch (evt)
                {
                    case EnClient.Connect:
                        { if (_connections.TryGetValue(user, out var entry)) entry.Network.Client.NoDelay = true; }
                        break;
                    case EnClient.Fail:
                    case EnClient.Close:
                        { if (_connections.Remove(user, out var entry)) entry.Network.Dispose(); }
                        break;
                }
            }
            finally 
            { 
                _lock.Release();
            }
        }

        private ValueTask OnServerMessage(ReceiveBytes<System.Net.Sockets.Socket> receive)
        {
            P2pSessionRegistry.Instance.OnTcpMessage(receive.Span);
            return ValueTask.CompletedTask;
        }

        private async Task RecycleIdleConnectionsAsync()
        {
            await _lock.WaitAsync();
            try
            {
                var now = DateTime.UtcNow;
                List<Ipv4Port> toRemove = null;
                foreach (var kv in _connections)
                {
                    if (now - kv.Value.LastActive > IdleTimeout) (toRemove ??= new()).Add(kv.Key);
                }
                if (toRemove != null)
                {
                    foreach (var ep in toRemove)
                    {
                        if (_connections.Remove(ep, out var entry)) entry.Network.Dispose();
                    }
                }
            }
            finally { _lock.Release(); }
        }
    }
}