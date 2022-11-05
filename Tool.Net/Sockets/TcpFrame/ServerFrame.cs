using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.SupportCode;
using Tool.Sockets.TcpHelper;
using Tool.Utils;

namespace Tool.Sockets.TcpFrame
{
    /// <summary>
    /// 封装的一个TCP框架（服务端）
    /// </summary>
    public class ServerFrame
    {
        //private static readonly object Lock = new();

        /**
         * 当前要同步等待的线程组信息
         */
        private readonly ConcurrentDictionary<string, ThreadObj> _DataPacketThreads = new();

        /**
         * 包大小
         */
        private readonly int DataLength = 2048;

        /**
         * 调用TCP长连接
         */
        private readonly TcpServerAsync serverAsync = null;

        /**
         * 各种发生的事件
         */
        private Action<string, EnServer, DateTime> Completed = null;

        /**
         * Ip:Port解释器
         */
        private Func<string, string> IpParser = null;

        //**
        // * 信号
        // */
        //private readonly ManualResetEvent _mre;

        //**
        // * 事件消息Queue
        // */
        //private readonly ConcurrentQueue<GetQueOnEnum> _que;

        /// <summary>
        /// 服务器创建时的信息
        /// </summary>
        public string Server => serverAsync.Server;

        /// <summary>
        /// 标识服务端连接是否关闭
        /// </summary>
        public bool IsClose => serverAsync.IsClose;

        /// <summary>
        /// 已建立连接的集合
        /// key:ip:port
        /// value:TcpClient
        /// </summary>
        public ConcurrentDictionary<string, Socket> ListClient => serverAsync.ListClient;

        /// <summary>
        /// 无参构造
        /// </summary>
        public ServerFrame() : this(2048)
        {
        }

        /// <summary>
        /// 初始化包
        /// </summary>
        /// <param name="DataLength">单位（KB）,单个包不能大于20M(DataLength 《 1024 * 20)</param>
        public ServerFrame(int DataLength)
        {
            if (DataLength <= 8)
            {
                throw new ArgumentException("DataLength 值必须大于8！", "client");
            }
            if (DataLength > 1024 * 20)
            {
                throw new ArgumentException("DataLength 值必须是在20M(DataLength < 1024 * 20)以内！", "client");
            }

            DataTcp.InitDicDataTcps<ServerFrame>();

            this.DataLength = DataLength * 1024 - 6; //这个6是上层包装必须满足大小
            serverAsync = new TcpServerAsync(true, this.DataLength + 6) { Millisecond = 0, IsThreadPool = false };//这里就必须加回去

            serverAsync.SetCompleted(Server_Completed);
            serverAsync.SetReceived(Server_Received);
        }

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(Action<string, EnServer, DateTime> Completed)
        {
            this.Completed ??= Completed;
        }

