using BenchmarkDotNet.Disassemblers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.NetFrame;

namespace TcpFrameTest
{

    public class Class1 : DataBase
    {
        [DataTcp(1)]
        public Class1()
        {

        }

        protected override bool Initialize(DataTcp dataTcp)
        {
            return true;
        }

        [DataTcp(100)]
        public IGoOut A(int a)
        {
            return Json(new { a });
        }

        [DataTcp(101)]
        public IGoOut B(string path)
        {
            Interlocked.Increment(ref c);
            byte[] s = File.ReadAllBytes(path);
            Interlocked.Increment(ref d);
            return Ok("Ok", s);
        }

        [DataTcp(102)]
        public async Task<IGoOut> C(string path)
        {
            Interlocked.Increment(ref c);
            //if (!File.Exists(path)) File.WriteAllBytes(path, Bytes.Array ?? throw new());
            Interlocked.Increment(ref d);
            return await WriteAsync("Ok"); 
        }

        [DataTcp(103)]
        public IGoOut D(string path)
        {
            return Write("保存成功！");
        }

        [DataTcp(104)]
        public async Task<IGoOut> E() 
        {
            return await WriteAsync("测试结果！");
        }

        public static ulong c;
        public static ulong d;
        public static ulong e;

        [DataTcp(250)]
        public IGoOut A(string a)
        {
            Interlocked.Increment(ref c);
            var hh = Random.Shared.Next(2, 50);
            Thread.Sleep(hh);
            Interlocked.Increment(ref d);
            //Console.WriteLine(Interlocked.Increment(ref c));
            return Write(a);
        }

    }
}
