using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using Tool.Sockets.Kernels;
using Tool.Sockets.TcpHelper;
using Tool.Utils;
using Tool.Sockets.NetFrame.Internal;

namespace Tool.Sockets.NetFrame
{
    /// <summary>
    /// 封装的一个TCP框架（服务端）
    /// </summary>
    /// <remarks>代码由逆血提供支持</remarks>
    public class ServerFrame : EnServerEventDrive
    {
        /**
         * 当前要同步等待的线程组信息
         */
        private readonly ThreadKeyObj threadKeyObj;

        /**
         * 调用TCP长连接
         */
        private readonly TcpServerAsync serverAsync = null;

        /**
         * 各种发生的事件
         */
        private CompletedEvent<EnServer> Completed = null;

        /**
         * Ip:Port解释器
         */
        private IpParserEvent IpParser = null;

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
        public UserKey Server => serverAsync.Server;

        /// <summary>
        /// 标识服务端连接是否关闭
        /// </summary>
        public bool IsClose => serverAsync.IsClose;

        /// <summary>
        /// 是否使用线程池调度接收后的数据（允许使用者初始化时设置，消息是否有序获取）
        /// 默认 true 开启
        /// <list type="table">不使用线程池处理消息时，会使每个连接的接收线程去调度任务。</list>
        /// <list type="table">适用于需要保证通信流顺序的场景</list>
        /// </summary>
        public bool IsThreadPool { get => serverAsync.IsThreadPool; init => serverAsync.IsThreadPool = value; }

        /// <summary>
        /// 已建立连接的集合
        /// key:ip:port
        /// value:Socket
        /// </summary>
        public IReadOnlyDictionary<UserKey, Socket> ListClient => serverAsync.listClient;

        /// <summary>
        /// 表示服务器是否接受转发消息（默认接受）
        /// </summary>
        public bool IsAllowRelay { get; init; } = true;

        /// <summary>
        /// 无参构造
        /// </summary>
        public ServerFrame() : this(NetBufferSize.Default) { }

        /// <summary>
        /// 初始化包
        /// </summary>
        /// <param name="bufferSize">收包规范</param>
        public ServerFrame(NetBufferSize bufferSize)
        {
            //if (DataLength <= 8)
            //{
            //    throw new ArgumentException("DataLength 值必须大于8！", "client");
            //}
            //if (DataLength > 1024 * 20)
            //{
            //    throw new ArgumentException("DataLength 值必须是在20M(DataLength < 1024 * 20)以内！", "client");
            //}

            DataNet.InitDicDataTcps<ServerFrame>();

            threadKeyObj = new ThreadKeyObj();

            serverAsync = new TcpServerAsync(bufferSize, true) { Millisecond = 0 };//这里就必须加回去
            serverAsync.OpenAllEvent().OnInterceptor(EnServer.Receive, true);
            serverAsync.CloseAllQueue();
            serverAsync.SetCompleted(Server_Completed);
            serverAsync.SetReceived(Server_Received);
        }

        /// <summary>
        /// 连接、发送、关闭事件 <see cref="EnServer"/>
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(CompletedEvent<EnServer> Completed)
        {
            this.Completed ??= Completed;
        }

