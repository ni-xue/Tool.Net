using System;
using System.Buffers;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipelines;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Tool.Sockets.Kernels;
using Tool.Utils;
using Tool.Utils.Data;

namespace Tool.Sockets.UdpHelper.Extend
{
    internal class UdpStream : IUdpCore
    {
        /// <summary>
        /// 保证数据完整性模式
        /// </summary>
        private readonly struct Pack
        {
            public Pack(uint sequenceId, in BytesCore bytesCore, Range range)
            {
                this.bytesCore = bytesCore;
                SequenceId = sequenceId;
                Position = range;
                CreateTime = DateTime.UtcNow;
            }

            private readonly BytesCore bytesCore;       //数据包信息（完整或不完整（分包））
            public readonly uint SequenceId;            //序列ID 唯一身份标识
            public readonly Range Position;             //标记当前包开始位和结束位
            public readonly DateTime CreateTime;        //创建时的时间

            public readonly Memory<byte> Memory => bytesCore.Memory;        //数据包信息（完整或不完整（分包））
            public readonly int Size => Memory.Length; //包大小
            public readonly bool IsTimeout(int replyDelay) => (DateTime.UtcNow - CreateTime).TotalMilliseconds > replyDelay;
            public readonly Pack Copy() => new(SequenceId, bytesCore, Position);
        }

        public const int ReserveSize = 18;
        public const byte Code00 = 00;
        public const byte Code01 = 10;
        public const byte Code02 = 20;
        public const byte Code03 = 30;

        /// <summary>
        /// 默认的缓冲区大小（用于确保在保证数据完整性模式下，滑动窗口合理设计）- 内网环境
        /// </summary>
        public const int IntranetBufferSizs = 1472; //MTU=1500 - 20(IP头) - 8(UDP头) = 1472(有效部分包大小)

        /// <summary>
        /// Internet的缓冲区大小（用于确保在保证数据完整性模式下，滑动窗口合理设计）- 公网环境
        /// </summary>
        public const int InternetBufferSizs = 548; //MTU=576 - 20(IP头) - 8(UDP头) = 548(有效部分包大小)

        private bool _dispose;
        private bool _isClose;
        private bool _loading; //描述（特定行为，当用户加入时标记身份）

        private uint packOrderCount = uint.MinValue; //大包ID
        private uint writeOrderCount = uint.MinValue;//传输ID

        #region 接收记录

        private uint readMinId = uint.MinValue;
        private uint readMaxId = uint.MinValue;
        private readonly Memory<byte> RepeatObj = new byte[] { 30, 0, 0, 0, 0, 31 }; //获取重发包流

        #endregion

        private readonly int adoptBufferSizs;
        private readonly bool isserver;
        private readonly bool isp2p;
        private readonly int replyDelay;
        private readonly INetworkCore networkCore;
        private readonly Func<UserKey, byte, ValueTask> complete;
        private readonly UdpStateObject udpState;//TaskCompletionSource
        //private readonly PipeWriter sendwriter;
        //private readonly PipeWriter receivewriter;
        private readonly ConcurrentDictionary<uint, Pack> packInfos; //发送数据包存根
        private readonly ActionBlock<ProtocolBody> block; //即时接收采用缓存集合
        private readonly KeepAlive Keep;
        //private readonly Task readertask;

