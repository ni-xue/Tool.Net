﻿using System;
using System.Buffers;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using System.Xml.Linq;
using Tool;
using Tool.Sockets.SupportCode;
using Tool.Sockets.TcpFrame;
using Tool.Utils;
using Tool.Utils.ActionDelegate;
using Tool.Utils.Data;

namespace TcpTest
{
    internal class Program
    {

        public class abc 
        {
            public string A;
            public  int a { get; init; }

            public string b { get; set; } = string.Empty;

            public DateTime c { get; init; }

            public bool d { get; set; }

            public byte e { get; set; }

            public byte f { get; }

            public string g { get; }
        }

        static void Main(string[] args)
        {
            DataTable dataTable = new();

            DataColumn dc = new();

            dataTable.Columns.Add("column0", typeof(string));

            DataRow dr = dataTable.NewRow();

            dr[0] = "张三";

            dataTable.Rows.Add(dr);

            //dataTable.Rows[0]

            dataTable.Rows[0][0] = "张三"; //通过索引赋值

            dataTable.Rows[0]["COLumn0"] = "66666666666666666666666666666666666666666666666666666";

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

            var type = typeof(abc);

             //new ClassFieldDispatcher(type);


            //abc.c = DateTime.Now;
            //Hashtable hashtable = new();
            //hashtable.Add("a", 4446);
            //hashtable.Add("b", "66666");
            //hashtable.Add("c", DateTime.Now);
            //hashtable.Add("d", true);
            //hashtable.Add("e", (byte)66);

            //hashtable["a"] = i;
            //hashtable["b"] = "123";
            //hashtable["c"] = DateTime.Now;
            //hashtable["d"] = true;
            //hashtable["e"] = (byte)100;

            int i = 0, i1 = 0;

            for (; i < 1000000; i++)
            {
                var a2 = EntityBuilder.GetEntity(type);

                var abc = a2.New;

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

            for (; i1 < 1000000; i1++)
            {
                var a2 = EntityBuilder.GetEntity(type);
                var abc = a2.New;
                var sa = a2.Get(abc);
            }

            Console.WriteLine();

            for (; i >= 0; i--)
            {
                var abc = Activator.CreateInstance(type);
                PropertyInfo[] properties = type.GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    if (property.Name == "a")
                    {
                        property.SetValue(abc, i);
                    }
                    if (property.Name == "b")
                    {
                        property.SetValue(abc, i.ToString());
                    }
                    if (property.Name == "c")
                    {
                        property.SetValue(abc, DateTime.Now);
                    }
                    if (property.Name == "d")
                    {
                        property.SetValue(abc, true);
                    }
                    if (property.Name == "e")
                    {
                        property.SetValue(abc, (byte)250);
                    }
                }
            }
            
            Console.WriteLine();

            for (; i1 >= 0; i1--)
            {
                var abc = Activator.CreateInstance(type);
                //IDictionary<string, object> hashtable = new Dictionary<string, object>(6);
                //PropertyInfo[] properties = type.GetProperties();
                //foreach (PropertyInfo property in properties)
                //{
                //    hashtable.Add(property.Name, property.GetValue(abc) ?? throw new());
                //}

                abc.ToDictionary();
            }

            Console.WriteLine();

            for (; i1 < 1000000; i1++)
            {
                var abc = Activator.CreateInstance(type);
                //IDictionary<string, object> hashtable = new Dictionary<string, object>(6);
                //PropertyInfo[] properties = type.GetProperties();
                //foreach (PropertyInfo property in properties)
                //{
                //    hashtable.Add(property.Name, property.GetValue(abc) ?? throw new());
                //}

                abc.GetDictionary();
            }

            Console.WriteLine();

            for (; i < 1000000; i++)
            {
                var abc = Activator.CreateInstance(type);

                IDictionary<string, object> hashtable = new Dictionary<string, object>(5);
                hashtable.Add("a", i);
                hashtable.Add("b", i.ToString());
                hashtable.Add("c", DateTime.Now);
                hashtable.Add("d", true);
                hashtable.Add("e", (byte)66);

                //hashtable.TryGetValue("e", out object v);

                abc.SetDictionary(hashtable);
                //s1(abc, hashtable);
            }

            Console.WriteLine();
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

            KeepAlive keep = new(1, () =>
            {
                Console.Clear();
                Console.WriteLine("情况：{0}，{1}，{2}", ThreadPool.ThreadCount, ThreadPool.PendingWorkItemCount, ThreadPool.CompletedWorkItemCount);
                for (int i = 0; i < 20; i++)
                {
                    Thread.Sleep(i);
                    Console.WriteLine("发起：总 {0},ok {1},断 {2},时 {3},错 {4},无 {5},Id {6},对 {7}", a, d, e, f, g, b, c, s);
                }
            });

            //ClientFrameList clientFrameList = new(10);

            //for (int i = 0; i < 10; i++)
            //{
            ClientFrame client = new(TcpBufferSize.Default, 108, true);

            client.SetCompleted((a1, b1, c1) =>
            {
                if (b1 == EnClient.Connect)
                {
                    a = d = e = f = g = b = c = s = 0;
                }
                Console.WriteLine("\nIP:{0} \t{1} \t{2}", a1, b1, c1.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
            });

            client.ConnectAsync("127.0.0.1", 444);//120.79.58.17 
            client.AddKeepAlive(5);

            //    clientFrameList.AddClientFrame(client);
            //}

            //clientFrameList.Completed += ClientFrameList_Completed;

            TcpEventQueue.OnInterceptor(EnClient.SendMsg, true);
            TcpEventQueue.OnInterceptor(EnClient.Receive, true);

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

                            switch (mag.OnTcpFrame)
                            {
                                case TcpFrameState.Default:
                                    Interlocked.Increment(ref b);
                                    break;
                                case TcpFrameState.OnlyID:
                                    Interlocked.Increment(ref c);
                                    break;
                                case TcpFrameState.Success:
                                    Interlocked.Increment(ref d);
                                    try
                                    {
                                        //if (!File.Exists(url)) File.WriteAllBytes(url, mag.Bytes.Array ?? throw new());
                                        if(mag.Text == "Ok")
                                        Interlocked.Increment(ref s);
                                    }
                                    catch (Exception)
                                    {
                                    }
                                    break;
                                case TcpFrameState.SendFail:
                                    Interlocked.Increment(ref e);
                                    break;
                                case TcpFrameState.Timeout:
                                    Interlocked.Increment(ref f);
                                    break;
                                case TcpFrameState.Exception:
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
                packet.Bytes = new byte[] { 1, 2, 3, 4 };
                var response = client.Send(packet);// clientFrameList.Send(packet, out _);
                //a.Response.StatusCode = 200;
                //a.Response.Write(s.Text);
                //if (s.OnTcpFrame != TcpFrameState.Success)
                //{
                //    Console.WriteLine("失败->{0}", s.OnTcpFrame.ToString());
                //}

                switch (response.OnTcpFrame)
                {
                    case TcpFrameState.Default:
                        Interlocked.Increment(ref b);
                        break;
                    case TcpFrameState.OnlyID:
                        Interlocked.Increment(ref c);
                        break;
                    case TcpFrameState.Success:
                        Interlocked.Increment(ref d);
                        if (guid == response.Text)
                        {
                            Interlocked.Increment(ref s);
                        }
                        break;
                    case TcpFrameState.SendFail:
                        Interlocked.Increment(ref e);
                        break;
                    case TcpFrameState.Timeout:
                        Interlocked.Increment(ref f);
                        break;
                    case TcpFrameState.Exception:
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