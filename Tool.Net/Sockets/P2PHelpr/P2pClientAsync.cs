using System;
using System.Diagnostics;
using System.Diagnostics.Tracing;
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
    /// 封装一个可用的P2P打洞实现
    /// 帮助实现P2P打洞
    /// </summary>
    public static class P2pClientAsync
    {
        /// <summary>
        /// 描述P2P验证协议
        /// </summary>
        private static readonly ArraySegment<byte> Bytes = new byte[] { 255, 225, 195, 155, 125, 95, 55, 25, 0 };

        /// <summary>
        /// 描述P2P完成协议
        /// </summary>
        private static readonly ArraySegment<byte> Ready = new byte[] { 10, 20, 40, 80, 160, 255 };

        /// <summary>
        /// 尝试使用TCP模式进行P2P
        /// </summary>
        /// <param name="tcpClient">调起方</param>
        /// <param name="localEP">尝试绑定的IP端口</param>
        /// <param name="RemoteEP">尝试连接的IP端口</param>
        /// <param name="TimedDelay">P2P超时等待时长（默认5秒）</param>
        /// <returns>任务</returns>
        public static async Task P2PConnectAsync(this TcpClientAsync tcpClient, Ipv4Port localEP, Ipv4Port RemoteEP, int TimedDelay = 5000)
        {
            if (TimedDelay < 1000) throw new Exception($"TimedDelay < 1000 毫秒");
            await tcpClient.P2PConnectAsync(localEP, RemoteEP, _ => ValueTask.FromResult(TimedDelay));
        }

        /// <summary>
        /// 尝试使用UDP模式进行P2P
        /// </summary>
        /// <param name="udpClient">调起方</param>
        /// <param name="localEP">尝试绑定的IP端口</param>
        /// <param name="RemoteEP">尝试连接的IP端口</param>
        /// <param name="TimedDelay">P2P超时等待时长（默认5秒）</param>
        /// <returns>任务</returns>
        public static async Task P2PConnectAsync(this UdpClientAsync udpClient, Ipv4Port localEP, Ipv4Port RemoteEP, int TimedDelay = 5000)
        {
            if (TimedDelay < 1000) throw new Exception($"TimedDelay < 1000 毫秒");
            await udpClient.P2PConnectAsync(localEP, RemoteEP, _ => ValueTask.FromResult(TimedDelay));
        }

        internal static async Task P2PConnectAsync(this TcpClientAsync tcpClient, Ipv4Port localEP, Ipv4Port RemoteEP, Func<CancellationToken, ValueTask<int>> func)
        {
            tcpClient.TryP2PConnect = (endPoint) => TryP2PConnect(true, tcpClient.BufferSize, new IPEndPoint(localEP.Ip, localEP.Port), endPoint, func); //new IPEndPoint(IPAddress.Any, port); //IPEndPoint.Parse($"127.0.0.1:{port}");
            await tcpClient.ConnectAsync(RemoteEP);
        }

        internal static async Task P2PConnectAsync(this UdpClientAsync udpClient, Ipv4Port localEP, Ipv4Port RemoteEP, Func<CancellationToken, ValueTask<int>> func)
        {
            udpClient.TryP2PConnect = (endPoint) => TryP2PConnect(false, udpClient.BufferSize, new IPEndPoint(localEP.Ip, localEP.Port), endPoint, func);
            await udpClient.ConnectAsync(RemoteEP);
        }

        private static async Task<Socket> TryP2PConnect(bool isTcp, NetBufferSize bufferSize, EndPoint localEP, EndPoint endPoint, Func<CancellationToken, ValueTask<int>> func)
        {
            Socket socket = StateObject.CreateSocket(isTcp, bufferSize);
            bool p2psuccess = false;
            int timedDelay = await func(CancellationToken.None); //动态获取剩余超时值

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

            async Task ConnectAsync(CancellationToken token)
            {
                socket.Bind(localEP);

                bool isConnect = false;
                using var eventArgs = SocketEventPool.Pop();
                eventArgs.RemoteEndPoint = endPoint;
                eventArgs.Completed += (_, _) => { isConnect = true; };
                eventArgs.SocketError = SocketError.ConnectionRefused;
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        if (!socket.ConnectAsync(eventArgs)) isConnect = true;
                        SpinWait.SpinUntil(() =>
                        {
                            if (token.IsCancellationRequested) throw new Exception("超时取消任务！");
                            return isConnect;
                        });
                        if (!isTcp && socket.RemoteEndPoint == null) eventArgs.SocketError = SocketError.SocketError;
                        if (eventArgs.SocketError is SocketError.Success)
                        {
                            await Task.WhenAll(SendAsync(token), ReceiveAsync(token));
                            break;
                        }
                        else
                        {
                            isConnect = false;
                            //int delay = DateTime.UtcNow.Millisecond % 100; //延迟到统一时差
                            //Debug.WriteLine($"{endPoint}:Connect失败！补齐{delay}ms");
                            //await Task.Delay(delay, token);
                        }
                    }
                    catch
                    {
                    }
                }
            }

            async Task SendAsync(CancellationToken token)
            {
                Debug.WriteLine($"{endPoint}:Send验证！");
                while (!token.IsCancellationRequested && !p2psuccess)
                {
                    try
                    {
                        await Send(Bytes).IsNewTask();
                        int millisecondsDelay = isTcp ? 100 : 50;
                        await Task.Delay(millisecondsDelay, token);
                    }
                    catch
                    {
                    }
                }
            }

            async Task ReceiveAsync(CancellationToken token)
            {
                Debug.WriteLine($"{endPoint}:Receive验证！");
                ArraySegment<byte> buffer = new byte[9];
                while (!token.IsCancellationRequested && !p2psuccess)
                {
                    try
                    {
                        int coun = await Receive(buffer, token).IsNewTask();
                        if (Utility.SequenceCompare(Bytes, buffer[..coun])) p2psuccess = true; //receivecount.Increment(); else return;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }

            async Task<Socket> ReadyAsync()
            {
                DateTime StartTime = DateTime.UtcNow;
                using CancellationTokenSource tokenSource = new(timedDelay);
                while (!tokenSource.IsCancellationRequested)
                {
                    await Task.Run(async () =>
                    {
                        Debug.WriteLine($"{endPoint}:开启{timedDelay - (DateTime.UtcNow - StartTime).TotalMilliseconds}ms！");
                        using CancellationTokenSource tokenSource = new(500);
                        await ConnectAsync(tokenSource.Token);

                        //if (isTcp)
                        //{
                        //    await ConnectAsync(tokenSource.Token);
                        //}
                        //else
                        //{
                        //    using CancellationTokenSource tokenSource = new(500);
                        //    await ConnectAsync(tokenSource.Token);
                        //}
                    });
                    //if (isTcp) break; else 
                    if (p2psuccess) break;
                    bool isretry = true;
                    if (tokenSource.IsCancellationRequested) isretry = false;
                    //SpinWait.SpinUntil(() =>
                    //{
                    //    if (tokenSource.IsCancellationRequested)
                    //    {
                    //        isretry = false;
                    //        return true;
                    //    }
                    //    return DateTime.UtcNow.Millisecond % 100 < 5;
                    //});
                    if (isretry)
                    {
                        Debug.WriteLine($"{endPoint}:重试！{DateTime.Now:O}");
                        socket.Dispose(); //重置Socket，再尝试！
                        socket = StateObject.CreateSocket(isTcp, bufferSize);
                        if(timedDelay == await func(tokenSource.Token)) //重新获取剩余超时值（自实现）
                        {
                            //无实现模式，模拟实现
                            int delay = DateTime.UtcNow.Millisecond % 100; //延迟到统一时差
                            Debug.WriteLine($"{endPoint}:Connect失败！补齐{delay}ms");
                            await Task.Delay(delay, tokenSource.Token);
                        }
                    }
                }

                if (p2psuccess)
                {
                    await Send(Ready);
                    ArraySegment<byte> buffer = new byte[9];
                    Debug.WriteLine($"{endPoint}:认证{timedDelay - (DateTime.UtcNow - StartTime).TotalMilliseconds}ms！"); //auth
                    while (!tokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            int coun = await Receive(buffer, tokenSource.Token);
                            if (Utility.SequenceCompare(Ready, buffer[..coun])) return socket;
                        }
                        catch (Exception)
                        {
                        }
                    }
                    Throw("在P2P通道打通完成，但未能完成身份验证。");
                }
                else
                {
                    Throw("在P2P超时期到来前未能成功打通通道。");
                }

                return null;
            }

            void Throw(string mag)
            {
                //if (socket.Connected) socket.Close(); else if (!StateObject.SocketIsDispose(socket)) socket.Close();
                socket.Dispose();
                throw new Exception(mag);
            }

            return await ReadyAsync();
        }
    }
}
