using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
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
        public TcpStateObject(Socket Client) : this(Client, 2048)
        {
        }

        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="Client">对象</param>
        /// <param name="DataLength">包的大小</param>
        public TcpStateObject(Socket Client, int DataLength)
        {
            this.ListData = new byte[DataLength];
            this.Client = Client;
            this.IpPort = GetIpPort(Client);
            this.SpareSize = DataLength;
            this.DataLength = DataLength;
            doReceive = new(false);

            //vs = new List<ArraySegment<byte>>() { new ArraySegment<byte>(ListData) };
        }

        /// <summary>
        /// 将包重新封装打包
        /// </summary>
        /// <param name="onlyData"></param>
        /// <param name="dataLength"></param>
        /// <param name="buffers"></param>
        /// <returns></returns>
        internal static IList<ArraySegment<byte>> GetBuffers(bool onlyData, int dataLength, params ArraySegment<byte>[] buffers)
        {
            if (buffers == null || buffers.Length == 0)
            {
                return default;// new List<ArraySegment<byte>>() { new ArraySegment<byte>() };
            }
            List<ArraySegment<byte>> _buffs = new(buffers.Length * 2);
            for (int i = 0; i < buffers.Length; i++)
            {
                if (onlyData) _buffs.Add(TcpStateObject.GetDataSend(buffers[i].Count, dataLength));
                _buffs.Add(buffers[i]);
                //ArraySegment<byte> Data = onlyData ? TcpStateObject.GetDataSend(buffers[i], dataLength) : buffers[i];
                //_buffs.Add(Data);
            }

            return _buffs;
        }

        /// <summary>
        /// 根据TcpClient获取IP加端口
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public static string GetIpPort(Socket Client)
        {
            try
            {
                if (Client == null)
                {
                    return "0.0.0.0:0";
                }
                else if (Client.RemoteEndPoint == null)
                {
                    return "0.0.0.0:0";
                }
                IPEndPoint iep = Client.RemoteEndPoint as IPEndPoint;
                if (iep.AddressFamily != AddressFamily.InterNetwork)
                {
                    return string.Concat(iep.Address.MapToIPv4(), ':', iep.Port);
                }
                else
                {
                    return string.Concat(iep.Address, ':', iep.Port);
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
                if (IpPort != null) //!string.IsNullOrEmpty(IpPort)
                {
                    ReadOnlySpan<char> chars = IpPort.AsSpan();
                    int ipportLength = chars.Length, position = 0;//, ip0 = -1, ip1 = -1, ip2 = -1, lastport = -1; //chars.LastIndexOf(':'); 21
                    int[] ints = new int[4];

                    for (int i = 0; i < ipportLength; i++)
                    {
                        if (position != 3)
                        {
                            if (chars[i].Equals('.'))
                            {
                                ints[position] = i;
                                int i0 = position == 0 ? 0 : ints[position-1] + 1;
                                if (!byte.TryParse(chars[i0..ints[position]], out _)) return false;
                                position++;
                            }
                        }
                        else
                        {
                            if (chars[i].Equals(':'))
                            {
                                int i0 = i + 1;
                                if (ushort.TryParse(chars[i0..], out _)) return true;
                            }
                        }

                        //switch (position)
                        //{
                        //    case 0:
                        //        if (chars[i].Equals('.')) 
                        //        {
                        //            ip0 = i;
                        //            if (!byte.TryParse(chars[..ip0++], out _))return false;
                        //            position++;
                        //        }
                        //        break;
                        //    case 1:
                        //        if (chars[i].Equals('.'))
                        //        {
                        //            ip1 = i;
                        //            if (!byte.TryParse(chars[ip0..ip1++], out _)) return false;
                        //            position++;
                        //        }
                        //        break;
                        //    case 2:
                        //        if (chars[i].Equals('.'))
                        //        {
                        //            ip2 = i;
                        //            if (!byte.TryParse(chars[ip1..ip2++], out _)) return false;
                        //            position++;
                        //        }
                        //        break;
                        //    case 3:
                        //        if (chars[i].Equals(':'))
                        //        {
                        //            lastport = i + 1;
                        //            if (ushort.TryParse(chars[lastport..], out _)) return true;
                        //        }
                        //        break;
                        //}
                    }


                    //if (lastport > 0 && ipportLength > lastport)
                    //{
                    //    if (!ushort.TryParse(chars[(lastport + 1)..], out _)) return false;

                    //    int type0 = chars.IndexOf('.');
                    //    if (type0 != -1 && byte.TryParse(chars[..type0++], out _))
                    //    {
                    //        int type1 = chars[type0..].IndexOf('.');
                    //        if (type1 != -1 && byte.TryParse(chars[type0..(type0 + type1++)], out _))
                    //        {
                    //            int type2 = chars[(type0 + type1)..].IndexOf('.');
                    //            if (type2 != -1 && byte.TryParse(chars[(type0 + type1)..(type0 + type1 + type2++)], out _))
                    //            {
                    //                if (byte.TryParse(chars[(type0 + type1 + type2)..lastport], out _))
                    //                {
                    //                    return true;
                    //                }
                    //            }
                    //        }
                    //    }

                        //if (type0 != -1 && type1 != -1 && type2 != -1)
                        //{
                        //    if (!byte.TryParse(chars[..type0++], out _)) return false;
                        //    if (!byte.TryParse(chars[type0..(type0 + type1++)], out _)) return false;
                        //    if (!byte.TryParse(chars[(type0 + type1)..(type0 + type1 + type2++)], out _)) return false;
                        //    if (!byte.TryParse(chars[(type0 + type1 + type2)..lastport], out _)) return false;
                        //    return true;
                        //}
                        //return false;
                        //string[] vs = IpPort.Split(':');
                        //if (vs.Length == 2)
                        //{
                        //    if (ushort.TryParse(vs[1], out ushort portNum))
                        //    {
                        //        if (portNum >= 0 && portNum <= 65535)
                        //        {
                        //            if (IPAddress.TryParse(vs[0], out _))
                        //            {
                        //                return true;
                        //            }
                        //        }
                        //    }
                        //}
                    //}
                    //if (ip.Length >= 8 && ip.Length <= 20)//0.0.0.0:0,000.000.000.000:00000
                    //{

                    //}
                }
            }
            catch (Exception)
            {
            }

            return false;
        }

        /// <summary>
        /// 根据TcpClient获取当前连接是否已经断开
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public static bool IsConnected(Socket Client)
        {
            if (Client is null) return false;
            //if (Client is null) return false;
            if (!Client.Connected) return false;
            return true;
        }

        /**
         * 给包加头保证其数据完整性(内置)
         * listData 数据
         * datalength 限制数据量
         */
        internal static void SetDataHead(Span<byte> listData, int datalength, int start)
        {
            listData[start] = 40;
            BitConverter.TryWriteBytes(listData[(start + 1)..], datalength);
            listData[start + 5] = 41;
            //BitConverter.TryWriteBytes(start > 0 ? listData[start..] : listData, datalength);
        }

        /**
         * 给包加头保证其数据完整性
         * listData 数据
         * datalength 限制数据量
         */
        //internal static ArraySegment<byte> GetDataSend(Span<byte> listData, int datalength)
        //{
        //    int length = listData.Length + HeadSize;

        //    if (length > datalength)
        //    {
        //        throw new Exception("发送数据超过设置大小，请考虑分包发送！");
        //    }
        //    //char[] head = new char[10] { '(', '\0', '\0', '\0', '\0', '\0', '\0', '\0', '\0', ')' };//UInt32

        //    //string strlength = listData.Length.ToString();
        //    ////string strdatalength = datalength.ToString();
        //    //BitConverter.GetBytes(listData.Length);
        //    //if (strlength.Length > 8)
        //    //{
        //    //    throw new Exception("发送数据超过设置大小，请考虑分包发送！");
        //    //}

        //    //for (int i = 1; i < strlength.Length + 1; i++)
        //    //{
        //    //    head[head.Length - 1 - i] = strlength[strlength.Length - i];
        //    //}

        //    //byte[] headby = Encoding.ASCII.GetBytes(head); //"(20971520)‬"

        //    ArraySegment<byte> Data = new byte[length];
        //    Data[0] = 40;
        //    Data[5] = 41;
        //    BitConverter.TryWriteBytes(Data[1..], listData.Length);

        //    //byte[] headby = BitConverter.GetBytes(listData.Length);

        //    //Data[0] = 40;
        //    //Data[1] = headby[0];
        //    //Data[2] = headby[1];
        //    //Data[3] = headby[2];
        //    //Data[4] = headby[3];
        //    //Data[5] = 41;

        //    listData.CopyTo(Data[HeadSize..]);

        //    return Data;
        //}

        internal static ArraySegment<byte> GetDataSend(int listDatalength, int datalength)
        {
            if (listDatalength + HeadSize > datalength) throw new Exception("发送数据超过设置大小，请考虑分包发送！");
            ArraySegment<byte> Data = new byte[HeadSize] { 40, 0, 0, 0, 0, 41 };
            BitConverter.TryWriteBytes(Data[1..], listDatalength);
            return Data;
        }

        /**
         * 给包加头保证其数据完整性
         * headby 数据头
         */
        internal static int GetDataHead(ReadOnlySpan<byte> headby)
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

                    return BitConverter.ToInt32(headby[1..]);
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

        internal bool IsKeepAlive(Span<byte> ListSpan)
        {
            if (ListSpan.Length == KeepAliveObj.Length
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

        internal void OnReceive(bool IsThreadPool, Memory<byte> listData, Func<ReceiveBytes<Socket>, Task> Received)
        {
            try
            {
                var data = new ReceiveBytes<Socket>(IpPort, Client, listData.Length);
                listData.CopyTo(data.Memory);
                if (IsThreadPool)
                {
                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(ReceivedAsync, data, false);
                }
                else
                {
                    ReceivedAsync(data);//触发接收事件
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Error($"多包线程{(IsThreadPool ? "池" : "")}异常", ex, "Log/Tcp");
            }

            void ReceivedAsync(ReceiveBytes<Socket> receiveBytes)
            {
                Received.Invoke(receiveBytes).Wait();//触发接收事件（兼容异步模式）
            }
        }

        /// <summary>
        /// 默认大小
        /// </summary>
        public const int HeadSize = 6;

        /// <summary>
        /// 为 TCP 网络服务提供客户端连接。
        /// </summary>
        public Socket Client { get; set; }

        //internal Socket SocketClient { get { return Client.Client; } }

        /// <summary>
        /// 用于控制异步接收消息
        /// </summary>
        public readonly System.Threading.ManualResetEvent doReceive;

        ///// <summary>
        ///// 读取索引
        ///// </summary>
        //internal int ReadIndex { get; set; }

        /**
         * 写入索引
         */
        internal int WriteIndex { get; set; }

        /**
         * 剩余大小
         */
        internal int SpareSize { get; set; }

        /// <summary>
        /// 当前对象唯一的IP：端口
        /// </summary>
        public string IpPort { get; set; }

        /// <summary>
        /// 接收的数据
        /// </summary>
        internal byte[] ListData { get; set; }

        /// <summary>
        /// 一个连续的内存块
        /// </summary>
        public Memory<byte> MemoryData => ListData;

        private Span<byte> KeepAliveObj => KeepAlive.KeepAliveObj;

        /// <summary>
        /// 表示当前一共接收到了多少
        /// </summary>
        public int Count { get; set; } = -1;

        /// <summary>
        /// 可用最大空间
        /// </summary>
        public int DataLength { get; }

        /// <summary>
        /// 任务处理机制 合并
        /// </summary>
        internal void OnReceiveTask(bool OnlyData, Action<Memory<byte>> Received)
        {
            try
            {
                if (Count is -1 or 0) throw new Exception("空包，断线！");

                Count = WriteIndex + Count;//本轮可读总长度
                WriteIndex = 0;//清零

                //byte[] UserData;
                if (OnlyData)
                {
                    if (Count > HeadSize)
                    {
                        // 累计值
                        int manytophead = 0, toplength;
                        Verify:
                        //byte[] headby = new byte[6];
                        //Array.Copy(obj.ListData, 0, headby, 0, 6);
                        toplength = manytophead + HeadSize;
                        int head = GetDataHead(MemoryData[manytophead..toplength].Span), tophead = head + HeadSize; // 当前值

                        if (head != -1)
                        {
                            manytophead += tophead;
                            if (tophead > DataLength) throw new Exception("包体异常！"); //判断包长大于最大消息体
                            int writeIndex = WriteIndex + tophead; // 当前读取位置
                            if (writeIndex <= Count)
                            {
                                Received(MemoryData[toplength..manytophead]);

                                //UserData = new byte[head];
                                //MemoryData[toplength..manytophead].CopyTo(UserData);
                                ////Array.Copy(ListData, 6, UserData, 0, head);

                                WriteIndex = writeIndex;//记录当前位置

                                //OnReceived(IsThreadPool, IpPort, Client, UserData, Received);

                                if (WriteIndex == Count)
                                {
                                    //MemoryData[..WriteIndex].Span.Clear();
                                    WriteIndex = 0;
                                    SpareSize = DataLength;
                                }
                                else if (Count - WriteIndex > HeadSize)
                                {
                                    //count = 0;
                                    goto Verify;
                                }

                                //if (WriteIndex > 0)
                                //{
                                //    //byte[] NewData = new byte[WriteIndex];
                                //    //MemoryData[tophead..].CopyTo(NewData);

                                //    if (WriteIndex < tophead)
                                //    {
                                //        MemoryData[manytophead..].CopyTo(MemoryData);
                                //        MemoryData[WriteIndex..(WriteIndex + tophead)].Span.Clear();
                                //    }

                                //    //Array.Copy(ListData, tophead, NewData, 0, WriteIndex);
                                //    //Array.Clear(ListData, 0, WriteIndex + tophead);
                                //    //Array.Copy(NewData, 0, ListData, 0, WriteIndex);
                                //}
                                //else
                                //{
                                //    MemoryData[..tophead].Span.Clear();
                                //    //Array.Clear(ListData, 0, tophead);
                                //}


                            }
                            else
                            {
                                MemoryData[WriteIndex..Count].CopyTo(MemoryData);
                                WriteIndex = Count - WriteIndex;
                                SpareSize = DataLength - WriteIndex;
                                //MemoryData[..(count - WriteIndex)].Span.Clear();
                                //doReceive.Set();
                                //return;
                            }
                        }
                        else
                        {
                            Client.Close();
                        }
                    }
                }
                else
                {
                    Received(MemoryData[..Count]);

                    //UserData = new byte[Count];
                    ////Array.Copy(ListData, 0, ListData, 0, count);
                    ////Array.Clear(ListData, 0, count);
                    //var memory = MemoryData[..Count];
                    //memory.CopyTo(UserData);
                    ////memory.Span.Clear();//无需清理
                    WriteIndex = 0;
                    SpareSize = DataLength;

                    //OnReceived(IsThreadPool, IpPort, Client, UserData, Received);
                }

                Count = -1;
            }
            catch (Exception ex)
            {
                ClientClose();
                Utils.Log.Error("解析异常：", ex, "Log/Tcp");
            }
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        public void ClientClose() 
        {
            Client.Close();
        }

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
