using System;
using System.Buffers;
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
                if (!string.IsNullOrWhiteSpace(IpPort))
                {
                    if (IpPort.Contains(':'))
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
            if (Client is null) return false;
            if (Client.Client is null) return false;
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

        internal static void OnReceived(bool IsThreadPool, string ipPort, TcpClient client, IMemoryOwner<byte> listData, int length, Action<TcpBytes> Received)
        {
            try
            {
                var data = new TcpBytes(ipPort, client, listData, length);

                if (IsThreadPool)
                {
                    System.Threading.ThreadPool.QueueUserWorkItem(Received, data, false);
                }
                else
                {
                    Received?.Invoke(data);//触发接收事件
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Error($"多包线程{(IsThreadPool ? "池" : "")}异常", ex, "Log/Tcp");
            }
        }

        /// <summary>
        /// 默认大小
        /// </summary>
        public const int HeadSize = 6;

        /// <summary>
        /// 为 TCP 网络服务提供客户端连接。
        /// </summary>
        public TcpClient Client { get; set; }

        internal Socket SocketClient { get { return Client.Client; } }

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
        internal void OnReceiveTask(bool OnlyData, bool IsThreadPool, ref Action<TcpBytes> Received)
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
                                var memoryOwner = MemoryPool<byte>.Shared.Rent(head);
                                MemoryData[toplength..manytophead].CopyTo(memoryOwner.Memory);
                                OnReceived(IsThreadPool, IpPort, Client, memoryOwner, head, Received);

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
                    var memoryOwner = MemoryPool<byte>.Shared.Rent(Count);
                    MemoryData[..Count].CopyTo(memoryOwner.Memory);
                    OnReceived(IsThreadPool, IpPort, Client, memoryOwner, Count, Received);

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
                Client.Close();
                Utils.Log.Error("解析异常：", ex, "Log/Tcp");
            }
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

    /// <summary>
    /// Tcp 通讯资源 对象（必须回收，丢失风险大）
    /// </summary>
    public readonly struct TcpBytes//: IDisposable
    {
        private readonly IMemoryOwner<byte> _dataOwner;

        /// <summary>
        /// 流长度
        /// </summary>
        public readonly int Length;

        /// <summary>
        /// 资源对象
        /// </summary>
        /// <param name="key">IP端口</param>
        /// <param name="client">连接对象</param>
        /// <param name="dataOwner">可回收数据对象</param>
        /// <param name="length">包含长度</param>
        public TcpBytes(string key, TcpClient client, IMemoryOwner<byte> dataOwner, int length) : this()
        {
            Key = key;
            Client = client;
            _dataOwner = dataOwner;
            Length = length;   
            //Data = _dataOwner.Memory[..length];
        }

        /// <summary>
        /// IP端口
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// 连接对象
        /// </summary>
        public TcpClient Client { get; }

        /// <summary>
        /// 返回数据
        /// </summary>
        public Memory<byte> Data => _dataOwner.Memory[..Length];//{ get; }

        /// <summary>
        /// 使用完后及时回收
        /// </summary>
        public void Dispose() => _dataOwner.Dispose();
    }
}
