using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Utils;

namespace TcpTest
{
    internal class WebWorship
    {
        static int a1, b1, c1 = 0;

        internal static async Task OnMain(string[] args)
        {
            var segment1 = new MemorySegment<byte>().Append([1, 2]).Append([3, 4, 5]).Append([6, 7, 8, 9, 10]);
            //var segment2 = new MemorySegment<byte>([4, 5, 6], segment1);
            //var segment3 = new MemorySegment<byte>([1, 2, 3], segment2);

            ReadOnlySequence<byte> readOnlies = segment1.ToReadOnlySequence();

            //Task4();
            SendBytes<WebSocket> sendBytes = default;

            string? name = null;
            KeepAlive keep = new(1, async () =>
            {
                Console.Clear();
                Console.WriteLine("情况：{0}，{1}，{2} · {3}", ThreadPool.ThreadCount, ThreadPool.PendingWorkItemCount, ThreadPool.CompletedWorkItemCount, name);

                for (int i = 0; i < 20; i++)
                {
                    await Task.Delay(i);
                    Console.WriteLine("接收：总收 {0}，总发 {1}", a1, b1);
                }
            });

            //var _a = Interlocked.Exchange(ref abc1, new Abc());// ?? new Abc();

            Tool.Sockets.WebHelper.WebServerAsync webServer = new(NetBufferSize.Size1024K) { Millisecond = 0, IsSSL = false };
            webServer.Millisecond = 0;
            webServer.SetReceived(async (a) =>
            {
                try
                {
                    using (a)
                    {
                        if (Utility.SequenceCompare(a.Span, sendBytes.Span))
                        {
                            await Console.Out.WriteLineAsync("验证成功！");
                        }

                        //await Console.Out.WriteLineAsync($"ip:{a.Key},{Encoding.UTF8.GetString(a.Span)},{DateTime.Now}");
                        //await Task.Delay(10);
                        Interlocked.Increment(ref a1);
                        await webServer.SendAsync(a.Client, "接收成功！");
                    }
                }
                catch (Exception)
                {

                }
            });

            webServer.SetCompleted(async (a, b, c) =>
            {
                switch (b)
                {
                    case EnServer.Connect:
                        //await webClient.SendAsync($"打招呼{Interlocked.Increment(ref c1)} {DateTime.Now}");
                        break;
                    case EnServer.Fail:
                        Console.Clear();
                        break;
                    case EnServer.SendMsg:
                        break;
                    case EnServer.Receive:
                        break;
                    case EnServer.Close:
                        Console.Clear();
                        break;
                    case EnServer.HeartBeat:
                        break;
                }

                //Console.WriteLine($"Server:{b} {DateTime.Now}");

                await Task.CompletedTask;
            });

            await webServer.StartAsync("127.0.0.1", 9999);

            //Console.WriteLine($"已启动服务器！");
            //Console.ReadKey();
            //return;

            Tool.Sockets.WebHelper.WebClientAsync webClient = new(NetBufferSize.Size1024K, true) { Millisecond = 0, IsSSL = false };
            webClient.Millisecond = 0;
            webClient.SetReceived(async (a) =>
            {
                try
                {
                    using (a)
                    {
                        //await Console.Out.WriteLineAsync($"ip:{a.Key},{Encoding.UTF8.GetString(a.Span)},{DateTime.Now}");
                        //await Task.Delay(10);
                        Interlocked.Increment(ref b1);
                        await webClient.SendAsync(sendBytes);
                    }
                }
                catch (Exception)
                {

                }
            });

            bool IsRead = false;
            webClient.SetCompleted(async (a, b, c) =>
            {
                switch (b)
                {
                    case EnClient.Connect:
                        Interlocked.Increment(ref c1); //await webClient.SendAsync($"打招呼{Interlocked.Increment(ref c1)} {DateTime.Now}");
                        SpinWait.SpinUntil(() => IsRead);
                        await webClient.SendAsync(sendBytes);
                        break;
                    case EnClient.Fail:
                        Console.Clear();
                        break;
                    case EnClient.SendMsg:
                        break;
                    case EnClient.Receive:
                        break;
                    case EnClient.Close:
                        Console.Clear();
                        break;
                    case EnClient.HeartBeat:
                        break;
                }

                //Console.WriteLine($"Client:{b} {DateTime.Now}");

                await Task.CompletedTask;
            });

            await webClient.ConnectAsync("127.0.0.1", 9999);
            webClient.AddKeepAlive(1);

            FileStream ReadStream = new("D:\\ToDesk0.exe", FileMode.Open, FileAccess.Read);
            sendBytes = webClient.CreateSendBytes((int)ReadStream.Length);
            int len = await ReadStream.ReadAsync(sendBytes.Memory);
            if (len > 0) { }
            IsRead = true;

            while (Console.ReadKey(true).Key != ConsoleKey.F5) ;
            return;
        }
    }
}
