using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

//using BenchmarkDotNet.Toolchains.InProcess.NoEmit;
//using static System.Net.Mime.MediaTypeNames;
using System.IO;
//using BenchmarkDotNet.Attributes;
//using BenchmarkDotNet.Running;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Tool;
using Tool.Sockets.Kernels;
using Tool.Sockets.NetFrame;

namespace TcpFrameTest
{
    public class Program
    {
        static string ip = "127.0.0.1";

        static bool isCancell = false;

        static int i = 0;

        //BenchmarkDotNet
        //[Benchmark]
        public string abc()
        {
            Console.WriteLine(i++);
            //byte[] listData = new byte[50000];

            //byte[] headby = BitConverter.GetBytes(listData.Length);

            //byte[] Data = new byte[6 + listData.Length];

            //Data[0] = 40;
            //Data[1] = headby[0];
            //Data[2] = headby[1];
            //Data[3] = headby[2];
            //Data[4] = headby[3];
            //Data[5] = 41;

            //listData.CopyTo(Data, 6);

            //for (int i = 0; i < 5000000; i++)
            //{
            //    if (!TcpStateObject.IsIpPort("192.168.1.166:6514"))
            //    {

            //    }
            //}

            for (int i = 0; i < 500000; i++)
            {
                TestSystem system = new() { Id = 0, Key_cn = null, Key_en = null, Value = 0 };
                Abc abc = new() { Aid = 666, Bkey_en = "", Ckey_cn = "", Dvalue = "6666666666" };

                system.CopyEntity(abc, "Id=Aid", "Key_cn=Ckey_cn", "Value=Dvalue");
            }

            return "";
        }

        //[Benchmark]
        public string abc1()
        {
            for (int i = 0; i < 500000; i++)
            {
                TestSystem system = new() { Id = 0, Key_cn = null, Key_en = null, Value = 0 };
                Abc abc = new() { Aid = 666, Bkey_en = "", Ckey_cn = "", Dvalue = "6666666666" };

                system.Id = abc.Aid.ToVar<int>();
                system.Key_en = abc.Ckey_cn.ToVar<string>();
                system.Value = abc.Dvalue.ToVar<long>();
            }

            //Span<byte> listData = new byte[50000];

            //byte[] headby = BitConverter.GetBytes(listData.Length);

            //Span<byte> bytes = new byte[6 + listData.Length];

            //bytes[0] = 40;
            //bytes[1] = headby[0];
            //bytes[2] = headby[1];
            //bytes[3] = headby[2];
            //bytes[4] = headby[3];
            //bytes[5] = 41;

            //listData.CopyTo(bytes[6..]);

            //for (int i = 0; i < 500000; i++)
            //{
            //    if (!global::System.Net.IPEndPoint.TryParse("192.168.1.166:6514", out _))
            //    {

            //    }
            //}

            return "";
        }

