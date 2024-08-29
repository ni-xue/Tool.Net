using System;
using System.Buffers;
using System.Linq.Expressions;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Tool.Utils;

namespace Tool.Sockets.Kernels
{
    internal readonly struct ProtocolBody
    {
        public ProtocolBody(in ProtocolTop top, in BytesCore bytes)
        {
            Top = top;
            Bytes = bytes;
        }

        public readonly ProtocolTop Top;
        public readonly BytesCore Bytes;
    }

    internal readonly struct ProtocolTop
    {
        public ProtocolTop(uint orderCount, bool isPart, bool isTop, bool isRepeat, bool isClose)
        {
            OrderCount = orderCount;
            IsPart = isPart;
            IsTop = isTop;
            IsRepeat = isRepeat;
            IsClose = isClose;
        }

        public readonly uint OrderCount;
        public readonly bool IsPart;
        public readonly bool IsTop;
        public readonly bool IsRepeat;
        public readonly bool IsClose;
    }

    /// <summary>
    /// Socket接收数据委托
    /// </summary>
    /// <typeparam name="T">连接对象</typeparam>
    /// <param name="age0">数据包</param>
    /// <returns><see cref="ValueTask"/></returns>
    public delegate ValueTask ReceiveEvent<T>(ReceiveBytes<T> age0);

    /// <summary>
    /// Socket事件委托
    /// </summary>
    /// <typeparam name="T">连接对象</typeparam>
    /// <param name="age0">事件key</param>
    /// <param name="age1">事件枚举</param>
    /// <param name="age2">发生时间</param>
    /// <returns><see cref="ValueTask"/></returns>
    public delegate ValueTask CompletedEvent<T>(UserKey age0, T age1, DateTime age2) where T : Enum;

    /// <summary>
    /// Socket IpPort 解释器委托
    /// </summary>
    /// <param name="age0">带验证信息</param>
    /// <param name="age1">发送者信息</param>
    /// <returns>有效的<see cref="Ipv4Port"/></returns>
    public delegate ValueTask<Ipv4Port> IpParserEvent(Ipv4Port age0, Ipv4Port age1);

    /// <summary>
    /// 通信公共基础类
    /// </summary>
    public abstract class StateObject
    {

        private static Func<Socket, bool> socketDisposed;

        /// <summary>
        /// 默认IP信息
        /// </summary>
        protected static readonly Ipv4Port EmptyIpv4Port = new(new byte[] { 0, 0, 0, 0, 0, 0 });

        /// <summary>
        /// 用于提供锁服务
        /// </summary>
        internal static readonly object Lock = new();

        /// <summary>
        /// 默认大小
        /// </summary>
        public const int HeadSize = 6;

        /// <summary>
        /// 获取持久连接协议
        /// </summary>
        public Span<byte> KeepAliveObj => KeepAlive.KeepAliveObj.Span;

        /// <summary>
        /// 创建用于连接的 <see cref="Socket"/> 对象
        /// </summary>
        /// <param name="isTcp">是否是Tcp</param>
        /// <param name="bufferSize">缓冲区枚举</param>
        /// <returns><see cref="Socket"/> 对象</returns>
        public static Socket CreateSocket(bool isTcp, NetBufferSize bufferSize)
        {
            SocketType socketType;
            ProtocolType protocolType;
            if (isTcp)
            {
                socketType = SocketType.Stream;
                protocolType = ProtocolType.Tcp;
            }
            else
            {
                socketType = SocketType.Dgram;
                protocolType = ProtocolType.Udp;
            }
            return new(AddressFamily.InterNetwork, socketType, protocolType)
            {
                ReceiveBufferSize = (int)bufferSize,
                SendBufferSize = (int)bufferSize
            };
        }

