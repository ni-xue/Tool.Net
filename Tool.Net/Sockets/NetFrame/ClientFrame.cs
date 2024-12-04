using System;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.TcpHelper;
using Tool.Utils;
using Tool.Sockets.NetFrame.Internal;
using System.Net.Sockets;

namespace Tool.Sockets.NetFrame
{
    /// <summary>
    /// 封装的一个TCP框架（客户端）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class ClientFrame : EnClientEventDrive
    {
        /**
         * 当前要同步等待的线程组信息
         */
        private readonly ThreadUuIdObj threadUuIdObj;

        /**
         * 调用TCP长连接
         */
        private readonly TcpClientAsync clientAsync;

        private CompletedEvent<EnClient> Completed = null;

        /// <summary>
        /// 服务器的连接信息
        /// </summary>
        public UserKey Server { get { return clientAsync.Server; } }

        /// <summary>
        /// 当前设备的连接信息
        /// </summary>
        public Ipv4Port LocalPoint { get { return clientAsync.LocalPoint; } }

        /// <summary>
        /// 标识客户端是否关闭
        /// </summary>
        public bool IsClose { get { return clientAsync.IsClose; } }

        /// <summary>
        /// 获取一个值，该值指示 Client 的基础 Socket 是否已连接到远程主机。
        /// </summary>
        public bool Connected { get { return clientAsync.Connected; } }

        /// <summary>
        /// 是否使用线程池调度接收后的数据（允许使用者初始化时设置，消息是否有序获取）
        /// 默认 true 开启
        /// <list type="table">不使用线程池处理消息时，会使每个连接的接收线程去调度任务。</list>
        /// <list type="table">适用于需要保证通信流顺序的场景</list>
        /// </summary>
        public bool IsThreadPool { get => clientAsync.IsThreadPool; init => clientAsync.IsThreadPool = value; }

        /// <summary>
        /// 无参构造
        /// </summary>
        public ClientFrame() : this(NetBufferSize.Default) { }

        /// <summary>
        /// 初始化包
        /// </summary>
        /// <param name="bufferSize">收包规范</param>
        public ClientFrame(NetBufferSize bufferSize) : this(bufferSize, false) { }

        /// <summary>
        /// 初始化包
        /// </summary>
        /// <param name="bufferSize">收包规范</param>
        /// <param name="IsReconnect">是否在与服务器断开后主动重连？ </param>
        public ClientFrame(NetBufferSize bufferSize, bool IsReconnect)
        {
            //if (DataLength <= 8)
            //{
            //    throw new ArgumentException("DataLength 值必须大于8！", nameof(DataLength));
            //}
            //if (DataLength > 1024 * 20)
            //{
            //    throw new ArgumentException("DataLength 值必须是在20M(DataLength < 1024 * 20)以内！", nameof(DataLength));
            //}

            DataNet.InitDicDataTcps<ClientFrame>();

            threadUuIdObj = new();

            clientAsync = new TcpClientAsync(bufferSize, true, IsReconnect) { Millisecond = 0 };//这里就必须加回去
            clientAsync.OpenAllEvent().OnInterceptor(EnClient.Receive, true);
            clientAsync.CloseAllQueue();
            clientAsync.SetCompleted(Client_Completed);
            clientAsync.SetReceived(Client_Received);
        }

        /// <summary>
        /// 连接、发送、关闭事件 <see cref="EnClient"/>
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(CompletedEvent<EnClient> Completed)
        {
            if (this.Completed == null)
                this.Completed = Completed;
            else throw new InvalidOperationException("ClientFrame 已绑定委托回调");
        }

        /// <summary>
        /// 异步连接，连接ip地址为127.0.0.1
        /// </summary>
        /// <param name="port">要连接的服务器的端口</param>
        public async Task ConnectAsync(int port)
        {
            await ConnectAsync(StaticData.LocalIp, port);
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="ip">要连接的服务器的ip地址</param>
        /// <param name="port">要连接的服务器的端口</param>
        public async Task ConnectAsync(string ip, int port)
        {
            await clientAsync.ConnectAsync(ip, port);
        }

        /// <summary>
        /// 重连，返回是否重连，如果没有断开是不会重连的
        /// </summary>
        public async Task<bool> Reconnection()
        {
            return await clientAsync.Reconnection();
        }

        /**
         * 异步或同步发送消息
         * dataPacket 数据包
         */
        private async ValueTask SendAsync(IDataPacket dataPacket)
        {   //给定空包为100
            //分包算法
            //dataPacket.SetMany(DataLength);

            var length = dataPacket.TotalSize(out var size);
            var sendBytes = clientAsync.CreateSendBytes(length);
            dataPacket.ByteData(sendBytes, size);
            await SendAsync(sendBytes);

            //if (dataPacket.NotIsMany)
            //{
            //    //byte[] listData = Encoding.UTF8.GetBytes(dataPacket.ToJson());
            //    //string Json = dataPacket.StringData(); //Json();
            //    ArraySegment<byte> listData = dataPacket.ByteData();//Encoding.UTF8.GetBytes(Json);
            //    //if (listData.Length > DataLength)
            //    //{
            //    //    throw new System.SystemException($"发送数据的包大于配置的包体大小！（发送包大小{listData.Length},本该最大大小{DataLength}。）");
            //    //}
            //    await SendAsync(listData);
            //}
            //else
            //{
            //    //int count = dataPacket.Many.End.Value;// Substring(2).ToInt();
            //    //byte[][] buffers = new byte[count][];
            //    //byte[] bytes = dataPacket.Bytes;//Encoding.UTF8.GetBytes(dataPacket.Obj);// as byte[];string strobj = dataPacket.Obj;
            //    //for (int i = 0; i < count; i++)
            //    //{
            //    //    dataPacket.GetCount(bytes, i, count, DataLength);
            //    //    //string Json = dataPacket.StringData(); //data.Json();
            //    //    byte[] listData = dataPacket.ByteData();//Encoding.UTF8.GetBytes(Json);
            //    //    if (listData.Length > DataLength)
            //    //    {
            //    //        throw new System.SystemException($"发送数据的包大于配置的包体大小！（发送包大小{listData.Length},本该最大大小{DataLength}。）");
            //    //    }
            //    //    buffers[i] = listData;
            //    //}
            //    ArraySegment<byte>[] buffers = dataPacket.ByteDatas();
            //    await SendAsync(buffers);
            //}
            ////Keep?.ResetTime();
            //dataPacket.Dispose();
        }

        private async ValueTask SendAsync(SendBytes<Socket> sendBytes)
        {
            try
            {
                await clientAsync.SendAsync(sendBytes);
            }
            finally
            {
                sendBytes.Dispose();
            }
        }

        private NetResponse Send(Ipv4Port IpPort, ApiPacket api)
        {
            var task = OnSendWaitOne(api, IpPort);
            task.Wait();
            return task.Result;
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
        //public void SendRelayAsync(string IpPort, ApiPacket api, Action<TcpResponse> action)
        //{
        //    if (!StateObject.IsIpPort(IpPort))
        //    {
        //        throw new Exception("您输入的“IpPort”变量未满足IP端口的需求。");
        //    }
        //    SendAsync(IpPort, api, action);//true, 
        //}

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="api">接口调用信息</param>
        public async ValueTask<NetResponse> SendAsync(ApiPacket api)
        {
            return await SendAsync(Ipv4Port.Empty, api);//false, 
        }

        /// <summary>
        /// 异步发送消息（转发给指定客户端）
        /// </summary>
        /// <param name="IpPort">事件处理的服务器</param>
        /// <param name="api">接口调用信息</param>
        public async ValueTask<NetResponse> SendRelayAsync(string IpPort, ApiPacket api)
        {
            if (!StateObject.IsIpPort(IpPort, out Ipv4Port ipunm))
            {
                throw new Exception("您输入的“IpPort”变量未满足IP端口的需求。");
            }
            return await SendAsync(ipunm, api);//false, 
        }

        private async ValueTask<NetResponse> SendAsync(Ipv4Port IpPort, ApiPacket api)//bool isIpPort,
        {
            return await OnSendWaitOne(api, IpPort);
            //return await Task.Run(() => OnSendWaitOne(api));
        }

        /// <summary>
        /// 同步发送消息
        /// </summary>
        /// <param name="api">接口调用信息</param>
        public NetResponse Send(ApiPacket api)
        {
            return Send(Ipv4Port.Empty, api);
        }

        /// <summary>
        /// 同步发送消息（转发给指定客户端）
        /// </summary>
        /// <param name="IpPort">事件处理的服务器</param>
        /// <param name="api">接口调用信息</param>
        public NetResponse SendRelay(string IpPort, ApiPacket api)
        {
            if (!StateObject.IsIpPort(IpPort, out Ipv4Port ipunm))
            {
                throw new Exception("您输入的“IpPort”变量未满足IP端口的需求。");
            }
            return Send(ipunm, api);
        }

        private async Task<NetResponse> OnSendWaitOne(ApiPacket api, Ipv4Port ipPort = default)
        {
            Guid clmidmt = Guid.NewGuid();
            if (threadUuIdObj.TryThreadObj(in clmidmt, api.IsReply, out ThreadObj _threadObj, out var response)) // FrameCommon.TryThreadObj(_DataPacketThreads, clmidmt, api.IsReply);
            {
                using (_threadObj)
                {
                    try
                    {
                        using IDataPacket dataPacket = FrameCommon.GetDataPacket(api, clmidmt, false, in ipPort);

                        await SendAsync(dataPacket).IsNewTask();
                        if (!_threadObj.WaitOne(api.Millisecond))
                        {
                            threadUuIdObj.SetTimeout(in clmidmt, _threadObj);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (api.IsReply) threadUuIdObj.SetException(in clmidmt, _threadObj, ex); else _threadObj.SetSendFail(ex);
                    }
                    return _threadObj.GetResponse(in clmidmt);
                }
            }
            else
            {
                return response;
            }
        }

        /// <summary>
        /// 添加持久化消息（心跳），防止特殊情况下的断开连接
        /// </summary>
        public void AddKeepAlive(byte TimeInterval)
        {
            clientAsync.AddKeepAlive(TimeInterval);
        }

        /**
         * 回调包信息
         */
        private async ValueTask Client_Received(ReceiveBytes<Socket> tcpBytes)
        {
            if (FrameCommon.Receiveds(in tcpBytes, out var dataPacket))
            {
                var poolData = new PoolData(tcpBytes.Key, tcpBytes.Client, dataPacket);
                if (await Client_Received(poolData))
                {
                    poolData.Dispose();
                }
            }
        }

        /**
         * 有效包处理
         */
        private async ValueTask<bool> Client_Received(PoolData poolData)
        {
            try
            {
                //if (!json.IsServer) return;
                //if (!FrameCommon.IsComplete(false, ref json)) return;

                await OnComplete(poolData.Key, EnClient.Receive);
                var json = poolData.Packet;
                if (json.IsSend) //表示服务器发包
                {
                    //System.Threading.ThreadPool.UnsafeQueueUserWorkItem(SetPool, poolData, false);
                    await SetPool(poolData);
                }
                else if (threadUuIdObj.Complete(json.OnlyId, out ThreadObj Threads)) //_DataPacketThreads.TryRemove(json.OnlyId, out ThreadObj Threads))//表示服务器回包
                {
                    Threads.Set(json);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("客户端消息异常", ex, "Log/NetFrame");
            }
            return true;

            async ValueTask SetPool(PoolData data)
            {
                try
                {
                    using var dataPacket = await data.RequestAsync();
                    if (dataPacket.IsReply) await SendAsync(dataPacket);
                }
                catch (Exception ex)
                {
                    Log.Error("客户端消息异常", ex, "Log/NetFrame");
                }
            }
        }

        private async ValueTask Client_Completed(UserKey arg1, EnClient arg2, DateTime arg3)
        {
            switch (arg2)
            {
                case EnClient.Connect:
                    threadUuIdObj.protocol = ProtocolStatus.Connect;
                    break;
                case EnClient.Fail:
                    threadUuIdObj.protocol = ProtocolStatus.Fail;
                    break;
                case EnClient.Close:
                    threadUuIdObj.protocol = ProtocolStatus.Close;
                    threadUuIdObj.AllError();
                    break;
                case EnClient.Reconnect:
                    threadUuIdObj.protocol |= ProtocolStatus.Reconnect;
                    break;
            }
            if (Completed is not null) 
            {
                await OnComplete(arg1, arg2);
            }
        }

        /**
         * 消息发送类
         * key 指定发送对象
         * enAction 消息类型
         */
        private ValueTask<IGetQueOnEnum> OnComplete(in UserKey key, EnClient enAction)
        {
            if (IsEvent(enAction))
            {
                return EnumEventQueue.OnComplete(in key, enAction, IsQueue(enAction), Completed);
            }
            return IGetQueOnEnum.SuccessAsync;
        }

        /// <summary>
        /// 关闭连接，断开处于连接状态的服务器
        /// </summary>
        public void Close()
        {
            clientAsync.Dispose();
            threadUuIdObj.Dispose();
        }

        //~ClientFrame() 
        //{
        //    _mre.Set();
        //}
    }
}