        public static async Task Main(string[] args) //http://huodong2.4399.com/2022/zmwsezn/?pop_activity=1
        {
//#if !DEBUG
//            var ipadrs = await Tool.Utils.Utility.GetIPAddressAsync("nixue.top", System.Net.Sockets.AddressFamily.InterNetwork);
//            if (ipadrs is not null) ip = ipadrs.ToString();
//#endif

            await Console.Out.WriteLineAsync($"IP:{ip}");

            args = args.Length != 0 ? args : ["1"];
            //Dictionary<ushort, string> dict = new Dictionary<ushort, string>();
            //for (byte ClassID = 0; ClassID <= 255; ClassID++)
            //{
            //    for (byte ActionID = 0; ActionID <= 255; ActionID++)
            //    {
            //        var zhi = BitConverter.ToUInt16(new byte[] { ClassID, ActionID });
            //        var str = $"{ClassID}.{ActionID}";
            //        await Console.Out.WriteLineAsync($"有效数字：{zhi} {str}");
            //        dict.Add(zhi, str);
            //        if(ActionID == 255) break;
            //    }
            //    if (ClassID == 255) break;
            //}

            //Console.ReadKey();

            //Tool.Web.HttpContextExtension.IsIps("weqguyafyg", true);

            //TcpStateObject.IsIpPort("");

            //var ss = HttpHelpers.GetString("https://www.baidu.com", headers => headers.UserAgent.TryParseAdd("Mozilla/5.0 (Linux; Android 6.0; Nexus 5 Build/MRA58N) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Mobile Safari/537.36 Edg/108.0.1462.76"));

            //var ss1 = HttpHelpers.GetStringAsync("https://www.baidu.com", headers => headers.UserAgent.TryParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36 Edg/108.0.1462.76")).Result;

            //var ss2 = HttpHelpers.PostString("http://huodong2.4399.com/2022/zmwsezn/?pop_activity=1");

            //var ss3 = HttpHelpers.PostStringAsync("http://huodong2.4399.com/2022/zmwsezn/?pop_activity=1").Result;

            //Console.WriteLine("{0}\n\t{1}\n\t{2}\n\t{3}", ss, ss1, ss2, ss3);

            //string st = Console.ReadLine();
            //string val = "{ \"ok\":\""+ st +"\" }";
            //var json = val.JsonVar();

            //System system = new() { Id = 0, Key_cn = null, Key_en = null, Value = 0 };
            //Abc abc = new() { Aid = 666, Bkey_en = "", Ckey_cn = "", Dvalue = "6666666666" };

            //system.CopyEntity(abc, "Id=Aid", "Key_cn=Ckey_cn", "Value=Dvalue");

            //var summary = BenchmarkRunner.Run<Program>();
            //return;

            //Console.WriteLine(0);

            //for (int i = 0; i < 500000; i++)
            //{
            //    if (!TcpStateObject.IsIpPort("192.168.1.166:6514"))
            //    {

            //    }
            //}

            //Console.WriteLine(1);

            //for (int i = 0; i < 500000; i++)
            //{
            //    if (!System.Net.IPEndPoint.TryParse("192.168.1.166:6514", out _))
            //    {

            //    }
            //}

            //Console.WriteLine();

            //AppSettings.GetReloadToken();

            //Tool.SqlCore.PagerManager.GetFieldString(new string[] { "a", "b", "c" }, new string[] { "a1", "b1", "c1" });

            //ArraySegment<byte> ar = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };

            //var s = ar[2..4];

            //s[0] = 50;
            //s[1] = 100;

            //Console.WriteLine();

            //new Program().abc();

            //int c = 0;
            //for (int i = 0; i < 50; i++)
            //{
            //    //Thread.Sleep(200);
            //    Task.Run(() =>
            //    {
            //        for (int i = 0; i < 50_000; i++)
            //        {
            //            Log.Debug($"C:{Interlocked.Increment(ref c)}");
            //        }
            //    });
            //}

            ////TcpEventQueue.OnInterceptor(EnServer.SendMsg, true);
            ////TcpEventQueue.OnInterceptor(EnServer.Receive, true);
            ////TcpEventQueue.OnInterceptor(EnServer.SendMsg, false);
            ////TcpEventQueue.OnInterceptor(EnServer.Receive, false);

            ////var sbc = BenchmarkRunner.Run<Program>();

            //Console.ReadKey();
            //return;

            //Tool.SqlCore.DbHelper dbHelper = new("123465", Tool.SqlCore.DbProviderType.SqlServer, null);

            //Dictionary<string, object> map = new()
            //{
            //    { "1", 20 },
            //    { "2", DateTime.Now },
            //    { "3", 20 }
            //};

            //Hashtable hashtable = new()
            //{
            //    { "1", 20 },
            //    { "2", DateTime.Now },
            //    { "3", 20 }
            //};

            //var dd = new { a1 = 5, b2 = DateTime.Now };

            //dbHelper.SetDictionaryParam(map);

            //var ss = HttpHelpers.GetString("http://huodong2.4399.com/2022/zmwsezn/?pop_activity=1");

            //var ss1 = HttpHelpers.GetStringAsync("http://huodong2.4399.com/2022/zmwsezn/?pop_activity=1").Result;

            //var ss2 = HttpHelpers.PostString("http://huodong2.4399.com/2022/zmwsezn/?pop_activity=1");

            //var ss3 = HttpHelpers.PostStringAsync("http://huodong2.4399.com/2022/zmwsezn/?pop_activity=1").Result;

            //var q = "?a=1&b=6&c=aaa";

            //var q1 = HttpHelpers.FormatData(q);
            //var q2 = HttpHelpers.FormatData1(q);

            //TextTemplate1

            //ObjectExtension.Static.Add("ss", "sss");

            //HttpHelpers.Timeout = 3000;

            //var s1 = HttpHelpers.GetString("http://baidu.com");//("https://v1.hitokoto.cn/");
            //var s2 = HttpHelpers.GetString("http://baidu.com");//("https://v1.hitokoto.cn/");
            //var s3 = HttpHelpers.GetString("http://baidu.com");//("https://v1.hitokoto.cn/");
            //var s4 = HttpHelpers.GetString("http://baidu.com");//("https://v1.hitokoto.cn/");

            //var s5 = HttpHelpers.PostString("https://sdk-tj.img4399.com/playtime/collect.html", "action=APP_DID_BECOME_ACTIVE&device={\"DEVICE_IDENTIFIER\":\"\",\"SCREEN_RESOLUTION\":\"2340*1036\",\"DEVICE_MODEL\":\"Redmi K20 Pro\",\"DEVICE_MODEL_VERSION\":\"11\",\"SYSTEM_VERSION\":\"11\",\"PLATFORM_TYPE\":\"Android\",\"SDK_VERSION\":\"2.37.0.214\",\"GAME_KEY\":\"40025\",\"GAME_VERSION\":\"12.1.1\",\"BID\":\"org.yjmobile.zmxy\",\"IMSI\":\"\",\"PHONE\":\"\",\"RUNTIME\":\"Origin\",\"CANAL_IDENTIFIER\":\"\",\"UDID\":\"1100gihU8AkKanE4wnDVX6dac\",\"DEBUG\":\"false\",\"NETWORK_TYPE\":\"WIFI\",\"SERVER_SERIAL\":\"0\",\"UID\":\"266873866\"}");

            //var s6 = HttpHelpers.GetString("http://huodong2.4399.com/2022/zmwsezn/?pop_activity=1");

            //TaskOueue<int, bool> taskOueue = new((i) =>
            //{
            //    return i.IsWhether(2);
            //});

            //taskOueue.ContinueWith += (a, b, c) => { Console.WriteLine("I:{0},{1}", a, b); };

            //for (int i = 0; i < 1_000_000_000; i++)
            //{
            //    taskOueue.Add(i);
            //}

            //taskOueue.Add(Interlocked.Increment(ref c));

            //Thread.Sleep(10);

            //taskOueue.Add(Interlocked.Increment(ref c));

            //Thread.Sleep(20);

            //taskOueue.Add(Interlocked.Increment(ref c));

            //Thread.Sleep(30);

            //taskOueue.Add(Interlocked.Increment(ref c));

            //Thread.Sleep(40);

            //taskOueue.Add(Interlocked.Increment(ref c));

            //Thread.Sleep(50);

            //taskOueue.Add(Interlocked.Increment(ref c));

            //Thread.Sleep(2000);

            //for (int i = 0; i < 20; i++)
            //{
            //    //Thread.Sleep(200);
            //    Task.Run(() => 
            //    {
            //        for (int i = 0; i < 5_0000; i++)
            //        {
            //            ObjectExtension.Static.Add(i.ToString(), c);
            //            if (i.IsWhether(2)) Thread.Sleep(1);
            //            taskOueue.Add(Interlocked.Increment(ref c));
            //        }
            //    });
            //}

            //Console.ReadLine();

            //return;

            //for (int i = 0; i < 5; i++)
            //{
            //    //Thread.Sleep(200);
            //    Task.Run(() =>
            //    {
            //        for (int i = 0; i < 5_000; i++)
            //        {
            //            if (i.IsWhether(2)) Thread.Sleep(1);
            //            taskOueue.Add(Interlocked.Increment(ref c));
            //        }
            //    });
            //}

            //Console.ReadLine();

            //Console.BackgroundColor = ConsoleColor.Cyan;
            //Console.ForegroundColor = ConsoleColor.White;

            //TcpEventQueue.OnInterceptor(EnClient.SendMsg, true);
            //TcpEventQueue.OnInterceptor(EnClient.Receive, true);

            //ClientFrame client1 = new(TcpBufferSize.Default, 108, true);

            ////DataTcp.AddDataTcps(Assembly.LoadFrom(@"D:\Nixue工作室\Tool.Net\WebTestApp\bin\Debug\net5.0\WebTestApp.dll"));

            //client1.SetCompleted((a, b, c) =>
            //{
            //    //Console.WriteLine("\nIP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));

            //    if (b == EnClient.Connect) //|| b == Tool.Sockets.SupportCode.EnClient.Receive)
            //    {
            //        //Cstest();
            //        //var data = new ApiPacket(1, 102);
            //        //int c2 = ++c1;
            //        //data.Set("a", c2);
            //        //data.Set("path", StringExtension.GetGuid() + ".jpg");
            //        //data.Bytes = System.IO.File.ReadAllBytes(@"C:\Users\Administrator\Downloads\3cd107e4ec103f614b6f7f1eca9e18e6.jpeg");
            //        ////var mag = client.Send(data);
            //        ////Console.WriteLine("请求结果：{0},{1},{2} \t{3}", mag.Obj, mag.OnTcpFrame, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"), ++c1);

            //        ////client.SendAsync(data, (a1) => Console.WriteLine("请求结果：{0},{1},{2} \t{3}", a1.Obj, a1.OnTcpFrame, c.ToString("HH:mm:ss:fff"), c2));//, client.LocalPoint

            //        //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //        //client.SendAsync(data, (a1) => Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", a1.Obj, a1.OnTcpFrame, stopwatch.ElapsedMilliseconds, c2));
            //    }
            //});

            //client1.ConnectAsync("127.0.0.1", 444);//127.0.0.1

            //int c1 = 0;
            ////for (int i = 0; i < 1; i++)
            ////{
            //ClientFrame client = new(TcpBufferSize.Default, 108, true);

            //client.SetCompleted((a, b, c) =>
            //{
            //    Console.WriteLine("\nIP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));

            //    if (b == EnClient.Connect) //|| b == Tool.Sockets.SupportCode.EnClient.Receive)
            //    {
            //        //Cstest();
            //        //var data = new ApiPacket(1, 102);
            //        //int c2 = ++c1;
            //        //data.Set("a", c2);
            //        //data.Set("path", StringExtension.GetGuid() + ".jpg");
            //        //data.Bytes = System.IO.File.ReadAllBytes(@"C:\Users\Administrator\Downloads\3cd107e4ec103f614b6f7f1eca9e18e6.jpeg");
            //        ////var mag = client.Send(data);
            //        ////Console.WriteLine("请求结果：{0},{1},{2} \t{3}", mag.Obj, mag.OnTcpFrame, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"), ++c1);

            //        ////client.SendAsync(data, (a1) => Console.WriteLine("请求结果：{0},{1},{2} \t{3}", a1.Obj, a1.OnTcpFrame, c.ToString("HH:mm:ss:fff"), c2));//, client.LocalPoint

            //        //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //        //client.SendAsync(data, (a1) => Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", a1.Obj, a1.OnTcpFrame, stopwatch.ElapsedMilliseconds, c2));
            //    }
            //});

            //client.ConnectAsync("127.0.0.1", 444);//120.79.58.17 
            //client.AddKeepAlive(5);
            ////}

            //void Cstest()
            //{
            //    Task.Run(() =>
            //    {
            //        var data = new ApiPacket(1, 102);
            //        //data.Set("a", c2);
            //        data.Set("path", StringExtension.GetGuid() + ".jpg");

            //        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //        while (true)
            //        {
            //            int c2 = ++c1;
            //            stopwatch.Restart();

            //            //try
            //            //{
            //            //    data.Bytes = System.IO.File.ReadAllBytes("3cd107e4ec103f614b6f7f1eca9e18e6.jpeg");
            //            //}
            //            //catch (Exception)
            //            //{
            //            //    data.Bytes = new byte[] { 1, 0, 1 };
            //            //}

            //            data.Bytes = new byte[] { 1, 0, 1 };

            //            //client.SendAsync(data, (a1) =>
            //            //{
            //            //    Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", a1.Obj, a1.OnTcpFrame, stopwatch.ElapsedMilliseconds, c2);
            //            //});var mag = client.Send( data); //
            //            //var mag = client.SendIpIdea(client1.LocalPoint, data);
            //            var mag = client.Send(data);
            //            Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", mag.Text, mag.OnTcpFrame, stopwatch.Elapsed.TotalMilliseconds, c2);
            //            System.Threading.Thread.Sleep(1);
            //            //break;
            //        }
            //    });
            //}

            //var segment1 = new BufferSegment<int>([7, 8, 9]);
            //var segment2 = new BufferSegment<int>([4, 5, 6], segment1);
            //var segment3 = new BufferSegment<int>([1, 2, 3], segment2);
            //var index = 1;
            //foreach (var memory in new ReadOnlySequence<int>(segment3, 0, segment1, 3))
            //{
            //    var length = memory.Span.Length;
            //    for (var i = 0; i < length; i++)
            //    {
            //        Debug.Assert(memory.Span[i] == index++);
            //    }
            //}

            //MemoryWriteHeap memoryWrite = new(1024 * 1024 * 100);
            //memoryWrite.Dispose();

            //while (true)
            //{
            //    await Task.Delay(1000);
            //}

            EnumEventQueue.OnInterceptor(EnServer.SendMsg, true);
            EnumEventQueue.OnInterceptor(EnServer.Receive, true);

            EnumEventQueue.OnInterceptor(EnClient.SendMsg, true);
            EnumEventQueue.OnInterceptor(EnClient.Receive, true);

            bool isServer = true;
            //#if DEBUG
            if (Environment.CommandLine.EndsWith("TcpFrameTest.dll"))
            {
                await Console.Out.WriteLineAsync("请输入：0（Server）或1（Client）：");
                args[0] = Console.ReadKey(true).KeyChar.ToString();
            }
            //#endif
            if (string.Equals(args[0], "0"))
            {
                isServer = true;
            }
            else if (string.Equals(args[0], "1"))
            {
                isServer = false;
            }
            else
            {
                await Console.Out.WriteLineAsync("配置错误！");
                Console.ReadKey();
                return;
            }

            string? name = null;
            KeepAlive keep = new(1, async () =>
            {

                Console.Clear();//Process.GetProcesses
                Console.WriteLine("情况：{0}，{1}，{2} · {3}-{4}", ThreadPool.ThreadCount, ThreadPool.PendingWorkItemCount, ThreadPool.CompletedWorkItemCount, name, Environment.ProcessId);

                for (int i = 0; i < 20; i++)
                {
                    await Task.Delay(i);
                    Console.WriteLine("接收：总收 {0}，总发 {1}，总转 {2}", Class1.c, Class1.d, Class1.e);
                }
            });

            //Debug.WriteLine($"Thread:{Environment.CurrentManagedThreadId}");
            //"ss".GetHashCode();
            //Guid.NewGuid().GetHashCode();
            //var guid0 = new Guid(1, 1, 1, 1, 2, 3, 4, 5, 6, 7, 8);
            //var guid1 = new Guid(1, 1, 1, 1, 2, 3, 4, 5, 6, 7, 8);
            //if (guid0.GetHashCode() == guid1.GetHashCode())
            //{

            //}
            //var guid2 = new Guid(1, 1, 1, 8, 7, 6, 5, 4, 3, 2, 1);
            //if (guid0.GetHashCode() == guid2.GetHashCode())
            //{

            //}
            ////IEqualityComparer<>
            //global::System.Runtime.CompilerServices.RuntimeHelpers.GetHashCode(guid2);
            //ThreadPool.GetMaxThreads(out var workerThreads, out var maxThreads);
            //ThreadPool.GetMinThreads(out workerThreads, out var minThreads);
            //ThreadPool.GetAvailableThreads(out workerThreads, out var availableThreads);
            if (isServer)
            {
                name = "Server";
                await Server();
            }
            else
            {
                Task[] tasks = new Task[8];
                name = "Client";
                for (int i = 0; i < tasks.Length; i++)
                {
                    tasks[i] = ObjectExtension.RunTask(Client); //Task.Factory.StartNew(Client);
                }

                while (Console.ReadKey(true).Key is not ConsoleKey.F4) { }
                isCancell = true;
                Task.WaitAll(tasks);
            }

            //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //long cot = 0;
            //while (true)
            //{
            //    stopwatch.Restart();
            //    //Console.Write("输入参数：");
            //    //string str = Console.ReadLine();
            //    //if (string.IsNullOrWhiteSpace(str)) { Console.WriteLine(); continue; }
            //    var data = new ApiPacket(1, 100, 100);
            //    data.Set("a", cot++);

            //    //var mag = client1.Send(data);

            //    //client.SendAsync(data).ContinueWith(s =>
            //    //{
            //    //    var mag = s.Result;
            //    //    Console.WriteLine("\n请求结果：{0},{1} \t{2}ms", mag.Text, mag.OnTcpFrame, stopwatch.Elapsed.TotalMilliseconds);
            //    //});
            //    Console.WriteLine("请求结果：{0},{1} \t{2}ms", mag.Text, mag.OnTcpFrame, stopwatch.Elapsed.TotalMilliseconds);
            //    System.Threading.Thread.Sleep(1);
            //    if (cot == 10000) break;
            //}

            Console.ReadLine();
        }

