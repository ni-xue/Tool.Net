using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace Tool.Sockets.SupportCode
{
    /// <summary>
    /// 对异步接收时的对象状态的封装，将socket与接收到的数据封装在一起
    /// </summary>
    public class UdpStateObject
    {
        /// <summary>
        /// 为 TCP 网络服务提供客户端连接。
        /// </summary>
        public UdpClient Client { get; set; }
        private byte[] listData = new byte[2048];
        /// <summary>
        /// 接收的数据
        /// </summary>
        public byte[] ListData
        {
            get
            {
                return listData;
            }
            set
            {
                listData = value;
            }
        }
    }
}
