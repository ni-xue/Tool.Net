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
        /// P2P超时等待时长（默认10秒）
        /// </summary>
        public static int TimedDelay { get; set; } = 10000;

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
        /// <returns>任务</returns>
        public static async Task P2PConnectAsync(this TcpClientAsync tcpClient, Ipv4Port localEP, Ipv4Port RemoteEP)
        {
            tcpClient.TryP2PConnect = (socket, endPoint) => TryP2PConnect(socket, new IPEndPoint(localEP.Ip, localEP.Port), endPoint);// //new IPEndPoint(IPAddress.Any, port); //IPEndPoint.Parse($"127.0.0.1:{port}");
            await tcpClient.ConnectAsync(RemoteEP);
        }

        /// <summary>
        /// 尝试使用UDP模式进行P2P
        /// </summary>
        /// <param name="udpClient">调起方</param>
        /// <param name="localEP">尝试绑定的IP端口</param>
        /// <param name="RemoteEP">尝试连接的IP端口</param>
        /// <returns>任务</returns>
        public static async Task P2PConnectAsync(this UdpClientAsync udpClient, Ipv4Port localEP, Ipv4Port RemoteEP)
        {
            udpClient.TryP2PConnect = (socket, endPoint) => TryP2PConnect(socket, new IPEndPoint(localEP.Ip, localEP.Port), endPoint);// //new IPEndPoint(IPAddress.Any, port); //IPEndPoint.Parse($"127.0.0.1:{port}");
            await udpClient.ConnectAsync(RemoteEP);
        }

        private static async Task TryP2PConnect(Socket socket, EndPoint localEP, EndPoint endPoint)
        {
            using CancellationTokenSource tokenSource = new(TimedDelay);
            bool p2psuccess = false, isTcp = socket.ProtocolType == ProtocolType.Tcp;

            socket.Bind(localEP);

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
                    coun = result.ReceivedBytes;
                }
                return coun;
            }

            async Task ConnectAsync()
            {
                using var eventArgs = SocketEventPool.Pop();
                eventArgs.RemoteEndPoint = endPoint;
                eventArgs.Completed += EventArgs_Completed;
                eventArgs.SocketError = SocketError.ConnectionRefused;
                bool isConnect = false;
                while (!tokenSource.IsCancellationRequested)
                {
                    try
                    {
                        //using CancellationTokenSource tokenSource = new(1000);
                        //await socket.ConnectAsync(endPoint, tokenSource.Token);

                        //DateTime time = DateTime.UtcNow;
                        if (!socket.ConnectAsync(eventArgs)) isConnect = true;

                        SpinWait.SpinUntil(() => isConnect);

                        if (!isTcp && socket.RemoteEndPoint is null)
                        {

                        }

                        if (eventArgs.SocketError is SocketError.Success)
                        {
                            if (!isTcp) await Task.Delay(1);
                            await Task.WhenAll(SendAsync(), ReceiveAsync());
                            break;
                        }
                        else
                        {
                            isConnect = false;
                            int delay = DateTime.UtcNow.Millisecond % 100; //延迟到统一时差
                            Debug.WriteLine($"{endPoint}:Connect失败！补齐{delay}ms");
                            await Task.Delay(delay);
                        }
                    }
                    catch
                    {
                    }
                }

                void EventArgs_Completed(object sender, SocketAsyncEventArgs e)
                {
                    isConnect = true;
                }
            }

            async Task SendAsync()
            {
                Debug.WriteLine($"{endPoint}:Send验证！");
                while (!tokenSource.IsCancellationRequested && !p2psuccess)
                {
                    try
                    {
                        await Send(Bytes);
                        int millisecondsDelay = isTcp ? 1000 : 100;
                        await Task.Delay(millisecondsDelay, tokenSource.Token);
                    }
                    catch
                    {
                    }
                }
            }

            async Task ReceiveAsync()
            {
                Debug.WriteLine($"{endPoint}:Receive验证！");
                ArraySegment<byte> buffer = new byte[9];
                while (!tokenSource.IsCancellationRequested && !p2psuccess)
                {
                    try
                    {
                        int coun = await Receive(buffer, tokenSource.Token);
                        if (Utility.SequenceCompare(Bytes, buffer[..coun])) p2psuccess = true; //receivecount.Increment(); else return;
                    }
                    catch (Exception)
                    {
                        return;
                    }
                }
            }

            //Task task = Task.Run(ConnectAsync);
            async Task ReadyAsync()
            {
                await Task.Run(ConnectAsync);

                //if (!SpinWait.SpinUntil(() => p2psuccess, 10000))
                //{
                //    tokenSource.Cancel();
                //    throw new Exception("在P2P超时期到来前未能成功打通通道。");
                //}

                if (p2psuccess)
                {
                    await Send(Ready);
                    ArraySegment<byte> buffer = new byte[9];
                    while (!tokenSource.IsCancellationRequested)
                    {
                        try
                        {
                            using CancellationTokenSource tokenSource = new(1000);
                            int coun = await Receive(buffer, tokenSource.Token);
                            if (Utility.SequenceCompare(Ready, buffer[..coun])) break; //receivecount.Increment(); else return;
                        }
                        catch (Exception)
                        {
                            throw new Exception("在P2P通道打通完成，但未能完成身份验证。");
                        }
                    }
                }
                else
                {
                    throw new Exception("在P2P超时期到来前未能成功打通通道。");
                }
            }

            await ReadyAsync();
        }
    }
}
