using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Utils;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 对异步接收时的对象状态的封装，将Websocket与接收到的数据封装在一起
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class WebStateObject : StateObject
    {
        private static readonly SemaphoreSlim slimlock = new(1, 1); //发送数据限流

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
        public WebStateObject(in UserKey SocketKey, WebSocket Client, int DataLength)
        {
            this.ListData = new Memory<byte>(new byte[DataLength]);
            this.Client = Client;
            this.SocketKey = SocketKey;
            WriteHeap = new();
        }

        #region 静态函数

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
            if (Client.State != WebSocketState.Open)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 根据WebContext获取当前连接是否已经断开
        /// </summary>
        /// <returns></returns>
        internal bool IsConnected() => IsConnected(Client);

        /// <summary>
        /// 返回可用的IP信息
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static async Task<string> IsWebIpEffective(string ip)
        {
            if (ip.Equals("0.0.0.0") || ip.Equals("*"))
            {
                IPAddress ipadr = await Utility.GetIPAddressAsync(ip, AddressFamily.InterNetwork);
                return ipadr.ToString();
            }

            return ip;
        }

        #endregion

        internal bool IsKeepAlive()
        {
            bool isKeep = WriteHeap.IsEmpty && Utility.SequenceCompare(ListData.Span[..Count], KeepAliveObj);
            if (isKeep) Count = 0;
            return isKeep;
        }

        internal static async ValueTask SendAsync(WebSocket client, WebSocketMessageType messageType, Memory<byte> listData, int dataLength)
        {
            try
            {
                await slimlock.WaitAsync();
                bool sendend = true;
                int position = 0;
                while (sendend)
                {
                    Memory<byte> part;
                    if (listData.Length - position > dataLength)
                    {
                        part = listData.Slice(position, dataLength);
                        position += part.Length;
                    }
                    else
                    {
                        part = listData[position..];
                        sendend = false;
                    }
                    await client.SendAsync(part, messageType, !sendend, CancellationToken.None);// : WebSocketMessageType.Text WebSocketMessageType.Binary
                }
            }
            finally
            {
                slimlock.Release();
            }
        }

        internal async ValueTask<bool> ReceiveAsync()
        {
            if (Client.State != WebSocketState.Open) return false;

            ValueWebSocketReceiveResult receiveResult = await Client.ReceiveAsync(ListData[Count..], CancellationToken.None);

            switch (receiveResult.MessageType)
            {
                case WebSocketMessageType.Text:
                case WebSocketMessageType.Binary:
                    Count += receiveResult.Count;
                    if (receiveResult.EndOfMessage)
                    {
                        if (!WriteHeap.IsEmpty) CopyWriteHeap();
                        MessageType = receiveResult.MessageType;
                        return true;
                    }
                    else
                    {
                        if (Count >= ListData.Length) CopyWriteHeap();
                        return false;
                    }
                case WebSocketMessageType.Close:
                default:
                    Client.Abort();
                    return false;
            }

            void CopyWriteHeap()
            {
                WriteHeap.Copy(ListData[..Count]);
                Count = 0;
            }
        }

        internal async ValueTask OnReceivedAsync<T>(bool IsThreadPool, T Client, ReceiveEvent<T> Received)
        {
            try
            {
                int count = GetCount();
                if (Received is null) return;
                var receiveBytes = new ReceiveBytes<T>(SocketKey, Client, count, false);
                receiveBytes.SetMessageType(MessageType);
                if (WriteHeap.IsEmpty)
                {
                    receiveBytes.SetMemory(ListData[..count]);
                }
                else
                {
                    var memories = WriteHeap.ToReadOnlySequence();
                    receiveBytes.SetMemory(in memories);
                    WriteHeap.Empty();
                }
                if (IsThreadPool)
                {
                    QueueUserWorkItem(Received, receiveBytes);
                }
                else
                {
                    await ReceivedAsync(Received, receiveBytes);
                }
            }
            catch (Exception ex)
            {
                Log.Error($"任务Core{(IsThreadPool ? "池" : "")}异常", ex, "Log/WebSocket");
            }
        }

        private int GetCount()
        {
            int count = Count;
            Count = 0;
            if (WriteHeap.IsEmpty) return count;
            return WriteHeap.Length;
        }

        /// <summary>
        /// 关闭当前用户连接以及数据
        /// </summary>
        public async Task AbortAsync()
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
        public UserKey SocketKey { get; }

        /// <summary>
        /// 接收的数据
        /// </summary>
        public Memory<byte> ListData { get; private set; }

        /**
        * 表示当前一共接收到了多少
        */
        private int Count;

        /**
         * 表示当前数据包类型
         */
        private WebSocketMessageType MessageType;

        /// <summary>
        /// 接收的数据
        /// </summary>
        private MemorySegment<byte> WriteHeap;

        /// <summary>
        /// 回收对象所以资源
        /// </summary>
        public void Close()
        {
            ListData = Memory<byte>.Empty;//new ArraySegment<byte>();
            if (!WriteHeap.IsEmpty) WriteHeap.Empty();
            WriteHeap = null;
            //doReceive?.Close();
            Client.Dispose();
        }
    }
}
