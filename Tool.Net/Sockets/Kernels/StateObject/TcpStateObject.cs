using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Tool.Sockets.Kernels
{
    /// <summary>
    /// 对异步接收时的对象状态的封装，将socket与接收到的数据封装在一起
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class TcpStateObject : StateObject
    {
        private ReceiveEvent<Socket> Received;

        /// <summary>
        /// 构造包信息
        /// </summary>
        /// <param name="Client">对象</param>
        public TcpStateObject(Socket Client) : this(Client, 2048, false, null)
        {
        }

        /// <summary>
        /// 有参构造
        /// </summary>
        /// <param name="Client">对象</param>
        /// <param name="DataLength">包的大小</param>
        /// <param name="OnlyData">数据唯一标识</param>
        /// <param name="received">委托函数</param>
        public TcpStateObject(Socket Client, int DataLength, bool OnlyData, ReceiveEvent<Socket> received)
        {
            this.MemoryData = new byte[DataLength];
            this.Client = Client;
            this.IpPort = GetIpPort(Client);
            //this.SpareSize = DataLength;
            this.DataLength = DataLength;
            this.OnlyData = OnlyData;
            Received = received;
            //doReceive = new(false);
        }

        /// <summary>
        /// 将包重新封装打包
        /// </summary>
        /// <param name="dataLength"></param>
        /// <param name="buffers"></param>
        /// <returns></returns>
        internal IList<ArraySegment<byte>> GetBuffers(int dataLength, params ArraySegment<byte>[] buffers)
        {
            if (buffers == null || buffers.Length == 0)
            {
                return default;// new List<ArraySegment<byte>>() { new ArraySegment<byte>() };
            }
            List<ArraySegment<byte>> _buffs = new(buffers.Length * 2);
            for (int i = 0; i < buffers.Length; i++)
            {
                if (OnlyData) _buffs.Add(GetDataSend(buffers[i].Count, dataLength));
                _buffs.Add(buffers[i]);
                //ArraySegment<byte> Data = onlyData ? TcpStateObject.GetDataSend(buffers[i], dataLength) : buffers[i];
                //_buffs.Add(Data);
            }

            return _buffs;
        }

        /// <summary>
        /// 根据Socket获取当前连接是否已经断开
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public static bool IsConnected(Socket Client)
        {
            if (Client is null) return false;
            if (!Client.Connected) return false;
            return true;
        }

        /// <summary>
        /// 根据Socket获取当前连接是否已经断开
        /// </summary>
        /// <returns></returns>
        internal bool IsConnected() => IsConnected(Client);

        internal bool IsKeepAlive(in Memory<byte> ListSpan) => OnlyData && Utils.Utility.SequenceCompare(ListSpan.Span, KeepAlive.TcpKeepObj.Span);

        internal async ValueTask<bool> ReceiveAsync()
        {
            var emptyData = WriteHeap.IsEmpty ? getMemoryData() : WriteHeap.EmptyData;
            int count = await Client.ReceiveAsync(emptyData, SocketFlags.None, CancellationToken.None);
            if (count is -1 or 0) throw new Exception("空包，断线！");
            if (WriteHeap.IsEmpty)
            {
                Count += count;
                return true;
            }
            else
            {
                WriteHeap.SetCount(count);
                return WriteHeap.IsSuccess;
            }

            Memory<byte> getMemoryData()
            {
                int start = WriteIndex;
                if (start == -1)
                {
                    start = Count;
                    WriteIndex = 0;
                }
                return MemoryData[start..];
            }
        }

        internal async ValueTask OnReceiveAsync(bool IsThreadPool, Memory<byte> listData)
        {
            try
            {
                if (Received is null) return;
                ReceiveBytes<Socket> data;

                if (WriteHeap.IsEmpty)
                {
                    data = new ReceiveBytes<Socket>(IpPort, Client, listData.Length, OnlyData);
                    data.SetMemory(listData);
                }
                else
                {
                    data = WriteHeap.GetReceiveBytes(IpPort, Client);
                }

                if (IsThreadPool)
                {
                    QueueUserWorkItem(Received, data);
                }
                else
                {
                    await ReceivedAsync(Received, data);//触发接收事件
                }
            }
            catch (Exception ex)
            {
                Utils.Log.Error($"任务Core{(IsThreadPool ? "池" : "")}异常", ex, "Log/Tcp");
            }

            //async Task ReceivedPool(ReceiveBytes<Socket> receiveBytes)
            //{
            //    await ReceivedAsync(receiveBytes);
            //}

            //async ValueTask ReceivedAsync(ReceiveBytes<Socket> receiveBytes)
            //{
            //    await Received.Invoke(receiveBytes);//触发接收事件（兼容异步模式）
            //}
        }

        /// <summary>
        /// 为 TCP 网络服务提供客户端连接。
        /// </summary>
        public Socket Client { get; set; }

        /**
         * 写入索引
         */
        private int WriteIndex;

        /**
         * 表示当前一共接收到了多少
         */
        private int Count;

        /**
         * 一个连续的内存块
         */
        private Memory<byte> MemoryData;

        /**
         * 意料外的数据模板
         */
        private MemoryWriteHeap WriteHeap;

        /// <summary>
        /// 当前对象唯一的IP：端口
        /// </summary>
        public UserKey IpPort { get; set; }

        /// <summary>
        /// 可用最大空间
        /// </summary>
        public int DataLength { get; }

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        #region 旧版本

        //internal Socket SocketClient { get { return Client.Client; } }

        ///// <summary>
        ///// 用于控制异步接收消息
        ///// </summary>
        //public readonly System.Threading.ManualResetEvent doReceive;

        ///// <summary>
        ///// 读取索引
        ///// </summary>
        //internal int ReadIndex { get; set; }

        ////**
        // * 剩余大小
        // */
        //private int SpareSize;

        ///// <summary>
        ///// 一个连续的空内存
        ///// </summary>
        //public Memory<byte> EmptyData => MemoryData[WriteIndex..]; //[WriteIndex..SpareSize];

        ///// <summary>
        ///// 任务处理机制 合并
        ///// </summary>
        //internal void OnReceiveTask(Action<Memory<byte>> Received)
        //{
        //    try
        //    {
        //        if (Count is -1 or 0) throw new Exception("空包，断线！");

        //        Count = WriteIndex + Count;//本轮可读总长度
        //        WriteIndex = 0;//清零

        //        //byte[] UserData;
        //        if (OnlyData)
        //        {
        //            if (Count > HeadSize)
        //            {
        //                // 累计值
        //                int manytophead = 0, indexbit;//, toplength;
        //            Verify:
        //                //byte[] headby = new byte[6];
        //                //Array.Copy(obj.ListData, 0, headby, 0, 6);
        //                indexbit = manytophead; //toplength = (indexbit = manytophead) + HeadSize;
        //                int head, tophead;
        //                if (WriteHeap.IsEmpty)
        //                {
        //                    head = GetDataHeadTcp(MemoryData.Span.Slice(manytophead, HeadSize)/*[manytophead..toplength].Span*/);
        //                    tophead = head + HeadSize; // 当前值
        //                }
        //                else
        //                {
        //                    head = tophead = WriteHeap.Length;
        //                }

        //                if (head != -1)
        //                {
        //                    manytophead += tophead;
        //                    if (tophead > DataLength)
        //                    {
        //                        if (WriteHeap.IsEmpty) WriteHeap = new(manytophead, MemoryData[indexbit..Count]);

        //                        int count = 0;//.SetMemory(MemoryData, indexbit, Count);

        //                        //Debug.WriteLine($"{WriteHeap.WriteIndex} ~ {WriteHeap.SpareSize}");

        //                        if (WriteHeap.IsSuccess)
        //                        {
        //                            Received(WriteHeap.Memory);
        //                            WriteHeap.Empty();
        //                        }

        //                        if (count > 0)
        //                        {
        //                            manytophead = WriteIndex = Count - count;

        //                            if (Count - WriteIndex > HeadSize)
        //                            {
        //                                //count = 0;
        //                                goto Verify;
        //                            }
        //                        }
        //                        else
        //                        {
        //                            WriteIndex = 0;
        //                            //SpareSize = DataLength;
        //                        }
        //                        //throw new Exception("包体异常！");
        //                    } //判断包长大于最大消息体
        //                    else
        //                    {
        //                        int writeIndex = WriteIndex + tophead; // 当前读取位置
        //                        if (writeIndex <= Count)
        //                        {
        //                            Received(MemoryData[indexbit..manytophead]);

        //                            //UserData = new byte[head];
        //                            //MemoryData[toplength..manytophead].CopyTo(UserData);
        //                            ////Array.Copy(ListData, 6, UserData, 0, head);

        //                            WriteIndex = writeIndex;//记录当前位置

        //                            //OnReceived(IsThreadPool, IpPort, Client, UserData, Received);

        //                            if (WriteIndex == Count)
        //                            {
        //                                //MemoryData[..WriteIndex].Span.Clear();
        //                                WriteIndex = 0;
        //                                //SpareSize = DataLength;
        //                            }
        //                            else if (Count - WriteIndex > HeadSize)
        //                            {
        //                                //count = 0;
        //                                goto Verify;
        //                            }

        //                            //if (WriteIndex > 0)
        //                            //{
        //                            //    //byte[] NewData = new byte[WriteIndex];
        //                            //    //MemoryData[tophead..].CopyTo(NewData);

        //                            //    if (WriteIndex < tophead)
        //                            //    {
        //                            //        MemoryData[manytophead..].CopyTo(MemoryData);
        //                            //        MemoryData[WriteIndex..(WriteIndex + tophead)].Span.Clear();
        //                            //    }

        //                            //    //Array.Copy(ListData, tophead, NewData, 0, WriteIndex);
        //                            //    //Array.Clear(ListData, 0, WriteIndex + tophead);
        //                            //    //Array.Copy(NewData, 0, ListData, 0, WriteIndex);
        //                            //}
        //                            //else
        //                            //{
        //                            //    MemoryData[..tophead].Span.Clear();
        //                            //    //Array.Clear(ListData, 0, tophead);
        //                            //}
        //                        }
        //                        else
        //                        {
        //                            MemoryData[WriteIndex..Count].CopyTo(MemoryData);
        //                            WriteIndex = Count - WriteIndex;
        //                            //SpareSize = DataLength - WriteIndex;
        //                            //MemoryData[..(count - WriteIndex)].Span.Clear();
        //                            //doReceive.Set();
        //                            //return;
        //                        }
        //                    }
        //                }
        //                else
        //                {
        //                    Client.Close();
        //                }
        //            }
        //        }
        //        else
        //        {
        //            Received(MemoryData[..Count]);

        //            //UserData = new byte[Count];
        //            ////Array.Copy(ListData, 0, ListData, 0, count);
        //            ////Array.Clear(ListData, 0, count);
        //            //var memory = MemoryData[..Count];
        //            //memory.CopyTo(UserData);
        //            ////memory.Span.Clear();//无需清理
        //            WriteIndex = 0;
        //            //SpareSize = DataLength;

        //            //OnReceived(IsThreadPool, IpPort, Client, UserData, Received);
        //        }

        //        Count = -1;
        //    }
        //    catch (Exception ex)
        //    {
        //        ClientClose();
        //        Utils.Log.Error("解析异常：", ex, "Log/Tcp");
        //    }
        //}

        #endregion

        internal bool OnReceiveTask(ref Memory<byte> memory, ref bool isend)
        {
            if (isend) return false;
            static bool isTopHead(in Memory<byte> memory, out int tophead, out int count)
            {
                count = memory.Length;
                if (HeadSize >= count) { tophead = -1; return false; }
                int head = GetDataHeadTcp(memory.Span);
                if (head == -1) throw new NotSupportedException("与数据协议不一致，终止连接！");
                tophead = head + HeadSize;
                return count >= tophead;
            }
            try
            {
                var _memory = MemoryData.Slice(WriteIndex, Count);
                if (OnlyData)
                {
                    if (WriteHeap.IsSuccess)
                    {
                        EmptyBuffer();
                        return isend = true;
                    }
                    else if (isTopHead(in _memory, out int tophead, out int count))
                    {
                        if (tophead == count)
                        {
                            memory = _memory;
                            EmptyBuffer();
                            isend = true;
                        }
                        else
                        {
                            memory = _memory[..tophead];
                            EmptyBuffer(tophead);//记录当前位置
                            isend = false;
                        }
                        return true;
                    }
                    else if (tophead > DataLength) //判断包长大于最大消息体
                    {
                        WriteHeap = new(tophead, _memory);
                    }
                    else //特殊情况（收到的数据报小于数据头）判断当前包不够大
                    {
                        _memory.CopyTo(MemoryData);
                        WriteIndex = -1; //定义未完成的消息体
                    }
                }
                else
                {
                    memory = _memory;// MemoryData[..Count];
                    EmptyBuffer();
                    return isend = true;
                }
            }
            catch (Exception ex)
            {
                ClientClose();
                Utils.Log.Error("解析异常：", ex, "Log/Tcp");
            }
            return isend = false;
        }

        private void EmptyBuffer(int count = 0)
        {
            if (count == 0)
            {
                Count = count;
                WriteIndex = count;
            }
            else
            {
                Count -= count;
                WriteIndex += count;
            }
            //SpareSize = DataLength;
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
            WriteHeap.Dispose();
            MemoryData = null;
            //SpareSize = 0;
            WriteIndex = 0;
            Received = null;
            //doReceive?.Close();
        }
    }
}
