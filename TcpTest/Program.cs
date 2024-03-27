using Microsoft.Extensions.Logging;
using System;
using System.Buffers;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tool;
using Tool.Sockets.Kernels;
using Tool.Sockets.NetFrame;
using Tool.Utils;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;

namespace TcpTest
{
    public class Abc
    {
        public int? a { get; init; }
        public string? b { get; set; }
        public DateTime? c { get; init; } = DateTime.Now;
        public bool? d { get; set; }
        public byte? e { get; init; }
        public double? f { get; set; }
        public decimal? g { get; init; }
    }

    internal class Program
    {
        //private static async Task TaskAsync(ManualResetEvent @event) 
        //{
        //    Console.WriteLine("呜呜{0}", ObjectExtension.Thread.ManagedThreadId);
        //    await Task.Delay(1);
        //    @event.WaitOne(5 * 1000, true);
        //    Console.WriteLine("嘿嘿{0}", ObjectExtension.Thread.ManagedThreadId);
        //}

        static void Task4()
        {
            const string filePath = "D:\\NixueStudio\\Tool.Net\\TcpFrameTest\\bin\\Debug\\net7.0\\TcpFrameTest.exe";

            var task = Task.FromResult(File.ReadAllText(filePath)); // 只是方便举例，更好的代码应该是：File.ReadAllTextAsync(filePath); 

            Console.WriteLine("读取文件内容...");

            // 等待任务完成
            task.Wait();

            Console.WriteLine("文件内容: {0}", task.Result);
            Console.ReadKey();
        }

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

            //int c = 0;
            while (true)
            {
                Memory<byte> msg1 = new byte[256];
                IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
                EndPoint senderRemote = (EndPoint)sender;
                var i = await s.ReceiveFromAsync(msg1, SocketFlags.None, senderRemote);
                if (i.ReceivedBytes > 0)
                {
                    Console.WriteLine(Encoding.UTF8.GetString(msg1.ToArray(), 0, i.ReceivedBytes));
                }
                //c++;
            }

            s.Close();
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

            s.Close();
        }

        static int a1, b1, c1 = 0;

