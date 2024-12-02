using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.NetFrame;

namespace Tool.NetTests.Sockets
{
    /// <summary>
    /// ClientFrame类的行为
    /// </summary>
    [Flags]
    public enum EnClient : byte
    {
        /// <summary>
        /// 连接服务器成功时发生
        /// </summary>
        Connect = 1,
        /// <summary>
        /// 连接服务器失败时发生
        /// </summary>
        Fail = 2,
        /// <summary>
        /// 向服务器发送数据时发生
        /// </summary>
        SendMsg = 4,
        /// <summary>
        /// 收到服务器数据时发生
        /// </summary>
        Receive = 8,
        /// <summary>
        /// 与服务器断开连接时发生
        /// </summary>
        Close = 16,
        /// <summary>
        /// 心跳包事件（推送后触发）
        /// </summary>
        HeartBeat = 32
    }

    /// <summary>
    /// ServerFrame类的行为
    /// </summary>
    public enum EnServer : byte
    {
        /// <summary>
        /// 服务器创建成功时发生
        /// </summary>
        Create = 1,
        /// <summary>
        /// 服务器创建失败时发生
        /// </summary>
        Fail = 2,
        /// <summary>
        /// 客户端连接服务器成功时发生
        /// </summary>
        Connect = 3,
        /// <summary>
        /// 向客户端发送数据时发生
        /// </summary>
        SendMsg = 4,
        /// <summary>
        /// 收到客户端数据时发生
        /// </summary>
        Receive = 5,
        /// <summary>
        /// 当处于连接状态的客户端断开时发生
        /// </summary>
        ClientClose = 6,
        /// <summary>
        /// 服务端关闭时发生
        /// </summary>
        Close = 7,
        /// <summary>
        /// 心跳包事件（接收后触发）
        /// </summary>
        HeartBeat = 10
    }

    [TestClass()]
    public class Class1
    {
        private readonly struct EventState
        {
            public EventState(bool IsEvent, bool IsQueue)
            {
                this.IsEvent = IsEvent;
                this.IsQueue = IsQueue;
            }

            public bool IsEvent { get; }

            public bool IsQueue { get; }
        }

        ConcurrentDictionary<Enum, EventState> _IsEnumOns;

        EnClient enClient;

        public Class1()
        {
            enClient = EnClient.Connect | EnClient.Receive | EnClient.Fail | EnClient.SendMsg;

            _IsEnumOns = new ConcurrentDictionary<Enum, EventState>();
            _IsEnumOns.TryAdd(EnClient.Connect, new EventState(true, false));
            _IsEnumOns.TryAdd(EnClient.Close, new EventState(true, false));
            _IsEnumOns.TryAdd(EnClient.Fail, new EventState(true, false));

            _IsEnumOns.TryAdd(EnServer.Connect, new EventState(true, false));
            _IsEnumOns.TryAdd(EnServer.ClientClose, new EventState(true, false));

            _IsEnumOns.TryAdd(EnServer.Create, new EventState(true, false));
            _IsEnumOns.TryAdd(EnServer.Close, new EventState(true, false));
            _IsEnumOns.TryAdd(EnServer.Fail, new EventState(true, false));
        }

        [TestMethod()]
        public void Cs0()
        {
            for (int i = 0; i < 10000000; i++)
            {
                if (IsEnumOn(EnServer.ClientClose, out var isQueue))
                {

                }
                else
                {
                    Console.WriteLine("出错了1");
                }
            }

            Console.WriteLine("现用方式");
        }

        [TestMethod()]
        public void Cs1()
        {
            for (int i = 0; i < 10000000; i++)
            {
                if ((enClient & EnClient.Connect) != 0)
                {
                    if ((enClient & EnClient.Receive) != 0)
                    {

                    }
                }
                else
                {
                    Console.WriteLine("出错了2");
                }
            }

            Console.WriteLine("最新方式");
        }

        [TestMethod()]
        public async Task Cs2()
        {
            ClientFrame client = new(NetBufferSize.Size512K, true) { IsThreadPool = true };

            client.SetCompleted((a1, b1, c1) =>
            {
                Console.WriteLine("IP:{0} \t{1} \t{2}", a1, b1, c1.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
                return ValueTask.CompletedTask;
            });

            await client.ConnectAsync("127.0.0.1", 444);//120.79.58.17 
            client.AddKeepAlive(5);

            A:
            ApiPacket packet = new(1, 102, 60000, true);
            using (var netResponse = await client.SendAsync(packet))
            {
                switch (netResponse.State)
                {
                    case not NetFrameState.Success:
                        Console.WriteLine("访问状态：{0}，{1}", netResponse.State, netResponse.Error?.Message);
                        //await Task.Delay(1000);
                        goto A;
                    default:
                        Console.WriteLine("Ok");
                        break;
                }
            }

            using (await client.SendAsync(packet)) { }
            using (await client.SendAsync(packet)) { }
            using (await client.SendAsync(packet)) { }
            using (await client.SendAsync(packet)) { }
        }

        private bool IsEnumOn(Enum enAction, out bool IsQueue)
        {
            if (_IsEnumOns.IsEmpty is false && _IsEnumOns.TryGetValue(enAction, out EventState value)) { IsQueue = value.IsQueue; return value.IsEvent; }
            IsQueue = true;
            return IsQueue;
        }
    }
}
