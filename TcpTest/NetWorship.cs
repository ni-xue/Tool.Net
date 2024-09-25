using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Tool;
using Tool.Sockets.Kernels;
using Tool.Sockets.NetFrame;

namespace TcpTest
{
    internal class NetWorship
    {
        internal static async Task OnMain(string[] args)
        {
            ulong a = 0, b = 0, c = 0, d = 0, e = 0, f = 0, g = 0, s = 0;

            KeepAlive keepok = new(1, async () =>
            {
                Console.Clear();
                Console.WriteLine("情况：{0}，{1}，{2}", ThreadPool.ThreadCount, ThreadPool.PendingWorkItemCount, ThreadPool.CompletedWorkItemCount);
                for (int i = 0; i < 20; i++)
                {
                    await Task.Delay(i);
                    Console.WriteLine("发起：总 {0},ok {1},断 {2},时 {3},错 {4},无 {5},Id {6},对 {7}", a, d, e, f, g, b, c, s);
                }
            });

            ClientFrame client = new(NetBufferSize.Default, true);

            client.SetCompleted((a1, b1, c1) =>
            {
                if (b1 == EnClient.Connect)
                {
                    a = d = e = f = g = b = c = s = 0;
                }
                Console.WriteLine("IP:{0} \t{1} \t{2}", a1, b1, c1.ToString("yyyy/MM/dd HH:mm:ss:fffffff"));
                return ValueTask.CompletedTask;
            });

            await client.ConnectAsync("127.0.0.1", 444);//120.79.58.17 
            client.AddKeepAlive(5);

            EnumEventQueue.OnInterceptor(EnClient.SendMsg, true);
            EnumEventQueue.OnInterceptor(EnClient.Receive, true);

            ApiPacket packet = new(1, 104, 60000);
            packet.Set("a", "asd123");
            using var response = await client.SendRelayAsync("0.0.0.0:1", packet);

            Thread.Sleep(1000);
            Task[] tasks;
            switch (Console.ReadKey(true).KeyChar)
            {
                case '0':
                    tasks = new Task[8];
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        tasks[i] = Task.Factory.StartNew(example0);
                    }
                    break;
                default:
                    tasks = new Task[50];
                    for (int i = 0; i < tasks.Length; i++)
                    {
                        tasks[i] = Task.Factory.StartNew(example1);
                    }
                    break;
            }

            Task.WaitAll(tasks);

            async Task example0()
            {
                Stopwatch watch = Stopwatch.StartNew();
                var guid = StringExtension.GetGuid();
                ApiPacket packet = new(1, 104, 10000, false);
                packet.Set("a", guid);
                while (watch.ElapsedMilliseconds < 20000)
                {
                    //await Task.Delay(RandomNext("等待", 0, 2));
                    //Thread.Sleep(RandomNext("等待", 1, 5));
                    await get0(packet, guid);
                }
            }

            async Task example1()
            {
                string url = 0 == 0 ? "3cd107e4ec103f614b6f7f1eca9e18e6.jpeg" : "1f94a936494a49b6b2fbcadecd4ca16c.jpeg";
                ApiPacket packet = new(1, 102, 10000);
                packet.Set("path", url);
                packet.Bytes = File.ReadAllBytes(url);
                while (true)
                {
                    await Task.Delay(RandomNext("等待", 0, 1));
                    //Thread.Sleep(RandomNext("等待", 1, 5));

                    int count = RandomNext("次数", 10, 20);
                    for (int i = 0; i < count; i++)
                    {
                        //ThreadPool.UnsafeQueueUserWorkItem(get, default);
                        await get1(packet);
                    }
                };
            }

            async Task get0(ApiPacket api, string guid)
            {
                Interlocked.Increment(ref a);
                using var response = await client.SendAsync(api);
                OnNetFrame(response.State, () =>
                {
                    if (guid == response.Text) return true;
                    return false;
                });
            }

            async Task get1(ApiPacket api)
            {
                Interlocked.Increment(ref a);
                using var mag = await client.SendAsync(api);
                OnNetFrame(mag.State, () =>
                {
                    //if (!File.Exists(url)) File.WriteAllBytes(url, mag.Bytes.Array ?? throw new());
                    if (mag.Text == "Ok") return true;
                    return false;
                });
            }

            int RandomNext(string msg, int minValue, int maxValue)
            {
                int ret = Random.Shared.Next(minValue, maxValue);
                //Debug.WriteLine($"({msg}) 耗时：{ret}ms");
                return ret;
            }

            void OnNetFrame(NetFrameState state, Func<bool> isok)
            {
                switch (state)
                {
                    case NetFrameState.Default:
                        Interlocked.Increment(ref b);
                        break;
                    case NetFrameState.OnlyID:
                        Interlocked.Increment(ref c);
                        break;
                    case NetFrameState.Success:
                        Interlocked.Increment(ref d);
                        try
                        {
                            if (isok()) Interlocked.Increment(ref s);
                        }
                        catch (Exception)
                        {
                        }
                        break;
                    case NetFrameState.SendFail:
                        Interlocked.Increment(ref e);
                        break;
                    case NetFrameState.Timeout:
                        Interlocked.Increment(ref f);
                        break;
                    case NetFrameState.Exception:
                        Interlocked.Increment(ref g);
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