        static int a1 = -1;

        private static async Task Server()
        {
            UserKey userKey = default;

            ServerFrame server = new(NetBufferSize.Size512K) { IsAllowRelay = false };

            Func<Task> func = async () =>
            {
                while (!isCancell)
                {
                    try
                    {
                        ApiPacket packet = new(1, 102, 60000, true);
                        using var netResponse = await server.SendAsync(userKey, packet);
                        switch (netResponse.State)
                        {
                            case not NetFrameState.Success:
                                Debug.WriteLine("访问状态：{0}，{1}", netResponse.State, netResponse.Error?.Message);
                                //await Task.Delay(1000);
                                break;
                        }
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            };

            server.SetIpParser(GetKey);

            server.SetCompleted((a, b, c) =>
            {
                if (b == EnServer.Connect)
                {
                    userKey = a;
                    //func.RunTask();
                }
                Debug.WriteLine("IP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
                return ValueTask.CompletedTask;
            });

            await server.StartAsync(ip, 444);

            Console.ReadLine();

            ValueTask<Ipv4Port> GetKey(Ipv4Port age0, Ipv4Port age1)
            {
                if (age0.IsEmpty)
                {
                    return ValueTask.FromResult(age1);
                }
                IEnumerable<UserKey> strings = server.ListClient.Keys;
                Interlocked.Increment(ref Class1.e); //统计总转发计数

                int Count = strings.Count();
                if (Count == 1) return ValueTask.FromResult(age0);

                int i = Interlocked.Increment(ref a1), j = 0;
                foreach (UserKey s in strings)
                {
                    if (s == (UserKey)age0) continue;
                    if (j == i) return ValueTask.FromResult(age0);
                    j++;
                }
                Interlocked.Exchange(ref a1, -1);
                return ValueTask.FromResult(age0);
            }
        }

        private static async Task Client()
        {
            ClientFrame client = new(NetBufferSize.Size512K, true) { IsThreadPool = true };

            client.SetCompleted((a1, b1, c1) =>
            {
                Debug.WriteLine("IP:{0} \t{1} \t{2}", a1, b1, c1.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
                return ValueTask.CompletedTask;
            });

            await client.ConnectAsync(ip, 444);//120.79.58.17 
            client.AddKeepAlive(5);

            //await Parallel.ForAsync(1, 6, task);
            await task(1, default);

            async Task task(int i, CancellationToken token)
            {
                ApiPacket packet;
                switch (i)
                {
                    case 0:
                        packet = new(1, 103, 10000, false);
                        packet.Set("path", "cs.png");
                        packet.Bytes = File.ReadAllBytes("D:\\NixueStudio\\Tool.Net\\TcpFrameTest\\Download\\cs.png");
                        break;
                    default:
                        packet = new(1, 102, 60000, true);
                        packet.Set("path", "cs.zip");
                        packet.Bytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
                        break;
                }

                //await Console.Out.WriteLineAsync($"计数{i}");
                while (!isCancell)
                {
                    //Debug.WriteLine($"Thread:{Environment.CurrentManagedThreadId}");

#if true

                    Interlocked.Increment(ref Class1.d);//模拟发送计数
                    using var response = await client.SendAsync(packet);
                    //if (!packet.IsReply)
                    //{
                    //    Interlocked.Increment(ref Class1.d);//模拟发送计数
                    //}
                    Interlocked.Increment(ref Class1.c);//模拟发送计数
#else
                    using var response = await client.SendIpIdeaAsync("1.0.0.1:1", packet);
#endif
                    //Debug.WriteLine($"Thread:{Environment.CurrentManagedThreadId}");

                    switch (response.State)
                    {
                        case not NetFrameState.Success:
                            Debug.WriteLine("访问状态：{0}，{1}", response.State, response.Error?.Message);
                            await Task.Delay(1000, token);
                            break;
                    }
                }
            }
        }
    }
}
