using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tool;
using Tool.Sockets.NetFrame;
using Tool.Utils;
using Tool.Utils.ActionDelegate;
using WebTestApp.Test;

namespace WebTestApp
{
    public class Class1 : Tool.Sockets.NetFrame.DataBase
    {
        [Tool.Sockets.NetFrame.DataNet(1)]
        public Class1()
        {

        }

        [Tool.Sockets.NetFrame.DataNet(100)]
        public Tool.Sockets.NetFrame.IGoOut A(int a)
        {
            //this.Bytes
            return Json(new { a });
        }

        [Tool.Sockets.NetFrame.DataNet(101)]
        public Tool.Sockets.NetFrame.IGoOut B(string path)
        {
            byte[] s = System.IO.File.ReadAllBytes(path);

            return Write(s);//new byte[1024 * 100];
        }

        [Tool.Sockets.NetFrame.DataNet(102)]
        public Tool.Sockets.NetFrame.IGoOut C(string path)
        {
            //System.IO.File.WriteAllBytes(path, Bytes);//23797

            return Write("保存成功！");//new byte[1024 * 100];
        }

        [Tool.Sockets.NetFrame.DataNet(103)]
        public async ValueTask<IGoOut> D(string path)
        {
            //System.IO.File.WriteAllBytes(path, Bytes);//23797

            return await WriteAsync("保存成功！");//new byte[1024 * 100];
        }

        [Tool.Sockets.NetFrame.DataNet(104)]
        public async Task<GoOut> E(string path)
        {
            //System.IO.File.WriteAllBytes(path, Bytes);//23797

            return (GoOut)await WriteAsync("保存成功！");//new byte[1024 * 100];
        }
    }

    public class Program
    {
        public static void Abc(object a, in object b, ref object c, out string[] args)
        {
            a = 10;
            //b = 20;
            c = 30;
            args = null;
        }

        public static void Abc<A, B, C, D>(A a, in B b, ref C c, out D[] args) { args = null; }
        public static int GetInt(string a)
        {
            return int.Parse(a);
        }
        public static Task<int> GetIntAdync(string a)
        {
            return Task.FromResult(int.Parse(a));
        }

        public static void Main(string[] args)
        {
            //List<SystemTest> list = [];
            //for (int i = 0; i < 1000; i++)
            //{
            //    list.Add(new() { id = i, key_cn = StringExtension.GetGuid(), key_en = StringExtension.GetGuid(), value = StringExtension.GetGuid() });
            //}
            //var a = list.ToJson();
            //args = new string[] { "abc" };
            //MethodInfo[] methods = typeof(Program).GetMethods();
            //foreach (var method in methods)
            //{
            //    if (method.Name.StartsWith("GetInt")) 
            //    {
            //        ActionDispatcher<Program, object> dispatcher = new(method);
            //        if (dispatcher.IsTask)
            //        {
            //            var a = dispatcher.InvokeAsync(null, "654321").Result;
            //        }
            //        else
            //        {
            //            var a = dispatcher.Invoke(null, "654321");
            //        }
            //    }
            //        if (method.Name != "Abc") continue;
            //    ActionDispatcher action = new(method.ContainsGenericParameters ? method.AddMake(typeof(short), typeof(int), typeof(long), typeof(string)) : method);

            //    int[] ints = { 1, 2, 3, };

            //    action.Invoke(null, new object[] { (short)ints[0], ints[1], (long)ints[2], args });
            //}

            //System.Threading.Tasks.Dataflow.ActionBlock
            //Tool.Sockets.SupportCode.TcpEventQueue.OnInterceptor(Tool.Sockets.SupportCode.EnClient.SendMsg, true);

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

            //var sampleJson = File.ReadAllBytes("D:\\NixueStudio\\Tool.Net\\WebTestApp\\appsettings.json").AsSpan();
            //Utf8JsonReader reader = new Utf8JsonReader(sampleJson);
            //while (reader.Read())//reader就会移动到JSON数据里面的下一个Token那里
            //{
            //    string item = SwitchToken(reader);
            //    if (reader.TokenType == JsonTokenType.PropertyName)
            //    {
            //        string startIndex = reader.TokenStartIndex.ToString();//token 开始字符为止
            //        Console.Write(startIndex + item + ":");
            //    }
            //    else { Console.WriteLine(item); }
            //    //Console.WriteLine(reader.CurrentDepth);
            //    //Console.WriteLine(reader.CurrentState);
            //}

            //string SwitchToken(Utf8JsonReader reader) =>
            //    reader.TokenType switch
            //    {
            //        JsonTokenType.StartObject => "{",
            //        JsonTokenType.EndObject => "}",
            //        JsonTokenType.StartArray => "[",
            //        JsonTokenType.EndArray => "]",
            //        JsonTokenType.False => $"{reader.GetBoolean()},",
            //        JsonTokenType.PropertyName => $@"""{reader.GetString()!}""",

            //        JsonTokenType.True => $"{reader.GetBoolean()},",
            //        JsonTokenType.Number => $"{reader.GetInt32()},",

            //        JsonTokenType.String => @$"""{reader.GetString()}"",",
            //        JsonTokenType.Null => "Null",
            //        _ => "None",
            //    };

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
