using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tool;
using Tool.Sockets.Kernels;
using Tool.Sockets.NetFrame;
using Tool.Sockets.TcpHelper;
using Tool.Utils;

namespace TcpTest
{
    internal class TcpWorship
    {
        internal static async Task OnMain(string[] args)
        {
            //Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Socket socket0
            ////socket0.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 444));
            ////socket0.Listen();
            ////Socket socket = socket0.Accept();
            //socket.Connect("127.0.0.1", 444);
            //var _dataLength = 108 * 1024 - 6;
            //Tool.Sockets.TcpHelper.TcpClient tcpClient = new(socket, _dataLength);

            //Memory<byte> data = new byte[] { 40, 0, 0, 0, 0, 41, 123, 1, 2, 3, 2, 1, 123 };

            //BitConverter.TryWriteBytes(data[1..].Span, 7);

            ////while (true)
            ////{
            ////    var memoryOwner = MemoryPool<byte>.Shared.Rent(data.Length);
            ////    data.CopyTo(memoryOwner.Memory);
            ////    tcpClient.SendAsync(new TcpBytes("", socket, memoryOwner, data.Length));
            ////    Thread.Sleep(Random.Shared.Next(0, 2));
            ////}
            //int lre = int.Parse("100000");
            //KeepAlive keep = new(1, async () =>
            //{
            //    for (int i = 0; i < lre; i++)
            //    {
            //        var memoryOwner = MemoryPool<byte>.Shared.Rent(data.Length);
            //        data.CopyTo(memoryOwner.Memory);
            //        tcpClient.SendAsync(new SendBytes(memoryOwner, data.Length));
            //        await Task.Delay(i);
            //        //Thread.Sleep(Random.Shared.Next(0, 2));
            //    }
            //    //Thread.Sleep(100000);
            //});

            //Console.ReadKey();

            SendBytes<System.Net.Sockets.Socket> sendBytes = default;

            ulong a = 0, b = 0, c = 0;// e = 0, f = 0, g = 0, s = 0;

            KeepAlive keepok = new(1, async () =>
            {
                Console.Clear();
                Console.WriteLine("情况：{0}，{1}，{2}", ThreadPool.ThreadCount, ThreadPool.PendingWorkItemCount, ThreadPool.CompletedWorkItemCount);
                for (int i = 0; i < 20; i++)
                {
                    await Task.Delay(i);
                    Console.WriteLine("发起：收 {0},对 {1},发 {2}", a, b, c);
                }
            });

            TcpServerAsync server = new(NetBufferSize.Default, true);
            server.Millisecond = 0;
            //server.SetCompleted((a1, b1, c1) =>
            //{

            //});

            server.SetReceived(async (receive) =>
            {
                try
                {
                    using (receive)
                    {
                        if (Utility.SequenceCompare(receive.Span, sendBytes.Span))
                        {
                            //await Console.Out.WriteLineAsync("验证成功！");
                            Interlocked.Increment(ref b);
                        }

                        Interlocked.Increment(ref a);
                        await server.SendAsync(receive.Client, "接收成功！");
                    }
                }
                catch (Exception)
                {

                }
            });

            await server.StartAsync("127.0.0.1", 444);

            TcpClientAsync client = new(NetBufferSize.Default, true);
            client.Millisecond = 0;

            bool IsRead = false;
            client.SetCompleted(async (a1, b1, c1) =>
            {
                switch (b1)
                {
                    case EnClient.Connect:
                        a = b = c = 0;
                        SpinWait.SpinUntil(() => IsRead);
                        goto case EnClient.Receive;
                    case EnClient.Fail:
                        break;
                    case EnClient.SendMsg:
                        break;
                    case EnClient.Receive:
                        if (Interlocked.Increment(ref c) < 10)
                            await client.SendAsync(sendBytes);
                        break;
                    case EnClient.Close:
                        break;
                    case EnClient.HeartBeat:
                        break;
                    default:
                        break;
                }
                //Console.WriteLine("\nIP:{0} \t{1} \t{2}", a1, b1, c1.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
                await ValueTask.CompletedTask;
            });

            //client.SetReceived(async (receive) =>
            //{
            //    try
            //    {
            //        using (receive)
            //        {
            //            //await Console.Out.WriteLineAsync($"ip:{a.Key},{Encoding.UTF8.GetString(a.Span)},{DateTime.Now}");
            //            //await Task.Delay(10);
            //            Interlocked.Increment(ref b);
            //            await client.SendAsync(sendBytes);
            //        }
            //    }
            //    catch (Exception)
            //    {

            //    }
            //});

            await client.ConnectAsync("127.0.0.1", 444);//120.79.58.17 
            client.AddKeepAlive(5);

            FileStream ReadStream = new("D:\\ToDesk0.exe", FileMode.Open, FileAccess.Read);
            sendBytes = client.CreateSendBytes((int)ReadStream.Length);
            await ReadStream.ReadAsync(sendBytes.Memory);
            IsRead = true;
        }
    }
}
