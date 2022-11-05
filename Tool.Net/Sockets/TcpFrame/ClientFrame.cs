using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using Tool.Sockets.SupportCode;
using Tool.Sockets.TcpHelper;
using Tool.Utils;

namespace Tool.Sockets.TcpFrame
{
    /// <summary>
    /// 封装的一个TCP框架（客户端）
    /// </summary>
    public class ClientFrame
    {
        /**
         * 锁
         */
        private static readonly object Lock = new();

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
        private readonly TcpClientAsync clientAsync = null;

        private Action<string, EnClient, DateTime> Completed = null;

        /// <summary>
        /// 获取当前心跳信息
        /// </summary>
        public KeepAlive Keep { get; private set; }

        /// <summary>
        /// 服务器的连接信息
        /// </summary>
        public string Server { get { return clientAsync.Server; } }

        /// <summary>
        /// 当前设备的连接信息
        /// </summary>
        public string LocalPoint { get { return clientAsync.LocalPoint; } }

        /// <summary>
        /// 标识客户端是否关闭
        /// </summary>
        public bool IsClose { get { return clientAsync.IsClose; } }

        /// <summary>
        /// 获取一个值，该值指示 Client 的基础 Socket 是否已连接到远程主机。
        /// </summary>
        public bool Connected { get { return clientAsync.Connected; } }

        /// <summary>
        /// 无参构造
        /// </summary>
        public ClientFrame() : this(TcpBufferSize.Default, 2048)
        {
        }

        /// <summary>
        /// 初始化包
        /// </summary>
        /// <param name="DataLength">单位（KB）,单个包不能大于20M(DataLength 《 1024 * 20)</param>
        public ClientFrame(int DataLength) : this(TcpBufferSize.Default, DataLength)
        {
        }

        /// <summary>
        /// 初始化包
        /// </summary>
        /// <param name="bufferSize">收包规范</param>
        /// <param name="DataLength">单位（KB）,单个包不能大于20M(DataLength 《 1024 * 20)</param>
        public ClientFrame(TcpBufferSize bufferSize, int DataLength) : this(bufferSize, DataLength, false)
        {

        }

        /// <summary>
        /// 初始化包
        /// </summary>
        /// <param name="bufferSize">收包规范</param>
        /// <param name="DataLength">单位（KB）,单个包不能大于20M(DataLength 《 1024 * 20)</param>
        /// <param name="IsReconnect">是否在与服务器断开后主动重连？ </param>
        public ClientFrame(TcpBufferSize bufferSize, int DataLength, bool IsReconnect)
        {
            if (DataLength <= 8)
            {
                throw new ArgumentException("DataLength 值必须大于8！", nameof(DataLength));
            }
            if (DataLength > 1024 * 20)
            {
                throw new ArgumentException("DataLength 值必须是在20M(DataLength < 1024 * 20)以内！", nameof(DataLength));
            }

            DataTcp.InitDicDataTcps<ClientFrame>();

            this.DataLength = DataLength * 1024 - 6; //这个6是上层包装必须满足大小
            clientAsync = new TcpClientAsync(bufferSize, true, this.DataLength + 6, IsReconnect) { Millisecond = 0, IsThreadPool = false };//这里就必须加回去

            clientAsync.SetCompleted(Client_Completed);
            clientAsync.SetReceived(Client_Received);
        }

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(Action<string, EnClient, DateTime> Completed)
        {
            if (this.Completed == null)
                UpdateCompleted(Completed);
        }

        internal void UpdateCompleted(Action<string, EnClient, DateTime> Completed)
        {
            this.Completed = Completed;
        }