        /// <summary>
        /// 获取<see cref="Socket"/> Disposed 属性
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        public static bool SocketIsDispose(Socket socket)
        {
            if (Interlocked.CompareExchange(ref socketDisposed, null, null) == null)
            {
                var Disposed = typeof(Socket).GetProperty("Disposed", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
                ParameterExpression parameter = Expression.Parameter(typeof(Socket), "callclass");
                MethodCallExpression methodCall = Expression.Call(parameter, Disposed.GetMethod);
                socketDisposed = Expression.Lambda<Func<Socket, bool>>(methodCall, parameter).Compile();
            }
            return socketDisposed.Invoke(socket);
        }

        /// <summary>
        /// 根据字节获取哈希值
        /// </summary>
        /// <param name="bytes">字节</param>
        /// <returns>哈希值</returns>
        public static unsafe int HashCodeByte(in Memory<byte> bytes)
        {
            HashCode code = new();
            using (MemoryHandle handle = bytes.Pin())
            {
                byte* bytenint = (byte*)handle.Pointer;
                for (int i = 0; i < bytes.Length; i++) code.Add(bytenint[i]);
            }
            return code.ToHashCode();

            //Span<byte> span = bytes.Span;
            //ref byte reference = ref MemoryMarshal.GetReference(span);
            //void* voidnint = Unsafe.AsPointer(ref reference);
            //byte* bytenint = (byte*)voidnint;
            //HashCode code = new();
            //for (int i = 0; i < span.Length; i++) code.Add(bytenint[i]);
            ////int j = reference;
            ////for (int i = 1; i < span.Length; i++)
            ////{
            ////    j ^= Unsafe.Add(ref reference, i);
            ////}
            //return code.ToHashCode();
        }

        internal static Thread StartTask(string name, Func<Task> action)
        {
            Thread thread = new(TaskStartReceive)
            {
                Name = $"{name}线程",
                IsBackground = true,
                Priority = ThreadPriority.Lowest
            };
            thread.Start(action);
            return thread;

            static void TaskStartReceive(object obj)
            {
                if (obj is Func<Task> action)
                {
                    try
                    {
                        action.Invoke().Wait();
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal("重连线程崩溃：", ex, "Log/Net");
                    }
                }
            }
        }

        internal static Thread StartReceive<T>(string name, Func<T, Task> action, T client)
        {
            Thread thread = new(TaskStartReceive)
            {
                Name = $"{name}接收线程",
                IsBackground = true,
                Priority = ThreadPriority.AboveNormal
            };
            thread.Start(new object[] { action, client });
            return thread;

            static void TaskStartReceive(object obj)
            {
                if (obj is object[] objs && objs[0] is Func<T, Task> action && objs[1] is T client)
                {
                    try
                    {
                        action.Invoke(client).Wait();
                    }
                    catch (Exception ex)
                    {
                        Log.Fatal("通信Core线程崩溃：", ex, "Log/Net");
                    }
                }
            }
        }

        /// <summary>
        /// 任务事件线程池
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="receive">委托</param>
        /// <param name="data">数据</param>
        public static void QueueUserWorkItem<T>(ReceiveEvent<T> receive, ReceiveBytes<T> data)
        {
#if true
            A:
            try
            {
                ThreadPool.UnsafeQueueUserWorkItem(ReceivedPool, data, false);
            }
            catch (OutOfMemoryException)
            {
                Thread.Sleep(100); //无法排队时，等待排队
                goto A;
            }

            void ReceivedPool(ReceiveBytes<T> receiveBytes)
            {
                var task = receive.Invoke(receiveBytes).AsTask();
                if (task.IsFaulted)
                {
                    receiveBytes.Dispose();//崩溃时以防万一，帮助回收。
                    Log.Fatal("公共线程池任务崩溃：", task.Exception.InnerException, "Log/Net");
                }
            }
#else
            Task.Run(task);
            Task task() => receive.Invoke(data).AsTask();
#endif
        }

        /// <summary>
        /// 任务事件单实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="receive">委托</param>
        /// <param name="data">数据</param>
        /// <returns>任务结果</returns>
        public static async ValueTask ReceivedAsync<T>(ReceiveEvent<T> receive, ReceiveBytes<T> data)
        {
            try
            {
                await receive.Invoke(data);
            }
            catch (Exception ex)
            {
                data.Dispose();
                throw ex.InnerException; //只要子级错误
            }
        }

        /**
         * 给包加头保证其数据完整性
         * headby 数据头
         */
        internal static int GetDataHeadTcp(in ReadOnlySpan<byte> headby)
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

        /**
         * 给包加头保证其数据完整性
         * headby 数据头
         */
        internal static bool GetDataHeadUdp(in ReadOnlySpan<byte> headby, out ProtocolTop protocol)//, ref ushort count
        {
            switch (headby[0])
            {
                case 00 when headby[5] == 01:
                    protocol = new(BitConverter.ToUInt32(headby[1..]), false, false, false, false);
                    return true;
                case 10 when headby[5] == 11:
                    protocol = new(BitConverter.ToUInt32(headby[1..]), true, true, false, false);
                    return true;
                case 20 when headby[5] == 21:
                    protocol = new(BitConverter.ToUInt32(headby[1..]), true, false, false, false);
                    return true;
                case 30 when headby[5] == 31:
                    protocol = new(BitConverter.ToUInt32(headby[1..]), false, false, true, false);
                    return true;
                case 255 when headby[5] == 255:
                    protocol = new(0, false, false, false, true);
                    return true;
                default:
                    protocol = default;
                    return false;
            }
        }

        internal static ArraySegment<byte> GetDataSend(int listDatalength, int datalength)
        {
            if (listDatalength + HeadSize > datalength) throw new Exception("发送数据超过设置大小，请考虑分包发送！");
            ArraySegment<byte> Data = new byte[HeadSize] { 40, 0, 0, 0, 0, 41 };
            BitConverter.TryWriteBytes(Data[1..], listDatalength);
            return Data;
        }

        /**
        * 给包加头保证其数据完整性(内置)
        * listData 数据
        * datalength 限制数据量
        */
        internal static void SetDataHeadTcp(in Span<byte> listData, int datalength, int start)
        {
            listData[start++] = 40;
            BitConverter.TryWriteBytes(listData[start..], datalength);
            listData[start + 4] = 41;
            //BitConverter.TryWriteBytes(start > 0 ? listData[start..] : listData, datalength);
        }

        /**
       * 给包加头保证其数据完整性(内置)
       * listData 数据
       * datalength 限制数据量
       */
        internal static void SetDataHeadUdp(in Span<byte> listData, uint orderCount, int start = 0, byte code = 00)
        {
            ref byte a = ref listData[start];
            ref byte b = ref listData[start + 5];
            switch (code)
            {
                case 00:
                    a = 00;
                    b = 01;
                    break;
                case 10:
                    a = 10;
                    b = 11;
                    break;
                case 20:
                    a = 20;
                    b = 21;
                    break;
                case 30:
                    a = 30;
                    b = 31;
                    break;
                case 255:
                    a = 255;
                    b = 255;
                    break;
                default:
                    throw new Exception("未知代号，无效协议！");
            }
            BitConverter.TryWriteBytes(listData[++start..], orderCount);
        }

        /**
        * 给内存地址写入IP信息(内置)
        * listData 数据
        * IpPort IP信息
        */
        internal static void SetIpPort(in Span<byte> SpanByte, Ipv4Port IpPort)
        {
            IpPort.CopyTo(SpanByte);

            //System.Buffers.Binary.BinaryPrimitives.TryWriteInt64LittleEndian(SpanByte, IpPort);

            //BitConverter.TryWriteBytes(SpanByte, IpPort);

            //IPEndPoint s1 = IPEndPoint.Parse(IpPort);
            //s1.Address.TryWriteBytes(SpanByte, out int size);
            //BitConverter.TryWriteBytes(SpanByte[size..], (ushort)s1.Port);
        }

        /// <summary>
        /// 根据HttpListenerContext获取IP加端口
        /// </summary>
        /// <param name="Context"></param>
        /// <returns></returns>
        public static Ipv4Port GetIpPort(HttpListenerContext Context)
        {
            if (Context is not null && Context.Request is not null && Context.Request.RemoteEndPoint is not null)
            {
                return GetIpPort(Context.Request.RemoteEndPoint);
            }
            return EmptyIpv4Port;
        }

        /// <summary>
        /// 根据Socket获取IP加端口
        /// </summary>
        /// <param name="Client"></param>
        /// <returns></returns>
        public static Ipv4Port GetIpPort(Socket Client)
        {
            if (Client is not null && Client.RemoteEndPoint is not null)
            {
                return GetIpPort(Client.RemoteEndPoint);
            }
            return EmptyIpv4Port;
        }


        /// <summary>
        /// 根据EndPoint获取IP加端口
        /// </summary>
        /// <param name="iep"></param>
        /// <returns></returns>
        public static Ipv4Port GetIpPort(EndPoint iep)//IPEndPoint
        {
            try
            {
                if (iep is not null && iep.AddressFamily == AddressFamily.InterNetwork)
                {
                    //SocketAddress address = iep.Serialize();
                    //return $"{address[4]}.{address[5]}.{address[6]}.{address[7]}:{BitConverter.ToUInt16(new byte[] { address[3], address[2] })}";
                    return iep.ToString();
                }
            }
            catch (Exception) { }
            return EmptyIpv4Port;
        }

        /// <summary>
        /// 根据传入字符串验证是否是IP加端口
        /// </summary>
        /// <param name="IpPort">IP+端口</param>
        /// <param name="ipnum"></param>
        /// <returns></returns>
        public static unsafe bool IsIpPort(string IpPort, out Ipv4Port ipnum)
        {
            ReadOnlySpan<char> chars = IpPort.AsSpan();
            if (!chars.IsEmpty) //IpPort != null !string.IsNullOrEmpty(IpPort)
            {
#if false
                int ipportLength = chars.Length, old = 0, fresh = 0, position = 0;//, ip0 = -1, ip1 = -1, ip2 = -1, lastport = -1; //chars.LastIndexOf(':'); 21
                byte[] bytes = new byte[6];

                static bool isip(char* check, in int position, ref bool isend) => position == 3 ? check->Equals(':') && (isend = true) : check->Equals('.');
                fixed (char* ptr = &MemoryMarshal.GetReference(chars))
                {
                    fixed (byte* ptr0 = bytes)
                    {
                        bool isend = false;
                        for (; fresh < ipportLength; fresh++)
                        {
                            char* check = &ptr[fresh];
                            if (isip(check, in position, ref isend))
                            {
#if false
                                if (!TryByte<byte>(&ptr0[position], ptr, ref old, ref fresh)) break;
#else
                                if (!byte.TryParse(chars[old..fresh], NumberStyles.None, CultureInfo.InvariantCulture, out bytes[position])) break;
                                old = fresh + 1;
#endif

                                position++;
                                if (isend)
                                {
#if false
                                    fresh = ipportLength;
                                    if (TryByte<ushort>(&ptr0[position], ptr, ref old, ref fresh))
                                    {
                                        ipnum = new(bytes);
                                        return true;
                                    }
#else
                                    if (ushort.TryParse(chars[old..], NumberStyles.None, CultureInfo.InvariantCulture, out ushort port) /*&& BitConverter.TryWriteBytes(bytes.AsSpan(position, 2), port)*/)
                                    {
                                        Unsafe.Write(&ptr0[position], port);
                                        ipnum = new(bytes);
                                        return true;
                                    }
#endif
                                }
                            }
                        }
                    }
                }
#elif false
                int position = 0;
                byte[] bytes = new byte[6];
                while (true)
                {
                    switch (position)
                    {
                        case 0:
                        case 1:
                        case 2:
                            int fresh = chars.IndexOf('.');
                            if (fresh == -1) goto A;
                            B:
                            if (!byte.TryParse(chars[..fresh], System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out bytes[position])) goto A;
                            chars = chars[(fresh + 1)..];
                            position++;
                            break;
                        case 3:
                            fresh = chars.IndexOf(':');
                            if (fresh == -1) goto A;
                            goto B;
                        default:
                            if (ushort.TryParse(chars, System.Globalization.NumberStyles.None, System.Globalization.CultureInfo.InvariantCulture, out ushort port) /*&& BitConverter.TryWriteBytes(bytes.AsSpan(position, 2), port)*/)
                            {
                                fixed (byte* ptr = bytes) Unsafe.Write(&ptr[position], port);
                                ipnum = new(bytes);
                                return true;
                            }
                            goto A;
                    }
                }
            A:;
#else
                if (chars.Contains('.') && IPEndPoint.TryParse(chars, out var result) && result.AddressFamily is AddressFamily.InterNetwork)
                {
                    Memory<byte> bytes = new byte[6];
                    Span<byte> span = bytes.Span;
                    if (result.Address.TryWriteBytes(span, out var bytesWritten) && BitConverter.TryWriteBytes(span[bytesWritten..], (ushort)result.Port))
                    {
                        //fixed (byte* ptr = span) Unsafe.Write(&ptr[bytesWritten], result.Port);
                        ipnum = new(in bytes);
                        return true;
                    }
                }
#endif
            }

            ipnum = Ipv4Port.Empty;
            return false;
        }

        private static unsafe bool TryByte<T>(void* destination, char* chars, ref int old, ref int fresh) where T : struct
        {
            int size = fresh - old, result = 0, sizeOf = Unsafe.SizeOf<T>();
            if (sizeOf == 1)
            {
                if (size > 3) return false;
                if (!ToSize(chars, ref old, ref fresh, ref result)) return false;
                if (result is not >= byte.MinValue or not <= byte.MaxValue) return false;//destination[0] = (byte)result;//Unsafe.As<byte, byte*>(ref );
                old = fresh + 1;
            }
            else
            {
                if (size > 5) return false;
                if (!ToSize(chars, ref old, ref fresh, ref result)) return false;
                if (result is not >= ushort.MinValue or not <= ushort.MaxValue) return false;
                //Span<byte> bytes = new(destination, 5); //Unsafe.AsPointer(ref destination);
                //BitConverter.TryWriteBytes(bytes, Unsafe.As<int, ushort>(ref result));
            }

            Unsafe.Write(destination, Unsafe.As<int, T>(ref result));

            //if (chars.Length > 3) return false;
            //char size = chars[position];
            //int i = GetSize(size);
            //if (i == -1) return false;

            //val = (byte)(val >> i);

            //position--;
            return true;

            static byte GetSize(char size)
            {
                return size switch
                {
                    '0' => 0,
                    '1' => 1,
                    '2' => 2,
                    '3' => 3,
                    '4' => 4,
                    '5' => 5,
                    '6' => 6,
                    '7' => 7,
                    '8' => 8,
                    '9' => 9,
                    _ => 255,
                };
            }

            static void SetSize(int location, byte val, ref int result)
            {
                int number = location switch
                {
                    1 => val,
                    2 => val * 10,
                    3 => val * 100,
                    4 => val * 1000,
                    5 => val * 10000,
                    _ => -1,
                };
                result += number;
            }

            static bool ToSize(char* chars, ref int old, ref int fresh, ref int result)
            {
                for (int j = old; j < fresh; j++)
                {
                    byte val = GetSize(chars[j]);
                    if (val == 255) return false;
                    SetSize(fresh - j, val, ref result);
                }
                return true;
            }
        }
    }
}
