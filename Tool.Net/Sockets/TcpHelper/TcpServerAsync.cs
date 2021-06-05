using System;
using System.Collections.Concurrent;
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
    /// 封装一个底层异步TCP对象（服务端）
    /// </summary>
    public class TcpServerAsync : IDisposable
    {
        private readonly int DataLength = 1024 * 8;
        private TcpListener listener;
        //用于控制异步接受连接
        private readonly ManualResetEvent doConnect = new(false);
        ////用于控制异步接收数据
        //private readonly ManualResetEvent doReceive = new ManualResetEvent(false);
        //标识服务端连接是否关闭
        private bool isClose = false;
        private ConcurrentDictionary<string, TcpClient> listClient = new();

        /// <summary>
        /// 是否保证数据唯一性，开启后将采用框架验证保证其每次的数据唯一性，（如果不满足数据条件将直接与其断开连接）
        /// </summary>
        public bool OnlyData { get; }

        /// <summary>
        /// 标识服务端连接是否关闭
        /// </summary>
        public bool IsClose { get { return isClose; } }

        /// <summary>
        /// 已建立连接的集合
        /// key:ip:port
        /// value:TcpClient
        /// </summary>
        public ConcurrentDictionary<string, TcpClient> ListClient
        {
            get { return listClient; }
            private set { listClient = value; }
        }

        //服务端IP
        private string server = string.Empty;
        private int millisecond = 20; //默认20毫秒。

        /// <summary>
        /// 服务器创建时的信息
        /// </summary>
        public string Server { get { return server; } }

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

        /**
         * 连接、发送、关闭事件
         */
        private Action<string, EnSocketAction> Completed; //event

        /**
         * 接收到数据事件
         */
        private Action<string, byte[]> Received; //event

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
        /// 创建一个TCP服务器类
        /// </summary>
        public TcpServerAsync()
        {

        }

        /// <summary>
        /// 创建一个TCP服务器类，并确定是否开启框架验证模式保证数据唯一性。
        /// </summary>
        /// <param name="OnlyData">是否启动框架模式</param>
        public TcpServerAsync(bool OnlyData) : this(OnlyData, 8 * 1024)
        {

        }

        /// <summary>
        /// 创建一个TCP服务器类，设置流大小
        /// </summary>
        /// <param name="DataLength">每次包的最大大小(警告：请务必保证服务端的大小和客户端一致)</param>
        public TcpServerAsync(int DataLength) : this(false, DataLength)
        {
        }

        /// <summary>
        /// 创建一个TCP服务器类，确认模式和设置流大小
        /// </summary>
        /// <param name="OnlyData">是否启动框架模式</param>
        /// <param name="DataLength">每次包的最大大小(警告：请务必保证服务端的大小和客户端一致)</param>
        public TcpServerAsync(bool OnlyData, int DataLength)
        {
            if (DataLength <= 8 * 1024)
            {
                throw new ArgumentException("DataLength 值必须大于8KB！", nameof(DataLength));
            }
            if (DataLength > 1024 * 1024 * 20)
            {
                throw new ArgumentException("DataLength 值必须是在20M(DataLength < 1024 * 1024 * 20)以内！", nameof(DataLength));
            }
            this.DataLength = DataLength;
            this.OnlyData = OnlyData;
        }

        /// <summary>
        /// 开始异步监听ip地址的端口
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        public void StartAsync(string ip, int port)
        {
            IPAddress ipAddress = null;
            try
            {
                ipAddress = IPAddress.Parse(ip);
            }
            catch (Exception)
            {
                throw;
            }
            listener = new TcpListener(new IPEndPoint(ipAddress, port));
            server = $"{ip}:{port}";
            try
            {
                listener.Start();
                OnComplete(server, EnSocketAction.Create);
            }
            catch (Exception e)
            {
                OnComplete(server, EnSocketAction.Fail);
                throw new Exception("连接服务器时发生异常！", e);
            }

            Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.Name = $"Tcp服务端-监听({port})";
                while (!isClose)
                {
                    doConnect.Reset();
                    listener.BeginAcceptTcpClient(AcceptCallBack, listener);
                    doConnect.WaitOne();
                }
            }, TaskCreationOptions.LongRunning).ContinueWith((i) => i.Dispose());

            //ThreadPool.QueueUserWorkItem(x =>
            //{
            //    while (!isClose)
            //    {
            //        doConnect.Reset();
            //        listener.BeginAcceptTcpClient(AcceptCallBack, listener);
            //        doConnect.WaitOne();
            //    }
            //});
        }

        /// <summary>
        /// 开始异步监听本机127.0.0.1的端口号
        /// </summary>
        /// <param name="port"></param>
        public void StartAsync(int port)
        {
            StartAsync("127.0.0.1", port);
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="key">客户端的ip地址和端口号</param>
        /// <param name="msg">要发送的内容</param>
        public void SendAsync(string key, string msg)
        {
            if (ListClient.TryGetValue(key, out TcpClient client))
            {
                SendAsync(client, msg);
            }
            else
            {
                throw new Exception("在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。");//在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。
            }
        }

        /**
         * 开始异步发送数据
         * key 客户端的ip地址和端口号
         * Data 要发送的内容
         */
        internal void SendAsync(string key, params byte[][] Data)
        {
            if (ListClient.TryGetValue(key, out TcpClient client))
            {
                SendAsync(client, Data);
            }
            else
            {
                throw new Exception("在当前服务端找不到该客户端信息，可能是当前用户已经断开连接。");
            }
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="client">客户端的ip地址和端口号</param>
        /// <param name="msg">要发送的内容</param>
        public void SendAsync(TcpClient client, string msg)
        {
            byte[] listData = Encoding.UTF8.GetBytes(msg);
            SendAsync(client, listData);
        }

        /// <summary>
        /// 开始异步发送数据
        /// </summary>
        /// <param name="client">客户端的ip地址和端口号</param>
        /// <param name="listData">要发送的内容，允许多个包</param>
        public void SendAsync(TcpClient client, params byte[][] listData)// byte[] listData
        {
            if (client == null)
            {
                throw new ArgumentException("client 对象是空的！", nameof(client));
            }
            if (!TcpStateObject.IsConnected(client))
            {
                throw new Exception("与客户端的连接已断开！");
            }

            IList<ArraySegment<byte>> buffers = TcpStateObject.GetBuffers(this.OnlyData, DataLength, listData);

            client.Client.BeginSend(buffers, SocketFlags.None, SendCallBack, client);

            //if (this.OnlyData)
            //{
            //    byte[] Data = TcpStateObject.GetDataSend(listData[0], DataLength);

            //    if (client == null)
            //    {
            //        throw new ArgumentException("client 对象是空的！", nameof(client));
            //    }
            //    if (!TcpStateObject.IsConnected(client))
            //    {
            //        throw new Exception("与客户端的连接已断开！");
            //    }
            //    client.Client.BeginSend(Data, 0, Data.Length, SocketFlags.None, SendCallBack, client);
            //}
            //else
            //{
            //    if (client == null)
            //    {
            //        throw new ArgumentException("client 对象是空的！", nameof(client));
            //    }
            //    if (!TcpStateObject.IsConnected(client))
            //    {
            //        throw new Exception("与客户端的连接已断开！");
            //    }
            //    client.Client.BeginSend(listData[0], 0, listData.Length, SocketFlags.None, SendCallBack, client);
            //}
        }

        /**
         * 开始异步接收数据
         * obj 要接收的客户端包体
         */
        private void ReceiveAsync(TcpStateObject obj)
        {
            obj.doReceive.Reset();
            if (TcpStateObject.IsConnected(obj.Client))
            {
                try
                {
                    obj.SocketClient.BeginReceive(obj.ListData, obj.WriteIndex, obj.ListData.Length - obj.WriteIndex, SocketFlags.None, ReceiveCallBack, obj);
                }
                catch (Exception)
                {

                }
                obj.doReceive.WaitOne();
            }
        }

        /**
         * 异步接收连接的回调函数
         */
        private void AcceptCallBack(IAsyncResult ar)
        {
            TcpListener l = ar.AsyncState as TcpListener;
            if (isClose)
            {
                doConnect.Set();
                foreach (var _client in listClient)
                {
                    string IpPort = TcpStateObject.GetIpPort(_client.Value);
                    _client.Value.Close();
                    OnComplete(IpPort, EnSocketAction.Close);
                }
                listClient.Clear();
                OnComplete(server, EnSocketAction.MonitorClose);
                return;
            }

            TcpClient client = l.EndAcceptTcpClient(ar);
            doConnect.Set();

            string key = TcpStateObject.GetIpPort(client);
            if (ListClient.TryAdd(key, client))
            {
                StartReceive(key, client);
                OnComplete(key, EnSocketAction.Connect);
            }
        }

        /**
         * 启动新线程，用于专门接收消息
         */
        private void StartReceive(string key, TcpClient client)
        {
            Task.Factory.StartNew(() =>
            {
                Thread.CurrentThread.Name = "Tcp服务端-业务";
                //用于控制异步接收数据
                ManualResetEvent doReceive = new(false);
                TcpStateObject obj = new(client, this.DataLength) { doReceive = doReceive };
                while (ListClient.TryGetValue(key, out client) && !isClose)
                {
                    if (TcpStateObject.IsConnected(client))
                    {
                        Thread.Sleep(Millisecond);
                        ReceiveAsync(obj);
                        Thread.Sleep(Millisecond);
                    }
                    else
                    {
                        if (ListClient.TryRemove(key, out client))
                        {
                            client.Close();
                            OnComplete(key, EnSocketAction.Close);
                        }
                        break;
                    }
                }
                obj.Close();
            }, TaskCreationOptions.LongRunning).ContinueWith((i) => i.Dispose());

            //ThreadPool.QueueUserWorkItem(x =>
            //{
            //    //用于控制异步接收数据
            //    ManualResetEvent doReceive = new ManualResetEvent(false);
            //    StateObject obj = new StateObject(client, this.DataLength) { doReceive = doReceive };
            //    while (ListClient.TryGetValue(key, out client) && !isClose)
            //    {
            //        if (StateObject.IsConnected(client))
            //        {
            //            Thread.Sleep(Millisecond);
            //            ReceiveAsync(obj);
            //            Thread.Sleep(Millisecond);
            //        }
            //        else
            //        {
            //            if (ListClient.TryRemove(key, out client))
            //            {
            //                client.Close();
            //                OnComplete(key, EnSocketAction.Close);
            //            }
            //            break;
            //        }
            //    }
            //    doReceive.Close();
            //});
        }

        /**
         * 异步发送数据的回调函数
         */
        private void SendCallBack(IAsyncResult ar)
        {
            TcpClient client = ar.AsyncState as TcpClient;
            string key = TcpStateObject.GetIpPort(client);
            try
            {
                int count = client.Client.EndSend(ar);
                OnComplete(key, EnSocketAction.SendMsg);
            }
            catch (Exception)
            {
                client.Close();
                OnComplete(server, EnSocketAction.MonitorClose);
            }
            
        }

        /**
         * 异步接收数据的回调函数
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

                if (count == 0)
                {
                    obj.Client.Close();
                }
                else if (this.OnlyData)
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
            //        Received?.Invoke(obj.IpPort, msg);//触发接收事件
            //    }
            //}
        }

        /// <summary>
        /// 可供开发重写的事件方法
        /// </summary>
        /// <param name="key">指定发送对象</param>
        /// <param name="enAction">消息类型</param>
        public virtual void OnComplete(string key, EnSocketAction enAction)
        {
            Completed?.Invoke(key, enAction);
        }

        /// <summary>
        /// TCP关闭
        /// </summary>
        public void Close()
        {
            isClose = true;
            if (listener != null)
            {
                listener.Stop();//当他不在监听，就关闭监听。
            }
        }

        void IDisposable.Dispose()
        {
            Close();

            listClient.Clear();

            listClient = null;
            //listener.Server.Dispose();
            //((IDisposable)listener.Server).Dispose();
            doConnect.Close();
            //_mre.Close();
            GC.SuppressFinalize(this);
        }
    }
}
