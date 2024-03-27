using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tool;
using Tool.Utils;

namespace WebTestApp
{
    public class Class1 : Tool.Sockets.NetFrame.DataBase
    {
        [Tool.Sockets.NetFrame.DataTcp(1)]
        public Class1()
        {

        }

        [Tool.Sockets.NetFrame.DataTcp(100)]
        public Tool.Sockets.NetFrame.IGoOut A(int a)
        {
            //this.Bytes
            return Json(new { a });
        }

        [Tool.Sockets.NetFrame.DataTcp(101)]
        public Tool.Sockets.NetFrame.IGoOut B(string path)
        {
            byte[] s = System.IO.File.ReadAllBytes(path);

            return Write(s);//new byte[1024 * 100];
        }

        [Tool.Sockets.NetFrame.DataTcp(102)]
        public Tool.Sockets.NetFrame.IGoOut C(string path)
        {
            //System.IO.File.WriteAllBytes(path, Bytes);//23797

            return Write("保存成功！");//new byte[1024 * 100];
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            ////Tool.Sockets.SupportCode.TcpEventQueue.OnInterceptor(Tool.Sockets.SupportCode.EnClient.SendMsg, true);

            //Tool.Sockets.TcpFrame.ClientFrame client = new Tool.Sockets.TcpFrame.ClientFrame(Tool.Sockets.SupportCode.TcpBufferSize.Default, 9, true);
            //int c1 = 0;
            ////System.Diagnostics.Stopwatch stopwatch;// = System.Diagnostics.Stopwatch.StartNew();
            //client.SetCompleted((a, b, c) =>
            //{
            //    //Console.WriteLine("IP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));

            //    if (b == Tool.Sockets.SupportCode.EnClient.Connect || b == Tool.Sockets.SupportCode.EnClient.Receive)//)//
            //    {
            //        System.Diagnostics.Stopwatch stopwatch = System.Diagnostics.Stopwatch.StartNew();
            //        var data = new Tool.Sockets.TcpFrame.ApiPacket(1, 102);
            //        //data.Set("path", @"C:\Users\Administrator\Downloads\9a023c9b7f15f9f7ea8268437e269772.jpg");
            //        int c2 = ++c1;
            //        data.Set("path",StringExtension.GetGuid() + ".jpg");
            //        data.Bytes = System.IO.File.ReadAllBytes(@"C:\Users\Administrator\Downloads\3cd107e4ec103f614b6f7f1eca9e18e6.jpeg");  //new byte[1024 * 100];
            //        //var mag = client.Send(data);
            //        //Console.WriteLine("请求结果：{0},{1}", mag.Obj, mag.OnTcpFrame);

            //        client.SendAsync(data, (a1) => Console.WriteLine("请求结果：{0},{1} \t{2}ms \t{3}", a1.Obj, a1.OnTcpFrame, stopwatch.ElapsedMilliseconds, c2));

            //        //byte[] s = System.IO.File.ReadAllBytes(@"C:\Users\Administrator\Downloads\3cd107e4ec103f614b6f7f1eca9e18e6.jpeg");
            //        //for (int i = 0; i < s.Length; i++)
            //        //{
            //        //    if (s[i] != mag.Bytes[i])
            //        //    {

            //        //    }
            //        //}

            //        //System.IO.File.WriteAllBytes("1.jpg", mag.Bytes);//23797
            //    }
            //});

            //client.ConnectAsync("127.0.0.1", 440);

            //Tool.Sockets.TcpFrame.ServerFrame server = new Tool.Sockets.TcpFrame.ServerFrame(9);

            //server.SetCompleted((a, b, c) =>
            //{
            //    //Console.WriteLine("IP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
            //});

            //server.StartAsync("127.0.0.1", 440);

            CreateWebHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateWebHostBuilder(string[] args) =>
            //WebHost.CreateDefaultBuilder(args).UseUrls(AppSettings.Get("server.urls").Split(';'))
            //    .UseStartup<Startup>();

            Host.CreateDefaultBuilder(args)
                    .ConfigureWebHostDefaults(webBuilder =>
                    {
                        webBuilder.UseUrls(AppSettings.Get("server.urls").Split(';'));
                        webBuilder.UseStartup<Startup>();
                    });
    }
}
