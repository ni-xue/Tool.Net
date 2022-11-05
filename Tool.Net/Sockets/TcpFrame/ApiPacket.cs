using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using Tool.Sockets.SupportCode;
using Tool.Utils;
using Tool.Utils.Data;

namespace Tool.Sockets.TcpFrame
{
    /// <summary>
    /// 接口请求数据包
    /// </summary>
    public class ApiPacket
    {
        /// <summary>
        /// 该字段默认 为true, 出现这个字段的本意是 作者认为， 通知都在线程池中操作 用 同步方案 好像很合理。
        /// <para></para>
        /// 但是实际情况是 好像 IO 线程 可以帮忙 所以默认是 启用异步通讯，可以根据自己的实际效果而定。
        /// <para></para>
        /// 这里设置成 true, 默认用全用异步发送，设置为false 将根据请求 类型 选择相对于的方式
        /// </summary>
        public static bool TcpAsync = true;

        /// <summary>
        /// 数据包初始化
        /// </summary>
        /// <param name="ClassID">类ID</param>
        /// <param name="ActionID">方法ID</param>
        public ApiPacket(byte ClassID, byte ActionID) : this(ClassID, ActionID, 60 * 1000)
        {
        }

        /// <summary>
        /// 数据包初始化
        /// </summary>
        /// <param name="ClassID">类ID</param>
        /// <param name="ActionID">方法ID</param>
        /// <param name="Millisecond">请求等待的毫秒</param>
        public ApiPacket(byte ClassID, byte ActionID, int Millisecond)
        {
            this.ClassID = ClassID;
            this.ActionID = ActionID;
            this.Millisecond = Millisecond;
            Data = new Dictionary<string, string>();
        }

        /// <summary>
        /// 请求的类ID
        /// </summary>
        public byte ClassID { get; }

        /// <summary>
        /// 请求的方法ID
        /// </summary>
        public byte ActionID { get; }

        /// <summary>
        /// 默认等待超时时间为60秒
        /// </summary>
        public int Millisecond { get; }

        /// <summary>
        /// 当前消息携带的数据流
        /// </summary>
        public byte[] Bytes { get; set; } = null;

        /**
         * 发送的参数
         */
        internal readonly Dictionary<string, string> Data;// { get; set; }

        /// <summary>
        /// 加入数据（如果有则修改）
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值（支持传输转义）</param>
        public void Set(string key, object value)
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new FormatException("当前key的值，不能为空。");
            }

            char c = key[0];

            if (!('A' <= c && c <= 'Z' || 'a' <= c && c <= 'z'))
            {
                throw new FormatException("当前key的值不符合参数名的定义，请以首字母定义。");
            }

            var val = string.Concat(value);
            if (!Data.TryAdd(key, val))
            {
                Data[key] = val;
            }

            //if (key.Contains('=') || key.Contains('&'))
            //{
            //    throw new FormatException("当前key的值存在【=或&】符号，需要转义后再加入。");
            //}

            //var val = value.ToString();

            //if (val.Contains('=') || val.Contains('&'))
            //{
            //    throw new FormatException("当前value的值存在【=或&】符号，需要转义后再加入。");
            //}

