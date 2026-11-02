using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.TcpHelper;
using Tool.Sockets.UdpHelper;
using Tool.Utils;

namespace Tool.Sockets.P2PHelpr
{
    /// <summary>
    /// P2P 打洞成功后的结果。
    /// <para>包含打通用的 Socket，以及本轮实际使用的本地地址和对端地址。</para>
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class P2pConnectResult
    {
        /// <summary>
        /// 打洞成功的 Socket
        /// </summary>
        public Socket Socket { get; }

        /// <summary>
        /// 本地绑定的地址。
        /// </summary>
        public EndPoint LocalEP { get; }

        /// <summary>
        /// 对端的地址。
        /// </summary>
        public EndPoint EndPoint { get; }

        /// <summary>
        /// 构造 P2P 连接成功返回对象
        /// </summary>
        /// <param name="socket">Socket 连接</param>
        /// <param name="localEP">本地地址</param>
        /// <param name="endPoint">对端地址</param>
        public P2pConnectResult(Socket socket, EndPoint localEP, EndPoint endPoint)
        {
            Socket = socket;
            LocalEP = localEP;
            EndPoint = endPoint;
        }
    }

    /// <summary>
    /// P2P 打洞每一轮的地址与超时信息。
    /// <para>由调用方在 <c>TryP2PConnect</c> 的重试回调中返回，每轮重新获取，避免重试时复用旧的本地端口或对端地址。</para>
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class P2pRetryInfo
    {
        /// <summary>
        /// 本轮剩余的等待超时（毫秒）。
        /// <para>通常为“总超时减去已消耗时间”，用于本轮 <c>Bind</c> / <c>Connect</c> / 收发的超时控制。</para>
        /// </summary>
        public int Timeout { get; }

        /// <summary>
        /// 本轮本地绑定的地址。
        /// <para>TCP 打洞时通常复用同一个本地端口；UDP 打洞时每轮可能是新的端口（上一轮 UDP 已关闭，内核重新分配）。</para>
        /// </summary>
        public EndPoint LocalEP { get; }

        /// <summary>
        /// 本轮对端的地址。
        /// <para>由信标服务器下发，重试时可能变化（对端重新发 UDP 就绪后，映射端口可能更新）。</para>
        /// </summary>
        public EndPoint EndPoint { get; }

        /// <summary>
        /// 构造一轮打洞的地址与超时信息。
        /// </summary>
        /// <param name="timeout">本轮剩余超时（毫秒）</param>
        /// <param name="localEP">本轮本地绑定地址</param>
        /// <param name="endPoint">本轮对端地址</param>
        public P2pRetryInfo(int timeout, Ipv4Port localEP, Ipv4Port endPoint)
        {
            if (timeout < 1000) throw new Exception($"timeout < 1000 毫秒");
            if (localEP == Ipv4Port.Empty) throw new Exception($"localEP 不能为 Ipv4Port.Empty");
            if (endPoint == Ipv4Port.Empty) throw new Exception($"endPoint 不能为 Ipv4Port.Empty");
            Timeout = timeout;
            LocalEP = new IPEndPoint(localEP.Ip, localEP.Port);
            EndPoint = new IPEndPoint(endPoint.Ip, endPoint.Port);
        }

        /// <summary>
        /// 构造一轮打洞的地址与超时信息。
        /// </summary>
        /// <param name="timeout">本轮剩余超时（毫秒）</param>
        /// <param name="localEP">本轮本地绑定地址</param>
        /// <param name="endPoint">本轮对端地址</param>
        public P2pRetryInfo(int timeout, EndPoint localEP, EndPoint endPoint)
        {
            if (timeout < 1000) throw new Exception($"timeout < 1000 毫秒");
            if (localEP is null) throw new Exception($"localEP 不能为null");
            if (endPoint is null) throw new Exception($"endPoint 不能为null");
            Timeout = timeout;
            LocalEP = localEP;
            EndPoint = endPoint;
        }
    }

