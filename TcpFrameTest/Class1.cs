//using BenchmarkDotNet.Disassemblers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.NetFrame;
using Tool.Utils;

namespace TcpFrameTest
{
    public class Class1 : DataBase
    {
        [DataNet(1)]
        public Class1()
        {

        }

        protected override bool Initialize(DataNet dataTcp)
        {
            Interlocked.Increment(ref c);
            return true;
        }

        protected override void NetException(Exception ex)
        {
            Log.Warn("错误：", ex);
            base.NetException(ex);
        }

        protected override void Dispose()
        {
            if (IsReply) Interlocked.Increment(ref d);
        }

        [DataNet(100)]
        public IGoOut A(int a)
        {
            return Json(new { a });
        }

        [DataNet(101)]
        public IGoOut B(string path)
        {
            byte[] s = File.ReadAllBytes(path);
            return Ok("Ok", s);
        }

        [DataNet(102)]
        public async Task<IGoOut> C(string path)
        {
            //if (!File.Exists(path)) File.WriteAllBytes(path, Bytes.Array ?? throw new());
            return await OkAsync();
        }

        [DataNet(103)]
        public IGoOut D(string path)
        {
            using var fileStream = File.OpenWrite($"Download\\{OnlyID}{path}");
            fileStream.Write(Bytes);
            return Write("保存成功！");
        }

        [DataNet(104)]
        public async Task<IGoOut> E(string a)
        {
            return await WriteAsync(a);
        }

        public static ulong c;
        public static ulong d;
        public static ulong e;

        [DataNet(250)]
        public async ValueTask<GoOut> A(string a)
        {
            //    Interlocked.Increment(ref c);
            var hh = Random.Shared.Next(200, 500);
            await Task.Delay(hh);
            //Interlocked.Increment(ref d);
            return (GoOut)Write(a);
        }

    }
}
