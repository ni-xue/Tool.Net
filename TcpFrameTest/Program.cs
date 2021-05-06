using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Tool;
using Tool.Sockets.TcpFrame;

namespace TcpFrameTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //Console.BackgroundColor = ConsoleColor.Cyan;
            //Console.ForegroundColor = ConsoleColor.White;

            ClientFrame client1 = new ClientFrame(Tool.Sockets.SupportCode.TcpBufferSize.Default, 108, true);

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
                ClientFrame client = new ClientFrame(Tool.Sockets.SupportCode.TcpBufferSize.Default, 108, true);

                client.SetCompleted((a, b, c) =>
                {
                    Console.WriteLine("\nIP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));

                    if (b == Tool.Sockets.SupportCode.EnClient.Connect) //|| b == Tool.Sockets.SupportCode.EnClient.Receive)
                    {
                        Cstest();
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
                        try
                        {
                            data.Bytes = System.IO.File.ReadAllBytes("3cd107e4ec103f614b6f7f1eca9e18e6.jpeg");
                        }
                        catch (Exception)
                        {
                        }
                        
                        int c2 = ++c1;
                        stopwatch.Restart();
                        //client.SendAsync(data, (a1) =>
                        //{
                        //    Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", a1.Obj, a1.OnTcpFrame, stopwatch.ElapsedMilliseconds, c2);
                        //});var mag = client.Send( data); //
                        var mag = client.SendIpIdea(client1.LocalPoint, data);
                        Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", mag.Obj, mag.OnTcpFrame, stopwatch.ElapsedMilliseconds, c2);
                        System.Threading.Thread.Sleep(3 * 1000);
                        //break;
                    }
                });
            }

            ServerFrame server = new ServerFrame(108);

            server.SetCompleted((a, b, c) =>
            {
                Console.WriteLine("IP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
            });

            server.StartAsync("127.0.0.1", 444);

            while (true)
            {
                //Console.Write("输入参数：");
                string str = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(str)) { Console.WriteLine(); continue; }
                var data = new ApiPacket(1, 100, 100);
                data.Set("a", str);
                var mag = client.Send(data);
                Console.WriteLine("\n请求结果：{0},{1}", mag.Obj, mag.OnTcpFrame);
            }
        }
    }
}
