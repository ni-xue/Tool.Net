using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Quic;
using System.Reflection;
using System.Runtime.Versioning;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.QuicHelper;
using Tool.Utils;

namespace TcpTest
{
    [SupportedOSPlatform("windows")]
    internal class QuicWorship
    {
        [RequiresPreviewFeatures]
        internal static async Task OnMain(string[] args)
        {
            EnumEventQueue.OnInterceptor(EnServer.SendMsg, true);
            EnumEventQueue.OnInterceptor(EnServer.Receive, true);

            EnumEventQueue.OnInterceptor(EnClient.SendMsg, true);
            EnumEventQueue.OnInterceptor(EnClient.Receive, true);

            string? dns = null; //args[0] = "0";
#if DEBUG //false
            args[0] = "0";
            IPAddress ipadrs = await Utility.GetIPAddressAsync();
#else
            var ipadrs = await Utility.GetIPAddressAsync(dns = "nixue.top", System.Net.Sockets.AddressFamily.InterNetwork);
#endif
            string ip = "127.0.0.1";
            if (ipadrs is not null) ip = ipadrs.ToString();

            int type = 0;
            if (args.Length > 0)
            {
                _ = int.TryParse(args[0], out type);
                if (args.Length == 2) { dns = null; ip = args[1]; }
            }
            
            if (type == 2) goto A;
            QuicServerAsync server = new(NetBufferSize.Default, true);
            server.SetInitCertificate(async (conn, ssl) =>
            {
                X509Certificate2 certificate2 = new("nixue.top.pfx", "au1pcpa1");
                await Console.Out.WriteLineAsync($"发送证书：{certificate2.Subject}");
                return certificate2;
            });
            server.SetCompleted(async (age0, age1, age2) =>
            {
                await Console.Out.WriteLineAsync($"[Server]-[{age0}]-[{age1}]-[{age2}]");
            });
            server.SetReceived(async receive =>
            {
                using (receive)
                {
                    ProcessLine(receive.Span, true);
                }
                await Task.CompletedTask;
            });
            await server.StartAsync(ip, 4455);
            if (type == 1) goto B;

            A:
            QuicClientAsync client = new(NetBufferSize.Default, true);
            client.SetCompleted(async (age0, age1, age2) =>
            {
                await Console.Out.WriteLineAsync($"[Client]-[{age0}]-[{age1}]-[{age2}]");
            });
            client.SetReceived(async receive =>
            {
                using (receive)
                {
                    ProcessLine(receive.Span, false);
                }
                await Task.CompletedTask;
            });
            using (Task task = string.IsNullOrEmpty(dns) ? client.ConnectAsync(ip, 4455) : client.ConnectAsync(new DnsEndPoint(dns, 4455, System.Net.Sockets.AddressFamily.InterNetwork)))
            {
                await task;
            }
            //await Task.Delay(100);

            for (int j = 1; j <= 10; j++)
            {
                run(j);
            }

        B:
            async void run(int i)
            {
                await Task.Run(async () =>
                {
                    // 打开一个出站的双向流
                    while (true)
                    {
                        // 写入数据
                        await Task.Delay(i * 2);
                        var message = $"Hello Quic [{i}] \n";

                        try
                        {
                            await client.SendAsync(message);
                            Console.Write($"Send -> {message}");
                        }
                        catch (Exception)
                        {
                        }
                    }
                });
            }
        }

        static void ProcessLine(in Span<byte> buffer, bool isServer)
        {
            string name = isServer ? "Server" : "Client";
            Console.Write($"[{name}]Recevied -> {Encoding.UTF8.GetString(buffer)}");
        }
    }
}
