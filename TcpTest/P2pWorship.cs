using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.P2PHelpr;
using Tool.Sockets.TcpHelper;
using Tool.Sockets.UdpHelper;

namespace TcpTest
{
    internal class P2pWorship
    {
        static async ValueTask Completed<T>(string key, UserKey a1, T b1, DateTime c1) where T : Enum
        {
            await Console.Out.WriteLineAsync($"[{key}]IP:{a1} \t{b1} \t{c1:yyyy/MM/dd HH:mm:ss:fffffff}");
        }

        internal static async Task OnMain(string[] args, ILoggerFactory loggerFactory)
        {
            //while (true)
            //{
            //    Tool.Utils.Log.Debug("1111111111111111111111111");
            //    await Task.Delay(10);
            //}

            //var Client = new ClientFrame(NetBufferSize.Size128K, true) { IsThreadPool = false };
            //EnumEventQueue.OnInterceptor(EnClient.Receive, false);
            //EnumEventQueue.OnInterceptor(EnClient.SendMsg, false);
            //EnumEventQueue.OnInterceptor(EnClient.HeartBeat, false);
            //Client.SetCompleted(async (a1, b1, c1) =>
            //{
            //    switch (b1)
            //    {
            //        case EnClient.Connect:
            //            break;
            //        case EnClient.Fail:
            //        case EnClient.Close:
            //            break;
            //    }
            //    await Console.Out.WriteLineAsync($"IP:{a1} \t{b1} \t{c1:yyyy/MM/dd HH:mm:ss:fffffff}");
            //});

            //await Client.ConnectAsync("127.0.0.1", 8081);//120.79.58.17 //"127.0.0.1"

            //Console.ReadKey();

            //uint i0 = uint.MaxValue, i1 = uint.MaxValue;
            ////i++;

            //UdpClientAsync clientAsync0 = new(NetBufferSize.Size64K, false) { Millisecond = 0 /*ReceiveTimeout = 5000*/ };
            //clientAsync0.SetCompleted((age0, age1, age2) => age1 != EnClient.SendMsg ? Completed("UDPClient", age0, age1, age2) : ValueTask.CompletedTask);
            //await clientAsync0.ConnectAsync(12344);

            //using var sendBytes = clientAsync0.CreateSendBytes();
            //sendBytes.Span[5] = (byte)1;
            //await clientAsync0.SendAsync(sendBytes);

            //await clientAsync0.SendAsync(
            //       @"Hello Several .NET Aspire templates include ASP.NET Core projects that are configured to use HTTPS by default. 
            //        If this is the first time you're running the project, and you're using Visual Studio, you're prompted to install a
            //         localhost certificate.\r\n\r\nThere are situations in which you trust/install the development certificate, but you 
            //        don't close all your browser windows. In these cases, your browser might indicate that the certificate isn't trusted.
            //        \r\n\r\nThere are also situations where you don't trust the certificate at all. In these cases, your browser might indicate 
            //        that the certificate isn't trusted.\r\n\r\nAdditionally, there are warning messages from Kestrel written to the console that 
            //        indicate that the certificate is not trusted.

            //        实例 Stopwatch 可以测量一个间隔的已用时间，或跨多个间隔的总已用时间。 在典型Stopwatch方案中，
            //        调用 Start 方法，最终调用 Stop 方法，然后使用 属性检查运行时间Elapsed。

            //        Stopwatch实例正在运行或已停止;使用 IsRunning 确定 的Stopwatch当前状态。 使用 Start 开始测量已用时间;使用 Stop 停止测量已用时间。 
            //        通过属性 Elapsed、 ElapsedMilliseconds或 ElapsedTicks查询已用时间值。 可以在实例正在运行或停止时查询已用时间属性。 运行时间属性在 Stopwatch 运行时稳步增加;当实例停止时，它们保持不变。

            //        默认情况下，实例的已用时间值 Stopwatch 等于所有测量时间间隔的总和。 对 的每个调用 Start 在累积运行时间开始计数;
            //        对 的每次调用 Stop 将结束当前间隔度量并冻结累积已用时间值。 Reset使用 方法清除现有Stopwatch实例中的累积运行时间。

            //        通过 Stopwatch 对基础计时器机制中的计时器计时周期进行计数来测量运行时间。 如果安装的硬件和操作系统支持高分辨率性能计数器，
            //        则 Stopwatch 类使用该计数器来测量运行时间。 否则， Stopwatch 类使用系统计时器来测量已用时间。 Frequency使用 和 IsHighResolution 字段确定计时实现的Stopwatch精度和分辨率。

            //        类 Stopwatch 有助于在托管代码中操作与计时相关的性能计数器。 具体而言， Frequency 字段和 GetTimestamp 方法可用于代替非托管 
            //        Windows API QueryPerformanceFrequency 和 QueryPerformanceCounter。");

            //Console.ReadKey();
            //chaxun();

            //UdpServerAsync serverAsync = new(NetBufferSize.Size256K, true) { Millisecond = 0 /*ReceiveTimeout = 5000*/ };
            //serverAsync.SetCompleted((age0, age1, age2) => age1 != EnServer.SendMsg ? Completed($"UDPServer{i0.Increment()}", age0, age1, age2) : ValueTask.CompletedTask);
            //serverAsync.SetReceived(async a =>
            //{
            //    //await Console.Out.WriteLineAsync($"当前位：{a.OrderCount()} 原子计数：{i0.Increment()}");
            //    if (a.Length > 2)
            //    {
            //        await serverAsync.SendAsync(a.Client, "ok");
            //    }
            //    a.Dispose();
            //});
            //await serverAsync.StartAsync(12344);

            //for (int i = 0; i < 1; i++)
            //{
            //    await serverAsync.SendAsync("Hello Several .NET Aspire templates include ASP.NET Core projects that are configured to use HTTPS by default. If this is the first time you're running the project, and you're using Visual Studio, you're prompted to install a localhost certificate.\r\n\r\nThere are situations in which you trust/install the development certificate, but you don't close all your browser windows. In these cases, your browser might indicate that the certificate isn't trusted.\r\n\r\nThere are also situations where you don't trust the certificate at all. In these cases, your browser might indicate that the certificate isn't trusted.\r\n\r\nAdditionally, there are warning messages from Kestrel written to the console that indicate that the certificate is not trusted.");
            //}

            //Console.ReadKey();

            //UdpClientAsync clientAsync = new(NetBufferSize.Size256K, true) { Millisecond = 0 /*ReceiveTimeout = 5000*/ };
            //clientAsync.SetCompleted((age0, age1, age2) => age1 != EnClient.SendMsg ? Completed($"UDPClient{i1.Increment()}", age0, age1, age2) : ValueTask.CompletedTask);
            //clientAsync.SetReceived(async a =>
            //{
            //    //await Console.Out.WriteLineAsync($"当前位：{a.OrderCount()} 原子计数：{i0.Increment()}");
            //    if (a.Length > 2)
            //    {
            //        await clientAsync.SendAsync("ok");
            //    }
            //    a.Dispose();
            //});
            //await clientAsync.ConnectAsync(12344);

            //for (int i = 0; i < 1000; i++)
            //{
            //    await clientAsync.SendAsync(
            //        @"Hello Several .NET Aspire templates include ASP.NET Core projects that are configured to use HTTPS by default. 
            //        If this is the first time you're running the project, and you're using Visual Studio, you're prompted to install a
            //         localhost certificate.\r\n\r\nThere are situations in which you trust/install the development certificate, but you 
            //        don't close all your browser windows. In these cases, your browser might indicate that the certificate isn't trusted.
            //        \r\n\r\nThere are also situations where you don't trust the certificate at all. In these cases, your browser might indicate 
            //        that the certificate isn't trusted.\r\n\r\nAdditionally, there are warning messages from Kestrel written to the console that 
            //        indicate that the certificate is not trusted.

            //        实例 Stopwatch 可以测量一个间隔的已用时间，或跨多个间隔的总已用时间。 在典型Stopwatch方案中，
            //        调用 Start 方法，最终调用 Stop 方法，然后使用 属性检查运行时间Elapsed。

            //        Stopwatch实例正在运行或已停止;使用 IsRunning 确定 的Stopwatch当前状态。 使用 Start 开始测量已用时间;使用 Stop 停止测量已用时间。 
            //        通过属性 Elapsed、 ElapsedMilliseconds或 ElapsedTicks查询已用时间值。 可以在实例正在运行或停止时查询已用时间属性。 运行时间属性在 Stopwatch 运行时稳步增加;当实例停止时，它们保持不变。

            //        默认情况下，实例的已用时间值 Stopwatch 等于所有测量时间间隔的总和。 对 的每个调用 Start 在累积运行时间开始计数;
            //        对 的每次调用 Stop 将结束当前间隔度量并冻结累积已用时间值。 Reset使用 方法清除现有Stopwatch实例中的累积运行时间。

            //        通过 Stopwatch 对基础计时器机制中的计时器计时周期进行计数来测量运行时间。 如果安装的硬件和操作系统支持高分辨率性能计数器，
            //        则 Stopwatch 类使用该计数器来测量运行时间。 否则， Stopwatch 类使用系统计时器来测量已用时间。 Frequency使用 和 IsHighResolution 字段确定计时实现的Stopwatch精度和分辨率。

            //        类 Stopwatch 有助于在托管代码中操作与计时相关的性能计数器。 具体而言， Frequency 字段和 GetTimestamp 方法可用于代替非托管 
            //        Windows API QueryPerformanceFrequency 和 QueryPerformanceCounter。");
            //}

            //Console.ReadKey();
            //await PipeAsync();

            //P2pClientAsync.TimedDelay = 60000;

            //TcpClientAsync p2PClientAsync = new(NetBufferSize.Default, true);
            //p2PClientAsync.SetCompleted((age0, age1, age2) => Completed("TCP", age0, age1, age2));
            //await p2PClientAsync.P2PConnectAsync("192.168.31.33:54235", "192.168.31.19:3333");

            //112.19.21.42:2980
            //112.19.21.42:2981
            //127.0.0.1 192.168.31.33  106.55.49.68:8080
            //string server = "106.55.49.68";
            //string server = "127.0.0.1";

            ILogger 
                logger = loggerFactory.CreateLogger("P2pTest"),
                servicelog = loggerFactory.CreateLogger("P2pService"),
                freeInKeylog = loggerFactory.CreateLogger("FreeInKey"),
                freeOutKeylog = loggerFactory.CreateLogger("FreeOutKey");

            logger.LogInformation("尝试P2P：{Now:yyyy/MM/dd HH:mm:ss:fffffff}", DateTime.Now);

            var iPAddress = IPAddress.Parse("47.108.164.106"); // 47.108.164.106 | 127.0.0.1

            var endPoint = new IPEndPoint(iPAddress, 22222);

            //using P2pServiceAsync p2PService = new(servicelog);
            //await p2PService.StartAsync($"0.0.0.0:{endPoint.Port}");

            for (int i = 1; i <= 5; i++)
            {
                try
                {
                    logger.LogInformation("开始测试，第{i}次：{Now:yyyy/MM/dd HH:mm:ss:fffffff}", i, DateTime.Now);

                    {
                        var sessionA = await P2pServerAsync.GetFreeInKeyAsync(endPoint, logger);
                        var sessionB = await P2pServerAsync.GetFreeOutKeyAsync(endPoint, sessionA.Key, logger);

                        // 双方各自打洞（TCP 版本）
                        var clientA = new TcpClientAsync(NetBufferSize.Default, true);
                        var task0 = sessionA.P2PConnectAsync(clientA, 10000);

                        var clientB = new TcpClientAsync(NetBufferSize.Default, true);
                        var task1 = sessionB.P2PConnectAsync(clientB, 10000);

                        await Task.WhenAll(task0, task1);
                        logger.LogInformation("成功[Tcp]P2P：{Now:yyyy/MM/dd HH:mm:ss:fffffff}", DateTime.Now);
                    }

                    {
                        // UDP 轮：新 Key、新会话
                        var sessionA = await P2pServerAsync.GetFreeInKeyAsync(endPoint, freeInKeylog);
                        var sessionB = await P2pServerAsync.GetFreeOutKeyAsync(endPoint, sessionA.Key, freeOutKeylog);

                        // 双方各自打洞（UDP 版本）
                        var clientA = new UdpClientAsync(NetBufferSize.Default, true);
                        var task2 = sessionA.P2PConnectAsync(clientA, 10000);

                        var clientB = new UdpClientAsync(NetBufferSize.Default, true);
                        var task3 = sessionB.P2PConnectAsync(clientB, 10000);

                        await Task.WhenAll(task2, task3);
                        logger.LogInformation("成功[Udp]P2P：{Now:yyyy/MM/dd HH:mm:ss:fffffff}", DateTime.Now);
                    }

                    //Tool.Utils.JsonVar jsons = new Tool.Utils.JsonVar();
                    //jsons.Select();

                    //{
                    //    // 只测试创建 端口 行为
                    //    var sessionA = await P2pServerAsync.GetFreeInKeyAsync(endPoint, freeInKeylog);
                    //    var sessionB = await P2pServerAsync.GetFreeOutKeyAsync(endPoint, sessionA.Key, freeOutKeylog);

                    //    Func<P2PRetryInfo, ValueTask> puncherA = async info =>
                    //    {
                    //        freeInKeylog.LogInformation("[TestA] 窗口: local={local}, remote={remote}, timeout={timeout}",
                    //            info.LocalEP, info.EndPoint, info.Timeout);
                    //    };

                    //    Func<P2PRetryInfo, ValueTask> puncherB = async info =>
                    //    {
                    //        freeOutKeylog.LogInformation("[TestB] 窗口: local={local}, remote={remote}, timeout={timeout}",
                    //            info.LocalEP, info.EndPoint, info.Timeout);
                    //    };

                    //    var task2 = sessionA.P2PConnectTestAsync(60000, puncherA);
                    //    var task3 = sessionB.P2PConnectTestAsync(60000, puncherB);

                    //    await Task.WhenAll(task2, task3);
                    //    logger.LogInformation("成功[多次创建端口]P2P：{Now:yyyy/MM/dd HH:mm:ss:fffffff}", DateTime.Now);
                    //}
                }
                catch (Exception)
                {
                    logger.LogInformation("第{i}次，打洞失败！{Now:yyyy/MM/dd HH:mm:ss:fffffff}", i, DateTime.Now);
                }
            }

            await Task.Delay(Timeout.Infinite);

            //#if false
            //            P2pServerAsync p2PServerAsync0 = await P2pServerAsync.GetFreeTcp();
            //            P2pServerAsync p2PServerAsync1 = await P2pServerAsync.GetFreeTcp();

            //            await Task.Delay(2000);

            //            TcpClientAsync p2PClientAsync0 = new(NetBufferSize.Default, true);
            //            p2PClientAsync0.SetCompleted((age0, age1, age2) => Completed("TCP", age0, age1, age2));
            //            var task0 = p2PServerAsync0.P2PConnectAsync(p2PClientAsync0, p2PServerAsync1.RemoteEP);
            //            //var task0 = p2PClientAsync0.P2PConnectAsync(p2PServerAsync0.LocalEP, p2PServerAsync1.RemoteEP);

            //            await Task.Delay(2000);

            //            TcpClientAsync p2PClientAsync1 = new(NetBufferSize.Default, true);
            //            p2PClientAsync1.SetCompleted((age0, age1, age2) => Completed("TCP", age0, age1, age2));
            //            var task1 = p2PServerAsync1.P2PConnectAsync(p2PClientAsync1, p2PServerAsync0.RemoteEP);
            //            //var task1 = p2PClientAsync1.P2PConnectAsync(p2PServerAsync1.LocalEP, p2PServerAsync0.RemoteEP);
            //#else
            //            P2pServerAsync p2PServerAsync0 = await P2pServerAsync.GetFreeUdp();
            //            P2pServerAsync p2PServerAsync1 = await P2pServerAsync.GetFreeUdp();

            //            await Task.Delay(10000);

            //            UdpClientAsync p2PClientAsync0 = new(NetBufferSize.Default, true) { ReceiveTimeout = 5000 };
            //            p2PClientAsync0.SetCompleted((age0, age1, age2) => Completed("UDP", age0, age1, age2));
            //            var task0 = p2PServerAsync0.P2PConnectAsync(p2PClientAsync0, p2PServerAsync1.RemoteEP, 100000);
            //            //var task0 = p2PClientAsync0.P2PConnectAsync(p2PServerAsync0.LocalEP, p2PServerAsync1.RemoteEP);

            //            await Task.Delay(10000);

            //            UdpClientAsync p2PClientAsync1 = new(NetBufferSize.Default, true) { ReceiveTimeout = 5000 };
            //            p2PClientAsync1.SetCompleted((age0, age1, age2) => Completed("UDP", age0, age1, age2));
            //            var task1 = p2PServerAsync1.P2PConnectAsync(p2PClientAsync1, p2PServerAsync0.RemoteEP, 100000);
            //            //var task1 = p2PClientAsync1.P2PConnectAsync(p2PServerAsync1.LocalEP, p2PServerAsync0.RemoteEP);
            //#endif

            //            Task.WaitAll(task0, task1);
            //            await Console.Out.WriteLineAsync($"成功P2P：{DateTime.Now:yyyy/MM/dd HH:mm:ss:fffffff}");

            //            p2PServerAsync0.Dispose();
            //            p2PServerAsync1.Dispose();

            //            p2PClientAsync0.AddKeepAlive(1);
            //            p2PClientAsync1.AddKeepAlive(1);

            //            Console.ReadKey();
            //            p2PClientAsync0.Dispose();
            //            p2PClientAsync1.Dispose();

            //UserKey userKey = "1.1.1.1:65535";

            //Ipv4Port ipv4Port = userKey;

            //ipv4Port.GetHashCode();

            //StateObject.IsIpPort("120.255.1.1:1", out var ipnum);

            //Creates an IpEndPoint.
            //IPAddress ipAddress = IPAddress.Parse("127.0.0.1");
            //IPEndPoint ipLocalEndPoint = new IPEndPoint(ipAddress, 8000);
            //BitConverter.GetBytes((ushort)8000);
            ////Serializes the IPEndPoint.
            //SocketAddress socketAddress = ipLocalEndPoint.Serialize();

            ////Verifies that ipLocalEndPoint is now serialized by printing its contents.
            //Console.WriteLine("Contents of the socketAddress are: " + socketAddress.ToString());
            ////Checks the Family property.
            //Console.WriteLine("The address family of the socketAddress is: " + socketAddress.Family.ToString());
            ////Checks the underlying buffer size.
            //Console.WriteLine("The size of the underlying buffer is: " + socketAddress.Size.ToString());

            //byte[] data = { 0, 1, };

            //var a0 = BinaryPrimitives.ReadUInt16BigEndian(data);
            //var a1 = BinaryPrimitives.ReadUInt16LittleEndian(data);

            //var a2 = BitConverter.ToUInt16(data);
            //var a3 = BitConverter.ToUInt16(data.Reverse().ToArray());

        }
    }
}
