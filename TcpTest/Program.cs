using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Win32;
using System.Buffers;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reflection.PortableExecutable;
using System.Runtime.Versioning;
using System.Text;
using Tool;
using Tool.Sockets.Kernels;
using Tool.Sockets.NetFrame;
using Tool.Sockets.P2PHelpr;
using Tool.Sockets.TcpHelper;
using Tool.Sockets.UdpHelper;

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

    [SupportedOSPlatform("windows")]
    [RequiresPreviewFeatures]
    //CountdownEvent
    internal class Program
    {
        #region 暂无用
        //private static async Task TaskAsync(ManualResetEvent @event) 
        //{
        //    Console.WriteLine("呜呜{0}", ObjectExtension.Thread.ManagedThreadId);
        //    await Task.Delay(1);
        //    @event.WaitOne(5 * 1000, true);
        //    Console.WriteLine("嘿嘿{0}", ObjectExtension.Thread.ManagedThreadId);
        //}
        #endregion

        static async ValueTask Completed<T>(string key, UserKey a1, T b1, DateTime c1) where T : Enum
        {
            await Console.Out.WriteLineAsync($"[{key}]IP:{a1} \t{b1} \t{c1:yyyy/MM/dd HH:mm:ss:fffffff}");
        }

        static async Task PipeAsync()
        {
            Pipe pipe = new(new PipeOptions(minimumSegmentSize: 1472, pauseWriterThreshold: 1472 * 100, resumeWriterThreshold: 1472 * 50));
            async void a1()
            {
                Memory<byte> memory = new byte[] { 6, 5, 4, 3, 2, 1, 0 };
                for (int i = 0; i < 100000; i++)
                {
                    memory.CopyTo(pipe.Writer.GetMemory(memory.Length));
                    pipe.Writer.Advance(memory.Length);
                    await pipe.Writer.FlushAsync();
                    //await pipe.Writer.WriteAsync(memory);
                }
            }
            async void a2()
            {
                var reader = pipe.Reader;
                int position = 0;//默认位
                while (true)//running
                {
                    //等待writer写数据
                    ReadResult result = await reader.ReadAsync();
                    //获得内存区域
                    ReadOnlySequence<byte> buffer = result.Buffer;

                    SequencePosition sequence = position > 0 ? buffer.GetPosition(position) : buffer.Start;
                    while (buffer.TryGet(ref sequence, out var memory) && memory.IsEmpty is false)
                    {
                        StringBuilder builder = new();
                        for (int i = 0; i < memory.Length; i++)
                        {
                            builder.Append($"{memory.Span[i]} ");
                        }
                        Debug.WriteLine($"验证发出数据：{builder}");
                        //还未验证是否收到 ACK 消息
                        sequence = buffer.GetPosition(position + memory.Length); //buffer.End;
                        position += memory.Length;
                    }

                    //数据处理完毕，告诉pipe还剩下多少数据没有处理（数据包不完整的数据，找不到head）
                    reader.AdvanceTo(buffer.Start, buffer.End);

                    if (result.IsCompleted) break;
                }

                await reader.CompleteAsync();
            }

            a2();
            a1();

            await Task.Delay(100000);
        }

        static void chaxun()
        {
            const string name = "1477//";

            List<(RegistryKey, string)> names = new List<(RegistryKey, string)>();
            List<(RegistryKey, string)> values = new List<(RegistryKey, string)>();

            RegistryKey[] keys = { Registry.LocalMachine, Registry.Users, Registry.ClassesRoot, Registry.CurrentConfig, Registry.CurrentUser };// Registry.ClassesRoot, Registry.CurrentConfig
            foreach (RegistryKey key in keys)
            {
                string[] subkeys = key.GetSubKeyNames();
                Queue<String> al = new Queue<String>(subkeys);
                Queue<RegistryKey> qu = new Queue<RegistryKey>();
                for (int i = 0; i < subkeys.Length; i++)
                    qu.Enqueue(key);
                while (al.Count > 0)
                {
                    string[] subkeyNames;
                    string[] subvalueNames;
                    try
                    {
                        RegistryKey aimdir = qu.Dequeue();
                        aimdir = aimdir.OpenSubKey(al.Dequeue(), true);
                        subvalueNames = aimdir.GetValueNames();
                        foreach (string valueName in subvalueNames)
                        {
                            if (valueName.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1) //%%1
                            {
                                //aimdir.DeleteValue(valueName, false);

                                names.Add((aimdir, valueName));
                            }
                            else
                            {
                                string value = aimdir.GetValue(valueName) as string;
                                if (value != null)
                                {
                                    if (value.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1)
                                    {
                                        //aimdir.DeleteValue(value, false);
                                        names.Add((aimdir, valueName));
                                    }
                                }
                            }
                        }
                        subkeyNames = aimdir.GetSubKeyNames();
                        foreach (string keyName in subkeyNames)
                        {
                            if (keyName.IndexOf(name, StringComparison.OrdinalIgnoreCase) != -1)
                            {
                                //aimdir.DeleteSubKey(keyName, false);
                                values.Add((aimdir, keyName));
                            }
                            al.Enqueue(keyName);
                            qu.Enqueue(aimdir);
                        }
                    }
                    catch (Exception) { }
                }
                key.Close();
            }
        }

        static async Task Main(string[] args)
        {
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

            uint i0 = uint.MaxValue, i1 = uint.MaxValue;
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

            UdpServerAsync serverAsync = new(NetBufferSize.Size256K, true) { Millisecond = 0 /*ReceiveTimeout = 5000*/ };
            serverAsync.SetCompleted((age0, age1, age2) => age1 != EnServer.SendMsg ? Completed($"UDPServer{i0.Increment()}", age0, age1, age2) : ValueTask.CompletedTask);
            serverAsync.SetReceived(async a =>
            {
                //await Console.Out.WriteLineAsync($"当前位：{a.OrderCount()} 原子计数：{i0.Increment()}");
                if (a.Length > 2)
                {
                    await serverAsync.SendAsync(a.Client, "ok");
                }
                a.Dispose();
            });
            await serverAsync.StartAsync(12344);

            //for (int i = 0; i < 1; i++)
            //{
            //    await serverAsync.SendAsync("Hello Several .NET Aspire templates include ASP.NET Core projects that are configured to use HTTPS by default. If this is the first time you're running the project, and you're using Visual Studio, you're prompted to install a localhost certificate.\r\n\r\nThere are situations in which you trust/install the development certificate, but you don't close all your browser windows. In these cases, your browser might indicate that the certificate isn't trusted.\r\n\r\nThere are also situations where you don't trust the certificate at all. In these cases, your browser might indicate that the certificate isn't trusted.\r\n\r\nAdditionally, there are warning messages from Kestrel written to the console that indicate that the certificate is not trusted.");
            //}

            Console.ReadKey();

            UdpClientAsync clientAsync = new(NetBufferSize.Size256K, true) { Millisecond = 0 /*ReceiveTimeout = 5000*/ };
            clientAsync.SetCompleted((age0, age1, age2) => age1 != EnClient.SendMsg ? Completed($"UDPClient{i1.Increment()}", age0, age1, age2) : ValueTask.CompletedTask);
            clientAsync.SetReceived(async a =>
            {
                //await Console.Out.WriteLineAsync($"当前位：{a.OrderCount()} 原子计数：{i0.Increment()}");
                if (a.Length > 2)
                {
                    await clientAsync.SendAsync("ok");
                }
                a.Dispose();
            });
            await clientAsync.ConnectAsync(12344);

            for (int i = 0; i < 1000; i++)
            {
                await clientAsync.SendAsync(
                    @"Hello Several .NET Aspire templates include ASP.NET Core projects that are configured to use HTTPS by default. 
                    If this is the first time you're running the project, and you're using Visual Studio, you're prompted to install a
                     localhost certificate.\r\n\r\nThere are situations in which you trust/install the development certificate, but you 
                    don't close all your browser windows. In these cases, your browser might indicate that the certificate isn't trusted.
                    \r\n\r\nThere are also situations where you don't trust the certificate at all. In these cases, your browser might indicate 
                    that the certificate isn't trusted.\r\n\r\nAdditionally, there are warning messages from Kestrel written to the console that 
                    indicate that the certificate is not trusted.

                    实例 Stopwatch 可以测量一个间隔的已用时间，或跨多个间隔的总已用时间。 在典型Stopwatch方案中，
                    调用 Start 方法，最终调用 Stop 方法，然后使用 属性检查运行时间Elapsed。
                    
                    Stopwatch实例正在运行或已停止;使用 IsRunning 确定 的Stopwatch当前状态。 使用 Start 开始测量已用时间;使用 Stop 停止测量已用时间。 
                    通过属性 Elapsed、 ElapsedMilliseconds或 ElapsedTicks查询已用时间值。 可以在实例正在运行或停止时查询已用时间属性。 运行时间属性在 Stopwatch 运行时稳步增加;当实例停止时，它们保持不变。
                    
                    默认情况下，实例的已用时间值 Stopwatch 等于所有测量时间间隔的总和。 对 的每个调用 Start 在累积运行时间开始计数;
                    对 的每次调用 Stop 将结束当前间隔度量并冻结累积已用时间值。 Reset使用 方法清除现有Stopwatch实例中的累积运行时间。
                    
                    通过 Stopwatch 对基础计时器机制中的计时器计时周期进行计数来测量运行时间。 如果安装的硬件和操作系统支持高分辨率性能计数器，
                    则 Stopwatch 类使用该计数器来测量运行时间。 否则， Stopwatch 类使用系统计时器来测量已用时间。 Frequency使用 和 IsHighResolution 字段确定计时实现的Stopwatch精度和分辨率。
                    
                    类 Stopwatch 有助于在托管代码中操作与计时相关的性能计数器。 具体而言， Frequency 字段和 GetTimestamp 方法可用于代替非托管 
                    Windows API QueryPerformanceFrequency 和 QueryPerformanceCounter。");
            }

            Console.ReadKey();
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

            await Console.Out.WriteLineAsync($"尝试P2P：{DateTime.Now:yyyy/MM/dd HH:mm:ss:fffffff}");

#if false
            P2pServerAsync p2PServerAsync0 = await P2pServerAsync.GetFreeTcp();
            P2pServerAsync p2PServerAsync1 = await P2pServerAsync.GetFreeTcp();

            TcpClientAsync p2PClientAsync0 = new(NetBufferSize.Default, true);
            p2PClientAsync0.SetCompleted((age0, age1, age2) => Completed("TCP", age0, age1, age2));
            var task0 = p2PServerAsync0.P2PConnectAsync(p2PClientAsync0, p2PServerAsync1.RemoteEP);
            //var task0 = p2PClientAsync0.P2PConnectAsync(p2PServerAsync0.LocalEP, p2PServerAsync1.RemoteEP);

            await Task.Delay(2000);

            TcpClientAsync p2PClientAsync1 = new(NetBufferSize.Default, true);
            p2PClientAsync1.SetCompleted((age0, age1, age2) => Completed("TCP", age0, age1, age2));
            var task1 = p2PServerAsync1.P2PConnectAsync(p2PClientAsync1, p2PServerAsync0.RemoteEP);
            //var task1 = p2PClientAsync1.P2PConnectAsync(p2PServerAsync1.LocalEP, p2PServerAsync0.RemoteEP);

#else
            P2pServerAsync p2PServerAsync0 = await P2pServerAsync.GetFreeUdp();
            P2pServerAsync p2PServerAsync1 = await P2pServerAsync.GetFreeUdp();

            UdpClientAsync p2PClientAsync0 = new(NetBufferSize.Default, true) { ReceiveTimeout = 5000 };
            p2PClientAsync0.SetCompleted((age0, age1, age2) => Completed("UDP", age0, age1, age2));
            var task0 = p2PServerAsync0.P2PConnectAsync(p2PClientAsync0, p2PServerAsync1.RemoteEP, 100000);
            //var task0 = p2PClientAsync0.P2PConnectAsync(p2PServerAsync0.LocalEP, p2PServerAsync1.RemoteEP);

            await Task.Delay(1000);

            UdpClientAsync p2PClientAsync1 = new(NetBufferSize.Default, true) { ReceiveTimeout = 5000 };
            p2PClientAsync1.SetCompleted((age0, age1, age2) => Completed("UDP", age0, age1, age2));
            var task1 = p2PServerAsync1.P2PConnectAsync(p2PClientAsync1, p2PServerAsync0.RemoteEP, 100000);
            //var task1 = p2PClientAsync1.P2PConnectAsync(p2PServerAsync1.LocalEP, p2PServerAsync0.RemoteEP);
#endif

            Task.WaitAll(task0, task1);
            await Console.Out.WriteLineAsync($"成功P2P：{DateTime.Now:yyyy/MM/dd HH:mm:ss:fffffff}");

            p2PServerAsync0.Dispose();
            p2PServerAsync1.Dispose();

            p2PClientAsync0.AddKeepAlive(1);
            p2PClientAsync1.AddKeepAlive(1);

            Console.ReadKey();
            p2PClientAsync0.Dispose();
            p2PClientAsync1.Dispose();

            UserKey userKey = "1.1.1.1:65535";

            Ipv4Port ipv4Port = userKey;

            ipv4Port.GetHashCode();

            StateObject.IsIpPort("120.255.1.1:1", out var ipnum);

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

            //await NetWorship.OnMain(args);
            //await TcpWorship.OnMain(args);
            //await UdpWorship.OnMain(args);
            //await WebWorship.OnMain(args);
            await QuicWorship.OnMain(args);

            #region 暂无用

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

            //var type = typeof(Abc);

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

            //int i = 0, i1 = 0;

            //var a2 = EntityBuilder.GetEntity(type);

            //var abc = a2.New;

            //for (; i < 1000000; i++)
            //{
            //    IDictionary<string, object> hashtable = new Dictionary<string, object>(5, StringComparer.OrdinalIgnoreCase);
            //    hashtable.Add("a", i);
            //    hashtable.Add("B", i.ToString());
            //    hashtable.Add("c", DateTime.Now);
            //    hashtable.Add("D", i.IsWhether(2));
            //    hashtable.Add("e", (byte)66);

            //    //hashtable.TryGetValue("e", out object v);

            //    a2.Set(abc, hashtable);
            //    //s1(abc, hashtable);
            //}

            //Console.WriteLine();

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

            #endregion

            while (Console.ReadKey(true).KeyChar != '0') ;
        }
    }
}