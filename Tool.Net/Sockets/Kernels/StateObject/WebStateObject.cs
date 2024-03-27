using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.WebHelper;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 对异步接收时的对象状态的封装，将Websocket与接收到的数据封装在一起
    /// </summary>
    public class WebStateObject
    {
        /// <summary>
        /// 服务商构造
        /// </summary>
        /// <param name="context">对象</param>
        /// <param name="DataLength">包的大小</param>
        public WebStateObject(WebSocketContext context, int DataLength) : this(context.SecWebSocketKey, context.WebSocket, DataLength)
        {
            this.WebSocketContext = context;
        }

        /// <summary>
        /// 连接者构造
        /// </summary>
        /// <param name="Client">对象</param>
        /// <param name="DataLength">包的大小</param>
        /// <param name="SocketKey">连接标识</param>
        public WebStateObject(string SocketKey, WebSocket Client, int DataLength)
        {
            this.ListData = new Memory<byte>(new byte[DataLength]);
            this.Client = Client;
            this.SocketKey = SocketKey;
        }

        #region 静态函数

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
        public static bool IsConnected(WebSocket Client)
        {
            if (Client == null)
            {
                return false;
            }
            if (Client == null)
            {
                return false;
            }
            if (Client.State != WebSocketState.Open)
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

        public static async Task<string> IsWebIpEffective(string ip)
        {
            if (ip.Equals("0.0.0.0") || ip.Equals("*"))
            {
                string name = Dns.GetHostName();
                IPAddress[] ipadrlist = await Dns.GetHostAddressesAsync(name);
                foreach (IPAddress ipadr in ipadrlist)
                {
                    if (ipadr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                    {
                        return ipadr.ToString();
                    }
                }
            }

            return ip;
        }

        #endregion

        internal bool IsKeepAlive(int conut)
        {
            if (conut == KeepAliveObj.Length
                && ListSpan[0] == KeepAliveObj[0]
                && ListSpan[1] == KeepAliveObj[1]
                && ListSpan[2] == KeepAliveObj[2]
                && ListSpan[3] == KeepAliveObj[3]
                && ListSpan[4] == KeepAliveObj[4]
                && ListSpan[5] == KeepAliveObj[5]
                && ListSpan[6] == KeepAliveObj[6])
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal void OnReceived<T>(bool IsThreadPool, T Client, int Count, Func<ReceiveBytes<T>, Task> Received)
        {
            try
            {
                var receiveBytes = new ReceiveBytes<T>(SocketKey, Client, Count);
                ListData[..Count].CopyTo(receiveBytes.Memory);
                if (IsThreadPool)
                {
                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(ReceivedAsync, receiveBytes, false);
                }
                else
                {
                    ReceivedAsync(receiveBytes);
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Error($"多包线程{(IsThreadPool ? "池" : "")}异常", ex, "Log/WebSocket");
            }

            void ReceivedAsync(ReceiveBytes<T> receiveBytes) 
            {
                Received.Invoke(receiveBytes).Wait();//触发接收事件（兼容异步模式）
            }
        }

        /// <summary>
        /// 关闭当前用户连接以及数据
        /// </summary>
        public async void Abort()
        {
            await Client.CloseAsync(WebSocketCloseStatus.NormalClosure, "已经断开连接！", System.Threading.CancellationToken.None);
            //    .ContinueWith((i, ip) =>
            //{
            //    if (i.IsCompleted)
            //    {
            Debug.WriteLine("{0} ->已经断开！{1}", SocketKey, DateTime.Now.ToString());
            //    }
            //}, IpPort);
            Client.Abort();
        }

        /// <summary>
        /// 握手后的重要数据
        /// </summary>
        public WebSocketContext WebSocketContext { get; }

        /// <summary>
        /// 为 WebSocket 网络服务提供客户端连接。
        /// </summary>
        public WebSocket Client { get; }

        /// <summary>
        /// 返回 WebSocket 连接的当前状态。
        /// </summary>
        /// <remarks>WebSocket 连接的当前状态。</remarks>
        public WebSocketState State => Client.State;

        //internal System.Threading.ManualResetEvent doReceive { get; set; }移除改变思路

        /// <summary>
        /// 当前对象唯一的连接票据
        /// </summary>
        public string SocketKey { get; }

        /// <summary>
        /// 接收的数据
        /// </summary>
        public Memory<byte> ListData { get; private set; }

        /// <summary>
        /// 接收的数据
        /// </summary>
        public Span<byte> ListSpan => ListData.Span;

        private Span<byte> KeepAliveObj => KeepAlive.KeepAliveObj;

        /// <summary>
        /// 回收对象所以资源
        /// </summary>
        public void Close()
        {
            ListData = Memory<byte>.Empty;//new ArraySegment<byte>();
            //doReceive?.Close();
            Client.Dispose();
        }
    }
}