        internal UdpStream(INetworkCore networkCore, UdpEndPoint endPoint, Socket socket, int dataLength, bool onlyData, int replyDelay, bool isserver, bool isp2p, Func<UserKey, byte, ValueTask> complete, ReceiveEvent<IUdpCore> received)
        {
            if (endPoint is null) throw new ArgumentException("endPoint 对象是空的！", nameof(endPoint));
            if (socket is null) throw new ArgumentException("socket 对象是空的！", nameof(socket));
            EndPoint = endPoint;
            Socket = socket;
            this.replyDelay = replyDelay < 20 ? 20 : replyDelay;
            this.isserver = isserver;
            this._loading = !isserver;
            this.isp2p = isp2p;
            this.networkCore = networkCore ?? throw new ArgumentNullException(nameof(networkCore));
            this.complete = complete ?? throw new ArgumentNullException(nameof(complete));
            adoptBufferSizs = IPAddress.IsLoopback(endPoint.Address) || TextUtility.IsPrivateNetwork(endPoint.Address) ? IntranetBufferSizs : InternetBufferSizs;

            udpState = new UdpStateObject(this, dataLength, onlyData, received);

            packInfos = new();
            block = new(ReceiveBlockAsync);

            //if (OnlyData)
            //{
            //    //尝试缓冲区模式发送数据
            //    Pipe pipeSend = new(new PipeOptions(minimumSegmentSize: adoptBufferSizs, pauseWriterThreshold: adoptBufferSizs * 100, resumeWriterThreshold: adoptBufferSizs * 50));
            //    sendwriter = pipeSend.Writer;
            //    SendPipeAsync(pipeSend.Reader);
            //}
            //使用缓冲区接收，防止在协议层丢包
            //Pipe pipeReceive = new(new PipeOptions(minimumSegmentSize: adoptBufferSizs, pauseWriterThreshold: adoptBufferSizs * 100, resumeWriterThreshold: adoptBufferSizs * 50));
            //receivewriter = pipeReceive.Writer;
            //ReceivePipeAsync(pipeReceive.Reader);

            Keep = new(100, SendTimeoutEvent);
        }

        /// <summary>
        /// 连接的设备地址信息
        /// </summary>
        public UdpEndPoint EndPoint { get; }

        /// <summary>
        /// 连接的对象（请勿脱离框架使用，避免出现各种未知异常）
        /// </summary>
        public Socket Socket { get; }

        /// <summary>
        /// 可用最大空间
        /// </summary>
        public int DataLength => udpState.DataLength;

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData => udpState.OnlyData;

        UdpStateObject IUdpCore.UdpState => udpState;

        bool IUdpCore.IsOnLine(int receiveTimeout) => !_isClose && udpState.IsOnLine(receiveTimeout);

        Memory<byte> IUdpCore.GetSendMemory(in SendBytes<IUdpCore> sendBytes, ref bool ispart, ref int i)
        {
            if (sendBytes.OnlyData != OnlyData)
            {
                throw new ArgumentException("与当前套接字（OnlyData）协议不一致！", nameof(sendBytes));
            }

            if (sendBytes.Length > adoptBufferSizs - StateObject.HeadSize)
            {
                //分包模式
                //方案一：6字节头+16字节GUID+2字节包编号=24字节必须
                //方案二：6字节头+16字节GUID+4字节包编号+4字节末尾编号=30字节必须 采纳 补充说明：GUID可以用原子计数代替，因当前协议属于连接必要协议故双方必须保持一致性 最终大小：18字节

                byte code;
                uint packId;
                int i0 = i + 1;
                int body = adoptBufferSizs - ReserveSize;       //最终可用内容大小
                int total = sendBytes.Length.Ceiling(body);     //最终发送多少个包
                uint orderCount = AddWriteOrderCount();         //每个包的唯一编号
                if (i is 0)                                     //首次使用
                {
                    code = Code01;
                    packId = packOrderCount.Increment();
                    BitConverter.TryWriteBytes(sendBytes.OnlyBytes.Span, packId);
                }
                else
                {
                    code = Code02;
                    packId = BitConverter.ToUInt32(sendBytes.OnlyBytes.Span);
                }
                Range range = i0 == total ? Range.StartAt(i * body) : new Range(i * body, i0 * body);
                Memory<byte> memory = sendBytes.Memory[range];  //可用大小

                BytesCore owner = new(memory.Length + ReserveSize);
                StateObject.SetDataHeadUdp(owner.Span, orderCount, 0, code); //一
                BitConverter.TryWriteBytes(owner.Span[6..], packId);         //二
                BitConverter.TryWriteBytes(owner.Span[10..], i0);            //三
                BitConverter.TryWriteBytes(owner.Span[14..], total);         //四
                memory.CopyTo(owner.Memory[ReserveSize..]);                  //五

                Pack pack = new(orderCount, owner, new Range(i0, total));
                if (!packInfos.TryAdd(orderCount, pack))
                {
                    throw new Exception("发送数据失败！无法分配新的消息ID");
                }

                if (i0 == total)
                {
                    ispart = false;
                }
                else
                {
                    ispart = true;
                    i = i0;
                }

                return owner.Memory;
            }
            else
            {
                //整包模式
                uint orderCount = AddWriteOrderCount();
                var memory = sendBytes.GetMemory(orderCount); //00
                BytesCore owner = new(memory.Length);
                owner.SetMemory(in memory);
                Pack pack = new(orderCount, owner, new Range(i, adoptBufferSizs + StateObject.HeadSize));
                if (!packInfos.TryAdd(orderCount, pack))
                {
                    throw new Exception("发送数据失败！无法分配新的消息ID");
                }
                return owner.Memory;
            }
        }