        /// <summary>
        /// 异步连接，连接ip地址为127.0.0.1
        /// </summary>
        /// <param name="port">要连接的服务器的端口</param>
        public void ConnectAsync(int port)
        {
            clientAsync.ConnectAsync(port);
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="ip">要连接的服务器的ip地址</param>
        /// <param name="port">要连接的服务器的端口</param>
        public void ConnectAsync(string ip, int port)
        {
            clientAsync.ConnectAsync(ip, port);
        }

        /// <summary>
        /// 重连，返回是否重连，如果没有断开是不会重连的
        /// </summary>
        public bool Reconnection()
        {
            return clientAsync.Reconnection();
        }

        /**
         * 异步或同步发送消息
         * dataPacket 数据包
         */
        private void SendOrAsync(DataPacket dataPacket)
        {//给定空包为100
            //分包算法
            dataPacket.SetMany(DataLength);

            if (dataPacket.NotIsMany)
            {
                //byte[] listData = Encoding.UTF8.GetBytes(dataPacket.ToJson());
                //string Json = dataPacket.StringData(); //Json();
                ArraySegment<byte> listData = dataPacket.ByteData();//Encoding.UTF8.GetBytes(Json);
                //if (listData.Length > DataLength)
                //{
                //    throw new System.SystemException($"发送数据的包大于配置的包体大小！（发送包大小{listData.Length},本该最大大小{DataLength}。）");
                //}
                SendIsAsync(dataPacket.IsAsync, listData);
            }
            else
            {
                //int count = dataPacket.Many.End.Value;// Substring(2).ToInt();
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
                ArraySegment<byte>[] buffers = dataPacket.ByteDatas();
                SendIsAsync(dataPacket.IsAsync, buffers);
            }
            Keep?.ResetTime();
            dataPacket.Dispose();
        }

        private void SendIsAsync(bool IsAsync, params ArraySegment<byte>[] Data)
        {
            if (ApiPacket.TcpAsync || IsAsync)
            {
                clientAsync.SendAsync(Data);
            }
            else
            {
                clientAsync.Send(Data);
            }
        }

        /// <summary>
        /// 同步发送消息
        /// </summary>
        /// <param name="api">接口调用信息</param>
        /// <param name="IpPort"></param>
        private TcpResponse Send(ApiPacket api, string IpPort)
        {
            //string msg = api.FormatData();
            //string clmidmt = TcpResponse.GetOnlyID(api.ClassID, api.ActionID);

            DataPacket dataPacket = FrameCommon.GetDataPacket(api, IpPort, false, false);

            return OnSendWaitOne(dataPacket, api.Millisecond);
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
            ////    ClassID = 1,
            ////    ActionID = 25,
            ////    OnlyID = clmidmt,
            ////    Bytes = api.Bytes,
            ////    //ObjType = 1,
            ////    IsSend = true,
            ////    IsErr = false,
            ////    IsServer = false,
            ////    IsAsync = false,
            ////    //IsIpIdea = isIpPort,
            ////    IpPort = IpPort,
            ////    //ClMID = ClMID,
            ////    Obj = msg,
            ////};

            //_threadObj.Response.IsAsync = dataPacket.IsAsync;//是异步的？
            ////_threadObj.Response.OnTcpFrame = TcpFrameState.Timeout;
            ////_threadObj.AutoReset.WaitOne(api.Millisecond);
            ////if (_threadObj.Response.OnTcpFrame == TcpFrameState.Timeout)
            ////{
            ////    _DataPacketThreads.TryRemove(clmidmt, out _);
            ////}

            //try
            //{
            //    SendAsync(dataPacket);
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

        ///// <summary>
        ///// 异步发送消息
        ///// </summary>
        ///// <param name="IpPort"></param>
        ///// <param name="api">接口调用信息</param>
        ///// <param name="action">异步回调返回消息</param>
        //private void SendAsync(string IpPort, ApiPacket api, Action<TcpResponse> action)//bool isIpPort,
        //{
        //    //string msg = api.FormatData();
        //    //string clmidmt = TcpResponse.GetOnlyID(api.ClassID, api.ActionID);

        //    DataPacket dataPacket = TcpResponse.GetDataPacket(api, IpPort, false, true);

        //    string clmidmt = dataPacket.OnlyID;

        //    Task.Factory.StartNew(() =>
        //    {
        //        if (_DataPacketThreads.TryRemove(clmidmt, out ThreadObj _threadObj))
        //        {
        //            _threadObj.Response.OnTcpFrame = TcpFrameState.OnlyID;
        //            _threadObj.AutoReset.Set();
        //            _DataPacketThreads.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt) { Action = action });
        //        }
        //        else
        //        {
        //            _DataPacketThreads.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt) { Action = action });
        //        }

        //        //DataPacket dataPacket = new DataPacket
        //        //{
        //        //    OnlyID = clmidmt,
        //        //    //ObjType = 1,
        //        //    IsSend = true,
        //        //    IsErr = false,
        //        //    IsServer = false,
        //        //    IsAsync = true,
        //        //    //IsIpIdea = isIpPort,
        //        //    IpPort = IpPort,
        //        //    //ClMID = ClMID,
        //        //    Obj = msg,
        //        //};

        //        _threadObj.Response.IsAsync = dataPacket.IsAsync;//是异步的？
        //        //_threadObj.Response.OnTcpFrame = TcpFrameState.Timeout;
        //        //_threadObj.AutoReset.WaitOne(api.Millisecond);
        //        //if (_threadObj.Response.OnTcpFrame == TcpFrameState.Timeout)
        //        //{
        //        //    _DataPacketThreads.TryRemove(clmidmt, out _);
        //        //}

        //        try
        //        {
        //            SendAsync(dataPacket);
        //            if (!_threadObj.AutoReset.WaitOne(api.Millisecond, true))
        //            {
        //                _threadObj.Response.OnTcpFrame = TcpFrameState.Timeout;
        //                _DataPacketThreads.TryRemove(clmidmt, out _);
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            _threadObj.Response.Exception = ex;
        //            _threadObj.Response.OnTcpFrame = TcpFrameState.SendFail;
        //            _DataPacketThreads.TryRemove(clmidmt, out _);
        //        }

        //        using (_threadObj)
        //        {
        //            _threadObj.Action?.Invoke(_threadObj.Response);
        //        }
        //    });
        //}

        ///// <summary>
        ///// 异步发送消息
        ///// </summary>
        ///// <param name="api">接口调用信息</param>
        ///// <param name="action">异步回调返回消息</param>
        //public void SendAsync(ApiPacket api, Action<TcpResponse> action)
        //{
        //    SendAsync(null, api, action);//false, 
        //}

        ///// <summary>
        ///// 异步发送消息（转发给指定客户端）
        ///// </summary>
        ///// <param name="IpPort">事件处理的服务器</param>
        ///// <param name="api">接口调用信息</param>
        ///// <param name="action">异步回调返回消息</param>
        //public void SendIpIdeaAsync(string IpPort, ApiPacket api, Action<TcpResponse> action)
        //{
        //    if (!TcpStateObject.IsIpPort(IpPort))
        //    {
        //        throw new Exception("您输入的“IpPort”变量未满足IP端口的需求。");
        //    }
        //    SendAsync(IpPort, api, action);//true, 
        //}

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="api">接口调用信息</param>
        public async Task<TcpResponse> SendAsync(ApiPacket api)
        {
            return await SendAsync(null, api);//false, 
        }

        /// <summary>
        /// 异步发送消息（转发给指定客户端）
        /// </summary>
        /// <param name="IpPort">事件处理的服务器</param>
        /// <param name="api">接口调用信息</param>
        public async Task<TcpResponse> SendIpIdeaAsync(string IpPort, ApiPacket api)
        {
            if (!TcpStateObject.IsIpPort(IpPort))
            {
                throw new Exception("您输入的“IpPort”变量未满足IP端口的需求。");
            }
            return await SendAsync(IpPort, api);//false, 
        }

        private async Task<TcpResponse> SendAsync(string IpPort, ApiPacket api)//bool isIpPort,
        {
            //string msg = api.FormatData();
            //string clmidmt = TcpResponse.GetOnlyID(api.ClassID, api.ActionID);

            DataPacket dataPacket = FrameCommon.GetDataPacket(api, IpPort, false, true);

            return await Task.Run(() => OnSendWaitOne(dataPacket, api.Millisecond));

            //return await Task.Run<TcpResponse>(() =>
            //{
            //    if (_DataPacketThreads.TryRemove(clmidmt, out ThreadObj _threadObj))
            //    {
            //        _threadObj.Response.OnTcpFrame = TcpFrameState.OnlyID;
            //        _threadObj.AutoReset.Set();
            //        _DataPacketThreads.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt));
            //    }
            //    else
            //    {
            //        _DataPacketThreads.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt));
            //    }

            //    //DataPacket dataPacket = new DataPacket
            //    //{
            //    //    OnlyID = clmidmt,
            //    //    //ObjType = 1,
            //    //    IsSend = true,
            //    //    IsErr = false,
            //    //    IsServer = false,
            //    //    IsAsync = true,
            //    //    //IsIpIdea = isIpPort,
            //    //    IpPort = IpPort,
            //    //    //ClMID = ClMID,
            //    //    Obj = msg,
            //    //};

            //    _threadObj.Response.IsAsync = dataPacket.IsAsync;//是异步的？
            //    //_threadObj.Response.OnTcpFrame = TcpFrameState.Timeout;
            //    //_threadObj.AutoReset.WaitOne(api.Millisecond);
            //    //if (_threadObj.Response.OnTcpFrame == TcpFrameState.Timeout)
            //    //{
            //    //    _DataPacketThreads.TryRemove(clmidmt, out _);
            //    //}

            //    try
            //    {
            //        SendAsync(dataPacket);
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
            //        return _threadObj.Response;// _threadObj.Action?.Invoke(_threadObj.Response);
            //    }
            //});
        }

        /// <summary>
        /// 同步发送消息
        /// </summary>
        /// <param name="api">接口调用信息</param>
        public TcpResponse Send(ApiPacket api)
        {
            return Send(api, null);
        }

        /// <summary>
        /// 同步发送消息（转发给指定客户端）
        /// </summary>
        /// <param name="IpPort">事件处理的服务器</param>
        /// <param name="api">接口调用信息</param>
        public TcpResponse SendIpIdea(string IpPort, ApiPacket api)
        {
            if (!TcpStateObject.IsIpPort(IpPort))
            {
                throw new Exception("您输入的“IpPort”变量未满足IP端口的需求。");
            }
            return Send(api, IpPort);
        }

        private TcpResponse OnSendWaitOne(DataPacket dataPacket, int Millisecond)
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
            //    IsServer = false,
            //    IsAsync = true,
            //    //IsIpIdea = isIpPort,
            //    IpPort = IpPort,
            //    //ClMID = ClMID,
            //    Obj = msg,
            //};

            //_threadObj.Response.IsAsync = dataPacket.IsAsync;//是异步的？
            //_threadObj.Response.OnTcpFrame = TcpFrameState.Timeout;
            //_threadObj.AutoReset.WaitOne(api.Millisecond);
            //if (_threadObj.Response.OnTcpFrame == TcpFrameState.Timeout)
            //{
            //    _DataPacketThreads.TryRemove(clmidmt, out _);
            //}

            try
            {
                SendOrAsync(dataPacket);
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
                return _threadObj.Response;// _threadObj.Action?.Invoke(_threadObj.Response);
            }
        }

        /// <summary>
        /// 添加持久化消息（心跳），防止特殊情况下的断开连接
        /// </summary>
        public void AddKeepAlive(byte TimeInterval)
        {
            if (Keep == null)
            {
                lock (Lock)
                {
                    if (Keep == null)
                    {
                        Keep = new KeepAlive(TimeInterval, () =>
                        {
                            try
                            {
                                clientAsync.Send(FrameCommon.KeepAlive);
                            }
                            catch (Exception)
                            {

                            }
                        });
                        return;
                    }
                }
            }
            throw new Exception("心跳已经开启啦，请勿重复操作！");
        }

        /**
         * 回调包信息
         */
        private void Client_Received(TcpBytes bytes)
        {
            Keep?.ResetTime();
            if (FrameCommon.Receiveds(bytes, out var dataPacket))
            {
                Client_Received(bytes, dataPacket);
            }
            //string msg = Encoding.UTF8.GetString(arg2, 0, arg2.Length);
            //if (!string.IsNullOrEmpty(msg))
            //{
            //    TcpResponse.Receiveds(arg1, msg, ClientAsync_Received);
            //}
        }

        /**
         * 有效包处理
         */
        private void Client_Received(TcpBytes tcpBytes, DataPacket json)
        {
            try
            {
                //if (!json.IsServer) return;

                if (!FrameCommon.IsComplete(false , ref json)) return;

                OnComplete(tcpBytes.Key, EnClient.Receive);
                if (json.IsSend) //表示服务器发包
                {
                    System.Threading.ThreadPool.UnsafeQueueUserWorkItem(SetPool, (json, tcpBytes.Key), false);
                }
                else //表示服务器回包
                {
                    //if (json.Many.Equals(new Range()))
                    //{
                    if (_DataPacketThreads.TryRemove(json.OnlyID, out ThreadObj Threads))
                    {
                        //Threads.Response.OnTcpFrame = json.IsErr ? TcpFrameState.Exception : TcpFrameState.Success;
                        //if (!json.IsErr)
                        //{
                        //    Threads.Response.Text = json.Text;
                        //    Threads.Response.Bytes = json.Bytes.ToArray();
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
                    //}
                    //else
                    //{
                    //    if (_DataPacketThreads.TryGetValue(json.OnlyID, out ThreadObj Threads))
                    //    {
                    //        lock (Threads._lock)
                    //        {
                    //            string[] counts = new string[2]; //json.Many.Split('/');
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
                Log.Error("客户端消息异常", ex, "Log/TcpFrame");
            }

            void SetPool((DataPacket packet, string key) data)
            {
                try
                {
                    if (DataTcp.DicDataTcps.TryGetValue(data.packet.ActionKey, out DataTcp dataTcp))
                    {
                        DataBase handler = dataTcp.NewClass.Invoke();
                        using (handler)
                        {
                            DataPacket dataPacket = handler.Request(data.packet, data.key, dataTcp);
                            SendOrAsync(dataPacket);
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
                            IsServer = false,
                            IsAsync = data.packet.IsAsync,
                            IpPort = data.packet.IpPort,
                            Text = "接口不存在",
                        };
                        SendOrAsync(dataPacket);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("客户端消息异常", ex, "Log/TcpFrame");
                }
                finally
                {
                    data.packet.Dispose();
                }
            }
        }

        private void Client_Completed(string arg1, EnSocketAction arg2)
        {
            switch (arg2)
            {
                case EnSocketAction.Fail:
                    OnComplete(arg1, EnClient.Fail);
                    break;
                case EnSocketAction.Connect:
                    OnComplete(arg1, EnClient.Connect);
                    break;
                case EnSocketAction.SendMsg:
                    OnComplete(arg1, EnClient.SendMsg);
                    break;
                case EnSocketAction.Close:
                    OnComplete(arg1, EnClient.Close);
                    if (Keep != null) Keep.Close();
                    break;
            }
        }

        /**
         * 消息发送类
         * key 指定发送对象
         * enAction 消息类型
         */
        private void OnComplete(string key, EnClient enAction)
        {
            TcpEventQueue.OnComplete(key, enAction, Completed);
            //if (!_mre.SafeWaitHandle.IsClosed)
            //{
            //    _que.Enqueue(new GetQueOnEnum(key, enAction));//Completed?.Invoke(key, enAction)
            //    _mre.Set();//启动
            //}
        }

        /// <summary>
        /// 关闭连接，断开处于连接状态的服务器
        /// </summary>
        public void Close()
        {
            clientAsync.Close();
        }

        //~ClientFrame() 
        //{
        //    _mre.Set();
        //}
    }
}
