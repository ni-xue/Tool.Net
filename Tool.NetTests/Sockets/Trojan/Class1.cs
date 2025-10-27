using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.TrojanHelper;

namespace Tool.NetTests.Sockets.Trojan
{
    [TestClass()]
    public class Class1
    {
        [TestMethod()]
        public async Task Main()
        {
            var trojanClient = new TrojanTcpClient(new TrojanConnect("hk.ssazxc.net", 5575, "964d93708f25"));
            try
            {
                // 连接到 Trojan 服务器
                await trojanClient.ConnectAsync(CancellationToken.None);
                Console.WriteLine("已连接到 Trojan 服务器");

                // 测试连接到一个小型、快速响应的目标
                var stream = await trojanClient.ConnectToTargetAsync("www.gstatic.com", 80, CancellationToken.None);

                // 发送一个简单的 HTTP 请求来验证连接
                byte[] testRequest = Encoding.UTF8.GetBytes("GET /generate_204 HTTP/1.1\r\nHost: www.gstatic.com\r\n\r\n");
                await stream.WriteAsync(testRequest);

                // 尝试读取响应（带超时）
                byte[] buffer = new byte[1024];
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(5));
                int bytesRead = await stream.ReadAsync(buffer, cts.Token);

                //// 通过 Trojan 代理连接到目标网站
                //var stream = await trojanClient.ConnectToTargetAsync("www.google.com", 443);
                //Console.WriteLine("已通过 Trojan 连接到目标服务器");

                //// 发送 HTTP 请求
                //string httpRequest = "GET / HTTP/1.1\r\nHost: www.example.com\r\nConnection: close\r\n\r\n";
                //byte[] requestBytes = Encoding.UTF8.GetBytes(httpRequest);
                //await trojanClient.SendAsync(requestBytes);

                //// 接收响应
                //byte[] buffer = new byte[4096];
                //int bytesRead;
                //while ((bytesRead = await trojanClient.ReceiveAsync(buffer)) > 0)
                //{
                //    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                //    Console.WriteLine($"收到响应: {response}");
                //}
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
            }
            finally
            {
                trojanClient.Dispose();
            }
        }
    }
}
