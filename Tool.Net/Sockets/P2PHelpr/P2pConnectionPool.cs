using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.TcpHelper;

namespace Tool.Sockets.P2PHelpr
{
    internal class P2pConnectionPool
    {
        private static readonly P2pConnectionPool _instance = new();
        internal static P2pConnectionPool Instance => _instance;

        // 只存 TCP 长连接
        private readonly Dictionary<IPEndPoint, TcpClientAsync> _connections = new();
        private readonly Dictionary<IPEndPoint, DateTime> _lastActive = new();
        private readonly SemaphoreSlim _lock = new(1, 1);

        private static readonly TimeSpan IdleTimeout = TimeSpan.FromMinutes(5);

        private P2pConnectionPool()
        {
            KeepAlive keepok = new(30, RecycleIdleConnectionsAsync);
        }

        /// <summary>
        /// 按 endPoint 获取或创建共享的 TCP 长连接。已连接则直接返回。
        /// </summary>
        internal async Task<TcpClientAsync> GetOrCreateAsync(IPEndPoint endPoint)
        {
            await _lock.WaitAsync();
            try
            {
                if (_connections.TryGetValue(endPoint, out var existing))
                {
                    _lastActive[endPoint] = DateTime.UtcNow;
                    return existing;
                }

                var network = new TcpClientAsync(NetBufferSize.Size8K, true) { Millisecond = 0 };
                try
                {
                    await network.ConnectAsync(endPoint.Address.ToString(), endPoint.Port);
                }
                catch
                {
                    network.Dispose();
                    throw;
                }

                _connections[endPoint] = network;
                _lastActive[endPoint] = DateTime.UtcNow;
                return network;
            }
            finally
            {
                _lock.Release();
            }
        }

        internal async Task TouchAsync(IPEndPoint endPoint)
        {
            await _lock.WaitAsync();
            try
            {
                if (_connections.ContainsKey(endPoint))
                    _lastActive[endPoint] = DateTime.UtcNow;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task RecycleIdleConnectionsAsync()
        {
            await _lock.WaitAsync();
            try
            {
                var now = DateTime.UtcNow;
                List<IPEndPoint> toRemove = null;
                foreach (var kv in _connections)
                {
                    if (_lastActive.TryGetValue(kv.Key, out var last) && now - last > IdleTimeout)
                    {
                        (toRemove ??= new()).Add(kv.Key);
                    }
                }

                if (toRemove != null)
                {
                    foreach (var ep in toRemove)
                    {
                        if (_connections.Remove(ep, out var conn))
                        {
                            _lastActive.Remove(ep);
                            conn.Dispose();
                        }
                    }
                }
            }
            finally
            {
                _lock.Release();
            }
        }
    }
}