        internal async Task ShareSendAsync(SendBytes<IUdpCore> sendBytes, Func<IUdpCore, Memory<byte>, ValueTask> func)
        {
            if (sendBytes.OnlyData != true)
            {
                throw new ArgumentException("与当前套接字（OnlyData）协议不一致！", nameof(sendBytes));
            }

            //onlyData = true
            //需要实现保证数据传输保证成功
            //需要定义丢包重传，以及重传次数，重传超时时间等
            //需要在重传超过多少丢包后，发起断开连接行为
            //需要保证流式消息和表单消息均可以兼容使用
            //新版本
            //UdpState.GetMemory();

            var udp = sendBytes.Client;
            var udpState = udp.UdpState;
            uint orderCount = AddWriteOrderCount();

            var buffers = sendBytes.GetMemory(orderCount);

            if (sendBytes.OnlyData)
            {
                await udp.SendAsync(buffers);//写入带发送区域
            }
            else
            {
                await func(udp, buffers);
            }

            ////旧版本
            //await Task.Run(async () =>
            //{
            //    var point = sendBytes.Client;
            //    var udpState = point.UdpState;
            //    ushort orderCount = udpState.AddWriteOrderCount();

            //    var buffers = sendBytes.GetMemory(orderCount);
            //    int count = 1;
            //    if (replyDelay <= 0) replyDelay = 100;
            //    A:
            //    await func(point, buffers);
            //    if (!udpState.Wait(orderCount, replyDelay, ref count))
            //    {
            //        if (count == 10) { throw new Exception("发送失败：重试10次，无回应！"); }
            //        goto A;
            //    }
            //});
        }

        #region 公开接口

        /// <summary>
        /// 尝试关闭存在的连接
        /// </summary>
        public async Task CloseAsync()
        {
            if (!_isClose)
            {
                _isClose = true;
                await QuitMsg();
                if (isserver && networkCore is UdpServerAsync serverAsync)
                {
                    await serverAsync.ClientCloes(new KeyValuePair<UserKey, IUdpCore>(EndPoint.Ipv4, this));
                }
                else if (networkCore is INetworkConnect network)
                {
                    network.Close();
                }
            }

            //if (isserver)
            //{
            //    SpinWait.SpinUntil(() => !UdpStateObject.IsConnected(Socket)); //等待服务器关闭对应连接消息
            //}
            //else
            //{
            //    Socket.Close();
            //}
        }

        async Task IUdpCore.SendAsync(Memory<byte> memory)
        {
            await SendNoWaitAsync(memory);

            //Pipe
            //if (OnlyData && memory.Length > StateObject.HeadSize) //验证消息类型，有确认消息要提前发出
            //{
            //    try
            //    {
            //        FlushResult result = await sendwriter.WriteAsync(memory);

            //        if (result.IsCompleted)
            //        {
            //            throw new Exception("于对方建立的连接已断开！");
            //        }
            //    }
            //    catch
            //    {
            //        throw;
            //    }
            //}
            //else
            //{
            //    await SendNoWaitAsync(memory);
            //}

            udpState.UpDateSignal();
        }

