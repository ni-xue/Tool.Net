﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Tool.Sockets.SupportCode;
using Tool.Utils;

namespace Tool.Sockets.TcpHelper
{
    /// <summary>
    /// 封装一个底层异步TCP对象（客户端）
    /// </summary>
    public class TcpClientAsync
    {
        private readonly int DataLength = 1024 * 8;

        private TcpClient client;

        /**
         * 客户端连接完成、发送完成、连接异常或者服务端关闭触发的事件
         */
        private Action<string, EnSocketAction> Completed; //event

        /**
         * 客户端接收消息触发的事件
         */
        private Action<string, byte[]> Received; //event Span<byte>

        //标识客户端是否关闭
        private bool isClose = false;

        //服务端IP
        private string server = string.Empty;
        private int millisecond = 20; //默认20毫秒。

        /// <summary>
        /// 表示通讯的包大小
        /// </summary>
        public TcpBufferSize BufferSize { get; } = TcpBufferSize.Size8K;

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        /// <summary>
        /// 是否在与服务器断开后主动重连？ 
        /// </summary>
        public bool IsReconnect { get; private set; }

        /// <summary>
        /// 服务器的连接信息
        /// </summary>
        public string Server { get { return server; } }

        /// <summary>
        /// 当前设备的连接信息
        /// </summary>
        public string LocalPoint
        {
            get
            {
                if (TcpStateObject.IsConnected(client))
                {
                    return Client.Client.LocalEndPoint.ToString();
                }
                else
                {
                    return "0.0.0.0:0";
                }
            }
        }

        /// <summary>
        /// TCP 服务对象
        /// </summary>
        public TcpClient Client { get { return client; } }

        /// <summary>
        /// 标识客户端是否关闭，改状态为调用关闭方法后的状态。
        /// </summary>
        public bool IsClose { get { return isClose; } }

        /// <summary>
        /// 获取一个值，该值指示 Client 的基础 Socket 是否已连接到远程主机。
        /// </summary>
        public bool Connected { get { return client.Connected; } }

        /// <summary>
        /// 监听控制毫秒
        /// </summary>
        public int Millisecond
        {
            get
            {
                return millisecond;
            }
            set
            {
                if (value > 60 * 1000) { millisecond = 60 * 1000; }
                else if (value < 0) { millisecond = 0; }
                else { millisecond = value; }
            }
        }

        /// <summary>
        /// 连接、发送、关闭事件
        /// </summary>
        /// <param name="Completed"></param>
        public void SetCompleted(Action<string, EnSocketAction> Completed)
        {
            if (this.Completed == null)
                this.Completed = Completed;
        }

        /// <summary>
        /// 接收到数据事件
        /// </summary>
        /// <param name="Received"></param>
        public void SetReceived(Action<string, byte[]> Received)
        {
            if (this.Received == null)
                this.Received = Received;
        }

        /// <summary>
        /// 创建一个TCP客户端类
        /// </summary>
        public TcpClientAsync() : this(TcpBufferSize.Default, 1024 * 8)
        {
        }

        /// <summary>
        /// 创建一个TCP客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        public TcpClientAsync(TcpBufferSize bufferSize) : this(bufferSize, 1024 * 8)
        {
        }

        /// <summary>
        /// 创建一个TCP客户端类，设置流大小
        /// </summary>
        /// <param name="DataLength">每次包的最大大小(警告：请务必保证客户端的大小和客户端一致)</param>
        public TcpClientAsync(int DataLength) : this(TcpBufferSize.Default, DataLength)
        {
        }

        /// <summary>
        /// 创建一个TCP客户端类，设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        /// <param name="DataLength">每次包的最大大小(警告：请务必保证客户端的大小和客户端一致)</param>
        public TcpClientAsync(TcpBufferSize bufferSize, int DataLength) : this(bufferSize, false, DataLength)
        {
        }

        /// <summary>
        /// 创建一个TCP客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        /// <param name="OnlyData">是否启动框架模式</param>
        /// <param name="DataLength">每次包的最大大小(警告：请务必保证客户端的大小和客户端一致)</param>
        public TcpClientAsync(TcpBufferSize bufferSize, bool OnlyData, int DataLength) : this(bufferSize, OnlyData, DataLength, false)
        {
        }

