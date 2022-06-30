using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool;
using Tool.Sockets.SupportCode;
using Tool.Sockets.TcpFrame;
using Tool.Utils.ThreadQueue;

namespace TcpFrameTest
{
    public class Program
    {
        public static string get() 
        {
            return "哈哈哈，测试自动生成模板";
        }

        public static void Main(string[] args)
        {
            //TextTemplate1

            TaskOueue<int, bool> taskOueue = new((i) =>
            {
                return i.IsWhether(2);
            });

            taskOueue.ContinueWith += (a, b, c) => { Console.WriteLine("I:{0},{1}", a, b); };

            //for (int i = 0; i < 1_000_000_000; i++)
            //{
            //    taskOueue.Add(i);
            //}

            int c = 0;

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

            for (int i = 0; i < 20; i++)
            {
                //Thread.Sleep(200);
                Task.Run(() => 
                {
                    for (int i = 0; i < 5_0000; i++)
                    {
                        if(i.IsWhether(2)) Thread.Sleep(1);
                        taskOueue.Add(Interlocked.Increment(ref c));
                    }
                });
            }

            Console.ReadLine();

            return;

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

            TcpEventQueue.OnInterceptor(EnServer.SendMsg, true);
            TcpEventQueue.OnInterceptor(EnServer.Receive, true);

            TcpEventQueue.OnInterceptor(EnClient.SendMsg, true);
            TcpEventQueue.OnInterceptor(EnClient.Receive, true);

            ClientFrame client1 = new(Tool.Sockets.SupportCode.TcpBufferSize.Default, 108, true);

            //DataTcp.AddDataTcps(Assembly.LoadFrom(@"D:\Nixue工作室\Tool.Net\WebTestApp\bin\Debug\net5.0\WebTestApp.dll"));

            client1.SetCompleted((a, b, c) =>
            {
                //Console.WriteLine("\nIP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));

                if (b == Tool.Sockets.SupportCode.EnClient.Connect) //|| b == Tool.Sockets.SupportCode.EnClient.Receive)
                {
                    //Cstest();
                    //var data = new ApiPacket(1, 102);
                    //int c2 = ++c1;
                    //data.Set("a", c2);
                    //data.Set("path", StringExtension.GetGuid() + ".jpg");
                    //data.Bytes = System.IO.File.ReadAllBytes(@"C:\Users\Administrator\Downloads\3cd107e4ec103f614b6f7f1eca9e18e6.jpeg");
                    ////var mag = client.Send(data);
                    ////Console.WriteLine("请求结果：{0},{1},{2} \t{3}", mag.Obj, mag.OnTcpFrame, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"), ++c1);

                    ////client.SendAsync(data, (a1) => Console.WriteLine("请求结果：{0},{1},{2} \t{3}", a1.Obj, a1.OnTcpFrame, c.ToString("HH:mm:ss:fff"), c2));//, client.LocalPoint

                    //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    //client.SendAsync(data, (a1) => Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", a1.Obj, a1.OnTcpFrame, stopwatch.ElapsedMilliseconds, c2));
                }
            });

            client1.ConnectAsync("127.0.0.1", 444);//127.0.0.1

            int c1 = 0;
            //for (int i = 0; i < 1; i++)
            //{
            ClientFrame client = new(TcpBufferSize.Default, 108, true);

            client.SetCompleted((a, b, c) =>
            {
                Console.WriteLine("\nIP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));

                if (b == EnClient.Connect) //|| b == Tool.Sockets.SupportCode.EnClient.Receive)
                {
                    //Cstest();
                    //var data = new ApiPacket(1, 102);
                    //int c2 = ++c1;
                    //data.Set("a", c2);
                    //data.Set("path", StringExtension.GetGuid() + ".jpg");
                    //data.Bytes = System.IO.File.ReadAllBytes(@"C:\Users\Administrator\Downloads\3cd107e4ec103f614b6f7f1eca9e18e6.jpeg");
                    ////var mag = client.Send(data);
                    ////Console.WriteLine("请求结果：{0},{1},{2} \t{3}", mag.Obj, mag.OnTcpFrame, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"), ++c1);

                    ////client.SendAsync(data, (a1) => Console.WriteLine("请求结果：{0},{1},{2} \t{3}", a1.Obj, a1.OnTcpFrame, c.ToString("HH:mm:ss:fff"), c2));//, client.LocalPoint

                    //System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    //client.SendAsync(data, (a1) => Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", a1.Obj, a1.OnTcpFrame, stopwatch.ElapsedMilliseconds, c2));
                }
            });

            client.ConnectAsync("127.0.0.1", 444);//120.79.58.17 
            client.AddKeepAlive(5);
            //}

            void Cstest()
            {
                Task.Run(() =>
                {
                    var data = new ApiPacket(1, 102);
                    //data.Set("a", c2);
                    data.Set("path", StringExtension.GetGuid() + ".jpg");

                    System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
                    while (true)
                    {
                        int c2 = ++c1;
                        stopwatch.Restart();

                        //try
                        //{
                        //    data.Bytes = System.IO.File.ReadAllBytes("3cd107e4ec103f614b6f7f1eca9e18e6.jpeg");
                        //}
                        //catch (Exception)
                        //{
                        //    data.Bytes = new byte[] { 1, 0, 1 };
                        //}

                        data.Bytes = new byte[] { 1, 0, 1 };

                        //client.SendAsync(data, (a1) =>
                        //{
                        //    Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", a1.Obj, a1.OnTcpFrame, stopwatch.ElapsedMilliseconds, c2);
                        //});var mag = client.Send( data); //
                        //var mag = client.SendIpIdea(client1.LocalPoint, data);
                        var mag = client.Send(data);
                        Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", mag.Text, mag.OnTcpFrame, stopwatch.Elapsed.TotalMilliseconds, c2);
                        System.Threading.Thread.Sleep(1);
                        //break;
                    }
                });
            }

            ServerFrame server = new(108);

            server.SetCompleted((a, b, c) =>
            {
                Console.WriteLine("IP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
            });

            server.StartAsync("127.0.0.1", 444);

            Console.ReadLine();

            System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            long cot = 0;
            while (true)
            {
                stopwatch.Restart();
                //Console.Write("输入参数：");
                //string str = Console.ReadLine();
                //if (string.IsNullOrWhiteSpace(str)) { Console.WriteLine(); continue; }
                var data = new ApiPacket(1, 100, 100);
                data.Set("a", cot++);

                var mag = client1.Send(data);

                //client.SendAsync(data).ContinueWith(s =>
                //{
                //    var mag = s.Result;
                //    Console.WriteLine("\n请求结果：{0},{1} \t{2}ms", mag.Text, mag.OnTcpFrame, stopwatch.Elapsed.TotalMilliseconds);
                //});
                Console.WriteLine("请求结果：{0},{1} \t{2}ms", mag.Text, mag.OnTcpFrame, stopwatch.Elapsed.TotalMilliseconds);
                System.Threading.Thread.Sleep(1);
                if (cot == 10000) break;
            }

            Console.ReadLine();
        }
    }
}