    /// <summary>
    /// P2p发起打洞前 STUN（信标）规范
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public sealed class P2pHoleContext : IDisposable
    {
        /// <summary>
        /// 累计尝试次数
        /// </summary>
        public int HoleCount { get; }

        /// <summary>
        /// 累计已运行的时间
        /// </summary>
        public int InitElapsedMs { get; }

        /// <summary>
        /// 空的 STUN 服务器 <see cref="Socket"/> 允许关闭和释放，也可以交由系统管理
        /// </summary>
        public Socket StunSocket { get; }

        /// <summary>
        /// 打洞是否取消的令牌
        /// </summary>
        public CancellationToken Token { get; }

        /// <summary>
        /// 仅允许内部调用
        /// </summary>
        internal P2pHoleContext(bool isTcp, int round, int initElapsedMs, CancellationToken token)
        {
            StunSocket = P2pClientAsync.GetP2pSocket(isTcp, NetBufferSize.Size8K);
            HoleCount = round;
            InitElapsedMs = initElapsedMs;
            Token = token;
        }

        /// <summary>
        /// 释放 <see cref="Socket"/> 对象
        /// </summary>
        public void Dispose() => StunSocket?.Dispose();
    }

    /// <summary>
    /// 封装一个可用的P2P打洞实现
    /// 帮助实现P2P打洞
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public static class P2pClientAsync
    {
        /// <summary>
        /// P2P 模式专用
        /// </summary>
        public const string DefaultRemoteEP = "0.0.0.0:0";

        /// <summary>
        /// 描述P2P验证协议
        /// </summary>
        private static readonly ArraySegment<byte> Bytes = new byte[] { 255, 225, 195, 155, 125, 95, 55, 25, 0 };

        /// <summary>
        /// 描述P2P完成协议
        /// </summary>
        private static readonly ArraySegment<byte> Ready = new byte[] { 10, 20, 40, 80, 160, 255 };

        /// <summary>
        /// 获取 一个空的用于 P2p<see cref="Socket"/> 对象
        /// </summary>
        /// <param name="isTcp">Is TCP</param>
        /// <param name="bufferSize">创建的流数据包大小</param>
        /// <returns>P2p<see cref="Socket"/> 对象</returns>
        public static Socket GetP2pSocket(bool isTcp, NetBufferSize bufferSize)
        {
            Socket socket = StateObject.CreateSocket(isTcp, bufferSize);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true); // 两个 socket 各自都要做，不能只做其中一个
            //if (OperatingSystem.IsLinux()) socket.SetSocketOption(SocketOptionLevel.Socket, (SocketOptionName)15, 1); // SO_REUSEPORT
            return socket;
        }

        /// <summary>
        /// 尝试使用TCP模式进行P2P
        /// </summary>
        /// <param name="tcpClient">调起方</param>
        /// <param name="localEP">尝试绑定的IP端口</param>
        /// <param name="RemoteEP">尝试连接的IP端口</param>
        /// <param name="TimedDelay">P2P超时等待时长（默认5秒）</param>
        /// <param name="_logger">日志对象</param>
        /// <returns>任务</returns>
        public static async Task P2PConnectAsync(this TcpClientAsync tcpClient, Ipv4Port localEP, Ipv4Port RemoteEP, int TimedDelay = 5000, ILogger _logger = null)
        {
            if (TimedDelay < 1000) throw new Exception($"TimedDelay < 1000 毫秒");
            await tcpClient.P2PConnectAsync(p2PHole => ValueTask.FromResult(new P2pRetryInfo(TimedDelay - p2PHole.InitElapsedMs, localEP, RemoteEP)), _logger);
        }

        /// <summary>
        /// 尝试使用UDP模式进行P2P
        /// </summary>
        /// <param name="udpClient">调起方</param>
        /// <param name="localEP">尝试绑定的IP端口</param>
        /// <param name="RemoteEP">尝试连接的IP端口</param>
        /// <param name="TimedDelay">P2P超时等待时长（默认5秒）</param>
        /// <param name="_logger">日志对象</param>
        /// <returns>任务</returns>
        public static async Task P2PConnectAsync(this UdpClientAsync udpClient, Ipv4Port localEP, Ipv4Port RemoteEP, int TimedDelay = 5000, ILogger _logger = null)
        {
            if (TimedDelay < 1000) throw new Exception($"TimedDelay < 1000 毫秒");
            await udpClient.P2PConnectAsync(p2PHole => ValueTask.FromResult(new P2pRetryInfo(TimedDelay - p2PHole.InitElapsedMs, localEP, RemoteEP)), _logger);
        }