        /// <summary>
        /// 创建一个TCP客户端类，确认模式和设置流大小
        /// </summary>
        /// <param name="bufferSize">包大小枚举</param>
        /// <param name="OnlyData">是否启动框架模式</param>
        /// <param name="DataLength">每次包的最大大小(警告：请务必保证客户端的大小和客户端一致)</param>
        /// <param name="IsReconnect">是否在与服务器断开后主动重连？ </param>
        public TcpClientAsync(TcpBufferSize bufferSize, bool OnlyData, int DataLength, bool IsReconnect)
        {
            if (DataLength < 8 * 1024)
            {
                throw new ArgumentException("DataLength 值必须大于8KB！", nameof(DataLength));
            }
            if (DataLength > 1024 * 1024 * 20)
            {
                throw new ArgumentException("DataLength 值必须是在20M(DataLength < 1024 * 1024 * 20)以内！", nameof(DataLength));
            }
            this.BufferSize = bufferSize;
            if (TcpBufferSize.Default == this.BufferSize)
            {
                client = new TcpClient(AddressFamily.InterNetwork);
            }
            else
            {
                client = new TcpClient(AddressFamily.InterNetwork) { ReceiveBufferSize = (int)this.BufferSize, SendBufferSize = (int)this.BufferSize };
            }
            this.DataLength = DataLength;
            this.OnlyData = OnlyData;
            this.IsReconnect = IsReconnect;
        }

