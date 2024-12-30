using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.Win32;
using System.Buffers;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.IO.Pipelines;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.PortableExecutable;
using System.Runtime.Versioning;
using System.Text;
using System.Threading;
using Tool;
using Tool.Sockets.Kernels;
using Tool.Sockets.NetFrame;
using Tool.Sockets.P2PHelpr;
using Tool.Sockets.TcpHelper;
using Tool.Sockets.UdpHelper;
using Tool.SqlCore;
using Tool.Utils;
using Tool.Utils.Data;

namespace TcpTest
{
    public class Def
    {
        public int? a { get; init; }
        public string? b { get; set; }
        public DateTime? c { get; init; } = DateTime.Now;

        public string b1 = string.Empty;
    }

    public class Abc : Def
    {
        public new string b1 = string.Empty;

        public new int? a { get; init; }
        public new string? b { get; set; }
        public new DateTime? c { get; init; } = DateTime.Now;
        public bool? d { get; set; }
        public byte? e { get; init; }
        public double? f;
        public decimal? g { get; init; }

        protected object? s => throw new AggregateException();

        protected static object? a1 { get; set; }

        private static object? a2 { get; set; }

        public static object? a3 { get; set; }

        protected static object? a4;

        private static object? a5;

        public static object? a6;

        private readonly string rest = string.Empty;
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

        static async Task<int> GetIntAsync()
        {
            _ = Tool.Utils.ThreadQueue.TaskQueue.StaticEnqueue(GetIntAsync).ContinueWith((a) => { a.Dispose(); Console.WriteLine($"{Tool.Utils.ThreadQueue.TaskQueue.Count}\t{Tool.Utils.ThreadQueue.TaskQueue.CompleteCount}\t{Tool.Utils.ThreadQueue.TaskQueue.TotalCount}"); });
            return await Task.FromResult(123_456);
        }

        static async Task Main(string[] args)
        {
            //using Tool.Utils.TaskHelper.TaskWithTimeout taskWith = new(TimeSpan.FromMilliseconds(10000));
            //await taskWith;

            //var re = await Tool.Utils.ThreadQueue.TaskQueue.StaticEnqueue<int>(GetIntAsync);
            //await Tool.Utils.ThreadQueue.TaskQueue.StaticEnqueue(Main, args);
            //await GetIntAsync();
            //await Task.Delay(200000);
            ////await re;

            //var dic0 = """{ "key": "123" }""".Json();

            ////PropertyInfo[]? properties = null;
            ////var asda = Tool.Utils.ActionDelegate.ClassFieldDispatcher.GetClassFields(typeof(Abc), ref properties);

            ////var dir = asda.Invoke(new Abc());

            Abc abc = new Abc() { a = 20, b1 = "我在" };

            var b1 = abc.GetValue("b1");
            var a = abc.GetValue("a");

            //abc.GetValue("rest");

            //var dic = abc.GetDictionary();
            //dic["a"] = 999;
            //abc.SetDictionary(dic);

            //abc.SetFieldKey("f", 0.1);

            //abc.SetPropertyKey("a", 123456);

            ////abc.GetValue("a0");
            //abc.SetValue("a1", 0.1);
            //abc.SetValue("a2", 0.2);
            //abc.SetValue("a3", 0.3);
            //abc.SetValue("a4", 0.4);
            //abc.SetValue("a5", 0.5);
            //abc.SetValue("a6", 0.6);

            //abc.SetPropertyKey("a1", 1);
            //abc.SetPropertyKey("a2", 2);
            //abc.SetPropertyKey("a3", 3);
            //abc.SetFieldKey("a4", 4);
            //abc.SetFieldKey("a5", 5);
            //abc.SetFieldKey("a6", 6);

            //abc.GetPropertyKey("s", out var s);

            await P2pWorship.OnMain(args);
            //await NetWorship.OnMain(args);
            //await TcpWorship.OnMain(args);
            //await UdpWorship.OnMain(args);
            //await WebWorship.OnMain(args);
            //await QuicWorship.OnMain(args);

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