        /// <summary>
        /// 实现IP:Port解释器 （返回有效的IP:Port,返回空使用原值）
        /// </summary>
        /// <param name="IpParser"></param>
        public void SetIpParser(Func<string, string> IpParser)
        {
            this.IpParser ??= IpParser;
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="port"></param>
        public void StartAsync(int port)
        {
            serverAsync.StartAsync(port);
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void StartAsync(string ip, int port)
        {
            serverAsync.StartAsync(ip, port);
        }

        /**
         * 异步或同步发送消息
         * key 发送人的IP
         * dataPacket 数据包
         */
        private void SendOrAsync(DataPacket dataPacket, Socket client)
        {//给定空包为200
            //分包算法
            dataPacket.SetMany(DataLength);

            if (dataPacket.NotIsMany)
            {
                //string Json = dataPacket.StringData(); //Json();
                ArraySegment<byte> listData = dataPacket.ByteData();//Encoding.UTF8.GetBytes(Json);
                //if (listData.Length > DataLength)
                //{
                //    throw new System.SystemException($"发送数据的包大于配置的包体大小！（发送包大小{listData.Length},本该最大大小{DataLength}。）");
                //}
                SendIsAsync(client, dataPacket.IsAsync, listData);
            }
            else
            {
                //int count = dataPacket.Many.End.Value;//.Substring(2).ToInt();
                //byte[][] buffers = new byte[count][];
                //byte[] bytes = dataPacket.Bytes;//Encoding.UTF8.GetBytes(dataPacket.Obj);// as byte[];string strobj = dataPacket.Obj;
                //for (int i = 0; i < count; i++)
                //{
                //    dataPacket.GetCount(bytes, i, count, DataLength);
                //    //string Json = dataPacket.StringData(); //data.Json();
                //    byte[] listData = dataPacket.ByteData();//Encoding.UTF8.GetBytes(Json);
                //    if (listData.Length > DataLength)
                //    {
                //        throw new System.SystemException($"发送数据的包大于配置的包体大小！（发送包大小{listData.Length},本该最大大小{DataLength}。）");
                //    }
                //    buffers[i] = listData;
                //}
                //int listData = Encoding.UTF8.GetByteCount(strobj);
                ArraySegment<byte>[] buffers = dataPacket.ByteDatas();
                SendIsAsync(client, dataPacket.IsAsync, buffers);
            }
            dataPacket.Dispose();
        }

        /**
         * 异步代理发送消息
         * key 发送人的IP
         * dataPacket 数据包
         */
        private void AgentSendAsync(DataPacket dataPacket, Socket client, string key)
        {
            dataPacket.SetMany(DataLength);//分包算法
            ArraySegment<byte> listData = dataPacket.ByteData();
            if (client is null)
            {
                if (!OnIpParser(key, out client)) throw new("接收方不存在！");
            }
            SendIsAsync(client, dataPacket.IsAsync, listData);
            dataPacket.Dispose();
        }

        private void SendIsAsync(Socket client, bool IsAsync, params ArraySegment<byte>[] Data)
        {
            if (ApiPacket.TcpAsync || IsAsync)
            {
                serverAsync.SendAsync(client, Data);
            }
            else
            {
                serverAsync.Send(client, Data);
            }
        }

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="key">发送人的IP</param>
        /// <param name="api">接口调用信息</param>
        public async Task<TcpResponse> SendAsync(string key, ApiPacket api)
        {
            //string msg = api.FormatData();
            //string clmidmt = TcpResponse.GetOnlyID(api.ClassID, api.ActionID);

            DataPacket dataPacket = FrameCommon.GetDataPacket(api, null, true, true);

            return await Task.Run(() => OnSendWaitOne(key, dataPacket, api.Millisecond));

            //string clmidmt = dataPacket.OnlyID;

            //Task.Factory.StartNew(() =>
            //{
            //    if (_DataPacketThreads.TryRemove(clmidmt, out ThreadObj _threadObj))
            //    {
            //        _threadObj.Response.OnTcpFrame = TcpFrameState.OnlyID;
            //        _threadObj.AutoReset.Set();
            //        _DataPacketThreads.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt) { Action = action });
            //    }
            //    else
            //    {
            //        _DataPacketThreads.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt) { Action = action });
            //    }

            //    //DataPacket dataPacket = new DataPacket
            //    //{
            //    //    OnlyID = clmidmt,
            //    //    //ObjType = 1,
            //    //    IsSend = true,
            //    //    IsErr = false,
            //    //    IsServer = true,
            //    //    IsAsync = true,
            //    //    //ClMID = ClMID,
            //    //    Obj = msg,
            //    //};

            //    _threadObj.Response.IsAsync = dataPacket.IsAsync;//是异步的？
            //    try
            //    {
            //        SendAsync(key, dataPacket);
            //        if (!_threadObj.AutoReset.WaitOne(api.Millisecond, true))
            //        {
            //            _threadObj.Response.OnTcpFrame = TcpFrameState.Timeout;
            //            _DataPacketThreads.TryRemove(clmidmt, out _);
            //        }
            //    }
            //    catch (Exception ex)
            //    {
            //        _threadObj.Response.Exception = ex;
            //        _threadObj.Response.OnTcpFrame = TcpFrameState.SendFail;
            //        _DataPacketThreads.TryRemove(clmidmt, out _);
            //    }

            //    using (_threadObj)
            //    {
            //        _threadObj.Action(_threadObj.Response);
            //    }
            //});
        }

        /// <summary>
        /// 同步发送消息
        /// </summary>
        /// <param name="key">发送人的IP</param>
        /// <param name="api">接口调用信息</param>
        public TcpResponse Send(string key, ApiPacket api)
        {
            //string msg = api.FormatData();

            //string clmidmt = TcpResponse.GetOnlyID(api.ClassID, api.ActionID);

            DataPacket dataPacket = FrameCommon.GetDataPacket(api, null, true, false);

            return OnSendWaitOne(key, dataPacket, api.Millisecond);

            //string clmidmt = dataPacket.OnlyID;

            //if (_DataPacketThreads.TryRemove(clmidmt, out ThreadObj _threadObj))
            //{
            //    _threadObj.Response.OnTcpFrame = TcpFrameState.OnlyID;
            //    _threadObj.AutoReset.Set();
            //    _DataPacketThreads.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt));
            //}
            //else
            //{
            //    _DataPacketThreads.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt));
            //}

            ////DataPacket dataPacket = new DataPacket
            ////{
            ////    OnlyID = clmidmt,
            ////    //ObjType = 1,
            ////    IsSend = true,
            ////    IsErr = false,
            ////    IsServer = true,
            ////    IsAsync = false,
            ////    //ClMID = ClMID,
            ////    Obj = msg,
            ////};

            //_threadObj.Response.IsAsync = dataPacket.IsAsync;//是异步的？
            //try
            //{
            //    SendAsync(key, dataPacket);
            //    if (!_threadObj.AutoReset.WaitOne(api.Millisecond, true))
            //    {
            //        _threadObj.Response.OnTcpFrame = TcpFrameState.Timeout;
            //        _DataPacketThreads.TryRemove(clmidmt, out _);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    _threadObj.Response.Exception = ex;
            //    _threadObj.Response.OnTcpFrame = TcpFrameState.SendFail;
            //    _DataPacketThreads.TryRemove(clmidmt, out _);
            //}

            //using (_threadObj)
            //{
            //    return _threadObj.Response;
            //}
        }

        private TcpResponse OnSendWaitOne(string key, DataPacket dataPacket, int Millisecond)
        {
            string clmidmt = dataPacket.OnlyID;
            if (_DataPacketThreads.TryRemove(clmidmt, out ThreadObj _threadObj))
            {
                _threadObj.Response.OnTcpFrame = TcpFrameState.OnlyID;
                _threadObj.Set();
            }

            _DataPacketThreads.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt));

            //DataPacket dataPacket = new DataPacket
            //{
            //    OnlyID = clmidmt,
            //    //ObjType = 1,
            //    IsSend = true,
            //    IsErr = false,
            //    IsServer = true,
            //    IsAsync = false,
            //    //ClMID = ClMID,
            //    Obj = msg,
            //};

            //_threadObj.Response.IsAsync = dataPacket.IsAsync;//是异步的？
            try
            {
                if (!OnIpParser(key, out var client)) throw new("接收方不存在！");
                SendOrAsync(dataPacket, client);
                if (!_threadObj.WaitOne(Millisecond))
                {
                    _threadObj.Response.OnTcpFrame = TcpFrameState.Timeout;
                    _DataPacketThreads.TryRemove(clmidmt, out _);
                }
            }
            catch (Exception ex)
            {
                _threadObj.Response.Exception = ex;
                _threadObj.Response.OnTcpFrame = TcpFrameState.SendFail;
                _DataPacketThreads.TryRemove(clmidmt, out _);
            }

            using (_threadObj)
            {
                return _threadObj.Response;
            }
        }