        /// <summary>
        /// 重连，返回是否重连，如果没有断开是不会重连的
        /// </summary>
        public bool Reconnection()
        {
            try
            {
                if (!isClose)
                {
                    if (!TcpStateObject.IsConnected(client))
                    {
                        string[] Connect = server.Split(':');
                        string ip = Connect[0];
                        int port = Connect[1].ToInt();
                        client.Close();
                        if (TcpBufferSize.Default == this.BufferSize)
                        {
                            client = new TcpClient();
                        }
                        else
                        {
                            client = new TcpClient() { ReceiveBufferSize = (int)this.BufferSize, SendBufferSize = (int)this.BufferSize };
                        }
                        client.Connect(ip, port);
                        if (TcpStateObject.IsConnected(client))
                        {
                            StartReceive(server, client);
                            OnComplete(server, EnSocketAction.Connect);
                        }
                        else
                        {
                            InsideClose();
                            OnComplete(server, EnSocketAction.Fail);
                        }
                    }
                }
                return true;
            }
            catch
            {
                InsideClose();
                return false;
            }
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="ip">要连接的服务器的ip地址</param>
        /// <param name="port">要连接的服务器的端口</param>
        public void Connect(string ip, int port)
        {
            IPAddress ipAddress;
            try
            {
                ipAddress = IPAddress.Parse(ip);
            }
            catch (Exception)
            {
                throw new Exception("ip地址格式不正确，请使用正确的ip地址！");
            }
            server = ip + ":" + port;

            try
            {
                client.Connect(ipAddress, port);
            }
            catch //(Exception ex)
            {
                throw;
            }
            finally
            {
                ActionConnect(client);
            }
        }

        /// <summary>
        /// 异步连接，连接ip地址为127.0.0.1
        /// </summary>
        /// <param name="port">要连接服务端的端口</param>
        public void Connect(int port)
        {
            Connect("127.0.0.1", port);
        }

        /// <summary>
        /// 异步连接
        /// </summary>
        /// <param name="ip">要连接的服务器的ip地址</param>
        /// <param name="port">要连接的服务器的端口</param>
        public void ConnectAsync(string ip, int port)
        {
            IPAddress ipAddress = null;
            try
            {
                ipAddress = IPAddress.Parse(ip);
            }
            catch (Exception)
            {
                throw new Exception("ip地址格式不正确，请使用正确的ip地址！");
            }
            server = ip + ":" + port;
            client.BeginConnect(ipAddress, port, ConnectCallBack, client);
        }

        /// <summary>
        /// 异步连接，连接ip地址为127.0.0.1
        /// </summary>
        /// <param name="port">要连接服务端的端口</param>
        public void ConnectAsync(int port)
        {
            ConnectAsync("127.0.0.1", port);
        }

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendAsync(string msg)
        {
            byte[] listData = Encoding.UTF8.GetBytes(msg);
            SendAsync(listData);
            //client.Client.BeginSend(listData, 0, listData.Length, SocketFlags.None, SendCallBack, client);
        }

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="listData">数据包</param>
        public void SendAsync(params byte[][] listData)
        {
            if (!TcpStateObject.IsConnected(client))
            {
                throw new Exception("与服务端的连接已中断！");
            }

            IList<ArraySegment<byte>> buffers = TcpStateObject.GetBuffers(this.OnlyData, DataLength, listData);

            client.Client.BeginSend(buffers, SocketFlags.None, SendCallBack, client);

            //if (this.OnlyData)
            //{
            //    byte[] Data = TcpStateObject.GetDataSend(listData, DataLength);

            //    if (!TcpStateObject.IsConnected(client))
            //    {
            //        throw new Exception("与服务端的连接已中断！");
            //    }
            //    client.Client.BeginSend(Data, 0, Data.Length, SocketFlags.None, SendCallBack, client);
            //}
            //else
            //{
            //    if (!TcpStateObject.IsConnected(client))
            //    {
            //        throw new Exception("与服务端的连接已中断！");
            //    }
            //    client.Client.BeginSend(listData, 0, listData.Length, SocketFlags.None, SendCallBack, client);
            //}
        }

        /**
         * 异步接收消息
         */
        private void ReceiveAsync(TcpStateObject obj)
        {
            obj.doReceive.Reset();
            if (TcpStateObject.IsConnected(obj.Client))
            {
                try
                {
                    //obj.SocketClient.BeginReceive(obj.vs, SocketFlags.None, ReceiveCallBack, obj);

                    obj.SocketClient.BeginReceive(obj.ListData, obj.WriteIndex, obj.ListData.Length - obj.WriteIndex, SocketFlags.None, ReceiveCallBack, obj);
                    obj.doReceive.WaitOne();
                }
                catch (Exception)
                {
                    obj.Client.Close();
                }
            }
        }

        /**
         * 异步连接的回调函数
         */
        private void ConnectCallBack(IAsyncResult ar)
        {
            TcpClient client = ar.AsyncState as TcpClient;
            try
            {
                ActionConnect(client);
            }
            catch (Exception)
            {
                InsideClose();
                OnComplete(server, EnSocketAction.Fail);
            }
            finally
            {
                if (client.Connected)
                    client?.EndConnect(ar);
            }
        }

        /**
         * 连接的公共回调函数
         */
        private void ActionConnect(TcpClient client)
        {
            if (TcpStateObject.IsConnected(client))
            {
                //OnComplete(client, EnSocketAction.Create);
                string key = server; //StateObject.GetIpPort(client);
                                     //client.NoDelay = true;
                StartReceive(key, client);
                OnComplete(key, EnSocketAction.Connect);
            }
            else
            {
                InsideClose();
                OnComplete(server, EnSocketAction.Fail);

                if (this.IsReconnect)
                {
                    WhileReconnect();
                }
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private void StartReceive(string key, TcpClient client)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.Name ??= "Tcp客户端-业务";
                //用于控制异步接收消息
                ManualResetEvent doReceive = new(false);
                //接收数据包
                TcpStateObject obj = new(client, this.DataLength) { doReceive = doReceive };
                while (!isClose)
                {
                    if (TcpStateObject.IsConnected(client))
                    {
                        Thread.Sleep(Millisecond);
                        ReceiveAsync(obj);
                        Thread.Sleep(Millisecond);
                    }
                    else
                    {
                        //如果发生异常，说明客户端失去连接，触发关闭事件
                        InsideClose();
                        OnComplete(obj.IpPort, EnSocketAction.Close);
                        if (this.IsReconnect)
                        {
                            WhileReconnect();
                        }
                        break;
                    }
                }
                obj.Close();
            }, TaskCreationOptions.LongRunning).ContinueWith((i) => i.Dispose());

            //ThreadPool.QueueUserWorkItem(x =>
            //{
            //    //接收数据包
            //    StateObject obj = new StateObject(client, this.DataLength) { doReceive = doReceive };
            //    while (!isClose)
            //    {
            //        if (StateObject.IsConnected(client))
            //        {
            //            Thread.Sleep(Millisecond);
            //            ReceiveAsync(obj);
            //            Thread.Sleep(Millisecond);
            //        }
            //        else
            //        {
            //            //如果发生异常，说明客户端失去连接，触发关闭事件
            //            Close();
            //            OnComplete(obj.IpPort, EnSocketAction.Close);
            //            break;
            //        }
            //    }

            //    //client.Close();
            //});
        }

