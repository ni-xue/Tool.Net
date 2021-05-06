using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Tool.Sockets.SupportCode
{
    /// <summary>
    /// 对异步接收时的对象状态的封装，将socket与接收到的数据封装在一起
    /// </summary>
    public class TcpStateObject
    {
        /// <summary>
        /// 构造包信息
        /// </summary>
        /// <param name="Client">对象</param>
        public TcpStateObject(TcpClient Client) : this(Client, 2048)
        {
        }

        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="Client">对象</param>
        /// <param name="DataLength">包的大小</param>
        public TcpStateObject(TcpClient Client, int DataLength)
        {
            this.ListData = new byte[DataLength];
            this.Client = Client;
            this.IpPort = GetIpPort(Client);
        }

        /// <summary>
        /// 将包重新封装打包
        /// </summary>
        /// <param name="onlyData"></param>
        /// <param name="dataLength"></param>
        /// <param name="buffers"></param>
        /// <returns></returns>
        internal static IList<ArraySegment<byte>> GetBuffers(bool onlyData, int dataLength, params byte[][] buffers) 
        {
            if (buffers == null || buffers.Length == 0)
            {
                return new List<ArraySegment<byte>>() { new ArraySegment<byte>() };
            }
            IList<ArraySegment<byte>> _buffs = new List<ArraySegment<byte>>(buffers.Length);
            for (int i = 0; i < buffers.Length; i++)
            {
                byte[] Data;
                if (onlyData)
                {
                    Data = TcpStateObject.GetDataSend(buffers[i], dataLength);
                }
                else
                {
                    Data = buffers[i];
                }
                _buffs.Add(new ArraySegment<byte>(Data));
            }
            
            return _buffs;
        }

        /// <summary>
        /// 根据TcpClient获取IP加端口
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public static string GetIpPort(TcpClient Client)
        {
            try
            {
                if (Client.Client == null)
                {
                    return "0.0.0.0:0";
                }
                else if (Client.Client.RemoteEndPoint == null)
                {
                    return "0.0.0.0:0";
                }
                IPEndPoint iep = Client.Client.RemoteEndPoint as IPEndPoint;
                if (iep.AddressFamily != AddressFamily.InterNetwork)
                {
                    return string.Concat(iep.Address.MapToIPv4().ToString(), ':', iep.Port);
                }
                else
                {
                    return string.Concat(iep.Address.ToString(), ':', iep.Port);
                }
            }
            catch (Exception)
            {
                return "0.0.0.0:0";
            }
        }

        /// <summary>
        /// 根据传入字符串验证是否是IP加端口
        /// </summary>
        /// <param name="IpPort">IP+端口</param>
        /// <returns></returns>
        public static bool IsIpPort(string IpPort)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(IpPort))
                {
                    if (IpPort.Contains(":"))
                    {
                        string[] vs = IpPort.Split(':');
                        if (vs.Length == 2)
                        {
                            if (ushort.TryParse(vs[1], out ushort portNum))
                            {
                                if (portNum >= 0 && portNum <= 65535)
                                {
                                    if (IPAddress.TryParse(vs[0], out _))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    //if (ip.Length >= 8 && ip.Length <= 20)//0.0.0.0:0,000.000.000.000:00000
                    //{

                    //}
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据TcpClient获取当前连接是否已经断开
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public static bool IsConnected(TcpClient Client)
        {
            if (Client == null)
            {
                return false;
            }
            if (Client.Client == null)
            {
                return false;
            }
            if (!Client.Connected)
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
            if (listData.Length + 6 > datalength)
            {
                throw new Exception("发送数据超过设置大小，请考虑分包发送！");
            }
            //char[] head = new char[10] { '(', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', ')' };//UInt32

            //string strlength = listData.Length.ToString();
            ////string strdatalength = datalength.ToString();
            //BitConverter.GetBytes(listData.Length);
            //if (strlength.Length > 8)
            //{
            //    throw new Exception("发送数据超过设置大小，请考虑分包发送！");
            //}

            //for (int i = 1; i < strlength.Length + 1; i++)
            //{
            //    head[head.Length - 1 - i] = strlength[strlength.Length - i];
            //}

            //byte[] headby = Encoding.ASCII.GetBytes(head); //"(20971520)‬"

            byte[] headby = BitConverter.GetBytes(listData.Length);

            byte[] Data = new byte[6 + listData.Length];
            Data[0] = 40;
            Data[5] = 41;
            headby.CopyTo(Data, 1);

            listData.CopyTo(Data, 6);

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
                if (headby[0] == 40 && headby[5] == 41)
                {
                    //int index;
                    //for (index = 1; index < 9; index++)
                    //{
                    //    if (headby[index] != '\0') break;
                    //}
                    //string head = Encoding.ASCII.GetString(headby, index, 9 - index); //"(20971520)‬"

                    //return head.ToInt();

                   return BitConverter.ToInt32(headby,1);
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

        internal static void OnReceived(string ipPort, byte[] listData, Action<string, byte[]> Received)
        {
            try
            {
                System.Threading.ThreadPool.QueueUserWorkItem<(string, byte[])>(x =>
                {
                    Received?.Invoke(x.Item1, x.Item2);//触发接收事件
                }, (ipPort, listData), false);
            }
            catch (Exception ex)
            {
                Utils.Log.Error("多包线程池异常", ex, "Log/Tcp");
            }
        }

        /// <summary>
        /// 为 TCP 网络服务提供客户端连接。
        /// </summary>
        public TcpClient Client { get; set; }

        internal Socket SocketClient { get { return Client.Client; } }

        internal System.Threading.ManualResetEvent doReceive { get; set; }

        ///// <summary>
        ///// 读取索引
        ///// </summary>
        //internal int ReadIndex { get; set; }

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
        public byte[] ListData { get; set; }

        /// <summary>
        /// 回收对象所以资源
        /// </summary>
        public void Close()
        {
            ListData = null;
            //ReadIndex = 0;
            WriteIndex = 0;
            if (doReceive != null) doReceive.Close();
        }
    }
}