        async Task IUdpCore.ReceiveAsync(Memory<byte> memory)
        {
            //var head = memory[..StateObject.HeadSize];
            //if (udpState.OnReceiveTask(head, memory.Length, out bool isreply, out bool isReceive))//尝试使用，原线程处理包解析，验证效率
            //{
            //    if (isreply) await SendNoWaitAsync(head);
            //    if (isReceive) await func(memory, udpState);
            //}

            //临时示例
            if (StateObject.GetDataHeadUdp(memory.Span, out ProtocolTop protocol))
            {
                if (memory.Length is StateObject.HeadSize) //处理确认收到的信号包
                {
                    if (protocol.IsClose)
                    {
                        await CloseAsync();
                    }
                    else if (protocol.IsRepeat)
                    {
                        if (packInfos.TryGetValue(protocol.OrderCount, out var pack))
                        {
                            await SendNoWaitAsync(pack.Memory); //补发
                        }
                    }
                    else
                    {
                        packInfos.TryRemove(protocol.OrderCount, out _); //回信销毁
                    }
                }
                else
                {
                    //验证是不是需要重发数据包？
                    //else
                    //{
                    await SendNoWaitAsync(memory[..StateObject.HeadSize]);
                    BytesCore owner = new(memory.Length);
                    owner.SetMemory(in memory);
                    if (!block.Post(new ProtocolBody(in protocol, in owner)))
                    {
                        throw new Exception("缓冲池已关闭！");
                    }
                    //}

                    //try
                    //{
                    //    FlushResult result = await receivewriter.WriteAsync(memory);

                    //    if (result.IsCompleted)
                    //    {
                    //        throw new Exception("于对方建立的连接已断开！");
                    //    }
                    //}
                    //catch
                    //{
                    //    throw;
                    //}
                }
            }
            else if ((isserver || isp2p) && udpState.IsKeepAlive(in memory))
            {
                await complete(udpState.IpPort, 0);
            }
        }

        void Dispose()
        {
            _dispose = true;
            Keep.Close();
            //sendwriter?.Complete();//new Exception("UDP连接已关闭！")
            //receivewriter.Complete();
            //readertask.Wait();
            //readertask.Dispose();
            udpState.Close();
            block.Complete();//标记任务已结束了
        }

        public async ValueTask DisposeAsync()
        {
            if (!_dispose)
            {
                await CloseAsync();
                Dispose();
                GC.SuppressFinalize(this);
            }
            //return ValueTask.CompletedTask;
        }

        #endregion

        private async ValueTask<int> SendNoWaitAsync(ReadOnlyMemory<byte> buffers)
        {
            if (!UdpStateObject.IsConnected(Socket)) throw new Exception(isserver ? "服务端已关闭！" : "与服务端的连接已中断！");
#if NET5_0
            return await Socket.SendToAsync(buffers.AsArraySegment(), SocketFlags.None, EndPoint);
#else
            return await Socket.SendToAsync(buffers, SocketFlags.None, EndPoint);
#endif
        }

