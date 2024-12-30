using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.NetFrame.Internal;
using Tool.Sockets.TcpHelper;
using Tool.Sockets.UdpHelper;
using Tool.Utils;
using Tool.Utils.TaskHelper;

namespace Tool.Sockets.P2PHelpr
{
    /// <summary>
    /// 开放用于P2P模块化的支持类
    /// 可以获取免费开放的打洞公网服务器支持
    /// </summary>
    public sealed class P2pServerAsync : IDisposable
    {
        /// <summary>
        /// TCP验证头
        /// </summary>
        public static Memory<byte> TcpTop { get; } = new byte[] { 111, 121, 212, 222 };

        /// <summary>
        /// UDP验证头
        /// </summary>
        public static Memory<byte> UdpTop { get; } = new byte[] { 128, 168, 218, 248 };

        /// <summary>
        /// 设置连接服务器超时时间
        /// </summary>
        public int Millisecond { get; init; } = 10000;

        private const int port = 11111;

        private bool success;
        private bool okwait;
        private INetworkConnect network;
        private TaskCompletionSource<bool> taskWith;

        /// <summary>
        /// 用于本地绑定的IP:Port
        /// </summary>
        public Ipv4Port LocalEP { get; private set; }

        /// <summary>
        /// 用于P2P绑定的IP:Port
        /// </summary>
        public Ipv4Port RemoteEP { get; private set; }

        private static async ValueTask<IPAddress> GetHost() => await Utility.GetIPAddressAsync("p2p.nixue.top", AddressFamily.InterNetwork) ?? throw new Exception("无法获取云端服务器信息，请确认网络是否正常！");

        private static async ValueTask<P2pServerAsync> GetFree(bool isTcp) => await GetFree(new IPEndPoint(await GetHost(), port), isTcp);

        /// <summary>
        /// 验证一个数据流，确定它是否是符合协议的P2P（发起）协议
        /// </summary>
        /// <param name="span"></param>
        /// <param name="localEP"></param>
        /// <returns></returns>
        public static bool IsP2pAuth(Span<byte> span, out Ipv4Port localEP)
        {
            if (span.Length == 10 && (Utility.SequenceCompare(span[..4], TcpTop.Span) || Utility.SequenceCompare(span[..4], UdpTop.Span)))
            {
                localEP = new(span[4..10].ToArray());
                return true;
            }
            localEP = null;
            return false;
        }

        /// <summary>
        /// 验证一个数据流，确定它是否是符合协议的P2P（等待）协议
        /// </summary>
        /// <param name="span"></param>
        /// <param name="localEP"></param>
        /// <returns></returns>
        public static bool IsP2pWait(Span<byte> span, out Ipv4Port localEP)
        {
            if (span.Length == 10 && (Utility.SequenceCompare(span[6..], TcpTop.Span) || Utility.SequenceCompare(span[6..], UdpTop.Span)))
            {
                localEP = new(span[0..6].ToArray());
                return true;
            }
            localEP = null;
            return false;
        }

        /// <summary>
        /// 获取公共的P2PServer服务器的通信消息 TCP版本
        /// <list type="table">提供方：p2p.nixue.top</list>
        /// </summary>
        /// <returns>成功后的结果</returns>
        public static ValueTask<P2pServerAsync> GetFreeTcp() => GetFree(true);

        /// <summary>
        /// 获取公共的P2PServer服务器的通信消息 UDP版本
        /// <list type="table">提供方：p2p.nixue.top</list>
        /// </summary>
        /// <returns>成功后的结果</returns>
        public static ValueTask<P2pServerAsync> GetFreeUdp() => GetFree(false);

        /// <summary>
        /// 用于获取私有P2PServer服务器的通信消息
        /// </summary>
        /// <param name="endPoint">服务器IP端口</param>
        /// <param name="isTcp">获取的P2P类型</param>
        /// <returns>成功后的结果</returns>
        public static async ValueTask<P2pServerAsync> GetFree(IPEndPoint endPoint, bool isTcp)
        {
            P2pServerAsync p2PServerAsync = new(isTcp);
            try
            {
                await p2PServerAsync.ConnectAsync(endPoint);
                await p2PServerAsync.SendAuthAsync(); //需要增加心跳业务，确保双方均未断开
                await p2PServerAsync.EndAuthAsync();
            }
            catch (Exception)
            {
                p2PServerAsync.Dispose();
                throw;
            }
            return p2PServerAsync;
        }