        static async Task Main(string[] args)
        {
            //Task4();

            Tool.Sockets.WebHelper.WebServer webServer = new() { Millisecond = 0, IsSSL = false };
            webServer.StartAsync("127.0.0.1", 9999);

            webServer.SetReceived(async (a) =>
            {
                try
                {
                    using (a)
                    {
                        await Console.Out.WriteLineAsync($"ip:{a.Key},{Encoding.UTF8.GetString(a.Memory.Span)},{DateTime.Now}");
                        await Task.Delay(10);
                        await webServer.SendAsync(a.Client, $"Server:已回应{Interlocked.Increment(ref a1)} {DateTime.Now}");
                    }
                }
                catch (Exception)
                {

                }
            });

            //Console.WriteLine($"已启动服务器！");
            //Console.ReadKey();
            //return;

            Tool.Sockets.WebHelper.WebClient webClient = new(true) { Millisecond = 0, IsSSL = false };
            webClient.ConnectAsync("127.0.0.1", 9999);

            webClient.SetReceived(async (a) =>
            {
                try
                {
                    using (a)
                    {
                        await Console.Out.WriteLineAsync($"ip:{a.Key},{Encoding.UTF8.GetString(a.Memory.Span)},{DateTime.Now}");
                        await Task.Delay(10);
                        await webClient.SendAsync($"Client:已回应{Interlocked.Increment(ref b1)} {DateTime.Now}");
                    }
                }
                catch (Exception)
                {

                }
            });

            webClient.SetCompleted(async (a, b, c) =>
            {
                switch (b)
                {
                    case EnClient.Connect:
                        await webClient.SendAsync($"打招呼{Interlocked.Increment(ref c1)} {DateTime.Now}");
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
            });

            await Task.Delay(3000);

            //try
            //{
            //    await webClient.SendAsync($"打招呼{DateTime.Now}");
            //}
            //catch (Exception)
            //{

            //}

            Console.ReadKey();
            return;

            //Memory<byte> obj = new byte[] { 1, 2, 3, 4, 5, 0, 0, 0, 0, 0 };

            //var ListData0 = obj[..5];
            //byte[] ListData1 = ListData0.ToArray();

            //ListData1[0] = 6;

            //obj.Span[0] = 0;

            ReceiveFrom3();
            Thread.Sleep(1000);
            SendTo4();

            Console.ReadKey();

            //var @event = new ManualResetEvent(false);CountdownEvent
            //CountdownEvent
            //_ = TaskAsync(@event);

            //Tool.Utils.ThreadQueue.AtomCountLock atomCountLock = new(5);

            //KeepAlive keep66 = new(1, () =>
            //{
            //    atomCountLock.Set();
            //});

            //for (int ii = 0; ii < 10; ii++)
            //{
            //    atomCountLock.Wait();
            //    Console.WriteLine(ii);
            //}

            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//Socket socket0
            //socket0.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 444));
            //socket0.Listen();
            //Socket socket = socket0.Accept();
            socket.Connect("127.0.0.1", 444);
            var _dataLength = 108 * 1024 - 6;
            Tool.Sockets.TcpHelper.TcpClient tcpClient = new(socket, _dataLength);

            Memory<byte> data = new byte[] { 40, 0, 0, 0, 0, 41, 123, 1, 2, 3, 2, 1, 123 };

            BitConverter.TryWriteBytes(data[1..].Span, 7);

            //while (true)
            //{
            //    var memoryOwner = MemoryPool<byte>.Shared.Rent(data.Length);
            //    data.CopyTo(memoryOwner.Memory);
            //    tcpClient.SendAsync(new TcpBytes("", socket, memoryOwner, data.Length));
            //    Thread.Sleep(Random.Shared.Next(0, 2));
            //}
            int lre = int.Parse("100000");
            KeepAlive keep = new(1, async () =>
            {
                for (int i = 0; i < lre; i++)
                {
                    var memoryOwner = MemoryPool<byte>.Shared.Rent(data.Length);
                    data.CopyTo(memoryOwner.Memory);
                    tcpClient.SendAsync(new SendBytes(memoryOwner, data.Length));
                    await Task.Delay(i);
                    //Thread.Sleep(Random.Shared.Next(0, 2));
                }
                //Thread.Sleep(100000);
            });

            Console.ReadKey();

            //abc abc1 = new();
            //var q1 = abc1.ToXml();
            //var q2 = q1.Xml<abc>();

            //var aa = new { a = 66, b = "ok" };

            //__reftype(T);__makeref();__refvalue();__arglist("sb",1024);TypedReference

            //var bb = __makeref(aa);
            //var cc = __refvalue(bb, int);

            //abc abc = new();

            //var aa = new { a = 66, b = "ok" };

            //var ab= new { a = 77, b = "jj" };

            //var aaa = EntityBuilder.GetEntity(ab.GetType());

            //var aaa1 = aaa.New;

            //var aaaaa = TypeDescriptor.GetProperties(abc);

            //var s1 = new ClassFieldDispatcher(aa.GetType(), ClassField.All); //ClassFieldDispatcher.GetClassFields(aa.GetType());

            //var aaa = s1.Get(aa);
            //s1.Set(aa, new Dictionary<string, object> { { "b", "jj" } });

            //var aa1 = s1.Get(ab);

            //var ass = null as object;

            var type = typeof(Abc);

            //DataTable dataTable = new();

            //dataTable.Columns.Add("a", typeof(int));
            //dataTable.Columns.Add("B", typeof(string));
            //dataTable.Columns.Add("c", typeof(DateTime));
            //dataTable.Columns.Add("D", typeof(long));
            //dataTable.Columns.Add("e", typeof(byte));
            //dataTable.Columns.Add("F", typeof(double));
            //dataTable.Columns.Add("g", typeof(decimal));
            //byte e1 = 0;
            //for (int q = 0; q < 100000; q++)
            //{
            //    DataRow dr = dataTable.NewRow();

            //    dr[0] = q;
            //    dr[1] = q.ToString();
            //    dr[2] = DateTime.Now.AddMinutes(q);
            //    dr[3] = q.IsWhether(2) ? q : DBNull.Value;
            //    dr[4] = unchecked(e1++);
            //    dr[5] = q / 10.0;
            //    dr[6] = q / 100m;

            //    dataTable.Rows.Add(dr);
            //}

            //var 啊1111111 = dataTable.ToEntity<abc>(5);

            //var 啊11111111 = dataTable.ToEntityList<abc>(5);

            //var aaaaaa = dataTable.ToEntityList<abc>();

            //var aaaaaa1 = DataHelper.ConvertDataTableToObjects<abc>(dataTable);

            ////new ClassFieldDispatcher(type);


            ////abc.c = DateTime.Now;
            ////Hashtable hashtable = new();
            ////hashtable.Add("a", 4446);
            ////hashtable.Add("b", "66666");
            ////hashtable.Add("c", DateTime.Now);
            ////hashtable.Add("d", true);
            ////hashtable.Add("e", (byte)66);

            ////hashtable["a"] = i;
            ////hashtable["b"] = "123";
            ////hashtable["c"] = DateTime.Now;
            ////hashtable["d"] = true;
            ////hashtable["e"] = (byte)100;

            int i = 0, i1 = 0;

            var a2 = EntityBuilder.GetEntity(type);

            var abc = a2.New;

            for (; i < 1000000; i++)
            {
                IDictionary<string, object> hashtable = new Dictionary<string, object>(5, StringComparer.OrdinalIgnoreCase);
                hashtable.Add("a", i);
                hashtable.Add("B", i.ToString());
                hashtable.Add("c", DateTime.Now);
                hashtable.Add("D", i.IsWhether(2));
                hashtable.Add("e", (byte)66);

                //hashtable.TryGetValue("e", out object v);

                a2.Set(abc, hashtable);
                //s1(abc, hashtable);
            }

            Console.WriteLine();

            //for (; i1 < 1000000; i1++)
            //{
            //    var a2 = EntityBuilder.GetEntity(type);
            //    var abc = a2.New;
            //    var sa = a2.Get(abc);
            //}

            //Console.WriteLine();

            //for (; i >= 0; i--)
            //{
            //    var abc = Activator.CreateInstance(type);
            //    PropertyInfo[] properties = type.GetProperties();
            //    foreach (PropertyInfo property in properties)
            //    {
            //        if (property.Name == "a")
            //        {
            //            property.SetValue(abc, i);
            //        }
            //        if (property.Name == "b")
            //        {
            //            property.SetValue(abc, i.ToString());
            //        }
            //        if (property.Name == "c")
            //        {
            //            property.SetValue(abc, DateTime.Now);
            //        }
            //        if (property.Name == "d")
            //        {
            //            property.SetValue(abc, true);
            //        }
            //        if (property.Name == "e")
            //        {
            //            property.SetValue(abc, (byte)250);
            //        }
            //    }
            //}

            //Console.WriteLine();

            //for (; i1 >= 0; i1--)
            //{
            //    var abc = Activator.CreateInstance(type);
            //    //IDictionary<string, object> hashtable = new Dictionary<string, object>(6);
            //    //PropertyInfo[] properties = type.GetProperties();
            //    //foreach (PropertyInfo property in properties)
            //    //{
            //    //    hashtable.Add(property.Name, property.GetValue(abc) ?? throw new());
            //    //}

            //    abc.ToDictionary();
            //}

            //Console.WriteLine();

            //for (; i1 < 1000000; i1++)
            //{
            //    var abc = Activator.CreateInstance(type);
            //    //IDictionary<string, object> hashtable = new Dictionary<string, object>(6);
            //    //PropertyInfo[] properties = type.GetProperties();
            //    //foreach (PropertyInfo property in properties)
            //    //{
            //    //    hashtable.Add(property.Name, property.GetValue(abc) ?? throw new());
            //    //}

            //    abc.GetDictionary();
            //}

            //Console.WriteLine();

            //for (; i < 1000000; i++)
            //{
            //    var abc = Activator.CreateInstance(type);

            //    IDictionary<string, object> hashtable = new Dictionary<string, object>(5);
            //    hashtable.Add("a", i);
            //    hashtable.Add("b", i.ToString());
            //    hashtable.Add("c", DateTime.Now);
            //    hashtable.Add("d", true);
            //    hashtable.Add("e", (byte)66);

            //    //hashtable.TryGetValue("e", out object v);

            //    abc.SetDictionary(hashtable);
            //    //s1(abc, hashtable);
            //}

            //Console.WriteLine();
            //try
            //{
            //    try
            //    {
            //        throw new Exception("呜呜呜");
            //    }
            //    catch
            //    {
            //        throw;
            //    }
            //    finally
            //    {
            //        Console.WriteLine("123456");
            //    }
            //}
            //catch (Exception)
            //{
            //}

            //ApiPacket.TcpAsync = false;

            //for (int i = 0; i < 1000; i++)
            //{
            //   using var as1 = MemoryPool<byte>.Shared.Rent(108 * 1024);
            //}
            ////MemoryPool<byte>.Shared.Dispose();
            //ArraySegment<byte> ar = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            //var s66 = ar[2..4];

            //s66[0] = 50;
            //s66[1] = 100;

            //UInt64 a66 = UInt64.MaxValue;
            //a66 = unchecked(a66 + 1);

            //Memory<byte> memory = new byte[] { 5, 4, 3, 2, 1, 0, 1, 2, 3, 4, 5 };

            //var a1 = memory[5..];
            //var a3 = new byte[10];
            //var a2 = memory[5..].TryCopyTo(a3);

            //memory.Span[5..].Clear();
            //int w = 1000, h = 1000;
            //ThreadPool.SetMaxThreads(w, h);

            ulong a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, s = 0;

            Console.WriteLine("Hello, World!");

            KeepAlive keepok = new(1,async () =>
            {
                Console.Clear();
                Console.WriteLine("情况：{0}，{1}，{2}", ThreadPool.ThreadCount, ThreadPool.PendingWorkItemCount, ThreadPool.CompletedWorkItemCount);
                for (int i = 0; i < 20; i++)
                {
                    await Task.Delay(i);
                    Console.WriteLine("发起：总 {0},ok {1},断 {2},时 {3},错 {4},无 {5},Id {6},对 {7}", a, d, e, f, g, b, c, s);
                }
            });

            //ClientFrameList clientFrameList = new(10);

            //for (int i = 0; i < 10; i++)
            //{
            ClientFrame client = new(TcpBufferSize.Default, 108, true);

            client.SetCompleted((a1, b1, c1) =>
            {
                if ((EnClient)b1 == EnClient.Connect)
                {
                    a = d = e = f = g = b = c = s = 0;
                }
                Console.WriteLine("\nIP:{0} \t{1} \t{2}", a1, b1, c1.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
                return Task.CompletedTask;
            });

            client.ConnectAsync("127.0.0.1", 444);//120.79.58.17 
            client.AddKeepAlive(5);

            //    clientFrameList.AddClientFrame(client);
            //}

            //clientFrameList.Completed += ClientFrameList_Completed;

            EnumEventQueue.OnInterceptor(EnClient.SendMsg, true);
            EnumEventQueue.OnInterceptor(EnClient.Receive, true);

            //Task.Run(() =>
            //{
            //    while (true)
            //    {
            //        var hh = Random.Shared.Next(1, 5);
            //        Thread.Sleep(hh);

            //        for (int i = 0; i < Random.Shared.Next(1, 20); i++)
            //        {
            //            Interlocked.Increment(ref a);
            //            ThreadPool.UnsafeQueueUserWorkItem(get, default);
            //        }
            //        //if (a == 3)
            //        //{
            //        //    break;
            //        //}
            //    };
            //});

            Thread.Sleep(1000);

            //ApiPacket packet = new(1, 101, 10000);
            //packet.Set("path", "123456");
            //var mag = client.SendIpIdea("192.168.1.22:9999", packet);

            switch (Console.ReadKey(true).KeyChar)
            {
                case '0':
                    Parallel.For(0, 2, cout =>
                    {
                        while (true)
                        {
                            Thread.Sleep(Random.Shared.Next(1, 5));

                            string url = cout == 0 ? "3cd107e4ec103f614b6f7f1eca9e18e6.jpeg" : "1f94a936494a49b6b2fbcadecd4ca16c.jpeg";
                            Interlocked.Increment(ref a);

                            //ApiPacket packet = new(1, 101, 10000);
                            //packet.Set("path", url);

                            ApiPacket packet = new(1, 102, 10000);
                            packet.Set("path", url);
                            packet.Bytes = File.ReadAllBytes(url);

                            var mag = client.Send(packet);

                            switch (mag.OnNetFrame)
                            {
                                case NetFrameState.Default:
                                    Interlocked.Increment(ref b);
                                    break;
                                case NetFrameState.OnlyID:
                                    Interlocked.Increment(ref c);
                                    break;
                                case NetFrameState.Success:
                                    Interlocked.Increment(ref d);
                                    try
                                    {
                                        //if (!File.Exists(url)) File.WriteAllBytes(url, mag.Bytes.Array ?? throw new());
                                        if (mag.Text == "Ok")
                                            Interlocked.Increment(ref s);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    break;
                                case NetFrameState.SendFail:
                                    Interlocked.Increment(ref e);
                                    break;
                                case NetFrameState.Timeout:
                                    Interlocked.Increment(ref f);
                                    break;
                                case NetFrameState.Exception:
                                    Interlocked.Increment(ref g);
                                    break;
                                default:
                                    break;
                            }
                        }
                    });
                    break;
                default:
                    //for (int i = 0; i < 40; i++)
                    //{
                    //    Task.Run(() =>
                    //    {
                    //        while (true)
                    //        {
                    //            var hh = Random.Shared.Next(1, 5);
                    //            Thread.Sleep(hh);

                    //            for (int i = 0; i < Random.Shared.Next(1, 20); i++)
                    //            {
                    //                Interlocked.Increment(ref a);
                    //                //ThreadPool.UnsafeQueueUserWorkItem(get, default);
                    //                get(default);
                    //            }
                    //            //if (a == 3)
                    //            //{
                    //            //    break;
                    //            //}
                    //        };
                    //    });
                    //}

                    Parallel.For(0, 40, cout =>
                    {
                        while (true)
                        {
                            var hh = Random.Shared.Next(1, 5);
                            Thread.Sleep(hh);

                            for (int i = 0; i < Random.Shared.Next(1, 20); i++)
                            {
                                Interlocked.Increment(ref a);
                                //ThreadPool.UnsafeQueueUserWorkItem(get, default);
                                get(default);
                            }
                            //if (a == 3)
                            //{
                            //    break;
                            //}
                        };
                    });
                    break;
            }



            //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //while (true)
            //{
            //    int c2 = ++a;
            //    stopwatch.Restart();
            //    ApiPacket packet = new(1, 101, 10000);
            //    packet.Set("path", "3cd107e4ec103f614b6f7f1eca9e18e6.jpeg");
            //    //try
            //    //{
            //    //    data.Bytes = System.IO.File.ReadAllBytes("3cd107e4ec103f614b6f7f1eca9e18e6.jpeg");
            //    //}
            //    //catch (Exception)
            //    //{
            //    //    data.Bytes = new byte[] { 1, 0, 1 };
            //    //}

            //    var mag = client.Send(packet);

            //    try
            //    {
            //        System.IO.File.WriteAllBytes("3cd107e4ec103f614b6f7f1eca9e18e6.jpeg", mag.Bytes);
            //    }
            //    catch (Exception)
            //    {
            //    }

            //    Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", mag.Text, mag.OnTcpFrame, stopwatch.Elapsed.TotalMilliseconds, c2);
            //    Thread.Sleep(1);
            //}

            void get(object? a)
            {
                var guid = StringExtension.GetGuid();
                ApiPacket packet = new(1, 250, 10000);
                packet.Set("a", guid);
                packet.Set("b", null);
                packet.Bytes = new byte[] { 1, 2, 3, 4 };
                var response = client.Send(packet);// clientFrameList.Send(packet, out _);
                //a.Response.StatusCode = 200;
                //a.Response.Write(s.Text);
                //if (s.OnTcpFrame != TcpFrameState.Success)
                //{
                //    Console.WriteLine("失败->{0}", s.OnTcpFrame.ToString());
                //}

                switch (response.OnNetFrame)
                {
                    case NetFrameState.Default:
                        Interlocked.Increment(ref b);
                        break;
                    case NetFrameState.OnlyID:
                        Interlocked.Increment(ref c);
                        break;
                    case NetFrameState.Success:
                        Interlocked.Increment(ref d);
                        if (guid == response.Text)
                        {
                            Interlocked.Increment(ref s);
                        }
                        break;
                    case NetFrameState.SendFail:
                        Interlocked.Increment(ref e);
                        break;
                    case NetFrameState.Timeout:
                        Interlocked.Increment(ref f);
                        break;
                    case NetFrameState.Exception:
                        Interlocked.Increment(ref g);
                        break;
                    default:
                        break;
                }
            }

            while (Console.ReadKey(true).KeyChar != '0') ;
        }
    }
}