        /// <summary>
        /// 尝试使用TCP模式进行P2P
        /// </summary>
        /// <param name="tcpClient">调起方</param>
        /// <param name="func">连接尝试委托</param>
        /// <param name="_logger">日志对象</param>
        /// <returns>异步任务</returns>
        public static async Task P2PConnectAsync(this TcpClientAsync tcpClient, Func<P2pHoleContext, ValueTask<P2pRetryInfo>> func, ILogger _logger = null)
        {
            if (tcpClient is null) throw new Exception($"tcpClient 不能为null");
            if (func is null) throw new Exception($"func 不能为null");
            tcpClient.TryP2PConnect = (endPoint) => TryP2PConnect(true, tcpClient.BufferSize, func, _logger);
            await tcpClient.ConnectAsync(DefaultRemoteEP);
        }

        /// <summary>
        /// 尝试使用UDP模式进行P2P
        /// </summary>
        /// <param name="udpClient">调起方</param>
        /// <param name="func">连接尝试委托</param>
        /// <param name="_logger">日志对象</param>
        /// <returns>异步任务</returns>
        public static async Task P2PConnectAsync(this UdpClientAsync udpClient, Func<P2pHoleContext, ValueTask<P2pRetryInfo>> func, ILogger _logger = null)
        {
            if (udpClient is null) throw new Exception($"udpClient 不能为null");
            if (func is null) throw new Exception($"func 不能为null");
            udpClient.TryP2PConnect = (endPoint) => TryP2PConnect(false, udpClient.BufferSize, func, _logger);
            await udpClient.ConnectAsync(DefaultRemoteEP);
        }