        /// <summary>
        /// 回收相关资源
        /// </summary>
        public void Dispose()
        {
            _disposed = true;
            if (network is not null)
            {
                network.Dispose();
                network = null;
                GC.SuppressFinalize(this);
            }
        }

        bool _disposed = false;

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                ThrowObjectDisposedException();
            }

            void ThrowObjectDisposedException() => throw new ObjectDisposedException(GetType().FullName);
        }

        private P2pServerAsync(bool isTcp)
        {
            if (isTcp)
            {
                network = new TcpClientAsync(NetBufferSize.Size8K, true) { Millisecond = 0 };
            }
            else
            {
                network = new UdpClientAsync(NetBufferSize.Size8K, true) { Millisecond = 0 };
            }
        }

        private Task ConnectAsync(IPEndPoint endPoint)
        {
            taskWith = new(TaskCreationOptions.RunContinuationsAsynchronously);
            if (network is TcpClientAsync tcpClientAsync)
            {
                tcpClientAsync.SetReceived(async (receive) =>
                {
                    using (receive)
                    {
                        await P2PValidate(receive.Memory, TcpTop);
                    }
                });
                tcpClientAsync.SetCompleted(valueTask);
            }
            if (network is UdpClientAsync udpClientAsync)
            {
                udpClientAsync.SetReceived(async (receive) =>
                {
                    using (receive)
                    {
                        await P2PValidate(receive.Memory, UdpTop);
                    }
                });
                udpClientAsync.SetCompleted(valueTask);
            }

            ValueTask valueTask(UserKey a, EnClient b, DateTime c)
            {
                if (b is EnClient.Fail or EnClient.Close)
                {
                    taskWith.TrySetException(new Exception("与服务器协议不一致或中断连接。"));
                }
                return ValueTask.CompletedTask;
            }

            return network.ConnectAsync(endPoint.Address.ToString(), endPoint.Port);
        }

        private ValueTask P2PValidate(Memory<byte> memory, Memory<byte> isor)
        {
            if (memory.Length == 16 && Utility.SequenceCompare(memory.Span[..4], isor.Span))
            {
                LocalEP = new(memory[4..10].ToArray());
                RemoteEP = new(memory[10..16].ToArray());
                success = true;
                taskWith.TrySetResult(success);
            }
            else if (success && memory.Length == 10 && Utility.SequenceCompare(memory.Span[6..], isor.Span))
            {
                Ipv4Port testip = new(memory[0..6].ToArray());
                if (RemoteEP == testip)
                {
                    okwait = true;
                    taskWith.TrySetResult(okwait);
                }
            }
            return ValueTask.CompletedTask;
        }

        private async Task SendAuthAsync()
        {
            ThrowIfDisposed();
            if (network is TcpClientAsync tcpClientAsync)
            {
                tcpClientAsync.AddKeepAlive(100); //毫秒
                using var sendBytes = tcpClientAsync.CreateSendBytes(10);
                sendBytes.SetMemory(TcpTop);
                sendBytes.SetMemory(network.LocalPoint.Bytes, 4);
                await tcpClientAsync.SendAsync(sendBytes);
            }
            if (network is UdpClientAsync udpClientAsync)
            {
                udpClientAsync.AddKeepAlive(10); //毫秒
                using var sendBytes = udpClientAsync.CreateSendBytes(10);
                sendBytes.SetMemory(UdpTop);
                sendBytes.SetMemory(network.LocalPoint.Bytes, 4);
                await udpClientAsync.SendAsync(sendBytes);
            }
        }

        private bool NotConnected => network is TcpClientAsync && !network.Connected;

        private async Task EndAuthAsync()
        {
            ThrowIfDisposed();
            if (NotConnected) throw new Exception("与服务器协议不一致或中断连接。");
            try
            {
                using CancellationTokenSource cts = new(Millisecond);
                cts.Token.UnsafeRegister((state) =>
                {
                    taskWith.TrySetCanceled();
                }, null);
                await taskWith.Task;
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException("在P2P服务器等待结果期间超时。");
            }
            //if (!SpinWait.SpinUntil(IsSuccess, 10000)) throw new Exception("在P2P服务器等待结果期间超时。");

            //return Task.CompletedTask;
        }

        /// <summary>
        /// 判断是否P2P是否可用
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess() => NotConnected || success;

        /// <summary>
        /// 判断是否通过双方等待验证
        /// </summary>
        /// <returns></returns>
        public bool OkWait(CancellationToken token)
        {
            return token.IsCancellationRequested || okwait;
        }

        /// <summary>
        /// 尝试发起P2P连接，通过服务器验证确保可以更可靠的完成连接
        /// </summary>
        /// <param name="clientAsync">要建立连接的对象</param>
        /// <param name="RemoteEP">连接的对方设备</param>
        /// <param name="timedDelay">尝试等待超时时间（不能小于1000ms）</param>
        /// <returns>结果</returns>
        /// <exception cref="Exception">不可用 或 模式不一致 或 超时！</exception>
        public async Task P2PConnectAsync(TcpClientAsync clientAsync, Ipv4Port RemoteEP, int timedDelay = 5000)
        {
            if (timedDelay < 1000) throw new Exception($"timedDelay < 1000 毫秒");
            TryP2P();
            if (network is TcpClientAsync tcpClientAsync)
            {
                try
                {
                    using var sendBytes = tcpClientAsync.CreateSendBytes(10);
                    sendBytes.SetMemory(RemoteEP.Bytes);
                    sendBytes.SetMemory(TcpTop, 6);

                    await clientAsync.P2PConnectAsync(LocalEP, RemoteEP, async token =>
                    {
                        DateTime Now01 = DateTime.Now;
                        await tcpClientAsync.SendAsync(sendBytes);//.IsNewTask();
                        return await WaitP2pOk(Now01, timedDelay, token);
                    });
                }
                finally
                {
                    Dispose(); //成功后回收当前类
                }
            }
            else
            {
                throw new Exception("P2P模式，建立的通道是UDP模式！");
            }
        }

        /// <summary>
        /// 尝试发起P2P连接，通过服务器验证确保可以更可靠的完成连接
        /// </summary>
        /// <param name="clientAsync">要建立连接的对象</param>
        /// <param name="RemoteEP">连接的对方设备</param>
        /// <param name="timedDelay">尝试等待超时时间（不能小于1000ms）</param>
        /// <returns>结果</returns>
        /// <exception cref="Exception">不可用 或 模式不一致 或 超时！</exception>
        public async Task P2PConnectAsync(UdpClientAsync clientAsync, Ipv4Port RemoteEP, int timedDelay = 5000)
        {
            if (timedDelay < 1000) throw new Exception($"timedDelay < 1000 毫秒");
            TryP2P();
            if (network is UdpClientAsync udpClientAsync)
            {
                try
                {
                    using var sendBytes = udpClientAsync.CreateSendBytes(10);
                    sendBytes.SetMemory(RemoteEP.Bytes);
                    sendBytes.SetMemory(UdpTop, 6);

                    await clientAsync.P2PConnectAsync(LocalEP, RemoteEP, async token =>
                    {
                        DateTime Now01 = DateTime.Now;
                        await udpClientAsync.SendAsync(sendBytes);//.IsNewTask();
                        return await WaitP2pOk(Now01, timedDelay, token);
                    });
                }
                finally
                {
                    Dispose(); //成功后回收当前类
                }
            }
            else
            {
                throw new Exception("P2P模式，建立的通道是TCP模式！");
            }
        }

        private async Task<int> WaitP2pOk(DateTime Now01, int timedDelay, CancellationToken token)
        {
            okwait = false;
            taskWith = new(TaskCreationOptions.RunContinuationsAsynchronously); //重置新任务

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(token);
            cts.Token.UnsafeRegister((state) =>
            {
                taskWith.TrySetCanceled();
            }, null);
            cts.CancelAfter(timedDelay);

            try
            {
                await taskWith.Task;
            }
            catch (TaskCanceledException)
            {
                throw new TimeoutException("等待对方发起P2P尝试，超时！");
            }

            //await Task.Run(() => { if (!SpinWait.SpinUntil(OkWait, timedDelay)) throw new Exception("等待对方发起P2P尝试，超时！"); });
            //if (!SpinWait.SpinUntil(() => OkWait(token), timedDelay) && !okwait) throw new Exception("等待对方发起P2P尝试，超时！");
            DateTime Now02 = DateTime.Now;
            double seconds = (Now02 - Now01).TotalMilliseconds;
            Debug.WriteLine($"P2P同步协议！{Now02:O},{seconds}ms");
            return (int)(timedDelay - seconds);
        }

        private void TryP2P()
        {
            ThrowIfDisposed();
            if (!IsSuccess()) throw new Exception("当前P2P模式，不可用！");
        }
    }
}
