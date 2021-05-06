using System;
using System.Collections.Generic;
using System.Text;
using Tool.Sockets.SupportCode;
using Tool.Utils;

namespace Tool.Sockets.TcpFrame
{
    /// <summary>
    /// 请求服务器返回的数据包信息类
    /// </summary>
    public class TcpResponse
    {
        /// <summary>
        /// 无参构造
        /// </summary>
        public TcpResponse(string onlyID)
        {
            OnlyID = onlyID;
        }

        /// <summary>
        /// 消息唯一ID
        /// </summary>
        public string OnlyID { get; }

        /// <summary>
        /// 用于表示当前数据包的执行情况
        /// </summary>
        public TcpFrameState OnTcpFrame { get; internal set; } = TcpFrameState.Default;

        /// <summary>
        /// 是否是异步
        /// </summary>
        public bool IsAsync { get; internal set; }

        /// <summary>
        /// 数据包
        /// </summary>
        public string Obj { get; internal set; }

        /// <summary>
        /// 当前发生的异常
        /// </summary>
        public Exception Exception { get; internal set; }

        /// <summary>
        /// 当前消息携带的数据流
        /// </summary>
        public byte[] Bytes { get; internal set; }

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
                Obj = msg,
            };

            return dataPacket;
        }

        /// <summary>
        /// 验证并确保包100%完整
        /// </summary>
        /// <param name="packet">原始包</param>
        /// <param name="dictionary"></param>
        /// <param name="_lock"></param>
        /// <returns></returns>
        internal static bool IsComplete(ref DataPacket packet, System.Collections.Concurrent.ConcurrentDictionary<string, ThreadObj> dictionary, object _lock)
        {
            if (packet.NotIsMany || packet.IsIpIdea && !packet.IsServer)
            {
                return true;
            }
            else
            {
                ThreadObj _threadObj;
                if (packet.IsSend)
                {
                    lock (_lock)
                    {
                        if (!dictionary.TryGetValue(packet.OnlyID, out _threadObj))
                        {
                            dictionary.TryAdd(packet.OnlyID, _threadObj = new ThreadObj(packet.OnlyID));
                        }
                    }
                }
                else if (!dictionary.TryGetValue(packet.OnlyID, out _threadObj))
                {
                    packet.Dispose();
                    return false;
                }

                lock (_threadObj._lock)
                {
                    if (_threadObj.Count == 0)
                    {
                        _threadObj.Count = packet.Many.End.Value;
                        _threadObj.OjbCount = new byte[_threadObj.Count][];
                    }

                    _threadObj.OjbCount[packet.Many.Start.Value] = packet.Bytes;//.Clone() as byte[]
                    _threadObj.Length += packet.Bytes.Length;
                    _threadObj.Count--;

                    if (_threadObj.Count == 0)
                    {
                        byte[] bytes = new byte[_threadObj.Length];
                        int count = 0; //计数用于处理多包叠加
                        for (int i = 0; i < _threadObj.OjbCount.Length; i++)
                        {
                            count += i > 0 ? _threadObj.OjbCount[i - 1].Length : 0;
                            _threadObj.OjbCount[i].CopyTo(bytes, count);
                        }

                        int length = TcpStateObject.GetDataHead(bytes[0..6]);

                        if (length > 0)
                        {
                            packet.Obj = Encoding.UTF8.GetString(bytes, 6, length);
                            packet.Bytes = bytes[(6 + length)..];
                        }
                        else
                        {
                            packet.Bytes = bytes;
                        }
                        packet.EmptyMany();
                        if (packet.IsSend)
                        {
                            dictionary.TryRemove(packet.OnlyID, out _);
                            _threadObj.Dispose();
                        }
                        return true;
                    }
                    packet.Dispose();
                }

                return false;
            }
        }

        //**
        // * 返回可查找的方法键值
        // */
        //internal static string GetActionID(string OnlyID)
        //{
        //    return OnlyID.Substring(32);
        //}

        /**
         * 返回可查找的方法键值
         */
        internal static string GetActionID(DataPacket packet)
        {
            return string.Concat(packet.ClassID, '.', packet.ActionID); //OnlyID.Substring(32);
        }

        /// <summary>
        /// 获取持久连接协议
        /// </summary>
        /// <returns></returns>
        internal static byte[] KeepAlive => new byte[] { 123, 1, 2, 3, 2, 1, 123 };

        /**
         * 根据一次性回复数据包解析成一个或多个包
         */
        internal static void Receiveds(string key, byte[] bytes, Action<string, DataPacket> action)
        {
            try
            {
                if (bytes[0] != 123) return;
                DataPacket json = DataPacket.DataByte(bytes);
                action(key, json);
            }
            catch (Exception ex)
            {
                Log.Error("消息解包异常", ex, "Log/TcpFrame");
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