        private static async Task<P2pConnectResult> TryP2PConnect(bool isTcp, NetBufferSize bufferSize, Func<P2pHoleContext, ValueTask<P2pRetryInfo>> func, ILogger _logger = null)
        {
            string name = isTcp ? "TCP" : "UDP";
            bool islog = _logger != null;
            Stopwatch sw = Stopwatch.StartNew();

            int round = 0;
            Socket socket = null;
            EndPoint localEP = null, endPoint = null;

            Socket CreateSocket()
            {
                socket?.Dispose(); //重置Socket，再尝试！
                socket = GetP2pSocket(isTcp, bufferSize);
                return socket;
            }

            async Task<(IDisposable, int)> GetP2pRetry(CancellationToken token)
            {
                socket = CreateSocket();
                P2pHoleContext p2PHole = new(isTcp, ++round, (int)sw.ElapsedMilliseconds, token);
                var init = await func(p2PHole);//动态获取剩余超时值
                localEP = init.LocalEP;
                endPoint = init.EndPoint;
                return (p2PHole, init.Timeout);
            }

            async Task<int> Send(ArraySegment<byte> bytes)
            {
                Task<int> task = isTcp ? socket.SendAsync(bytes, SocketFlags.None) : socket.SendToAsync(bytes, SocketFlags.None, endPoint);
                return await task; //SocketTaskExtensions
            }

            async Task<int> Receive(ArraySegment<byte> buffer, CancellationToken token)
            {
                int coun;
                if (isTcp)
                {
                    coun = await socket.ReceiveAsync(buffer, SocketFlags.None, token);
                }
                else
                {
#if NET5_0
                    var result = await Task.Run(() => socket.ReceiveFromAsync(buffer, SocketFlags.None, endPoint), token);
#else
                    var result = await socket.ReceiveFromAsync(buffer, SocketFlags.None, endPoint, token);
#endif
                    //if (result.RemoteEndPoint is UdpEndPoint point0 && endPoint is UdpEndPoint point1 && point0 == point1)
                    //{
                    //    await socket.ConnectAsync(point0);
                    //}
                    //else
                    //{
                    //    throw new Exception("建立通道错误！");
                    //}
                    coun = result.ReceivedBytes;
                }
                return coun;
            }

            async Task<bool> ConnectAsync(int timedDelay)
            {
                int connectWindowMs = isTcp ? 500 : Math.Min(1000, timedDelay);
                using CancellationTokenSource tokenSource = new(connectWindowMs);
                var token = tokenSource.Token;
                TaskCompletionSource<bool> taskWith = new(TaskCreationOptions.RunContinuationsAsynchronously);

                try
                {
                    socket.Bind(localEP);
                    if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[Bind OK] {localEP}", localEP);
                }
                catch (SocketException ex)
                {
                    if (islog && _logger.IsEnabled(LogLevel.Warning)) _logger.LogWarning("[Bind失败] {localEP} -> {SocketErrorCode}", localEP, ex.SocketErrorCode);
                    return false;
                }

                bool isConnect = false;
                using var eventArgs = SocketEventPool.Pop();
                eventArgs.RemoteEndPoint = endPoint;
                eventArgs.Completed += (_, e) => { if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[Connect完成] Error={SocketError}, RemoteEP={RemoteEndPoint}", e.SocketError, e.RemoteEndPoint); isConnect = true; taskWith.TrySetResult(isConnect); };
                eventArgs.SocketError = SocketError.ConnectionRefused;
                token.UnsafeRegister((state) =>
                {
                    taskWith.TrySetResult(isConnect);
                }, null);

                if (!socket.ConnectAsync(eventArgs))
                {
                    isConnect = true;
                    taskWith.TrySetResult(isConnect);
                }

                if (await taskWith.Task)
                {
                    if (!isTcp && socket.RemoteEndPoint == null) eventArgs.SocketError = SocketError.SocketError;
                    if (eventArgs.SocketError is SocketError.Success)
                    {
                        var task = ReceiveAsync(token);
                        await Task.WhenAny(SendAsync(token), task);
                        if (!token.IsCancellationRequested) tokenSource.Cancel();
                        return await task;
                    }
                }
                if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[Connect失败] SocketError={SocketError}, RemoteEndPoint={RemoteEndPoint}", eventArgs.SocketError, socket.RemoteEndPoint);
                return false;

                //while (!token.IsCancellationRequested)
                //{
                //    try
                //    {
                //        if (!socket.ConnectAsync(eventArgs)) isConnect = true;
                //        SpinWait.SpinUntil(() =>
                //        {
                //            if (token.IsCancellationRequested) throw new Exception("超时取消任务！");
                //            return isConnect;
                //        });
                //        if (!isTcp && socket.RemoteEndPoint == null) eventArgs.SocketError = SocketError.SocketError;
                //        if (eventArgs.SocketError is SocketError.Success)
                //        {
                //            await Task.WhenAll(SendAsync(token), ReceiveAsync(token));
                //            break;
                //        }
                //        else
                //        {
                //            isConnect = false;
                //            //int delay = DateTime.UtcNow.Millisecond % 100; //延迟到统一时差
                //            //if (islog) _logger.LogDebug($"{endPoint}:Connect失败！补齐{delay}ms");
                //            //await Task.Delay(delay, token);
                //        }
                //    }
                //    catch
                //    {
                //    }
                //}
            }

            async Task SendAsync(CancellationToken token)
            {
                if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("{endPoint}:Send验证！", endPoint);
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        await Send(Bytes);//.IsNewTask();
                        int millisecondsDelay = isTcp ? 100 : 50;
                        await Task.Delay(millisecondsDelay, token);
                    }
                    catch
                    {
                    }
                }
            }

            async Task<bool> ReceiveAsync(CancellationToken token)
            {
                return await Task.Run(async () =>
                {
                    if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("{endPoint}:Receive验证！", endPoint);
                    ArraySegment<byte> buffer = new byte[9];
                    while (!token.IsCancellationRequested)
                    {
                        try
                        {
                            int coun = await Receive(buffer, token);//.IsNewTask();
                            if (Utility.SequenceCompare(Bytes, buffer[..coun])) return true; //receivecount.Increment(); else return;
                        }
                        catch (Exception)
                        {
                            break;
                        }
                    }
                    return false;
                }, CancellationToken.None);
            }

