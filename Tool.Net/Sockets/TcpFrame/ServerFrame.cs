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
        private readonly TcpServerAsync serverAsync = null;

        /**
         * 各种发生的事件
         */
        private Action<string, EnServer, DateTime> Completed = null;

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
        public string Server { get { return serverAsync.Server; } }

        /// <summary>
        /// 标识服务端连接是否关闭
        /// </summary>
        public bool IsClose { get { return serverAsync.IsClose; } }

        /// <summary>
        /// 已建立连接的集合
        /// key:ip:port
        /// value:TcpClient
        /// </summary>
        public ConcurrentDictionary<string, TcpClient> ListClient
        {
            get { return serverAsync.ListClient; }
        }

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
            serverAsync = new TcpServerAsync(true, this.DataLength + 6) { Millisecond = 0 };//这里就必须加回去

            serverAsync.SetCompleted(Server_Completed);
            serverAsync.SetReceived(Server_Received);

            //_que = new ConcurrentQueue<GetQueOnEnum>();
            //_mre = new ManualResetEvent(false);

            //TaskOnComplete();
        }

        //**
        // * 异步事件处理类
        // */
        //private void TaskOnComplete()
        //{
        //    Task.Factory.StartNew(() =>
        //    {
        //        while (!IsClose)
        //        {
        //            // 等待信号通知
        //            _mre.WaitOne();

        //            GetQueOnEnum getQueOn;
        //            // 判断是否有内容需要执行的事件 从列队中获取内容，并删除列队中的内容
        //            while (_que.Count > 0 && _que.TryDequeue(out getQueOn))
        //            {
        //                try
        //                {
        //                    Completed?.Invoke(getQueOn.Key, getQueOn.ServerEnum);
        //                }
        //                catch
        //                {
        //                }
        //            }

        //            // 重新设置信号
        //            _mre.Reset();
        //            Thread.Sleep(1);
        //        }
        //    }, TaskCreationOptions.LongRunning).ContinueWith((i) => /*_mre.Close();*/ i.Dispose());
        //}

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(Action<string, EnServer, DateTime> Completed)
        {
            if (this.Completed == null)
                this.Completed = Completed;
        }

        ///// <summary>
        ///// 接收到数据事件
        ///// </summary>
        ///// <param name="Received"></param>
        //public void SetReceived(Action<string, object> Received)
        //{
        //    if (this.Received == null)
        //        this.Received = Received;
        //}

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
         * 异步发送消息
         * key 发送人的IP
         * dataPacket 数据包
         */
        private void SendAsync(string key, DataPacket dataPacket)
        {//给定空包为200
            //分包算法
            dataPacket.SetMany(DataLength);

            if (dataPacket.NotIsMany)
            {
                //string Json = dataPacket.StringData(); //Json();
                byte[] listData = dataPacket.ByteData();//Encoding.UTF8.GetBytes(Json);
                //if (listData.Length > DataLength)
                //{
                //    throw new System.SystemException($"发送数据的包大于配置的包体大小！（发送包大小{listData.Length},本该最大大小{DataLength}。）");
                //}
                serverAsync.SendAsync(key, listData);
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
                byte[][] buffers = dataPacket.ByteDatas();
                serverAsync.SendAsync(key, buffers);
            }
            dataPacket.Dispose();
        }

        /**
         * 异步代理发送消息
         * key 发送人的IP
         * dataPacket 数据包
         */
        private void AgentSendAsync(string key, DataPacket dataPacket)
        {
            dataPacket.SetMany(DataLength);//分包算法
            byte[] listData = dataPacket.ByteData();
            serverAsync.SendAsync(key, listData);
            dataPacket.Dispose();
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

            DataPacket dataPacket = TcpResponse.GetDataPacket(api, null, true, true);

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

            DataPacket dataPacket = TcpResponse.GetDataPacket(api, null, true, false);

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
                _threadObj.AutoReset.Set();
                _DataPacketThreads.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt));
            }
            else
            {
                _DataPacketThreads.TryAdd(clmidmt, _threadObj = new ThreadObj(clmidmt));
            }

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

            _threadObj.Response.IsAsync = dataPacket.IsAsync;//是异步的？
            try
            {
                SendAsync(key, dataPacket);
                if (!_threadObj.AutoReset.WaitOne(Millisecond, true))
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
        private void Server_Received(string arg1, byte[] arg2)
        {
            if (arg2.Length < 23 )
            {
                if (arg2[1] != TcpResponse.KeepAlive[1] &&
                    arg2[2] != TcpResponse.KeepAlive[2] &&
                    arg2[3] != TcpResponse.KeepAlive[3] &&
                    arg2[4] != TcpResponse.KeepAlive[4] &&
                    arg2[5] != TcpResponse.KeepAlive[5] &&
                    arg2[6] != TcpResponse.KeepAlive[6]) return;
                OnComplete(arg1, EnServer.HeartBeat);
            }
            else
            {
                TcpResponse.Receiveds(arg1, arg2, Server_Received);
            }

            
            //string msg = Encoding.UTF8.GetString(arg2, 0, arg2.Length);
            //if (!string.IsNullOrEmpty(msg))
            //{
            //    TcpResponse.Receiveds(arg1, msg, Server_Received);
            //}
        }

        /**
         * 有效包处理
         */
        private void Server_Received(string key, DataPacket json)
        {
            try
            {
                if (json.IsServer) return;

                if (!TcpResponse.IsComplete(ref json, _DataPacketThreads, Lock)) return;

                OnComplete(key, EnServer.Receive);
                if (json.IsSend)
                {
                    if (json.IsIpIdea)
                    {
                        json.IsServer = true;
                        try
                        {
                            string _IpPort = json.IpPort;
                            json.IpPort = key;
                            AgentSendAsync(_IpPort, json);
                        }
                        catch (Exception e)
                        {
                            json.IsErr = true;
                            json.IsSend = false;
                            json.Obj = e.Message;
                            AgentSendAsync(key, json);
                        }
                    }
                    else if (DataTcp.DicDataTcps.TryGetValue(TcpResponse.GetActionID(json), out DataTcp dataTcp))
                    {
                        DataBase handler = dataTcp.NewClass.Invoke();// Activator.CreateInstance(dataTcp.Action.Method.DeclaringType) as DataBase;
                        using (handler)
                        {
                            DataPacket dataPacket = handler.Request(json, key, dataTcp);
                            SendAsync(key, dataPacket);
                        }
                    }
                    else
                    {
                        DataPacket dataPacket = new()
                        {
                            ClassID = json.ClassID,
                            ActionID = json.ActionID,
                            OnlyId = json.OnlyId,
                            //OnlyID = json.OnlyID,
                            IsSend = false,
                            IsErr = true,
                            IsServer = true,
                            IsAsync = json.IsAsync,
                            IpPort = json.IpPort,
                            Obj = "接口不存在",
                        };
                        SendAsync(key, dataPacket);
                    }
                }
                else
                {
                    if (json.IsIpIdea)
                    {
                        json.IsServer = true;
                        try
                        {
                            AgentSendAsync(json.IpPort, json);
                        }
                        catch { }
                    }
                    else //if (json.Many.Equals(new Range()))
                    {
                        if (_DataPacketThreads.TryRemove(json.OnlyID, out ThreadObj Threads))
                        {
                            Threads.Response.OnTcpFrame = json.IsErr ? TcpFrameState.Exception : TcpFrameState.Success;
                            if (!json.IsErr)
                            {
                                Threads.Response.Text = json.Obj;
                                Threads.Response.Bytes = json.Bytes;
                            }
                            else
                            {
                                Threads.Response.Exception = new Exception(json.Obj);
                            }
                            Threads.Response.IsAsync = json.IsAsync;//是异步的？
                            Threads.AutoReset.Set();
                        }
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
                json.Dispose();
            }
            catch (Exception ex)
            {
                Log.Error("服务器消息异常", ex, "Log/TcpFrame");
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

        /// <summary>
        /// 关闭服务器并断开所有连接的客户端
        /// </summary>
        public void Close()
        {
            serverAsync.Close();
        }
    }
}