        /**
         * 回调包信息
         */
        private void Server_Received(TcpBytes tcpBytes)
        {
            if (tcpBytes.Length < 23)
            {
                var spans = tcpBytes.Memory.Span;
                if (spans[0] == FrameCommon.KeepAlive[0]
                    && spans[1] == FrameCommon.KeepAlive[1]
                    && spans[2] == FrameCommon.KeepAlive[2]
                    && spans[3] == FrameCommon.KeepAlive[3]
                    && spans[4] == FrameCommon.KeepAlive[4]
                    && spans[5] == FrameCommon.KeepAlive[5]
                    && spans[6] == FrameCommon.KeepAlive[6])
                {
                    OnComplete(tcpBytes.Key, EnServer.HeartBeat);
                }
            }
            else
            {
                if (FrameCommon.Receiveds(tcpBytes, out var dataPacket))
                {
                    Server_Received(tcpBytes, dataPacket);
                }
            }
        }

        /**
         * 有效包处理
         */
        private void Server_Received(TcpBytes tcpBytes, DataPacket json)
        {
            try
            {
                //if (json.IsServer) return;

                if (!FrameCommon.IsComplete(true, ref json)) return;

                OnComplete(tcpBytes.Key, EnServer.Receive);
                if (json.IsSend)
                {
                    if (json.IsIpIdea)//转发 单包无问题 分包 有问题
                    {
                        json.IsServer = true;
                        try
                        {
                            string _IpPort = json.IpPort;
                            json.IpPort = tcpBytes.Key;
                            AgentSendAsync(json, default, _IpPort);
                        }
                        catch (Exception e)
                        {
                            json.IsErr = true;
                            json.IsSend = false;
                            json.Text = e.Message;
                            AgentSendAsync(json, tcpBytes.Client, default);
                        }
                    }
                    else
                    {
                        System.Threading.ThreadPool.UnsafeQueueUserWorkItem(SetPool, (json, tcpBytes), false);
                    }
                }
                else
                {
                    if (json.IsIpIdea)
                    {
                        json.IsServer = true;
                        try
                        {
                            if(serverAsync.TrySocket(json.IpPort, out var client))
                            {
                                AgentSendAsync(json, client, default);
                            }
                            else
                            {
                                json.Dispose();
                            }
                        }
                        catch { json.Dispose(); }
                    }
                    else //if (json.Many.Equals(new Range()))
                    {
                        if (_DataPacketThreads.TryRemove(json.OnlyID, out ThreadObj Threads))
                        {
                            //Threads.Response.OnTcpFrame = json.IsErr ? TcpFrameState.Exception : TcpFrameState.Success;
                            //if (!json.IsErr)
                            //{
                            //    Threads.Response.Text = json.Text;
                            //    Threads.Response.Bytes = json.Bytes.Count is 0 ? null : json.Bytes.ToArray();
                            //}
                            //else
                            //{
                            //    Threads.Response.Exception = new Exception(json.Text);
                            //}
                            //Threads.Response.IsAsync = json.IsAsync;//是异步的？
                            Threads.Response.Complete(ref json);
                            Threads.Set();
                        }
                        json.Dispose();
                    }
                    //else
                    //{
                    //    if (_DataPacketThreads.TryGetValue(json.OnlyID, out ThreadObj Threads))
                    //    {
                    //        lock (Threads._lock)
                    //        {
                    //            string[] counts = new string[2];//json.Many.Split('/');
                    //            int index = counts[0].ToInt();

                    //            //if (Threads.OjbCount == null)
                    //            //{
                    //            //    Threads.Count = counts[1].ToInt();
                    //            //    Threads.OjbCount = new string[Threads.Count];
                    //            //}

                    //            //Threads.OjbCount[index] = json.Obj;

                    //            //Threads.Count--;

                    //            //Threads.Response.OnTcpFrame = json.IsErr ? TcpFrameState.Exception : TcpFrameState.Success;

                    //            //Threads.Response.IsAsync = json.IsAsync;//是异步的？ //Threads.AutoReset.Set();
                    //            //if (Threads.Count == 0)
                    //            //{
                    //            //    if (!json.IsErr)
                    //            //    {
                    //            //        Threads.Response.Obj = string.Concat(Threads.OjbCount);
                    //            //    }
                    //            //    else
                    //            //    {
                    //            //        Threads.Response.Exception = new Exception(string.Concat(Threads.OjbCount));
                    //            //    }
                    //            //    Threads.AutoReset.Set();
                    //            //}
                    //        }
                    //    }
                    //}
                }
            }
            catch (Exception ex)
            {
                json.Dispose();
                Log.Error("服务器消息异常", ex, "Log/TcpFrame");
            }

            void SetPool((DataPacket packet, TcpBytes tcp) data)
            {
                try
                {
                    if (DataTcp.DicDataTcps.TryGetValue(data.packet.ActionKey, out DataTcp dataTcp))
                    {
                        DataBase handler = dataTcp.NewClass.Invoke();
                        using (handler)
                        {
                            DataPacket dataPacket = handler.Request(data.packet, data.tcp.Key, dataTcp);
                            SendOrAsync(dataPacket, data.tcp.Client);
                        }
                    }
                    else
                    {
                        DataPacket dataPacket = new()
                        {
                            ClassID = data.packet.ClassID,
                            ActionID = data.packet.ActionID,
                            OnlyId = data.packet.OnlyId,
                            //OnlyID = json.OnlyID,
                            IsSend = false,
                            IsErr = true,
                            IsServer = true,
                            IsAsync = data.packet.IsAsync,
                            IpPort = data.packet.IpPort,
                            Text = "接口不存在",
                        };
                        SendOrAsync(dataPacket, data.tcp.Client);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("服务器消息异常", ex, "Log/TcpFrame");
                }
                finally
                {
                    data.packet.Dispose();
                }
            }
        }

        private void Server_Completed(string arg1, EnSocketAction arg2)
        {
            switch (arg2)
            {
                case EnSocketAction.Create:
                    OnComplete(arg1, EnServer.Create);
                    break;
                case EnSocketAction.Fail:
                    OnComplete(arg1, EnServer.Fail);
                    break;
                case EnSocketAction.Connect:
                    OnComplete(arg1, EnServer.Connect);
                    break;
                case EnSocketAction.SendMsg:
                    OnComplete(arg1, EnServer.SendMsg);
                    break;
                case EnSocketAction.Close:
                    OnComplete(arg1, EnServer.ClientClose);
                    break;
                case EnSocketAction.MonitorClose:
                    OnComplete(arg1, EnServer.Close);
                    break;
            }
        }

        /**
         * 消息发送类
         * key 指定发送对象
         * enAction 消息类型
         */
        private void OnComplete(string key, EnServer enAction)
        {
            TcpEventQueue.OnComplete(key, enAction, Completed);
            //if (!_mre.SafeWaitHandle.IsClosed)
            //{
            //    _que.Enqueue(new GetQueOnEnum(key, enAction));//Completed?.Invoke(key, enAction)
            //    _mre.Set();//启动
            //}
        }

        private bool OnIpParser(string key, out Socket client)
        {
            string strip = IpParser?.Invoke(key) ?? key;
            return serverAsync.TrySocket(strip, out client);
        }

        /// <summary>
        /// 关闭服务器并断开所有连接的客户端
        /// </summary>
        public void Close()
        {
            serverAsync.Close();
        }
    }
}