            async Task<bool> ReadyAsync(int timedDelay, CancellationToken token)
            {
                try { await Send(Ready); } catch (Exception) { return false; }

                using var cts = !isTcp ? new CancellationTokenSource(1000) : CancellationTokenSource.CreateLinkedTokenSource(token);
                token = cts.Token;

                ArraySegment<byte> buffer = new byte[9];
                if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("{endPoint}:认证{TotalMilliseconds}ms", endPoint, timedDelay - (int)sw.ElapsedMilliseconds); //auth
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        int coun = await Receive(buffer, token);
                        if (Utility.SequenceCompare(Ready, buffer[..coun])) return true;
                    }
                    catch (OperationCanceledException) // 取消，返回 false
                    {
                        if (isTcp) Throw($"[{name}]在P2P通道打通完成，但未能完成身份验证。");
                        return false;
                    }
                    catch (Exception)
                    {
                        // 旧数据 / 打洞残留，吞掉，继续等下一个 Ready
                    }
                }
                return false;
            }

            async Task<P2pConnectResult> StartP2pAsync()
            {
                bool p2psuccess = false;
                (IDisposable p2PHole, int timedDelay) = await GetP2pRetry(CancellationToken.None);
                //DateTime StartTime = DateTime.UtcNow;
                using CancellationTokenSource tokenSource = new(timedDelay);
                CancellationToken token = tokenSource.Token;
                while (!token.IsCancellationRequested)
                {
                    //await Task.Run(async () =>
                    //{
                    //    if (islog) _logger.LogDebug($"{endPoint}:开启{timedDelay - (DateTime.UtcNow - StartTime).TotalMilliseconds}ms！");

                    //    await ConnectAsync();

                    //    //if (isTcp)
                    //    //{
                    //    //    await ConnectAsync(token);
                    //    //}
                    //    //else
                    //    //{
                    //    //    using CancellationTokenSource tokenSource = new(500);
                    //    //    await ConnectAsync(tokenSource.Token);
                    //    //}
                    //});
                    //if (isTcp) break; else 

                    try
                    {
                        if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("[开启] localEP={localEP}, endPoint={endPoint}, 剩余={TotalMilliseconds:F0}ms", localEP, endPoint, timedDelay - (int)sw.ElapsedMilliseconds);
                        p2psuccess = await ConnectAsync(timedDelay) && await ReadyAsync(timedDelay, token);

                        if (p2psuccess) return new P2pConnectResult(socket, localEP, endPoint);
                        if (token.IsCancellationRequested) continue;
                    }
                    finally
                    {
                        p2PHole?.Dispose(); //可重复释放
                    }

                    if (islog && _logger.IsEnabled(LogLevel.Debug)) _logger.LogDebug("{endPoint}:[重试] 重建 Socket {Now:O}", endPoint, DateTime.Now);
                    (p2PHole, timedDelay) = await GetP2pRetry(token); //重新等待对接

                    //SpinWait.SpinUntil(() =>
                    //{
                    //    if (token.IsCancellationRequested)
                    //    {
                    //        isretry = false;
                    //        return true;
                    //    }
                    //    return DateTime.UtcNow.Millisecond % 100 < 5;
                    //});
                    //if (isretry)
                    //{
                    //    if (islog) _logger.LogDebug($"{endPoint}:重试！{DateTime.Now:O}");
                    //    socket.Dispose(); //重置Socket，再尝试！
                    //    socket = StateObject.CreateSocket(isTcp, bufferSize);
                    //    if (timedDelay == await func(token)) //重新获取剩余超时值（自实现）
                    //    {
                    //        //无实现模式，模拟实现
                    //        int delay = DateTime.UtcNow.Millisecond % 100; //延迟到统一时差
                    //        if (islog) _logger.LogDebug($"{endPoint}:Connect失败！补齐{delay}ms");
                    //        await Task.Delay(delay, token);
                    //    }
                    //}
                }

                Throw($"[{name}]在P2P超时期到来前未能成功打通通道。");
                return null;
            }

            void Throw(string mag)
            {
                //if (socket.Connected) socket.Close(); else if (!StateObject.SocketIsDispose(socket)) socket.Close();
                socket?.Dispose();
                throw new Exception(mag);
            }

            return await StartP2pAsync();
        }
    }
}
