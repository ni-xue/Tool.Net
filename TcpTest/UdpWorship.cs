using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.UdpHelper;
using System.Diagnostics;
using System.Threading;
using Tool.Utils;
using Tool;
using System.Runtime.Versioning;

namespace TcpTest
{
    public class UdpWorship
    {
        public static async void SendTo4()
        {
            //IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 11000);

            Socket s = new Socket(endPoint.Address.AddressFamily,
                SocketType.Dgram,
                ProtocolType.Udp);

            Memory<byte> msg = Encoding.UTF8.GetBytes($"主动推送");
            //Console.WriteLine("Sending data.");
            // This call blocks.
            await s.SendToAsync(msg, SocketFlags.None, endPoint);

            int c = 1;
            while (true)
            {
                Memory<byte> msg1 = new byte[256 * c];
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint senderRemote = (EndPoint)sender;
                try
                {
                    var i = await s.ReceiveFromAsync(msg1, SocketFlags.None, senderRemote);
                    if (i.ReceivedBytes > 0)
                    {
                        Console.WriteLine(Encoding.UTF8.GetString(msg1.ToArray(), 0, i.ReceivedBytes));
                    }
                }
                catch (Exception)
                {
                    c++;
                }

                //c++;
            }

            //s.Close();
        }

        public static async void ReceiveFrom3()
        {
            //IPHostEntry hostEntry = Dns.GetHostEntry(Dns.GetHostName());
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Loopback, 11000);

            Socket s = new Socket(endPoint.Address.AddressFamily,
                SocketType.Dgram,
                ProtocolType.Udp);

            // Creates an IPEndPoint to capture the identity of the sending host.

            // Binding is required with ReceiveFrom calls.
            s.Bind(endPoint);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint senderRemote = (EndPoint)sender;
            Memory<byte> msg = new byte[256];
            //Console.WriteLine("Waiting to receive datagrams from client...");
            // This call blocks.
            var i = await s.ReceiveFromAsync(msg, SocketFlags.None, senderRemote, default);
            if (i.ReceivedBytes > 0)
            {
                senderRemote = i.RemoteEndPoint;
                Console.WriteLine(Encoding.UTF8.GetString(msg.ToArray(), 0, i.ReceivedBytes));
            }

            int c = 0;
            while (true)
            {
                Memory<byte> msg1 = Encoding.UTF8.GetBytes($"回复：{c}");
                //Console.WriteLine("Sending data.");
                // This call blocks.
                await s.SendToAsync(msg1, SocketFlags.None, senderRemote);
                c++;
            }
            //s.Close();
        }

        public static async Task Cs0() 
        {
            Console.WriteLine($"（原始）开始：");
            Stopwatch watch = Stopwatch.StartNew();
            int g1 = 0;
            void cs1()
            {
                while (watch.ElapsedMilliseconds < 20000)
                {
                    var slim = new ManualResetEventSlim();
                    Interlocked.Increment(ref g1);
                    if (slim.IsSet)
                    {

                    }
                    else
                    {

                    }

                    slim.Wait(0);
                    slim.Set();
                    slim.Reset();

                    slim.Dispose();
                }
            }

            var q1 = Task.Run(cs1);
            var q2 = Task.Run(cs1);
            var q3 = Task.Run(cs1);
            var q4 = Task.Run(cs1);
            var q5 = Task.Run(cs1);
            var q6 = Task.Run(cs1);
            var q7 = Task.Run(cs1);
            var q8 = Task.Run(cs1);
            var q9 = Task.Run(cs1);

            await Task.WhenAll(q1, q2, q3, q4, q5, q6, q7, q8, q9);

            Console.WriteLine($"（原始）累计创建数：{g1} - {watch.ElapsedMilliseconds}ms");
        }