        /// <summary>
        /// 接收数据包（带丢包验证）
        /// </summary>
        /// <param name="reader"></param>
        /// <returns></returns>
        private async void ReceivePipeAsync(PipeReader reader)
        {
            try
            {
                int position = 0;//默认位
                while (true)//running
                {
                    //等待writer写数据
                    ReadResult result = await reader.ReadAsync();
                    //获得内存区域
                    ReadOnlySequence<byte> buffer = result.Buffer;

                    if (OnlyData) //处理需要校验包顺序的
                    {

                    }
                    else //不处理，把文报按原样提交
                    {
                        //SequencePosition sequence = position > 0 ? buffer.GetPosition(position) : buffer.Start;
                        //while (buffer.TryGet(ref sequence, out var memory) && memory.IsEmpty is false)
                        //{
                        //    await SendNoWaitAsync(memory);

                        //    StringBuilder builder = new();
                        //    for (int i = 0; i < memory.Length; i++)
                        //    {
                        //        builder.Append($"{memory.Span[i]} ");
                        //    }
                        //    builder.Append($" {Socket.LocalEndPoint}->{Ipv4}");
                        //    Debug.WriteLine($"验证发出数据：{builder}");
                        //    //还未验证是否收到 ACK 消息
                        //    sequence = buffer.GetPosition(position + memory.Length); //buffer.End;
                        //    position += memory.Length;
                        //}
                    }

                    SequencePosition sequence = position > 0 ? buffer.GetPosition(position) : buffer.Start;
                    while (buffer.TryGet(ref sequence, out var memory) && memory.IsEmpty is false)
                    {
                        await SendNoWaitAsync(memory);

                        StringBuilder builder = new();
                        for (int i = 0; i < memory.Length; i++)
                        {
                            builder.Append($"{memory.Span[i]} ");
                        }
                        builder.Append($" {Socket.LocalEndPoint}->{EndPoint.Ipv4}");
                        Debug.WriteLine($"验证发出数据：{builder}");
                        //还未验证是否收到 ACK 消息
                        sequence = buffer.GetPosition(position + memory.Length); //buffer.End;
                        position += memory.Length;
                    }

                    //数据处理完毕，告诉pipe还剩下多少数据没有处理（数据包不完整的数据，找不到head）
                    reader.AdvanceTo(buffer.Start, buffer.End);

                    if (result.IsCompleted) break;
                }

                await reader.CompleteAsync();
            }
            catch (Exception ex)
            {
                if (ex.Source != "Tool.Net")
                {
                    Log.Error("UDP保证数据模式", ex);
                }
            }
        }

        private uint AddWriteOrderCount()
        {
            if (!OnlyData) return 0;
            if (SpinWait.SpinUntil(isOrder, IUdpCore.SpinWaitTimeout))//Timeout.Infinite
            {
                return writeOrderCount.Increment();//SpinWaitTimeout
            }
            throw new Exception("无法获取，新的序列号，滑动窗口长时间没有结果！");
            bool isOrder() => packInfos.Count < IUdpCore.LimitingSize;
        }

        uint IUdpCore.AddWriteOrderCount() => AddWriteOrderCount();

