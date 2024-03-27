using System;
using System.Net.Sockets;
using Tool.Sockets.Kernels;
using Tool.Utils;

namespace Tool.Sockets.NetFrame.Internal
{
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

                NetByteObjs _byteObjs = StaticData.TcpByteObjs.GetOrAdd(OnlyID, a => new NetByteObjs(objcount));

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

        /**
         * 根据一次性回复数据包解析成一个或多个包
         */
        internal static bool Receiveds(ReceiveBytes<Socket> tcpBytes, out DataPacket dataPacket)
        {
            try
            {
                var spans = tcpBytes.Memory.Span;
                if (spans[0] == KeepAlive.KeepAliveObj[0])
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