        private void WhileReconnect()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.Name ??= "Tcp客户端-重连";
                bool _reconnect = true;
                while (_reconnect && IsReconnect)
                {
                    if (Reconnection())
                    {
                        _reconnect = false;
                        break;
                    }
                    Thread.Sleep(20);
                }
            }, TaskCreationOptions.LongRunning);
        }

        /**
         * 异步接收消息的回调函数
         */
        private void ReceiveCallBack(IAsyncResult ar)
        {
            TcpStateObject obj = ar.AsyncState as TcpStateObject;
            int count = -1;
            byte[] ListData = null;
            try
            {
                if (TcpStateObject.IsConnected(obj.Client))
                    count = obj.SocketClient.EndReceive(ar);

                if (count == 0) obj.Client.Close();

                if (this.OnlyData)
                {
                    if (obj.WriteIndex + count > 6)
                    {
                    Verify:
                        byte[] headby = new byte[6];
                        Array.Copy(obj.ListData, 0, headby, 0, 6); 
                        int head = TcpStateObject.GetDataHead(headby);

                        if (head != -1)
                        {
                            if (head + 6 > obj.ListData.Length)
                            {
                                obj.Client.Close();
                                obj.doReceive.Set();
                                return;
                            }
                            int writeIndex = (count + obj.WriteIndex) - (head + 6);
                            if (writeIndex > -1)
                            {
                                ListData = new byte[head];
                                Array.Copy(obj.ListData, 6, ListData, 0, head);

                                obj.WriteIndex = writeIndex;

                                if (obj.WriteIndex > 0)
                                {
                                    byte[] NewData = new byte[obj.WriteIndex];
                                    Array.Copy(obj.ListData, head + 6, NewData, 0, obj.WriteIndex);
                                    Array.Clear(obj.ListData, 0, obj.WriteIndex + head + 6);
                                    Array.Copy(NewData, 0, obj.ListData, 0, obj.WriteIndex);
                                }
                                else
                                {
                                    Array.Clear(obj.ListData, 0, head + 6);
                                }

                                //try
                                //{
                                //    ThreadPool.QueueUserWorkItem(x =>
                                //    {
                                //        Received?.Invoke(obj.IpPort, x as byte[]);//触发接收事件
                                //    }, ListData);
                                //}
                                //catch (Exception ex)
                                //{
                                //    Log.Error("多包线程池异常", ex, "Log/Tcp");
                                //}

                                TcpStateObject.OnReceived(obj.IpPort, ListData, Received);

                                if (obj.WriteIndex > 6)
                                {
                                    count = 0;
                                    goto Verify;
                                }
                            }
                            else
                            {
                                obj.WriteIndex = (count + obj.WriteIndex);
                                obj.doReceive.Set();
                                return;
                            }
                        }
                        else
                        {
                            obj.Client.Close();
                        }
                    }
                }
                else
                {
                    ListData = new byte[count];
                    Array.Copy(obj.ListData, 0, ListData, 0, count);
                    Array.Clear(obj.ListData, 0, count);
                    obj.WriteIndex = 0;
                    //try
                    //{
                    //    ThreadPool.QueueUserWorkItem(x =>
                    //    {
                    //        Received?.Invoke(obj.IpPort, x as byte[]);//触发接收事件
                    //    }, ListData);
                    //}
                    //catch (Exception ex)
                    //{
                    //    Log.Error("多包线程池异常", ex, "Log/Tcp");
                    //}

                    TcpStateObject.OnReceived(obj.IpPort, ListData, Received);
                }
                obj.doReceive.Set();
            }
            catch (Exception)
            {
                obj.Client.Close();
                obj.doReceive.Set();
                return;
            }
            //if (count > 0)
            //{
            //    string msg = Encoding.UTF8.GetString(ListData, 0, count);
            //    if (!string.IsNullOrEmpty(msg))
            //    {
            //        Received?.Invoke(obj.IpPort, msg);
            //    }
            //}
        }

        private void SendCallBack(IAsyncResult ar)
        {
            TcpClient client = ar.AsyncState as TcpClient;

            try
            {
                int count = client.Client.EndSend(ar);
                //string key = StateObject.GetIpPort(client);
                OnComplete(server, EnSocketAction.SendMsg);
            }
            catch (Exception)
            {
                //如果发生异常，说明客户端失去连接，触发关闭事件
                InsideClose();
                //string key = StateObject.GetIpPort(client);
                OnComplete(server, EnSocketAction.Close);
            }

        }

        /// <summary>
        /// 可供开发重写的事件方法
        /// </summary>
        /// <param name="IpPort">IP：端口</param>
        /// <param name="enAction">消息类型</param>
        public virtual void OnComplete(string IpPort, EnSocketAction enAction)
        {
            Completed?.Invoke(IpPort, enAction);
        }

        /// <summary>
        /// TCP关闭
        /// </summary>
        void InsideClose()
        {
            if (client != null)
            {
                client.Close();
            }
        }

        /// <summary>
        /// TCP关闭
        /// </summary>
        public void Close()
        {
            IsReconnect = false;
            isClose = true;
            InsideClose();
        }
    }
}