            //if (value.GetType().IsType())
            //{
            //    var val = value ?? value.ToString();
            //    if (Data.ContainsKey(key))
            //    {
            //        Data[key] = val;
            //    }
            //    else
            //    {
            //        Data.Add(key, val);
            //    }
            //}
            //else
            //{
            //    throw new FormatException("当前value的值，不是系统变量值，无法发送。");
            //}
        }

        /// <summary>
        /// 加入数据,如果有则修改（以虚构对象参数传入，请确保已认真读注释。）（支持传输转义）
        /// </summary>
        /// <param name="dictionary">虚构对象</param>
        public void Set(object dictionary)
        {
            var _objs = dictionary.GetDictionary();

            foreach (var item in _objs)
            {
                string key = item.Key;

                object value = item.Value;

                Set(key, value);
            }
        }

        /// <summary>
        /// 获取键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">要返回的值</param>
        /// <returns>是否存在</returns>
        public bool TryGet(string key, out string value)
        {
            return Data.TryGetValue(key, out value);
        }

        /// <summary>
        /// 从发送数据中移除所指定的键的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>是否成功移除</returns>
        public bool Remove(string key)
        {
            return Data.Remove(key);
        }

        /**
         * 返回 Format 表单提交
         */
        internal string FormatData()
        {
            //var s = Data is null
            //    ? string.Empty
            //    : string.Join("&",
            //        Data.Select(d => string.Concat(d.Key, "=", d.Value)));
            return HttpHelpers.QueryString(Data);
        }
    }

    /**
     * 内部包
     */
    internal class ThreadObj : IDisposable
    {
        private ManualResetEvent Reset;

        public ThreadObj(string onlyID)
        {
            Reset = new ManualResetEvent(false);
            Response = new TcpResponse(onlyID);
        }

        public TcpResponse Response { get; private set; }

        public bool Set() => Reset.Set();

        public bool WaitOne(int Millisecond) => Reset.WaitOne(Millisecond, true);

        /**
         * 回收资源
         */
        public void Dispose()
        {
            if (Reset is null) return;
            Reset.Close();
            Reset.Dispose();
            Reset = null;
            Response = null;
            GC.SuppressFinalize(this);
        }
    }

    /**
     * 内部多包处理
     */
    internal class TcpByteObjs //: IDisposable
    {
        public TcpByteObjs(int count)
        {
            //_lock = new();
            Count = count;
            OjbCount = new ArraySegment<byte>[Count];
            Length = 0;
        }

        //public readonly object _lock; //一个锁，保证其在线程中的安全

        public int Count { get; set; }

        public int Length { get; set; }

        public ArraySegment<byte>[] OjbCount { get; private set; }

        //public void Dispose()
        //{
        //    if (this.OjbCount is not null)
        //    {
        //        // 请将清理代码放入下面
        //        this.Length = 0;
        //        this.Count = 0;
        //        this.OjbCount = null;
        //        GC.SuppressFinalize(this);
        //    }
        //}
    }

    internal class FrameCommon 
    {
        ///**
        // * 返回可查找的方法键值
        // */
        //internal static string GetOnlyID(int ClassID, int ActionID)
        //{
        //    //string ClMID = string.Empty;

        //    //int ManagedThreadId = ObjectExtension.Thread.ManagedThreadId;
        //    string guid = StringExtension.GetGuid();
        //    string clmidmt = string.Concat(guid, ClassID.ToString(), ".", ActionID.ToString()); //$"{ClMID}/{ManagedThreadId}";.Substring(10,21)
        //    return clmidmt;
        //}

        internal static DataPacket GetDataPacket(ApiPacket api, string ipPort, bool isServer, bool isAsync) //bool isSend, bool isErr, 
        {
            if (api == null) throw new ArgumentException(" ApiPacket 对象不能为空！", nameof(api));

            string msg = api.FormatData();
            DataPacket dataPacket = new()
            {
                ClassID = api.ClassID,
                ActionID = api.ActionID,
                OnlyId = Guid.NewGuid(),
                //OnlyID = clmidmt,
                Bytes = api.Bytes,
                IsSend = true,
                IsErr = false,
                IsServer = isServer,
                IsAsync = isAsync,
                //IsIpIdea = isIpPort,
                IpPort = ipPort,
                Text = msg,
            };

            return dataPacket;
        }

        /// <summary>
        /// 验证并确保包100%完整
        /// </summary>
        /// <param name="isSorC">验证差异</param>
        /// <param name="packet">原始包</param>
        /// <returns></returns>
        internal static bool IsComplete(bool isSorC, ref DataPacket packet)
        {
            if (packet.IsServer == isSorC)// 验证差异 服务端客户端 差异
            {
                packet.Dispose();
                return false;
            }

            if (packet.NotIsMany || packet.IsIpIdea && !packet.IsServer)
            {
                return true;
            }
            else
            {
                string OnlyID = packet.OnlyID;
                int objcount = packet.Many.End.Value;

                //if (packet.IsSend)
                //{
                //    int count = packet.Many.End.Value;
                //    _threadObj = StaticData.TcpByteObjs.GetOrAdd(OnlyID, a => new TcpByteObjs(count));
                //}
                //else if (!StaticData.TcpByteObjs.TryGetValue(OnlyID, out _threadObj))
                //{
                //    packet.Dispose();
                //    return false;
                //}

                TcpByteObjs _byteObjs = StaticData.TcpByteObjs.GetOrAdd(OnlyID, a => new TcpByteObjs(objcount));

                //lock (_byteObjs._lock)
                //{
                _byteObjs.OjbCount[packet.Many.Start.Value] = packet.Bytes;
                _byteObjs.Length += packet.Bytes.Count;
                _byteObjs.Count--;

                if (_byteObjs.Count == 0)
                {
                    // Memory<>
                    ArraySegment<byte> bytes = new byte[_byteObjs.Length];
                    int count = 0; //计数用于处理多包叠加
                    for (int i = 0; i < _byteObjs.OjbCount.Length; i++)
                    {
                        count += i > 0 ? _byteObjs.OjbCount[i - 1].Count : 0;
                        _byteObjs.OjbCount[i].CopyTo(bytes[count..]);
                    }

                    int length = TcpStateObject.GetDataHead(bytes);//(bytes[0..TcpStateObject.HeadSize]);

                    if (length > 0)//这里为处理问题
                    {
                        //packet.Text = Encoding.UTF8.GetString(bytes, 6, length);
                        packet.TextBytes = bytes.Slice(TcpStateObject.HeadSize, length).ToArray();
                        int bytelength = TcpStateObject.HeadSize + length;
                        if (bytes.Count > bytelength) packet.Bytes = bytes[bytelength..].ToArray();//完全拷贝
                    }
                    else
                    {
                        packet.Bytes = bytes;
                    }
                    packet.EmptyMany();
                    //if (packet.IsSend)
                    //{
                    //    StaticData.TcpByteObjs.TryRemove(OnlyID, out _);
                    //}
                    StaticData.TcpByteObjs.TryRemove(OnlyID, out _);
                    //_byteObjs.Dispose();
                    return true;
                }
                packet.Dispose();
                //}

                return false;
            }
        }

        /// <summary>
        /// 获取持久连接协议
        /// </summary>
        /// <returns></returns>
        internal static byte[] KeepAlive => new byte[] { 123, 1, 2, 3, 2, 1, 123 };

        /**
         * 根据一次性回复数据包解析成一个或多个包
         */
        internal static bool Receiveds(TcpBytes tcpBytes, out DataPacket dataPacket)
        {
            try
            {
                var spans = tcpBytes.Memory.Span;
                if (spans[0] == KeepAlive[0])
                {
                    dataPacket = DataPacket.DataByte(spans);
                    return true;
                }
                dataPacket = default;
                return false;
            }
            catch (Exception ex)
            {
                Log.Error("消息解包异常", ex, "Log/TcpFrame");
                dataPacket = default;
                return false;
            }
            finally
            {
                tcpBytes.Dispose();
            }
        }

        //**
        // * 根据一次性回复数据包解析成一个或多个包
        // */
        //internal static void Receiveds(string key, string jsonstr, Action<string, DataPacket> action)
        //{
        //    //string data1 = "0";
        //    //if (jsonstr[0] == '{')
        //    //{
        //    //    for (int i = 1; i < 10; i++)
        //    //    {
        //    //        if (jsonstr[i] == '}' && jsonstr[i + 1] == '[' && jsonstr[i + 2] == '#')
        //    //        {
        //    //            data1 = jsonstr.Substring(1, i - 1);
        //    //            break;
        //    //        }
        //    //    }
        //    //    int length1 = data1.ToInt();

        //    //    string data2 = jsonstr.Substring(2 + data1.Length, length1 - 2);

        //    //    char random1 = data2[2];
        //    //    int length2 = data2.Length;
        //    //    if (data2[length2 - 3] == random1 && data2[length2 - 2] == '#' && data2[length2 - 1] == ']')
        //    //    {
        //    //        DataPacket json = DataPacket.DataString(data2);
        //    //        action(key, json);
        //    //    }
        //    //    return;
        //    //}
        //    if (jsonstr[0] != '[' && jsonstr[1] != '#') return;

        //    char random = jsonstr[2];
        //    int length = jsonstr.Length;
        //    if (jsonstr[length - 3] == random && jsonstr[length - 2] == '#' && jsonstr[length - 1] == ']')
        //    {
        //        try
        //        {
        //            DataPacket json = DataPacket.DataString(jsonstr);
        //            action(key, json);
        //        }
        //        catch (Exception ex)
        //        {
        //            Log.Error("消息解包异常", ex, "Log/TcpFrame");
        //        }
        //    }
        //    else //进入该方法表明当前包多收了
        //    {
        //        //Builder.Append(jsonstr);
        //        //return;

        //        //string toptail = "#][#";
        //        //int le = 0;
        //        //while (le < length)
        //        //{
        //        //    int let = jsonstr.IndexOf(toptail, le);//空包必须大于50个字符, length-1

        //        //    string data;
        //        //    if (let == -1)
        //        //    {
        //        //        le = length;
        //        //        data = jsonstr.Substring(le);
        //        //        Builder.Append(data);
        //        //        break;
        //        //    }
        //        //    else
        //        //    {
        //        //        le = let;
        //        //        data = jsonstr.Substring(0, le + 2);
        //        //    }

        //        //    try
        //        //    {
        //        //        ThreadPool.QueueUserWorkItem(x =>
        //        //        {
        //        //            Receiveds(key, data, action, Builder);
        //        //        });
        //        //    }
        //        //    catch (Exception ex)
        //        //    {
        //        //        Log.Error("多包线程池异常", ex, "Log/TcpFrame");
        //        //    }
        //        //}

        //        //jsonstr.LastIndexOf

        //        //string[] packages = jsonstr.Split("#][#");
        //        //foreach (string paka in packages)
        //        //{
        //        //    StringBuilder @string = new StringBuilder(paka);
        //        //    if (@string[0] != '[' && @string[1] != '#')
        //        //    {
        //        //        @string.Insert(0, "[#");
        //        //    }
        //        //    if (@string[@string.Length - 2] != '#' && @string[@string.Length - 1] != ']')
        //        //    {
        //        //        @string.Append("#]");
        //        //    }
        //        //    try
        //        //    {
        //        //        string strdata = @string.ToString();
        //        //        ThreadPool.QueueUserWorkItem(x =>
        //        //        {
        //        //            try
        //        //            {
        //        //                DataPacket json = DataPacket.DataString(strdata); //.Json<DataPacket>();
        //        //                action(key, json);
        //        //            }
        //        //            catch (Exception ex)
        //        //            {
        //        //                Log.Error("消息解包异常", ex, "Log/TcpFrame");
        //        //            }
        //        //        });
        //        //    }
        //        //    catch (Exception ex)
        //        //    {
        //        //        Log.Error("多包线程池异常", ex, "Log/TcpFrame");
        //        //    }
        //        //    @string.Clear();
        //        //}
        //    }

        //    //if (jsonstr.Contains("#][#"))//进入该方法表明当前包多收了
        //    //{
        //    //    string[] packages = jsonstr.Split("#][#");
        //    //    foreach (string paka in packages)
        //    //    {
        //    //        StringBuilder @string = new StringBuilder(paka);
        //    //        if (@string[0] != '{')
        //    //        {
        //    //            @string.Insert(0, "[#");
        //    //        }
        //    //        if (@string[@string.Length - 1] != '}')
        //    //        {
        //    //            @string.Append("#]");
        //    //        }
        //    //        try
        //    //        {
        //    //            DataPacket json = @string.ToString().Json<DataPacket>();
        //    //            action(key, json);
        //    //        }
        //    //        catch (Exception ex)
        //    //        {
        //    //            Log.Error("消息解包异常", ex, "Log/TcpFrame");
        //    //        }
        //    //        @string.Clear();
        //    //    }
        //    //}
        //    //else
        //    //{
        //    //    try
        //    //    {
        //    //        DataPacket json = jsonstr.Json<DataPacket>();
        //    //        action(key, json);
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        Log.Error("消息解包异常", ex, "Log/TcpFrame");
        //    //    }
        //    //}
        //}
    }
}