        private readonly Dictionary<uint, bool> cs = new();
        private readonly Dictionary<uint, Memory<BytesCore>> packList = new();
        private readonly Stopwatch stopWatch = new();
        private async Task ReceiveBlockAsync(ProtocolBody body)
        {
            await FirstLoading();
            await Task.Delay(networkCore.Millisecond);

            #region 补发验证
            try
            {
                const int MaxSeconds = 10;
                uint orderCount = body.Top.OrderCount;
                //标记消息顺序
                if (readMinId + 1 == orderCount)
                {
                    readMinId = orderCount;
                    if (readMinId > readMaxId) readMaxId = readMinId;
                    //验证是否需要缩编
                    if (cs.Remove(readMinId))
                    {
                        uint i = readMaxId + 1;
                        while (true)
                        {
                            if (cs.TryGetValue(i, out bool ok) && ok is true)
                            {
                                cs.Remove(i);
                                i++;
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                }
                else if (orderCount > readMaxId)
                {
                    readMaxId = orderCount;
                    //验证扩编范围
                    for (uint i = readMinId + 1; i <= readMaxId; ++i)
                    {
                        cs.TryAdd(i, i == readMaxId);
                    }
                    if (!stopWatch.IsRunning) stopWatch.Start();
                }
                else //收到的数据位于 两端的中间部分 思路无
                {
                    // readMaxId = body.Top.OrderCount;
                    cs[orderCount] = true;
                    //发范围内的数据包，不触发补发逻辑，并重置补发延迟
                    stopWatch.Restart();
                }

                //验证消息（判断是否需要告知对方补发消息）
                if (cs.Count > 3 && stopWatch.ElapsedMilliseconds > MaxSeconds)//因为一个已经接收到的尾包在其中
                {
                    foreach (var item in cs)
                    {
                        if (!item.Value)
                        {
                            StateObject.SetDataHeadUdp(RepeatObj.Span, item.Key, 0, 30);
                            await SendNoWaitAsync(RepeatObj);
                        }
                    }
                    stopWatch.Restart();
                }
            }
            catch (Exception ex)
            {
                Log.Error("补发模式：", ex, Path.Combine("Log","UdpStream"));
            }
            #endregion

            #region 数据整理块
            try
            {
                BytesCore bytesCore = body.Bytes;
                //核心业务
                if (body.Top.IsPart)//大于文报体的业务拆包了
                {
                    #region 凑整包模式
                    var memory = body.Bytes.Memory;
                    uint packId = BitConverter.ToUInt32(memory.Span[6..]);         //二
                    int i0 = BitConverter.ToInt32(memory.Span[10..]);              //三
                    int i1 = BitConverter.ToInt32(memory.Span[14..]);              //四
                                                                                   //if (body.Top.IsTop) {} else {}
                    if (packList.TryGetValue(packId, out var msg))
                    {
                        msg.Span[i0 - 1] = bytesCore;
                        for (int i = 0; i < msg.Span.Length; i++)
                        {
                            if (msg.Span[i].IsEmpty)
                            {
                                return;
                            }
                        }
                        //成功走到这一步，表示已完整获取了所有数据包，可以进行整包下发了
                        int bodyLength = adoptBufferSizs - ReserveSize;       //最终可用内容大小
                        BytesCore owner = new(bodyLength * msg.Length + StateObject.HeadSize);
                        for (int i = 0; i < msg.Length; i++)
                        {
                            using BytesCore core = msg.Span[i];
                            if (i is 0) owner.SetMemory(memory.Slice(StateObject.HeadSize, 4), 1);//补一个任务ID
                            Memory<byte> bytes = core.Memory[ReserveSize..];
                            owner.SetMemory(in bytes, bodyLength * i + StateObject.HeadSize);
                        }
                        bytesCore = owner;//覆盖原值
                        goto A;
                    }
                    else
                    {
                        Memory<BytesCore> bytesCores = new BytesCore[i1]; //初始化时创建默认空间
                        bytesCores.Span[i0 - 1] = bytesCore;
                        packList.Add(packId, bytesCores);
                    }
                    return;
                    #endregion
                }

                A:
                //小包业务，数据小于文报体
                await complete(udpState.IpPort, 1);
                await udpState.OnReceive(networkCore.IsThreadPool, bytesCore);
            }
            catch (Exception ex)
            {
                Log.Error("合包模式：", ex, Path.Combine("Log", "UdpStream"));
            }
            #endregion
        }

        private async Task SendTimeoutEvent()
        {
            var datas = from a in packInfos.Values where a.IsTimeout(replyDelay) select a;
            foreach (var pack in datas)
            {
                if (packInfos.TryUpdate(pack.SequenceId, pack.Copy(), pack))
                {
                    await SendNoWaitAsync(pack.Memory);
                }
            }
        }

        private async ValueTask FirstLoading()
        {
            if (!_loading)
            {
                _loading = true;
                await complete(udpState.IpPort, 2);
            }
        }

        private async Task QuitMsg()
        {
            try
            {
                if (UdpStateObject.IsConnected(Socket))
                {
                    Memory<byte> msg = RepeatObj.ToArray();//拷贝副本确保数据有效
                    StateObject.SetDataHeadUdp(msg.Span, 0, 0, 255);
                    await SendNoWaitAsync(msg);
                    if (!isserver)
                    {
                        Socket.Dispose();
                    }
                }
            }
            catch (Exception)
            {
                //需要抛弃错误
            }
        }
    }
}
