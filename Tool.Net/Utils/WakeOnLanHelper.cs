using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Tool.Utils
{
    /// <summary>
    /// “魔术包”唤醒计算机
    /// </summary>
    public static class WakeOnLanHelper
    {
        /// <summary>
        /// 通过MAC地址唤醒一台主机
        /// </summary>
        /// <param name="macAddress">目标主机的MAC地址，格式如 "11-22-33-44-55-66" 或 "11:22:33:44:55:66"</param>
        /// <param name="broadcastAddress">广播地址，默认为 "255.255.255.255"</param>
        /// <param name="port">端口号，默认为 9</param>
        /// <returns>一个表示异步操作的任务</returns>
        public static async Task SendWakeOnLanAsync(string macAddress, string broadcastAddress = "255.255.255.255", int port = 9)
        {
            if (string.IsNullOrWhiteSpace(macAddress))
                throw new ArgumentException("MAC地址不能为空", nameof(macAddress));

            // 解析MAC地址
            byte[] macBytes = ParseMacAddress(macAddress);

            // 构建魔术包
            byte[] magicPacket = BuildMagicPacket(macBytes);

            // 使用UDP客户端发送数据包
            using UdpClient client = new();
            // 允许发送广播包
            client.EnableBroadcast = true;
            // 如果你需要发送到特定子网，可以取消注释下行
            //client.Client.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.Broadcast, true);

            IPEndPoint endPoint = new(IPAddress.Parse(broadcastAddress), port);
            await client.SendAsync(magicPacket, magicPacket.Length, endPoint);
        }

        /// <summary>
        /// 解析MAC地址字符串为字节数组
        /// </summary>
        private static byte[] ParseMacAddress(string macAddress)
        {
            // 移除分隔符
            string cleanMac = macAddress.Replace(":", "").Replace("-", "").Replace(" ", "");

            if (cleanMac.Length != 12)
                throw new ArgumentException("MAC地址格式无效。应为 'XX:XX:XX:XX:XX:XX' 或 'XX-XX-XX-XX-XX-XX'");

            byte[] bytes = new byte[6];
            for (int i = 0; i < 6; i++)
            {
                string byteValue = cleanMac.Substring(i * 2, 2);
                bytes[i] = Convert.ToByte(byteValue, 16);
            }
            return bytes;
        }

        /// <summary>
        /// 根据MAC地址构建魔术包
        /// </summary>
        private static byte[] BuildMagicPacket(byte[] macAddress)
        {
            byte[] packet = new byte[102]; // 6 + 16*6 = 102

            // 1. 添加6个0xFF作为前导符
            for (int i = 0; i < 6; i++)
            {
                packet[i] = 0xFF;
            }

            // 2. 重复16次MAC地址
            for (int i = 1; i <= 16; i++)
            {
                Buffer.BlockCopy(macAddress, 0, packet, i * 6, 6);
            }

            return packet;
        }

        /// <summary>
        /// 通过MAC地址唤醒一台主机
        /// </summary>
        /// <param name="macAddress">目标主机的MAC地址，格式如 "11-22-33-44-55-66" 或 "11:22:33:44:55:66"</param>
        /// <param name="broadcastAddress">广播地址，默认为 "255.255.255.255"</param>
        /// <param name="port">端口号，默认为 9</param>
        public static void SendWakeOnLan(string macAddress, string broadcastAddress = "255.255.255.255", int port = 9)
        {
            SendWakeOnLanAsync(macAddress, broadcastAddress, port).GetAwaiter().GetResult();
        }
    }
}