        public static async Task Cs1()
        {
            Console.WriteLine($"（自制版）开始：");
            Stopwatch watch = Stopwatch.StartNew();
            int g1 = 0;
            ObjectPool<ManualResetEventSlim> resetEventPool = new();

            var q1 = Task.Run(cs);
            var q2 = Task.Run(cs);
            var q3 = Task.Run(cs);
            var q4 = Task.Run(cs);
            var q5 = Task.Run(cs);
            var q6 = Task.Run(cs);
            var q7 = Task.Run(cs);
            var q8 = Task.Run(cs);
            var q9 = Task.Run(cs);

            await Task.WhenAll(q1, q2, q3, q4, q5, q6, q7, q8, q9);

            void cs()
            {
                while (watch.ElapsedMilliseconds < 20000)
                {
                    var slim = resetEventPool.Get();
                    Interlocked.Increment(ref g1);
                    if (slim.IsSet)
                    {

                    }
                    else
                    {

                    }

                    slim.Wait(0);
                    slim.Set();
                    slim.Reset();

                    resetEventPool.Return(slim);
                }
            }

            Console.WriteLine($"（自制版）累计创建数：{g1} - {watch.ElapsedMilliseconds}ms");
        }

        public static async Task Cs2()
        {
            Console.WriteLine($"（自带版）开始：");
            Stopwatch watch = Stopwatch.StartNew();
            int g1 = 0;
            var resetEventPool = Microsoft.Extensions.ObjectPool.ObjectPool.Create<ManualResetEventSlim>();

            var q1 = Task.Run(cs);
            var q2 = Task.Run(cs);
            var q3 = Task.Run(cs);
            var q4 = Task.Run(cs);
            var q5 = Task.Run(cs);
            var q6 = Task.Run(cs);
            var q7 = Task.Run(cs);
            var q8 = Task.Run(cs);
            var q9 = Task.Run(cs);

            await Task.WhenAll(q1, q2, q3, q4, q5, q6, q7, q8, q9);

            void cs()
            {
                while (watch.ElapsedMilliseconds < 20000)
                {
                    var slim = resetEventPool.Get();
                    Interlocked.Increment(ref g1);
                    if (slim.IsSet)
                    {

                    }
                    else
                    {

                    }

                    slim.Wait(0);
                    slim.Set();
                    slim.Reset();

                    resetEventPool.Return(slim);
                }
            }

            Console.WriteLine($"（自带版）累计创建数：{g1} - {watch.ElapsedMilliseconds}ms");
        }

        public static int a1, b1, c1 = 0;

