using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.NetFrame;
using Tool.Utils;

namespace Tool.NetTests.Sockets.NetFrame
{
    [TestClass()]
    public class ClientFrameTests
    {
        public ClientFrameTests() 
        {
            ServerFrame server = new(NetBufferSize.Size512K) { IsAllowRelay = true };

            //server.SetIpParser(GetKey);

            server.SetCompleted((a, b, c) =>
            {
                Console.WriteLine("IP:{0} \t{1} \t{2}", a, b, c.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
                return ValueTask.CompletedTask;
            });

            server.StartAsync("0.0.0.0", 444).Wait();
        }

        [TestMethod()]
        public async Task RelayTestAsync()
        {
            ClientFrame client0 = new(NetBufferSize.Default, true);

            client0.SetCompleted((a1, b1, c1) =>
            {
                Console.WriteLine("IP:{0} \t{1} \t{2}", a1, b1, c1.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
                return ValueTask.CompletedTask;
            });

            await client0.ConnectAsync("127.0.0.1", 444);//120.79.58.17 
            client0.AddKeepAlive(5);

            ClientFrame client1 = new(NetBufferSize.Default, true);

            client1.SetCompleted((a1, b1, c1) =>
            {
                Console.WriteLine("IP:{0} \t{1} \t{2}", a1, b1, c1.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
                return ValueTask.CompletedTask;
            });

            await client1.ConnectAsync("127.0.0.1", 444);//120.79.58.17 
            client1.AddKeepAlive(5);

            ApiPacket packet = new(1, 104, 60000);
            packet.Set("a", "asd123");
            using var response0 = await client1.SendRelayAsync(client0.LocalPoint, packet);

            using var response1 = await client0.SendAsync(packet);
        }
    }

    public class Class1 : DataBase
    {
        [DataNet(1)]
        public Class1()
        {

        }

        protected override bool Initialize(DataNet dataTcp)
        {
            return true;
        }

        protected override void NetException(Exception ex)
        {
            Log.Warn("错误：", ex);
            base.NetException(ex);
        }

        protected override void Dispose()
        {
        }

        [DataNet(104, IsRelay = false)]
        public async Task<IGoOut> E(string a)
        {
            return await WriteAsync(a);
        }
    }
}
