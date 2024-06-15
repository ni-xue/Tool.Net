using System.Runtime.Versioning;
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

        static async ValueTask Completed(string key, UserKey a1, EnClient b1, DateTime c1)
        {
            await Console.Out.WriteLineAsync($"[{key}]IP:{a1} \t{b1} \t{c1:yyyy/MM/dd HH:mm:ss:fffffff}");
        }

        static async Task Main(string[] args)
        {
            P2pClientAsync.TimedDelay = 60000;

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

            await Task.Delay(5000);

            TcpClientAsync p2PClientAsync1 = new(NetBufferSize.Default, true);
            p2PClientAsync1.SetCompleted((age0, age1, age2) => Completed("TCP", age0, age1, age2));
            var task1 = p2PServerAsync1.P2PConnectAsync(p2PClientAsync1, p2PServerAsync0.RemoteEP);
            //var task1 = p2PClientAsync1.P2PConnectAsync(p2PServerAsync1.LocalEP, p2PServerAsync0.RemoteEP);

#else
            P2pServerAsync p2PServerAsync0 = await P2pServerAsync.GetFreeUdp();
            P2pServerAsync p2PServerAsync1 = await P2pServerAsync.GetFreeUdp();

            UdpClientAsync p2PClientAsync0 = new(NetBufferSize.Default, true) { ReceiveTimeout = 5000 };
            p2PClientAsync0.SetCompleted((age0, age1, age2) => Completed("UDP", age0, age1, age2));
            var task0 = p2PServerAsync0.P2PConnectAsync(p2PClientAsync0, p2PServerAsync1.RemoteEP);
            //var task0 = p2PClientAsync0.P2PConnectAsync(p2PServerAsync0.LocalEP, p2PServerAsync1.RemoteEP);

            await Task.Delay(5000);

            UdpClientAsync p2PClientAsync1 = new(NetBufferSize.Default, true) { ReceiveTimeout = 5000 };
            p2PClientAsync1.SetCompleted((age0, age1, age2) => Completed("UDP", age0, age1, age2));
            var task1 = p2PServerAsync1.P2PConnectAsync(p2PClientAsync1, p2PServerAsync0.RemoteEP);
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