        /// <summary>
        /// 实现IP:Port解释器 （返回有效的IP:Port,返回空使用原值）
        /// 参数1：发起方信息空表示无发起方，参数2：接收方信息
        /// </summary>
        /// <param name="IpParser"></param>
        public void SetIpParser(IpParserEvent IpParser)
        {
            this.IpParser ??= IpParser;
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="port"></param>
        public async Task StartAsync(int port)
        {
            await StartAsync(StaticData.LocalIp, port);
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public async Task StartAsync(string ip, int port)
        {
            await serverAsync.StartAsync(ip, port);
        }

        /// <summary>
        /// 关闭存在的连接用户
        /// </summary>
        /// <param name="ipv4">IpV4</param>
        /// <returns>成功/失败</returns>
        public bool ClientClose(Ipv4Port ipv4)
        {
            bool isOk = serverAsync.TrySocket(ipv4, out var client);
            if (isOk)
            {
                client.Close();
            }
            return isOk;
        }

        /**
         * 异步或同步发送消息
         * key 发送人的IP
         * dataPacket 数据包
         */
        private async ValueTask SendAsync(IDataPacket dataPacket, Socket client)
        {   //给定空包为200
            //分包算法
            //dataPacket.SetMany(DataLength);

            var length = dataPacket.TotalSize(out var size);
            var sendBytes = serverAsync.CreateSendBytes(client, length);
            dataPacket.ByteData(sendBytes, size);
            await SendAsync(sendBytes);

            //if (dataPacket.NotIsMany)
            //{
            //    //string Json = dataPacket.StringData(); //Json();
            //    ArraySegment<byte> listData = dataPacket.ByteData();//Encoding.UTF8.GetBytes(Json);
            //    //if (listData.Length > DataLength)
            //    //{
            //    //    throw new System.SystemException($"发送数据的包大于配置的包体大小！（发送包大小{listData.Length},本该最大大小{DataLength}。）");
            //    //}
            //    await SendAsync(client, listData);
            //}
            //else
            //{
            //    //int count = dataPacket.Many.End.Value;//.Substring(2).ToInt();
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
            //    //int listData = Encoding.UTF8.GetByteCount(strobj);
            //    ArraySegment<byte>[] buffers = dataPacket.ByteDatas();
            //    await SendAsync(client, buffers);
            //}
        }

        /**
         * 异步代理发送消息
         * key 发送人的IP
         * dataPacket 数据包
         */
        private async ValueTask AgentSendAsync(IDataPacket dataPacket, Ipv4Port key, Ipv4Port SponsorIp)
        {
            IsIpParser ipParser = await OnIpParser(key, SponsorIp);
            if (!ipParser.IsOk) throw new("接收方不存在！");
            await AgentSendAsync(dataPacket, ipParser.Client);
        }

        /**
         * 异步代理发送消息
         * key 发送人的IP
         * dataPacket 数据包
         */
        private async ValueTask AgentSendAsync(IDataPacket dataPacket, Socket client)
        {
            //dataPacket.SetMany(DataLength);//分包算法
            var sendBytes = dataPacket.GetAgentBytes(client);
            await SendAsync(sendBytes);
        }

        private async ValueTask SendAsync(SendBytes<Socket> sendBytes)
        {
            try
            {
                await serverAsync.SendAsync(sendBytes);
            }
            finally
            {
                sendBytes.Dispose();
            }
        }

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="key">发送人的IP</param>
        /// <param name="api">接口调用信息</param>
        public async ValueTask<NetResponse> SendAsync(Ipv4Port key, ApiPacket api)
        {
            return await OnSendWaitOne(key, api);
            //return await Task.Run(() => OnSendWaitOne(key, api)); 
        }

        /// <summary>
        /// 同步发送消息
        /// </summary>
        /// <param name="key">发送人的IP</param>
        /// <param name="api">接口调用信息</param>
        public NetResponse Send(in Ipv4Port key, ApiPacket api)
        {
            var task = OnSendWaitOne(key, api);
            task.Wait();
            return task.Result;
        }

        private async Task<NetResponse> OnSendWaitOne(Ipv4Port key, ApiPacket api)
        {
            Guid clmidmt = Guid.NewGuid();
            if (threadKeyObj.TryThreadObj(in key, in clmidmt, api.IsReply, out var threadUuIdObj, out var _threadObj, out var response))  //FrameCommon.TryThreadObj(_DataPacketThreads, clmidmt, api.IsReply);
            {
                using (_threadObj)
                {
                    try
                    {
                        IsIpParser ipParser = await OnIpParser(key, Ipv4Port.Empty);
                        if (!ipParser.IsOk) throw new("接收方不存在！");

                        using IDataPacket dataPacket = FrameCommon.GetDataPacket(api, clmidmt, true);

                        await SendAsync(dataPacket, ipParser.Client).IsNewTask();
                        if (!_threadObj.WaitOne(api.Millisecond))
                        {
                            threadUuIdObj.SetTimeout(in clmidmt);
                        }
                    }
                    catch (Exception ex)
                    {
                        if (api.IsReply) threadUuIdObj.SetException(in clmidmt, in ex); else _threadObj.SetSendFail(ex);
                    }

                    return _threadObj.GetResponse(in clmidmt);
                }
            }
            else
            {
                return response;
            }
        }

        /**
         * 回调包信息
         */
        private async ValueTask Server_Received(ReceiveBytes<Socket> tcpBytes)
        {
            if (FrameCommon.Receiveds(in tcpBytes, out var dataPacket))
            {
                var poolData = new PoolData(tcpBytes.Key, tcpBytes.Client, dataPacket);
                if (await Server_Received(poolData))
                {
                    poolData.Dispose();
                }
            }
        }

        /**
         * 有效包处理
         */
        private async ValueTask<bool> Server_Received(PoolData poolData)
        {
            try
            {
                //if (json.IsServer) return;
                //if (!FrameCommon.IsComplete(true, ref json)) return;

                await OnComplete(poolData.Key, EnServer.Receive);
                var json = poolData.Packet;

                if (json.IsRelay)//转发 单包无问题 分包 有问题
                {
                    IDataPacket dataPacket = json.Clone();
                    dataPacket.ResetValue(IsServer: true);
                    try
                    {
                        if (IsAllowRelay)
                        {
                            if (json.IsSend)
                            {
                                Ipv4Port _IpPort = dataPacket.IpPort, _Key = poolData.Key;
                                dataPacket.ResetValue(IpPort: _Key);
                                await AgentSendAsync(dataPacket, _IpPort, _Key);
                            }
                            else if (serverAsync.TrySocket(dataPacket.IpPort, out var client))
                            {
                                await AgentSendAsync(dataPacket, client);
                            }
                        }
                        else
                        {
                            throw new Exception("Server:拒绝Relay协议！");
                        }
                    }
                    catch (Exception e)
                    {
                        if (json.IsSend)
                        {
                            dataPacket.ResetValue(IsSend: false);
                            dataPacket.SetErr(e.Message);
                            await AgentSendAsync(dataPacket, poolData.Client);
                        }
                    }
                    finally
                    {
                        dataPacket.Dispose();
                    }
                }
                else if (json.IsSend) //表示收到需要回复发包
                {
                    //System.Threading.ThreadPool.UnsafeQueueUserWorkItem(SetPool, poolData, false);
                    await SetPool(poolData);
                }
                else if (threadKeyObj.Complete(in poolData.Key, json.OnlyId, out ThreadObj Threads))  //_DataPacketThreads.TryRemove(json.OnlyId, out ThreadObj Threads)
                {
                    Threads.Set(in json);
                    return false;
                }
            }
            catch (Exception ex)
            {
                Log.Error("服务器消息异常", ex, "Log/NetFrame");
            }
            return true;

            async ValueTask SetPool(PoolData data)
            {
                try
                {
                    using var dataPacket = await data.RequestAsync();
                    if (dataPacket.IsReply) await SendAsync(dataPacket, data.Client);
                }
                catch (Exception ex)
                {
                    Log.Error("服务器消息异常", ex, "Log/NetFrame");
                }
            }
        }

        private async ValueTask Server_Completed(UserKey arg1, EnServer arg2, DateTime arg3)
        {
            switch (arg2)
            {
                case EnServer.Connect:
                    threadKeyObj.TryAdd(arg1);
                    break;
                case EnServer.ClientClose:
                    threadKeyObj.Release(arg1);
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
        private ValueTask<IGetQueOnEnum> OnComplete(in UserKey key, EnServer enAction)
        {
            if (IsEvent(enAction))
            {
                return EnumEventQueue.OnComplete(in key, enAction, IsQueue(enAction), Completed);
            }
            return IGetQueOnEnum.SuccessAsync;
        }

        private async ValueTask<IsIpParser> OnIpParser(Ipv4Port key, Ipv4Port SponsorIp)
        {
            UserKey strip;
            if (IpParser is not null)
            {
                strip = await IpParser.Invoke(SponsorIp, key);
            }
            else
            {
                strip = key;
            }
            bool isOk = serverAsync.TrySocket(in strip, out var client);
            return new IsIpParser(isOk, client);
        }

        /// <summary>
        /// 关闭服务器并断开所有连接的客户端
        /// </summary>
        public void Close()
        {
            serverAsync.Stop();
            threadKeyObj.Dispose();
        }
    }
}
