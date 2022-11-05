using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Tool.Sockets.SupportCode;

namespace Tool.Sockets.UdpHelper
{
    /// <summary>
    /// 封装一个底层异步Udp对象（客户端）
    /// </summary>
    public class UdpClientAsync
    {
        private readonly UdpClient client;
        /// <summary>
        /// 客户端连接完成、发送完成、连接异常或者服务端关闭触发的事件
        /// </summary>
        public event Action<UdpClient, EnSocketAction> Completed;
        /// <summary>
        /// 客户端接收消息触发的事件
        /// </summary>
        public event Action<string, string> Received;
        /// <summary>
        /// 用于控制异步接收消息
        /// </summary>
        private readonly ManualResetEvent doReceive = new(false);
        //标识客户端是否关闭
        private bool isClose = false;

        /// <summary>
        /// 无参构造
        /// </summary>
        public UdpClientAsync()
        {
            client = new UdpClient();
        }

        /// <summary>
        /// 发送数据目标
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
            client.Connect(ipAddress, port);
            OnComplete(client, EnSocketAction.Connect);
        }

        /// <summary>
        /// 发送数据目标，指定ip地址为127.0.0.1
        /// </summary>
        /// <param name="port">要连接服务端的端口</param>
        public void Connect(int port)
        {
            Connect("127.0.0.1", port);
        }

        /// <summary>
        /// 异步接收消息
        /// </summary>
        private void ReceiveAsync()
        {
            doReceive.Reset();
            UdpStateObject obj = new UdpStateObject
            {
                Client = client
            };

            client.BeginReceive(ReceiveCallBack, obj);
            doReceive.WaitOne();
        }

        /// <summary>
        /// 异步发送消息
        /// </summary>
        /// <param name="msg"></param>
        public void SendAsync(string msg)
        {
            byte[] listData = Encoding.UTF8.GetBytes(msg);
            client.BeginSend(listData, listData.Length, SendCallBack, client);
        }

        ///// <summary>
        ///// 异步连接的回调函数
        ///// </summary>
        ///// <param name="ar"></param>
        //private void ConnectCallBack(IAsyncResult ar)
        //{
        //    UdpClient client = ar.AsyncState as UdpClient;
        //    client.Client.EndConnect(ar);
        //    OnComplete(client, EnSocketAction.Connect);
        //}

        /// <summary>
        /// 异步接收消息的回调函数
        /// </summary>
        /// <param name="ar"></param>
        private void ReceiveCallBack(IAsyncResult ar)
        {
            UdpStateObject obj = ar.AsyncState as UdpStateObject;
            IPEndPoint iep = obj.Client.Client.RemoteEndPoint as IPEndPoint;
            int count = -1;
            try
            {
                obj.ListData = obj.Client.EndReceive(ar, ref iep);
                count = obj.ListData.Length;
                doReceive.Set();
            }
            catch (Exception)
            {
                //如果发生异常，说明客户端失去连接，触发关闭事件
                Close();
                OnComplete(obj.Client, EnSocketAction.Close);
            }
            if (count > 0)
            {
                string msg = Encoding.UTF8.GetString(obj.ListData, 0, count);
                if (!string.IsNullOrEmpty(msg))
                {
                    if (Received != null)
                    {
                        string key = string.Format("{0}:{1}", iep.Address, iep.Port);
                        Received(key, msg);
                    }
                }
            }
        }
        private void SendCallBack(IAsyncResult ar)
        {
            UdpClient client = ar.AsyncState as UdpClient;
            try
            {
                client.EndSend(ar);
                OnComplete(client, EnSocketAction.SendMsg);
            }
            catch (Exception)
            {
                //如果发生异常，说明客户端失去连接，触发关闭事件
                Close();
                OnComplete(client, EnSocketAction.Close);
            }
        }

        /// <summary>
        /// 可供开发重写的实现类
        /// </summary>
        /// <param name="client">链接对象</param>
        /// <param name="enAction">消息类型</param>
        public virtual void OnComplete(UdpClient client, EnSocketAction enAction)
        {
            Completed?.Invoke(client, enAction);
            if (enAction == EnSocketAction.Connect)//建立连接后，开始接收数据
            {
                ThreadPool.QueueUserWorkItem(x =>
                {
                    while (!isClose)
                    {
                        try
                        {
                            Thread.Sleep(20);
                            ReceiveAsync();
                            Thread.Sleep(20);
                        }
                        catch (Exception)
                        {
                            Close();
                            OnComplete(client, EnSocketAction.Close);
                        }
                    }
                });
            }
        }
        /// <summary>
        /// TCP关闭
        /// </summary>
        public void Close()
        {
            isClose = true;
        }
    }
}
