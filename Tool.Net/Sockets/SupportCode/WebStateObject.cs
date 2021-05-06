using System;
using System.Collections.Generic;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using Tool.Sockets.WebTcp;

namespace Tool.Sockets.SupportCode
{
    /// <summary>
    /// 对异步接收时的对象状态的封装，将Websocket与接收到的数据封装在一起
    /// </summary>
    public class WebStateObject
    {
        /// <summary>
        /// 构造包信息
        /// </summary>
        /// <param name="Client">对象</param>
        public WebStateObject(WebContext Client) : this(Client, 2048)
        {
        }

        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="Client">对象</param>
        /// <param name="DataLength">包的大小</param>
        public WebStateObject(WebContext Client, int DataLength)
        {
            this.ListData = new ArraySegment<byte>(new byte[DataLength]);
            this.Client = Client;
            this.IpPort = Client.IpPort;
        }

        /// <summary>
        /// 根据WebContext获取IP加端口
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public static string GetIpPort(HttpListenerContext Context)
        {
            try
            {
                if (Context.Request == null)
                {
                    return "0.0.0.0:0";
                }
                else if (Context.Request.RemoteEndPoint == null)
                {
                    return "0.0.0.0:0";
                }
                IPEndPoint iep = Context.Request.RemoteEndPoint as IPEndPoint;
                return string.Format("{0}:{1}", iep.Address, iep.Port);
            }
            catch (Exception)
            {
                return "0.0.0.0:0";
            }
        }

        /// <summary>
        /// 根据WebContext获取当前连接是否已经断开
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public static bool IsConnected(WebContext Client)
        {
            if (Client == null)
            {
                return false;
            }
            if (Client.Socket == null)
            {
                return false;
            }
            if (Client.Socket.State != WebSocketState.Open)
            {
                return false;
            }
            return true;
        }

        /**
         * 给包加头保证其数据完整性
         * listData 数据
         * datalength 限制数据量
         */
        internal static byte[] GetDataSend(byte[] listData, int datalength)
        {
            if (listData.Length + 10 > datalength)
            {
                throw new Exception("发送数据超过设置大小，请考虑分包发送！");
            }
            char[] head = new char[10] { '(', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', ')' };

            string strlength = listData.Length.ToString();
            //string strdatalength = datalength.ToString();

            if (strlength.Length > 8)
            {
                throw new Exception("发送数据超过设置大小，请考虑分包发送！");
            }

            for (int i = 1; i < strlength.Length + 1; i++)
            {
                head[head.Length - 1 - i] = strlength[strlength.Length - i];
            }

            byte[] headby = Encoding.ASCII.GetBytes(head); //"(20971520)‬"

            byte[] Data = new byte[10 + listData.Length];

            headby.CopyTo(Data, 0);

            listData.CopyTo(Data, 10);

            return Data;
        }

        /**
         * 给包加头保证其数据完整性
         * headby 数据头
         */
        internal static int GetDataHead(byte[] headby)
        {
            try
            {
                if (headby[0] == 40 && headby[9] == 41)
                {
                    int index;
                    for (index = 1; index < 9; index++)
                    {
                        if (headby[index] != '\0') break;
                    }
                    string head = Encoding.ASCII.GetString(headby, index, 9 - index); //"(20971520)‬"

                    return head.ToInt();
                }
                else
                {
                    return -1;
                }
            }
            catch
            {

                return -1;
            }
        }

        /// <summary>
        /// 为 TCP 网络服务提供客户端连接。
        /// </summary>
        public WebContext Client { get; set; }

        internal WebSocket SocketClient { get { return Client.Socket; } }

        internal System.Threading.ManualResetEvent doReceive { get; set; }

        /**
         * 写入索引
         */
        internal int WriteIndex { get; set; }

        /// <summary>
        /// 当前对象唯一的IP：端口
        /// </summary>
        public string IpPort { get; set; }

        /// <summary>
        /// 接收的数据
        /// </summary>
        public ArraySegment<byte> ListData { get; set; }

        /// <summary>
        /// 回收对象所以资源
        /// </summary>
        public void Close()
        {
            ListData = new ArraySegment<byte>();
            //ReadIndex = 0;
            WriteIndex = 0;
            if (doReceive != null) doReceive.Close();
        }
    }
}