        //[SupportedOSPlatform("windows")]
        internal static async Task OnMain(string[] args)
        {
            //await Cs0();
            //await Cs1();
            //await Cs2();
            //return;

            SpinWait spinWait = new();
            Stopwatch watch = Stopwatch.StartNew();
            bool aii = true;
            while (aii)
            {
                spinWait.SpinOnce();
                Debug.WriteLine(spinWait.Count);
                aii = !(spinWait.Count == 10);
            }

            Debug.WriteLine(watch.ElapsedMilliseconds);

            //ReceiveFrom3();
            //Thread.Sleep(1000);
            //SendTo4();

            //Console.ReadKey();

            //#if DEBUG
            //            var ipadrs = await Utility.GetIPAddressAsync();
            //#else
            var ipadrs = await Utility.GetIPAddressAsync("nixue.top");
            //#endif
            string ip = "127.0.0.1";
            if (ipadrs is not null) ip = ipadrs.ToString();

            //Task4();
            ArraySegment<byte> memory = new byte[10];
            memory[1] = 5;
            memory[2] = 115;
            memory[3] = 5;
            memory[4] = 115;
            memory[5] = 5;

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

            UdpServerAsync udpServer = new(NetBufferSize.Size32K, true) { Millisecond = 0, IsThreadPool = false };

            FileStream WriteStream = new("ToDesk1.exe", FileMode.OpenOrCreate, FileAccess.Write);

            SpinLock @lock = new(false);
            udpServer.SetReceived(async (a) =>
            {
                try
                {
                    using (a)
                    {
                        //bool lockTaken = false;
                        //@lock.TryEnter(ref lockTaken);
                        //while (!lockTaken) 
                        //{
                        //    @lock.TryEnter(ref lockTaken);
                        //}

                        a1.Increment();
                        //await udpServer.SendAsync(a.Client, memory);

                        //uint cc = a.UdpOrderCount;
                        //long ccc = cc * a.Length;
                        //Debug.WriteLine("当前索引：{0}，写入位：{1}", cc, ccc);

                        long position = BitConverter.ToInt64(a.Span);
                        var memory = a.Memory[8..];

                        WriteStream.Position = position;

                        await WriteStream.WriteAsync(memory);

                        //@lock.Exit(true);
                    }
                }
                catch (Exception)
                {

                }
            });

            udpServer.SetCompleted(async (a, b, c) =>
            {
                switch (b)
                {
                    case EnServer.Connect:
                        //await udpServer.SendAsync(a, $"打招呼{Interlocked.Increment(ref c1)} {DateTime.Now}");
                        break;
                    case EnServer.Fail:
                        Console.Clear();
                        break;
                    case EnServer.SendMsg:
                        break;
                    case EnServer.Receive:
                        //Interlocked.Increment(ref a1);
                        //await udpServer.SendAsync(a, $"打招呼{Interlocked.Increment(ref c1)} {DateTime.Now}");
                        break;
                    case EnServer.Close:
                        Console.Clear();
                        break;
                    case EnServer.HeartBeat:
                        break;
                }

                //Console.WriteLine($"Ip:{a} Server:{b} {DateTime.Now}");

                await Task.CompletedTask;
            });

            //await udpServer.StartAsync(ip, 8000);

            //Console.WriteLine($"已启动服务器！");
            //Console.ReadKey();
            //return;

            UdpClientAsync udpClient = new(NetBufferSize.Size32K, true) { Millisecond = 0, IsThreadPool = false };
            udpClient.SetReceived(async (a) =>
            {
                try
                {
                    using (a)
                    {
                        //await Console.Out.WriteLineAsync($"ip:{a.Key},{Encoding.UTF8.GetString(a.Span)},{DateTime.Now}");
                        //await Task.Delay(10);
                        b1.Increment();
                        //await udpClient.SendAsync(memory);

                        await Task.CompletedTask;
                    }
                }
                catch (Exception)
                {

                }
            });

            udpClient.SetCompleted(async (a, b, c) =>
            {
                switch (b)
                {
                    case EnClient.Connect:
                        //await udpClient.SendAsync($"打招呼{Interlocked.Increment(ref c1)} {DateTime.Now}");
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

                //Console.WriteLine($"Ip:{a} Client:{b} {DateTime.Now}");

                await Task.CompletedTask;
            });

            await udpClient.ConnectAsync(ip, 8000);
            udpClient.AddKeepAlive(1);

            //await Task.Delay(3000);

            //try
            //{
            //    await webClient.SendAsync($"打招呼{DateTime.Now}");
            //}
            //catch (Exception)
            //{

            //}

            //Console.ReadKey();

            //await webClient.SendAsync($"打招呼{Interlocked.Increment(ref c1)} {DateTime.Now}");

            FileStream ReadStream = new("ToDesk0.exe", FileMode.Open, FileAccess.Read);

            var sendBytes = udpClient.CreateSendBytes();
            while (ReadStream.Position != ReadStream.Length)
            {
                BitConverter.TryWriteBytes(sendBytes.Span[..8], ReadStream.Position);
                int con = await ReadStream.ReadAsync(sendBytes.Memory[8..]) + 8;

                //var a = sendBytes.Array;
                //if (sendBytes.Length > con) { }
                //await Task.Delay(10);
                await udpClient.SendAsync(sendBytes[..con]);
            }
            sendBytes.Dispose();

            while (Console.ReadKey(true).Key != ConsoleKey.F5) ;
            return;
        }
    }
}
